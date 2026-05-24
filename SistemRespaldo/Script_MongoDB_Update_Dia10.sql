

USE SistemaRespaldos;



ALTER TABLE BasesDatos 
ADD COLUMN TipoMotor VARCHAR(20) NOT NULL DEFAULT 'MySQL';

ALTER TABLE BasesDatos 
ADD COLUMN CadenaConexion VARCHAR(255) NULL;

-- 
-- ALTER TABLE ConfiguracionRespaldos ADD COLUMN TipoMotor VARCHAR(20) NOT NULL DEFAULT 'MySQL';
-- ALTER TABLE ConfiguracionRespaldos ADD COLUMN CadenaConexion VARCHAR(255) NULL;
