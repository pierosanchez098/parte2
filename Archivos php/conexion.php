<?php
$servidor = "localhost";
$usuario = "root";
$password = "";
$base_datos = "plataforma_evalis";

$conn = new mysqli($servidor, $usuario, $password, $base_datos);

if ($conn->connect_error) {
    die(json_encode([
        "status" => "error",
        "motivo" => "Fallo en la conexión con la base de datos: " . $conn->connect_error
    ]));
}

$conn->set_charset("utf8");
?>