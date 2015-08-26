/* *****************************************************************************
** Script que muestra un reporte de los ingresos realizados en el período
** especificado.
** Creado: 10/04/2014 Moisés
***************************************************************************** */

DECLARE @Desde DATE = '2014-01-01'
DECLARE @Hasta DATE = '2014-04-30'

SET @Hasta = DATEADD(d, 1, @Hasta)

SELECT
	ci.CajaIngresoID AS Folio
	, cti.NombreTipoIngreso AS Tipo
	, ci.Concepto
	, ci.Fecha
	, ur.NombreUsuario AS Solicito
	, ua.NombreUsuario AS Autorizo
	, ci.Importe
	, s.NombreSucursal AS Sucursal
FROM
	CajaIngreso ci
	LEFT JOIN CajaTipoIngreso cti ON cti.CajaTipoIngresoID = ci.CajaIngresoID AND cti.Estatus = 1
	LEFT JOIN Usuario ur ON ur.UsuarioID = ci.RealizoUsuarioID AND ur.Estatus = 1
	LEFT JOIN Autorizacion a ON a.AutorizacionProcesoID = 5 AND a.TablaRegistroID = ci.CajaIngresoID AND a.Estatus = 1
	LEFT JOIN Usuario ua ON ua.UsuarioID = a.AutorizoUsuarioID AND ua.Estatus = 1
	LEFT JOIN Sucursal s ON s.SucursalID = ci.SucursalID AND s.Estatus = 1
WHERE
	ci.Estatus = 1
	AND ci.CajaTipoIngresoID != 1
	AND (ci.Fecha >= @Desde AND ci.Fecha < @Hasta)
ORDER BY ci.Fecha