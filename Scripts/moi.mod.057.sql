/* Script con modificaciones a la base de datos de Theos. Archivo 057
 * Creado: 2015/12/28
 * Subido: 2015/12/28
 */

DECLARE @ScriptID INT = 57
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)
-- SELECT * FROM ScriptSql ORDER BY ScriptID DESC

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE Nomina ADD EsAguinaldo BIT NULL

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

