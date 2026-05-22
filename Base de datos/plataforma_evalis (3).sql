-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 23-05-2026 a las 00:52:47
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `plataforma_evalis`
--

DELIMITER $$
--
-- Procedimientos
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `calcular_mitjana_final` (IN `p_nia` INT, IN `p_modul` VARCHAR(10))   BEGIN
    DECLARE v_mitjana_calculada DECIMAL(5,2);
    DECLARE v_mitjana_entera    INT;

    -- Calculamos la media de todas las notas del alumno en ese módulo
    SELECT COALESCE(AVG(n.nota), 0) INTO v_mitjana_calculada
    FROM notes n
    JOIN unitat u ON n.uf_id = u.id
    WHERE n.nia = p_nia
      AND u.modul = p_modul;

    -- Convertimos a entero (redondeando)
    SET v_mitjana_entera = ROUND(v_mitjana_calculada);

    -- Insertamos en historic_mitjanes
    INSERT INTO historic_mitjanes (mitjana, modul)
    VALUES (v_mitjana_entera, p_modul);

END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `eliminar_alumne_de_grup` (IN `p_nia` INT, IN `p_nom_grup` VARCHAR(20))   BEGIN
    DECLARE files INT DEFAULT 0;

    DELETE FROM estudiants_grupclasse 
    WHERE nia = p_nia 
      AND nom_grup = p_nom_grup;

    SET files = ROW_COUNT();

    IF files > 0 THEN
        SELECT CONCAT('Alumne ', p_nia, ' eliminat correctament del grup ', p_nom_grup, '.') AS Resultat;
    ELSE
        SELECT CONCAT('No s''ha trobat l''alumne ', p_nia, ' al grup ', p_nom_grup, '.') AS Resultat;
    END IF;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `administradors`
--

