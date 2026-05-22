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
$id_centre = $_POST['id_centre'] ?? ''; 
$estudios_json = $_POST['estudios'] ?? '[]';

$nom_centro = $_POST['nom_centro'] ?? '';
$direccio_centro = $_POST['direccio_centro'] ?? '';
$pla_centro = $_POST['pla_centro'] ?? '';

$res_seguridad = verificar_y_rotar_token($conn, $token);
if (!$res_seguridad['valido']) {
    echo json_encode(["status" => "error", "motivo" => "Sesión inválida o expirada."]);
    exit;
}
$new_token = $res_seguridad['new_token'];

if (empty($id_centre)) {
    echo json_encode(["status" => "error", "motivo" => "No se ha seleccionado ningún centro para modificar.", "new_token" => $new_token]);
    exit;
}

if (!empty($nom_centro) && !empty($direccio_centro) && !empty($pla_centro)) {
    $stmtUpdate = $conn->prepare("UPDATE centre SET nom = ?, direccio = ?, pla = ? WHERE id_centre = ?");
    $stmtUpdate->bind_param("sssi", $nom_centro, $direccio_centro, $pla_centro, $id_centre);
    $stmtUpdate->execute();
    $stmtUpdate->close();
}


if (isset($_FILES['logo_file']) && $_FILES['logo_file']['error'] === UPLOAD_ERR_OK) {
    $fileTmpPath = $_FILES['logo_file']['tmp_name'];
    $fileName = $_FILES['logo_file']['name'];
    $fileExtension = strtolower(pathinfo($fileName, PATHINFO_EXTENSION));
    
    $extensions_allowed = ['jpg', 'jpeg', 'png', 'gif'];
    if (in_array($fileExtension, $extensions_allowed)) {
        $dest_dir = './uploads/logos/';
        if (!is_dir($dest_dir)) {
            mkdir($dest_dir, 0755, true);
        }
        
        $newFileName = 'logo_centro_' . $id_centre . '.' . $fileExtension;
        $dest_path = $dest_dir . $newFileName;
        
        if (move_uploaded_file($fileTmpPath, $dest_path)) {
            $stmtLogo = $conn->prepare("UPDATE centre SET logo = ? WHERE id_centre = ?");
            $stmtLogo->bind_param("si", $dest_path, $id_centre);
            $stmtLogo->execute();
            $stmtLogo->close();
        }
    }
}


$lista_estudios = json_decode($estudios_json, true);

if (is_array($lista_estudios)) {
    $conn->begin_transaction();
    try {
        $stmtDel = $conn->prepare("DELETE FROM estudios_centre WHERE id_centre = ?");
        $stmtDel->bind_param("i", $id_centre);
        $stmtDel->execute();
        $stmtDel->close();

        $stmtIns = $conn->prepare("INSERT INTO estudios_centre (id_centre, nom_estudio, curso) VALUES (?, ?, ?)");
        $curso_actual = '2025-2026';

        foreach ($lista_estudios as $nom_ciclo) {
            $nombre_limpio = trim($nom_ciclo);
            if (!empty($nombre_limpio)) {
                $stmtIns->bind_param("iss", $id_centre, $nombre_limpio, $curso_actual);
                $stmtIns->execute();
            }
        }
        $stmtIns->close();
        $conn->commit();
    } catch (Exception $e) {
        $conn->rollback();
        echo json_encode(["status" => "error", "motivo" => "Error al actualizar estudios: " . $e->getMessage(), "new_token" => $new_token]);
        exit;
    }
}

echo json_encode([
    "status" => "success",
    "motivo" => "¡Centro modificado con éxito a través del archivo de Excel!",
    "new_token" => $new_token
]);

$conn->close();
?>