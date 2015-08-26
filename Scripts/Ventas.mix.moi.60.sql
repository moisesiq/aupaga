/* Script con modificaciones para el módulo de ventas. Archivo 60
 * Creado: 2014/09/19
 * Subido: 2014/09/23
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	CREATE INDEX Ix_Estatus ON Parte(Estatus)
	CREATE INDEX Ix_Estatus ON ParteEquivalente(Estatus)
	CREATE INDEX Ix_Estatus ON PartePrecio(Estatus)
	CREATE INDEX Ix_Estatus ON ParteExistencia(Estatus)
	CREATE INDEX Ix_Estatus ON ParteImagen(Estatus)
	CREATE INDEX Ix_Estatus ON ParteCodigoAlterno(Estatus)
	CREATE INDEX Ix_Estatus ON ParteVehiculo(Estatus)
	CREATE INDEX Ix_Estatus ON Linea(Estatus)
	CREATE INDEX Ix_Estatus ON MarcaParte(Estatus)
	CREATE INDEX Ix_Estatus ON Proveedor(Estatus)
	CREATE INDEX Ix_Estatus ON Subsistema(Estatus)
	CREATE INDEX Ix_Estatus ON Sistema(Estatus)

	CREATE INDEX Ix_ParteID ON ParteEquivalente(ParteID)
	CREATE INDEX Ix_ParteID ON PartePrecio(ParteID)
	CREATE INDEX Ix_ParteID ON ParteExistencia(ParteID)
	CREATE INDEX Ix_ParteID ON ParteImagen(ParteID)
	CREATE INDEX Ix_ParteID ON ParteCodigoAlterno(ParteID)
	CREATE INDEX Ix_ParteID ON ParteVehiculo(ParteID)

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

ALTER VIEW [dbo].[ClienteKardexDetalleView] AS
	SELECT
		Venta.VentaID
		,Venta.Fecha
		,Venta.Folio
		,Venta.ClienteID
		, Venta.SucursalID
		,Sucursal.NombreSucursal AS Sucursal
		,VentaDetalle.ParteID
		, VentaDetalle.Cantidad
		,VentaDetalle.PrecioUnitario
		,VentaDetalle.Iva
		,VentaDetalle.PrecioUnitario + VentaDetalle.Iva AS Importe
		,VentaEstatus.Descripcion AS EstatusActual
		, cf.NumeroEconomico
		, md.NombreModelo AS Modelo
		, cf.Anio
		, cf.Placa
		, Venta.Kilometraje
	FROM
		Venta
		INNER JOIN VentaDetalle ON Venta.VentaID = VentaDetalle.VentaID AND VentaDetalle.Estatus = 1
		INNER JOIN VentaEstatus ON VentaEstatus.VentaEstatusID = Venta.VentaEstatusID
		INNER JOIN Parte ON Parte.ParteID = VentaDetalle.ParteID AND Parte.Estatus = 1
		INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = Parte.MarcaParteID AND MarcaParte.Estatus = 1
		INNER JOIN Linea ON Linea.LineaID = Parte.LineaID AND Linea.Estatus = 1
		INNER JOIN Proveedor ON Proveedor.ProveedorID = Parte.ProveedorID AND Proveedor.Estatus = 1
		INNER JOIN Sucursal ON Sucursal.SucursalID = Venta.SucursalID AND Sucursal.Estatus = 1
		LEFT JOIN ClienteFlotilla cf ON cf.ClienteFlotillaID = Venta.ClienteVehiculoID AND cf.Estatus = 1
		LEFT JOIN Motor mt ON mt.MotorID = cf.MotorID AnD mt.Estatus = 1
		LEFT JOIN Modelo md ON md.ModeloID = mt.ModeloID AND md.Estatus = 1
	WHERE
		Venta.Estatus = 1
GO

CREATE VIEW PartesExistenciasView AS
	SELECT
		p.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, p.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, p.MarcaParteID AS MarcaID
		, mp.NombreMarcaParte AS Marca
		, p.LineaID
		, l.NombreLinea AS Linea
		, pp.Costo
		, SUM(pe.Existencia) AS Existencia
		, (pp.Costo * SUM(pe.Existencia)) AS CostoTotal
	FROM
		Parte p
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND p.Estatus = 1
	WHERE
		p.Estatus = 1
	GROUP BY
		p.ParteID
		, p.NumeroParte
		, p.NombreParte
		, p.ProveedorID
		, pv.NombreProveedor
		, p.MarcaParteID
		, mp.NombreMarcaParte
		, p.LineaID
		, l.NombreLinea
		, pp.Costo
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

/**/
ALTER PROCEDURE [dbo].[pauClienteKardex] (
	@ClienteID AS INT
	, @FechaInicial AS DATE
	, @FechaFinal AS DATE
	, @SucursalID AS INT = NULL
	, @VehiculoID INT = NULL
) 
AS BEGIN
SET NOCOUNT ON

