<?php
$contrasena_plano = "123456";

$hash = password_hash($contrasena_plano, PASSWORD_DEFAULT);

echo "Contraseña en plano: $contrasena_plano<br>";
echo "Hash generado: $hash";
?>