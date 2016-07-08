/* Caja **********************************************************************/
SELECT * FROM CajaEfectivoPorDia ORDER BY FechaRegistro DESC
/* Caja **********************************************************************/
/* Pagos mal aplicados *******************************************************/
SELECT * FROM VentasView WHERE Pagado != Total AND ACredito = 0 AND VentaEstatusID NOT IN (1, 4)
/* Pagos mal aplicados *******************************************************/

/* Consulta ******************************************************************/
SELECT * FROM VentasView WHERE Folio = 'MAT1745'
DECLARE @VentaID INT = 246063
SELECT * FROM Venta WHERE VentaID = @VentaID
SELECT * FROM VentaDetalle WHERE VentaID = @VentaID
SELECT * FROM VentaPago WHERE VentaID = @VentaID
SELECT * FROM VentaPagoDetalle WHERE VentaPagoID IN (SELECT VentaPagoID FROM VentaPago WHERE VentaID = @VentaID)
SELECT * FROM VentaDevolucion WHERE VentaID = @VentaID
SELECT * FROM VentaDevolucionDetalle WHERE VentaDevolucionID IN
	(SELECT VentaDevolucionID FROM VentaDevolucion WHERE VentaID = @VentaID)
SELECT * FROM VentaGarantia WHERE VentaID = @VentaID
GO
/* Consulta ******************************************************************/

/* Cancelación ***************************************************************/
/*
BEGIN TRAN
DECLARE @VentaID INT = 151028
DECLARE @SucursalID INT = 1
DECLARE @MotivoID INT = 4
DECLARE @Observacion VARCHAR(64) = 'Cancelación manual de factura múltiple (MAT1745).'
DECLARE @FormaDePagoID INT = 6
DECLARE @Ahora DATETIME = GETDATE()
DECLARE @Importe DECIMAL(12, 2) = NULL

INSERT INTO VentaDevolucion
	(VentaID, Fecha, SucursalID, MotivoID, Observacion, RealizoUsuarioID, UsuarioID, TipoFormaPagoID, EsCancelacion)
VALUES
	(@VentaID, @Ahora, @SucursalID, @MotivoID, @Observacion, 1, 1, @FormaDePagoID, 1)
INSERT INTO VentaDevolucionDetalle (VentaDevolucionID, ParteID, Cantidad, PrecioUnitario, Iva, UsuarioID)
	SELECT @@IDENTITY, ParteID, Cantidad, PrecioUnitario, Iva, 1 FROM VentaDetalle WHERE VentaID = @VentaID AND Estatus = 1

UPDATE ParteExistencia SET Existencia = (Existencia + vd.Cantidad) FROM
	ParteExistencia pe
	INNER JOIN VentaDetalle vd ON vd.VentaID = @VentaID AND vd.Estatus = 1
		AND vd.ParteID = pe.ParteID AND pe.SucursalID = @SucursalID

UPDATE VentaDetalle SET Estatus = 0 WHERE VentaID = @VentaID AND Estatus = 1
UPDATE Venta SET VentaEstatusID = (CASE WHEN @Importe IS NULL THEN 5 ELSE 4 END) WHERE VentaID = @VentaID

-- Para el pago
IF @Importe IS NOT NULL BEGIN
	INSERT INTO VentaPago (VentaID, Fecha, UsuarioID, SucursalID) VALUES
		(@VentaID, @Ahora, 1, @SucursalID)
	INSERT INTO VentaPagoDetalle (VentaPagoID, TipoFormaPagoID, Importe, UsuarioID) VALUES
		((SELECT @@IDENTITY), @FormaDePagoID, @Importe, 1)
END

SELECT * FROM Venta WHERE VentaID = @VentaID
SELECT * FROM VentaDetalle WHERE VentaID = @VentaID
SELECT * FROM VentaPago WHERE VentaID = @VentaID
SELECT * FROM VentaPagoDetalle WHERE VentaPagoID IN (SELECT VentaPagoID FROM VentaPago WHERE VentaID = @VentaID)
SELECT * FROM VentaDevolucion WHERE VentaID = @VentaID
SELECT * FROM VentaDevolucionDetalle WHERE VentaDevolucionID IN
	(SELECT VentaDevolucionID FROM VentaDevolucion WHERE VentaID = @VentaID)
SELECT * FROM ParteExistencia WHERE SucursalID = @SucursalID AND ParteID IN 
	(SELECT ParteID FROM VentaDetalle WHERE VentaID = @VentaID)
ROLLBACK TRAN

-- Para la factura
DECLARE @Serie VARCHAR(4) = 'MAT'
DECLARE @Folio VARCHAR(8) = '1745'
DECLARE @SucursalID INT = 1
DECLARE @Ahora DATETIME = GETDATE()

DECLARE @VentaFacturaID INT = (SELECT VentaFacturaID FROM VentaFactura WHERE Serie = @Serie AND Folio = @Folio)
INSERT INTO VentaFacturaDevolucion (VentaFacturaID, Fecha, SucursalID, EsCancelacion, UsuarioID, Procesada) VALUES
	(@VentaFacturaID, @Ahora, @SucursalID, 1, 1, 0)
UPDATE VentaFactura SET EstatusGenericoID = 4 WHERE VentaFacturaID = @VentaFacturaID

-- Para cancelar una nota de crédito
UPDATE NotaDeCredito SET Valida = 0, Observacion = (Observacion + ' > Cancelación manual') WHERE NotaDeCreditoID = 26
*/
/* Cancelación ***************************************************************/

