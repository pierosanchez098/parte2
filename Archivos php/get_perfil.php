<?php
header('Content-Type: application/json; charset=utf-8');
header('Access-Control-Allow-Origin: *');

include 'seguridad.php';

$conn = new mysqli("localhost", "root", "", "plataforma_evalis");

if ($conn->connect_error) {
    echo json_encode(["error" => "Error de conexión", "datos" => null, "asignaturas" => [], "expired" => false]);
    exit;
}

$token = $_POST['token'] ?? $_GET['token'] ?? '';
$dni_persona = $_POST['dni_persona'] ?? $_GET['dni_persona'] ?? '';
$user_agent_hash_recibido = $_POST['user_agent_hash'] ?? $_GET['user_agent_hash'] ?? '';

if (empty($token) || empty($dni_persona)) {
    echo json_encode(["error" => "Faltan token o dni_persona", "datos" => null, "asignaturas" => [], "expired" => true]);
    exit;
}

$resultado = verificar_y_rotar_token($conn, $token);

if (!$resultado['valido']) {
    echo json_encode([
        "error" => $resultado['motivo'] ?? "Sesión expirada o inválida", 
        "datos" => null, 
        "asignaturas" => [], 
        "expired" => true
    ]);
    $conn->close();
    exit;
}

$sql = "
    SELECT 
        p.nom, 
        p.cognom, 
        p.foto,
        e.nia, 
        gc.nom AS nombre_grupo, 
        gc.aula,
        a.nom AS nombre_asignatura
    FROM persona p
    INNER JOIN estudiants e ON p.dni = e.dni_persona
    LEFT JOIN estudiants_grupclasse egc ON e.nia = egc.nia
    LEFT JOIN grup_classe gc ON egc.nom_grup = gc.nom 
    LEFT JOIN grupclasse_assignatura gca ON gc.nom = gca.nom_grupclasse
    LEFT JOIN assignatura a ON gca.codi_assignatura = a.codi
    WHERE p.dni = ?
";

$stmt = $conn->prepare($sql);
$stmt->bind_param("s", $dni_persona);
$stmt->execute();
$result = $stmt->get_result();

$perfil = null;
$asignaturas = [];

while ($row = $result->fetch_assoc()) {
    if ($perfil === null) {
        $perfil = [
            "nom" => $row['nom'],
            "cognom" => $row['cognom'],
            "foto" => $row['foto'],
            "nia" => $row['nia'],
            "grupo" => $row['nombre_grupo'] ?: "Sin grupo",
            "aula" => $row['aula'] ?: "N/A"
        ];
    }
    if (!empty($row['nombre_asignatura'])) {
        $asignaturas[] = $row['nombre_asignatura'];
    }
}

if ($perfil === null) {
    echo json_encode([
        "error" => "No se encontró el perfil",
        "datos" => null,
        "asignaturas" => [],
        "expired" => false
    ]);
} else {
    echo json_encode([
        "error" => null,
        "datos" => $perfil,
        "asignaturas" => $asignaturas,
        "new_token" => $resultado['new_token']
    ]);
}

$stmt->close();
$conn->close();
?>