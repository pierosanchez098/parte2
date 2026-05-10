<?php
header('Content-Type: application/json; charset=utf-8');
header('Access-Control-Allow-Origin: *');

include 'seguridad.php';      

$conn = new mysqli("localhost", "root", "", "plataforma_evalis");

if ($conn->connect_error) {
    echo json_encode(["error" => "Error de conexión a la BD", "notas" => [], "expired" => false]);
    exit;
}

$token = $_POST['token'] ?? $_GET['token'] ?? '';
$dni_persona = $_POST['dni_persona'] ?? $_GET['dni_persona'] ?? '';

if (empty($token) || empty($dni_persona)) {
    echo json_encode([
        "error" => "Faltan token o dni_persona",
        "notas" => [],
        "expired" => true
    ]);
    exit;
}

$resultado = verificar_y_rotar_token($conn, $token);

if (!$resultado['valido']) {
    echo json_encode([
        "error" => "Sesión expirada o inválida",
        "notas" => [],
        "expired" => true
    ]);
    $conn->close();
    exit;
}

$curso_actual = '2025-2026'; 

$sql = "
    SELECT 
        m.nom_complet AS modul,
        u.nom AS unitat,
        n.nota,
        DATE_FORMAT(n.data_nota, '%d/%m/%Y') AS data_nota
    FROM notes n
    INNER JOIN unitat u ON n.uf_id = u.id
    INNER JOIN modul m ON u.modul = m.nom
    INNER JOIN estudiants est ON n.nia = est.nia
    INNER JOIN matricula mat ON est.nia = mat.nia 
        AND mat.codi_assignatura = m.codi_assignatura
    WHERE est.dni_persona = ?
      AND mat.curso = ?                 
    ORDER BY m.nom_complet, u.nom
";

$stmt = $conn->prepare($sql);
$stmt->bind_param("ss", $dni_persona, $curso_actual);
$stmt->execute();
$result = $stmt->get_result();

$notas = [];
while ($row = $result->fetch_assoc()) {
    $notas[] = $row;
}

echo json_encode([
    "error"     => null,
    "notas"     => $notas,
    "curso"     => $curso_actual,
    "new_token" => $resultado['new_token']
]);

$stmt->close();
$conn->close();
?>