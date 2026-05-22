<?php
header('Content-Type: application/json; charset=utf-8');

error_reporting(0);
ini_set('display_errors', 0);

require_once 'conexion.php';
require_once 'seguridad_desktop.php';

$token = $_POST['token'] ?? '';
$user_agent_hash = $_POST['user_agent_hash'] ?? '';

$resultado = verificar_y_rotar_token($conn, $token);

if (!$resultado['valido']) {
    echo json_encode([
        "status" => "error",
        "motivo" => $resultado['motivo'] ?? "Sesión inválida."
    ]);
    exit;
}

$new_token = $resultado['new_token'];

$res = $conn->query("SELECT periodo_abierto FROM estado_evaluacion WHERE id = 1");
$fila = $res->fetch_assoc();

$abierto = false;
if ($fila) {
    $abierto = (intval($fila['periodo_abierto']) === 1);
}

echo json_encode([
    "status" => "success",
    "periodo_abierto" => $abierto,
    "new_token" => $new_token
]);

$conn->close();
?>