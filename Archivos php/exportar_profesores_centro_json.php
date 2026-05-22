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
$id_centro = $_POST['id_centro'] ?? '';

$res_seguridad = verificar_y_rotar_token($conn, $token);
if (!$res_seguridad['valido']) {
    echo json_encode([
        "status" => "error",
        "motivo" => "Sesión inválida o expirada."
    ]);
    exit;
}
$new_token = $res_seguridad['new_token'];

if (empty($id_centro)) {
    echo json_encode([
        "status" => "error",
        "motivo" => "No se ha especificado el centro educativo.",
        "new_token" => $new_token
    ]);
    exit;
}

$sql = "SELECT nom, cognom, data_naix, poblacio, codi_postal, tel_mobil, tel_fix, email, edat 
        FROM persona 
        WHERE id_centre = ? AND (rol = 'professor' OR rol = 'professor_directiu')
        ORDER BY cognom ASC, nom ASC";

$stmt = $conn->prepare($sql);
$stmt->bind_param("i", $id_centro);
$stmt->execute();
$result = $stmt->get_result();

$lista = [];
while ($row = $result->fetch_assoc()) {
    $lista[] = [
        "nombre" => $row['nom'],
        "apellidos" => $row['cognom'],
        "fecha_nacimiento" => $row['data_naix'],
        "poblacion" => $row['poblacio'],
        "codigo_postal" => $row['codi_postal'],
        "telefono_movil" => $row['tel_mobil'],
        "telefono_fijo" => $row['tel_fix'],
        "email" => $row['email'],
        "edad" => (int)$row['edat']
    ];
}

$stmt->close();
$conn->close();

echo json_encode([
    "status" => "success",
    "lista" => $lista,
    "new_token" => $new_token
]);
?>