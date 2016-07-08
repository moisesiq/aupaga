/* Script con modificaciones a la base de datos de Theos. Archivo 049
 * Creado: 2015/11/16
 * Subido: 2015/11/17
 */

DECLARE @ScriptID INT = 49
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
	('Reportes.Pedidos.Pedido', 'D', 'I', 'Salida donde se muestra el reporte de Pedidos (D - Diseño, P - Pantalla, I - Impresora).');

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


