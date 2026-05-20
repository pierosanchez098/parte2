<?php
header('Content-Type: application/json; charset=utf-8');
header('Access-Control-Allow-Origin: *');

include 'seguridad.php';

$conn = new mysqli("localhost", "root", "", "plataforma_evalis");

if ($conn->connect_error) {
    echo json_encode(["error" => "Error de conexión a la BD", "ultima_nota" => null]);
    exit;
}

$token = $_POST['token'] ?? $_GET['token'] ?? '';
$dni_persona = $_POST['dni_persona'] ?? $_GET['dni_persona'] ?? '';

if (empty($token) || empty($dni_persona)) {
    echo json_encode(["error" => "Faltan token o dni_persona", "ultima_nota" => null, "expired" => true]);
    exit;
}

$resultado = verificar_y_rotar_token($conn, $token);

if (!$resultado['valido']) {
    echo json_encode(["error" => "Sesión expirada o inválida", "ultima_nota" => null, "expired" => true]);
    $conn->close();
    exit;
}

$sql = "
    SELECT 
        n.nota,
        n.uf_id,
        DATE_FORMAT(n.data_nota, '%d/%m/%Y %H:%i') AS data_nota_formateada,
        n.data_nota AS data_nota_raw
    FROM notes n
    INNER JOIN estudiants est ON n.nia = est.nia
    WHERE est.dni_persona = ?
    ORDER BY n.data_nota DESC 
    LIMIT 1
";

$stmt = $conn->prepare($sql);
$stmt->bind_param("s", $dni_persona);
$stmt->execute();
$result = $stmt->get_result();

$ultima_nota = null;

if ($row = $result->fetch_assoc()) {
    $ultima_nota = [
        "nota" => $row['nota'],
        "uf_id" => $row['uf_id'],
        "data_nota" => $row['data_nota_formateada'],
        "data_nota_raw" => $row['data_nota_raw'] 
    ];
}

echo json_encode([
    "error" => null,
    "ultima_nota" => $ultima_nota,
    "new_token" => $resultado['new_token']
]);

$stmt->close();
$conn->close();
?>