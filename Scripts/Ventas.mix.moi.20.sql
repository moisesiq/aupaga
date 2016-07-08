/* Script con modificaciones para el módulo de ventas. Archivo 20
 * Creado: 2014/01/09
 * Subido: 2014/01/11
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	

	COMMIT TRAN
END TRY
BEGIN CATCH
	PRINT 'Hubo un error al ejecutar el script:'
	PRINT ERROR_MESSAGE()
	ROLLBACK TRAN
	RETURN
END CATCH
GO

/* *****************************************************************************
** Modificar tablas
***************************************************************************** */



/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

CREATE VIEW VentasFacturasCancelacionesView AS
	SELECT
		vfd.VentaFacturaDevolucionID
		, vfd.VentaFacturaID
		, (vf.Serie + vf.Folio) AS Factura
		, vf.Fecha AS FechaFactura
		, vf.FolioFiscal
		, c.Nombre AS Cliente
		, vfd.Fecha AS FechaSolicitudCancelacion
	FROM
		VentaFacturaDevolucion vfd
		LEFT JOIN VentaFactura vf ON vf.VentaFacturaID = vfd.VentaFacturaID AND vf.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = vf.ClienteID AND c.Estatus = 1
	WHERE
		vfd.Estatus = 1
		AND vfd.EsCancelacion = 1
		AND vfd.Procesada = 0
GO

/* *****************************************************************************
** Modificar procedimientos almacenados
***************************************************************************** */

