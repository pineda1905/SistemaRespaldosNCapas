-- ========================================================
-- SCRIPT DÍA 12: Agregar columna TipoMotor a HistorialLogs
-- Ejecutar en MySQL sobre la base SistemaRespaldos
-- ========================================================

USE SistemaRespaldos;

-- Agregamos la columna TipoMotor para saber qué motor generó el respaldo
-- DEFAULT 'MySQL' para que los registros existentes queden correctos
ALTER TABLE HistorialLogs 
ADD COLUMN TipoMotor VARCHAR(20) NOT NULL DEFAULT 'MySQL';
