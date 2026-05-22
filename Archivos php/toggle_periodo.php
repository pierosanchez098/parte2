<?php
header('Content-Type: application/json; charset=utf-8');

error_reporting(0);
ini_set('display_errors', 0);

require_once 'conexion.php';
require_once 'seguridad_desktop.php';

$token = $_POST['token'] ?? '';
$user_agent_hash = $_POST['user_agent_hash'] ?? '';
$abrir_param = $_POST['abrir'] ?? 'false';
$abrir = ($abrir_param === 'true' || $abrir_param === '1');

$resultado = verificar_y_rotar_token($conn, $token);

if (!$resultado['valido']) {
    echo json_encode([
        "status" => "error",
        "motivo" => $resultado['motivo'] ?? "Sesión expirada o inválida."
    ]);
    exit;
}

$new_token = $resultado['new_token'];
$username = $resultado['username'];

$stmt_rol = $conn->prepare("
    SELECT p.rol 
    FROM usuari u 
    INNER JOIN persona p ON u.dni_persona = p.dni 
    WHERE u.username = ?
");
$stmt_rol->bind_param("s", $username);
$stmt_rol->execute();
$user_data = $stmt_rol->get_result()->fetch_assoc();
$stmt_rol->close();

$rol = isset($user_data['rol']) ? strtolower(trim($user_data['rol'])) : '';

if ($rol !== 'admin' && $rol !== 'directiu' && $rol !== 'jefe_estudios') {
    echo json_encode([
        "status" => "error",
        "motivo" => "Acceso denegado: No tienes permisos para modificar el periodo de evaluación.",
        "new_token" => $new_token
    ]);
    exit;
}

$nuevoEstado = $abrir ? 1 : 0;
$textoAccion = $abrir ? "abierto" : "cerrado";

$sql = "UPDATE estado_evaluacion SET periodo_abierto = ? WHERE id = 1";
$stmt = $conn->prepare($sql);
$stmt->bind_param("i", $nuevoEstado);
$stmt->execute();
$stmt->close();

echo json_encode([
    "status" => "success",
    "periodo_abierto" => $abrir,
    "motivo" => "El periodo de evaluación se ha " . $textoAccion . " correctamente.",
    "new_token" => $new_token
]);

$conn->close();
?>