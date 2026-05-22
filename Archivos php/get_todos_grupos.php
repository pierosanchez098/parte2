<?php
require_once 'conexion.php';
require_once 'seguridad_desktop.php'; 

header('Content-Type: application/json; charset=utf-8');

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    echo json_encode(["status" => "error", "motivo" => "Método no permitido"]);
    exit;
}

$token = $_POST['token'] ?? '';
$user_agent_hash = $_POST['user_agent_hash'] ?? '';

$res_seguridad = verificar_y_rotar_token($conn, $token);
if (!$res_seguridad['valido']) {
    echo json_encode([
        "status" => "error",
        "motivo" => $res_seguridad['motivo'] ?? "Sesión inválida o expirada."
    ]);
    exit;
}
$new_token = $res_seguridad['new_token'];

$sql = "SELECT nom, aula FROM grup_classe ORDER BY nom";
$result = $conn->query($sql);

$grupos = [];
while ($fila = $result->fetch_assoc()) {
    $grupos[] = [
        "nom" => $fila['nom'],
        "aula" => $fila['aula']
    ];
}

$result->close();
$conn->close();

echo json_encode([
    "status" => "success",
    "grupos" => $grupos,
    "new_token" => $new_token
]);
?>