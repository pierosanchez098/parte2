<?php
require_once 'conexion.php';
require_once 'seguridad_desktop.php'; 

header('Content-Type: application/json');

$token = $_POST['token'] ?? '';
$user_agent_hash = $_POST['user_agent_hash'] ?? '';
$nom_grup = $_POST['nom_grup'] ?? '';

$res_seguridad = verificar_y_rotar_token($conn, $token);

if (!$res_seguridad['valido']) {
    echo json_encode([
        "status" => "error",
        "motivo" => $res_seguridad['motivo'] ?? "Sesión inválida o expirada."
    ]);
    exit;
}

$new_token = $res_seguridad['new_token'];

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

$alumnos = [];
while ($fila = $resultado->fetch_assoc()) {
    $alumnos[] = [
        "nia" => $fila['nia'],
        "nom" => $fila['nom'],
        "cognom" => $fila['cognom'],
        "email" => $fila['email'],
        "dni" => $fila['dni']
    ];
}

$stmt->close();

echo json_encode([
    "status" => "success",
    "alumnos" => $alumnos,
    "new_token" => $new_token
]);
?>