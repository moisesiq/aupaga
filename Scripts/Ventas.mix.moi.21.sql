/* Script con modificaciones para el módulo de ventas. Archivo 21
 * Creado: 2014/01/13
 * Subido: 2014/01/14
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	/* IF NOT EXISTS(SELECT 1 FROM VentaEstatus WHERE VentaEstatusID = 5) BEGIN
		INSERT INTO VentaEstatus (Descripcion, UsuarioID) VALUES
			('Cancelada sin pago', 1)
	END
	*/

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



/* *****************************************************************************
** Modificar procedimientos almacenados
***************************************************************************** */

-- EXEC pauComisiones 7, '2014-01-04', '2014-01-10'
ALTER PROCEDURE pauComisiones (
	@VendedorID INT
	, @Desde DATE
	, @Hasta DATE
) AS BEGIN
	SET @Hasta = DATEADD(d, 1, @Hasta)
	-- De momento no se toma en cuenta la sucursal
	DECLARE @PorComision DECIMAL(5, 2) = (SELECT (CONVERT(DECIMAL(5, 2), Valor) / 100) FROM Configuracion WHERE Nombre = 'Comisiones.Vendedor.Porcentaje')
	DECLARE @IvaMul DECIMAL(5, 2) = (SELECT (1 + (CONVERT(DECIMAL(5, 2), Valor) / 100)) FROM Configuracion WHERE Nombre = 'IVA')

	;WITH _ComVentas AS (
		SELECT
			vp.VentaID
			, MAX(vp.Fecha) AS Fecha
			, c.Nombre AS Cliente
			, v.Folio
			, SUM(vpd.Importe) AS Importe
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
					INNER JOIN Parte p ON p.ParteID = vdt.ParteID AND p.AplicaComision = 1 AND p.Estatus = 1 --r
					LEFT JOIN PartePrecio pp ON pp.ParteID = vdt.ParteID AND pp.Estatus = 1
					LEFT JOIN Cliente com ON com.ClienteID = v.ComisionistaClienteID AND com.Estatus = 1
				WHERE
					vdt.Estatus = 1
					AND vdt.VentaID = vp.VentaID
				
			) AS Comision
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
							- pp.Costo) * vddi.Cantidad) * @PorComision, 2))
					ELSE
						SUM(ROUND(((vddi.PrecioUnitario - pp.Costo) * vddi.Cantidad) * @PorComision, 2))
					END
				FROM
					VentaDevolucion vdi
					INNER JOIN VentaDevolucionDetalle vddi ON vddi.VentaDevolucionID = vdi.VentaDevolucionID AND vddi.Estatus = 1
					INNER JOIN Parte p ON p.ParteID = vddi.ParteID AND p.AplicaComision = 1 AND p.Estatus = 1 --r
					LEFT JOIN PartePrecio pp ON pp.ParteID = vddi.ParteID AND pp.Estatus = 1
					LEFT JOIN Cliente com ON com.ClienteID = v.ComisionistaClienteID AND com.Estatus = 1
				WHERE
					vdi.Estatus = 1
					AND vdi.VentaID = vp.VentaID
			) AS ComisionDev
			, v.ACredito
			, 'V' Caracteristica
			, 1 AS Orden
		FROM
			VentaPago vp
			INNER JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Importe > 0 AND vpd.Estatus = 1
			INNER JOIN Venta v ON v.VentaID = vp.VentaID AND v.RealizoUsuarioID = @VendedorID
				AND v.VentaEstatusID IN (3, 4) AND v.Estatus = 1
			LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		WHERE
			vp.Estatus = 1
			AND (vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
		GROUP BY
			vp.VentaID
			, v.Folio
			, c.Nombre
			, v.ACredito
			, v.ComisionistaClienteID
	)
	
	, _ComDevoluciones AS (
		SELECT
			vd.VentaID
			, vd.Fecha
			, c.Nombre AS Cliente
			, v.Folio
			, (SUM((vdd.PrecioUnitario + vdd.Iva) * vdd.Cantidad)) AS Importe
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
					INNER JOIN Parte p ON p.ParteID = vddt.ParteID AND p.AplicaComision = 1 AND p.Estatus = 1 --r
					LEFT JOIN PartePrecio pp ON pp.ParteID = vddt.ParteID AND pp.Estatus = 1
					LEFT JOIN Cliente com ON com.ClienteID = v.ComisionistaClienteID AND com.Estatus = 1
				WHERE
					vddt.Estatus = 1
					AND vddt.VentaDevolucionID = vd.VentaDevolucionID
			)) AS Comision
			, v.ACredito
			, 'D' AS Caracteristica
			, 2 AS Orden

			-- Para sacar el total de lo pagado
			, (
				SELECT SUM(vpd.Importe)
				FROM
					VentaPago vp
					LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID and vpd.Importe > 0 and vpd.Estatus = 1
				WHERE vp.VentaID = vd.VentaID and vp.Estatus = 1
			) AS Pagado
		FROM
			VentaDevolucion vd
			LEFT JOIN VentaDevolucionDetalle vdd ON vdd.VentaDevolucionID = vd.VentaDevolucionID AND vdd.Estatus = 1
			INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.RealizoUsuarioID = @VendedorID 
				AND (v.VentaEstatusID IN (3, 4)) AND v.Estatus = 1
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
		, (ISNULL(Comision, 0.00) + ISNULL(ComisionDev, 0.00)) AS Comision
		, ACredito
		, CASE WHEN ComisionDev IS NULL THEN Caracteristica ELSE 'VD' END AS Caracteristica
		, Orden
	FROM _ComVentas
	UNION
	SELECT
		VentaID
		, Fecha
		, Cliente
		, Folio
		, CASE WHEN ACredito = 1 THEN 0.00 ELSE (Importe * -1) END AS Importe
		, CASE WHEN ACredito = 1 THEN (Importe * -1) ELSE 0.00 END AS Cobranza
		, ISNULL((Comision * -1), 0.00) AS Comision
		, ACredito
		, Caracteristica
		, Orden
	FROM _ComDevoluciones
	
	WHERE Pagado >= Importe

	ORDER BY Orden, Fecha

END
GO