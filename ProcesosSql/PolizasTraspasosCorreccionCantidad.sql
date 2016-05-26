/*****************************************************************************
 * Script para corregir las pólizas al hacer y recibir un traspaso, cuando
 * la cantidad es mayor a 1, porque no se estaba considerando la cantidad
 * total.
 * Moi - 2015/09/10
 *****************************************************************************/

BEGIN TRAN

UPDATE ContaPolizaDetalle SET
	Cargo = CASE WHEN Cargo > 0 THEN imp.Importe ELSE 0.0 END
	, Abono = CASE WHEN Abono > 0 THEN imp.Importe ELSE 0.0 END
FROM
	ContaPolizaDetalle cpd
	INNER JOIN (
		SELECT
			cp.ContaPolizaID
			, cp.RelacionID
			, mit.MovimientoInventarioID
			, mit.Importe
		FROM
			ContaPoliza cp
			-- INNER JOIN MovimientoInventarioDetalle mid ON mid.MovimientoInventarioID = cp.RelacionID AND mid.Estatus = 1	
			INNER JOIN (
				SELECT
					mid.MovimientoInventarioID
					, SUM(mid.Cantidad * pp.Costo) AS Importe
				FROM
					MovimientoInventarioDetalle mid
					LEFT JOIN PartePrecio pp ON pp.ParteID = mid.ParteID AND pp.Estatus = 1
				WHERE
					mid.MovimientoInventarioID IN 
						(SELECT DISTINCT MovimientoInventarioID FROM MovimientoInventarioDetalle WHERE Cantidad > 1 AND Estatus = 1)
				GROUP BY mid.MovimientoInventarioID
			) mit ON mit.MovimientoInventarioID = cp.RelacionID
		WHERE cp.Concepto LIKE 'TRASPASO ORIGEN %'
	) imp ON imp.ContaPolizaID = cpd.ContaPolizaID

-- select * from ContaPolizaDetalle where ContaPolizaID = 139007
-- select * from ContaPolizaDetalle where ContaPolizaID = 139001

ROLLBACK TRAN