/* DECLARE @ClienteID AS INT = 1
DECLARE @FechaInicial AS DATE = '2014-06-19'
DECLARE @FechaFinal AS DATE = GETDATE()
DECLARE @SucursalID AS INT = null
DECLARE @VehiculoID INT = null
*/

	DECLARE @EstCobrada INT = 2
	DECLARE @EstPagada INT = 3

	SELECT
		v.VentaID
		, vd.ParteID
		, p.NumeroParte
		, p.NombreParte AS Descripcion
		, m.NombreMarcaParte AS Marca
		, l.NombreLinea AS Linea
		, pv.NombreProveedor AS Proveedor
		, SUM(vd.Cantidad) AS Cantidad
		, p.NumeroParte + ' ' + p.NombreParte AS Busqueda
	FROM
		Venta v
		LEFT JOIN VentaDetalle vd ON v.VentaID = vd.VentaID AND vd.Estatus = 1
		-- LEFT JOIN VentaEstatus ve ON ve.VentaEstatusID = v.VentaEstatusID
		LEFT JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
		LEFT JOIN MarcaParte m ON m.MarcaParteID = p.MarcaParteID AND m.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		-- LEFT JOIN Sucursal s ON s.SucursalID = v.SucursalID AND s.Estatus = 1
	WHERE
		v.Estatus = 1
		AND v.ClienteID = @ClienteID		
		AND v.Fecha BETWEEN @FechaInicial AND @FechaFinal
		AND v.VentaEstatusID IN (@EstCobrada, @EstPagada)
		AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
		AND (@VehiculoID IS NULL OR v.ClienteVehiculoID = @VehiculoID)
	GROUP BY
		v.VentaID
		, vd.ParteID
		, p.NumeroParte
		, p.NombreParte
		, m.NombreMarcaParte
		, l.NombreLinea
		, pv.NombreProveedor
	ORDER BY
		l.NombreLinea

END
GO

