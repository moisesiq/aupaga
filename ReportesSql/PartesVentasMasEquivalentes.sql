/*
 * Script para ver el número de ventas que tiene cada parte, más las ventas de todos sus equivalentes
 * Moi 2016-06-23
 */

DECLARE @ProveedorID INT = 10  -- Apymsa

DECLARE @VentaEstatusPagada INT = 3
DECLARE @VentaEstatusCobrada INT = 2

;WITH _Partes AS (
	SELECT
		p.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, pe.GrupoID
	FROM
		Parte p
		LEFT JOIN ParteEquivalente pe ON pe.ParteID = p.ParteID
	WHERE
		p.Estatus = 1
		AND p.ProveedorID = @ProveedorID
)
SELECT
	p.ParteID
	, p.NumeroDeParte
	, p.Descripcion
	, SUM(CASE WHEN v.VentaEstatusID IN (@VentaEstatusCobrada, @VentaEstatusPagada) THEN vd.Cantidad ELSE 0.0 END) AS Ventas
	, (ISNULL(vg.Ventas, 0.0) - SUM(CASE WHEN v.VentaEstatusID IN (@VentaEstatusCobrada, @VentaEstatusPagada)
		THEN vd.Cantidad ELSE 0.0 END)) AS VentasEquivalentes
	, ISNULL(vg.Ventas, 0.0) AS Total
FROM
	_Partes p
	LEFT JOIN VentaDetalle vd ON vd.ParteID = p.ParteID AND vd.Estatus = 1
	LEFT JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
	LEFT JOIN (
		SELECT
			pe.GrupoID
			, SUM(CASE WHEN v.VentaEstatusID IN (@VentaEstatusCobrada, @VentaEstatusPagada) THEN vd.Cantidad ELSE 0.0 END)
				AS Ventas
		FROM
			ParteEquivalente pe
			LEFT JOIN VentaDetalle vd ON vd.ParteID = pe.ParteID AND vd.Estatus = 1
			LEFT JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
		WHERE pe.GrupoID IN (SELECT DISTINCT GrupoID FROM _Partes)
		GROUP BY pe.GrupoID
	) vg ON vg.GrupoID = p.GrupoID
GROUP BY
	p.ParteID
	, p.NumeroDeParte
	, p.Descripcion
	, vg.GrupoID
	, vg.Ventas
ORDER BY p.NumeroDeParte