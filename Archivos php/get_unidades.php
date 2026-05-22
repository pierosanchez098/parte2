<?php
require_once 'conexion.php';
require_once 'seguridad_desktop.php'; 

header('Content-Type: application/json');

$token = $_POST['token'] ?? '';
$nom_grup = $_POST['nom_grup'] ?? '';

$res_seguridad = verificar_y_rotar_token($conn, $token);
if (!$res_seguridad['valido']) {
    echo json_encode(["status" => "error", "motivo" => "Sesión inválida."]);
    exit;
}
$new_token = $res_seguridad['new_token'];

$sql = "SELECT u.id, u.num, CONCAT(m.nom_complet, ' - ', u.tipus, ' ', u.nom) AS unidad
        FROM unitat u
        INNER JOIN modul m ON u.modul = m.nom
        INNER JOIN grupclasse_assignatura gca ON m.codi_assignatura = gca.codi_assignatura
        WHERE gca.nom_grupclasse = ?
        ORDER BY m.nom_complet, u.num";

$stmt = $conn->prepare($sql);
$stmt->bind_param("s", $nom_grup);
$stmt->execute();
$resultado = $stmt->get_result();

$unidades = [];
while ($fila = $resultado->fetch_assoc()) {
    $displayText = $fila['unidad'];
    if (!empty($fila['num'])) {
        $displayText .= " (RA " . $fila['num'] . ")";
    }

    $unidades[] = [
        "id" => $fila['id'],
        "unidad" => $displayText
    ];
}
$stmt->close();

echo json_encode([
    "status" => "success",
    "unidades" => $unidades,
    "new_token" => $new_token
]);
?>