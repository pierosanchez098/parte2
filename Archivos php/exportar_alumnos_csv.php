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
$nom_grup = $_POST['nom_grup'] ?? '';

$res_seguridad = verificar_y_rotar_token($conn, $token);
if (!$res_seguridad['valido']) {
    echo json_encode([
        "status" => "error",
        "motivo" => "Sesión inválida o expirada."
    ]);
    exit;
}
$new_token = $res_seguridad['new_token'];

if (empty($nom_grup)) {
    echo json_encode([
        "status" => "error",
        "motivo" => "No se ha seleccionado ningún grupo.",
        "new_token" => $new_token
    ]);
    exit;
}

$sql = "SELECT e.nia, p.nom, p.cognom, p.email, p.dni 
        FROM estudiants_grupclasse eg
        INNER JOIN estudiants e ON eg.nia = e.nia
        INNER JOIN persona p ON e.dni_persona = p.dni
        WHERE eg.nom_grup = ? 
        ORDER BY p.cognom, p.nom";

$stmt = $conn->prepare($sql);
$stmt->bind_param("s", $nom_grup);
$stmt->execute();
$resultado = $stmt->get_result();

$lineas_csv = [];

$lineas_csv[] = '"NIA";"Nombre";"Apellidos";"Email";"DNI"';

while ($fila = $resultado->fetch_assoc()) {
    $nia   = str_replace('"', '""', str_replace(';', ',', $fila['nia'] ?? ''));
    $nom   = str_replace('"', '""', str_replace(';', ',', $fila['nom'] ?? ''));
    $cognom = str_replace('"', '""', str_replace(';', ',', $fila['cognom'] ?? ''));
    $email = str_replace('"', '""', str_replace(';', ',', $fila['email'] ?? ''));
    $dni   = str_replace('"', '""', str_replace(';', ',', $fila['dni'] ?? ''));

    $lineas_csv[] = '"' . $nia . '";"' . $nom . '";"' . $cognom . '";"' . $email . '";"' . $dni . '"';
}

$stmt->close();
$conn->close();

echo json_encode([
    "status" => "success",
    "csv_data" => implode("\r\n", $lineas_csv),
    "new_token" => $new_token
]);
?>