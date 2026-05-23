-- ========================================================
-- SPRINT MONGODB - DÍA 10
-- Tarea de Alex: Actualización del esquema de Base de Datos
-- ========================================================

USE SistemaRespaldos;

-- NOTA: Pineda mencionó la tabla 'configuracionrespaldos', 
-- pero el código en la capa DAL (ConsultasDAL.cs) busca la tabla 'BasesDatos'.
-- Por seguridad, este script actualiza 'BasesDatos'. Si en tu servidor 
-- la tabla se llama distinto, cambia el nombre a continuación.

ALTER TABLE BasesDatos 
ADD COLUMN TipoMotor VARCHAR(20) NOT NULL DEFAULT 'MySQL';

ALTER TABLE BasesDatos 
ADD COLUMN CadenaConexion VARCHAR(255) NULL;

-- Si por alguna razón la tabla sí se llama ConfiguracionRespaldos como dijo Pineda:
-- ALTER TABLE ConfiguracionRespaldos ADD COLUMN TipoMotor VARCHAR(20) NOT NULL DEFAULT 'MySQL';
-- ALTER TABLE ConfiguracionRespaldos ADD COLUMN CadenaConexion VARCHAR(255) NULL;
