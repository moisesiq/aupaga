/* Script con modificaciones para el módulo de ventas. Archivo 51
 * Creado: 2014/07/28
 * Subido: 2014/08/13
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	ALTER TABLE ContaCuentaAuxiliar ADD DivisorDia DECIMAL(12, 6) NULL
	ALTER TABLE ContaCuentaAuxiliar DROP COLUMN DiasMovimiento

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
** Modificar tablas
***************************************************************************** */



/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

ALTER PROCEDURE pauComisiones (
	@ModoID INT
	, @VendedorID INT
	, @Desde DATE = NULL
	, @Hasta DATE = NULL
) AS BEGIN
	SET NOCOUNT ON

	/* EXEC pauComisiones 1, 1, '2014-07-12', '2014-07-18'
	DECLARE @ModoID INT = 1
	DECLARE @VendedorID INT = 1
	DECLARE @Desde DATE = '2014-03-20'
	DECLARE @Hasta DATE = '2014-03-20'
	*/

	-- Definición de variables tipo constante
	DECLARE @ModoNormalID INT = 1
	DECLARE @ModoNoPagadasID INT = 2
	
	DECLARE @EstCobradaID INT = 2
	DECLARE @EstPagadaID INT = 3
	DECLARE @EstCanceladaID INT = 4
	DECLARE @EstGenCompletado INT = 3

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)
	-- De momento no se toma en cuenta la sucursal
	DECLARE @PorComision DECIMAL(5, 2) = (SELECT (CONVERT(DECIMAL(5, 2), Valor) / 100) FROM Configuracion WHERE Nombre = 'Comisiones.Vendedor.Porcentaje')
	DECLARE @PorComision9500 DECIMAL(5, 2) = (SELECT (CONVERT(DECIMAL(5, 2), Valor) / 100) FROM Configuracion WHERE Nombre = 'Comisiones.Vendedor.Porcentaje9500')
	DECLARE @IvaMul DECIMAL(5, 2) = (SELECT (1 + (CONVERT(DECIMAL(5, 2), Valor) / 100)) FROM Configuracion WHERE Nombre = 'IVA')

	-- Modo normal
	IF @ModoID = @ModoNormalID BEGIN
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
							CASE WHEN c9.VentaID IS NULL THEN
								SUM(((
									CASE com.ListaDePrecios
										WHEN 1 THEN (pp.PrecioUno / @IvaMul)
										WHEN 2 THEN (pp.PrecioDos / @IvaMul)
										WHEN 3 THEN (pp.PrecioTres / @IvaMul)
										WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
										WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
									END
									- vdt.Costo) * vdt.Cantidad) * @PorComision)
							ELSE
								SUM(((
									CASE com.ListaDePrecios
										WHEN 1 THEN (pp.PrecioUno / @IvaMul)
										WHEN 2 THEN (pp.PrecioDos / @IvaMul)
										WHEN 3 THEN (pp.PrecioTres / @IvaMul)
										WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
										WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
									END
									- vdt.Costo) * vdt.Cantidad) * @PorComision9500)
							END
						ELSE
							CASE WHEN c9.VentaID IS NULL THEN
								SUM(((
									(CASE WHEN vdt.PrecioUnitario > (pp.PrecioUno / @IvaMul) THEN (pp.PrecioUno / @IvaMul) 
										ELSE vdt.PrecioUnitario END)
									- vdt.Costo) * vdt.Cantidad) * @PorComision)
							ELSE
								SUM(((
									(CASE WHEN vdt.PrecioUnitario > (pp.PrecioUno / @IvaMul) THEN (pp.PrecioUno / @IvaMul) 
										ELSE vdt.PrecioUnitario END)
									- vdt.Costo) * vdt.Cantidad) * @PorComision9500)
							END
						END
					FROM
						VentaDetalle vdt
						INNER JOIN Parte p ON p.ParteID = vdt.ParteID AND p.Estatus = 1
						LEFT JOIN PartePrecio pp ON pp.ParteID = vdt.ParteID AND pp.Estatus = 1
						LEFT JOIN Cliente com ON com.ClienteID = v.ComisionistaClienteID AND com.Estatus = 1
					WHERE
						vdt.Estatus = 1
						AND vdt.VentaID = vp.VentaID
						AND p.AplicaComision = 1
				) AS Comision
				, (
					SELECT
						CASE WHEN v.ComisionistaClienteID > 0 THEN
							CASE WHEN c9.VentaID IS NULL THEN
								SUM(((
									CASE com.ListaDePrecios
										WHEN 1 THEN (pp.PrecioUno / @IvaMul)
										WHEN 2 THEN (pp.PrecioDos / @IvaMul)
										WHEN 3 THEN (pp.PrecioTres / @IvaMul)
										WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
										WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
									END
									- vddi.Costo) * vddi.Cantidad) * @PorComision)
							ELSE
								SUM(((
									CASE com.ListaDePrecios
										WHEN 1 THEN (pp.PrecioUno / @IvaMul)
										WHEN 2 THEN (pp.PrecioDos / @IvaMul)
										WHEN 3 THEN (pp.PrecioTres / @IvaMul)
										WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
										WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
									END
									- vddi.Costo) * vddi.Cantidad) * @PorComision9500)
							END
						ELSE
							CASE WHEN c9.VentaID IS NULL THEN
								SUM(((
									(CASE WHEN vddi.PrecioUnitario > (pp.PrecioUno / @IvaMul) THEN (pp.PrecioUno / @IvaMul) 
										ELSE vddi.PrecioUnitario END)
									- vddi.Costo) * vddi.Cantidad) * @PorComision)
							ELSE
								SUM(((
									(CASE WHEN vddi.PrecioUnitario > (pp.PrecioUno / @IvaMul) THEN (pp.PrecioUno / @IvaMul) 
										ELSE vddi.PrecioUnitario END)
									- vddi.Costo) * vddi.Cantidad) * @PorComision9500)
							END
						END
					FROM
						VentaDevolucion vdi
						INNER JOIN VentaDevolucionDetalle vddi ON vddi.VentaDevolucionID = vdi.VentaDevolucionID
							AND vddi.Estatus = 1
						INNER JOIN Parte p ON p.ParteID = vddi.ParteID AND p.Estatus = 1
						LEFT JOIN PartePrecio pp ON pp.ParteID = vddi.ParteID AND pp.Estatus = 1
						LEFT JOIN Cliente com ON com.ClienteID = v.ComisionistaClienteID AND com.Estatus = 1
					WHERE
						vdi.Estatus = 1
						AND vdi.VentaID = vp.VentaID
						AND p.AplicaComision = 1
				) AS ComisionDev
				, v.ACredito
				, ('V' + CASE WHEN c9.VentaID IS NULL THEN '' ELSE '9500' END) AS Caracteristica
				, 1 AS Orden
			FROM
				VentaPago vp
				INNER JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
				INNER JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
				LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.EstatusGenericoID = @EstGenCompletado
					AND c9.Estatus = 1
				LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
			WHERE
				vp.Estatus = 1
				AND (vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
				AND vpd.Importe > 0
				AND v.RealizoUsuarioID = @VendedorID
				AND v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)
				-- Se quitan las ventas de los domingos
				AND DATEPART(DW, v.Fecha) != 1
			GROUP BY
				vp.VentaID
				, v.Folio
				, c.Nombre
				, v.ACredito
				, v.ComisionistaClienteID
				, c9.VentaID
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
							CASE WHEN c9.VentaID IS NULL THEN
								SUM(((
									CASE com.ListaDePrecios
										WHEN 1 THEN (pp.PrecioUno / @IvaMul)
										WHEN 2 THEN (pp.PrecioDos / @IvaMul)
										WHEN 3 THEN (pp.PrecioTres / @IvaMul)
										WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
										WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
									END
									- vddt.Costo) * vddt.Cantidad) * @PorComision)
							ELSE
								SUM(((
									CASE com.ListaDePrecios
										WHEN 1 THEN (pp.PrecioUno / @IvaMul)
										WHEN 2 THEN (pp.PrecioDos / @IvaMul)
										WHEN 3 THEN (pp.PrecioTres / @IvaMul)
										WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
										WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
									END
									- vddt.Costo) * vddt.Cantidad) * @PorComision9500)
							END
						ELSE
							CASE WHEN c9.VentaID IS NULL THEN
								SUM(((
									(CASE WHEN vddt.PrecioUnitario > (pp.PrecioUno / @IvaMul) THEN (pp.PrecioUno / @IvaMul) 
										ELSE vddt.PrecioUnitario END)
									- vddt.Costo) * vddt.Cantidad) * @PorComision)
							ELSE
								SUM(((
									(CASE WHEN vddt.PrecioUnitario > (pp.PrecioUno / @IvaMul) THEN (pp.PrecioUno / @IvaMul) 
										ELSE vddt.PrecioUnitario END)
									- vddt.Costo) * vddt.Cantidad) * @PorComision9500)
							END
						END
					FROM
						VentaDevolucionDetalle vddt
						INNER JOIN Parte p ON p.ParteID = vddt.ParteID AND p.Estatus = 1
						LEFT JOIN PartePrecio pp ON pp.ParteID = vddt.ParteID AND pp.Estatus = 1
						LEFT JOIN Cliente com ON com.ClienteID = v.ComisionistaClienteID AND com.Estatus = 1
					WHERE
						vddt.Estatus = 1
						AND vddt.VentaDevolucionID = vd.VentaDevolucionID
						AND p.AplicaComision = 1
				)) AS Comision
				, v.ACredito
				, ('D' + CASE WHEN c9.VentaID IS NULL THEN '' ELSE '9500' END) AS Caracteristica
				, 2 AS Orden

				-- Para sacar el total de lo pagado
				, (
					SELECT SUM(vpd.Importe)
					FROM
						VentaPago vp
						LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID
							AND vpd.Importe > 0 AND vpd.Estatus = 1
					WHERE vp.VentaID = vd.VentaID and vp.Estatus = 1
				) AS Pagado
			FROM
				VentaDevolucion vd
				LEFT JOIN VentaDevolucionDetalle vdd ON vdd.VentaDevolucionID = vd.VentaDevolucionID AND vdd.Estatus = 1
				INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
				LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.EstatusGenericoID = @EstGenCompletado
					AND c9.Estatus = 1
				LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
			WHERE
				vd.Estatus = 1
				AND (
					(vd.Fecha >= @Desde AND vd.Fecha < @Hasta)
					OR vd.VentaID IN (SELECT DISTINCT VentaID FROM _ComVentas WHERE Fecha < @Desde)
				)
				AND v.RealizoUsuarioID = @VendedorID
				AND v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)
				-- Se quitan las ventas de los domingos
				AND DATEPART(DW, v.Fecha) != 1
			GROUP BY
				vd.VentaDevolucionID
				, vd.VentaID
				, vd.Fecha
				, c.Nombre
				, v.Folio
				, v.ACredito
				, v.ComisionistaClienteID
				, c9.VentaID
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

	-- Modo ventas no pagadas (serían las de crédito)
	IF @ModoID = @ModoNoPagadasID BEGIN
		SELECT
			v.VentaID
			, v.Fecha
			, c.Nombre AS Cliente
			, v.Folio
			-- , SUM((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) AS Importe
			, vdc.Importe
			, (
				SELECT
					CASE WHEN v.ComisionistaClienteID > 0 THEN
							CASE WHEN c9.VentaID IS NULL THEN
								SUM(((
									CASE com.ListaDePrecios
										WHEN 1 THEN (pp.PrecioUno / @IvaMul)
										WHEN 2 THEN (pp.PrecioDos / @IvaMul)
										WHEN 3 THEN (pp.PrecioTres / @IvaMul)
										WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
										WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
									END
									- vdt.Costo) * vdt.Cantidad) * @PorComision)
							ELSE
								SUM(((
									CASE com.ListaDePrecios
										WHEN 1 THEN (pp.PrecioUno / @IvaMul)
										WHEN 2 THEN (pp.PrecioDos / @IvaMul)
										WHEN 3 THEN (pp.PrecioTres / @IvaMul)
										WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
										WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
									END
									- vdt.Costo) * vdt.Cantidad) * @PorComision9500)
							END
						ELSE
							CASE WHEN c9.VentaID IS NULL THEN
								SUM(((
									(CASE WHEN vdt.PrecioUnitario > (pp.PrecioUno / @IvaMul) THEN (pp.PrecioUno / @IvaMul) 
										ELSE vdt.PrecioUnitario END)
									- vdt.Costo) * vdt.Cantidad) * @PorComision)
							ELSE
								SUM(((
									(CASE WHEN vdt.PrecioUnitario > (pp.PrecioUno / @IvaMul) THEN (pp.PrecioUno / @IvaMul) 
										ELSE vdt.PrecioUnitario END)
									- vdt.Costo) * vdt.Cantidad) * @PorComision9500)
							END
						END
				FROM
					VentaDetalle vdt
					INNER JOIN Parte p ON p.ParteID = vdt.ParteID AND p.Estatus = 1
					LEFT JOIN PartePrecio pp ON pp.ParteID = vdt.ParteID AND pp.Estatus = 1
					LEFT JOIN Cliente com ON com.ClienteID = v.ComisionistaClienteID AND com.Estatus = 1
				WHERE
					vdt.Estatus = 1
					AND vdt.VentaID = v.VentaID
					AND p.AplicaComision = 1
			) AS Comision
			, 0.00 AS Cobranza
			, v.ACredito
			, '' AS Caracteristica
			, 1 AS Orden
		FROM
			Venta v
			-- LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
			LEFT JOIN (
				SELECT
					VentaId
					, SUM((precioUnitario + iva) * cantidad) as Importe
				FROM VentaDetalle
				WHERE Estatus = 1
				GROUP BY VentaID
			) vdc ON vdc.VentaID = v.VentaID
			LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.EstatusGenericoID = @EstGenCompletado
					AND c9.Estatus = 1
			LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		WHERE
			v.Estatus = 1
			-- AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			AND v.VentaEstatusID = @EstCobradaID
			AND v.RealizoUsuarioID = @VendedorID
			-- Se quitan las ventas de los domingos
			AND DATEPART(DW, v.Fecha) != 1
		/* GROUP BY
			v.VentaID
			, v.Fecha
			, v.Folio
			, c.Nombre
			, v.ACredito
			, v.ComisionistaClienteID
		*/
		ORDER BY Fecha
	END

END
GO

ALTER PROCEDURE pauComisionesAgrupado (
	@Desde DATE = NULL
	, @Hasta DATE = NULL
) AS BEGIN
	SET NOCOUNT ON

	/* EXEC pauComisionesAgrupado '2014-07-12', '2014-07-18'
	DECLARE @Desde DATE = '2014-04-01'
	DECLARE @Hasta DATE = '2014-05-30'
	*/

	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3
	DECLARE @EstCanceladaID INT = 4
	DECLARE @EstGenCompletado INT = 3

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)
	-- De momento no se toma en cuenta la sucursal
	DECLARE @PorComision DECIMAL(5, 2) = (SELECT (CONVERT(DECIMAL(5, 2), Valor) / 100) FROM Configuracion WHERE Nombre = 'Comisiones.Vendedor.Porcentaje')
	DECLARE @PorComision9500 DECIMAL(5, 2) = (SELECT (CONVERT(DECIMAL(5, 2), Valor) / 100) FROM Configuracion WHERE Nombre = 'Comisiones.Vendedor.Porcentaje9500')
	DECLARE @IvaMul DECIMAL(5, 2) = (SELECT (1 + (CONVERT(DECIMAL(5, 2), Valor) / 100)) FROM Configuracion WHERE Nombre = 'IVA')

	-- Procedimiento
	SELECT
		vp.VentaID
		-- , MAX(vp.Fecha) AS Fecha
		, v.SucursalID
		, v.RealizoUsuarioID
		-- , SUM((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) AS Importe
		, SUM((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) AS Utilidad
		, (
			CASE WHEN v.ComisionistaClienteID > 0 THEN
				SUM(
					CASE WHEN DATEPART(DW, v.Fecha) != 1 THEN
						((CASE com.ListaDePrecios
							WHEN 1 THEN (pp.PrecioUno / @IvaMul)
							WHEN 2 THEN (pp.PrecioDos / @IvaMul)
							WHEN 3 THEN (pp.PrecioTres / @IvaMul)
							WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
							WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
						END
						- vd.Costo) * vd.Cantidad)
						* CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE @PorComision9500 END
					ELSE 0 END)
			ELSE
				SUM(
					CASE WHEN DATEPART(DW, v.Fecha) != 1 THEN
						((CASE WHEN vd.PrecioUnitario > (pp.PrecioUno / @IvaMul) THEN (pp.PrecioUno / @IvaMul)
							ELSE vd.PrecioUnitario END
						- vd.Costo) * vd.Cantidad)
						* CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE @PorComision9500 END
					ELSE 0 END)
			END
		) AS Comision
		, 'V' AS Tipo
	FROM
		-- VentaPago vp
		(
			SELECT DISTINCT VentaID
			FROM
				VentaPago vp
				INNER JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
			WHERE
				vp.Estatus = 1
				AND (vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
				AND vpd.Importe > 0
		) vp
		INNER JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
		LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		INNER JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = vd.ParteID AND pp.Estatus = 1
		LEFT JOIN Cliente com ON com.ClienteID = v.ComisionistaClienteID AND com.Estatus = 1
		LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.EstatusGenericoID = @EstGenCompletado
			AND c9.Estatus = 1
		-- LEFT JOIN VentaDevolucion vdv ON vdv.VentaID = v.VentaID AND vdv.Estatus = 1
		-- LEFT JOIN VentaDevolucionDetalle vdvd ON vdvd.VentaDevolucionID = vdv.VentaDevolucionID AND vdvd.Estatus = 1
	WHERE
		-- vp.Estatus = 1
		-- AND (vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
		v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)
		-- Se quitan las ventas de los domingos
		-- AND DATEPART(DW, v.Fecha) != 1
		-- Se quitan las partes que no apliquen para comisión, porque tampoco aplica para utilidad
		AND p.AplicaComision = 1
	GROUP BY
		vp.VentaID
		, v.SucursalID
		, v.RealizoUsuarioID
		, v.ComisionistaClienteID
		, c9.VentaID
	
	-- Cancelaciones
	UNION ALL
	SELECT
		vd.VentaID
		-- , MAX(vp.Fecha) AS Fecha
		, v.SucursalID
		, v.RealizoUsuarioID
		-- , SUM((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) AS Importe
		, (SUM((vdd.PrecioUnitario - vdd.Costo) * vdd.Cantidad) * -1) AS Utilidad
		, ( (
			CASE WHEN v.ComisionistaClienteID > 0 THEN
				SUM(
					CASE WHEN DATEPART(DW, v.Fecha) != 1 THEN
						((CASE com.ListaDePrecios
							WHEN 1 THEN (pp.PrecioUno / @IvaMul)
							WHEN 2 THEN (pp.PrecioDos / @IvaMul)
							WHEN 3 THEN (pp.PrecioTres / @IvaMul)
							WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
							WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
						END
						- vdd.Costo) * vdd.Cantidad)
						* CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE @PorComision9500 END
					ELSE 0 END)
			ELSE
				SUM(
					CASE WHEN DATEPART(DW, v.Fecha) != 1 THEN
						((CASE WHEN vdd.PrecioUnitario > (pp.PrecioUno / @IvaMul) THEN (pp.PrecioUno / @IvaMul)
							ELSE vdd.PrecioUnitario END
						- vdd.Costo) * vdd.Cantidad)
						* CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE @PorComision9500 END
					ELSE 0 END)
			END
		) * -1) AS Comision
		, 'D' AS Tipo
	FROM
		VentaDevolucion vd
		LEFT JOIN VentaDevolucionDetalle vdd ON vdd.VentaDevolucionID = vd.VentaDevolucionID AND vdd.Estatus = 1
		INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
		INNER JOIN Parte p ON p.ParteID = vdd.ParteID AND p.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = vdd.ParteID AND pp.Estatus = 1
		LEFT JOIN Cliente com ON com.ClienteID = v.ComisionistaClienteID AND com.Estatus = 1
		LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.EstatusGenericoID = @EstGenCompletado
			AND c9.Estatus = 1
	WHERE
		vd.Estatus = 1
		AND (vd.Fecha >= @Desde AND vd.Fecha < @Hasta)
		AND v.Fecha < @Desde
		AND v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)
		-- Se quitan las ventas de los domingos
		-- AND DATEPART(DW, v.Fecha) != 1
		-- Se quitan las partes que no apliquen para comisión, porque tampoco aplica para utilidad
		AND p.AplicaComision = 1
	GROUP BY
		vd.VentaID
		, v.SucursalID
		, v.RealizoUsuarioID
		, v.ComisionistaClienteID
		, c9.VentaID

END
GO

ALTER PROCEDURE pauProUtilidadComision
AS BEGIN
	SET NOCOUNT ON

	/* Script para registrar, cada semana, la utilidad y las comisiones
	 * Creado: 2014/05/19
	 * Mod.  : 2014/07/30
	 * Por: Moisés
	 */

	DECLARE @Desde DATE = DATEADD(d, -7, GETDATE())
	DECLARE @Hasta DATE = DATEADD(d, -1, GETDATE())
	DECLARE @Registro DATE = @Hasta

	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3
	DECLARE @EstCanceladaID INT = 4
	DECLARE @EstGenCompletado INT = 3

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)
	-- De momento no se toma en cuenta la sucursal
	DECLARE @PorComision DECIMAL(5, 2) = (SELECT (CONVERT(DECIMAL(5, 2), Valor) / 100) FROM Configuracion WHERE Nombre = 'Comisiones.Vendedor.Porcentaje')
	DECLARE @PorComision9500 DECIMAL(5, 2) = (SELECT (CONVERT(DECIMAL(5, 2), Valor) / 100) FROM Configuracion WHERE Nombre = 'Comisiones.Vendedor.Porcentaje9500')
	DECLARE @IvaMul DECIMAL(5, 2) = (SELECT (1 + (CONVERT(DECIMAL(5, 2), Valor) / 100)) FROM Configuracion WHERE Nombre = 'IVA')

	-- Procedimiento
	DECLARE @Datos TABLE (
		SucursalID INT
		, VendedorID INT
		, Utilidad DECIMAL(12, 2)
		, UtilidadConDescuento DECIMAL(12, 2)
		, Comision DECIMAL(12, 2)
	)
	INSERT INTO @Datos
		SELECT
			-- vp.VentaID
			 v.SucursalID
			, v.RealizoUsuarioID
			, SUM((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) AS Utilidad
			, SUM((vd.PrecioUnitario - vd.CostoConDescuento) * vd.Cantidad) AS UtilidadConDescuento
			, (
				CASE WHEN v.ComisionistaClienteID > 0 THEN
					SUM(
						CASE WHEN DATEPART(DW, v.Fecha) != 1 THEN
							((CASE com.ListaDePrecios
								WHEN 1 THEN (pp.PrecioUno / @IvaMul)
								WHEN 2 THEN (pp.PrecioDos / @IvaMul)
								WHEN 3 THEN (pp.PrecioTres / @IvaMul)
								WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
								WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
							END
							- vd.Costo) * vd.Cantidad)
							* CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE @PorComision9500 END
						ELSE 0 END)
				ELSE
					SUM(
						CASE WHEN DATEPART(DW, v.Fecha) != 1 THEN
							((CASE WHEN vd.PrecioUnitario > (pp.PrecioUno / @IvaMul) THEN 
								(pp.PrecioUno / @IvaMul) ELSE vd.PrecioUnitario END
							- vd.Costo) * vd.Cantidad)
							* CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE @PorComision9500 END
						ELSE 0 END)
				END
			) AS Comision
		FROM
			-- VentaPago vp
			(
				SELECT DISTINCT VentaID
				FROM
					VentaPago vp
					INNER JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
				WHERE
					vp.Estatus = 1
					AND (vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
					AND vpd.Importe > 0
			) vp
			INNER JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
			LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
			INNER JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
			LEFT JOIN PartePrecio pp ON pp.ParteID = vd.ParteID AND pp.Estatus = 1
			LEFT JOIN Cliente com ON com.ClienteID = v.ComisionistaClienteID AND com.Estatus = 1
			LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.EstatusGenericoID = @EstGenCompletado
				AND c9.Estatus = 1
		WHERE
			v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)
			-- Se quitan las partes que no apliquen para comisión, porque tampoco aplica para utilidad
			AND p.AplicaComision = 1
		GROUP BY
			vp.VentaID
			, v.SucursalID
			, v.RealizoUsuarioID
			, v.ComisionistaClienteID
			, c9.VentaID
		
		-- Cancelaciones
		UNION ALL
		SELECT
			-- vd.VentaID
			 v.SucursalID
			, v.RealizoUsuarioID
			, (SUM((vdd.PrecioUnitario - vdd.Costo) * vdd.Cantidad) * -1) AS Utilidad
			, (SUM((vdd.PrecioUnitario - vdd.CostoConDescuento) * vdd.Cantidad) * -1) AS UtilidadConDescuento
			, ( (
				CASE WHEN v.ComisionistaClienteID > 0 THEN
					SUM(
						CASE WHEN DATEPART(DW, v.Fecha) != 1 THEN
							((CASE com.ListaDePrecios
								WHEN 1 THEN (pp.PrecioUno / @IvaMul)
								WHEN 2 THEN (pp.PrecioDos / @IvaMul)
								WHEN 3 THEN (pp.PrecioTres / @IvaMul)
								WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
								WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
							END
							- vdd.Costo) * vdd.Cantidad)
							* CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE @PorComision9500 END
						ELSE 0 END)
				ELSE
					SUM(
						CASE WHEN DATEPART(DW, v.Fecha) != 1 THEN
							((CASE WHEN vdd.PrecioUnitario > (pp.PrecioUno / @IvaMul) THEN
								(pp.PrecioUno / @IvaMul) ELSE vdd.PrecioUnitario END
							- vdd.Costo) * vdd.Cantidad)
							* CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE @PorComision9500 END
						ELSE 0 END)
				END
			) * -1) AS Comision
		FROM
			VentaDevolucion vd
			LEFT JOIN VentaDevolucionDetalle vdd ON vdd.VentaDevolucionID = vd.VentaDevolucionID AND vdd.Estatus = 1
			INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
			INNER JOIN Parte p ON p.ParteID = vdd.ParteID AND p.Estatus = 1
			LEFT JOIN PartePrecio pp ON pp.ParteID = vdd.ParteID AND pp.Estatus = 1
			LEFT JOIN Cliente com ON com.ClienteID = v.ComisionistaClienteID AND com.Estatus = 1
			LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.EstatusGenericoID = @EstGenCompletado
				AND c9.Estatus = 1
		WHERE
			vd.Estatus = 1
			AND (vd.Fecha >= @Desde AND vd.Fecha < @Hasta)
			AND v.Fecha < @Desde
			AND v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)
			-- Se quitan las partes que no apliquen para comisión, porque tampoco aplica para utilidad
			AND p.AplicaComision = 1
		GROUP BY
			vd.VentaID
			, v.SucursalID
			, v.RealizoUsuarioID
			, v.ComisionistaClienteID
			, c9.VentaID

	-- Se insertan las utilidades
	INSERT INTO MetaUtilidadSucursal (SucursalID, Fecha, Utilidad, UtilidadConDescuento)
		SELECT
			SucursalID
			, @Registro
			, SUM(Utilidad)
			, SUM(UtilidadConDescuento)
		FROM @Datos
		GROUP BY SucursalID

	-- Se insertan las comisiones
	INSERT INTO MetaComisionVendedor (VendedorID, Fecha, Comision)
		SELECT
			VendedorID
			, @Registro
			, SUM(Comision)
		FROM @Datos
		GROUP BY VendedorID
END
GO

ALTER PROCEDURE pauCuadroDeControlGeneral (
	@SucursalID INT = NULL
	, @Pagadas BIT
	, @Cobradas BIT
	, @Solo9500 BIT
	, @OmitirDomingo BIT
	, @CostoConDescuento BIT
	, @Desde DATE = NULL
	, @Hasta DATE = NULL
) AS BEGIN
	SET NOCOUNT ON

	/* EXEC pauCuadroDeControlGeneral 1, 1, 1, 0, 0, 1, '2014-01-01', '2014-08-01'
	DECLARE @SucursalID INT = 1
	DECLARE @Pagadas BIT = 1
	DECLARE @Cobradas BIT = 1
	DECLARE @Anio INT = 2014
	DECLARE @Desde DATE = '2014-06-01'
	DECLARE @Hasta DATE = '2014-06-30'
	*/

	-- Definición de variables tipo constante
	DECLARE @EstCobradaID INT = 2
	DECLARE @EstPagadaID INT = 3
	DECLARE @EstCanceladaID INT = 4
	DECLARE @EstCanceladaSinPagoID INT = 5
	
	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)
	DECLARE @DesdeAnt DATE = DATEADD(YEAR, -1, @Desde)
	DECLARE @HastaAnt DATE = DATEADD(YEAR, -1, @Hasta)
	
	-- Consulta
	SELECT
		vpc.VentaID
		, v.Folio
		, vpc.Fecha
		, v.SucursalID
		, s.NombreSucursal AS Sucursal
		, v.ClienteID
		, c.Nombre AS Cliente
		, v.RealizoUsuarioID AS VendedorID
		, u.NombreUsuario AS Vendedor
		, l.LineaID
		, l.NombreLinea AS Linea
		, pv.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, SUM(
			CASE WHEN (vpc.Fecha >= @Desde AND vpc.Fecha < @Hasta) THEN
				((vd.PrecioUnitario - CASE WHEN @CostoConDescuento = 1 THEN vd.CostoConDescuento ELSE vd.Costo END)
				* vd.Cantidad)
			ELSE
				0
			END) AS Actual
		, SUM(
			CASE WHEN (vpc.Fecha >= @DesdeAnt AND vpc.Fecha < @HastaAnt) THEN
				((vd.PrecioUnitario - CASE WHEN @CostoConDescuento = 1 THEN vd.CostoConDescuento ELSE vd.Costo END)
				* vd.Cantidad)
			ELSE
				0
			END) AS Anterior
	FROM
		-- VentaPago vp
		-- INNER JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
		(
			SELECT
				VentaID
				, MAX(vp.Fecha) AS Fecha
			FROM
				VentaPago vp
				INNER JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
			WHERE
				vp.Estatus = 1
				AND (
					(vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
					OR (vp.Fecha >= @DesdeAnt AND vp.Fecha < @HastaAnt)
				)
				AND vpd.Importe > 0
			GROUP BY VentaID
		) vpc
		INNER JOIN Venta v ON v.VentaID = vpc.VentaID AND v.Estatus = 1
		LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = v.SucursalID AND s.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
		INNER JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
	WHERE
		-- vp.Estatus = 1
		-- AND (vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
		-- AND vpd.Importe > 0
		p.AplicaComision = 1
		
		AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
		AND (
			(@Pagadas = 1 AND (v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)))
			OR (@Cobradas = 1 AND (v.VentaEstatusID IN (@EstCobradaID)))
		)
		AND (@Solo9500 = 0 OR c9.Cotizacion9500ID IS NOT NULL)
		/* AND (
			(vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
			OR (vp.Fecha >= @DesdeAnt AND vp.Fecha < @HastaAnt)
		)
		*/
		-- Se quitan las ventas de los domingos, si aplica
		AND (@OmitirDomingo = 0 OR DATEPART(DW, v.Fecha) != 1)
	GROUP BY
		vpc.VentaID
		, v.Folio
		, vpc.Fecha
		, v.SucursalID
		, s.NombreSucursal
		, v.ClienteID
		, c.Nombre
		, v.RealizoUsuarioID
		, u.NombreUsuario
		, l.LineaID
		, l.NombreLinea
		, pv.ProveedorID
		, pv.NombreProveedor

	-- Canceladas
	UNION ALL
	SELECT
		vd.VentaID
		, v.Folio
		, vd.Fecha
		, v.SucursalID
		, s.NombreSucursal AS Sucursal
		, v.ClienteID
		, c.Nombre AS Cliente
		, v.RealizoUsuarioID AS VendedorID
		, u.NombreUsuario AS Vendedor
		, l.LineaID
		, l.NombreLinea AS Linea
		, pv.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, (SUM(
			CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta) THEN
				((vdd.PrecioUnitario - CASE WHEN @CostoConDescuento = 1 THEN vdd.CostoConDescuento ELSE vdd.Costo END)
				* vdd.Cantidad)
			ELSE
				0
			END) * -1) AS Actual
		, (SUM(
			CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt) THEN
				((vdd.PrecioUnitario - CASE WHEN @CostoConDescuento = 1 THEN vdd.CostoConDescuento ELSE vdd.Costo END)
				* vdd.Cantidad)
			ELSE
				0
			END) * -1) AS Anterior
	FROM
		VentaDevolucion vd
		LEFT JOIN VentaDevolucionDetalle vdd ON vdd.VentaDevolucionID = vd.VentaDevolucionID AND vdd.Estatus = 1
		INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
		LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = v.SucursalID AND s.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
		INNER JOIN Parte p ON p.ParteID = vdd.ParteID AND p.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
	WHERE
		vd.Estatus = 1
		-- AND (vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
		AND p.AplicaComision = 1
		AND v.Fecha < @Desde
		
		AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
		AND (
			(@Pagadas = 1 AND (v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)))
			-- Se quita que se consideren las canceladas sin pago, porque restan cuando no deberían
			OR (@Cobradas = 1 AND (v.VentaEstatusID IN (@EstCobradaID)))
		)
		AND (@Solo9500 = 0 OR c9.Cotizacion9500ID IS NOT NULL)
		AND (
			(vd.Fecha >= @Desde AND vd.Fecha < @Hasta)
			OR (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt)
		)
		-- Se quitan las ventas de los domingos, si aplica
		AND (@OmitirDomingo = 0 OR DATEPART(DW, v.Fecha) != 1)
	GROUP BY
		vd.VentaID
		, v.Folio
		, vd.Fecha
		, v.SucursalID
		, s.NombreSucursal
		, v.ClienteID
		, c.Nombre
		, v.RealizoUsuarioID
		, u.NombreUsuario
		, l.LineaID
		, l.NombreLinea
		, pv.ProveedorID
		, pv.NombreProveedor

	/* ORDER BY
		DATEPART(DAYOFYEAR, vp.Fecha)
		, DATEPART(HOUR, vp.Fecha)
	*/

