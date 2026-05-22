<?php
require_once 'conexion.php';
require_once 'seguridad_desktop.php'; 

header('Content-Type: application/vnd.ms-excel; charset=utf-8');

$token = $_POST['token'] ?? '';
$user_agent_hash = $_POST['user_agent_hash'] ?? '';
$nom_grup = $_POST['nom_grup'] ?? '';

$res_seguridad = verificar_y_rotar_token($conn, $token);
if (!$res_seguridad['valido']) {
    header('Content-Type: application/json; charset=utf-8');
    echo json_encode(["status" => "error", "motivo" => "Sesión inválida o expirada."]);
    exit;
}

if (empty($nom_grup)) {
    header('Content-Type: application/json; charset=utf-8');
    echo json_encode(["status" => "error", "motivo" => "No se ha seleccionado ningún grupo."]);
    exit;
}

$filename = "Alumnos_" . str_replace(' ', '_', $nom_grup) . ".xls";
header("Content-Disposition: attachment; filename=\"$filename\"");

$sql = "SELECT e.nia, p.nom, p.cognom, p.email, p.dni, p.edat 
        FROM estudiants_grupclasse eg
        INNER JOIN estudiants e ON eg.nia = e.nia
        INNER JOIN persona p ON e.dni_persona = p.dni
        WHERE eg.nom_grup = ? 
        ORDER BY p.cognom, p.nom";

$stmt = $conn->prepare($sql);
$stmt->bind_param("s", $nom_grup);
$stmt->execute();
$resultado = $stmt->get_result();

echo "<!DOCTYPE html>";
echo "<html>";
echo "<head><meta charset='utf-8'></head>"; 
echo "<body>";
echo "<table border='1'>"; 

echo "<tr style='background-color: #D3D3D3; font-weight: bold;'>";
echo "<td>NIA</td>";
echo "<td>DNI</td>";
echo "<td>Nombre</td>";
echo "<td>Apellidos</td>";
echo "<td>Email</td>";
echo "<td>Edad</td>";
echo "</tr>";

while ($fila = $resultado->fetch_assoc()) {
    echo "<tr>";
    echo "<td>" . htmlspecialchars($fila['nia']) . "</td>";
    echo "<td>" . htmlspecialchars($fila['dni']) . "</td>";
    echo "<td>" . htmlspecialchars($fila['nom']) . "</td>";
    echo "<td>" . htmlspecialchars($fila['cognom']) . "</td>";
    echo "<td>" . htmlspecialchars($fila['email']) . "</td>";
    echo "<td>" . htmlspecialchars($fila['edat']) . "</td>";
    echo "</tr>";
}

echo "</table>";
echo "</body>";
echo "</html>";

$stmt->close();
$conn->close();
?>