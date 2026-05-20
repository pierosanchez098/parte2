<?php
header('Content-Type: application/json; charset=utf-8');

include 'seguridad.php';

$conn = new mysqli("localhost", "root", "", "plataforma_evalis");

$token = $_POST['token'] ?? '';

$resultado = verificar_y_rotar_token($conn, $token);

if (!$resultado['valido']) {
    echo json_encode(["error" => "Sesión inválida", "expired" => true]);
    exit;
}

$sql = "SELECT periodo_abierto FROM estado_evaluacion WHERE id = 1";
$result = $conn->query($sql);
$row = $result->fetch_assoc();

$abierto = $row ? (bool)$row['periodo_abierto'] : false;

echo json_encode([
    "error" => null,
    "periodo_abierto" => $abierto,
    "new_token" => $resultado['new_token']
]);

$conn->close();
?>