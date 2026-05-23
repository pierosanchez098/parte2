<?php
require_once 'conexion.php';
require_once 'seguridad_desktop.php'; 

header('Content-Type: application/json');

$token = $_POST['token'] ?? '';
$user_agent_hash = $_POST['user_agent_hash'] ?? '';
$nia = $_POST['nia'] ?? ''; 

$res_seguridad = verificar_y_rotar_token($conn, $token);

if (!$res_seguridad['valido']) {
    echo json_encode([
        "status" => "error",
        "motivo" => $res_seguridad['motivo'] ?? "Sesión inválida o expirada."
    ]);
    exit;
}

$new_token = $res_seguridad['new_token'];

if (empty($nia)) {
    echo json_encode([
        "status" => "error",
        "motivo" => "Falta el parámetro NIA del alumno.",
        "new_token" => $new_token
    ]);
    exit;
}

$sql_alumno = "SELECT p.nom, p.cognom, p.dni, p.email, c.nom AS centro_nom, c.logo AS centro_logo
               FROM estudiants e 
               INNER JOIN persona p ON e.dni_persona = p.dni 
               LEFT JOIN centre c ON p.id_centre = c.id_centre
               WHERE e.nia = ?";

$stmt_A = $conn->prepare($sql_alumno);
$stmt_A->bind_param("s", $nia);
$stmt_A->execute();
$resultado_A = $stmt_A->get_result();
$alumnoData = $resultado_A->fetch_assoc();
$stmt_A->close();

if (!$alumnoData) {
    echo json_encode([
        "status" => "error",
        "motivo" => "Alumno no encontrado en la base de datos.",
        "new_token" => $new_token
    ]);
    exit;
}

$sql_estudios = "SELECT nom_estudi, curs_inici, curs_fi, status, nota_final 
                 FROM estudis 
                 WHERE nia = ?";

$stmt_E = $conn->prepare($sql_estudios);
$stmt_E->bind_param("s", $nia);
$stmt_E->execute();
$resultado_E = $stmt_E->get_result();

$estudios = [];
while ($fila = $resultado_E->fetch_assoc()) {
    $estudios[] = [
        "nom_estudi" => $fila['nom_estudi'],
        "curs_inici" => $fila['curs_inici'],
        "curs_fi" => $fila['curs_fi'],
        "status" => $fila['status'],
        "nota_final" => $fila['nota_final']
    ];
}
$stmt_E->close();

echo json_encode([
    "status" => "success",
    "nombre_alumno" => $alumnoData['nom'] . " " . $alumnoData['cognom'],
    "dni_alumno" => $alumnoData['dni'],
    "email_alumno" => $alumnoData['email'],
    "centro_educativo" => $alumnoData['centro_nom'] ?? "Centro no asignado",
    "logo_centro" => $alumnoData['centro_logo'],
    "estudis" => $estudios,
    "new_token" => $new_token
]);
?>