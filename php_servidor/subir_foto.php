<?php
$conn = new mysqli("localhost", "root", "", "plataforma_evalis");

$dni = $_POST['dni'];
$imagen_base64 = $_POST['imagen'];

$nombre_archivo = "perfil_" . $dni . "_" . time() . ".jpg";
$ruta_destino = "fotos/" . $nombre_archivo;

if (file_put_contents($ruta_destino, base64_decode($imagen_base64))) {
    $url_final = "http://10.0.2.2/fotos/" . $nombre_archivo;
    $sql = "UPDATE persona SET foto = '$url_final' WHERE dni = '$dni'";
    
    if ($conn->query($sql)) {
        echo json_encode(["status" => "ok", "url" => $url_final]);
    } else {
        echo json_encode(["error" => "Error al actualizar BD"]);
    }
} else {
    echo json_encode(["error" => "Error al guardar archivo"]);
}
?>