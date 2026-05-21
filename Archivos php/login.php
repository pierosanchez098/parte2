<?php
header("Content-Type: application/json; charset=utf-8");

$host = "localhost";
$user = "root";
$pass = "";
$db = "plataforma_evalis";

$conn = new mysqli($host, $user, $pass, $db);
if ($conn->connect_error) {
    echo json_encode(["pot_entrar" => false, "tipus_derror" => "Error de BD"]);
    exit;
}

$username = $_REQUEST["user"] ?? "";
$password = $_REQUEST["pass"] ?? "";
$ip_address = $_SERVER['REMOTE_ADDR'];
$user_agent_hash = $_REQUEST["user_agent_hash"] ?? "unknown";

if ($username === "" || $password === "") {
    echo json_encode(["pot_entrar" => false, "tipus_derror" => "Falten camps"]);
    exit;
}

$stmt = $conn->prepare("SELECT password_hash, dni_persona FROM usuari WHERE username = ?");
$stmt->bind_param("s", $username);
$stmt->execute();
$res = $stmt->get_result();
$user_data = $res->fetch_assoc();

$login_success = 0;
$token = null;
$dni_persona = null;

if ($user_data && password_verify($password, $user_data['password_hash'])) {
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
}

$stmt_log = $conn->prepare("
    INSERT INTO login_logs (login_timestamp, username, ip_direccio, login_success) 
    VALUES (NOW(), ?, ?, ?)
");
$stmt_log->bind_param("ssi", $username, $ip_address, $login_success);
$stmt_log->execute();

if ($login_success) {
    echo json_encode([
        "pot_entrar" => true,
        "dni_persona" => $dni_persona,
        "token" => $token,
        "missatge" => "Bienvenido/a"
    ]);
} else {
    echo json_encode([
        "pot_entrar" => false,
        "tipus_derror" => "Usuari o contrasenya incorrectes"
    ]);
}

$conn->close();