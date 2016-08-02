/* Script con modificaciones a la base de datos de Theos. Archivo 077
 * Creado: 2016/08/02
 * Subido: 2016/08/02
 */

DECLARE @ScriptID INT = 77
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)
-- SELECT * FROM ScriptSql ORDER BY ScriptID DESC

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE VentaCambio ADD RepartidorIDAntes INT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
ALTER TABLE VentaCambio ADD RepartidorIDDespues INT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)

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

