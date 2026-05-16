# Plataforma Evalis

## 1. Arquitectura y Estructura del Repositorio

El proyecto se encuentra estructurado de forma modular para garantizar la separación de responsabilidades, dividiendo el frontend de escritorio, el frontend móvil, la capa de servicios backend y los artefactos junto con los recursos de defensa del proyecto:

```text
Evalis/
├── Aplicación escritorio/   # Sistema de gestión docente y administrativo [VB.NET / WinForms]
├── Aplicacion_movil/        # Portal nativo de servicios para el estudiante [Kotlin / Android Studio]
├── Archivos php/            # Capa intermedia de servicios y endpoints de la API REST [PHP]
├── Base de datos/           # Scripts estructurales y cargas de datos relacionales [MySQL]
├── diagramas de flujo/      # Modelado de procesos algorítmicos y lógica de negocio por rol
├── Documentación/           # Memorias técnicas del proyecto, esquemas y recursos de edición
├── presentacion/            # Diapositivas y material didáctico optimizado para la defensa pública
├── Wireframes/              # Prototipos de fidelidad media de las interfaces de usuario (UI)
└── .gitignore               # Directivas de exclusión para archivos de compilación y temporales


# Logins de prueba para más comodidad

## Móvil
* **Estudiante:** `anna.rodriguez`

## Escritorio
* **Administrador:** `maria.garcia`
* **Jefe de estudios:** `xavier.molins`
* **Profesor:** `elena.blanco`