<?php
header('Content-Type: application/json; charset=utf-8');
header('Access-Control-Allow-Origin: *');

include 'seguridad.php';

$conn = new mysqli("localhost", "root", "", "plataforma_evalis");

if ($conn->connect_error) {
    echo json_encode(["error" => "Error de conexión", "aula" => null, "professors" => [], "expired" => false]);
    exit;
}

$token = $_POST['token'] ?? $_GET['token'] ?? '';
$dni_persona = $_POST['dni_persona'] ?? $_GET['dni_persona'] ?? '';
$user_agent_hash_recibido = $_POST['user_agent_hash'] ?? $_GET['user_agent_hash'] ?? '';

if (empty($token) || empty($dni_persona)) {
    echo json_encode(["error" => "Faltan token o dni_persona", "aula" => null, "professors" => [], "expired" => true]);
    exit;
}

$resultado = verificar_y_rotar_token($conn, $token);

if (!$resultado['valido']) {
    echo json_encode([
        "error" => $resultado['motivo'] ?? "Sesión expirada o inválida", 
        "aula" => null, 
        "professors" => [], 
        "expired" => true
    ]);
    $conn->close();
    exit;
}

$aula = null;
$stmt_aula = $conn->prepare("
    SELECT gc.aula
    FROM estudiants est
    INNER JOIN estudiants_grupclasse eg ON est.nia = eg.nia
    INNER JOIN grup_classe gc ON eg.nom_grup = gc.nom
    WHERE est.dni_persona = ?
");
$stmt_aula->bind_param("s", $dni_persona);
$stmt_aula->execute();
$result_aula = $stmt_aula->get_result();
if ($row = $result_aula->fetch_assoc()) {
    $aula = $row['aula'];
}
$stmt_aula->close();

$sql = "
    SELECT
        pers.dni,
        pers.nom,
        pers.cognom,
        pers.email,
        pers.foto,
        GROUP_CONCAT(
            CONCAT(assign.nom, ' (matriculado el ', DATE_FORMAT(mat.data_matricula, '%d/%m/%Y'), ')')
            SEPARATOR ' | '
        ) AS assignatures_impartides
    FROM estudiants est
    INNER JOIN persona alum ON est.dni_persona = alum.dni
    INNER JOIN matricula mat ON est.nia = mat.nia
    INNER JOIN assignatura assign ON mat.codi_assignatura = assign.codi
    INNER JOIN assignatura_professor ap ON assign.codi = ap.codi_assig
    INNER JOIN professor prof ON ap.id_intern_prof = prof.id_intern
    INNER JOIN persona pers ON prof.dni_persona = pers.dni
    WHERE alum.dni = ?
    GROUP BY prof.id_intern
    ORDER BY pers.cognom, pers.nom
";

$stmt = $conn->prepare($sql);
$stmt->bind_param("s", $dni_persona);
$stmt->execute();
$result = $stmt->get_result();

$professors = [];
$currentId = null;
$currentProf = null;

while ($row = $result->fetch_assoc()) {
    if ($row['dni'] != $currentId) {
        if ($currentProf !== null) {
            $professors[] = $currentProf;
        }
        $currentProf = [
            "nomComplet" => trim($row['cognom'] . ', ' . $row['nom']),
            "email" => $row['email'],
            "foto" => $row['foto'] ?: null,
            "assignatures" => []
        ];
        $currentId = $row['dni'];
    }
    if ($row['assignatures_impartides']) {
        $currentProf["assignatures"] = array_merge(
            $currentProf["assignatures"],
            $currentProf["assignatures"] = explode(' | ', $row['assignatures_impartides'])
        );
    }
}

if ($currentProf !== null) {
    $professors[] = $currentProf;
}

echo json_encode([
    "error" => null,
    "aula" => $aula,
    "professors" => $professors,
    "new_token" => $resultado['new_token']
]);

$stmt->close();
$conn->close();
?>