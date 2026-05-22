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
        "motivo" => "Sesión inválida o expirada."
    ]);
    exit;
}
$new_token = $res_seguridad['new_token'];

$sql = "SELECT login_timestamp, username, ip_direccio, login_success 
        FROM login_logs 
        ORDER BY login_timestamp DESC";

$result = $conn->query($sql);

$xml = new SimpleXMLElement('<?xml version="1.0" encoding="UTF-8"?><RegistrosLogin></RegistrosLogin>');

while ($row = $result->fetch_assoc()) {
    $loginNode = $xml->addChild('Login');
    $loginNode->addChild('Fecha', $row['login_timestamp']);
    $loginNode->addChild('Usuario', htmlspecialchars($row['username']));
    $loginNode->addChild('IP', $row['ip_direccio']);
    
    $exitoTexto = (intval($row['login_success']) === 1) ? 'Sí' : 'No';
    $loginNode->addChild('Exito', $exitoTexto);
}

$result->close();
$conn->close();

$dom = dom_import_simplexml($xml)->ownerDocument;
$dom->formatOutput = true;
$xml_formateado = $dom->saveXML();

echo json_encode([
    "status" => "success",
    "xml_data" => $xml_formateado,
    "new_token" => $new_token
]);
?>