-- 1. Creamos la base de datos (si no existe) y la seleccionamos
CREATE DATABASE IF NOT EXISTS SistemaRespaldos;
USE SistemaRespaldos;

-- 2. Tabla para la configuración de los respaldos
CREATE TABLE IF NOT EXISTS ConfiguracionRespaldos (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    NombreBaseDatos VARCHAR(100) NOT NULL,
    TipoRespaldoCompletoOParcial BOOLEAN NOT NULL DEFAULT TRUE, -- TRUE = Completo, FALSE = Parcial
    TablasAIgnorar VARCHAR(255) NULL
);

-- 3. Tabla para los horarios
CREATE TABLE IF NOT EXISTS Horarios (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    HoraEjecucion TIME NOT NULL
);

-- ==========================================
-- DATOS INICIALES (Sacados del .bat del ingeniero)
-- ==========================================

-- Base de datos que ocupa respaldo completo
INSERT INTO ConfiguracionRespaldos (NombreBaseDatos, TipoRespaldoCompletoOParcial, TablasAIgnorar) 
VALUES ('campanaoficial', TRUE, NULL);

-- Bases de datos que ocupan respaldo reducido (FALSE) y sus tablas a ignorar
INSERT INTO ConfiguracionRespaldos (NombreBaseDatos, TipoRespaldoCompletoOParcial, TablasAIgnorar) VALUES 
('bdmilady', FALSE, 'archivodata,error,nbitacoradet'),
('hsmoficial', FALSE, 'archivodata,error,nbitacoradet'),
('casonaoficial', FALSE, 'archivodata,error,nbitacoradet'),
('farmaciasanjulian', FALSE, 'archivodata,error,nbitacoradet');

-- Horario de prueba (Ejemplo: 11:00 PM)
INSERT INTO Horarios (HoraEjecucion) VALUES ('23:00:00');