END
GO

ALTER PROCEDURE pauCuadroDeControlCancelaciones (
	@SucursalID INT = NULL
	, @Pagadas BIT
	, @Cobradas BIT
	, @Solo9500 BIT
	, @OmitirDomingo BIT
	, @CostoConDescuento BIT
	, @Desde DATE = NULL
	, @Hasta DATE = NULL
) AS BEGIN
	SET NOCOUNT ON
	
	/* EXEC pauCuadroDeControlCancelaciones 1, 1, 1, 0, 0, 1, '2014-01-01', '2014-07-30'
	DECLARE @SucursalID INT = 1
	DECLARE @Pagadas BIT = 1
	DECLARE @Cobradas BIT = 1

	DECLARE @Desde DATE = '2014-06-01'
	DECLARE @Hasta DATE = '2014-06-30'
	*/

	-- Definición de variables tipo constante
	DECLARE @EstCancelada INT = 4
	DECLARE @EstCanceladaSinPago INT = 5
	
	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)
	DECLARE @DesdeAnt DATE = DATEADD(YEAR, -1, @Desde)
	DECLARE @HastaAnt DATE = DATEADD(YEAR, -1, @Hasta)
	
	-- Consulta
	SELECT
		vd.VentaDevolucionID
		, vd.MotivoID
		, vd.TipoFormaPagoID
		, v.VentaID
		, v.Folio
		, vd.Fecha
		, v.SucursalID
		, s.NombreSucursal AS Sucursal
		, v.ClienteID
		, c.Nombre AS Cliente
		, v.RealizoUsuarioID AS VendedorID
		, u.NombreUsuario AS Vendedor
		, l.LineaID
		, l.NombreLinea AS Linea
		, pv.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, SUM(
			CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta) THEN
				((vdd.PrecioUnitario - CASE WHEN @CostoConDescuento = 1 THEN vdd.CostoConDescuento ELSE vdd.Costo END)
				* vdd.Cantidad)
			ELSE
				0
			END) AS Actual
		, SUM(
			CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt) THEN
				((vdd.PrecioUnitario - CASE WHEN @CostoConDescuento = 1 THEN vdd.CostoConDescuento ELSE vdd.Costo END)
				* vdd.Cantidad)
			ELSE
				0
			END) AS Anterior
		, SUM(CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta) THEN vdd.Cantidad ELSE 0 END) AS CantidadActual
		, SUM(CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt) THEN vdd.Cantidad ELSE 0 END) AS CantidadAnterior
		, CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta) THEN 1 ELSE 2 END AS ActualAnt
	FROM
		VentaDevolucion vd
		LEFT JOIN VentaDevolucionDetalle vdd ON vdd.VentaDevolucionID = vd.VentaDevolucionID AND vdd.Estatus = 1
		INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
		LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = vd.VentaID AND c9.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = vd.SucursalID AND s.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
		INNER JOIN Parte p ON p.ParteID = vdd.ParteID AND p.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
	WHERE
		vd.Estatus = 1
		AND p.AplicaComision = 1
		AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
		AND (
			(@Pagadas = 1 AND v.VentaEstatusID = @EstCancelada)
			OR (@Cobradas = 1 AND v.VentaEstatusID = @EstCanceladaSinPago)
		)
		AND (@Solo9500 = 0 OR c9.Cotizacion9500ID IS NOT NULL)
		AND (
			(vd.Fecha >= @Desde AND vd.Fecha < @Hasta)
			OR (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt)
		)
		-- Se quitan las ventas de los domingos, si aplica
		AND (@OmitirDomingo = 0 OR DATEPART(DW, v.Fecha) != 1)
	GROUP BY
		vd.VentaDevolucionID
		, vd.MotivoID
		, vd.TipoFormaPagoID
		, v.VentaID
		, v.Folio
		, vd.Fecha
		, v.SucursalID
		, s.NombreSucursal
		, v.ClienteID
		, c.Nombre
		, v.RealizoUsuarioID
		, u.NombreUsuario
		, l.LineaID
		, l.NombreLinea
		, pv.ProveedorID
		, pv.NombreProveedor
	ORDER BY
		DATEPART(DAYOFYEAR, vd.Fecha)
		, DATEPART(HOUR, vd.Fecha)

