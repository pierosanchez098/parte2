<?php
require_once 'conexion.php';
require_once 'seguridad_desktop.php'; 

header('Content-Type: application/json');

$token = $_POST['token'] ?? '';
$user_agent_hash = $_POST['user_agent_hash'] ?? '';
$dni_profesor = $_POST['dni_profesor'] ?? '';

$res_seguridad = verificar_y_rotar_token($conn, $token);

if (!$res_seguridad['valido']) {
    echo json_encode([
        "status" => "error",
        "motivo" => $res_seguridad['motivo'] ?? "Sesión inválida o expirada."
    ]);
    exit;
}

$new_token = $res_seguridad['new_token'];

$stmt_rol = $conn->prepare("SELECT 1 FROM professor WHERE dni_persona = ?");
$stmt_rol->bind_param("s", $dni_profesor);
$stmt_rol->execute();
if (!$stmt_rol->get_result()->fetch_assoc()) {
    echo json_encode([
        "status" => "error",
        "motivo" => "Acceso denegado: No tienes rol de profesor.",
        "new_token" => $new_token
    ]);
    exit;
}
$stmt_rol->close();

$sql = "SELECT DISTINCT gc.nom, gc.aula 
        FROM professor_grup_classe pgc
        INNER JOIN grup_classe gc ON pgc.nom_grup = gc.nom
        WHERE pgc.dni_persona = ?
        ORDER BY gc.nom";

$stmt = $conn->prepare($sql);
$stmt->bind_param("s", $dni_profesor);
$stmt->execute();
$resultado = $stmt->get_result();

$grupos = [];
while ($fila = $resultado->fetch_assoc()) {
    $grupos[] = [
        "nom" => $fila['nom'],
        "aula" => $fila['aula']
    ];
}

$stmt->close();

echo json_encode([
    "status" => "success",
    "grupos" => $grupos,
    "new_token" => $new_token
]);
?>