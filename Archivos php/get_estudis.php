<?php
header('Content-Type: application/json; charset=utf-8');
header('Access-Control-Allow-Origin: *');

include 'seguridad.php';

$conn = new mysqli("localhost", "root", "", "plataforma_evalis");

if ($conn->connect_error) {
    echo json_encode(["error" => "Error de conexión a la BD", "estudis" => [], "foto_carnet" => null, "expired" => false]);
    exit;
}

$token = $_POST['token'] ?? $_GET['token'] ?? '';
$dni_persona = $_POST['dni_persona'] ?? $_GET['dni_persona'] ?? '';
$user_agent_hash_recibido = $_POST['user_agent_hash'] ?? $_GET['user_agent_hash'] ?? '';

if (empty($token) || empty($dni_persona)) {
    echo json_encode([
        "error" => "Faltan token o dni_persona", 
        "estudis" => [], 
        "foto_carnet" => null, 
        "expired" => true
    ]);
    exit;
}

$resultado = verificar_y_rotar_token($conn, $token);

if (!$resultado['valido']) {
    echo json_encode([
        "error" => $resultado['motivo'] ?? "Sesión expirada o inválida", 
        "estudis" => [], 
        "foto_carnet" => null, 
        "expired" => true
    ]);
    $conn->close();
    exit;
}

$sql = "
    SELECT 
        e.nom_estudi,
        e.curs_inici,
        e.curs_fi,
        e.status,
        e.nota_final,
        DATE_FORMAT(e.curs_inici, '%d/%m/%Y') AS data_inici,
        DATE_FORMAT(e.curs_fi, '%d/%m/%Y') AS data_fi,
        u.foto_carnet
    FROM estudis e
    INNER JOIN estudiants est ON e.nia = est.nia
    INNER JOIN usuari u ON est.dni_persona = u.dni_persona
    WHERE est.dni_persona = ?
    ORDER BY e.curs_inici DESC, e.nom_estudi
";

$stmt = $conn->prepare($sql);
$stmt->bind_param("s", $dni_persona);
$stmt->execute();
$result = $stmt->get_result();

$estudis = [];
$foto_carnet = null;

while ($row = $result->fetch_assoc()) {
    if ($foto_carnet === null) {
        $foto_carnet = $row['foto_carnet'];  
    }
    $estudis[] = [
        "nom_estudi" => $row['nom_estudi'],
        "curs_inici" => $row['curs_inici'],
        "curs_fi"    => $row['curs_fi'],
        "status"     => $row['status'],
        "nota_final" => $row['nota_final'] ?: "Pendiente"
    ];
}

echo json_encode([
    "error" => null,
    "estudis" => $estudis,
    "foto_carnet" => $foto_carnet,
    "new_token" => $resultado['new_token']
]);

$stmt->close();
$conn->close();
?>