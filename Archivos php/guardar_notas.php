<?php
require_once 'conexion.php'; 
require_once 'seguridad_desktop.php'; 

header('Content-Type: application/json');

$token = $_POST['token'] ?? '';
$user_agent_hash = $_POST['user_agent_hash'] ?? '';
$dni_profesor = $_POST['dni_profesor'] ?? '';
$uf_id = $_POST['uf_id'] ?? '';
$notas_json = $_POST['notas'] ?? ''; 

$res_seguridad = verificar_y_rotar_token($conn, $token);

if (!$res_seguridad['valido']) {
    echo json_encode([
        "status" => "error",
        "motivo" => $res_seguridad['motivo'] ?? "Sesión inválida."
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
        "motivo" => "Acceso denegado: No tienes permisos de profesor.",
        "new_token" => $new_token
    ]);
    exit;
}
$stmt_rol->close();

if (!empty($uf_id) && !empty($notas_json)) {
    $lista_notas = json_decode($notas_json, true);
    
    if (is_array($lista_notas)) {
        $stmt_upsert = $conn->prepare("
            INSERT INTO notes (nia, uf_id, nota, data_nota) 
            VALUES (?, ?, ?, NOW()) 
            ON DUPLICATE KEY UPDATE nota = ?, data_nota = NOW()
        ");

        foreach ($lista_notas as $item) {
            $nia = intval($item['nia']);
            $nota = floatval($item['nota']);

            if ($nota >= 0 && $nota <= 10) {
                $stmt_upsert->bind_param("iidd", $nia, $uf_id, $nota, $nota);
                $stmt_upsert->execute();
            }
        }
        $stmt_upsert->close();
    }
}

echo json_encode([
    "status" => "success",
    "motivo" => "Notas actualizadas correctamente.",
    "new_token" => $new_token
]);
?>