CREATE TABLE `administradors` (
  `dni_administrador` varchar(9) NOT NULL,
  `dades_sensibles` tinyint(1) DEFAULT NULL,
  `gestions_backup` tinyint(1) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `administradors`
--

INSERT INTO `administradors` (`dni_administrador`, `dades_sensibles`, `gestions_backup`) VALUES
('12345678A', NULL, 0),
('33445566G', NULL, 1),
('39728461T', NULL, 0),
('41350692Y', NULL, 0),
('45092831K', NULL, 1),
('45678901X', NULL, 0),
('47210583M', NULL, 0),
('66778811U', NULL, 0),
('74185296H', NULL, 0),
('95135782L', NULL, 0);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `asistencia_per_hora`
--

CREATE TABLE `asistencia_per_hora` (
  `id_hora` int(11) NOT NULL,
  `nia` int(11) NOT NULL,
  `present` tinyint(1) NOT NULL DEFAULT 1,
  `retard_minuts` tinyint(4) DEFAULT NULL,
  `observacions` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `asistencia_per_hora`
--

INSERT INTO `asistencia_per_hora` (`id_hora`, `nia`, `present`, `retard_minuts`, `observacions`) VALUES
(13, 151704, 1, NULL, NULL),
(13, 270477, 1, NULL, NULL),
(13, 430700, 1, NULL, NULL),
(13, 458305, 1, 7, 'Tard'),
(13, 567540, 1, NULL, NULL),
(13, 857629, 1, NULL, NULL),
(13, 968100, 0, NULL, 'Malalt'),
(14, 151704, 1, NULL, NULL),
(14, 270477, 1, NULL, NULL),
(14, 430700, 1, NULL, NULL),
(14, 458305, 1, NULL, NULL),
(14, 567540, 1, NULL, NULL),
(14, 857629, 1, NULL, NULL),
(14, 968100, 1, NULL, NULL),
(15, 234033, 1, NULL, NULL),
(15, 273885, 1, NULL, NULL),
(15, 313056, 1, NULL, NULL),
(15, 345805, 1, NULL, NULL),
(15, 665021, 1, NULL, NULL),
(15, 727264, 1, NULL, NULL),
(15, 970564, 0, NULL, 'Cita'),
(15, 993346, 1, NULL, NULL),
(16, 234033, 1, NULL, NULL),
(16, 273885, 1, NULL, NULL),
(16, 313056, 1, NULL, NULL),
(16, 345805, 1, NULL, NULL),
(16, 395902, 1, NULL, NULL),
(16, 554569, 0, NULL, 'Justificada metge'),
(16, 665021, 1, NULL, NULL),
(16, 727264, 1, NULL, NULL),
(16, 727471, 1, NULL, NULL),
(16, 970564, 1, NULL, NULL),
(16, 993346, 1, NULL, NULL),
(17, 234033, 1, NULL, NULL),
(17, 273885, 1, NULL, NULL),
(17, 313056, 1, NULL, NULL),
(17, 395902, 1, NULL, NULL),
(17, 665021, 1, NULL, NULL),
(17, 727264, 1, NULL, NULL),
(17, 970564, 1, NULL, NULL),
(17, 993346, 0, NULL, 'Justificada'),
(18, 151704, 1, NULL, NULL),
(18, 270477, 0, NULL, 'Família'),
(18, 430700, 1, NULL, NULL),
(18, 458305, 1, NULL, NULL),
(18, 567540, 1, NULL, NULL),
(18, 857629, 1, NULL, NULL),
(18, 968100, 1, NULL, NULL),
(19, 234033, 0, NULL, 'Gripe'),
(19, 273885, 1, 12, 'Tard bus'),
(19, 313056, 1, NULL, NULL),
(19, 345805, 1, NULL, NULL),
(19, 395902, 1, NULL, NULL),
(19, 554569, 1, NULL, NULL),
(19, 665021, 1, NULL, NULL),
(19, 727264, 1, NULL, NULL),
(19, 727471, 1, NULL, NULL),
(19, 968618, 1, NULL, NULL),
(19, 970564, 1, NULL, NULL),
(19, 993346, 1, NULL, NULL),
(20, 151704, 1, NULL, NULL),
(20, 270477, 1, NULL, NULL),
(20, 430700, 1, NULL, NULL),
(20, 458305, 1, NULL, NULL),
(20, 567540, 1, NULL, NULL),
(20, 857629, 0, NULL, 'Cita'),
(20, 968100, 1, NULL, NULL),
(21, 234033, 1, NULL, NULL),
(21, 273885, 1, NULL, NULL),
(21, 313056, 1, NULL, NULL),
(21, 345805, 0, NULL, 'Dentista'),
(21, 395902, 1, NULL, NULL),
(21, 665021, 1, NULL, NULL),
(21, 727264, 1, NULL, NULL),
(21, 727471, 1, NULL, NULL),
(21, 970564, 0, NULL, 'Cita oftalmòleg'),
(21, 993346, 1, 18, 'Tard despertador'),
(22, 234033, 1, NULL, NULL),
(22, 273885, 1, NULL, NULL),
(22, 313056, 1, NULL, NULL),
(22, 395902, 1, NULL, NULL),
(22, 554569, 1, NULL, NULL),
(22, 665021, 1, NULL, NULL),
(22, 727264, 1, NULL, NULL),
(22, 727471, 1, NULL, NULL),
(22, 970564, 1, NULL, NULL),
(22, 993346, 1, NULL, NULL),
(23, 234033, 1, NULL, NULL),
(23, 273885, 1, NULL, NULL),
(23, 313056, 1, NULL, NULL),
(23, 345805, 1, NULL, NULL),
(23, 395902, 1, NULL, NULL),
(23, 554569, 1, NULL, NULL),
(23, 665021, 1, NULL, NULL),
(23, 727264, 1, NULL, NULL),
(23, 727471, 1, NULL, NULL),
(23, 968618, 1, NULL, NULL),
(23, 970564, 1, NULL, NULL),
(23, 993346, 1, NULL, NULL),
(24, 234033, 1, NULL, NULL),
(24, 273885, 1, NULL, NULL),
(24, 313056, 1, NULL, NULL),
(24, 345805, 1, NULL, NULL),
(24, 665021, 1, NULL, NULL),
(24, 727264, 0, NULL, 'Metge'),
(24, 970564, 1, NULL, NULL),
(24, 993346, 1, NULL, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `assignatura`
--

CREATE TABLE `assignatura` (
  `codi` varchar(10) NOT NULL,
  `nom` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `assignatura`
--

INSERT INTO `assignatura` (`codi`, `nom`) VALUES
('ANG04', 'Anglès II'),
('BIO09', 'Biologia i Geologia I'),
('ECO13', 'Economia I'),
('EFI10', 'Educació Física'),
('FIL11', 'Filosofia'),
('FIS07', 'Física III'),
('FOL14', 'Formació i Orientació Laboral II'),
('HIS12', 'Història Universal'),
('INF06', 'Informàtica II'),
('LLC02', 'Llengua Catalana i Literatura'),
('LLC03', 'Llengua Castellana i Literatura'),
('MAT01', 'Matemàtiques II'),
('QUI08', 'Química I'),
('TEC05', 'Tecnologia II');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `assignatura_professor`
--

CREATE TABLE `assignatura_professor` (
  `codi_assig` varchar(10) NOT NULL,
  `id_intern_prof` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `assignatura_professor`
--

INSERT INTO `assignatura_professor` (`codi_assig`, `id_intern_prof`) VALUES
('ANG04', 'PROF0010'),
('BIO09', 'PROF0014'),
('EFI10', 'PROF0014'),
('FIL11', 'PROF0009'),
('FIS07', 'PROF0012'),
('FOL14', 'PROF0002'),
('HIS12', 'PROF0003'),
('INF06', 'PROF0023'),
('LLC02', 'PROF0009'),
('LLC03', 'PROF0005'),
('MAT01', 'PROF0008'),
('MAT01', 'PROF0012'),
('QUI08', 'PROF0007'),
('TEC05', 'PROF0001');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `centre`
--

CREATE TABLE `centre` (
  `id_centre` int(11) NOT NULL,
  `nom` varchar(150) NOT NULL,
  `direccio` varchar(255) DEFAULT NULL,
  `pla` set('nou','antic') NOT NULL,
  `logo` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `centre`
--

INSERT INTO `centre` (`id_centre`, `nom`, `direccio`, `pla`, `logo`) VALUES
(1, 'IES Joan Oró', 'Carrer de l\'Escola Industrial, 27 – 08980 Sant Feliu de Llobregat', 'nou', NULL),
(2, 'IES La Bastida', 'Avinguda de la Via Augusta, 101 – 08172 Sant Cugat del Vallès', 'nou', NULL),
(3, 'Institut Eugeni d\'Ors', 'Carrer de Sant Joan, 10 – 08720 Vilafranca del Penedès', 'nou', NULL),
(4, 'Universidad Santa María', 'Avenida de las Ciencias 404', 'antic', './uploads/logos/logo_centro_4.png'),
(5, 'Institut Thos i Codina', 'Carrer de l\'Església, 15 – 08301 Mataró', 'nou', NULL),
(6, 'IES Barri Besòs', 'Carrer d\'Alfons XII, 13 – 08911 Badalona', 'nou', NULL),
(7, 'Institut Vilatzara', 'Carrer Major, 56 – 08729 Viladecavalls', 'nou', NULL),
(8, 'IES Castellbisbal', 'Avinguda Pau Casals, 12 – 08755 Castellbisbal', 'nou', NULL),
(9, 'Institut Voltrera', 'Carrer del Sol, 8 – 08785 Vallirana', 'nou', NULL),
(10, 'Instituto Politécnico Evalis', 'Avenida de las Ciencias 404', '', NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `centre_administradors`
--

CREATE TABLE `centre_administradors` (
  `dni_administrador` varchar(9) NOT NULL,
  `id_centre` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `centre_administradors`
--

INSERT INTO `centre_administradors` (`dni_administrador`, `id_centre`) VALUES
('12345678A', 1),
('12345678A', 4),
('12345678A', 7),
('33445566G', 1),
('33445566G', 2),
('33445566G', 3),
('39728461T', 5),
('39728461T', 8),
('41350692Y', 2),
('41350692Y', 6),
('45092831K', 3),
('45092831K', 9),
('45092831K', 10),
('45678901X', 4),
('45678901X', 10),
('47210583M', 6),
('47210583M', 7),
('66778811U', 5),
('74185296H', 8),
('74185296H', 9),
('95135782L', 1),
('95135782L', 10);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `consentiments`
--

CREATE TABLE `consentiments` (
  `id` int(11) NOT NULL,
  `nia` int(11) NOT NULL,
  `tipus_consentiment` enum('contacte_parents','dreta_imatge','us_serveis_tercers','comunicacions_institucionals','altres') NOT NULL,
  `assunto` enum('Autorització de contacte amb pares/tutors (major d''edat)','Autorització de drets d''imatge (fotos, vídeos, web i xarxes)','Autorització ús de serveis educatius externs (Kahoot, Genially, etc.)','Rebre comunicacions del centre','Altres') NOT NULL,
  `informacio` text NOT NULL,
  `consent_fecha` datetime NOT NULL DEFAULT current_timestamp(),
  `consent_revocation` datetime DEFAULT NULL,
  `observacions` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `consentiments`
--

INSERT INTO `consentiments` (`id`, `nia`, `tipus_consentiment`, `assunto`, `informacio`, `consent_fecha`, `consent_revocation`, `observacions`) VALUES
(1, 313056, 'dreta_imatge', 'Autorització de drets d\'imatge (fotos, vídeos, web i xarxes)', 'He llegit i accepto que aparegui en fotos del centre...', '2025-09-10 09:15:00', NULL, NULL),
(2, 313056, 'us_serveis_tercers', 'Autorització ús de serveis educatius externs (Kahoot, Genially, etc.)', 'Autorització per crear comptes a plataformes educatives', '2025-09-10 09:15:00', NULL, NULL),
(3, 993346, 'dreta_imatge', 'Autorització de drets d\'imatge (fotos, vídeos, web i xarxes)', 'He llegit i accepto...', '2025-09-11 10:30:00', '2025-11-15 14:22:00', 'Revocat per mail'),
(4, 993346, 'contacte_parents', 'Autorització de contacte amb pares/tutors (major d\'edat)', 'Com sóc major d\'edat, autoritzo contacte amb pares', '2025-09-11 10:30:00', NULL, NULL),
(5, 970564, 'us_serveis_tercers', 'Autorització ús de serveis educatius externs (Kahoot, Genially, etc.)', 'Accepto crear comptes externs', '2025-09-12 11:11:00', NULL, NULL),
(6, 273885, 'dreta_imatge', 'Autorització de drets d\'imatge (fotos, vídeos, web i xarxes)', 'NO accepto sortir a fotos ni xarxes', '2025-09-10 08:45:00', NULL, 'Negativa expressa'),
(7, 727264, 'dreta_imatge', 'Autorització de drets d\'imatge (fotos, vídeos, web i xarxes)', 'Accepto només fotos de grup', '2025-09-13 12:00:00', NULL, 'Condicionat'),
(8, 234033, 'us_serveis_tercers', 'Autorització ús de serveis educatius externs (Kahoot, Genially, etc.)', 'Sí, accepto', '2025-09-10 09:20:00', NULL, NULL),
(9, 665021, 'contacte_parents', 'Autorització de contacte amb pares/tutors (major d\'edat)', 'Sí, podeu contactar amb la meva mare', '2025-09-15 10:10:00', NULL, NULL),
(10, 345805, 'dreta_imatge', 'Autorització de drets d\'imatge (fotos, vídeos, web i xarxes)', 'Accepto tot', '2025-09-10 09:18:00', NULL, NULL),
(11, 395902, 'us_serveis_tercers', 'Autorització ús de serveis educatius externs (Kahoot, Genially, etc.)', 'Accepto', '2025-09-14 13:33:00', NULL, NULL),
(12, 727471, 'dreta_imatge', 'Autorització de drets d\'imatge (fotos, vídeos, web i xarxes)', 'Accepto', '2025-09-10 09:25:00', NULL, NULL),
(13, 554569, 'contacte_parents', 'Autorització de contacte amb pares/tutors (major d\'edat)', 'Sí', '2025-09-16 11:11:00', NULL, NULL),
(14, 968618, 'us_serveis_tercers', 'Autorització ús de serveis educatius externs (Kahoot, Genially, etc.)', 'Accepto', '2025-09-10 09:30:00', NULL, NULL),
(15, 567540, 'dreta_imatge', 'Autorització de drets d\'imatge (fotos, vídeos, web i xarxes)', 'NO accepto', '2025-09-17 12:12:00', NULL, 'Negativa'),
(17, 151704, 'us_serveis_tercers', 'Autorització ús de serveis educatius externs (Kahoot, Genially, etc.)', 'Sí', '2025-09-18 14:44:00', NULL, NULL),
(18, 968100, 'contacte_parents', 'Autorització de contacte amb pares/tutors (major d\'edat)', 'Sí', '2025-09-10 09:40:00', NULL, NULL),
(19, 270477, 'dreta_imatge', 'Autorització de drets d\'imatge (fotos, vídeos, web i xarxes)', 'Accepto', '2025-09-19 15:55:00', NULL, NULL),
(20, 857629, 'us_serveis_tercers', 'Autorització ús de serveis educatius externs (Kahoot, Genially, etc.)', 'Accepto', '2025-09-10 09:45:00', NULL, NULL);

--
-- Disparadores `consentiments`
--
DELIMITER $$
CREATE TRIGGER `trg_consentiments_update` AFTER UPDATE ON `consentiments` FOR EACH ROW BEGIN
    IF OLD.consent_revocation IS NULL AND NEW.consent_revocation IS NOT NULL THEN
        INSERT INTO estudiants_historia (nia, nom_camp, valor_nou, valor_antic, descripcio)
        VALUES (NEW.nia, 'consentiment_revocat', NEW.tipus_consentiment, NULL,
                CONCAT('Revocat: ', NEW.assunto));
    END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contractes`
--

CREATE TABLE `contractes` (
  `id_intern_professor` varchar(20) NOT NULL,
  `id_centre` int(11) NOT NULL,
  `data_alta` date NOT NULL,
  `data_baixa` date DEFAULT NULL,
  `vinculacio_laboral` enum('substitut','interi','funcionari') NOT NULL,
  `dedicacio` enum('jornada_completa','mitja_jornada') NOT NULL DEFAULT 'jornada_completa',
  `descripcio` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `contractes`
--

INSERT INTO `contractes` (`id_intern_professor`, `id_centre`, `data_alta`, `data_baixa`, `vinculacio_laboral`, `dedicacio`, `descripcio`) VALUES
('PROF0001', 1, '2023-10-01', '2024-01-15', 'substitut', 'jornada_completa', 'Substitución maternidad 3 meses'),
('PROF0001', 2, '2024-09-01', '2025-06-30', 'interi', 'jornada_completa', 'Interina plaza sin definitivo'),
('PROF0001', 3, '2025-09-01', NULL, 'funcionari', 'jornada_completa', 'Plaza definitiva por oposición'),
('PROF0002', 3, '2025-02-01', NULL, 'interi', 'jornada_completa', NULL),
('PROF0002', 4, '2024-09-01', '2025-01-31', 'substitut', 'mitja_jornada', 'Substitución baja médica'),
('PROF0003', 3, '2020-09-01', NULL, 'funcionari', 'jornada_completa', NULL),
('PROF0004', 5, '2025-09-01', NULL, 'substitut', 'jornada_completa', 'Substitución excedencia'),
('PROF0015', 2, '2025-09-01', NULL, 'interi', 'mitja_jornada', NULL),
('PROF0017', 3, '2025-09-01', NULL, 'funcionari', 'jornada_completa', NULL),
('PROF0018', 3, '2025-09-01', NULL, 'interi', 'jornada_completa', NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `directiva`
--

CREATE TABLE `directiva` (
  `id_intern_professor` varchar(20) NOT NULL,
  `experiencia` int(11) NOT NULL DEFAULT 0 COMMENT 'Años de experiencia docente',
  `data_nomenament` date DEFAULT NULL,
  `carrec` varchar(80) DEFAULT NULL COMMENT 'Ej. Director/a, Vicedirector/a, Secretario/a, etc.'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `directiva`
--

INSERT INTO `directiva` (`id_intern_professor`, `experiencia`, `data_nomenament`, `carrec`) VALUES
('PROF0015', 19, '2023-09-01', 'Director/a'),
('PROF0016', 16, '2023-09-01', 'Vicedirector/a'),
('PROF0017', 22, '2023-09-01', 'Cap d\'estudis'),
('PROF0018', 14, '2024-01-15', 'Secretari/a'),
('PROF0019', 18, '2024-09-01', 'Coordinador/a de cicle'),
('PROF0020', 15, '2025-01-10', 'Cap de departament EF'),
('PROF0021', 20, '2023-09-01', 'Coordinador/a de convivència'),
('PROF0022', 13, '2025-01-10', 'Responsable de biblioteca'),
('PROF0023', 17, '2024-09-01', 'Coordinador/a TIC'),
('PROF0024', 12, '2025-01-10', 'Responsable d\'intercanvis');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `empresa_fct_practiques`
--

CREATE TABLE `empresa_fct_practiques` (
  `id_practica` int(11) NOT NULL,
  `nia` int(11) NOT NULL,
  `nom_empresa` varchar(150) NOT NULL,
  `mes_any_inici` varchar(7) NOT NULL COMMENT 'Formato YYYY-MM (ej. 2025-03)',
  `hores_total` int(11) NOT NULL CHECK (`hores_total` > 0),
  `hores_fetes` int(11) NOT NULL DEFAULT 0,
  `estudi` varchar(100) NOT NULL COMMENT 'Ciclo en el que hace/hizo las prácticas (DAM, DAW, ASIX, AF...)',
  `finalitzat` tinyint(1) NOT NULL DEFAULT 0 COMMENT 'TRUE = prácticas terminadas y validadas'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `empresa_fct_practiques`
--

INSERT INTO `empresa_fct_practiques` (`id_practica`, `nia`, `nom_empresa`, `mes_any_inici`, `hores_total`, `hores_fetes`, `estudi`, `finalitzat`) VALUES
(1, 313056, 'Everis - NTT Data', '2024-03', 384, 384, 'CFGS Desenvolupament d\'Aplicacions Multiplataforma (DAM)', 1),
(2, 993346, 'Capgemini Engineering', '2024-03', 384, 384, 'CFGS Desenvolupament d\'Aplicacions Multiplataforma (DAM)', 1),
(3, 970564, 'CaixaBank Tech', '2024-03', 400, 400, 'CFGS Administració i Finances (AF)', 1),
(4, 665021, 'IBM España', '2024-03', 416, 416, 'CFGS Desenvolupament d\'Aplicacions Multiplataforma (DAM)', 1),
(5, 727264, 'Accenture España', '2025-03', 384, 280, 'CFGS Desenvolupament d\'Aplicacions Web (DAW)', 0),
(6, 151704, 'T-Systems Iberia', '2025-03', 384, 384, 'CFGS Desenvolupament d\'Aplicacions Multiplataforma (DAM)', 1),
(7, 273885, 'SEAT CODE (Volkswagen Group)', '2025-03', 370, 150, 'CFGS Desenvolupament d\'Aplicacions Web (DAW)', 0),
(8, 345805, 'Deloitte Digital', '2024-03', 384, 384, 'CFGS Administració i Finances (AF)', 1),
(9, 554569, 'Indra Sistemas', '2025-03', 384, 320, 'CFGS Desenvolupament d\'Aplicacions Multiplataforma (DAM)', 0),
(10, 968618, 'Freelance - Desarrollador autónomo', '2025-01', 370, 370, 'CFGS Desenvolupament d\'Aplicacions Multiplataforma (DAM)', 1),
(11, 567540, 'HP SCDS Barcelona', '2025-03', 400, 95, 'CFGS Desenvolupament d\'Aplicacions Web (DAW)', 0),
(13, 313056, 'Everis - NTT Data', '2024-03', 384, 384, 'CFGS Desenvolupament d\'Aplicacions Multiplataforma (DAM)', 1),
(14, 993346, 'Capgemini Engineering', '2024-03', 384, 384, 'CFGS Desenvolupament d\'Aplicacions Multiplataforma (DAM)', 1),
(15, 970564, 'CaixaBank Tech', '2024-03', 400, 400, 'CFGS Administració i Finances (AF)', 1),
(16, 665021, 'IBM España', '2024-03', 416, 416, 'CFGS Desenvolupament d\'Aplicacions Multiplataforma (DAM)', 1),
(17, 727264, 'Accenture España', '2025-03', 384, 280, 'CFGS Desenvolupament d\'Aplicacions Web (DAW)', 0),
(18, 151704, 'T-Systems Iberia', '2025-03', 384, 384, 'CFGS Desenvolupament d\'Aplicacions Multiplataforma (DAM)', 1),
(19, 273885, 'SEAT CODE (Volkswagen Group)', '2025-03', 370, 150, 'CFGS Desenvolupament d\'Aplicacions Web (DAW)', 0),
(20, 345805, 'Deloitte Digital', '2024-03', 384, 384, 'CFGS Administració i Finances (AF)', 1),
(21, 554569, 'Indra Sistemas', '2025-03', 384, 320, 'CFGS Desenvolupament d\'Aplicacions Multiplataforma (DAM)', 0),
(22, 968618, 'Freelance - Desarrollador autónomo', '2025-01', 370, 370, 'CFGS Desenvolupament d\'Aplicacions Multiplataforma (DAM)', 1),
(23, 567540, 'HP SCDS Barcelona', '2025-03', 400, 95, 'CFGS Desenvolupament d\'Aplicacions Web (DAW)', 0);

--
-- Disparadores `empresa_fct_practiques`
--
DELIMITER $$
CREATE TRIGGER `trg_fct_update` AFTER UPDATE ON `empresa_fct_practiques` FOR EACH ROW BEGIN
    IF OLD.nom_empresa <> NEW.nom_empresa THEN
        INSERT INTO estudiants_historia (nia, nom_camp, valor_nou, valor_antic, descripcio)
        VALUES (NEW.nia, 'empresa_fct', NEW.nom_empresa, OLD.nom_empresa, 'Canvi d''empresa de pràctiques');
    END IF;
    
    IF OLD.finalitzat = 0 AND NEW.finalitzat = 1 THEN
        INSERT INTO estudiants_historia (nia, nom_camp, valor_nou, valor_antic, descripcio)
        VALUES (NEW.nia, 'fct_finalitzada', 'Sí', 'No', 'Pràctiques FCT validades i finalitzades');
    END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `estado_evaluacion`
--

CREATE TABLE `estado_evaluacion` (
  `id` int(11) NOT NULL DEFAULT 1,
  `periodo_abierto` tinyint(1) NOT NULL DEFAULT 0,
  `ultima_modificacion` datetime DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `modificado_por` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `estado_evaluacion`
--

INSERT INTO `estado_evaluacion` (`id`, `periodo_abierto`, `ultima_modificacion`, `modificado_por`) VALUES
(1, 1, '2026-05-22 11:56:24', NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `estudiants`
--

CREATE TABLE `estudiants` (
  `nia` int(11) NOT NULL,
  `dni_persona` varchar(9) NOT NULL,
  `ex_alumne` tinyint(1) NOT NULL,
  `treballa` tinyint(1) NOT NULL,
  `empresa_treball` varchar(100) DEFAULT NULL,
  `repetidor` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `estudiants`
--

INSERT INTO `estudiants` (`nia`, `dni_persona`, `ex_alumne`, `treballa`, `empresa_treball`, `repetidor`) VALUES
(126440, '98765432W', 0, 0, NULL, 0),
(151704, '36925814E', 0, 0, NULL, 0),
(234033, '22334411R', 0, 1, NULL, 0),
(270477, '55667788F', 0, 0, NULL, 0),
(273885, '13579246A', 0, 0, 'Empresa 29', 0),
(313056, '11223344E', 0, 0, NULL, 0),
(345805, '24680246Q', 0, 1, 'Empresa 26', 0),
(395902, '24681357C', 0, 1, NULL, 0),
(430700, '80246802W', 0, 0, NULL, 0),
(439078, '96385274G', 0, 1, NULL, 0),
(458305, '79135791V', 0, 1, NULL, 1),
(554569, '25896314I', 0, 0, 'Empresa 62', 0),
(567540, '34567890M', 0, 0, NULL, 0),
(603640, '99887722P', 0, 1, NULL, 0),
(665021, '22334455I', 0, 0, NULL, 0),
(727264, '21098765Y', 0, 0, NULL, 0),
(727471, '25123456K', 0, 0, NULL, 0),
(807566, '82468013M', 0, 1, 'Empresa 70', 0),
(857629, '57913579T', 0, 1, NULL, 0),
(968100, '46802468S', 0, 1, NULL, 0),
(968618, '33445599T', 0, 0, NULL, 0),
(970564, '13579136Z', 0, 0, NULL, 0),
(993346, '13579135P', 0, 1, 'Empresa 15', 0);

--
-- Disparadores `estudiants`
--
DELIMITER $$
CREATE TRIGGER `trg_alta_alumne_crea_usuari` AFTER INSERT ON `estudiants` FOR EACH ROW BEGIN
    DECLARE v_username VARCHAR(50);
    DECLARE v_base_user VARCHAR(40);
    DECLARE v_contador INT DEFAULT 1;
    DECLARE v_edat INT;
    
    -- Obtener datos de persona
    SELECT LOWER(CONCAT(
        REGEXP_REPLACE(nom, '[^a-z]', ''),
        '.',
        REGEXP_REPLACE(SUBSTRING_INDEX(cognom, ' ', 1), '[^a-z]', '')
    )), 
    TIMESTAMPDIFF(YEAR, data_naix, CURDATE())
    INTO v_base_user, v_edat
    FROM persona WHERE dni = NEW.dni_persona;

    SET v_username = v_base_user;

    -- Evitar colisión de username
    WHILE EXISTS(SELECT 1 FROM usuaris WHERE username = v_username) DO
        SET v_username = CONCAT(v_base_user, v_contador);
        SET v_contador = v_contador + 1;
    END WHILE;

    -- Crear usuario
    INSERT INTO usuaris (username, password, data_alta, dni_persona, edat)
    VALUES (
        v_username,
        '$2y$10$8Qz9jY5fZfZfZfZfZfZfZu9jY5fZfZfZfZfZfZfZfZfZfZfZfZfZfZfZ', -- centre2025
        CURDATE(),
        NEW.dni_persona,
        v_edat
    );

    -- Auditoría
    INSERT INTO estudiants_historia (nia, nom_camp, valor_nou, valor_antic, descripcio)
    VALUES (NEW.nia, 'usuari_creat', v_username, NULL, 
            CONCAT('Usuari creat automàticament: ', v_username));
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_baixa_alumne` AFTER UPDATE ON `estudiants` FOR EACH ROW BEGIN
    IF OLD.ex_alumne = 0 AND NEW.ex_alumne = 1 THEN
        
        -- Marcar como ex-alumne en persona
        UPDATE persona p 
        JOIN estudiants e ON p.dni = e.dni_persona 
        SET p.rol = 'ex-alumne' 
        WHERE e.nia = NEW.nia;

        -- Eliminar del grupo
        DELETE FROM estudiants_grupclasse WHERE nia = NEW.nia;

        -- Cerrar estudios activos
        UPDATE estudis SET status = 'abandona', curs_fi = CURDATE() 
        WHERE nia = NEW.nia AND status = 'actiu';

        -- Auditoría
        INSERT INTO estudiants_historia (nia, nom_camp, valor_nou, valor_antic, descripcio)
        VALUES (NEW.nia, 'baixa_centre', 'ex-alumne', 'actiu', 'Baixa del centre (motiu no especificat)');
    END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `estudiants_grupclasse`
--

CREATE TABLE `estudiants_grupclasse` (
  `nia` int(11) NOT NULL,
  `nom_grup` varchar(20) NOT NULL,
  `anyo` year(4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `estudiants_grupclasse`
--

INSERT INTO `estudiants_grupclasse` (`nia`, `nom_grup`, `anyo`) VALUES
(126440, '1r BATX A', '2025'),
(151704, '1r BATX A', '2025'),
(151704, 'DAW 1', '0000'),
(234033, '1r BATX A', '2025'),
(234033, '2n ESO A', '0000'),
(270477, '1r BATX A', '2025'),
(270477, '1r ESO B', '0000'),
(273885, '1r BATX A', '2025'),
(273885, 'DAW 2', '0000'),
(313056, '1r BATX A', '2025'),
(313056, '1r ESO A', '0000'),
(345805, '1r BATX A', '2025'),
(345805, 'DAM 1', '0000'),
(395902, '1r BATX A', '2025'),
(395902, 'DAM 1', '0000'),
(430700, '1r BATX A', '2025'),
(439078, '1r BATX A', '2025'),
(458305, '1r BATX A', '2025'),
(554569, '1r BATX A', '2025'),
(554569, 'DAW 2', '0000'),
(567540, '1r BATX A', '0000'),
(603640, '1r BATX A', '2025'),
(665021, '1r BATX A', '2025'),
(727264, '1r BATX A', '2025'),
(727471, '1r BATX A', '2025'),
(807566, '1r BATX A', '2025'),
(857629, '1r BATX A', '2025'),
(968100, '1r BATX A', '2025'),
(968618, '1r BATX A', '2025'),
(970564, '1r BATX A', '2025'),
(993346, '1r BATX A', '2025');

--
-- Disparadores `estudiants_grupclasse`
--
DELIMITER $$
CREATE TRIGGER `trg_grupclasse_delete` AFTER DELETE ON `estudiants_grupclasse` FOR EACH ROW BEGIN
    INSERT INTO estudiants_historia (nia, nom_camp, valor_nou, valor_antic, descripcio)
    VALUES (OLD.nia, 'grup_classe', NULL, OLD.nom_grup,
            CONCAT('Eliminat del grup ', OLD.nom_grup));
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_grupclasse_insert` AFTER INSERT ON `estudiants_grupclasse` FOR EACH ROW BEGIN
    INSERT INTO estudiants_historia (nia, nom_camp, valor_nou, valor_antic, descripcio)
    VALUES (NEW.nia, 'grup_classe', NEW.nom_grup, NULL,
            CONCAT('Assignat al grup ', NEW.nom_grup, ' (', NEW.anyo, ')'));
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_grupclasse_update` AFTER UPDATE ON `estudiants_grupclasse` FOR EACH ROW BEGIN
    INSERT INTO estudiants_historia (nia, nom_camp, valor_nou, valor_antic, descripcio)
    VALUES (NEW.nia, 'grup_classe', NEW.nom_grup, OLD.nom_grup,
            CONCAT('Canvi de grup: ', OLD.nom_grup, ' → ', NEW.nom_grup));
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `estudiants_historia`
--

CREATE TABLE `estudiants_historia` (
  `id` int(11) NOT NULL,
  `nia` int(11) NOT NULL,
  `nom_camp` varchar(50) NOT NULL,
  `valor_nou` varchar(255) NOT NULL,
  `valor_antic` varchar(255) DEFAULT NULL,
  `data_canvi` datetime NOT NULL DEFAULT current_timestamp(),
  `descripcio` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `estudiants_historia`
--

INSERT INTO `estudiants_historia` (`id`, `nia`, `nom_camp`, `valor_nou`, `valor_antic`, `data_canvi`, `descripcio`) VALUES
(1, 313056, 'grup', '1ESO-B', '1ESO-A', '2025-10-15 12:30:00', 'Canvi de grup per problemes greus de convivència (bullying detectat).'),
(2, 313056, 'genere', 'Femení', 'Masculí', '2025-11-05 09:15:00', 'Canvi de gènere i nom social solicitat per l\'alumne i família.'),
(3, 993346, 'nom_complet', 'Anna Rodríguez Castro', 'Alejandro Rodríguez Castro', '2025-11-05 09:15:00', 'Actualització de nom social segons solicitud familiar.'),
(4, 970564, 'nota_M9', '7.5', '5.0', '2025-11-20 14:22:00', 'Reclamació acceptada: error en correcció examen pràctic UF2.'),
(5, 273885, 'grup', '1ESO-C', '1ESO-B', '2025-09-25 10:00:00', 'Canvi de grup per millorar dinàmica i evitar conflictes.'),
(6, 727264, 'contacte_parents', 'Denegat', 'Autoritzat', '2025-11-10 11:11:00', 'Revocació consentiment contacte pares (major d\'edat).'),
(7, 234033, 'adaptacio_curricular', 'Exempt treball final', NULL, '2025-10-30 13:45:00', 'Adaptació significativa per TDAH sever. Aprovat per EAP.'),
(8, 665021, 'genere', 'Masculí', 'Femení', '2025-11-12 10:30:00', 'Canvi de gènere i nom social. Nou nom: Marc Escudero Vila.'),
(9, 395902, 'nota_INF06', '9.0', '4.5', '2025-11-25 16:10:00', 'Error en càlcul nota final. Recàlcul correcte.'),
(10, 554569, 'mesures_neae', 'Pla individualitzat', NULL, '2025-10-05 11:20:00', 'Activació mesures extraordinàries per NESE.'),
(11, 968618, 'dreta_imatge', 'Autoritzat', 'Denegat', '2025-11-08 09:55:00', 'Canvi d\'opinió: ara sí autoritza drets d\'imatge.'),
(12, 567540, 'mesura_disciplinaria', 'Expulsió 3 dies', NULL, '2025-11-21 13:00:00', 'Incident greu al pati. Acta signada.'),
(14, 151704, 'avaluacio_final', 'Aprovat per compensació', 'Suspès', '2025-11-28 15:30:00', 'Aplicada compensació curricular segons normativa.'),
(15, 968100, 'domicili', 'C/ Nou 45, Manacor', 'Carrer Major 12, Manacor', '2025-11-01 10:00:00', 'Actualització adreça i telèfon de contacte.'),
(16, 270477, 'exempcio_ef', 'Exempt educació física', NULL, '2025-09-30 09:40:00', 'Justificant mèdic per lesió genoll. Exempció 3 mesos.'),
(17, 857629, 'promocio', 'Promoció excepcional', 'Repetidor', '2025-11-27 17:00:00', 'Promoció amb avaluació positiva global malgrat suspens.'),
(18, 458305, 'nota_M12', '8.0', '6.5', '2025-11-26 11:11:00', 'Revisió examen: error en pregunta. Nota ajustada.'),
(19, 430700, 'suport', 'Suport intensiu PT', NULL, '2025-10-10 10:10:00', 'Derivació a PT per dificultats greus d\'aprenentatge.'),
(20, 345805, 'grup', 'DAM 1B', 'DAM 1A', '2025-11-18 10:00:00', 'Canvi de grup per petició familiar i millor adequació horària.'),
(21, 313056, 'nom', 'Anna Castillejo', 'Anna', '2025-11-30 16:04:52', 'Canvi de nom personal'),
(45, 313056, 'grup_classe', '1r BATX A', NULL, '2026-05-06 13:01:41', 'Assignat al grup 1r BATX A (2025)'),
(46, 993346, 'grup_classe', '1r BATX A', NULL, '2026-05-06 13:01:41', 'Assignat al grup 1r BATX A (2025)'),
(47, 970564, 'grup_classe', '1r BATX A', NULL, '2026-05-06 13:01:41', 'Assignat al grup 1r BATX A (2025)'),
(48, 273885, 'grup_classe', '1r BATX A', NULL, '2026-05-06 13:01:41', 'Assignat al grup 1r BATX A (2025)'),
(49, 727264, 'grup_classe', '1r BATX A', NULL, '2026-05-06 13:01:41', 'Assignat al grup 1r BATX A (2025)'),
(50, 234033, 'grup_classe', '1r BATX A', NULL, '2026-05-06 13:01:41', 'Assignat al grup 1r BATX A (2025)'),
(51, 665021, 'grup_classe', '1r BATX A', NULL, '2026-05-06 13:01:41', 'Assignat al grup 1r BATX A (2025)'),
(52, 345805, 'grup_classe', '1r BATX A', NULL, '2026-05-06 13:01:41', 'Assignat al grup 1r BATX A (2025)'),
(53, 395902, 'grup_classe', '1r BATX A', NULL, '2026-05-06 13:01:41', 'Assignat al grup 1r BATX A (2025)'),
(54, 727471, 'grup_classe', '1r BATX A', NULL, '2026-05-06 13:01:41', 'Assignat al grup 1r BATX A (2025)'),
(55, 554569, 'grup_classe', '1r BATX A', NULL, '2026-05-06 13:01:41', 'Assignat al grup 1r BATX A (2025)'),
(56, 968618, 'grup_classe', '1r BATX A', NULL, '2026-05-06 13:01:41', 'Assignat al grup 1r BATX A (2025)'),
(57, 151704, 'grup_classe', '1r BATX A', NULL, '2026-05-06 13:01:41', 'Assignat al grup 1r BATX A (2025)'),
(58, 968100, 'grup_classe', '1r BATX A', NULL, '2026-05-06 13:01:41', 'Assignat al grup 1r BATX A (2025)'),
(59, 270477, 'grup_classe', '1r BATX A', NULL, '2026-05-06 13:01:41', 'Assignat al grup 1r BATX A (2025)'),
(60, 857629, 'grup_classe', '1r BATX A', NULL, '2026-05-06 13:01:41', 'Assignat al grup 1r BATX A (2025)'),
(61, 458305, 'grup_classe', '1r BATX A', NULL, '2026-05-06 13:01:41', 'Assignat al grup 1r BATX A (2025)'),
(62, 430700, 'grup_classe', '1r BATX A', NULL, '2026-05-06 13:01:41', 'Assignat al grup 1r BATX A (2025)'),
(63, 807566, 'grup_classe', '1r BATX A', NULL, '2026-05-06 13:01:41', 'Assignat al grup 1r BATX A (2025)'),
(64, 439078, 'grup_classe', '1r BATX A', NULL, '2026-05-06 13:01:41', 'Assignat al grup 1r BATX A (2025)'),
(65, 126440, 'grup_classe', '1r BATX A', NULL, '2026-05-06 13:01:41', 'Assignat al grup 1r BATX A (2025)'),
(66, 603640, 'grup_classe', '1r BATX A', NULL, '2026-05-06 13:01:41', 'Assignat al grup 1r BATX A (2025)'),
(67, 313056, 'nota_UF_21', '9.00', '10.00', '2026-05-22 11:00:31', 'Modificació de nota: 10.00 → 9.00 | UF: Comunicació professional escrita (LLC02)'),
(68, 313056, 'nota_UF_21', '10.00', '9.00', '2026-05-22 11:03:51', 'Modificació de nota: 9.00 → 10.00 | UF: Comunicació professional escrita (LLC02)'),
(69, 313056, 'nota_UF_22', '10.00', '5.00', '2026-05-22 11:31:37', 'Modificació de nota: 5.00 → 10.00 | UF: Comunicació oral en català (LLC02)');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `estudios_centre`
--

CREATE TABLE `estudios_centre` (
  `id` int(11) NOT NULL,
  `id_centre` int(11) NOT NULL,
  `nom_estudio` varchar(150) NOT NULL,
  `curso` varchar(9) DEFAULT '2025-2026'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `estudios_centre`
--

INSERT INTO `estudios_centre` (`id`, `id_centre`, `nom_estudio`, `curso`) VALUES
(1, 1, 'CFGS Desarrollo de Aplicaciones Multiplataforma (DAM)', '2025-2026'),
(2, 1, 'CFGS Desarrollo de Aplicaciones Web (DAW)', '2025-2026'),
(3, 2, 'CFGS Administracion de Sistemas Informaticos en Red (ASIR)', '2025-2026'),
(4, 2, 'CFGS Automatizacion y Robotica Industrial', '2025-2026'),
(5, 3, 'CFGS Administracion y Finanzas', '2025-2026'),
(6, 3, 'CFGS Asistencia a la Direccion', '2025-2026'),
(9, 5, 'CFGS Marketing y Publicidad', '2025-2026'),
(10, 5, 'CFGS Gestion de Ventas y Espacios Comerciales', '2025-2026'),
(11, 6, 'CFGS Laboratorio Clinico y Biomedico', '2025-2026'),
(12, 6, 'CFGS Anatomia Patologica y Citodiagnostico', '2025-2026'),
(13, 7, 'CFGS Imagen para el Diagnostico y Medicina Nuclear', '2025-2026'),
(14, 7, 'CFGS Dietetica', '2025-2026'),
(15, 8, 'CFGS Educacion Infantil', '2025-2026'),
(16, 8, 'CFGS Integracion Social', '2025-2026'),
(17, 9, 'CFGS Animacion Sociocultural y Turistica', '2025-2026'),
(18, 9, 'CFGS Guiado, Informacion y Asistencias Turisticas', '2025-2026'),
(21, 10, 'Desarrollo de Aplicaciones Multiplataforma', '2025-2026'),
(22, 10, 'Desarrollo de Aplicaciones Web', '2025-2026'),
(23, 10, 'Administración de Sistemas Informáticos', '2025-2026'),
(24, 10, 'Automatización y Robótica Industrial', '2025-2026'),
(29, 4, 'Desarrollo de Aplicaciones Multiplataforma', '2025-2026'),
(30, 4, 'Desarrollo de Aplicaciones Web', '2025-2026'),
(31, 4, 'Administración de Sistemas Informáticos', '2025-2026'),
(32, 4, 'Automatización y Robótica Industrial', '2025-2026'),
(33, 4, 'Derechos Humanos', '2025-2026');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `estudis`
--

CREATE TABLE `estudis` (
  `nia` int(11) NOT NULL,
  `nom_estudi` varchar(150) NOT NULL,
  `curs_inici` date NOT NULL,
  `status` enum('actiu','finalitzat','suspendit','abandonat') NOT NULL DEFAULT 'finalitzat',
  `curs_fi` date DEFAULT NULL,
  `nota_final` float DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `estudis`
--

INSERT INTO `estudis` (`nia`, `nom_estudi`, `curs_inici`, `status`, `curs_fi`, `nota_final`) VALUES
(151704, 'CFGS Desenvolupament d\'Aplicacions Multiplataforma (DAM)', '2023-09-15', 'finalitzat', '2025-06-20', 8.15),
(234033, 'CFGS Desenvolupament d\'Aplicacions Multiplataforma (DAM)', '2023-09-15', 'finalitzat', '2025-06-20', 7.45),
(273885, 'CFGM Sistemes Microinformàtics i Xarxes (SMX)', '2021-09-15', 'finalitzat', '2023-06-20', 6.85),
(273885, 'CFGS Desenvolupament d\'Aplicacions Web (DAW)', '2023-09-15', 'finalitzat', '2025-06-20', 7.3),
(313056, 'Batxillerat Científic', '2020-09-01', 'finalitzat', '2022-06-15', 7.8),
(313056, 'CFGS Desenvolupament d\'Aplicacions Multiplataforma (DAM)', '2022-09-15', 'finalitzat', '2024-06-20', 8.75),
(313056, 'Educació Secundària Obligatòria (ESO)', '2016-09-01', 'finalitzat', '2020-06-20', 7.2),
(345805, 'CFGS Administració i Finances (AF)', '2023-09-15', 'finalitzat', '2025-06-20', 8.05),
(395902, 'CFGS Desenvolupament d\'Aplicacions Web (DAW)', '2023-09-15', 'finalitzat', '2025-06-20', 7.6),
(554569, 'CFGS Desenvolupament d\'Aplicacions Multiplataforma (DAM)', '2023-09-15', 'finalitzat', '2025-06-20', 6.95),
(567540, 'CFGS Desenvolupament d\'Aplicacions Web (DAW)', '2023-09-15', 'finalitzat', '2025-06-20', 7.7),
(665021, 'CFGS Desenvolupament d\'Aplicacions Multiplataforma (DAM)', '2023-09-15', 'finalitzat', '2025-06-20', 9.3),
(727264, 'CFGS Desenvolupament d\'Aplicacions Multiplataforma (DAM)', '2023-09-15', 'finalitzat', '2025-06-20', 8.4),
(727264, 'Educació Secundària Obligatòria (ESO)', '2017-09-01', 'finalitzat', '2021-06-20', 8.2),
(727471, 'CFGS Desenvolupament d\'Aplicacions Multiplataforma (DAM)', '2023-09-15', 'finalitzat', '2025-06-20', 8.4),
(968618, 'CFGS Desenvolupament d\'Aplicacions Multiplataforma (DAM)', '2023-09-15', 'finalitzat', '2025-06-20', 9),
(970564, 'Batxillerat Humanístic', '2020-09-01', 'finalitzat', '2022-06-15', 8.1),
(970564, 'CFGS Administració i Finances (AF)', '2022-09-15', 'finalitzat', '2024-06-20', 9.1),
(993346, 'Batxillerat Tecnològic', '2020-09-01', 'finalitzat', '2022-06-15', 7.4),
(993346, 'CFGS Desenvolupament d\'Aplicacions Multiplataforma (DAM)', '2022-09-15', 'finalitzat', '2024-06-20', 7.9),
(993346, 'Educació Secundària Obligatòria (ESO)', '2016-09-01', 'finalitzat', '2020-06-20', 6.9);

--
-- Disparadores `estudis`
--
DELIMITER $$
CREATE TRIGGER `trg_estudis_update` AFTER UPDATE ON `estudis` FOR EACH ROW BEGIN
    IF OLD.status <> NEW.status THEN
        INSERT INTO estudiants_historia (nia, nom_camp, valor_nou, valor_antic, descripcio)
        VALUES (NEW.nia, 'estat_cicle', NEW.status, OLD.status,
                CONCAT('Canvi d''estat del cicle: ', OLD.status, ' → ', NEW.status));
    END IF;
    
    IF (OLD.nota_final IS NULL OR OLD.nota_final <> NEW.nota_final) AND NEW.nota_final IS NOT NULL THEN
        INSERT INTO estudiants_historia (nia, nom_camp, valor_nou, valor_antic, descripcio)
        VALUES (NEW.nia, 'nota_final_cicle', NEW.nota_final, OLD.nota_final, 'Nota final del cicle assignada');
    END IF;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_graduacio_alumne` AFTER UPDATE ON `estudis` FOR EACH ROW BEGIN
    IF OLD.status = 'actiu' AND NEW.status = 'finalitzat' AND NEW.curs_fi IS NOT NULL THEN
        
        -- Marcar como ex-alumne
        UPDATE estudiants SET ex_alumne = 1 WHERE nia = NEW.nia;
        UPDATE persona p 
        JOIN estudiants e ON p.dni = e.dni_persona 
        SET p.rol = 'ex-alumne' 
        WHERE e.nia = NEW.nia;

        -- Eliminar del grupo
        DELETE FROM estudiants_grupclasse WHERE nia = NEW.nia;

        -- Auditoría
        INSERT INTO estudiants_historia (nia, nom_camp, valor_nou, valor_antic, descripcio)
        VALUES (
            NEW.nia,
            'graduacio',
            CONCAT(NEW.nom_estudi, ' - Titulat'),
            'Estudiant actiu',
            CONCAT('Graduació: ', NEW.nom_estudi,
                   ' | Nota final: ', IFNULL(NEW.nota_final, 'sense nota'),
                   ' | Data: ', NEW.curs_fi)
        );
    END IF;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_promocio_curs` AFTER UPDATE ON `estudis` FOR EACH ROW BEGIN
    IF OLD.status = 'actiu' AND NEW.status = 'actiu'
       AND OLD.curs_fi IS NULL AND NEW.curs_fi IS NULL
       AND YEAR(CURDATE()) > YEAR(OLD.curs_inici) THEN

        UPDATE estudiants_grupclasse gc
        JOIN pla_estudis pe_old ON gc.nom_grup LIKE CONCAT('%1r ', pe_old.tipus, '%') AND pe_old.curs = 1
        JOIN pla_estudis pe_new ON pe_new.tipus = pe_old.tipus AND pe_new.curs = 2 AND pe_new.pla = pe_old.pla
        SET gc.nom_grup = pe_new.curs_num,
            gc.anyo = YEAR(CURDATE())
        WHERE gc.nia = NEW.nia;

        INSERT INTO estudiants_historia (nia, nom_camp, valor_nou, valor_antic, descripcio)
        VALUES (NEW.nia, 'promocio_curs', '2n curs', '1r curs', 'Promoció automàtica a 2n curs');
    END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `gestio_absencies`
--

CREATE TABLE `gestio_absencies` (
  `id_absencia` int(11) NOT NULL,
  `id_intern_professor` varchar(20) NOT NULL,
  `motiu` varchar(255) NOT NULL,
  `data` date NOT NULL,
  `descripcio` text DEFAULT NULL,
  `justificat` tinyint(1) NOT NULL DEFAULT 0,
  `link_document` varchar(500) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `gestio_absencies`
--

INSERT INTO `gestio_absencies` (`id_absencia`, `id_intern_professor`, `motiu`, `data`, `descripcio`, `justificat`, `link_document`) VALUES
(1, 'PROF0001', 'Absència', '2025-03-12', 'Retencions tràfic AP-7', 0, NULL),
(2, 'PROF0001', 'Permís paternitat', '2025-04-01', 'Naixement fill/a (16 setmanes)', 1, '/docs/2025_permís_paternitat_PROF0001.pdf'),
(3, 'PROF0001', 'Absència', '2025-05-20', 'Cita mèdica especialista', 1, '/docs/2025_cita_medica_PROF0001.pdf'),
(4, 'PROF0002', 'Baixa mèdica', '2025-02-10', 'Fractura braç esquí (fins 15/04)', 1, '/docs/2025_baixa_medica_PROF0002.pdf'),
(5, 'PROF0002', 'Absència', '2025-01-15', 'Avaria cotxe - grua', 0, NULL),
(6, 'PROF0003', 'Permís matrimoni', '2025-06-10', 'Casament propi (15 dies)', 1, '/docs/2025_casament_PROF0003.pdf'),
(7, 'PROF0003', 'Absència', '2025-11-05', 'Defunció avi', 1, '/docs/2025_defuncio_PROF0003.pdf'),
(8, 'PROF0015', 'Absència', '2025-10-22', 'Examen teòric conduir', 1, '/docs/2025_examen_conduir_PROF0015.pdf'),
(9, 'PROF0017', 'Baixa mèdica', '2025-09-18', 'Operació genoll (recuperació 3 mesos)', 1, '/docs/2025_baixa_genoll_PROF0017.pdf'),
(10, 'PROF0018', 'Absència', '2025-04-03', 'Retard 30 min - avaria tren', 0, NULL),
(11, 'PROF0004', 'Permís maternitat', '2024-12-20', 'Naixement fill/a (16 setmanes)', 1, '/docs/2024_maternitat_PROF0004.pdf'),
(12, 'PROF0005', 'Absència', '2025-03-05', 'Cita dentista urgent', 1, '/docs/2025_dentista_PROF0005.docx'),
(13, 'PROF0016', 'Absència', '2025-05-15', 'Reunió escolar fill (ESO)', 1, '/docs/2025_reunio_escolar_PROF0016.pdf'),
(14, 'PROF0001', 'Absència', '2025-11-11', 'Visita veterinari gos malalt', 0, NULL),
(15, 'PROF0020', 'Permís', '2025-06-02', 'Examen oposicions mestre (2 dies)', 1, '/docs/2025_oposicions_PROF0020.pdf');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `grupclasse_assignatura`
--

CREATE TABLE `grupclasse_assignatura` (
  `nom_grupclasse` varchar(20) NOT NULL,
  `codi_assignatura` varchar(10) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `grupclasse_assignatura`
--

INSERT INTO `grupclasse_assignatura` (`nom_grupclasse`, `codi_assignatura`) VALUES
('1r BATX A', 'ANG04'),
('1r BATX A', 'BIO09'),
('1r BATX A', 'ECO13'),
('1r BATX A', 'EFI10'),
('1r BATX A', 'FIL11'),
('1r BATX A', 'FIS07'),
('1r BATX A', 'HIS12'),
('1r BATX A', 'LLC02'),
('1r BATX A', 'LLC03'),
('1r BATX A', 'MAT01'),
('1r BATX A', 'QUI08'),
('1r BATX A', 'TEC05'),
('1r ESO A', 'ANG04'),
('1r ESO A', 'BIO09'),
('1r ESO A', 'EFI10'),
('1r ESO A', 'FIS07'),
('1r ESO A', 'HIS12'),
('1r ESO A', 'LLC02'),
('1r ESO A', 'LLC03'),
('1r ESO A', 'MAT01'),
('1r ESO A', 'QUI08'),
('1r ESO A', 'TEC05'),
('1r ESO B', 'ANG04'),
('1r ESO B', 'BIO09'),
('1r ESO B', 'EFI10'),
('1r ESO B', 'FIS07'),
('1r ESO B', 'HIS12'),
('1r ESO B', 'LLC02'),
('1r ESO B', 'LLC03'),
('1r ESO B', 'MAT01'),
('1r ESO B', 'QUI08'),
('1r ESO B', 'TEC05'),
('2n BATX A', 'ANG04'),
('2n BATX A', 'BIO09'),
('2n BATX A', 'ECO13'),
('2n BATX A', 'EFI10'),
('2n BATX A', 'FIL11'),
('2n BATX A', 'FIS07'),
('2n BATX A', 'HIS12'),
('2n BATX A', 'LLC02'),
('2n BATX A', 'LLC03'),
('2n BATX A', 'MAT01'),
('2n BATX A', 'QUI08'),
('2n BATX A', 'TEC05'),
('2n ESO A', 'ANG04'),
('2n ESO A', 'BIO09'),
('2n ESO A', 'EFI10'),
('2n ESO A', 'FIS07'),
('2n ESO A', 'HIS12'),
('2n ESO A', 'LLC02'),
('2n ESO A', 'LLC03'),
('2n ESO A', 'MAT01'),
('2n ESO A', 'QUI08'),
('2n ESO A', 'TEC05'),
('3r ESO A', 'ANG04'),
('3r ESO A', 'BIO09'),
('3r ESO A', 'EFI10'),
('3r ESO A', 'FIS07'),
('3r ESO A', 'HIS12'),
('3r ESO A', 'LLC02'),
('3r ESO A', 'LLC03'),
('3r ESO A', 'MAT01'),
('3r ESO A', 'QUI08'),
('3r ESO A', 'TEC05'),
('4t ESO A', 'ANG04'),
('4t ESO A', 'BIO09'),
('4t ESO A', 'EFI10'),
('4t ESO A', 'FIS07'),
('4t ESO A', 'HIS12'),
('4t ESO A', 'LLC02'),
('4t ESO A', 'LLC03'),
('4t ESO A', 'MAT01'),
('4t ESO A', 'QUI08'),
('4t ESO A', 'TEC05'),
('DAM 1', 'ANG04'),
('DAM 1', 'FOL14'),
('DAM 1', 'INF06'),
('DAM 1', 'LLC02'),
('DAW 1', 'ANG04'),
('DAW 1', 'FOL14'),
('DAW 1', 'INF06'),
('DAW 1', 'LLC02'),
('DAW 2', 'ANG04'),
('DAW 2', 'FOL14'),
('DAW 2', 'INF06'),
('DAW 2', 'LLC02');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `grup_classe`
--

CREATE TABLE `grup_classe` (
  `nom` varchar(20) NOT NULL,
  `aula` varchar(15) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `grup_classe`
--

INSERT INTO `grup_classe` (`nom`, `aula`) VALUES
('1r BATX A', 'C-301'),
('1r ESO A', 'A-101'),
('1r ESO B', 'A-102'),
('2n BATX A', 'C-302'),
('2n ESO A', 'A-201'),
('3r ESO A', 'B-105'),
('4t ESO A', 'B-206'),
('DAM 1', 'Informàtica-3'),
('DAW 1', 'Informàtica-1'),
('DAW 2', 'Informàtica-2');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `historic_mitjanes`
--

CREATE TABLE `historic_mitjanes` (
  `id` int(11) NOT NULL,
  `nia` int(11) NOT NULL,
  `modul` varchar(10) NOT NULL COMMENT 'Ej: INF06, FOL14, LLC02...',
  `mitjana` decimal(4,2) NOT NULL COMMENT 'Nota media actual del módulo o UF (0.00 a 10.00)',
  `data_calcul` date NOT NULL DEFAULT curdate() COMMENT 'Fecha en que se calculó esta media'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `historic_mitjanes`
--

INSERT INTO `historic_mitjanes` (`id`, `nia`, `modul`, `mitjana`, `data_calcul`) VALUES
(1, 313056, 'INF06', 8.80, '2025-11-30'),
(2, 313056, 'INF06', 8.50, '2025-10-15'),
(3, 313056, 'FOL14', 7.90, '2025-11-30'),
(4, 313056, 'LLC02', 8.60, '2025-11-30'),
(5, 993346, 'INF06', 7.30, '2025-11-30'),
(6, 993346, 'INF06', 7.10, '2025-10-20'),
(7, 993346, 'FOL14', 6.80, '2025-11-30'),
(8, 970564, 'INF06', 4.70, '2025-11-30'),
(9, 970564, 'INF06', 4.20, '2025-10-10'),
(10, 273885, 'INF06', 9.60, '2025-11-30'),
(11, 234033, 'INF06', 9.85, '2025-11-30'),
(12, 727264, 'INF06', 7.70, '2025-11-30'),
(13, 665021, 'INF06', 6.90, '2025-11-30'),
(14, 345805, 'INF06', 8.40, '2025-11-30'),
(15, 395902, 'INF06', 8.10, '2025-11-30');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `hores_de_classe`
--

CREATE TABLE `hores_de_classe` (
  `id_hora` int(11) NOT NULL,
  `grup_classe` varchar(20) NOT NULL,
  `codi_assignatura` varchar(10) NOT NULL,
  `data_hora_inici` datetime NOT NULL,
  `data_hora_fi` datetime NOT NULL,
  `retard_minuts` tinyint(4) DEFAULT NULL,
  `observacions` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `hores_de_classe`
--

INSERT INTO `hores_de_classe` (`id_hora`, `grup_classe`, `codi_assignatura`, `data_hora_inici`, `data_hora_fi`, `retard_minuts`, `observacions`) VALUES
(13, 'DAM 1', 'INF06', '2025-11-03 08:30:00', '2025-11-03 09:25:00', NULL, 'Introducció a la programació'),
(14, 'DAM 1', 'INF06', '2025-11-05 08:30:00', '2025-11-05 09:25:00', 10, 'Retard per avaria de cotxe'),
(15, 'DAM 1', 'INF06', '2025-11-10 08:30:00', '2025-11-10 09:25:00', NULL, 'Examen UF1 - Variables i tipus'),
(16, 'DAM 1', 'FOL14', '2025-11-11 11:20:00', '2025-11-11 12:15:00', NULL, 'Seguretat Social i contractes'),
(17, 'DAW 2', 'INF06', '2025-11-04 09:30:00', '2025-11-04 10:25:00', NULL, 'PHP - Sessions i autenticació'),
(18, 'DAW 2', 'INF06', '2025-11-06 09:30:00', '2025-11-06 10:25:00', NULL, 'API REST amb Laravel'),
(19, '1r BATX A', 'ANG04', '2025-11-07 08:30:00', '2025-11-07 09:25:00', NULL, 'Present Perfect vs Past Simple'),
(20, '1r BATX A', 'MAT01', '2025-11-10 10:20:00', '2025-11-10 11:15:00', 5, 'Retard per reunió de departament'),
(21, '1r BATX A', 'FIL11', '2025-11-12 12:15:00', '2025-11-12 13:10:00', NULL, 'Plató: Mite de la caverna'),
(22, 'DAM 1', 'LLC02', '2025-11-13 10:20:00', '2025-11-13 11:15:00', NULL, 'Redacció formal: carta de presentació'),
(23, 'DAW 2', 'FOL14', '2025-11-14 11:20:00', '2025-11-14 12:15:00', NULL, 'Nòmines i seguretat social'),
(24, 'DAM 1', 'INF06', '2025-11-17 08:30:00', '2025-11-17 09:25:00', NULL, 'Projecte UF2 entregat - Arrays');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `hores_de_classe_professors`
--

CREATE TABLE `hores_de_classe_professors` (
  `id_hora` int(11) NOT NULL,
  `id_professor` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `hores_de_classe_professors`
--

INSERT INTO `hores_de_classe_professors` (`id_hora`, `id_professor`) VALUES
(13, 'PROF0002'),
(13, 'PROF0018'),
(14, 'PROF0001'),
(14, 'PROF0002'),
(14, 'PROF0018'),
(15, 'PROF0004'),
(15, 'PROF0018'),
(16, 'PROF0001'),
(16, 'PROF0004'),
(16, 'PROF0017'),
(17, 'PROF0003'),
(17, 'PROF0015'),
(18, 'PROF0003'),
(18, 'PROF0015'),
(19, 'PROF0001'),
(19, 'PROF0015'),
(19, 'PROF0017'),
(20, 'PROF0002'),
(20, 'PROF0004'),
(21, 'PROF0001'),
(21, 'PROF0017'),
(22, 'PROF0003'),
(22, 'PROF0004'),
(22, 'PROF0017'),
(23, 'PROF0001'),
(23, 'PROF0017'),
(24, 'PROF0001'),
(24, 'PROF0018');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `login_logs`
--

CREATE TABLE `login_logs` (
  `id` int(11) NOT NULL,
  `login_timestamp` datetime DEFAULT NULL,
  `username` varchar(50) NOT NULL,
  `ip_direccio` varchar(45) NOT NULL,
  `login_success` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `login_logs`
--

INSERT INTO `login_logs` (`id`, `login_timestamp`, `username`, `ip_direccio`, `login_success`) VALUES
(1, '0000-00-00 00:00:00', 'tomeu.ramirez', '83.42.127.18', 1),
(2, '0000-00-00 00:00:00', 'xavier.molins', '192.168.1.100', 1),
(3, '0000-00-00 00:00:00', 'marina.canal', '85.48.201.90', 1),
(4, '0000-00-00 00:00:00', 'guillem.cervera', '88.23.145.201', 1),
(5, '0000-00-00 00:00:00', 'ivan.riera', '79.153.24.120', 1),
(6, '0000-00-00 00:00:00', 'marc.escudero', '62.57.188.50', 1),
(7, '0000-00-00 00:00:00', 'ainab', '88.17.200.45', 0),
(8, '0000-00-00 00:00:00', 'ainab', '88.17.200.45', 0),
(9, '0000-00-00 00:00:00', 'ainab', '88.17.200.45', 1),
(10, '0000-00-00 00:00:00', 'noelia.segura', '83.42.150.22', 1),
(11, '0000-00-00 00:00:00', 'margalida.crespi', '62.57.188.33', 1),
(12, '0000-00-00 00:00:00', 'lidia.farre', '79.153.45.112', 1),
(13, '0000-00-00 00:00:00', 'alba.vives', '87.123.45.67', 1),
(14, '0000-00-00 00:00:00', 'claudia.ortega', '85.48.201.78', 1),
(15, '0000-00-00 00:00:00', 'joan.vilaseca', '10.45.67.89', 1),
(16, '0000-00-00 00:00:00', 'eric.guillen', '79.153.24.100', 1),
(17, '0000-00-00 00:00:00', 'maria.garcia', '185.220.101.45', 0),
(18, '0000-00-00 00:00:00', 'maria.garcia', '185.220.101.45', 0),
(19, '0000-00-00 00:00:00', 'maria.garcia', '185.220.101.45', 0),
(20, '0000-00-00 00:00:00', 'maria.garcia', '185.220.101.45', 0),
(21, '0000-00-00 00:00:00', 'maria.garcia', '185.220.101.45', 0),
(22, '0000-00-00 00:00:00', 'maria.garcia', '185.220.101.45', 0),
(23, '0000-00-00 00:00:00', 'maria.garcia', '185.220.101.45', 0),
(24, '0000-00-00 00:00:00', 'tomeu.ramirez', '83.42.127.18', 1),
(25, '0000-00-00 00:00:00', 'maria.garcia', '192.168.1.50', 1),
(26, '0000-00-00 00:00:00', 'silvia.moreno', '88.17.94.112', 1),
(27, '0000-00-00 00:00:00', 'anna.rodriguez', '79.153.24.89', 1),
(28, '0000-00-00 00:00:00', 'quim.rovira', '85.48.201.56', 1),
(29, '0000-00-00 00:00:00', 'elena.blanco', '10.10.15.34', 1),
(30, '0000-00-00 00:00:00', 'anna.rodriguez', '::1', 0),
(31, '2026-05-05 23:57:34', 'anna.rodriguez', '::1', 1),
(32, '2026-05-05 23:58:02', 'anna.rodriguez', '::1', 1),
(33, '2026-05-06 00:07:41', 'anna.rodriguez', '127.0.0.1', 1),
(34, '2026-05-06 01:53:07', 'anna.rodriguez', '127.0.0.1', 1),
(35, '2026-05-06 01:58:22', 'anna.rodriguez', '127.0.0.1', 1),
(36, '2026-05-06 02:02:21', 'anna.rodriguez', '127.0.0.1', 1),
(37, '2026-05-06 02:08:38', 'anna.rodriguez', '127.0.0.1', 1),
(38, '2026-05-06 04:55:24', 'anna.rodriguez', '127.0.0.1', 1),
(39, '2026-05-06 04:58:49', 'anna.rodriguez ', '127.0.0.1', 1),
(40, '2026-05-06 05:21:16', 'anna.rodriguez', '127.0.0.1', 1),
(41, '2026-05-06 09:13:57', 'anna.rodriguez', '127.0.0.1', 1),
(42, '2026-05-06 09:17:07', 'anna.rodriguez', '::1', 1),
(43, '2026-05-06 09:27:52', 'anna.rodriguez', '::1', 1),
(44, '2026-05-06 09:30:31', 'tomeu.ramirez', '::1', 0),
(45, '2026-05-06 09:31:49', 'tomeu.ramirez', '::1', 1),
(46, '2026-05-06 09:56:00', 'tomeu.ramirez', '::1', 1),
(47, '2026-05-06 09:58:51', 'tomeu.ramirez', '::1', 1),
(48, '2026-05-06 10:01:39', 'tomeu.ramirez', '::1', 1),
(49, '2026-05-06 10:06:03', 'tomeu.ramirez', '::1', 1),
(50, '2026-05-06 10:08:42', 'tomeu.ramirez', '::1', 1),
(51, '2026-05-06 10:13:40', 'tomeu.ramirez', '::1', 1),
(52, '2026-05-06 10:34:41', 'tomeu.ramirez', '::1', 1),
(53, '2026-05-06 10:41:36', 'tomeu.ramirez', '::1', 1),
(54, '2026-05-06 10:57:06', 'tomeu.ramirez', '::1', 1),
(55, '2026-05-06 10:58:59', 'tomeu.ramirez', '::1', 1),
(56, '2026-05-06 11:01:33', 'tomeu.ramirez', '::1', 1),
(57, '2026-05-06 12:18:22', 'elena.blanco', '::1', 1),
(58, '2026-05-06 12:19:42', 'elena.blanco', '::1', 1),
(59, '2026-05-06 12:22:55', 'elena.blanco', '::1', 1),
(60, '2026-05-06 12:29:42', 'elena.blanco', '::1', 1),
(61, '2026-05-06 13:02:33', 'elena.blanco', '::1', 1),
(62, '2026-05-06 13:04:34', 'elena.blanco', '::1', 1),
(63, '2026-05-06 13:04:53', 'elena.blanco', '::1', 1),
(64, '2026-05-06 13:05:40', 'elena.blanco', '::1', 1),
(65, '2026-05-06 13:14:11', 'elena.blanco', '::1', 1),
(66, '2026-05-06 13:43:45', 'elena.blanco', '::1', 1),
(67, '2026-05-06 13:45:19', 'tomeu.ramirez', '::1', 1),
(68, '2026-05-06 13:49:50', 'tomeu.ramirez', '::1', 0),
(69, '2026-05-06 13:49:54', 'tomeu.ramirez', '::1', 1),
(70, '2026-05-06 13:51:58', 'tomeu.ramirez', '::1', 1),
(71, '2026-05-06 13:53:41', 'tomeu.ramirez', '::1', 0),
(72, '2026-05-06 13:53:45', 'tomeu.ramirez', '::1', 1),
(73, '2026-05-06 13:57:02', 'tomeu.ramirez', '::1', 1),
(74, '2026-05-06 14:05:17', 'tomeu.ramirez', '::1', 1),
(80, '2026-05-06 14:12:48', 'elena.blanco', '::1', 1),
(84, '2026-05-06 14:16:51', 'xavier.molins', '::1', 1),
(85, '2026-05-06 14:21:29', 'xavier.molins', '::1', 1),
(86, '2026-05-06 14:26:37', 'xavier.molins', '::1', 1),
(87, '2026-05-06 14:42:50', 'elena.blanco', '::1', 1),
(88, '2026-05-06 15:00:29', 'maria.garcia', '::1', 1),
(89, '2026-05-06 15:06:27', 'maria.garcia', '::1', 1),
(90, '2026-05-06 15:10:39', 'maria.garcia', '::1', 1),
(91, '2026-05-06 15:12:37', 'anna.rodriguez', '127.0.0.1', 1),
(92, '2026-05-06 15:31:02', 'anna.rodriguez', '127.0.0.1', 1),
(93, '2026-05-06 16:09:52', 'maria.garcia', '::1', 1),
(94, '2026-05-06 16:12:00', 'maria.garcia', '::1', 1),
(95, '2026-05-06 16:21:05', 'elena.blanco', '::1', 1),
(96, '2026-05-06 16:22:13', 'elena.blanco', '::1', 1),
(97, '2026-05-06 16:26:15', 'elena.blanco', '::1', 1),
(98, '2026-05-06 16:36:11', 'anna.rodriguez', '127.0.0.1', 1),
(99, '2026-05-06 18:43:32', 'anna.rodriguez', '127.0.0.1', 0),
(100, '2026-05-06 18:43:42', 'anna.rodriguez', '127.0.0.1', 1),
(101, '2026-05-06 19:06:54', 'anna.rodriguez', '127.0.0.1', 1),
(102, '2026-05-06 19:22:24', 'xavier.molins', '::1', 1),
(103, '2026-05-06 19:25:30', 'xavier.molins', '::1', 1),
(104, '2026-05-06 19:33:39', 'elena.blanco', '::1', 1),
(105, '2026-05-06 19:37:51', 'maria.garcia', '::1', 0),
(106, '2026-05-06 19:37:56', 'maria.garcia', '::1', 1),
(107, '2026-05-06 20:23:14', 'nuria.torres', '127.0.0.1', 1),
(108, '2026-05-06 20:45:14', 'anna.rodriguez', '127.0.0.1', 1),
(109, '2026-05-06 20:51:03', 'elena.blanco', '::1', 1),
(110, '2026-05-06 20:53:21', 'maria.garcia', '::1', 1),
(111, '2026-05-22 10:38:42', 'anna.rodriguez', '127.0.0.1', 1),
(112, '2026-05-22 10:51:25', 'maria.garcia', '::1', 1),
(113, '2026-05-22 10:55:17', 'tomeu.ramirez', '::1', 1),
(114, '2026-05-22 10:57:29', 'elena.blanco', '::1', 1),
(115, '2026-05-22 11:22:01', 'anna.rodriguez', '127.0.0.1', 1),
(116, '2026-05-22 11:30:16', 'elena.blanco', '::1', 1),
(117, '2026-05-22 11:55:58', 'tomeu.ramirez', '::1', 1),
(118, '2026-05-22 12:11:23', 'anna.rodriguez', '::1', 0),
(119, '2026-05-22 12:11:47', 'anna.rodriguez', '::1', 1),
(120, '2026-05-22 12:16:43', 'anna.rodriguez', '::1', 1),
(121, '2026-05-22 12:17:11', 'anna.rodriguez', '::1', 1),
(122, '2026-05-22 12:22:50', 'anna.rodriguez', '::1', 1),
(123, '2026-05-22 12:23:10', 'maria.garcia', '::1', 1),
(124, '2026-05-22 12:23:53', 'elena.blanco', '::1', 1),
(125, '2026-05-22 12:24:11', 'tomeu.ramirez', '::1', 1),
(126, '2026-05-22 13:07:26', 'anna.rodriguez', '::1', 1),
(127, '2026-05-22 13:08:14', 'anna.rodriguez', '::1', 1),
(128, '2026-05-22 13:18:23', 'anna.rodriguez', '::1', 1),
(129, '2026-05-22 13:20:43', 'anna.rodriguez', '::1', 1),
(130, '2026-05-22 14:10:56', 'elena.blanco', '::1', 1),
(131, '2026-05-22 14:15:12', 'elena.blanco', '::1', 1),
(132, '2026-05-22 14:20:58', 'elena.blanco', '::1', 1),
(133, '2026-05-22 14:49:57', 'elena.blanco', '::1', 1),
(134, '2026-05-22 14:50:42', 'anna.rodriguez', '::1', 1),
(135, '2026-05-22 14:52:41', 'anna.rodriguez', '::1', 1),
(136, '2026-05-22 14:52:58', 'elena.blanco', '::1', 1),
(137, '2026-05-22 14:59:32', 'elena.blanco', '::1', 1),
(138, '2026-05-22 15:05:34', 'elena.blanco', '::1', 1),
(139, '2026-05-22 15:10:52', 'elena.blanco', '::1', 1),
(140, '2026-05-22 15:18:38', 'elena.blanco', '::1', 1),
(141, '2026-05-22 15:30:45', 'elena.blanco', '::1', 0),
(142, '2026-05-22 15:30:49', 'elena.blanco', '::1', 1),
(143, '2026-05-22 15:36:18', 'elena.blanco', '::1', 1),
(144, '2026-05-22 16:23:07', 'anna.rodriguez', '::1', 1),
(145, '2026-05-22 16:26:09', 'anna.rodriguez', '::1', 1),
(146, '2026-05-22 16:36:36', 'anna.rodriguez', '::1', 1),
(147, '2026-05-22 17:15:56', 'elena.blanco', '::1', 0),
(148, '2026-05-22 17:15:59', 'elena.blanco', '::1', 1),
(149, '2026-05-22 18:38:13', 'tomeu.ramirez', '::1', 1),
(151, '2026-05-22 18:41:01', 'tomeu.ramirez', '::1', 1),
(152, '2026-05-22 18:45:13', 'tomeu.ramirez', '::1', 1),
(153, '2026-05-22 18:46:45', 'tomeu.ramirez', '::1', 1),
(154, '2026-05-22 18:47:30', 'tomeu.ramirez', '::1', 1),
(155, '2026-05-22 18:50:10', 'tomeu.ramirez', '::1', 1),
(156, '2026-05-22 19:09:38', 'tomeu.ramirez', '::1', 1),
(157, '2026-05-22 19:16:15', 'tomeu.ramirez', '::1', 1),
(158, '2026-05-22 19:22:21', 'tomeu.ramirez', '::1', 1),
(159, '2026-05-22 22:10:19', 'maria.garcia', '::1', 1),
(160, '2026-05-22 22:30:46', 'maria.garcia', '::1', 1),
(161, '2026-05-22 23:03:25', 'maria.garcia', '::1', 1),
(162, '2026-05-23 00:05:38', 'maria.garcia', '::1', 1),
(163, '2026-05-23 00:07:51', 'maria.garcia', '::1', 1),
(164, '2026-05-23 00:14:21', 'maria.garcia', '::1', 1),
(165, '2026-05-23 00:15:02', 'maria.garcia', '::1', 1),
(166, '2026-05-23 00:20:03', 'maria.garcia', '::1', 1),
(167, '2026-05-23 00:22:20', 'maria.garcia', '::1', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `matricula`
--

CREATE TABLE `matricula` (
  `id` int(11) NOT NULL,
  `nia` int(11) NOT NULL,
  `codi_assignatura` varchar(10) NOT NULL COMMENT 'Ej: INF06, FOL14, ANG04, LLC02...',
  `curso` varchar(20) NOT NULL DEFAULT '2025-2026',
  `nom_grupclasse` varchar(50) DEFAULT NULL,
  `data_matricula` date NOT NULL DEFAULT curdate(),
  `aprovada` tinyint(1) NOT NULL DEFAULT 0 COMMENT '0 = no aprovada, 1 = aprovada',
  `nota_final` decimal(4,2) DEFAULT NULL COMMENT 'Nota final de la assignatura (0.00 a 10.00)'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `matricula`
--

INSERT INTO `matricula` (`id`, `nia`, `codi_assignatura`, `curso`, `nom_grupclasse`, `data_matricula`, `aprovada`, `nota_final`) VALUES
(1, 313056, 'INF06', '2025-2026', '1r ESO A', '2025-09-10', 1, 8.80),
(2, 313056, 'FOL14', '2025-2026', '1r ESO A', '2025-09-10', 1, 7.90),
(3, 313056, 'LLC02', '2025-2026', '1r ESO A', '2025-09-10', 1, 8.50),
(4, 313056, 'ANG04', '2025-2026', '1r ESO A', '2025-09-10', 1, 9.10),
(5, 993346, 'INF06', '2025-2026', NULL, '2025-09-10', 1, 7.30),
(6, 993346, 'FOL14', '2025-2026', NULL, '2025-09-10', 1, 6.80),
(7, 993346, 'LLC02', '2025-2026', NULL, '2025-09-10', 1, 8.00),
(8, 970564, 'INF06', '2025-2026', NULL, '2025-09-10', 0, 4.70),
(9, 970564, 'FOL14', '2025-2026', NULL, '2025-09-10', 0, 4.90),
(10, 273885, 'INF06', '2025-2026', 'DAW 2', '2025-09-10', 1, 9.60),
(11, 234033, 'INF06', '2025-2026', '2n ESO A', '2025-09-10', 1, 9.85),
(12, 727264, 'INF06', '2025-2026', NULL, '2025-09-10', 1, 7.70),
(13, 665021, 'INF06', '2025-2026', NULL, '2025-09-10', 1, 6.90),
(14, 345805, 'INF06', '2025-2026', 'DAM 1', '2025-09-10', 1, 8.40),
(15, 395902, 'INF06', '2025-2026', 'DAM 1', '2025-09-10', 1, 8.10);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `modul`
--

CREATE TABLE `modul` (
  `nom` varchar(10) NOT NULL,
  `nom_complet` varchar(100) NOT NULL,
  `hores` int(11) NOT NULL,
  `codi_assignatura` varchar(10) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `modul`
--

INSERT INTO `modul` (`nom`, `nom_complet`, `hores`, `codi_assignatura`) VALUES
('ANG04', 'Anglès tècnic', 80, 'ANG04'),
('FOL14', 'Formació i orientació laboral', 60, 'FOL14'),
('INF01', 'Sistemes informàtics', 120, 'INF06'),
('INF02', 'Bases de dades', 100, 'INF06'),
('INF03', 'Programació bàsica', 140, 'INF06'),
('INF04', 'Entorns de desenvolupament', 90, 'INF06'),
('INF05', 'Llenguatges de marques', 80, 'INF06'),
('INF06', 'Programació avançada', 120, 'INF06'),
('INF07', 'Sistemes gestors de bases de dades', 100, 'INF06'),
('INF08', 'Administració de sistemes operatius', 110, 'INF06'),
('INF09', 'Seguretat informàtica', 90, 'INF06'),
('LLC02', 'Llengua catalana professional', 80, 'LLC02');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `notes`
--

CREATE TABLE `notes` (
  `nia` int(11) NOT NULL,
  `uf_id` int(11) NOT NULL,
  `nota` decimal(4,2) DEFAULT NULL CHECK (`nota` between 0 and 10),
  `data_nota` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `notes`
--

INSERT INTO `notes` (`nia`, `uf_id`, `nota`, `data_nota`) VALUES
(126440, 23, 6.00, '2026-05-06 00:00:00'),
(126440, 24, 7.00, '2026-05-06 00:00:00'),
(151704, 1, 8.00, '2025-11-25 00:00:00'),
(151704, 24, 5.00, '2026-05-06 00:00:00'),
(234033, 1, 9.80, '2025-11-25 00:00:00'),
(234033, 2, 9.50, '2025-11-25 00:00:00'),
(234033, 10, 9.70, '2025-11-25 00:00:00'),
(234033, 24, 7.00, '2026-05-06 00:00:00'),
(270477, 1, 6.90, '2025-11-25 00:00:00'),
(270477, 24, 6.00, '2026-05-06 00:00:00'),
(273885, 1, 9.40, '2025-11-25 00:00:00'),
(273885, 2, 8.70, '2025-11-25 00:00:00'),
(273885, 10, 9.00, '2025-11-25 00:00:00'),
(273885, 23, 4.00, '2026-05-06 00:00:00'),
(273885, 24, 2.00, '2026-05-06 00:00:00'),
(313056, 1, 8.90, '2025-11-25 00:00:00'),
(313056, 2, 9.20, '2025-11-25 00:00:00'),
(313056, 3, 8.50, '2025-11-25 00:00:00'),
(313056, 5, 9.10, '2025-11-25 00:00:00'),
(313056, 6, 8.80, '2025-11-25 00:00:00'),
(313056, 9, 8.50, '2025-11-25 00:00:00'),
(313056, 21, 10.00, '2026-05-22 11:03:51'),
(313056, 22, 10.00, '2026-05-22 11:31:37'),
(313056, 23, 6.00, '2026-05-06 00:00:00'),
(313056, 24, 2.00, '2026-05-06 00:00:00'),
(345805, 1, 8.20, '2025-11-25 00:00:00'),
(345805, 14, 8.30, '2025-11-25 00:00:00'),
(345805, 24, 7.00, '2026-05-06 00:00:00'),
(395902, 1, 7.90, '2025-11-25 00:00:00'),
(395902, 14, 7.50, '2025-11-25 00:00:00'),
(395902, 24, 5.00, '2026-05-06 00:00:00'),
(430700, 1, 6.20, '2025-11-25 00:00:00'),
(430700, 24, 8.00, '2026-05-06 00:00:00'),
(439078, 24, 6.00, '2026-05-06 00:00:00'),
(458305, 1, 7.70, '2025-11-25 00:00:00'),
(458305, 23, 5.00, '2026-05-06 00:00:00'),
(458305, 24, 3.00, '2026-05-06 00:00:00'),
(554569, 1, 5.70, '2025-11-25 00:00:00'),
(554569, 23, 2.00, '2026-05-06 00:00:00'),
(554569, 24, 2.00, '2026-05-06 00:00:00'),
(567540, 1, 7.00, '2025-11-25 00:00:00'),
(567540, 21, 10.00, '2026-05-06 00:00:00'),
(567540, 24, 6.00, '2026-05-06 00:00:00'),
(603640, 23, 6.00, '2026-05-06 00:00:00'),
(603640, 24, 4.00, '2026-05-06 00:00:00'),
(665021, 1, 6.30, '2025-11-25 00:00:00'),
(665021, 12, 6.80, '2025-11-25 00:00:00'),
(665021, 23, 4.00, '2026-05-06 00:00:00'),
(665021, 24, 1.00, '2026-05-06 00:00:00'),
(727264, 1, 7.60, '2025-11-25 00:00:00'),
(727264, 2, 6.90, '2025-11-25 00:00:00'),
(727264, 12, 8.20, '2025-11-25 00:00:00'),
(727264, 23, 5.00, '2026-05-06 00:00:00'),
(727264, 24, 5.00, '2026-05-06 00:00:00'),
(727471, 1, 8.10, '2025-11-25 00:00:00'),
(727471, 24, 6.00, '2026-05-06 00:00:00'),
(807566, 23, 4.00, '2026-05-06 00:00:00'),
(807566, 24, 10.00, '2026-05-06 00:00:00'),
(857629, 1, 8.40, '2025-11-25 00:00:00'),
(857629, 24, 5.00, '2026-05-06 00:00:00'),
(968100, 1, 7.30, '2025-11-25 00:00:00'),
(968100, 23, 4.00, '2026-05-06 00:00:00'),
(968100, 24, 10.00, '2026-05-06 00:00:00'),
(968618, 1, 8.60, '2025-11-25 00:00:00'),
(968618, 23, 5.00, '2026-05-06 00:00:00'),
(968618, 24, 1.00, '2026-05-06 00:00:00'),
(970564, 1, 5.20, '2025-11-25 00:00:00'),
(970564, 2, 4.80, '2025-11-25 00:00:00'),
(970564, 3, 6.10, '2025-11-25 00:00:00'),
(970564, 9, 5.00, '2025-11-25 00:00:00'),
(970564, 24, 6.00, '2026-05-06 00:00:00'),
(993346, 1, 6.80, '2025-11-25 00:00:00'),
(993346, 2, 8.90, '2025-11-25 00:00:00'),
(993346, 3, 7.20, '2025-11-25 00:00:00'),
(993346, 4, 5.90, '2025-11-25 00:00:00'),
(993346, 9, 7.10, '2025-11-25 00:00:00'),
(993346, 24, 7.00, '2026-05-06 00:00:00');

--
-- Disparadores `notes`
--
DELIMITER $$
CREATE TRIGGER `trg_notes_update` AFTER UPDATE ON `notes` FOR EACH ROW BEGIN
    IF OLD.nota <=> NEW.nota = 0 THEN
        INSERT INTO estudiants_historia (nia, nom_camp, valor_nou, valor_antic, descripcio)
        SELECT NEW.nia,
               CONCAT('nota_UF_', u.id),
               NEW.nota,
               OLD.nota,
               CONCAT('Modificació de nota: ', IFNULL(OLD.nota,'pendent'), ' → ', IFNULL(NEW.nota,'pendent'),
                      ' | UF: ', u.nom, ' (', m.nom, ')')
        FROM unitat u
        JOIN modul m ON u.modul = m.nom
        WHERE u.id = NEW.uf_id;
    END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `observacions_incidencies`
--

CREATE TABLE `observacions_incidencies` (
  `id_observacio` int(11) NOT NULL,
  `id_practica` int(11) NOT NULL,
  `data_observacio` datetime NOT NULL DEFAULT current_timestamp(),
  `tipus` enum('observacio','incidencia') NOT NULL DEFAULT 'observacio',
  `descripcio` text NOT NULL,
  `autor_id` varchar(20) NOT NULL COMMENT 'id_intern del professor que és tutor FCT'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `observacions_incidencies`
--

INSERT INTO `observacions_incidencies` (`id_observacio`, `id_practica`, `data_observacio`, `tipus`, `descripcio`, `autor_id`) VALUES
(1, 1, '2024-04-15 09:30:00', 'observacio', 'Integració molt ràpida a l\'equip de backend.', 'PROF0001'),
(2, 1, '2024-05-28 12:15:00', 'observacio', 'Felicitació del client per la qualitat del codi.', 'PROF0001'),
(3, 2, '2024-04-10 08:00:00', 'incidencia', 'Baixa mèdica 4 dies. Justificant entregat.', 'PROF0001'),
(4, 2, '2024-05-20 14:30:00', 'observacio', 'Ha recuperat totes les hores perdudes.', 'PROF0017'),
(5, 5, '2025-03-18 10:00:00', 'incidencia', 'Problema accés VPN. Ja resolt per informàtica.', 'PROF0002'),
(6, 5, '2025-04-05 13:20:00', 'observacio', 'Bon nivell de React + TypeScript.', 'PROF0002'),
(7, 7, '2025-03-25 09:15:00', 'observacio', 'Molt motivat amb el projecte de vehicle connectat.', 'PROF0018'),
(8, 7, '2025-04-12 11:45:00', 'incidencia', 'Retard sprint per documentació interna. Solucionat.', 'PROF0002'),
(9, 3, '2024-04-22 09:00:00', 'incidencia', '2 faltes injustificades. Cal entrevista.', 'PROF0001'),
(10, 4, '2024-04-30 11:00:00', 'observacio', 'Excel·lent comunicació en anglès amb client.', 'PROF0015'),
(11, 6, '2025-04-03 12:10:00', 'observacio', 'Participació activa a les daily meetings.', 'PROF0003'),
(12, 9, '2025-04-08 13:30:00', 'observacio', 'Ha detectat vulnerabilitat crítica. Gran feina!', 'PROF0002'),
(13, 10, '2025-02-20 16:00:00', 'observacio', 'Client freelance molt satisfet. Pagament avançat.', 'PROF0017'),
(14, 11, '2025-04-10 14:50:00', 'observacio', 'Desplegament en AWS sense errors. Professional.', 'PROF0001'),
(15, 1, '2024-06-10 10:30:00', 'observacio', 'Pràctiques finalitzades amb excel·lent. Recomanat per contracte.', 'PROF0001');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `persona`
--

CREATE TABLE `persona` (
  `dni` varchar(9) NOT NULL,
  `nom` varchar(50) NOT NULL,
  `cognom` varchar(100) NOT NULL,
  `data_naix` date NOT NULL,
  `esMajor` tinyint(1) DEFAULT NULL,
  `poblacio` varchar(80) DEFAULT NULL,
  `codi_postal` varchar(5) DEFAULT NULL,
  `nacionalitat` varchar(50) DEFAULT NULL,
  `mun_naixement` varchar(80) DEFAULT NULL,
  `tel_mobil` int(11) DEFAULT NULL,
  `tel_fix` int(11) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `rol` enum('professor','estudiant','directiu','professor_directiu','admin') NOT NULL,
  `edat` int(11) NOT NULL,
  `foto` text DEFAULT NULL,
  `id_centre` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `persona`
--

INSERT INTO `persona` (`dni`, `nom`, `cognom`, `data_naix`, `esMajor`, `poblacio`, `codi_postal`, `nacionalitat`, `mun_naixement`, `tel_mobil`, `tel_fix`, `email`, `rol`, `edat`, `foto`, `id_centre`) VALUES
('02468024Y', 'Tomeu', 'Ramírez Bauzà', '1975-05-18', 1, 'Manacor', '07500', 'Espanya', 'Manacor', 633001122, 971550011, 'tomeu.ramirez@centre.edu', 'directiu', 50, NULL, 2),
('11223344E', 'Anna Castillejo', 'Rodríguez Castro', '2002-09-05', 1, 'Barcelona', '08028', 'Espanya', 'Barcelona', 601234567, NULL, 'anna.rodriguez.est@centre.edu', 'estudiant', 23, 'http://10.0.2.2/fotos/perfil_11223344E_1779442420.jpg', 4),
('11234567N', 'Xavier', 'Molins Ferrer', '1976-07-19', 1, 'Granollers', '08402', 'Espanya', 'Granollers', 644556677, 938401122, 'xavier.molins@centre.edu', 'directiu', 49, NULL, 4),
('12145448B', 'Katia', 'Sanchez Consa', '2003-08-20', 1, 'Barcelona', '08001', 'Española', 'Barcelona', 655678987, 934425266, 'daniela.sfoia@ejemplo.com', 'admin', 22, 'saddaf3af.jpg', 6),
('12345678A', 'Maria', 'García López', '1985-03-15', 1, 'Barcelona', '08015', 'Espanya', 'Barcelona', 612345678, 933221100, 'maria.garcia@centre.edu', 'admin', 40, NULL, 10),
('12345678Z', 'Marc Antonio', 'Rius Pla', '2003-07-20', 1, 'Barcelona', '08001', 'Española', 'Barcelona', 600111222, 934445566, 'marc.rius@ejemplo.com', 'admin', 22, 'perfil_marc.jpg', 9),
('12345679V', 'Elena', 'Blanco Ruiz', '1990-10-28', 1, 'Badalona', '08912', 'Espanya', 'Badalona', 622223344, NULL, 'elena.blanco@centre.edu', 'professor', 35, NULL, 3),
('13579135P', 'Quim', 'Rovira Tordera', '2003-02-27', 1, 'Blanes', '17300', 'Espanya', 'Blanes', 633112233, NULL, 'quim.rovira.est@centre.edu', 'estudiant', 22, NULL, 10),
('13579136Z', 'Margalida', 'Crespí Socias', '1999-07-30', 1, 'Alcúdia', '07400', 'Espanya', 'Alcúdia', 644112233, NULL, 'margalida.crespi.est@centre.edu', 'estudiant', 26, NULL, 1),
('13579246A', 'Guillem', 'Cervera Pla', '2000-07-04', 1, 'Castelldefels', '08860', 'Espanya', 'Castelldefels', 677778899, NULL, 'guillem.cervera.est@centre.edu', 'estudiant', 25, NULL, 2),
('14725836F', 'Aina', 'Ballester Roig', '1991-10-10', 1, 'Sant Boi', '08830', 'Espanya', 'Sant Boi', 633223344, NULL, 'aina.ballester@centre.edu', 'professor', 34, 'https://images.unsplash.com/photo-1525134479668-1bee5c7c6845?fm=jpg&q=60&w=3000&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8OHx8cmFuZG9tJTIwcGVvcGxlfGVufDB8fDB8fHww', 8),
('15975328J', 'Noelia', 'Segura Pons', '1989-01-31', 1, 'Esplugues', '08950', 'Espanya', 'Esplugues', 677667788, NULL, 'noelia.segura@centre.edu', 'professor', 36, NULL, 4),
('21098765Y', 'Iván', 'Riera Mas', '1997-09-12', 1, 'Santa Coloma', '08922', 'Espanya', 'Santa Coloma', 655556677, NULL, 'ivan.riera.est@centre.edu', 'estudiant', 28, NULL, 3),
('22334411R', 'Lídia', 'Farré Gil', '2001-08-22', 1, 'Igualada', '08700', 'Espanya', 'Igualada', 677889900, NULL, 'lidia.farre.est@centre.edu', 'estudiant', 24, NULL, 4),
('22334455I', 'Clàudia', 'Ortega Navarro', '2003-05-19', 1, 'Granollers', '08401', 'Espanya', 'Granollers', 678901234, NULL, 'claudia.ortega.est@centre.edu', 'estudiant', 22, NULL, 2),
('24680246Q', 'Marina', 'Canal Clavera', '1995-09-04', 1, 'Lloret de Mar', '17310', 'Espanya', 'Lloret', 644223344, NULL, 'marina.canal.est@centre.edu', 'estudiant', 30, NULL, 7),
('24681357C', 'Marc', 'Escudero Vila', '1996-12-30', 1, 'Gavà', '08850', 'Espanya', 'Gavà', 699990011, NULL, 'marc.escudero.est@centre.edu', 'estudiant', 28, NULL, 10),
('25123456K', 'Marta', 'Sánchez Puig', '1992-02-14', 1, 'Terrassa', '08221', 'Espanya', 'Terrassa', 611223344, NULL, 'marta.sanchez.est@centre.edu', 'estudiant', 33, NULL, 7),
('25896314I', 'Eric', 'Guillén Arnau', '2004-09-08', 1, 'Montcada i Reixac', '08110', 'Espanya', 'Montcada', 666556677, NULL, 'eric.guillen.est@centre.edu', 'estudiant', 21, NULL, 4),
('33445566G', 'Sílvia', 'Moreno Herrera', '1982-06-25', 1, 'Sabadell', '08201', 'Espanya', 'Terrassa', 645678901, 937654321, 'silvia.moreno@centre.edu', 'admin', 43, NULL, 10),
('33445599T', 'Alba', 'Vives Pons', '2003-01-17', 1, 'Sant Cugat', '08173', 'Espanya', 'Sant Cugat', 699001122, NULL, 'alba.vives.est@centre.edu', 'estudiant', 22, NULL, 6),
('34567890M', 'Núria', 'Torres Vidal', '2004-11-11', 1, 'Sabadell', '08205', 'Espanya', 'Sabadell', 633445566, NULL, 'nuria.torres.est@centre.edu', 'estudiant', 21, 'http://10.0.2.2/fotos/perfil_34567890M_1778091873.jpg', 9),
('35791357R', 'Joan', 'Vilaseca Pera', '1978-03-11', 1, 'Tordera', '08490', 'Espanya', 'Tordera', 655334455, 937643322, 'joan.vilaseca@centre.edu', 'directiu', 47, NULL, 7),
('36925814E', 'Oriol', 'Sabaté Moya', '2002-03-25', 1, 'Cerdanyola', '08193', 'Espanya', 'Cerdanyola', 622112233, NULL, 'oriol.sabate.est@centre.edu', 'estudiant', 23, NULL, 9),
('39728461T', 'Carles', 'Pujol i Costa', '1983-07-18', 1, 'Vic', '08500', 'Espanya', 'Vic', 687412589, 938891234, 'carles.pujol@centre.edu', 'admin', 42, NULL, 4),
('41350692Y', 'Laia', 'Domènech Roura', '1988-12-04', 1, 'Sant Feliu de Llobregat', '08980', 'Espanya', 'Sant Feliu de Llobregat', 654789321, 936661122, 'laia.domenech@centre.edu', 'admin', 36, NULL, 1),
('44556677C', 'Laura', 'Pérez Sánchez', '1990-07-30', 1, 'Girona', '17001', 'Espanya', 'Girona', 678901234, 972112233, 'laura.perez@centre.edu', 'professor', 35, NULL, 1),
('45092831K', 'Albert', 'Casas i Noguer', '1979-05-27', 1, 'Igualada', '08700', 'Espanya', 'Igualada', 676543210, 938012345, 'albert.casas@centre.edu', 'admin', 46, NULL, 5),
('45678901X', 'Sara', 'Herrero Soler', '1981-11-07', 1, 'Cornellà', '08940', 'Espanya', 'Cornellà', 644445566, 933765432, 'sara.herrero@centre.edu', 'admin', 44, NULL, 9),
('46802468S', 'Carla', 'Masó Ferrer', '2000-10-29', 1, 'Palafolls', '08389', 'Espanya', 'Palafolls', 666445566, NULL, 'carla.maso.est@centre.edu', 'estudiant', 25, NULL, 8),
('47210583M', 'Natàlia', 'Solé i Bernaus', '1992-02-19', 1, 'Manresa', '08240', 'Espanya', 'Manresa', 612987654, NULL, 'natalia.sole@centre.edu', 'admin', 33, NULL, 5),
('50123456P', 'Marc', 'Torres i Vidal', '1982-03-15', 1, 'Terrassa', '08227', 'Espanya', 'Terrassa', 612345678, 937123456, 'marc.torres@centre.edu', 'professor_directiu', 43, NULL, 1),
('50234567Q', 'Alba', 'Rius i Pons', '1985-06-22', 1, 'Sabadell', '08201', 'Espanya', 'Sabadell', 623456789, 937654321, 'alba.rius@centre.edu', 'professor_directiu', 40, NULL, 8),
('50345678R', 'Pau', 'García i Soler', '1979-11-08', 1, 'Granollers', '08401', 'Espanya', 'Granollers', 634567890, NULL, 'pau.garcia@centre.edu', 'professor_directiu', 46, NULL, 8),
('50456789S', 'Clàudia', 'Mas i Ferrer', '1987-09-30', 1, 'Mataró', '08301', 'Espanya', 'Mataró', 645678901, 937890123, 'claudia.mas@centre.edu', 'professor_directiu', 38, NULL, 5),
('50567890T', 'Arnau', 'Vila i Costa', '1980-01-27', 1, 'Badalona', '08917', 'Espanya', 'Badalona', 656789012, 933221100, 'arnau.vila@centre.edu', 'professor_directiu', 45, NULL, 10),
('50678901U', 'Judith', 'Camps i Roura', '1988-04-18', 1, 'Vic', '08500', 'Espanya', 'Vic', 667890123, NULL, 'judith.camps@centre.edu', 'professor_directiu', 37, NULL, 3),
('50789012V', 'Roger', 'Solé i Bernaus', '1981-12-05', 1, 'Manresa', '08241', 'Espanya', 'Manresa', 678901234, 938761234, 'roger.sole@centre.edu', 'professor_directiu', 43, NULL, 7),
('50890123W', 'Núria', 'Pla i Domènech', '1986-07-14', 1, 'Sant Cugat', '08173', 'Espanya', 'Sant Cugat', 689012345, NULL, 'nuria.pla@centre.edu', 'professor_directiu', 39, NULL, 6),
('50901234X', 'Èric', 'Ribó i Casals', '1983-10-20', 1, 'Cerdanyola', '08193', 'Espanya', 'Cerdanyola', 690123456, 935678901, 'eric.ribo@centre.edu', 'professor_directiu', 42, 'https://plus.unsplash.com/premium_photo-1689530775582-83b8abdb5020?fm=jpg&q=60&w=3000&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MXx8cmFuZG9tJTIwcGVyc29ufGVufDB8fDB8fHww', 9),
('51012345Y', 'Mireia', 'Farré i Olivé', '1989-02-25', 1, 'Igualada', '08700', 'Espanya', 'Igualada', 691234567, NULL, 'mireia.farre@centre.edu', 'professor_directiu', 36, NULL, 5),
('54321098Z', 'Judith', 'Oliveras Font', '1986-02-28', 1, 'Mollet', '08100', 'Espanya', 'Mollet', 666667788, NULL, 'judith.oliveras@centre.edu', 'professor', 39, NULL, 8),
('55663322Q', 'Roger', 'Pujol Serra', '1987-12-03', 1, 'Vic', '08500', 'Espanya', 'Vic', 666778899, NULL, 'roger.pujol@centre.edu', 'professor', 37, NULL, 4),
('55667788F', 'Pau', 'Domínguez Torres', '2001-12-12', 1, 'Badalona', '08917', 'Espanya', 'Badalona', 623456789, NULL, 'pau.dominguez.est@centre.edu', 'estudiant', 23, NULL, 7),
('57913579T', 'Nil', 'Estapé Costa', '1998-06-15', 1, 'Malgrat de Mar', '08380', 'Espanya', 'Malgrat', 677556677, NULL, 'nil.estape.est@centre.edu', 'estudiant', 27, NULL, 10),
('66778811U', 'David', 'Casas Moreno', '1983-06-05', 1, 'Rubí', '08191', 'Espanya', 'Rubí', 611112233, 936992233, 'david.casas@centre.edu', 'admin', 42, NULL, 8),
('66778899J', 'Èric', 'Soler Mendoza', '1975-10-03', 1, 'Vic', '08500', 'Espanya', 'Vic', 690123456, 938890011, 'eric.soler@centre.edu', 'directiu', 50, NULL, 10),
('68024680U', 'Eva', 'Campanyà Vidal', '1987-08-23', 1, 'Arenys de Mar', '08350', 'Espanya', 'Arenys', 688667788, NULL, 'eva.campanya@centre.edu', 'professor', 38, NULL, 8),
('68024691N', 'Gemma', 'Bofill Segarra', '1980-12-19', 1, 'Pineda de Mar', '08397', 'Espanya', 'Calella', 622001122, NULL, 'gemma.bofill@centre.edu', 'professor', 44, NULL, 8),
('74185296H', 'Montserrat', 'Prat Collado', '1977-04-02', 1, 'Ripollet', '08291', 'Espanya', 'Ripollet', 655445566, 935921133, 'montse.prat@centre.edu', 'admin', 48, NULL, 5),
('78901234H', 'Marc', 'Vila Castillo', '1988-01-08', 1, 'Mataró', '08301', 'Espanya', 'Mataró', 667890123, NULL, 'marc.vila@centre.edu', 'professor', 37, 'https://images.unsplash.com/photo-1539571696357-5a69c17a67c6?fm=jpg&q=60&w=3000&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8OHx8cmFuZG9tJTIwcGVyc29ufGVufDB8fDB8fHww', 10),
('78912345L', 'Albert', 'Ribas Costa', '1980-09-30', 1, 'Manresa', '08241', 'Espanya', 'Manresa', 622334455, 938765432, 'albert.ribas@centre.edu', 'professor', 45, 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?fm=jpg&q=60&w=3000&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8Nnx8cmFuZG9tJTIwcGVyc29ufGVufDB8fDB8fHww', 4),
('79135791V', 'Hugo', 'Serrats Puig', '2002-04-07', 1, 'Calella', '08370', 'Espanya', 'Calella', 699778899, NULL, 'hugo.serrats.est@centre.edu', 'estudiant', 23, NULL, 1),
('80246802W', 'Lluc', 'Fernández Ribot', '1991-01-14', 1, 'Palma', '07001', 'Espanya', 'Palma', 611889900, NULL, 'lluc.fernandez.est@centre.edu', 'estudiant', 34, NULL, 2),
('82468013M', 'Adrià', 'Palomer Ribas', '1993-05-06', 1, 'Premià de Mar', '08330', 'Espanya', 'Premià', 611990011, NULL, 'adria.palomer.est@centre.edu', 'estudiant', 32, NULL, 4),
('86420975B', 'Laia', 'Marqués Coll', '1979-05-21', 1, 'Viladecans', '08840', 'Espanya', 'Viladecans', 688889900, 936590011, 'laia.marques@centre.edu', 'directiu', 46, NULL, 7),
('87654321B', 'Jordi', 'Martínez Ruiz', '1978-11-22', 1, 'Lleida', '25001', 'Espanya', 'Lleida', 654789123, NULL, 'jordi.martinez@centre.edu', 'directiu', 47, NULL, 2),
('88776655S', 'Pol', 'Giménez Roca', '1994-03-09', 1, 'Vilanova i la Geltrú', '08800', 'Espanya', 'Vilanova', 688990011, NULL, 'pol.gimenez@centre.edu', 'professor', 31, NULL, 8),
('91357913X', 'Aina', 'Servera Pons', '1984-11-26', 1, 'Inca', '07300', 'Espanya', 'Inca', 622990011, NULL, 'aina.servera@centre.edu', 'professor', 40, NULL, 5),
('95135782L', 'Victòria', 'Aguilar Bosch', '1985-07-13', 1, 'Sant Adrià', '08930', 'Espanya', 'Sant Adrià', 699889900, 933811223, 'victoria.aguilar@centre.edu', 'admin', 40, NULL, 9),
('96385274G', 'Jan', 'Comas Sala', '1999-06-18', 1, 'Barberà del Vallès', '08210', 'Espanya', 'Barberà', 644334455, NULL, 'jan.comas.est@centre.edu', 'estudiant', 26, NULL, 1),
('97531924D', 'Cristina', 'Torrents Busquets', '1984-08-16', 1, 'El Prat', '08820', 'Espanya', 'El Prat', 611001122, NULL, 'cristina.torrents@centre.edu', 'professor', 41, NULL, 7),
('98765432W', 'Arnau', 'López Duran', '2005-04-15', 1, 'L\'Hospitalet', '08901', 'Espanya', 'L\'Hospitalet', 633334455, NULL, 'arnau.lopez.est@centre.edu', 'estudiant', 20, NULL, 10),
('99887722P', 'Irene', 'Camps Navarro', '1998-05-27', 1, 'Mataró', '08302', 'Espanya', 'Mataró', 655667788, NULL, 'irene.camps.est@centre.edu', 'estudiant', 27, NULL, 1),
('99887766D', 'Carles', 'Fernández Vidal', '1995-04-18', 1, 'Tarragona', '43001', 'Espanya', 'Reus', 689123456, NULL, 'carles.fernandez@centre.edu', 'professor', 30, NULL, 3);

--
-- Disparadores `persona`
--
DELIMITER $$
CREATE TRIGGER `trg_persona_before_insert` BEFORE INSERT ON `persona` FOR EACH ROW BEGIN
    SET NEW.edat = TIMESTAMPDIFF(YEAR, NEW.data_naix, CURDATE());
    
    IF TIMESTAMPDIFF(YEAR, NEW.data_naix, CURDATE()) >= 18 THEN
        SET NEW.esMajor = 1;
    ELSE
        SET NEW.esMajor = 0;
    END IF;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_persona_before_update` BEFORE UPDATE ON `persona` FOR EACH ROW BEGIN
    IF NEW.data_naix != OLD.data_naix OR NEW.data_naix IS NULL OR OLD.data_naix IS NULL THEN
        SET NEW.edat = TIMESTAMPDIFF(YEAR, NEW.data_naix, CURDATE());
        
        IF TIMESTAMPDIFF(YEAR, NEW.data_naix, CURDATE()) >= 18 THEN
            SET NEW.esMajor = 1;
        ELSE
            SET NEW.esMajor = 0;
        END IF;
    END IF;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_persona_update` AFTER UPDATE ON `persona` FOR EACH ROW BEGIN
    DECLARE v_nia INT DEFAULT NULL;
    
    SELECT nia INTO v_nia FROM estudiants WHERE dni_persona = NEW.dni LIMIT 1;
    
    IF v_nia IS NOT NULL THEN
        IF OLD.nom <> NEW.nom THEN
            INSERT INTO estudiants_historia (nia, nom_camp, valor_nou, valor_antic, descripcio)
            VALUES (v_nia, 'nom', NEW.nom, OLD.nom, 'Canvi de nom personal');
        END IF;

        IF OLD.cognom <> NEW.cognom THEN
            INSERT INTO estudiants_historia (nia, nom_camp, valor_nou, valor_antic, descripcio)
            VALUES (v_nia, 'cognom', NEW.cognom, OLD.cognom, 'Canvi de cognoms');
        END IF;

        IF OLD.data_naix <> NEW.data_naix THEN
            INSERT INTO estudiants_historia (nia, nom_camp, valor_nou, valor_antic, descripcio)
            VALUES (v_nia, 'data_naixement', NEW.data_naix, OLD.data_naix, 'Canvi de data de naixement');
        END IF;

        IF OLD.tel_mobil <=> NEW.tel_mobil = 0 THEN
            INSERT INTO estudiants_historia (nia, nom_camp, valor_nou, valor_antic, descripcio)
            VALUES (v_nia, 'tel_mobil', NEW.tel_mobil, OLD.tel_mobil, 'Canvi de telèfon mòbil');
        END IF;

        IF OLD.email <=> NEW.email = 0 THEN
            INSERT INTO estudiants_historia (nia, nom_camp, valor_nou, valor_antic, descripcio)
            VALUES (v_nia, 'email', NEW.email, OLD.email, 'Canvi d''email personal');
        END IF;

        IF OLD.poblacio <=> NEW.poblacio = 0 OR OLD.codi_postal <=> NEW.codi_postal = 0 THEN
            INSERT INTO estudiants_historia (nia, nom_camp, valor_nou, valor_antic, descripcio)
            VALUES (v_nia, 'domicili', 
                   CONCAT(NEW.poblacio, ' ', IFNULL(NEW.codi_postal,'')),
                   CONCAT(OLD.poblacio, ' ', IFNULL(OLD.codi_postal,'')),
                   'Canvi d''adreça o població');
        END IF;
    END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `professor`
--

CREATE TABLE `professor` (
  `id_intern` varchar(20) NOT NULL,
  `dni_persona` varchar(9) NOT NULL,
  `especialitzacio` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `professor`
--

INSERT INTO `professor` (`id_intern`, `dni_persona`, `especialitzacio`) VALUES
('PROF0001', '12345679V', 'Tecnologia'),
('PROF0002', '14725836F', 'Tecnologia'),
('PROF0003', '15975328J', 'Llengua i Literatura'),
('PROF0004', '44556677C', 'Tecnologia'),
('PROF0005', '54321098Z', 'Llengua i Literatura'),
('PROF0006', '55663322Q', 'Llengua i Literatura'),
('PROF0007', '68024680U', 'Tecnologia'),
('PROF0008', '68024691N', 'Matemàtiques'),
('PROF0009', '78901234H', 'Llengua i Literatura'),
('PROF0010', '78912345L', 'Tecnologia'),
('PROF0011', '88776655S', 'Llengua i Literatura'),
('PROF0012', '91357913X', 'Matemàtiques'),
('PROF0013', '97531924D', 'Llengua i Literatura'),
('PROF0014', '99887766D', 'Educació Física'),
('PROF0015', '50123456P', 'Matemàtiques'),
('PROF0016', '50234567Q', 'Llengua i Literatura'),
('PROF0017', '50345678R', 'Tecnologia'),
('PROF0018', '50456789S', 'Anglès'),
('PROF0019', '50567890T', 'Ciències Socials'),
('PROF0020', '50678901U', 'Educació Física'),
('PROF0021', '50789012V', 'Filosofia'),
('PROF0022', '50890123W', 'Llengua Catalana'),
('PROF0023', '50901234X', 'Informàtica'),
('PROF0024', '51012345Y', 'Biologia');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `professor_grup_classe`
--

CREATE TABLE `professor_grup_classe` (
  `id` int(11) NOT NULL,
  `dni_persona` varchar(20) NOT NULL,
  `nom_grup` varchar(100) NOT NULL,
  `fecha_asignacion` date DEFAULT curdate()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Tabla intermedia: Profesores que imparten en cada grupo/clase';

--
-- Volcado de datos para la tabla `professor_grup_classe`
--

INSERT INTO `professor_grup_classe` (`id`, `dni_persona`, `nom_grup`, `fecha_asignacion`) VALUES
(1, '12345679V', '1r BATX A', '2025-09-01'),
(2, '12345679V', '2n BATX A', '2025-09-01'),
(3, '14725836F', 'DAM 1', '2025-09-01'),
(4, '14725836F', 'DAW 1', '2025-09-01'),
(5, '68024680U', 'DAM 1', '2025-09-01'),
(6, '50234567Q', 'DAW 2', '2025-09-01'),
(7, '15975328J', '1r ESO A', '2025-09-01'),
(8, '15975328J', '2n ESO A', '2025-09-01'),
(9, '44556677C', '3r ESO A', '2025-09-01'),
(10, '78901234H', '4t ESO A', '2025-09-01'),
(11, '68024691N', '1r ESO B', '2025-09-01'),
(12, '91357913X', '2n ESO A', '2025-09-01'),
(13, '50123456P', '1r ESO A', '2025-09-01'),
(14, '50678901U', '2n BATX A', '2025-09-01'),
(15, '50456789S', '3r ESO A', '2025-09-01'),
(16, '50567890T', '4t ESO A', '2025-09-01');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `sessions`
--

CREATE TABLE `sessions` (
  `token_hash` varchar(255) NOT NULL COMMENT 'bcrypt o argon2id del token (nunca el token plano)',
  `username` varchar(50) NOT NULL COMMENT 'A quin usuari pertany la sessió',
  `ip_address` varchar(45) NOT NULL COMMENT 'IP desde la que se creó la sesión',
  `user_agent` varchar(255) NOT NULL COMMENT 'Navegador/app que hizo login',
  `session_start` datetime NOT NULL DEFAULT current_timestamp() COMMENT 'Quan es va crear la sessió',
  `last_activity` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp() COMMENT 'Última petición con este token (para timeout)',
  `session_end` datetime DEFAULT NULL COMMENT 'NULL = activa, fecha = cerrada (logout o timeout)'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `sessions`
--

INSERT INTO `sessions` (`token_hash`, `username`, `ip_address`, `user_agent`, `session_start`, `last_activity`, `session_end`) VALUES
('$2y$10$abc123...', 'tomeu.ramirez', '83.42.127.18', 'Mozilla/5.0 (Windows NT 10.0)', '2025-11-29 07:55:12', '2025-11-29 08:30:22', NULL),
('$2y$10$bcd890...', 'claudia.ortega', '85.48.201.78', 'Chrome/120 (Linux)', '2025-11-28 22:11:55', '2025-11-28 22:30:00', NULL),
('$2y$10$def456...', 'maria.garcia', '192.168.1.50', 'Mozilla/5.0 (Macintosh)', '2025-11-29 08:01:45', '2025-11-29 08:29:10', NULL),
('$2y$10$ghi789...', 'elena.blanco', '79.153.24.90', 'App-Evalis-Mobile/2.1', '2025-11-29 08:10:44', '2025-11-29 08:28:55', NULL),
('$2y$10$jkl012...', 'anna.rodriguez', '85.48.201.56', 'Mozilla/5.0 (Android 14)', '2025-11-29 08:04:55', '2025-11-29 08:15:30', NULL),
('$2y$10$mno345...', 'joan.vilaseca', '10.45.67.89', 'Mozilla/5.0 (iPhone)', '2025-11-28 22:45:10', '2025-11-29 07:20:00', NULL),
('$2y$10$pqr678...', 'ainab', '88.17.200.45', 'Evalis-Desktop/1.5', '2025-11-28 18:22:30', '2025-11-28 20:15:44', NULL),
('$2y$10$stu901...', 'margalida.crespi', '62.57.188.33', 'Chrome/119 (Android)', '2025-11-28 20:33:44', '2025-11-28 21:10:22', NULL),
('$2y$10$vwx234...', 'lidia.farre', '79.153.45.112', 'Firefox/118 (Windows)', '2025-11-28 20:45:01', '2025-11-28 21:05:11', NULL),
('$2y$10$yza567...', 'alba.vives', '87.123.45.67', 'Safari/17 (iPad)', '2025-11-28 21:30:22', '2025-11-28 22:00:00', NULL),
('02edfd511225e408c8256d1573416235235f76525a111eb218a5878073202ced', 'elena.blanco', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 14:15:12', '2026-05-22 14:15:27', NULL),
('037656267ad52120ad64d8b007dcfc3b9b09b84cbffd8428a966e42581127118', 'xavier.molins', '::1', 'unknown', '2026-05-06 14:16:51', '2026-05-06 14:16:51', NULL),
('059b875160def001011957416e2ec6fff7af5f7d7ce81d60c45902907ff1a691', 'anna.rodriguez', '127.0.0.1', 'Dalvik/2.1.0 (Linux; U; Android 16; sdk_gphone64_x86_64 Build/BE2A.250530.026.F3)', '2026-05-06 00:07:41', '2026-05-06 01:50:18', '2026-05-06 01:50:18'),
('0ac5445cec42d1487703cee9fc7ed1862b0ffcb10e793ea7b6f51fdb967f587f', 'elena.blanco', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 15:30:49', '2026-05-22 15:33:12', NULL),
('105c9abc85bf5eaa118f19a7749a9d7567f928d5da275c7034bec9413a7e564f', 'maria.garcia', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 23:03:25', '2026-05-22 23:07:45', NULL),
('1115b16ca868c02d1d629acca36c107ddab3cf77513d27ea05cf7174abe9a5f6', 'elena.blanco', '::1', 'unknown', '2026-05-06 16:22:13', '2026-05-06 16:22:13', NULL),
('1199709b9f4bc3b64329d563e788dc57f682b4f119962a2129ce362745ddc643', 'elena.blanco', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 15:36:18', '2026-05-22 15:36:40', NULL),
('1732be8169444d1c0721b8b7a30cc27104a924d14edfee1662fc2af8798bb515', 'anna.rodriguez', '127.0.0.1', 'Dalvik/2.1.0 (Linux; U; Android 16; sdk_gphone64_x86_64 Build/BE2A.250530.026.F3)', '2026-05-06 15:31:02', '2026-05-06 15:31:09', NULL),
('19196056abf67849faa9602a2cbc331adecce1f11b38067f2a59e5a49dfa066a', 'tomeu.ramirez', '::1', 'unknown', '2026-05-06 09:31:49', '2026-05-06 09:31:49', NULL),
('1e6dc426b0b5ad4908929fd5ad175ca2d496d4b7d8f77605cd1e9540c39eb980', 'anna.rodriguez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 12:11:47', '2026-05-22 12:12:04', NULL),
('2110d63830e1882d4e5c58097f4239040db3b690e897ae1a3d5874570467d846', 'anna.rodriguez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 13:07:26', '2026-05-22 13:07:26', NULL),
('2127e807b836ebca6554beae7564d29744c81c816a6231353e7d3d2fb4b7bb86', 'tomeu.ramirez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 18:46:45', '2026-05-22 18:46:48', NULL),
('244495825b2bdd0f34e944ee2cec7af5251e6d5322d1fed308b88dcf62ea50c8', 'xavier.molins', '::1', 'unknown', '2026-05-06 14:26:37', '2026-05-06 14:26:37', NULL),
('26378f86fb6cfe370a8a54f129ce5bf8c15e6b5c53e09ee92224a3375b29791c', 'elena.blanco', '::1', 'unknown', '2026-05-06 12:22:55', '2026-05-06 12:22:55', NULL),
('26abbadacff7e79da9511ca0d441ec6f92df6e73ca23509388ef14a926891169', 'anna.rodriguez', '127.0.0.1', 'Dalvik/2.1.0 (Linux; U; Android 16; sdk_gphone64_x86_64 Build/BE2A.250530.026.F3)', '2026-05-06 02:02:21', '2026-05-06 02:08:01', NULL),
('28cb96bc34ec76eeaf435b305edb5942fcd8a6e73ed827c3d569620809da6695', 'elena.blanco', '::1', 'unknown', '2026-05-06 13:04:34', '2026-05-06 13:04:34', NULL),
('2903a10c6167fd3398198ef92be59fff0d8b7b911b4f133fe7a75702bd5565e2', 'tomeu.ramirez', '::1', 'unknown', '2026-05-06 13:53:45', '2026-05-06 13:53:45', NULL),
('2cb98d2b22a829f4b9aa7f97f5a6941a34f4ea60a433c7e5133c74dde6cbd60a', 'maria.garcia', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 22:10:19', '2026-05-22 22:12:22', NULL),
('2ddf68e1dc1bab3bb4a89a2fc11f2b64635ea7a49bd543e3ec6435ad7090df39', 'anna.rodriguez', '127.0.0.1', 'Dalvik/2.1.0 (Linux; U; Android 16; sdk_gphone64_x86_64 Build/BE2A.250530.026.F3)', '2026-05-06 16:36:11', '2026-05-06 16:36:26', NULL),
('2e985ff46f66d2553f870848d68cc3d289e01d4c701f1c07315b169b168857fd', 'elena.blanco', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 15:10:52', '2026-05-22 15:10:52', NULL),
('3050ae8500dc6f5503119e9439afe8a3af574313271d9345df4cc81c954714f8', 'elena.blanco', '::1', 'unknown', '2026-05-06 12:18:22', '2026-05-06 12:18:22', NULL),
('32a65cdfa3ac0a86bc3cad85fa678eeb0606f7e8931404404beef1c27a9d89be', 'maria.garcia', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-23 00:22:20', '2026-05-23 00:22:27', NULL),
('32c9fdaea4df160c8ed3235a444dd7811b464050cd891a8506cb0c962b7d8132', 'anna.rodriguez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 14:50:42', '2026-05-22 14:50:45', NULL),
('33664a6bf6d76c60fc5b3e189b154838a3a9dd716e1d54f822981239e332d199', 'maria.garcia', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-23 00:05:38', '2026-05-23 00:05:41', NULL),
('3631ff3b4070a48fcf623dea49d4c3b63503caa49e8303e2ce19c61ab26f4ee6', 'anna.rodriguez', '::1', 'unknown', '2026-05-06 09:27:52', '2026-05-06 09:27:52', NULL),
('36d651fd18809089def18534d13befe6f8638f954e2f2de0bb427419dd539930', 'elena.blanco', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 11:30:16', '2026-05-22 11:31:37', NULL),
('37affeb573e5b3ebbda760faeb42ba6b6f866274c229c560e09b80ad92b8aa60', 'maria.garcia', '::1', 'unknown', '2026-05-06 15:00:29', '2026-05-06 15:00:29', NULL),
('3ac95fda9a1bf93346942e8692e7baad8ed0629489724941dce4b0118173ca75', 'anna.rodriguez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 12:16:43', '2026-05-22 12:16:47', NULL),
('40974cf8c1573a106afd7fc2eb8b537a8b30d9d08d3247cfd55eebccfd92b27c', 'maria.garcia', '::1', 'unknown', '2026-05-06 20:53:21', '2026-05-06 20:53:21', NULL),
('441b6e536e8326f799d614bdcf0e3d6c713c884584a988a3c54e0d67a21b4ca6', 'tomeu.ramirez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 11:55:58', '2026-05-22 11:56:26', NULL),
('44d9ce12b5913281f3bf8735c2b67a275b3aa081db921fcd684d855f3eda981b', 'tomeu.ramirez', '::1', 'unknown', '2026-05-06 13:45:19', '2026-05-06 13:45:19', NULL),
('493ba1b38f3c0a6db553f6d6fb2fd91dba6c85eb1f681264e670cebfbf12a60d', 'tomeu.ramirez', '::1', 'unknown', '2026-05-06 13:57:02', '2026-05-06 13:57:02', NULL),
('53a9753d9ce2978a7b461b988be69f9a8986e7a843fe07a30aa55ce8bfcdd0e8', 'tomeu.ramirez', '::1', 'unknown', '2026-05-06 10:13:40', '2026-05-06 10:13:40', NULL),
('565cd247cd039a712c8efa3b1d1d5f434a48016cf90e228ff2a18d676f742bd1', 'xavier.molins', '::1', 'unknown', '2026-05-06 14:21:29', '2026-05-06 14:21:29', NULL),
('56cedf434180064e5c5261004e82e65b9aa3d68a88596d3c4a918ef16070bc06', 'maria.garcia', '::1', 'unknown', '2026-05-06 15:06:27', '2026-05-06 15:06:27', NULL),
('5a8c50ba305eaa079eb7ef6574ca3e3900e859aa4acb3c5bd71aef64672313a7', 'anna.rodriguez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 12:22:50', '2026-05-22 12:22:52', NULL),
('5adc4d720c58522fc9563f6d6f6f11ee7e4932c8c049500ac8ffb165c58e7926', 'tomeu.ramirez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 18:38:13', '2026-05-22 18:38:16', NULL),
('5f6b4cb93485d6c21d96a001e90b0d8b4c0b11f11a3909fb06b9c8b42baa5fa9', 'anna.rodriguez', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/147.0.0.0 Safari/537.36', '2026-05-05 23:58:02', '2026-05-05 23:58:02', NULL),
('6018d1a1c8b61644fc9385c588119613b6f2af664863edbee02212d2c7f5dd9e', 'nuria.torres', '127.0.0.1', 'Dalvik/2.1.0 (Linux; U; Android 16; sdk_gphone64_x86_64 Build/BE2A.250530.026.F3)', '2026-05-06 20:23:14', '2026-05-06 20:24:44', NULL),
('64053469ff404fe941739a9e4fd192463e2395d2e2e8206e8a1f5f2c2cc6e812', 'anna.rodriguez', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/147.0.0.0 Safari/537.36', '2026-05-06 09:17:07', '2026-05-06 09:17:07', NULL),
('67b49da66119b8dc49d0b1b3ca0d5006a371da1ad4c577e947a162977d119b25', 'anna.rodriguez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 16:36:36', '2026-05-22 16:36:38', NULL),
('6989b4bb567bceb3c2ff0e4aebda0b407934bcf3c7282b20ebff17963229ab3f', 'tomeu.ramirez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 10:55:17', '2026-05-22 10:57:12', NULL),
('6a06f98bcf831b57c809d94cf17cad71b374f7a55a583ed2c14b3036d90e8af2', 'tomeu.ramirez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 18:41:01', '2026-05-22 18:41:03', NULL),
('6c5b9561c7230b79d4b9a024072a48c41e7835442ed96ef0e52eaa1d53ccbf8d', 'tomeu.ramirez', '::1', 'unknown', '2026-05-06 10:34:41', '2026-05-06 10:34:41', NULL),
('6f5e2e5400268d1ac5a06bf08d48a68fe3527e7857febd25211234a2f0b991f8', 'maria.garcia', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-23 00:15:02', '2026-05-23 00:15:09', NULL),
('772586a2dd92267c660d65490c29debaf226f6d6d2f35e35c7863860c853c5f8', 'elena.blanco', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 14:20:58', '2026-05-22 14:20:58', NULL),
('7c9190e47fda1390f489da0c4d41f4c00903517f6a20dd5067e49687c0259862', 'elena.blanco', '::1', 'unknown', '2026-05-06 13:04:53', '2026-05-06 13:04:53', NULL),
('7cdbcb173434bcc9f58b5e871a0266b5f4fb0f15e724364f317578fc854a601d', 'anna.rodriguez', '127.0.0.1', 'Dalvik/2.1.0 (Linux; U; Android 16; sdk_gphone64_x86_64 Build/BE2A.250530.026.F3)', '2026-05-06 19:06:54', '2026-05-06 19:07:07', NULL),
('81247277cf2debefe6d8813c9dbc4b851295ab851a2303ad780a464de9128de3', 'anna.rodriguez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 14:52:41', '2026-05-22 14:52:44', NULL),
('812fa36c0a28881f489ae1b70dbd61cac4bb885e4eb8df554165ea09e95d758f', 'tomeu.ramirez', '::1', 'unknown', '2026-05-06 14:05:17', '2026-05-06 14:05:17', NULL),
('83964c8f7c851ee94b97ed221601552e153fc9888eb05f360406d54a7ec60d92', 'anna.rodriguez', '127.0.0.1', 'Dalvik/2.1.0 (Linux; U; Android 16; sdk_gphone64_x86_64 Build/BE2A.250530.026.F3)', '2026-05-06 01:58:22', '2026-05-06 02:00:09', '2026-05-06 02:00:09'),
('84c1958fdca509330a123e999f781f080c071bb853b871c20b064b7811ff01d4', 'maria.garcia', '::1', 'unknown', '2026-05-06 16:09:52', '2026-05-06 16:09:52', NULL),
('8a4d2385a46e74a68ee035a9def154e90b12a601e496c909b69f916bf215b614', 'maria.garcia', '::1', 'unknown', '2026-05-06 16:12:00', '2026-05-06 16:12:00', NULL),
('8ae2d749cd22b7149fa798d49b3b93a0700710437c9635f40d542cb27a7b0fe2', 'anna.rodriguez', '127.0.0.1', 'Dalvik/2.1.0 (Linux; U; Android 16; sdk_gphone64_x86_64 Build/BE2A.250530.026.F3)', '2026-05-06 18:43:42', '2026-05-06 18:52:29', NULL),
('8c4893ed53292d1f074e1b1cd1c4f64dd8bbcc1f949f114f7d4904bf0cef4df0', 'elena.blanco', '::1', 'unknown', '2026-05-06 14:12:48', '2026-05-06 14:12:48', NULL),
('8ce2533979e4dc89a7cd8a8afb1046a187afb5dc550fa5f5dc9ff0c049dc68a6', 'elena.blanco', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 12:23:53', '2026-05-22 12:23:55', NULL),
('8f16b4a1527b5c6494cae81962f7ff2430a98e692182537336493cea5fbad11a', 'anna.rodriguez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 13:18:23', '2026-05-22 13:18:26', NULL),
('91e532bcf6017f8a20cdcd8f712b7a70f6fad5b8b5f9dd25777bf9b070f79d2e', 'xavier.molins', '::1', 'unknown', '2026-05-06 19:25:30', '2026-05-06 19:25:30', NULL),
('92688e70ef249fdefb70bcc510e9ad68786df1aaecd233f47419b84be65b3c5f', 'anna.rodriguez ', '127.0.0.1', 'Dalvik/2.1.0 (Linux; U; Android 16; sdk_gphone64_x86_64 Build/BE2A.250530.026.F3)', '2026-05-06 04:58:49', '2026-05-06 04:58:49', NULL),
('9335adf373813acb77a64cdddfb3fb2ce6e67bbad651fc0f26ea7378e0c283f7', 'elena.blanco', '::1', 'unknown', '2026-05-06 20:51:03', '2026-05-06 20:51:03', NULL),
('93c08850c505e498ab6522069065a4247daaabd003424f62490a5852ab3ed9ea', 'maria.garcia', '::1', 'unknown', '2026-05-06 19:37:56', '2026-05-06 19:37:56', NULL),
('9417a6db3ac96ad63e651014cae37a71fb9714cbc6156c7385f6f97538073e2a', 'anna.rodriguez', '127.0.0.1', 'Dalvik/2.1.0 (Linux; U; Android 16; sdk_gphone64_x86_64 Build/BE2A.250530.026.F3)', '2026-05-06 20:45:14', '2026-05-06 20:50:28', NULL),
('971cd2c07d533cb4618db507308d8b099bbceb42cf7aed992c792051f363de41', 'elena.blanco', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 14:59:32', '2026-05-22 14:59:32', NULL),
('979c6e6e56ff5d0082f949811cac96f177017876c58274e4333eefd741408a1d', 'elena.blanco', '::1', 'unknown', '2026-05-06 12:19:42', '2026-05-06 12:19:42', NULL),
('99de0d8a19ab5110f532666ddf27c0d5db9d6f3df0a4a05ad0b136abfac75d87', 'elena.blanco', '::1', 'unknown', '2026-05-06 16:21:05', '2026-05-06 16:21:05', NULL),
('9a2ea64128fac6e80b973f0579ed1a9e3b4fd33a5451c49729f95b0384955993', 'maria.garcia', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 10:51:25', '2026-05-22 10:51:52', NULL),
('9b660b6422c1f60e02a4910a0374a5a5814ce8b14420a791de1adcc454b59056', 'tomeu.ramirez', '::1', 'unknown', '2026-05-06 09:58:51', '2026-05-06 09:58:54', NULL),
('9b84e4cb236eb1ef85bd6d527bd91a0080935e3e103cd89c83dae909969ef81e', 'tomeu.ramirez', '::1', 'unknown', '2026-05-06 10:58:59', '2026-05-06 10:58:59', NULL),
('9ce878d841b3f8187aa26ab99277cb427de33d0ec3fad40bec64cc599cdda2b1', 'elena.blanco', '::1', 'unknown', '2026-05-06 13:43:45', '2026-05-06 13:43:45', NULL),
('9d813ef5e86336f60eddddb291787cd093db820c29f5c21cfd95b0d08592b586', 'elena.blanco', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 14:10:56', '2026-05-22 14:10:56', NULL),
('9f29de6e060e8d003af9d61c3821dbf220ca9c02ccf06149c0ced583d9ff6775', 'maria.garcia', '::1', 'unknown', '2026-05-06 15:10:39', '2026-05-06 15:10:39', NULL),
('9f98ccc8c418c4d674cf306eea428df59487f8b020361956cb0077fa28782955', 'tomeu.ramirez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 18:45:13', '2026-05-22 18:45:16', NULL),
('a098bd47610ddc188a1774ea99a20a41c3aed823d0dbda840515b7038d42892a', 'anna.rodriguez', '127.0.0.1', 'Dalvik/2.1.0 (Linux; U; Android 16; sdk_gphone64_x86_64 Build/BE2A.250530.026.F3)', '2026-05-06 15:12:37', '2026-05-06 15:13:47', NULL),
('a2c16bce95680bc7c84c3827f73da35f4f1582f84d44d1609eb88a5e2a6ec304', 'tomeu.ramirez', '::1', 'unknown', '2026-05-06 11:01:33', '2026-05-06 11:01:33', NULL),
('a4383be43c66b0ed77c95ec6b36c2afd9ef3c31dc5b83194dc8bb96430f2aa49', 'elena.blanco', '::1', 'unknown', '2026-05-06 13:05:40', '2026-05-06 13:05:40', NULL),
('a6e53628bd2dc8cd49fbbecd2e29263ecd716c216f6a9ceef0933b8dbf26f5e6', 'elena.blanco', '::1', 'unknown', '2026-05-06 13:14:11', '2026-05-06 13:14:11', NULL),
('a7a718aac3dfb9cbc0764da05c0516f24eb856e1e79d91c9250a38ae087a891c', 'anna.rodriguez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 12:17:11', '2026-05-22 12:17:11', NULL),
('a853de637cb534ef9bd04c4cab0478edc0ae93ad59780c0eaa3bf5e2f5543a3b', 'anna.rodriguez', '127.0.0.1', 'Dalvik/2.1.0 (Linux; U; Android 16; sdk_gphone64_x86_64 Build/BE2A.250530.026.F3)', '2026-05-06 01:53:07', '2026-05-06 01:58:07', '2026-05-06 01:58:07'),
('a91b2d258b3f2295dac293fe89b535db5fa7414219547b46ce68e0e9d0452431', 'anna.rodriguez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 13:20:43', '2026-05-22 13:20:45', NULL),
('ae2b9e4dafad82f790ecd619719d1b1a668414317a7eee764ad10138460c96eb', 'elena.blanco', '::1', 'unknown', '2026-05-06 14:42:50', '2026-05-06 14:42:50', NULL),
('b173fb5d4eb5c23c6213c8e829d79e8e00d477da22bb1cedc69f667d4f23a245', 'anna.rodriguez', '127.0.0.1', '771794427', '2026-05-22 10:38:42', '2026-05-22 11:21:18', NULL),
('b1e14aa1109fc5e0a2f9ad53f0bb1590d0fb145a1ab1a41c6cbbffb5a42d818c', 'anna.rodriguez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 16:26:09', '2026-05-22 16:29:33', NULL),
('b3c31d1097fc469ec482e924b8ef575e852ad783989f5dbee7df29f708f5e0ab', 'elena.blanco', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 14:52:58', '2026-05-22 14:52:58', NULL),
('b44560637d6c4b8ccc93cf489977f2032edc5e0420411b9ee508eb7c9c5db135', 'xavier.molins', '::1', 'unknown', '2026-05-06 19:22:24', '2026-05-06 19:22:24', NULL),
('b50bdd94b35191db30dac9c20b21e319fb9e5a11a1a88e41c7133f007fa05894', 'elena.blanco', '::1', 'unknown', '2026-05-06 13:02:33', '2026-05-06 13:02:33', NULL),
('ba8a0b37d2248ebbde6e576382f6c9fcf23a84df8ba57d08ae620de2ce65c3ff', 'tomeu.ramirez', '::1', 'unknown', '2026-05-06 10:01:39', '2026-05-06 10:01:42', NULL),
('bcf0c01d4d0d54200ce50e05bf52f847bb992fa7851336fd526f75106a750135', 'elena.blanco', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 15:05:34', '2026-05-22 15:05:34', NULL),
('bd26b40df8d95b164857d9f22c1d66936b3c4708ad07f7ddb8d928cbfa0c6c62', 'tomeu.ramirez', '::1', 'unknown', '2026-05-06 13:51:58', '2026-05-06 13:51:58', NULL),
('bd76ca3393f708346ccc52d6d45ba79b63d4f5e5434e689dd3a3d673714fad3b', 'anna.rodriguez', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/147.0.0.0 Safari/537.36', '2026-05-05 23:48:19', '2026-05-05 23:48:19', NULL),
('bdccf0b6fd63c1fcfab4547b65222927c6ac980dc526e85c55cb8cc29752e863', 'elena.blanco', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 17:15:59', '2026-05-22 17:16:43', NULL),
('bfc1d03999c8b11c04229ff68745f07b9077ba3d941a631c228ad1f9a41ea3ee', 'tomeu.ramirez', '::1', 'unknown', '2026-05-06 10:41:36', '2026-05-06 10:41:36', NULL),
('c09ebfa7cfab0f7df684df1a954cfcd13b3d03812f08ee7009efa12b0ee38a5e', 'tomeu.ramirez', '::1', 'unknown', '2026-05-06 13:49:54', '2026-05-06 13:49:54', NULL),
('c24883bcf7a422f483aea900f2821c561e66c1b39fbc32e843fde7077c12a1ae', 'maria.garcia', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 22:30:46', '2026-05-22 22:30:59', NULL),
('c282ee72729042ffdd6d8f4daed50c565d89ee83241cef18b2f9c1d986ffd7b7', 'tomeu.ramirez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 19:09:38', '2026-05-22 19:09:46', NULL),
('c821931c31178595e4b8aa1948b093319a0bedb15400e8a2969751555b157e6d', 'elena.blanco', '::1', 'unknown', '2026-05-06 19:33:39', '2026-05-06 19:33:39', NULL),
('c97274f3ccc144c28200854d79eedbc5dfb1bb6eb711c17a97eb63ddb64ca50e', 'tomeu.ramirez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 19:22:21', '2026-05-22 19:39:09', NULL),
('cb38e75a313e79136422cf24d0b0d8b5faed5e020f35e2ba1302640f80de6253', 'anna.rodriguez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 16:23:07', '2026-05-22 16:23:07', NULL),
('cd3d03301e20d66f17b45e71712735df072a19937a96f2f8ce7a49e0056738e7', 'tomeu.ramirez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 18:47:30', '2026-05-22 18:47:58', NULL),
('d617ccd1b111e43deb366fbaa13452304aa14f9302ee97c5780992bafddd6018', 'tomeu.ramirez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 18:50:10', '2026-05-22 18:55:45', NULL),
('d6e91b36cfcb55f3b3d0bbc58cf0aa2d5f7ba76efe647207e1d5cdbbf3ea7037', 'elena.blanco', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 15:18:38', '2026-05-22 15:18:38', NULL),
('d7ad415241ec93416954dcb630dffa925b08ee0939adf379148f0c91f4caea71', 'tomeu.ramirez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 19:16:15', '2026-05-22 19:16:20', NULL),
('d7c5fc4faa0c55929f6cea0f9b675f852aae3f76add9f994c606fe1bac3e9219', 'maria.garcia', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-23 00:14:21', '2026-05-23 00:14:34', NULL),
('d7ece451d22f26d8a773ec0bdd41022c010af816c7cfb66b75c0988cfabb833b', 'anna.rodriguez', '::1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/147.0.0.0 Safari/537.36', '2026-05-05 23:57:34', '2026-05-05 23:57:34', NULL),
('d80947c20ae3c49553d45ea26e6bad51459cb20ef5d06d48996bd178eacde5bf', 'anna.rodriguez', '127.0.0.1', 'Dalvik/2.1.0 (Linux; U; Android 16; sdk_gphone64_x86_64 Build/BE2A.250530.026.F3)', '2026-05-06 02:08:38', '2026-05-06 02:08:41', NULL),
('dba12085d9d3f036245290b1b33ab07ea039dfd02251ce2a11b6289c6f6f3f46', 'anna.rodriguez', '127.0.0.1', 'Dalvik/2.1.0 (Linux; U; Android 16; sdk_gphone64_x86_64 Build/BE2A.250530.026.F3)', '2026-05-06 04:55:24', '2026-05-06 04:55:24', NULL),
('dbe040f4eba41bb456952d85fe053ee5b24494de31e9ad6e0b4da5168a36b5ea', 'tomeu.ramirez', '::1', 'unknown', '2026-05-06 10:57:06', '2026-05-06 10:57:06', NULL),
('dc8438b05e578834c2b5d0f72c7313b641b4c62e875d658022b2375584a47ba2', 'tomeu.ramirez', '::1', 'unknown', '2026-05-06 09:56:00', '2026-05-06 09:56:03', NULL),
('dccdf39cac6fb2932f4b71bd506e9c0926e7e55d4cc8b30daeac3794df464a67', 'elena.blanco', '::1', 'unknown', '2026-05-06 16:26:15', '2026-05-06 16:26:15', NULL),
('e05b9a2c1591cf7236811659b4f0bc7227b83fb9be9f44a92c2463647eee9c2a', 'anna.rodriguez', '127.0.0.1', '771794427', '2026-05-22 11:22:01', '2026-05-22 11:50:40', NULL),
('e4efae91e39821deb805b3da45342a15a61d49451dd9c09dc4f0def5b0825638', 'elena.blanco', '::1', 'unknown', '2026-05-06 12:29:42', '2026-05-06 12:29:42', NULL),
('e7f049164cb12da1e012e7bb8d505f9a4fccd13f22f12cf7376d0920cc225073', 'anna.rodriguez', '127.0.0.1', 'Dalvik/2.1.0 (Linux; U; Android 16; sdk_gphone64_x86_64 Build/BE2A.250530.026.F3)', '2026-05-06 09:13:57', '2026-05-06 09:14:03', NULL),
('e9767054095c4453ffea6c2ec177e9eeb22621d8673f0f74b5f8d9989a3b5e7c', 'tomeu.ramirez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 12:24:11', '2026-05-22 12:24:19', NULL),
('eaad3230bf5554013af334fa712968a227ad4ca01a587c85c6ed3b085688dffe', 'anna.rodriguez', '127.0.0.1', 'Dalvik/2.1.0 (Linux; U; Android 16; sdk_gphone64_x86_64 Build/BE2A.250530.026.F3)', '2026-05-06 05:21:16', '2026-05-06 05:22:42', NULL),
('eba237271350e5152270a2e4a22e5e6ef3758753a97620e6c2893edc2ffa15aa', 'maria.garcia', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 12:23:10', '2026-05-22 12:23:23', NULL),
('f616eca69fd81b8f2f79fd1091473b80fe8e061e2dbe06cb336d94139e32eaa6', 'elena.blanco', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 14:49:57', '2026-05-22 14:49:57', NULL),
('f864e801c7ebd147ef79402f32a3d1a3dd9e0ec41b1b0c5fd4e54666b0db9f0a', 'anna.rodriguez', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-22 13:08:14', '2026-05-22 13:08:14', NULL),
('f88ccb65efff7fd31e9d314d658ecfa002d9af8a7a4cf7b2a23613770c80250c', 'tomeu.ramirez', '::1', 'unknown', '2026-05-06 10:06:03', '2026-05-06 10:06:05', NULL),
('fa231583c3318ffd6a36f257abd69984ca440acea5142ae8c3991920f3c40080', 'maria.garcia', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-23 00:07:51', '2026-05-23 00:07:59', NULL),
('fdd68ed531cf7b79a4818336a3f25a35d77e66eec284838d4a9821bca33139ab', 'tomeu.ramirez', '::1', 'unknown', '2026-05-06 10:08:42', '2026-05-06 10:08:45', NULL),
('fe58cd165d8c246cc8fb7ff6383e1854a6f1b8c8db1d9f1623a4bdd8e2a09316', 'maria.garcia', '::1', '2f0bcd739a35981783e2b64f145eef2202a4eaff7f652d9645c6aad77ae90826', '2026-05-23 00:20:03', '2026-05-23 00:20:08', NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tutor_fct`
--

CREATE TABLE `tutor_fct` (
  `id_intern_professor` varchar(20) NOT NULL,
  `grup_classe_nom` varchar(20) NOT NULL,
  `curs_academic` varchar(9) NOT NULL DEFAULT '2025-2026',
  `actiu_com_a_tutor` tinyint(1) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tutor_fct`
--

INSERT INTO `tutor_fct` (`id_intern_professor`, `grup_classe_nom`, `curs_academic`, `actiu_com_a_tutor`) VALUES
('PROF0001', 'DAM 1', '2025-2026', 1),
('PROF0001', 'DAW 1', '2025-2026', 1),
('PROF0002', 'DAW 2', '2025-2026', 1),
('PROF0003', '1r BATX A', '2025-2026', 1),
('PROF0003', '2n BATX A', '2025-2026', 1),
('PROF0004', '1r ESO A', '2025-2026', 1),
('PROF0005', '1r ESO B', '2025-2026', 1),
('PROF0006', '2n ESO A', '2025-2026', 1),
('PROF0015', '3r ESO A', '2025-2026', 1),
('PROF0016', '4t ESO A', '2025-2026', 1),
('PROF0017', 'DAM 1', '2025-2026', 1),
('PROF0018', 'DAW 2', '2025-2026', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `unitat`
--

CREATE TABLE `unitat` (
  `id` int(11) NOT NULL,
  `modul` varchar(10) NOT NULL,
  `nom` varchar(100) NOT NULL,
  `hores` int(11) NOT NULL,
  `tipus` enum('UF','RA','Altres') NOT NULL DEFAULT 'UF',
  `num` int(2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `unitat`
--

INSERT INTO `unitat` (`id`, `modul`, `nom`, `hores`, `tipus`, `num`) VALUES
(1, 'INF06', 'PHP i Laravel', 40, 'RA', 1),
(2, 'INF06', 'JavaScript avançat', 30, 'RA', 2),
(3, 'INF06', 'APIs REST i consum', 25, 'RA', 3),
(4, 'INF06', 'Seguretat en aplicacions web', 25, 'RA', 4),
(5, 'INF03', 'Estructures de control', 35, 'RA', 1),
(6, 'INF03', 'Funcions i mòduls', 30, 'RA', 2),
(7, 'INF03', 'Programació orientada a objectes bàsica', 40, 'RA', 3),
(8, 'INF03', 'Gestió de fitxers', 35, 'RA', 4),
(9, 'INF02', 'Modelatge de bases de dades', 30, 'RA', 1),
(10, 'INF02', 'SQL avançat', 40, 'RA', 2),
(11, 'INF02', 'Procediments emmagatzemats i triggers', 30, 'RA', 3),
(12, 'FOL14', 'Contractació laboral', 30, 'RA', 1),
(13, 'FOL14', 'Seguretat social i nòmines', 30, 'RA', 2),
(14, 'INF01', 'Instal·lació de sistemes operatius', 40, 'RA', 1),
(15, 'INF01', 'Maquinari i components', 40, 'RA', 2),
(16, 'INF01', 'Xarxes locals', 40, 'RA', 3),
(17, 'INF05', 'HTML5 i CSS3', 40, 'RA', 1),
(18, 'INF05', 'Accessibilitat i usabilitat web', 40, 'RA', 2),
(19, 'INF08', 'Administració Windows Server', 55, 'RA', 1),
(20, 'INF08', 'Administració Linux', 55, 'RA', 2),
(21, 'LLC02', 'Comunicació professional escrita', 40, 'RA', 1),
(22, 'LLC02', 'Comunicació oral en català', 40, 'RA', 2),
(23, 'ANG04', 'Technical English for IT', 50, 'RA', 1),
(24, 'ANG04', 'Writing reports and emails', 30, 'RA', 2),
(25, 'INF09', 'Ciberseguretat bàsica', 45, 'RA', 1),
(26, 'INF09', 'Xifrat i signatura digital', 45, 'RA', 0),
(27, 'INF04', 'Git i control de versions', 30, 'RA', 0),
(28, 'INF04', 'Entorns integrats (IDE)', 30, 'RA', 0),
(29, 'INF04', 'Metodologies àgils', 30, 'RA', 0),
(30, 'INF07', 'Optimització de consultes SQL', 40, 'RA', 0),
(31, 'INF07', 'Backup i recuperació', 30, 'RA', 0),
(32, 'INF07', 'Administració MySQL/MariaDB', 30, 'RA', 0);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuari`
--

CREATE TABLE `usuari` (
  `username` varchar(50) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `foto_carnet` varchar(255) DEFAULT NULL,
  `data_alta` date DEFAULT curdate(),
  `dni_persona` varchar(9) NOT NULL,
  `edat` int(3) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuari`
--

INSERT INTO `usuari` (`username`, `password_hash`, `foto_carnet`, `data_alta`, `dni_persona`, `edat`) VALUES
('ainab', '$2y$10$rNupMLIFtVQl9kYJK/vwYe.c9kVpUYZaUn3E.Yg5riDidk1ZPRVce', NULL, '2025-11-29', '14725836F', 34),
('alba.vives', '$2y$10$8Qz9jY5fZfZfZfZfZu9jY5fZfZfZfZfZfZfZfZfZfZfZfZfZfZ', NULL, '2025-11-29', '33445599T', 22),
('anna.rodriguez', '$2y$10$dPwTkqARWEFf0EZsfArE6O2vq3g5./B0JM2JGj1NGly31kXl8kVVy', 'https://veridas.com/wp-content/uploads/2021/08/frontal-dni-40.jpeg', '2025-11-29', '11223344E', 23),
('claudia.ortega', '$2y$10$8Qz9jY5fZfZfZfZfZu9jY5fZfZfZfZfZfZfZfZfZfZfZfZfZfZ', NULL, '2025-11-29', '22334455I', 22),
('elena.blanco', '$2y$10$dPwTkqARWEFf0EZsfArE6O2vq3g5./B0JM2JGj1NGly31kXl8kVVy', NULL, '2025-11-29', '12345679V', 35),
('eric.guillen', '$2y$10$8Qz9jY5fZfZfZfZfZu9jY5fZfZfZfZfZfZfZfZfZfZfZfZfZfZfZ', NULL, '2025-11-29', '25896314I', 21),
('guillem.cervera', '$2y$10$8Qz9jY5fZfZfZfZfZu9jY5fZfZfZfZfZfZfZfZfZfZfZfZfZfZ', NULL, '2025-11-29', '13579246A', 25),
('ivan.riera', '$2y$10$8Qz9jY5fZfZfZfZfZu9jY5fZfZfZfZfZfZfZfZfZfZfZfZfZfZ', NULL, '2025-11-29', '21098765Y', 28),
('joan.vilaseca', '$2y$10$8Qz9jY5fZfZfZfZfZu9jY5fZfZfZfZfZfZfZfZfZfZfZfZfZfZ', NULL, '2025-11-29', '35791357R', 47),
('lidia.farre', '$2y$10$8Qz9jY5fZfZfZfZfZu9jY5fZfZfZfZfZfZfZfZfZfZfZfZfZfZfZ', NULL, '2025-11-29', '22334411R', 24),
('marc.escudero', '$2y$10$8Qz9jY5fZfZfZfZfZu9jY5fZfZfZfZfZfZfZfZfZfZfZfZfZfZ', NULL, '2025-11-29', '24681357C', 28),
('margalida.crespi', '$2y$10$8Qz9jY5fZfZfZfZfZu9jY5fZfZfZfZfZfZfZfZfZfZfZfZfZ', NULL, '2025-11-29', '13579136Z', 26),
('maria.garcia', '$2y$10$dPwTkqARWEFf0EZsfArE6O2vq3g5./B0JM2JGj1NGly31kXl8kVVy', NULL, '2025-11-29', '12345678A', 40),
('marina.canal', '$2y$10$8Qz9jY5fZfZfZfZfZu9jY5fZfZfZfZfZfZfZfZfZfZfZfZfZfZ', NULL, '2025-11-29', '24680246Q', 30),
('marta.sanchez', '$2y$10$8Qz9jY5fZfZfZfZfZu9jY5fZfZfZfZfZfZfZfZfZfZfZfZfZfZ', NULL, '2025-11-29', '25123456K', 33),
('noelia.segura', '$2y$10$8Qz9jY5fZfZfZfZfZu9jY5fZfZfZfZfZfZfZfZfZfZfZfZfZfZ', NULL, '2025-11-29', '15975328J', 36),
('nuria.torres', '$2y$10$dPwTkqARWEFf0EZsfArE6O2vq3g5./B0JM2JGj1NGly31kXl8kVVy', NULL, '2025-11-29', '34567890M', 21),
('quim.rovira', '$2y$10$8Qz9jY5fZfZfZfZu9jY5fZfZfZfZfZfZfZfZfZfZfZfZfZfZ', NULL, '2025-11-29', '13579135P', 22),
('silvia.moreno', '$2y$10$8Qz9jY5fZfZfZfZfZu9jY5fZfZfZfZfZfZfZfZfZfZfZfZfZfZ', NULL, '2025-11-29', '33445566G', 43),
('tomeu.ramirez', '$2y$10$dPwTkqARWEFf0EZsfArE6O2vq3g5./B0JM2JGj1NGly31kXl8kVVy', NULL, '2025-11-29', '02468024Y', 50),
('xavier.molins', '$2y$10$dPwTkqARWEFf0EZsfArE6O2vq3g5./B0JM2JGj1NGly31kXl8kVVy', NULL, '2025-11-29', '11234567N', 49);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `administradors`
--
ALTER TABLE `administradors`
  ADD PRIMARY KEY (`dni_administrador`);

--
-- Indices de la tabla `asistencia_per_hora`
--
ALTER TABLE `asistencia_per_hora`
  ADD PRIMARY KEY (`id_hora`,`nia`),
  ADD KEY `fk_asistencia_alumne` (`nia`);

--
-- Indices de la tabla `assignatura`
--
ALTER TABLE `assignatura`
  ADD PRIMARY KEY (`codi`);

--
-- Indices de la tabla `assignatura_professor`
--
ALTER TABLE `assignatura_professor`
  ADD PRIMARY KEY (`codi_assig`,`id_intern_prof`),
  ADD KEY `fk_professor_id` (`id_intern_prof`);

--
-- Indices de la tabla `centre`
--
ALTER TABLE `centre`
  ADD PRIMARY KEY (`id_centre`);

--
-- Indices de la tabla `centre_administradors`
--
ALTER TABLE `centre_administradors`
  ADD PRIMARY KEY (`dni_administrador`,`id_centre`),
  ADD KEY `fk_ca_centre` (`id_centre`);

--
-- Indices de la tabla `consentiments`
--
ALTER TABLE `consentiments`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_consentiment_alumne` (`nia`);

--
-- Indices de la tabla `contractes`
--
ALTER TABLE `contractes`
  ADD PRIMARY KEY (`id_intern_professor`,`id_centre`,`data_alta`),
  ADD KEY `fk_contracte_centre` (`id_centre`);

--
-- Indices de la tabla `directiva`
--
ALTER TABLE `directiva`
  ADD PRIMARY KEY (`id_intern_professor`);

--
-- Indices de la tabla `empresa_fct_practiques`
--
ALTER TABLE `empresa_fct_practiques`
  ADD PRIMARY KEY (`id_practica`),
  ADD KEY `fk_practiques_estudiant` (`nia`);

--
-- Indices de la tabla `estado_evaluacion`
--
ALTER TABLE `estado_evaluacion`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `estudiants`
--
ALTER TABLE `estudiants`
  ADD PRIMARY KEY (`nia`),
  ADD UNIQUE KEY `uniq_dni_estudiant` (`dni_persona`);

--
-- Indices de la tabla `estudiants_grupclasse`
--
ALTER TABLE `estudiants_grupclasse`
  ADD PRIMARY KEY (`nia`,`nom_grup`),
  ADD KEY `fk_grupclasse_nom` (`nom_grup`);

--
-- Indices de la tabla `estudiants_historia`
--
ALTER TABLE `estudiants_historia`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_historia_estudiant` (`nia`);

--
-- Indices de la tabla `estudios_centre`
--
ALTER TABLE `estudios_centre`
  ADD PRIMARY KEY (`id`),
  ADD KEY `id_centre` (`id_centre`);

--
-- Indices de la tabla `estudis`
--
ALTER TABLE `estudis`
  ADD PRIMARY KEY (`nia`,`nom_estudi`,`curs_inici`);

--
-- Indices de la tabla `gestio_absencies`
--
ALTER TABLE `gestio_absencies`
  ADD PRIMARY KEY (`id_absencia`),
  ADD KEY `fk_absencia_professor` (`id_intern_professor`);

--
-- Indices de la tabla `grupclasse_assignatura`
--
ALTER TABLE `grupclasse_assignatura`
  ADD PRIMARY KEY (`nom_grupclasse`,`codi_assignatura`),
  ADD KEY `codi_assignatura` (`codi_assignatura`);

--
-- Indices de la tabla `grup_classe`
--
ALTER TABLE `grup_classe`
  ADD PRIMARY KEY (`nom`);

--
-- Indices de la tabla `historic_mitjanes`
--
ALTER TABLE `historic_mitjanes`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_historic_estudiant` (`nia`);

--
-- Indices de la tabla `hores_de_classe`
--
ALTER TABLE `hores_de_classe`
  ADD PRIMARY KEY (`id_hora`),
  ADD UNIQUE KEY `uk_hora_unica` (`grup_classe`,`codi_assignatura`,`data_hora_inici`),
  ADD KEY `fk_hora_assignatura` (`codi_assignatura`);

--
-- Indices de la tabla `hores_de_classe_professors`
--
ALTER TABLE `hores_de_classe_professors`
  ADD PRIMARY KEY (`id_hora`,`id_professor`),
  ADD KEY `fk_hcp_professor` (`id_professor`);

--
-- Indices de la tabla `login_logs`
--
ALTER TABLE `login_logs`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_login_logs_usuari` (`username`);

--
-- Indices de la tabla `matricula`
--
ALTER TABLE `matricula`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `uk_alumne_assignatura` (`nia`,`codi_assignatura`),
  ADD KEY `fk_matricula_assignatura` (`codi_assignatura`),
  ADD KEY `fk_matricula_grupclasse` (`nom_grupclasse`);

--
-- Indices de la tabla `modul`
--
ALTER TABLE `modul`
  ADD PRIMARY KEY (`nom`),
  ADD KEY `fk_modul_assignatura` (`codi_assignatura`);

--
-- Indices de la tabla `notes`
--
ALTER TABLE `notes`
  ADD PRIMARY KEY (`nia`,`uf_id`),
  ADD KEY `fk_notes_uf` (`uf_id`);

--
-- Indices de la tabla `observacions_incidencies`
--
ALTER TABLE `observacions_incidencies`
  ADD PRIMARY KEY (`id_observacio`),
  ADD KEY `fk_obs_practica` (`id_practica`),
  ADD KEY `fk_obs_autor_tutor` (`autor_id`);

--
-- Indices de la tabla `persona`
--
ALTER TABLE `persona`
  ADD PRIMARY KEY (`dni`),
  ADD UNIQUE KEY `email` (`email`),
  ADD KEY `fk_persona_centre` (`id_centre`);

--
-- Indices de la tabla `professor`
--
ALTER TABLE `professor`
  ADD PRIMARY KEY (`id_intern`),
  ADD UNIQUE KEY `uniq_dni_professor` (`dni_persona`);

--
-- Indices de la tabla `professor_grup_classe`
--
ALTER TABLE `professor_grup_classe`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `uk_profesor_grupo` (`dni_persona`,`nom_grup`),
  ADD KEY `nom_grup` (`nom_grup`);

--
-- Indices de la tabla `sessions`
--
ALTER TABLE `sessions`
  ADD PRIMARY KEY (`token_hash`),
  ADD KEY `fk_session_usuari` (`username`);

--
-- Indices de la tabla `tutor_fct`
--
ALTER TABLE `tutor_fct`
  ADD PRIMARY KEY (`id_intern_professor`,`grup_classe_nom`,`curs_academic`),
  ADD KEY `fk_tutorfct_grups_grup_classe` (`grup_classe_nom`);

--
-- Indices de la tabla `unitat`
--
ALTER TABLE `unitat`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `uk_modul_nom` (`modul`,`nom`);

--
-- Indices de la tabla `usuari`
--
ALTER TABLE `usuari`
  ADD PRIMARY KEY (`username`),
  ADD UNIQUE KEY `uniq_dni` (`dni_persona`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `centre`
--
ALTER TABLE `centre`
  MODIFY `id_centre` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT de la tabla `consentiments`
--
ALTER TABLE `consentiments`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- AUTO_INCREMENT de la tabla `empresa_fct_practiques`
--
ALTER TABLE `empresa_fct_practiques`
  MODIFY `id_practica` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=25;

--
-- AUTO_INCREMENT de la tabla `estudiants_historia`
--
ALTER TABLE `estudiants_historia`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=70;

--
-- AUTO_INCREMENT de la tabla `estudios_centre`
--
ALTER TABLE `estudios_centre`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=34;

--
-- AUTO_INCREMENT de la tabla `gestio_absencies`
--
ALTER TABLE `gestio_absencies`
  MODIFY `id_absencia` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT de la tabla `historic_mitjanes`
--
ALTER TABLE `historic_mitjanes`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- AUTO_INCREMENT de la tabla `hores_de_classe`
--
ALTER TABLE `hores_de_classe`
  MODIFY `id_hora` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=25;

--
-- AUTO_INCREMENT de la tabla `login_logs`
--
ALTER TABLE `login_logs`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=168;

--
-- AUTO_INCREMENT de la tabla `matricula`
--
ALTER TABLE `matricula`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT de la tabla `observacions_incidencies`
--
ALTER TABLE `observacions_incidencies`
  MODIFY `id_observacio` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT de la tabla `professor_grup_classe`
--
ALTER TABLE `professor_grup_classe`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- AUTO_INCREMENT de la tabla `unitat`
--
ALTER TABLE `unitat`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=33;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `administradors`
--
ALTER TABLE `administradors`
  ADD CONSTRAINT `fk_administradors_persona` FOREIGN KEY (`dni_administrador`) REFERENCES `persona` (`dni`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `asistencia_per_hora`
--
ALTER TABLE `asistencia_per_hora`
  ADD CONSTRAINT `fk_asistencia_alumne` FOREIGN KEY (`nia`) REFERENCES `estudiants` (`nia`) ON DELETE CASCADE,
  ADD CONSTRAINT `fk_asistencia_hora` FOREIGN KEY (`id_hora`) REFERENCES `hores_de_classe` (`id_hora`) ON DELETE CASCADE;

--
-- Filtros para la tabla `assignatura_professor`
--
ALTER TABLE `assignatura_professor`
  ADD CONSTRAINT `fk_assignatura_codi` FOREIGN KEY (`codi_assig`) REFERENCES `assignatura` (`codi`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_professor_id` FOREIGN KEY (`id_intern_prof`) REFERENCES `professor` (`id_intern`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `centre_administradors`
--
ALTER TABLE `centre_administradors`
  ADD CONSTRAINT `fk_ca_administrador` FOREIGN KEY (`dni_administrador`) REFERENCES `administradors` (`dni_administrador`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_ca_centre` FOREIGN KEY (`id_centre`) REFERENCES `centre` (`id_centre`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `consentiments`
--
ALTER TABLE `consentiments`
  ADD CONSTRAINT `fk_consentiment_alumne` FOREIGN KEY (`nia`) REFERENCES `estudiants` (`nia`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `contractes`
--
ALTER TABLE `contractes`
  ADD CONSTRAINT `fk_contracte_centre` FOREIGN KEY (`id_centre`) REFERENCES `centre` (`id_centre`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_contracte_professor` FOREIGN KEY (`id_intern_professor`) REFERENCES `professor` (`id_intern`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `directiva`
--
ALTER TABLE `directiva`
  ADD CONSTRAINT `fk_directiva_professor` FOREIGN KEY (`id_intern_professor`) REFERENCES `professor` (`id_intern`) ON UPDATE CASCADE;

--
-- Filtros para la tabla `empresa_fct_practiques`
--
ALTER TABLE `empresa_fct_practiques`
  ADD CONSTRAINT `fk_practiques_estudiant` FOREIGN KEY (`nia`) REFERENCES `estudiants` (`nia`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `estudiants`
--
ALTER TABLE `estudiants`
  ADD CONSTRAINT `estudiants_ibfk_1` FOREIGN KEY (`dni_persona`) REFERENCES `persona` (`dni`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `estudiants_grupclasse`
--
ALTER TABLE `estudiants_grupclasse`
  ADD CONSTRAINT `fk_estudiant_nia` FOREIGN KEY (`nia`) REFERENCES `estudiants` (`nia`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_grupclasse_nom` FOREIGN KEY (`nom_grup`) REFERENCES `grup_classe` (`nom`) ON UPDATE CASCADE;

--
-- Filtros para la tabla `estudiants_historia`
--
ALTER TABLE `estudiants_historia`
  ADD CONSTRAINT `fk_historia_estudiant` FOREIGN KEY (`nia`) REFERENCES `estudiants` (`nia`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `estudios_centre`
--
ALTER TABLE `estudios_centre`
  ADD CONSTRAINT `estudios_centre_ibfk_1` FOREIGN KEY (`id_centre`) REFERENCES `centre` (`id_centre`) ON DELETE CASCADE;

--
-- Filtros para la tabla `estudis`
--
ALTER TABLE `estudis`
  ADD CONSTRAINT `fk_estudis_estudiante` FOREIGN KEY (`nia`) REFERENCES `estudiants` (`nia`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `gestio_absencies`
--
ALTER TABLE `gestio_absencies`
  ADD CONSTRAINT `fk_absencia_professor` FOREIGN KEY (`id_intern_professor`) REFERENCES `professor` (`id_intern`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `grupclasse_assignatura`
--
ALTER TABLE `grupclasse_assignatura`
  ADD CONSTRAINT `grupclasse_assignatura_ibfk_1` FOREIGN KEY (`nom_grupclasse`) REFERENCES `grup_classe` (`nom`),
  ADD CONSTRAINT `grupclasse_assignatura_ibfk_2` FOREIGN KEY (`codi_assignatura`) REFERENCES `assignatura` (`codi`);

--
-- Filtros para la tabla `historic_mitjanes`
--
ALTER TABLE `historic_mitjanes`
  ADD CONSTRAINT `fk_historic_estudiant` FOREIGN KEY (`nia`) REFERENCES `estudiants` (`nia`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `hores_de_classe`
--
ALTER TABLE `hores_de_classe`
  ADD CONSTRAINT `fk_hora_assignatura` FOREIGN KEY (`codi_assignatura`) REFERENCES `assignatura` (`codi`),
  ADD CONSTRAINT `fk_hora_grup` FOREIGN KEY (`grup_classe`) REFERENCES `grup_classe` (`nom`);

--
-- Filtros para la tabla `hores_de_classe_professors`
--
ALTER TABLE `hores_de_classe_professors`
  ADD CONSTRAINT `fk_hcp_hora` FOREIGN KEY (`id_hora`) REFERENCES `hores_de_classe` (`id_hora`) ON DELETE CASCADE,
  ADD CONSTRAINT `fk_hcp_professor` FOREIGN KEY (`id_professor`) REFERENCES `professor` (`id_intern`);

--
-- Filtros para la tabla `login_logs`
--
ALTER TABLE `login_logs`
  ADD CONSTRAINT `fk_login_logs_usuari` FOREIGN KEY (`username`) REFERENCES `usuari` (`username`) ON UPDATE CASCADE;

--
-- Filtros para la tabla `matricula`
--
ALTER TABLE `matricula`
  ADD CONSTRAINT `fk_matricula_assignatura` FOREIGN KEY (`codi_assignatura`) REFERENCES `assignatura` (`codi`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_matricula_estudiant` FOREIGN KEY (`nia`) REFERENCES `estudiants` (`nia`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_matricula_grupclasse` FOREIGN KEY (`nom_grupclasse`) REFERENCES `grup_classe` (`nom`) ON DELETE SET NULL ON UPDATE CASCADE;

--
-- Filtros para la tabla `modul`
--
ALTER TABLE `modul`
  ADD CONSTRAINT `fk_modul_assignatura` FOREIGN KEY (`codi_assignatura`) REFERENCES `assignatura` (`codi`);

--
-- Filtros para la tabla `notes`
--
ALTER TABLE `notes`
  ADD CONSTRAINT `fk_notes_alumne` FOREIGN KEY (`nia`) REFERENCES `estudiants` (`nia`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_notes_uf` FOREIGN KEY (`uf_id`) REFERENCES `unitat` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `observacions_incidencies`
--
ALTER TABLE `observacions_incidencies`
  ADD CONSTRAINT `fk_obs_autor_tutor` FOREIGN KEY (`autor_id`) REFERENCES `tutor_fct` (`id_intern_professor`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_obs_practica` FOREIGN KEY (`id_practica`) REFERENCES `empresa_fct_practiques` (`id_practica`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `persona`
--
ALTER TABLE `persona`
  ADD CONSTRAINT `fk_persona_centre` FOREIGN KEY (`id_centre`) REFERENCES `centre` (`id_centre`) ON UPDATE CASCADE;

--
-- Filtros para la tabla `professor`
--
ALTER TABLE `professor`
  ADD CONSTRAINT `professor_ibfk_1` FOREIGN KEY (`dni_persona`) REFERENCES `persona` (`dni`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `professor_grup_classe`
--
ALTER TABLE `professor_grup_classe`
  ADD CONSTRAINT `professor_grup_classe_ibfk_1` FOREIGN KEY (`dni_persona`) REFERENCES `professor` (`dni_persona`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `professor_grup_classe_ibfk_2` FOREIGN KEY (`nom_grup`) REFERENCES `grup_classe` (`nom`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `sessions`
--
ALTER TABLE `sessions`
  ADD CONSTRAINT `fk_session_usuari` FOREIGN KEY (`username`) REFERENCES `usuari` (`username`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `tutor_fct`
--
ALTER TABLE `tutor_fct`
  ADD CONSTRAINT `fk_tutorfct_grups_grup_classe` FOREIGN KEY (`grup_classe_nom`) REFERENCES `grup_classe` (`nom`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_tutorfct_grups_professor` FOREIGN KEY (`id_intern_professor`) REFERENCES `professor` (`id_intern`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `unitat`
--
ALTER TABLE `unitat`
  ADD CONSTRAINT `fk_unitat_modul` FOREIGN KEY (`modul`) REFERENCES `modul` (`nom`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `usuari`
--
ALTER TABLE `usuari`
  ADD CONSTRAINT `usuari_ibfk_1` FOREIGN KEY (`dni_persona`) REFERENCES `persona` (`dni`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