/* Caja **********************************************************************/
SELECT
	c.SucursalID
	, c.Dia
	, SUM(c.Inicio) AS Inicio
	, SUM(p.Importe) AS Pagos
	, SUM(ci.Importe) AS Ingresos
	, SUM(ce.Importe) AS Egresos
	, SUM(c.Cierre) AS Cierre
	, SUM(c.Inicio + ISNULL(p.Importe, 0) + ISNULL(ci.Importe, 0) - ISNULL(ce.Importe, 0)) AS Total
	, SUM(c.Cierre - (c.Inicio + ISNULL(p.Importe, 0) + ISNULL(ci.Importe, 0) - ISNULL(ce.Importe, 0))) AS Diferencia
FROM
	CajaEfectivoPorDia c
	LEFT JOIN (
		SELECT
			vp.SucursalID
			, CONVERT(DATE, vp.Fecha) AS Fecha
			, SUM(vpd.Importe) AS Importe
		FROM
			VentaPagoDetalle vpd
			LEFT JOIN VentaPago vp ON vp.VentaPagoID = vpd.VentaPagoID AND vp.Estatus = 1
		WHERE
			vpd.Estatus = 1
			AND vpd.TipoFormaPagoID != 6
		GROUP BY
			vp.SucursalID
			, CONVERT(DATE, vp.Fecha)
	) p ON p.SucursalID = c.SucursalID AND p.Fecha = c.Dia
	LEFT JOIN (
		SELECT
			SucursalID
			, CONVERT(DATE, Fecha) AS Fecha
			, SUM(Importe) AS Importe
		FROM CajaIngreso
		WHERE Estatus = 1
		GROUP BY
			SucursalID
			, CONVERT(DATE, Fecha)
	) ci ON ci.SucursalID = c.SucursalID AND ci.Fecha = c.Dia
	LEFT JOIN (
		SELECT
			SucursalID
			, CONVERT(DATE, Fecha) AS Fecha
			, SUM(Importe) AS Importe
		FROM CajaEgreso
		WHERE Estatus = 1
		GROUP BY
			SucursalID
			, CONVERT(DATE, Fecha)
	) ce ON ce.SucursalID = c.SucursalID AND ce.Fecha = c.Dia
WHERE c.Estatus = 1
GROUP BY c.SucursalID, c.Dia
ORDER BY c.Dia desc, c.SucursalID
/* Caja **********************************************************************/