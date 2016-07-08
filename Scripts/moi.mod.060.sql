/* Script con modificaciones a la base de datos de Theos. Archivo 060
 * Creado: 2016/01/05
 * Subido: 2016/01/06
 */

DECLARE @ScriptID INT = 60
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)
-- SELECT * FROM ScriptSql ORDER BY ScriptID DESC

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
	('Reportes.Traspasos.Excedente', 'D', 'I', 'Salida donde debe mostrarse el reporte de excedentes en traspasos.')

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO



/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

ALTER PROCEDURE [dbo].[pauComisiones] (
	@ModoID INT
	, @VendedorID INT
	, @Desde DATE = NULL
	, @Hasta DATE = NULL
) AS BEGIN
	SET NOCOUNT ON

	/*
	DECLARE @ModoID INT = 1
	DECLARE @VendedorID INT = 1
	DECLARE @Desde DATE = '2016-01-02'
	DECLARE @Hasta DATE = '2016-01-08'
	-- EXEC pauComisiones 1, 1, '2016-01-02', '2016-01-08'
	*/

	-- Definición de variables tipo constante
	DECLARE @ModoNormalID INT = 1
	DECLARE @ModoNoPagadasID INT = 2
	
	DECLARE @EstCobradaID INT = 2
	DECLARE @EstPagadaID INT = 3
	DECLARE @EstCanceladaID INT = 4
	DECLARE @EstGenCompletado INT = 3
	DECLARE @EstGenCanceladoDP INT = 13
	DECLARE @NcOrigenGarantia INT = 5

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)
	-- De momento no se toma en cuenta la sucursal
	DECLARE @PorComision DECIMAL(5, 2) = (SELECT (CONVERT(DECIMAL(5, 2), Valor) / 100) FROM Configuracion WHERE Nombre = 'Comisiones.Vendedor.Porcentaje')
	DECLARE @PorComision9500 DECIMAL(5, 2) = ISNULL((SELECT TOP 1 (Porcentaje9500 / 100) FROM MetaVendedor WHERE VendedorID = @VendedorID), 0.00)
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
				-- Utilidad
				, (SELECT SUM((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) FROM VentaDetalle vd
					INNER JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
					WHERE vd.Estatus = 1 AND vd.VentaID = vp.VentaID AND p.AplicaComision = 1)
				AS Utilidad
				, (SELECT SUM((vdd.PrecioUnitario - vdd.Costo) * vdd.Cantidad) FROM
					VentaDevolucion vd
					INNER JOIN VentaDevolucionDetalle vdd ON vdd.VentaDevolucionID = vd.VentaDevolucionID AND vdd.Estatus = 1
					INNER JOIN Parte p ON p.ParteID = vdd.ParteID AND p.Estatus = 1
					WHERE vdd.Estatus = 1 AND vd.VentaID = vp.VentaID AND p.AplicaComision = 1)
				AS UtilidadDev
				-- Comisión
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
				LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID
					AND (c9.EstatusGenericoID = @EstGenCompletado OR c9.EstatusGenericoID = @EstGenCanceladoDP)
					AND c9.Estatus = 1
				LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
				-- Para lo de garantías
				LEFT JOIN NotaDeCredito ncg ON ncg.NotaDeCreditoID = vpd.NotaDeCreditoID AND ncg.OrigenID = @NcOrigenGarantia
			WHERE
				vp.Estatus = 1
				AND (vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
				AND vpd.Importe > 0
				AND v.RealizoUsuarioID = @VendedorID
				AND v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)
				-- Se quitan las ventas de los domingos, menos 9500
				AND (DATEPART(DW, v.Fecha) != 1 OR c9.VentaID IS NOT NULL)
				-- Se quitan las ventas pagadas con vales de garantías
				AND ncg.NotaDeCreditoID IS NULL
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
				-- Utilidad
				, (SELECT SUM((vddi.PrecioUnitario - vddi.Costo) * vddi.Cantidad)
					FROM VentaDevolucion vdi
					INNER JOIN VentaDevolucionDetalle vddi ON vddi.VentaDevolucionID = vdi.VentaDevolucionID AND vddi.Estatus = 1
					INNER JOIN Parte p ON p.ParteID = vddi.ParteID AND p.Estatus = 1
					WHERE vdi.Estatus = 1 AND vdi.VentaID = vd.VentaID AND p.AplicaComision = 1)
				AS Utilidad
				-- Comisión
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
				LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID
					AND (c9.EstatusGenericoID = @EstGenCompletado OR c9.EstatusGenericoID = @EstGenCanceladoDP)
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
				-- AND DATEPART(DW, v.Fecha) != 1
				AND (DATEPART(DW, v.Fecha) != 1 OR c9.VentaID IS NOT NULL)
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
			, (ISNULL(Utilidad, 0.00) + ISNULL(UtilidadDev, 0.00)) AS Utilidad
			, (ISNULL(Comision, 0.00) + ISNULL(ComisionDev, 0.00)) AS Comision
			, ACredito
			, CASE WHEN ComisionDev IS NULL THEN Caracteristica ELSE ('VD'
				+ (CASE WHEN Caracteristica LIKE '%9500%' THEN '9500' ELSE '' END)) END AS Caracteristica
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
			, ISNULL((Utilidad * -1), 0.00) AS Utilidad
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
			, 0.00 AS Utilidad
			-- Comisión
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
					, SUM((PrecioUnitario + Iva) * Cantidad) AS Importe
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
			-- AND DATEPART(DW, v.Fecha) != 1
			AND (DATEPART(DW, v.Fecha) != 1 OR c9.VentaID IS NOT NULL)
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