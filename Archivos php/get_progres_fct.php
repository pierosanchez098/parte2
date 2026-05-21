<?php
header('Content-Type: application/json; charset=utf-8');
header('Access-Control-Allow-Origin: *');

include 'seguridad.php';

$conn = new mysqli("localhost", "root", "", "plataforma_evalis");

if ($conn->connect_error) {
    echo json_encode(["error" => "Error de conexión", "fct" => null, "expired" => false]);
    exit;
}

$token = $_POST['token'] ?? $_GET['token'] ?? '';
$dni_persona = $_POST['dni_persona'] ?? $_GET['dni_persona'] ?? '';
$user_agent_hash_recibido = $_POST['user_agent_hash'] ?? $_GET['user_agent_hash'] ?? '';

if (empty($token) || empty($dni_persona)) {
    echo json_encode(["error" => "Faltan token o dni_persona", "fct" => null, "expired" => true]);
    exit;
}

$resultado = verificar_y_rotar_token($conn, $token);

if (!$resultado['valido']) {
    echo json_encode([
        "error" => $resultado['motivo'] ?? "Sesión expirada o inválida", 
        "fct" => null,
        "expired" => true
    ]);
    $conn->close();
    exit;
}

$sql = "
    SELECT 
        f.nom_empresa,
        f.hores_total,
        f.hores_fetes,
        f.estudi
    FROM persona p
    INNER JOIN estudiants e ON p.dni = e.dni_persona
    INNER JOIN empresa_fct_practiques f ON e.nia = f.nia
    WHERE p.dni = ?
    LIMIT 1
";

$stmt = $conn->prepare($sql);
$stmt->bind_param("s", $dni_persona);
$stmt->execute();
$result = $stmt->get_result();

if ($row = $result->fetch_assoc()) {
    echo json_encode([
        "error" => null,
        "fct" => [
            "empresa" => $row['nom_empresa'],
            "horas_totales" => (int)$row['hores_total'],
            "horas_hechas" => (int)$row['hores_fetes'],
            "estudio" => $row['estudi']
        ],
        "new_token" => $resultado['new_token']
    ]);
} else {
    echo json_encode([
        "error" => "El alumno no tiene prácticas registradas en este centro",
        "fct" => null,
        "new_token" => $resultado['new_token']
    ]);
}

$stmt->close();
$conn->close();
?>