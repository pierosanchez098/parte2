<?php

error_reporting(0);
ini_set('display_errors', 0);

function verificar_y_rotar_token($conn, $token_recibido) {
    
    if (empty($token_recibido)) {
        return [
            "valido" => false, 
            "motivo" => "Token ausente en la petición web."
        ];
    }

    $user_agent_hash = $_POST['user_agent_hash'] ?? '';
    if (empty($user_agent_hash)) {
        return [
            "valido" => false, 
            "motivo" => "Acceso denegado: Falta la huella digital del hardware del dispositivo."
        ];
    }

    $token_hash_recibido = hash('sha256', $token_recibido);

    $stmt = $conn->prepare("
        SELECT username, user_agent, last_activity 
        FROM sessions 
        WHERE token_hash = ?
    ");
    $stmt->bind_param("s", $token_hash_recibido);
    $stmt->execute();
    $session = $stmt->get_result()->fetch_assoc();
    $stmt->close();

    if (!$session) {
        return [
            "valido" => false, 
            "motivo" => "Sesión inválida o expirada. Vuelva a iniciar sesión."
        ];
    }

    if ($session['user_agent'] !== $user_agent_hash) {
        return [
            "valido" => false, 
            "motivo" => "Fallo de seguridad crítica: Este token pertenece a otra máquina (Hardware Mismatch)."
        ];
    }

    $ahora = time();
    $ultima_actividad = strtotime($session['last_activity']);
    
    if (($ahora - $ultima_actividad) > 900) {
        $stmt_del = $conn->prepare("DELETE FROM sessions WHERE token_hash = ?");
        $stmt_del->bind_param("s", $token_hash_recibido);
        $stmt_del->execute();
        $stmt_del->close();

        return [
            "valido" => false, 
            "motivo" => "Su sesión ha expirado por inactividad tras 15 minutos."
        ];
    }

    $nuevo_token = bin2hex(random_bytes(32));
    $nuevo_token_hash = hash('sha256', $nuevo_token);

    $stmt_update = $conn->prepare("
        UPDATE sessions 
        SET token_hash = ?, last_activity = NOW() 
        WHERE token_hash = ?
    ");
    $stmt_update->bind_param("ss", $nuevo_token_hash, $token_hash_recibido);
    $stmt_update->execute();
    $stmt_update->close();

    return [
        "valido" => true,
        "new_token" => $nuevo_token,
        "username" => $session['username']
    ];
}
?>