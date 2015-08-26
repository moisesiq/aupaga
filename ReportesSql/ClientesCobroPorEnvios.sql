/* *****************************************************************************
** Reporte de los cargos hechos a los Clientes por envío a domicilio
** Creado: 16/12/2014 Moisés
***************************************************************************** */

-- Parámetros del reporte
DECLARE @Desde DATE = '2014-12-01'
DECLARE @Hasta DATE = '2014-12-31'

-- Variables de uso local
DECLARE @EstPagada INT = 3
--
SET @Hasta = DATEADD(DAY, 1, @Hasta)

SELECT
	c.ClienteID
	, c.Nombre AS Cliente
	, s.NombreSucursal AS Sucursal
	, SUM(CASE WHEN (vd.PrecioUnitario + vd.Iva) >= c.ImporteParaCobroPorEnvio THEN vd.Cantidad ELSE 0.0 END) AS CobrosPorEnvio
	, (SUM(CASE WHEN (vd.PrecioUnitario + vd.Iva) >= c.ImporteParaCobroPorEnvio THEN vd.Cantidad ELSE 0.0 END)
		* c.ImporteCobroPorEnvio) AS ImportePorEnvio
FROM
	Cliente c
	LEFT JOIN Venta v ON v.ClienteID = c.ClienteID AND v.Estatus = 1
		AND v.VentaEstatusID = @EstPagada AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
	LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
	LEFT JOIN Sucursal s ON s.SucursalID = v.SucursalID AND s.Estatus = 1
WHERE
	c.Estatus = 1
	AND c.CobroPorEnvio = 1
GROUP BY
	c.ClienteID
	, c.Nombre
	, c.ImporteCobroPorEnvio
	, s.NombreSucursal
