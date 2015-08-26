/* *****************************************************************************
** Script para obtener el Total de Ventas por Cliente.
**  
***************************************************************************** */

DECLARE @Desde DATE = '2014-06-01'
DECLARE @Hasta DATE = '2014-11-11'

SET @Hasta = DATEADD(d, 1, @Hasta)

SELECT
	v.ClienteID
	, c.Nombre
	, COUNT(*) AS Ventas
FROM
	Venta v
	LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
WHERE
	v.Fecha < @Hasta AND v.Fecha > @Desde AND v.VentaEstatusID = 3
GROUP by
	v.ClienteID
	, c.Nombre
ORDER BY
	v.ClienteID
	, c.Nombre