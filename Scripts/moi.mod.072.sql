/* Script con modificaciones a la base de datos de Theos. Archivo 072
 * Creado: 2016/07/12
 * Subido: 2016/07/14
 */

DECLARE @ScriptID INT = 72
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)
-- SELECT * FROM ScriptSql ORDER BY ScriptID DESC

/* *****************************************************************************
** Creaci�n y modificaci�n de tablas
***************************************************************************** */

INSERT INTO TipoFormaPago (NombreTipoFormaPago, UsuarioID) VALUES ('TARJETA DE D�BITO', 1)
INSERT INTO VentaGarantiaAccion (Etapa, Accion) VALUES (0, 'TARJETA DE D�BITO')

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO



/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

