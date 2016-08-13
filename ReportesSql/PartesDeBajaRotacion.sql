/*
 * Reporte de artículos de baja rotación
 * Moi 2016-08-12
 */

DECLARE @Linea NVARCHAR(64) = 'acumulador'
DECLARE @Marca NVARCHAR(64) = 'lth'
DECLARE @Ventas INT = 12

DECLARE @VentaEstPagada INT = 3

DECLARE @AnioActual INT = YEAR(GETDATE())
DECLARE @LineaID INT = (SELECT LineaID FROM Linea WHERE NombreLinea = @Linea)
DECLARE @MarcaID INT = (SELECT MarcaParteID FROM MarcaParte WHERE NombreMarcaParte = @Marca)
IF @LineaID = 0 OR @MarcaID = 0 BEGIN
	GOTO Error
END

;WITH _Partes AS (
	SELECT
		p.ParteID
		, SUM(CASE WHEN YEAR(v.Fecha) = (@AnioActual - 4) THEN vd.Cantidad ELSE 0.0 END) AS Cantidad5
		, SUM(CASE WHEN YEAR(v.Fecha) = (@AnioActual - 3) THEN vd.Cantidad ELSE 0.0 END) AS Cantidad4
		, SUM(CASE WHEN YEAR(v.Fecha) = (@AnioActual - 2) THEN vd.Cantidad ELSE 0.0 END) AS Cantidad3
		, SUM(CASE WHEN YEAR(v.Fecha) = (@AnioActual - 1) THEN vd.Cantidad ELSE 0.0 END) AS Cantidad2
		, SUM(CASE WHEN YEAR(v.Fecha) = (@AnioActual) THEN vd.Cantidad ELSE 0.0 END) AS Cantidad1
		, [1] AS Fecha1
		, [2] AS Fecha2
		, [3] AS Fecha3
		, [4] AS Fecha4
		, [5] AS Fecha5
		, [6] AS Fecha6
		, [7] AS Fecha7
		, [8] AS Fecha8
		, [9] AS Fecha9
		, [10] AS Fecha10
		, [11] AS Fecha11
		, [12] AS Fecha12
		, DATEDIFF(DAY, [2], [1]) AS Dias1
		, DATEDIFF(DAY, [3], [2]) AS Dias2
		, DATEDIFF(DAY, [4], [3]) AS Dias3
		, DATEDIFF(DAY, [5], [4]) AS Dias4
		, DATEDIFF(DAY, [6], [5]) AS Dias5
		, DATEDIFF(DAY, [7], [6]) AS Dias6
		, DATEDIFF(DAY, [8], [7]) AS Dias7
		, DATEDIFF(DAY, [9], [8]) AS Dias8
		, DATEDIFF(DAY, [10], [9]) AS Dias9
		, DATEDIFF(DAY, [11], [10]) AS Dias10
		, DATEDIFF(DAY, [12], [11]) AS Dias11
		, ((ISNULL(DATEDIFF(DAY, [2], [1]), 0) + ISNULL(DATEDIFF(DAY, [3], [2]), 0) + ISNULL(DATEDIFF(DAY, [4], [3]), 0)
			+ ISNULL(DATEDIFF(DAY, [5], [4]), 0) + ISNULL(DATEDIFF(DAY, [6], [5]), 0) + ISNULL(DATEDIFF(DAY, [7], [6]), 0)
			+ ISNULL(DATEDIFF(DAY, [8], [7]), 0) + ISNULL(DATEDIFF(DAY, [9], [8]), 0) + ISNULL(DATEDIFF(DAY, [10], [9]), 0)
			+ ISNULL(DATEDIFF(DAY, [11], [10]), 0) + ISNULL(DATEDIFF(DAY, [12], [11]), 0))
			/ 12) AS Promedio
	FROM
		Venta v
		INNER JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		INNER JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
		LEFT JOIN (
			SELECT
				ParteID
				, [1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12]
			FROM (
				SELECT
					vd.ParteID
					, v.Fecha
					, ROW_NUMBER() OVER (PARTITION BY vd.ParteID ORDER BY v.Fecha DESC) AS Venta
				FROM
					VentaDetalle vd
					INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
				WHERE
					vd.Estatus = 1
					AND v.VentaEstatusID = @VentaEstPagada
			) c
			PIVOT (
				MAX(Fecha)
				FOR Venta IN ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])
			) AS P
			-- WHERE c.Venta <= 12
			
		) vf ON vf.ParteID = p.ParteID
	WHERE
		v.Estatus = 1
		AND v.VentaEstatusID = @VentaEstPagada
		AND p.MarcaParteID = @MarcaID
		AND p.LineaID = @LineaID
	GROUP BY
		p.ParteID
		, [1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12]
	HAVING SUM(vd.Cantidad) <= @Ventas
)
SELECT
	*
FROM _Partes


Error:
	PRINT 'Línea o Marca inválidos. Favor de verificar.'
	GOTO Fin
Fin:
	PRINT 'Fin'