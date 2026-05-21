<?php

function verificar_y_rotar_token($conn, $token_recibido) {
    $inactivity_limit = 1800; 

    if (empty($token_recibido)) {
        return ["valido" => false, "new_token" => null, "motivo" => "Token no proporcionado."];
    }

    $token_hash = hash('sha256', $token_recibido);

    $stmt = $conn->prepare("SELECT username, last_activity, ip_address, user_agent FROM sessions WHERE token_hash = ? AND session_end IS NULL");
    $stmt->bind_param("s", $token_hash);
    $stmt->execute();
    $res = $stmt->get_result();
    $session = $res->fetch_assoc();

    if (!$session) {
        return ["valido" => false, "new_token" => null, "motivo" => "Sesión no encontrada o ya finalizada."];
    }

    $ip_actual = $_SERVER['REMOTE_ADDR'];
    $ip_guardada = $session['ip_address'];
    
    $ua_guardado = $session['user_agent']; 

    $user_agent_hash_recibido = $_POST['user_agent_hash'] ?? $_GET['user_agent_hash'] ?? '';

    $parts_actual = explode('.', $ip_actual);
    $parts_guardada = explode('.', $ip_guardada);

    $subnet_actual = (count($parts_actual) === 4) ? "{$parts_actual[0]}.{$parts_actual[1]}.{$parts_actual[2]}" : $ip_actual;
    $subnet_guardada = (count($parts_guardada) === 4) ? "{$parts_guardada[0]}.{$parts_guardada[1]}.{$parts_guardada[2]}" : $ip_guardada;

    $cambio_red = ($subnet_actual !== $subnet_guardada);
    
    $cambio_dispositivo = ($ua_guardado !== $user_agent_hash_recibido);

    if ($cambio_red || $cambio_dispositivo) {
        $stmt_close = $conn->prepare("UPDATE sessions SET session_end = NOW() WHERE token_hash = ?");
        $stmt_close->bind_param("s", $token_hash);
        $stmt_close->execute();
        $stmt_close->close();
        
        return [
            "valido" => false, 
            "new_token" => null, 
            "motivo" => "Riesgo alto: Se ha detectado un cambio de entorno sospechoso. Por seguridad, inicie sesión de nuevo."
        ];
    }

    $ultima_act = strtotime($session['last_activity']);
    if ((time() - $ultima_act) > $inactivity_limit) {
        $stmt_timeout = $conn->prepare("UPDATE sessions SET session_end = NOW() WHERE token_hash = ?");
        $stmt_timeout->bind_param("s", $token_hash);
        $stmt_timeout->execute();
        $stmt_timeout->close();
        
        return ["valido" => false, "new_token" => null, "motivo" => "La sesión ha expirado por inactividad."];
    }

    $new_token = bin2hex(random_bytes(32));
    $new_hash = hash('sha256', $new_token);

    $stmt_update = $conn->prepare("UPDATE sessions SET token_hash = ?, last_activity = NOW() WHERE token_hash = ?");
    $stmt_update->bind_param("ss", $new_hash, $token_hash);
    $stmt_update->execute();
    
    $stmt->close();
    $stmt_update->close();

    return [
        "valido" => true, 
        "new_token" => $new_token, 
        "motivo" => null,
        "ip_address" => $ip_guardada,
        "user_agent" => $ua_guardado
    ];
}
?>