<?php
require_once 'seguridad_desktop.php'; 
require_once 'conexion.php';     

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    echo json_encode(["status" => "error", "motivo" => "Método no permitido"]);
    exit;
}

$dni_profesor = $_POST['dni_profesor'] ?? ''; 
$accion = $_POST['accion'] ?? '';

try {
    if ($accion === 'listar_grupos') {
        $stmt = $conexion->prepare("SELECT nom_grup FROM professor_grup_classe WHERE dni_persona = ?");
        $stmt->execute([$dni_profesor]);
        $grupos = $stmt->fetchAll(PDO::FETCH_ASSOC);
        
        echo json_encode(["status" => "success", "grupos" => $grupos, "new_token" => $tokenResultado]);
        
    } elseif ($accion === 'listar_alumnos') {
        $nom_grup = $_POST['nom_grup'] ?? '';
        $query = "SELECT e.nia, p.nom, p.cognom, p.email, p.dni 
                  FROM estudiants_grupclasse eg
                  INNER JOIN estudiants e ON eg.nia = e.nia
                  INNER JOIN persona p ON e.dni_persona = p.dni
                  WHERE eg.nom_grup = ? ORDER BY p.cognom, p.nom";
                  
        $stmt = $conexion->prepare($query);
        $stmt->execute([$nom_grup]);
        $alumnos = $stmt->fetchAll(PDO::FETCH_ASSOC);
        
        echo json_encode(["status" => "success", "alumnos" => $alumnos, "new_token" => $tokenResultado]);
        
    } elseif ($accion === 'ver_expediente') {
        $nia = $_POST['nia'] ?? '';
        
        $stmtA = $conexion->prepare("SELECT p.nom, p.cognom, p.dni, p.email, p.foto 
                                     FROM estudiants e 
                                     INNER JOIN persona p ON e.dni_persona = p.dni 
                                     WHERE e.nia = ?");
        $stmtA->execute([$nia]);
        $alumnoData = $stmtA->fetch(PDO::FETCH_ASSOC);
        
        if (!$alumnoData) {
            echo json_encode(["status" => "error", "motivo" => "Alumno no encontrado"]);
            exit;
        }
        
        $stmtE = $conexion->prepare("SELECT nom_estudi, curs_inici, curs_fi, status, nota_final FROM estudis WHERE nia = ?");
        $stmtE->execute([$nia]);
        $estudios = $stmtE->fetchAll(PDO::FETCH_ASSOC);
        
        echo json_encode([
            "status" => "success",
            "nombre_alumno" => $alumnoData['nom'] . " " . $alumnoData['cognom'],
            "dni_alumno" => $alumnoData['dni'],
            "email_alumno" => $alumnoData['email'],
            "foto_carnet" => $alumnoData['foto'],
            "estudis" => $estudios,
            "new_token" => $tokenResultado
        ]);
    } else {
        echo json_encode(["status" => "error", "motivo" => "Acción no válida"]);
    }
} catch (Exception $e) {
    echo json_encode(["status" => "error", "motivo" => "Error de base de datos: " . $e->getMessage()]);
}
?>