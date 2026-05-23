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
$nia = $_POST['nia'] ?? ''; 

$res_seguridad = verificar_y_rotar_token($conn, $token);

if (!$res_seguridad['valido']) {
    echo json_encode([
        "status" => "error",
        "motivo" => $res_seguridad['motivo'] ?? "Sesión inválida o expirada."
    ]);
    exit;
}

$new_token = $res_seguridad['new_token'];

if (empty($nia)) {
    echo json_encode([
        "status" => "error",
        "motivo" => "Falta el parámetro NIA del alumno.",
        "new_token" => $new_token
    ]);
    exit;
}

$curso_actual = '2025-2026'; 

$sql_centro = "SELECT c.nom AS centro_nom, c.logo AS centro_logo 
               FROM estudiants e
               INNER JOIN persona p ON e.dni_persona = p.dni
               LEFT JOIN centre c ON p.id_centre = c.id_centre 
               WHERE e.nia = ?";
$stmt_C = $conn->prepare($sql_centro);
$stmt_C->bind_param("s", $nia);
$stmt_C->execute();
$res_centro = $stmt_C->get_result()->fetch_assoc();
$stmt_C->close();

$centro_educativo = $res_centro['centro_nom'] ?? "Centro no asignado";
$logo_centro = $res_centro['centro_logo'] ?? null;

$sql = "SELECT 
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
        WHERE n.nia = ?
          AND mat.curso = ?                
        ORDER BY m.nom_complet, u.nom";

$stmt = $conn->prepare($sql);
$stmt->bind_param("ss", $nia, $curso_actual);
$stmt->execute();
$result = $stmt->get_result();

$notas = [];
while ($row = $result->fetch_assoc()) {
    $notas[] = [
        "modul" => $row['modul'],
        "unitat" => $row['unitat'],
        "nota" => $row['nota'],
        "data_nota" => $row['data_nota']
    ];
}

$stmt->close();
$conn->close();

echo json_encode([
    "status" => "success",
    "notas" => $notas,
    "curso" => $curso_actual,
    "centro_educativo" => $centro_educativo,
    "logo_centro" => $logo_centro,
    "new_token" => $new_token
]);
?>