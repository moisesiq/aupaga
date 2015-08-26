/* *****************************************************************************
** Script que muestra un reporte de los gastos realizados en el período
** especificado.
** Creado: 12/02/2014 Moisés
***************************************************************************** */

DECLARE @Desde DATE = '2014-01-01'
DECLARE @Hasta DATE = '2014-02-28'

SET @Hasta = DATEADD(d, 1, @Hasta)

SELECT
	ce.CajaEgresoID AS Folio
	, cte.NombreTipoEgreso AS Tipo
	, ce.Concepto
	, ce.Fecha
	, ur.NombreUsuario AS Solicito
	, ua.NombreUsuario AS Autorizo
	, ce.Importe
	, s.NombreSucursal AS Sucursal
FROM
	CajaEgreso ce
	LEFT JOIN CajaTipoEgreso cte ON cte.CajaTipoEgresoID = ce.CajaTipoEgresoID AND cte.Estatus = 1
	LEFT JOIN Usuario ur ON ur.UsuarioID = ce.RealizoUsuarioID AND ur.Estatus = 1
	LEFT JOIN Autorizacion a ON a.AutorizacionProcesoID = 5 AND a.TablaRegistroID = ce.CajaEgresoID AND a.Estatus = 1
	LEFT JOIN Usuario ua ON ua.UsuarioID = a.AutorizoUsuarioID AND ua.Estatus = 1
	LEFT JOIN Sucursal s ON s.SucursalID = ce.SucursalID AND s.Estatus = 1
WHERE
	ce.Estatus = 1
	AND ce.CajaTipoEgresoID != 1
	AND (ce.Fecha >= @Desde AND ce.Fecha < @Hasta)
ORDER BY ce.Fecha