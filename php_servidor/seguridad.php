<?php

function verificar_y_rotar_token($conn, $token_recibido) {
    $inactivity_limit = 1800; 

    if (empty($token_recibido)) {
        return ["valido" => false, "new_token" => null];
    }

    $token_hash = hash('sha256', $token_recibido);

    $stmt = $conn->prepare("SELECT username, last_activity FROM sessions WHERE token_hash = ? AND session_end IS NULL");
    $stmt->bind_param("s", $token_hash);
    $stmt->execute();
    $res = $stmt->get_result();
    $session = $res->fetch_assoc();

    if (!$session) {
        return ["valido" => false, "new_token" => null];
    }

    $ultima_act = strtotime($session['last_activity']);
    if ((time() - $ultima_act) > $inactivity_limit) {
        $stmt = $conn->prepare("UPDATE sessions SET session_end = NOW() WHERE token_hash = ?");
        $stmt->bind_param("s", $token_hash);
        $stmt->execute();
        return ["valido" => false, "new_token" => null];
    }

    $new_token = bin2hex(random_bytes(32));
    $new_hash = hash('sha256', $new_token);

    $stmt = $conn->prepare("UPDATE sessions SET token_hash = ?, last_activity = NOW() WHERE token_hash = ?");
    $stmt->bind_param("ss", $new_hash, $token_hash);
    $stmt->execute();

    return ["valido" => true, "new_token" => $new_token];
}
?>