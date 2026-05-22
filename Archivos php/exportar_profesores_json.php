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

$res_seguridad = verificar_y_rotar_token($conn, $token);
if (!$res_seguridad['valido']) {
    echo json_encode(["status" => "error", "motivo" => "Sesión inválida o expirada."]);
    exit;
}
$new_token = $res_seguridad['new_token'];

$sql = "SELECT p.dni, p.nom, p.cognom, p.data_naix, p.email, p.tel_mobil, p.tel_fix, p.foto, pr.especialitzacio, pr.id_intern 
        FROM persona p 
        INNER JOIN professor pr ON p.dni = pr.dni_persona 
        ORDER BY p.cognom, p.nom";

$result = $conn->query($sql);
$lista_profesores = [];

while ($row = $result->fetch_assoc()) {
    $lista_profesores[] = [
        "dni" => $row['dni'],
        "nombre" => $row['nom'],
        "apellidos" => $row['cognom'],
        "fechaNacimiento" => $row['data_naix'],
        "email" => $row['email'],
        "telefonoMovil" => $row['tel_mobil'],
        "telefonoFijo" => $row['tel_fix'],
        "especializacion" => $row['especialitzacio'],
        "id_intern" => $row['id_intern'], 
        "foto" => $row['foto'] ? $row['foto'] : null
    ];
}

$conn->close();

echo json_encode([
    "status" => "success",
    "lista" => $lista_profesores,
    "new_token" => $new_token
]);
?>