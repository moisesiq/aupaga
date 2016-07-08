/* Script con modificaciones a la base de datos de Theos. Archivo 004
 * Creado: 2015/04/24
 * Subido: 2015/04/24
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

UPDATE ContaConfigAfectacion SET Operacion = 'Compra a Crédito con Facura' WHERE ContaConfigAfectacionID = 13
INSERT INTO ContaConfigAfectacion (Operacion, ContaTipoPolizaID) VALUES
	('Compra a Crédito con Nota', 1)

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO

ALTER VIEW [dbo].[CajaEgresosView] AS
	SELECT
		ce.CajaEgresoID
		, ce.CajaTipoEgresoID
		, cte.NombreTipoEgreso AS Tipo
		, ce.Concepto
		, ce.Importe
		, ce.Fecha
		, ce.SucursalID
		, s.NombreSucursal AS Sucursal
		, ce.Facturado
		, ce.AfectadoEnProveedores
		, ce.AfectadoEnPolizas
		, u.NombreUsuario AS Usuario
		, ISNULL(a.Autorizado, 0) AS Autorizado
		, ISNULL(ua.NombreUsuario, 'Pendiente') AS AutorizoUsuario
		, cca.ContaCuentaAuxiliarID
		, cca.CuentaAuxiliar
	FROM
		CajaEgreso ce
		LEFT JOIN CajaTipoEgreso cte ON cte.CajaTipoEgresoID = ce.CajaTipoEgresoID AND cte.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = ce.SucursalID AND s.Estatus = 1
		LEFT JOIN Autorizacion a ON a.Tabla = 'CajaEgreso' AND a.TablaRegistroID = ce.CajaEgresoID AND a.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = ce.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN Usuario ua ON ua.UsuarioID = a.AutorizoUsuarioID AND ua.Estatus = 1
		LEFT JOIN ContaEgreso cne ON cne.ContaEgresoID = ce.ContaEgresoID -- AND cne.Estatus = 1
		LEFT JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = cne.ContaCuentaAuxiliarID
	WHERE ce.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

