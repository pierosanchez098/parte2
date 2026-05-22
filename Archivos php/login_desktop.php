<?php
error_reporting(0);
ini_set('display_errors', 0);

header('Content-Type: application/json; charset=utf-8');

require_once 'conexion.php';

$username = $_POST["user"] ?? "";
$password = $_POST["pass"] ?? "";
$user_agent_hash = $_POST["user_agent_hash"] ?? "";
$ip_address = $_SERVER['REMOTE_ADDR'];

if ($username === "" || $password === "" || $user_agent_hash === "") {
    echo json_encode(["pot_entrar" => false, "tipus_derror" => "Falten camps o petició insegura"]);
    exit;
}

$stmt = $conn->prepare("
    SELECT u.password_hash, u.dni_persona, p.rol 
    FROM usuari u
    INNER JOIN persona p ON u.dni_persona = p.dni
    WHERE u.username = ?
");
$stmt->bind_param("s", $username);
$stmt->execute();
$res = $stmt->get_result();
$user_data = $res->fetch_assoc();

$login_success = 0;
$token = null;
$dni_persona = null;
$rol = 'estudiant'; 

if ($user_data && password_verify($password, $user_data['password_hash'])) {
    $rol = strtolower(trim($user_data['rol']));


    $login_success = 1;
    $dni_persona = $user_data['dni_persona'];
    
    $token = bin2hex(random_bytes(32)); 
    $token_hash = hash('sha256', $token);

    $stmt_sess = $conn->prepare("
        INSERT INTO sessions (token_hash, username, ip_address, user_agent, session_start, last_activity) 
        VALUES (?, ?, ?, ?, NOW(), NOW())
    ");
    $stmt_sess->bind_param("ssss", $token_hash, $username, $ip_address, $user_agent_hash);
    $stmt_sess->execute();
    $stmt_sess->close();
}

$stmt_log = $conn->prepare("
    INSERT INTO login_logs (login_timestamp, username, ip_direccio, login_success) 
    VALUES (NOW(), ?, ?, ?)
");
$stmt_log->bind_param("ssi", $username, $ip_address, $login_success);
$stmt_log->execute();
$stmt_log->close();

if ($login_success) {
    echo json_encode([
        "status" => "success", 
        "pot_entrar" => true,
        "dni_persona" => $dni_persona,
        "token" => $token,
        "rol" => $rol,
        "missatge" => "Bienvenido/a al panel de gestión"
    ]);
} else {
    echo json_encode([
        "pot_entrar" => false,
        "tipus_derror" => "Usuari o contrasenya incorrectes"
    ]);
}

$conn->close();
?>