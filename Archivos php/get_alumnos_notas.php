<?php
require_once 'conexion.php';
require_once 'seguridad_desktop.php'; 

header('Content-Type: application/json');

$token = $_POST['token'] ?? '';
$uf_id = $_POST['uf_id'] ?? '';
$nom_grup = $_POST['nom_grup'] ?? '';

$res_seguridad = verificar_y_rotar_token($conn, $token);
if (!$res_seguridad['valido']) {
    echo json_encode(["status" => "error", "motivo" => "Sesión inválida."]);
    exit;
}
$new_token = $res_seguridad['new_token'];

$sql = "SELECT e.nia, CONCAT(p.nom, ' ', p.cognom) AS alumno, n.nota
        FROM estudiants_grupclasse eg
        INNER JOIN estudiants e ON eg.nia = e.nia
        INNER JOIN persona p ON e.dni_persona = p.dni
        LEFT JOIN notes n ON e.nia = n.nia AND n.uf_id = ?
        WHERE eg.nom_grup = ?
        ORDER BY p.nom, p.cognom";

$stmt = $conn->prepare($sql);
$stmt->bind_param("is", $uf_id, $nom_grup);
$stmt->execute();
$resultado = $stmt->get_result();

$alumnos = [];
while ($fila = $resultado->fetch_assoc()) {
    $alumnos[] = [
        "nia" => $fila['nia'],
        "alumno" => $fila['alumno'],
        "nota" => $fila['nota'] 
    ];
}
$stmt->close();

echo json_encode([
    "status" => "success",
    "alumnos" => $alumnos,
    "new_token" => $new_token
]);
?>