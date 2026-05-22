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
    SELECT u.dni_persona, p.rol, p.nom, p.cognom, p.email
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
$nombre_alumno = trim(($user_data['nom'] ?? '') . ' ' . ($user_data['cognom'] ?? ''));
$email_alumno = $user_data['email'] ?? '';

if ($rol !== 'estudiant' && $rol !== 'alumne') {
    echo json_encode([
        "status" => "error", 
        "motivo" => "Acceso denegado: Este endpoint es de uso exclusivo para estudiantes.",
        "new_token" => $new_token
    ]);
    exit;
}


$sql = "
    SELECT 
        e.nom_estudi,
        e.curs_inici,
        e.curs_fi,
        e.status,
        e.nota_final,
        u.foto_carnet
    FROM estudis e
    INNER JOIN estudiants est ON e.nia = est.nia
    INNER JOIN usuari u ON est.dni_persona = u.dni_persona
    WHERE est.dni_persona = ?
    ORDER BY e.curs_inici DESC, e.nom_estudi
";

$stmt = $conn->prepare($sql);
$stmt->bind_param("s", $dni_usuario);
$stmt->execute();
$res = $stmt->get_result();

$estudis = [];
$foto_carnet = null;

while ($row = $res->fetch_assoc()) {
    if ($foto_carnet === null) {
        $foto_carnet = $row['foto_carnet'];  
    }
    $estudis[] = [
        "nom_estudi" => $row['nom_estudi'] ?? '',
        "curs_inici" => $row['curs_inici'] ?? '',
        "curs_fi"    => $row['curs_fi'] ?? '',
        "status"     => $row['status'] ?? 'Pendiente',
        "nota_final" => $row['nota_final'] ?: "Pendiente"
    ];
}
$stmt->close();

echo json_encode([
    "status" => "success",
    "nombre_alumno" => $nombre_alumno,
    "email_alumno" => $email_alumno,
    "dni_alumno" => $dni_usuario,
    "estudis" => $estudis,
    "foto_carnet" => $foto_carnet,
    "new_token" => $new_token
]);

$conn->close();
?>