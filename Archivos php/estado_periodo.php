<?php
header('Content-Type: application/json; charset=utf-8');

include 'seguridad.php';

$conn = new mysqli("localhost", "root", "", "plataforma_evalis");

$token = $_POST['token'] ?? '';
$user_agent_hash_recibido = $_POST['user_agent_hash'] ?? '';

$resultado = verificar_y_rotar_token($conn, $token);

if (!$resultado['valido']) {
    echo json_encode([
        "error" => $resultado['motivo'] ?? "Sesión expirada o inválida", 
        "periodo_abierto" => false,
        "expired" => true
    ]);
    if ($conn) {
        $conn->close();
    }
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