END
GO

ALTER PROCEDURE pauContaCuentasPorSemana (
	@Desde DATE
	, @Hasta DATE
) AS BEGIN
	SET NOCOUNT ON
	
	/* EXEC pauContaCuentasPorSemana '2014-01-01', '2014-12-31'
	DECLARE @Desde DATE = '2014-01-01'
	DECLARE @Hasta DATE = '2014-12-31'
	*/

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)
	
	-- Inicio del Procedimiento
	SELECT
		ced.ContaEgresoDevengadoID
		, ced.ContaEgresoID
		, ced.SucursalID
		, s.NombreSucursal AS Sucursal
		, cc.ContaCuentaID
		, cc.Cuenta
		, cs.ContaSubcuentaID
		, cs.Subcuenta
		, ccm.ContaCuentaDeMayorID
		, ccm.CuentaDeMayor
		, cca.ContaCuentaAuxiliarID
		, cca.CuentaAuxiliar
		-- , ced.Fecha
		-- , DATEPART(WEEK, ced.Fecha) AS Semana
		, CONVERT(DATE, CASE WHEN DATEPART(DW, ced.Fecha) = 7 THEN ced.Fecha
			ELSE DATEADD(DAY, (DATEPART(DW, ced.Fecha) * -1), ced.Fecha) END) AS DiaIni
		, SUM(ced.Importe) AS ImporteDev
		-- , ce.Observaciones AS Egreso
		-- , cca.CalculoSemanal
		-- , cca.DiasMovimiento
		-- , (ISNULL(cca.DiasMovimiento, 7) / 7) AS Semanas
		, SUM(CASE WHEN cca.CalculoSemanal = 1 THEN ((ced.Importe / cca.DivisorDia) * 7) ELSE ced.Importe END) AS Importe
	FROM
		ContaEgresoDevengado ced
		LEFT JOIN ContaEgreso ce ON ce.ContaEgresoID = ced.ContaEgresoID AND ce.Estatus = 1
		LEFT JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = ce.ContaCuentaAuxiliarID

		LEFT JOIN Sucursal s ON s.SucursalID = ced.SucursalID AND s.Estatus = 1
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = cca.ContaCuentaDeMayorID
		LEFT JOIN ContaSubcuenta cs ON cs.ContaSubcuentaID = ccm.ContaSubcuentaID
		LEFT JOIN ContaCuenta cc ON cc.ContaCuentaID = cs.ContaCuentaID
	WHERE
		(ced.Fecha >= @Desde AND ced.Fecha < @Hasta)
	GROUP BY
		ced.ContaEgresoDevengadoID
		, ced.ContaEgresoID
		, ced.SucursalID
		, s.NombreSucursal
		, cc.ContaCuentaID
		, cc.Cuenta
		, cs.ContaSubcuentaID
		, cs.Subcuenta
		, ccm.ContaCuentaDeMayorID
		, ccm.CuentaDeMayor
		, cca.ContaCuentaAuxiliarID
		, cca.CuentaAuxiliar
		-- , DATEPART(WEEK, ced.Fecha)
		, CONVERT(DATE, CASE WHEN DATEPART(DW, ced.Fecha) = 7 THEN ced.Fecha
			ELSE DATEADD(DAY, (DATEPART(DW, ced.Fecha) * -1), ced.Fecha) END)
		-- , cca.DiasMovimiento
	ORDER BY
		Sucursal
		, Cuenta
		, Subcuenta
		, CuentaDeMayor
		, CuentaAuxiliar

END
GO