-- DROP PROCEDURE pauComisiones
CREATE PROCEDURE pauComisiones (
	@VendedorID INT
	, @Desde DATE
	, @Hasta DATE
) AS BEGIN
	SET @Hasta = DATEADD(d, 1, @Hasta)
	-- De momento no se toma en cuenta la sucursal
	DECLARE @PorComision DECIMAL(5, 2) = (SELECT (CONVERT(DECIMAL(5, 2), Valor) / 100) FROM Configuracion WHERE Nombre = 'Comisiones.Vendedor.Porcentaje')
	DECLARE @IvaMul DECIMAL(5, 2) = (SELECT (1 + (CONVERT(DECIMAL(5, 2), Valor) / 100)) FROM Configuracion WHERE Nombre = 'IVA')

	;WITH Comisiones AS (
		SELECT
			vp.VentaID
			, MAX(vp.Fecha) AS Fecha
			, c.Nombre AS Cliente
			, v.Folio
			, CASE WHEN vd.VentaID IS NULL THEN
				SUM(vpd.Importe)
			  ELSE
				vd.Importe
			  END AS Importe
			, (
				SELECT
					CASE WHEN v.ComisionistaClienteID > 0 THEN
						SUM(ROUND(((
							CASE com.ListaDePrecios
								WHEN 1 THEN ROUND(pp.PrecioUno / @IvaMul, 2)
								WHEN 2 THEN ROUND(pp.PrecioDos / @IvaMul, 2)
								WHEN 3 THEN ROUND(pp.PrecioTres / @IvaMul, 2)
								WHEN 4 THEN ROUND(pp.PrecioCuatro / @IvaMul, 2)
								WHEN 5 THEN ROUND(pp.PrecioCinco / @IvaMul, 2)
							END
							- pp.Costo) * vdt.Cantidad) * @PorComision, 2))
					ELSE
						SUM(ROUND(((vdt.PrecioUnitario - pp.Costo) * vdt.Cantidad) * @PorComision, 2))
					END
				FROM
					VentaDetalle vdt
					LEFT JOIN PartePrecio pp ON pp.ParteID = vdt.ParteID AND pp.Estatus = 1
					LEFT JOIN Cliente com ON com.ClienteID = v.ComisionistaClienteID AND com.Estatus = 1
				WHERE
					vdt.Estatus = 1
					AND vdt.VentaID = vp.VentaID
			) AS Comision
			, v.ACredito
			, CASE WHEN vd.VentaID IS NULL THEN 'V' ELSE 'VD' END AS Caracteristica
			, 1 AS Orden
		FROM
			VentaPago vp
			LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
			INNER JOIN Venta v ON v.VentaID = vp.VentaID AND v.RealizoUsuarioID = @VendedorID AND v.Estatus = 1
			LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
			LEFT JOIN (
				SELECT
					vd.VentaID
					, SUM((vdd.PrecioUnitario + vdd.Iva) * vdd.Cantidad) AS Importe
				FROM
					VentaDevolucion vd
					LEFT JOIN VentaDevolucionDetalle vdd ON vdd.VentaDevolucionID = vd.VentaDevolucionID AND vdd.Estatus = 1
				WHERE vd.Estatus = 1
				GROUP BY vd.VentaID
			) vd ON vd.VentaID = vp.VentaID
		WHERE
			vp.Estatus = 1
			AND (vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
		GROUP BY
			vp.VentaID
			, v.Folio
			, c.Nombre
			, v.ACredito
			, vd.VentaID
			, vd.Importe
			, v.ComisionistaClienteID

		UNION

		SELECT
			vd.VentaID
			, vd.Fecha
			, c.Nombre AS Cliente
			, v.Folio
			, (SUM((vdd.PrecioUnitario + vdd.Iva) * vdd.Cantidad) * -1) AS Importe
			, ((
				SELECT
					CASE WHEN v.ComisionistaClienteID > 0 THEN
						SUM(ROUND(((
							CASE com.ListaDePrecios
								WHEN 1 THEN ROUND(pp.PrecioUno / @IvaMul, 2)
								WHEN 2 THEN ROUND(pp.PrecioDos / @IvaMul, 2)
								WHEN 3 THEN ROUND(pp.PrecioTres / @IvaMul, 2)
								WHEN 4 THEN ROUND(pp.PrecioCuatro / @IvaMul, 2)
								WHEN 5 THEN ROUND(pp.PrecioCinco / @IvaMul, 2)
							END
							- pp.Costo) * vddt.Cantidad) * @PorComision, 2))
					ELSE
						SUM(ROUND(((vddt.PrecioUnitario - pp.Costo) * vddt.Cantidad) * @PorComision, 2))
					END
				FROM
					VentaDevolucionDetalle vddt
					LEFT JOIN PartePrecio pp ON pp.ParteID = vddt.ParteID AND pp.Estatus = 1
					LEFT JOIN Cliente com ON com.ClienteID = v.ComisionistaClienteID AND com.Estatus = 1
				WHERE
					vddt.Estatus = 1
					AND vddt.VentaDevolucionID = vd.VentaDevolucionID
			) * -1) AS Comision
			, v.ACredito
			, 'D' AS Caracteristica
			, 2 AS Orden
		FROM
			VentaDevolucion vd
			LEFT JOIN VentaDevolucionDetalle vdd ON vdd.VentaDevolucionID = vd.VentaDevolucionID AND vdd.Estatus = 1
			INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.RealizoUsuarioID = @VendedorID AND v.Estatus = 1
			LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		WHERE
			vd.Estatus = 1
			AND (vd.Fecha >= @Desde AND vd.Fecha < @Hasta)
		GROUP BY
			vd.VentaDevolucionID
			, vd.VentaID
			, vd.Fecha
			, c.Nombre
			, v.Folio
			, v.ACredito
			, v.ComisionistaClienteID
	)
	SELECT
		VentaID
		, Fecha
		, Cliente
		, Folio
		, CASE WHEN ACredito = 1 THEN 0.00 ELSE Importe END AS Importe
		, CASE WHEN ACredito = 1 THEN Importe ELSE 0.00 END AS Cobranza
		, ISNULL(Comision, 0.00) AS Comision
		, ACredito
		, Caracteristica
	FROM Comisiones
	ORDER BY Orden, Fecha

END
GO