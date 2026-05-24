# Plataforma Evalis

## 1. Arquitectura y Estructura del Repositorio

El proyecto está estructurado de forma modular para garantizar la separación de responsabilidades, dividiendo el frontend de escritorio, el frontend móvil, la capa de servicios backend y los artefactos junto con los recursos de defensa del proyecto:

- **`Aplicación escritorio`**: Proyecto de escritorio [VB.NET / WinForms]
- **`Aplicacion_movil`**: Proyecto de móvil [Kotlin / Android Studio]

### Desarrollo y Datos
- **`Archivos php`**: Endpoints del backend que conectan la base de datos con las aplicaciones.
- **`Base de datos`**: Script SQL y modelos de la base de datos.

### Modelado y Diagramas UML
- **`Casos de uso`**: Diagramas que definen las interacciones de los actores con el sistema.
- **`Diagrama de secuencia`**: Flujos de interacción detallados por plataforma:
    - `Android` / `Desktop`
- **`Diagramas de clases desktop UML`**: Estructura estática del sistema en la versión escritorio.
- **`Diagramas de componentes UML`**: Organización de los módulos de software.
- **`diagramas de flujo`**: Lógica de procesos específicos:
    - `Mostrar Expediente` / `Poner calificación`
- **`Diagramas de objetos`**: Instancias concretas de las clases durante la ejecución.

### Documentación y Presentación
- **`Documentación`**: Contiene la documentación técnica del proyecto en formatos:
    - `Lyx` (LaTeX) / `Word`
- **`Presentacion`**: Archivos fuente de la presentación final (realizada con PowerPoint y otra con LaTeX Beamer).

### Diseño (UI/UX)
- **`Wireframes`**: Prototipos visuales de las interfaces:
    - **Escritorio**: Clasificados por roles (`admin`, `cap de estudis`, `profesores`).
    - **Movil**: Interfaz diseñada para dispositivos Android.

---

## Tecnologías Utilizadas
* **Backend:** PHP
* **Escritorio:** .NET (Visual Studio)
* **Móvil:** Android Studio
* **Modelado:** UML
* **Documentación:** LaTeX (Lyx / Beamer)

---

## Logins de prueba para mayor comodidad

### Móvil
* **Estudiante:** `anna.rodriguez`

### Escritorio
* **Administrador:** `maria.garcia`
* **Jefe de estudios:** `xavier.molins`
* **Profesor:** `elena.blanco`

### Contraseña (para todos)
* `123456`