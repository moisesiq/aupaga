/* Script con modificaciones a la base de datos de Theos. Archivo 051
 * Creado: 2015/11/25
 * Subido: 2015/11/26
 */

DECLARE @ScriptID INT = 51
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

DECLARE @OrigenDirecto INT = 1
UPDATE NotaDeCredito SET Referencia = ISNULL(u.UsuarioID, nc.Referencia)
FROM
	NotaDeCredito nc
	LEFT JOIN Usuario u ON u.NombreUsuario = nc.Referencia AND u.Estatus = 1
WHERE
	nc.OrigenID = @OrigenDirecto
	AND nc.Referencia IS NOT NULL
	AND nc.Referencia != ''
ALTER TABLE NotaDeCredito ALTER COLUMN Referencia INT NULL
EXEC sp_rename 'NotaDeCredito.Referencia', 'RelacionID', 'COLUMN'

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

ALTER VIEW [dbo].[NotasDeCreditoView] AS
	SELECT
		nc.NotaDeCreditoID
		, nc.FechaDeEmision
		, nc.Importe
		, nc.ClienteID
		, c.Nombre AS Cliente
		, nc.Valida
		, nc.FechaDeUso
		, nc.Observacion
		, u.NombrePersona AS Autorizo
		, v1.Folio AS OrigenVentaFolio
		, v2.Folio AS UsoVentaFolio
		, nc.OrigenID
		, nco.Origen
		, nc.RelacionID
	FROM
		NotaDeCredito nc
		LEFT JOIN NotaDeCreditoOrigen nco ON nco.NotaDeCreditoOrigenID = nc.OrigenID
		LEFT JOIN Autorizacion a ON a.Tabla = 'NotaDeCredito' AND a.TablaRegistroID = nc.NotaDeCreditoID AND a.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = nc.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = a.UsuarioID AND u.Estatus = 1
		LEFT JOIN Venta v1 ON v1.VentaID = nc.OrigenVentaID AND v1.Estatus = 1
		LEFT JOIN Venta v2 ON v2.VentaID = nc.UsoVentaID AND v2.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

