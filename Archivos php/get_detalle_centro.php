<?php
require_once 'conexion.php';
require_once 'seguridad_desktop.php';

header('Content-Type: application/json; charset=utf-8');

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    echo json_encode(["status" => "error", "motivo" => "Método no permitido"]);
    exit;
}

$token = $_POST['token'] ?? '';
$id_centre = $_POST['id_centre'] ?? '';

$res_seguridad = verificar_y_rotar_token($conn, $token);
if (!$res_seguridad['valido']) {
    echo json_encode(["status" => "error", "motivo" => "Sesión inválida o expirada."]);
    exit;
}
$new_token = $res_seguridad['new_token'];

if (empty($id_centre)) {
    echo json_encode(["status" => "error", "motivo" => "ID de centro no proporcionado.", "new_token" => $new_token]);
    exit;
}

$sqlCentro = "SELECT nom, direccio, pla, logo FROM centre WHERE id_centre = ?";
$stmt = $conn->prepare($sqlCentro);
$stmt->bind_param("i", $id_centre);
$stmt->execute();
$resCentro = $stmt->get_result()->fetch_assoc();
$stmt->close();

if (!$resCentro) {
    echo json_encode(["status" => "error", "motivo" => "Centro no encontrado.", "new_token" => $new_token]);
    exit;
}

$sqlEstudios = "SELECT nom_estudio, curso FROM estudios_centre WHERE id_centre = ? ORDER BY nom_estudio ASC";
$stmtEst = $conn->prepare($sqlEstudios);
$stmtEst->bind_param("i", $id_centre);
$stmtEst->execute();
$resEstudios = $stmtEst->get_result();

$estudios = [];
while ($row = $resEstudios->fetch_assoc()) {
    $estudios[] = [
        "nom_estudio" => $row['nom_estudio'],
        "curso" => $row['curso']
    ];
}
$stmtEst->close();
$conn->close();

echo json_encode([
    "status" => "success",
    "info_centro" => [
        "nom" => $resCentro['nom'],
        "direccio" => $resCentro['direccio'],
        "pla" => $resCentro['pla'],
        "logo" => $resCentro['logo'] ?? ''
    ],
    "estudios" => $estudios,
    "new_token" => $new_token
]);
?>