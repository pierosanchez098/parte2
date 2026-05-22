<?php
header('Content-Type: application/json; charset=utf-8');

error_reporting(E_ALL);
ini_set('display_errors', 1);

require_once 'conexion.php';
require_once 'seguridad_desktop.php';

$token = $_POST['token'] ?? '';
$user_agent_hash = $_POST['user_agent_hash'] ?? '';

$resultado = verificar_y_rotar_token($conn, $token);
if (!$resultado['valido']) {
    echo json_encode([
        "status" => "error", 
        "motivo" => $resultado['motivo'] ?? "Sesión inválida o expirada"
    ]);
    exit;
}

$new_token = $resultado['new_token'];
$username = $resultado['username'];

$stmt_user = $conn->prepare("
    SELECT u.dni_persona, p.rol 
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

if ($rol !== 'admin' && $rol !== 'administrador') {
    echo json_encode([
        "status" => "error", 
        "motivo" => "Acceso denegado: Sin permisos de admin.",
        "new_token" => $new_token
    ]);
    exit;
}

$sql = "SELECT c.id_centre, c.nom 
        FROM centre_administradors ca
        INNER JOIN centre c ON ca.id_centre = c.id_centre
        WHERE ca.dni_administrador = ?";

$stmt = $conn->prepare($sql);
$stmt->bind_param("s", $dni_usuario);
$stmt->execute();
$res = $stmt->get_result();

$centros = [];
while ($fila = $res->fetch_assoc()) {
    $centros[] = [
        "id_centre" => $fila['id_centre'],
        "nom" => $fila['nom']
    ];
}
$stmt->close();

echo json_encode([
    "status" => "success",
    "centros" => $centros,
    "new_token" => $new_token
]);

$conn->close();
?>