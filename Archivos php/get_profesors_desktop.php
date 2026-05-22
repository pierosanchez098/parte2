<?php
ob_clean(); 
error_reporting(E_ALL);
ini_set('display_errors', 1);
ini_set('log_errors', 1);

header('Content-Type: application/json; charset=utf-8');

require_once 'conexion.php';
require_once 'seguridad_desktop.php';

$token = $_POST['token'] ?? '';
$user_agent_hash = $_POST['user_agent_hash'] ?? '';
$id_centre = $_POST['id_centre'] ?? ''; 

$resultado = verificar_y_rotar_token($conn, $token);
if (!$resultado['valido']) {
    echo json_encode([
        "status" => "error", 
        "motivo" => $resultado['motivo'] ?? "Sesión inválida o expirada."
    ]);
    exit;
}

$new_token = $resultado['new_token'];
$username = $resultado['username'];

$stmt_user = $conn->prepare("
    SELECT u.dni_persona, p.rol, p.id_centre
    FROM usuari u 
    INNER JOIN persona p ON u.dni_persona = p.dni 
    WHERE u.username = ?
");
$stmt_user->bind_param("s", $username);
$stmt_user->execute();
$user_data = $stmt_user->get_result()->fetch_assoc();
$stmt_user->close();

$dni_usuario = $user_data['dni_persona'] ?? '';
$rol = isset($user_data['rol']) ? strtolower(trim($user_data['rol'])) : '';
$id_centre_usuario = $user_data['id_centre'] ?? '';

$profesores = [];

if ($rol === 'estudiant' || $rol === 'alumne') {
    
    $sql = "
        SELECT
            pers.nom,
            pers.cognom,
            pers.email,
            pers.foto,
            GROUP_CONCAT(assign.nom SEPARATOR ', ') AS assignatures_impartides
        FROM estudiants est
        INNER JOIN persona alum ON est.dni_persona = alum.dni
        INNER JOIN matricula mat ON est.nia = mat.nia
        INNER JOIN assignatura assign ON mat.codi_assignatura = assign.codi
        INNER JOIN assignatura_professor ap ON assign.codi = ap.codi_assig
        INNER JOIN professor prof ON ap.id_intern_prof = prof.id_intern
        INNER JOIN persona pers ON prof.dni_persona = pers.dni
        WHERE alum.dni = ?
        GROUP BY prof.id_intern
        ORDER BY pers.nom, pers.cognom
    ";

    $stmt = $conn->prepare($sql);
    $stmt->bind_param("s", $dni_usuario);
    $stmt->execute();
    $res = $stmt->get_result();

    while ($fila = $res->fetch_assoc()) {
        $profesores[] = [
            "nombre_completo" => trim($fila['nom'] . ' ' . $fila['cognom']),
            "email" => $fila['email'] ?? '',
            "carrec" => "Prof. de: " . ($fila['assignatures_impartides'] ?? 'Docente'),
            "departament" => 'Mis Asignaturas',
            "foto" => $fila['foto'] ?? ''
        ];
    }
    $stmt->close();

} else {
    if ($id_centre === 'auto') {
        if ($rol === 'admin' || $rol === 'administrador') {
            echo json_encode([
                "status" => "error", 
                "motivo" => "Un administrador solo puede especificar la id del centro a ver.", 
                "new_token" => $new_token
            ]);
            exit;
        }

        $id_centre_final = $id_centre_usuario;

        if (empty($id_centre_final)) {
            echo json_encode([
                "status" => "error", 
                "motivo" => "No estás asignado a ningún centro.", 
                "new_token" => $new_token
            ]);
            exit;
        }
    } else {
        if ($rol !== 'admin' && $rol !== 'administrador') {
            echo json_encode([
                "status" => "error", 
                "motivo" => "Acceso denegado: No tienes privilegios de administración global.", 
                "new_token" => $new_token
            ]);
            exit;
        }

        $stmt_permiso = $conn->prepare("SELECT 1 FROM centre_administradors WHERE dni_administrador = ? AND id_centre = ?");
        $stmt_permiso->bind_param("ss", $dni_usuario, $id_centre);
        $stmt_permiso->execute();
        $tiene_permiso = $stmt_permiso->get_result()->fetch_assoc();
        $stmt_permiso->close();

        if (!$tiene_permiso) {
            echo json_encode([
                "status" => "error", 
                "motivo" => "Acceso denegado: Tu usuario administrador no está asignado a este centro.", 
                "new_token" => $new_token
            ]);
            exit;
        }

        $id_centre_final = $id_centre;
    }

    $sql = "SELECT 
                CONCAT(p.nom, ' ', p.cognom) AS nombre_completo,
                p.email,
                IF(p.rol = 'directiu', 'Cap d\'estudis', IFNULL(pr.especialitzacio, 'Docente')) AS carrec,
                'General' AS departament, 
                p.foto
            FROM persona p
            LEFT JOIN professor pr ON p.dni = pr.dni_persona
            WHERE p.id_centre = ? AND p.rol IN ('professor', 'directiu', 'professor_directiu')
            ORDER BY p.rol DESC, p.nom, p.cognom";

    $stmt = $conn->prepare($sql);
    $stmt->bind_param("s", $id_centre_final);
    $stmt->execute();
    $res = $stmt->get_result();

    while ($fila = $res->fetch_assoc()) {
        $profesores[] = [
            "nombre_completo" => $fila['nombre_completo'] ?? '',
            "email" => $fila['email'] ?? '',
            "carrec" => $fila['carrec'] ?? 'Docente',
            "departament" => $fila['departament'] ?? 'General',
            "foto" => $fila['foto'] ?? ''
        ];
    }
    $stmt->close();
}

echo json_encode([
    "status" => "success",
    "profesores" => $profesores,
    "new_token" => $new_token
]);

$conn->close();
?>