ALTER PROCEDURE [dbo].[pauComisiones] (
	@ModoID INT
	, @VendedorID INT
	, @Desde DATE = NULL
	, @Hasta DATE = NULL
) AS BEGIN
	SET NOCOUNT ON

	/* EXEC pauComisiones 1, 1, '2014-06-12', '2014-09-18'
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
				LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.EstatusGenericoID = @EstGenCompletado
					AND c9.Estatus = 1
				LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
			WHERE
				vp.Estatus = 1
				AND (vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
				AND vpd.Importe > 0
				AND v.RealizoUsuarioID = @VendedorID
				AND v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)
				-- Se quitan las ventas de los domingos, menos 9500
				AND (DATEPART(DW, v.Fecha) != 1 OR c9.VentaID IS NOT NULL)
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

ALTER PROCEDURE [dbo].[pauComisionesAgrupado] (
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
	-- DECLARE @PorComision9500 DECIMAL(5, 2) = (SELECT (CONVERT(DECIMAL(5, 2), Valor) / 100) FROM Configuracion WHERE Nombre = 'Comisiones.Vendedor.Porcentaje9500')
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
					CASE WHEN (DATEPART(DW, v.Fecha) != 1 OR c9.VentaID IS NOT NULL) THEN
						((CASE com.ListaDePrecios
							WHEN 1 THEN (pp.PrecioUno / @IvaMul)
							WHEN 2 THEN (pp.PrecioDos / @IvaMul)
							WHEN 3 THEN (pp.PrecioTres / @IvaMul)
							WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
							WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
						END
						- vd.Costo) * vd.Cantidad)
						* CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE (mv.Porcentaje9500 / 100) END
					ELSE 0 END)
			ELSE
				SUM(
					CASE WHEN (DATEPART(DW, v.Fecha) != 1 OR c9.VentaID IS NOT NULL) THEN
						((CASE WHEN vd.PrecioUnitario > (pp.PrecioUno / @IvaMul) THEN (pp.PrecioUno / @IvaMul)
							ELSE vd.PrecioUnitario END
						- vd.Costo) * vd.Cantidad)
						* CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE (mv.Porcentaje9500 / 100) END
					ELSE 0 END)
			END
		) AS Comision
		, 'V' AS Tipo
		, CONVERT(BIT, CASE WHEN c9.VentaID IS NULL THEN 0 ELSE 1 END) AS Es9500
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
		LEFT JOIN MetaVendedor mv ON mv.VendedorID = v.RealizoUsuarioID
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
					CASE WHEN (DATEPART(DW, v.Fecha) != 1 OR c9.VentaID IS NOT NULL) THEN
						((CASE com.ListaDePrecios
							WHEN 1 THEN (pp.PrecioUno / @IvaMul)
							WHEN 2 THEN (pp.PrecioDos / @IvaMul)
							WHEN 3 THEN (pp.PrecioTres / @IvaMul)
							WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
							WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
						END
						- vdd.Costo) * vdd.Cantidad)
						* CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE (mv.Porcentaje9500 / 100) END
					ELSE 0 END)
			ELSE
				SUM(
					CASE WHEN (DATEPART(DW, v.Fecha) != 1 OR c9.VentaID IS NOT NULL) THEN
						((CASE WHEN vdd.PrecioUnitario > (pp.PrecioUno / @IvaMul) THEN (pp.PrecioUno / @IvaMul)
							ELSE vdd.PrecioUnitario END
						- vdd.Costo) * vdd.Cantidad)
						* CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE (mv.Porcentaje9500 / 100) END
					ELSE 0 END)
			END
		) * -1) AS Comision
		, 'D' AS Tipo
		, CONVERT(BIT, CASE WHEN c9.VentaID IS NULL THEN 0 ELSE 1 END) AS Es9500
	FROM
		VentaDevolucion vd
		LEFT JOIN VentaDevolucionDetalle vdd ON vdd.VentaDevolucionID = vd.VentaDevolucionID AND vdd.Estatus = 1
		INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
		INNER JOIN Parte p ON p.ParteID = vdd.ParteID AND p.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = vdd.ParteID AND pp.Estatus = 1
		LEFT JOIN Cliente com ON com.ClienteID = v.ComisionistaClienteID AND com.Estatus = 1
		LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.EstatusGenericoID = @EstGenCompletado
			AND c9.Estatus = 1
		LEFT JOIN MetaVendedor mv ON mv.VendedorID = v.RealizoUsuarioID
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

ALTER PROCEDURE [dbo].[pauCuadroDeControlGeneral] (
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
			UNION ALL
			SELECT
				VentaID
				, Fecha
			FROM Venta v
			WHERE
				@Cobradas = 1
				AND v.Estatus = 1
				AND v.VentaEstatusID = @EstCobradaID
				AND (
					(v.Fecha >= @Desde AND v.Fecha < @Hasta)
					OR (v.Fecha >= @DesdeAnt AND v.Fecha < @HastaAnt)
				)
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
