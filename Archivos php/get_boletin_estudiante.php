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
$dni_persona = $_POST['dni_persona'] ?? ''; 

$res_seguridad = verificar_y_rotar_token($conn, $token);

if (!$res_seguridad['valido']) {
    echo json_encode([
        "status" => "error",
        "motivo" => $res_seguridad['motivo'] ?? "Sesión inválida o expirada."
    ]);
    exit;
}

$new_token = $res_seguridad['new_token'];
$curso_actual = '2025-2026'; 

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
        WHERE est.dni_persona = ?
          AND mat.curso = ?                
        ORDER BY m.nom_complet, u.nom";

$stmt = $conn->prepare($sql);
$stmt->bind_param("ss", $dni_persona, $curso_actual);
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
    "new_token" => $new_token
]);
?>