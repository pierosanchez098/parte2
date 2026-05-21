<?php
header('Content-Type: application/json; charset=utf-8');
header('Access-Control-Allow-Origin: *');

include 'seguridad.php';

$conn = new mysqli("localhost", "root", "", "plataforma_evalis");

if ($conn->connect_error) {
    echo json_encode(["error" => "Error de conexión", "medias" => [], "expired" => false]);
    exit;
}

$token = $_POST['token'] ?? $_GET['token'] ?? '';
$dni_persona = $_POST['dni_persona'] ?? $_GET['dni_persona'] ?? '';
$user_agent_hash_recibido = $_POST['user_agent_hash'] ?? $_GET['user_agent_hash'] ?? '';

if (empty($token) || empty($dni_persona)) {
    echo json_encode(["error" => "Faltan token o dni_persona", "medias" => [], "expired" => true]);
    exit;
}

$resultado = verificar_y_rotar_token($conn, $token);

if (!$resultado['valido']) {
    echo json_encode([
        "error" => $resultado['motivo'] ?? "Sesión expirada o inválida", 
        "medias" => [],
        "expired" => true
    ]);
    $conn->close();
    exit;
}

$sql = "
    SELECT 
        a.nom AS assignatura_nom, 
        ROUND(AVG(CAST(n.nota AS DECIMAL(4,2))), 2) AS media_nota
    FROM estudiants e
    INNER JOIN notes n ON e.nia = n.nia
    INNER JOIN unitat u ON n.uf_id = u.id
    INNER JOIN modul m ON u.modul = m.nom
    INNER JOIN assignatura a ON m.codi_assignatura = a.codi
    WHERE e.dni_persona = ? AND n.nota NOT IN ('-', '', 'null')
    GROUP BY a.nom
";

$stmt = $conn->prepare($sql);
$stmt->bind_param("s", $dni_persona);
$stmt->execute();
$result = $stmt->get_result();

$medias = [];
while ($row = $result->fetch_assoc()) {
    $medias[] = [
        "asignatura" => $row['assignatura_nom'],
        "media" => (float)$row['media_nota']
    ];
}

if (empty($medias)) {
    echo json_encode([
        "error" => "El alumno no tiene notas registradas para calcular medias",
        "medias" => [],
        "new_token" => $resultado['new_token']
    ]);
} else {
    echo json_encode([
        "error" => null,
        "medias" => $medias,
        "new_token" => $resultado['new_token']
    ]);
}

$stmt->close();
$conn->close();
?>