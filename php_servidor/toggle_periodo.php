<?php
header('Content-Type: application/json; charset=utf-8');

include 'seguridad.php';

$conn = new mysqli("localhost", "root", "", "plataforma_evalis");

$token = $_POST['token'] ?? '';
$abrir  = filter_var($_POST['abrir'] ?? '', FILTER_VALIDATE_BOOLEAN);

$resultado = verificar_y_rotar_token($conn, $token);

if (!$resultado['valido']) {
    echo json_encode(["error" => "Sesión inválida", "expired" => true]);
    exit;
}

$nuevoEstado = $abrir ? 1 : 0;

$sql = "UPDATE estado_evaluacion SET periodo_abierto = ? WHERE id = 1";
$stmt = $conn->prepare($sql);
$stmt->bind_param("i", $nuevoEstado);
$stmt->execute();

echo json_encode([
    "success" => true,
    "periodo_abierto" => $abrir,
    "new_token" => $resultado['new_token']
]);

$conn->close();
?>