/* *****************************************************************************
** Script que muestra un detalle de las ventas a crédito
** Creado: 25/03/2014 Moisés
***************************************************************************** */

DECLARE @Desde DATE = '2013-01-01'
DECLARE @Hasta DATE = '2014-12-31'

SET @Hasta = DATEADD(d, 1, @Hasta)

SELECT
	vv.Folio
	, vv.Cliente
	, vv.Fecha
	, (vv.Fecha + c.DiasDeCredito) AS Vencimiento
	, vv.Total AS Importe
	, vv.Pagado AS Abono
	, ISNULL(vv.Total - vv.Pagado, 0) AS Saldo
FROM
	VentasView vv
	LEFT JOIN Cliente c ON c.ClienteID = vv.ClienteID AND c.Estatus = 1
WHERE
	vv.ACredito = 1
	AND (vv.Fecha >= @Desde AND vv.Fecha < @Hasta)
ORDER BY vv.Fecha