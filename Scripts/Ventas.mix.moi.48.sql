/* Script con modificaciones para el módulo de ventas. Archivo 48
 * Creado: 2014/07/04
 * Subido: 2014/07/23
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	ALTER TABLE Cliente ADD Vip BIT NULL

	ALTER TABLE MovimientoInventarioDetalle ADD PrecioUnitarioConDescuento DECIMAL(12, 2) NULL

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

ALTER VIEW [dbo].[ClientesView]
AS
	SELECT
		Cliente.ClienteID
		,ClienteFacturacion.Rfc
		,Cliente.Nombre
		,Cliente.Calle
		,Cliente.NumeroInterior
		,Cliente.NumeroExterior
		,Cliente.Colonia
		,Cliente.CodigoPostal
		,Cliente.EstadoID
		,Estado.NombreEstado
		,Cliente.MunicipioID
		,Municipio.NombreMunicipio
		,Cliente.CiudadID
		,Ciudad.NombreCiudad
		,Cliente.Telefono
		,Cliente.Celular
		,Cliente.Particular
		,Cliente.Nextel
		,Cliente.ListaDePrecios
		,ISNULL(Cliente.TieneCredito, 0) AS TieneCredito
		-- ,Cliente.Credito
		,Cliente.DiasDeCredito
		,Cliente.TipoFormaPagoID
		,TipoFormaPago.NombreTipoFormaPago
		,Cliente.BancoID
		,Banco.NombreBanco
		,Cliente.TipoClienteID
		,TipoCliente.NombreTipoCliente
		,Cliente.CuentaBancaria
		,ISNULL(Cliente.Tolerancia, 0) AS Tolerancia
		,Cliente.LimiteCredito
		,ISNULL(Cliente.EsClienteComisionista, 0) AS EsClienteComisionista
		,ISNULL(Cliente.EsTallerElectrico, 0) AS EsTallerElectrico
		,ISNULL(Cliente.EsTallerMecanico, 0) AS EsTallerMecanico
		,ISNULL(Cliente.EsTallerDiesel, 0) AS EsTallerDiesel
		,ISNULL(Cliente.Vip, 0) AS Vip
		,Cliente.ClienteComisionistaID
		,Com.Nombre AS NombreClienteComisionista
		,Cliente.FechaRegistro
		,Cliente.Alias
		,dbo.fnuClienteCreditoDeuda(Cliente.ClienteID, 1) AS DeudaActual
		,dbo.fnuClienteCreditoDeuda(Cliente.ClienteID, 2) AS DeudaVencido
		,dbo.fnuClientePromedioPago(Cliente.ClienteID, 1) AS PromedioPagoTotal
		,dbo.fnuClientePromedioPago(Cliente.ClienteID, 2) AS PromedioPagoTresMeses
		,Cliente.Acumulado AS AcumuladoH
		,dbo.fnuClienteAcumulado(Cliente.ClienteID) AS Acumulado
	FROM
		Cliente
		LEFT JOIN ClienteFacturacion ON ClienteFacturacion.ClienteID = Cliente.ClienteID
		INNER JOIN Estado ON Estado.EstadoID = Cliente.EstadoID
		INNER JOIN Municipio ON Municipio.MunicipioID = Cliente.MunicipioID
		INNER JOIN Ciudad ON Ciudad.CiudadID = Cliente.CiudadID
		LEFT JOIN TipoFormaPago ON TipoFormaPago.TipoFormaPagoID = Cliente.TipoFormaPagoID
		LEFT JOIN Banco ON Banco.BancoID = Cliente.BancoID
		LEFT JOIN TipoCliente ON TipoCliente.TipoClienteID = Cliente.TipoClienteID
		LEFT JOIN Cliente Com ON Com.ClienteComisionistaID = Cliente.ClienteID
	WHERE
		Cliente.Estatus = 1
GO

ALTER VIEW VentasDetalleView AS
	SELECT
		vd.VentaDetalleID
		, vd.VentaID
		, vd.ParteID
		, p.NumeroParte
		, p.NombreParte
		, vd.Costo
		, vd.CostoConDescuento
		, vd.Cantidad
		, vd.PrecioUnitario
		, vd.Iva
		, m.NombreMedida AS Medida
		, p.LineaID
	FROM
		VentaDetalle vd
		LEFT JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
		LEFT JOIN Medida m ON m.MedidaID = p.MedidaID AND p.Estatus = 1
	WHERE vd.Estatus = 1
GO

ALTER VIEW [dbo].[MovimientoInventarioDetalleView]
AS
SELECT 
	MovimientoInventarioDetalle.MovimientoInventarioDetalleID
	,MovimientoInventarioDetalle.MovimientoInventarioID
	,MovimientoInventarioDetalle.ParteID
	,Parte.NumeroParte
	,Parte.NombreParte
	,MovimientoInventarioDetalle.Cantidad
	,MovimientoInventarioDetalle.PrecioUnitario
	,MovimientoInventarioDetalle.Importe
	,MovimientoInventarioDetalle.FueDevolucion
	,MovimientoInventarioDetalle.FechaRegistro
	,MovimientoInventarioDetalle.CantidadRecibida
	,MovimientoInventarioDetalle.PrecioUnitarioConDescuento
	,Parte.LineaID
FROM 
	MovimientoInventarioDetalle
	INNER JOIN Parte ON Parte.ParteID = MovimientoInventarioDetalle.ParteID
WHERE
	MovimientoInventarioDetalle.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

-- DROP PROCEDURE pauCuadroDeControlGeneral
CREATE PROCEDURE pauCuadroDeControlGeneral(
	@SucursalID INT = NULL
	, @Pagadas BIT
	, @Cobradas BIT
	, @Solo9500 BIT
	, @Desde DATE = NULL
	, @Hasta DATE = NULL
) AS BEGIN
	SET NOCOUNT ON
	
	/* EXEC pauCuadroDeControlGeneral 1, 1, 1, 0, '2014-01-01', '2014-07-30'
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
	
	-- Variables calculadas para el proceso
	-- DECLARE @DiaCero DATE = CONVERT(DATE, (CONVERT(VARCHAR(4), @AnioAnt) + '-12-31'))
	SET @Hasta = DATEADD(d, 1, @Hasta)
	DECLARE @DesdeAnt DATE = DATEADD(YEAR, -1, @Desde)
	DECLARE @HastaAnt DATE = DATEADD(YEAR, -1, @Hasta)
	
	-- Consulta
	SELECT
		v.VentaID
		, v.Folio
		, v.Fecha
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
		, SUM(CASE WHEN (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			THEN ((vd.PrecioUnitario - vd.CostoConDescuento) * vd.Cantidad)
			ELSE 0
			END) AS Actual
		, SUM(CASE WHEN (v.Fecha >= @DesdeAnt AND v.Fecha < @HastaAnt)
			THEN ((vd.PrecioUnitario - vd.CostoConDescuento) * vd.Cantidad)
			ELSE 0
			END) AS Anterior
	FROM
		Venta v
		LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = v.SucursalID AND s.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
	WHERE
		v.Estatus = 1
		AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
		AND (
			(@Pagadas = 1 AND v.VentaEstatusID = @EstPagadaID)
			OR (@Cobradas = 1 AND v.VentaEstatusID = @EstCobradaID)
		)
		AND (@Solo9500 = 0 OR c9.Cotizacion9500ID IS NOT NULL)
		AND (
			(v.Fecha >= @Desde AND v.Fecha < @Hasta)
			OR (v.Fecha >= @DesdeAnt AND v.Fecha < @HastaAnt)
		)
	GROUP BY
		v.VentaID
		, v.Folio
		, v.Fecha
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
		DATEPART(DAYOFYEAR, v.Fecha)
		, DATEPART(HOUR, v.Fecha)

/*
	-- Por día
	IF @Opcion = @OpPorDia BEGIN
		SELECT
			DATEPART(DAYOFYEAR, v.Fecha) AS Grupo
			, DATEADD(DAY, DATEPART(DAYOFYEAR, v.Fecha), @DiaCero) AS FechaDia
			, SUM(CASE WHEN DATEPART(YEAR, v.Fecha) = @Anio
				THEN ((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) ELSE 0 END) AS AnioActual
			, SUM(CASE WHEN DATEPART(YEAR, v.Fecha) = @AnioAnt
				THEN ((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) ELSE 0 END) AS AnioAnterior
		FROM
			Venta v
			LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		WHERE
			v.Estatus = 1
			AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
			AND (
				(@Pagadas = 1 AND v.VentaEstatusID = @EstPagadaID)
				OR (@Cobradas = 1 AND v.VentaEstatusID = @EstCobradaID)
			)
			AND DATEPART(YEAR, v.Fecha) IN (@Anio, @AnioAnt)
		GROUP BY DATEPART(DAYOFYEAR, v.Fecha)
		ORDER BY Grupo
	END

	IF @Opcion = @OpPorDiaSem BEGIN
		SELECT
			DATEPART(WEEKDAY, v.Fecha) AS Grupo
			, NULL AS FechaDia
			, SUM(CASE WHEN DATEPART(YEAR, v.Fecha) = @Anio
				THEN ((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) ELSE 0 END) AS AnioActual
			, SUM(CASE WHEN DATEPART(YEAR, v.Fecha) = @AnioAnt
				THEN ((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) ELSE 0 END) AS AnioAnterior
		FROM
			Venta v
			LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		WHERE
			v.Estatus = 1
			AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
			AND (
				(@Pagadas = 1 AND v.VentaEstatusID = @EstPagadaID)
				OR (@Cobradas = 1 AND v.VentaEstatusID = @EstCobradaID)
			)
			AND (
				(v.Fecha >= @Desde AND v.Fecha < @Hasta)
				OR (v.Fecha >= @DesdeAnt AND v.Fecha < @HastaAnt)
			)
		GROUP BY DATEPART(WEEKDAY, v.Fecha)
		ORDER BY Grupo
		
	END
	
	IF @Opcion = @OpPorHora BEGIN
		SELECT
			DATEPART(HOUR, v.Fecha) AS Grupo
			, NULL AS FechaDia
			, SUM(CASE WHEN DATEPART(YEAR, v.Fecha) = @Anio
				THEN ((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) ELSE 0 END) AS AnioActual
			, SUM(CASE WHEN DATEPART(YEAR, v.Fecha) = @AnioAnt
				THEN ((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) ELSE 0 END) AS AnioAnterior
		FROM
			Venta v
			LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		WHERE
			v.Estatus = 1
			AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
			AND (
				(@Pagadas = 1 AND v.VentaEstatusID = @EstPagadaID)
				OR (@Cobradas = 1 AND v.VentaEstatusID = @EstCobradaID)
			)
			AND (
				(v.Fecha >= @Desde AND v.Fecha < @Hasta)
				OR (v.Fecha >= @DesdeAnt AND v.Fecha < @HastaAnt)
			)
		GROUP BY DATEPART(HOUR, v.Fecha)
		ORDER BY Grupo
		
	END
*/

END
GO

-- DROP PROCEDURE pauCuadroDeControlCompras
CREATE PROCEDURE pauCuadroDeControlCompras(
	@Desde DATE = NULL
	, @Hasta DATE = NULL
) AS BEGIN
	SET NOCOUNT ON
	
	/* EXEC pauCuadroDeControlCompras '2014-01-01', '2014-07-30'
	DECLARE @Desde DATE = '2014-06-01'
	DECLARE @Hasta DATE = '2014-06-30'
	*/

	-- Definición de variables tipo constante
	DECLARE @TipoCompra INT = 1

	-- Variables calculadas para el proceso
	-- DECLARE @DiaCero DATE = CONVERT(DATE, (CONVERT(VARCHAR(4), @AnioAnt) + '-12-31'))
	SET @Hasta = DATEADD(d, 1, @Hasta)
	DECLARE @DesdeAnt DATE = DATEADD(YEAR, -1, @Desde)
	DECLARE @HastaAnt DATE = DATEADD(YEAR, -1, @Hasta)
	
	-- Consulta
	SELECT
		mi.MovimientoInventarioID
		, mi.FolioFactura
		, mi.FechaRegistro AS Fecha
		, mi.UsuarioID
		, u.NombreUsuario AS Usuario
		, l.LineaID
		, l.NombreLinea AS Linea
		, pv.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, SUM(CASE WHEN (mi.FechaRegistro >= @Desde AND mi.FechaRegistro < @Hasta)
			THEN (mid.PrecioUnitarioConDescuento * mid.Cantidad)
			ELSE 0
			END) AS Actual
		, SUM(CASE WHEN (mi.FechaRegistro >= @DesdeAnt AND mi.FechaRegistro < @HastaAnt)
			THEN (mid.PrecioUnitarioConDescuento * mid.Cantidad)
			ELSE 0
			END) AS Anterior
	FROM
		MovimientoInventario mi
		LEFT JOIN MovimientoInventarioDetalle mid ON mid.MovimientoInventarioID = mi.MovimientoInventarioID AND mid.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = mi.UsuarioID AND u.Estatus = 1
		LEFT JOIN Parte p ON p.ParteID = mid.ParteID AND p.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = mi.ProveedorID AND pv.Estatus = 1
	WHERE
		mi.Estatus = 1
		AND mi.TipoOperacionID = @TipoCompra
		AND (
			(mi.FechaRegistro >= @Desde AND mi.FechaRegistro < @Hasta)
			OR (mi.FechaRegistro >= @DesdeAnt AND mi.FechaRegistro < @HastaAnt)
		)
	GROUP BY
		mi.MovimientoInventarioID
		, mi.FolioFactura
		, mi.FechaRegistro
		, mi.UsuarioID
		, u.NombreUsuario
		, l.LineaID
		, l.NombreLinea
		, pv.ProveedorID
		, pv.NombreProveedor
	ORDER BY
		DATEPART(DAYOFYEAR, mi.FechaRegistro)
		, DATEPART(HOUR, mi.FechaRegistro)

END
GO

-- DROP PROCEDURE pauCuadroDeControlCancelaciones
CREATE PROCEDURE pauCuadroDeControlCancelaciones(
	@SucursalID INT = NULL
	, @Pagadas BIT
	, @Cobradas BIT
	, @Solo9500 BIT
	, @Desde DATE = NULL
	, @Hasta DATE = NULL
) AS BEGIN
	SET NOCOUNT ON
	
	/* EXEC pauCuadroDeControlCancelaciones 1, 1, 1, 0, '2014-01-01', '2014-07-30'
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
		, SUM(CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta)
			THEN ((vdd.PrecioUnitario - vdd.CostoConDescuento) * vdd.Cantidad)
			ELSE 0
			END) AS Actual
		, SUM(CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt)
			THEN ((vdd.PrecioUnitario - vdd.CostoConDescuento) * vdd.Cantidad)
			ELSE 0
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
		LEFT JOIN Parte p ON p.ParteID = vdd.ParteID AND p.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
	WHERE
		vd.Estatus = 1
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
	GROUP BY
		vd.VentaDevolucionID
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

ALTER PROCEDURE pauComisionesAgrupado (
	@Desde DATE = NULL
	, @Hasta DATE = NULL
) AS BEGIN
	SET NOCOUNT ON

	/* 
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
				SUM(((
					CASE com.ListaDePrecios
						WHEN 1 THEN (pp.PrecioUno / @IvaMul)
						WHEN 2 THEN (pp.PrecioDos / @IvaMul)
						WHEN 3 THEN (pp.PrecioTres / @IvaMul)
						WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
						WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
					END
					- vd.Costo) * vd.Cantidad)
					* CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE @PorComision9500 END)
			ELSE
				SUM(((
					CASE WHEN vd.PrecioUnitario > (pp.PrecioUno / @IvaMul) THEN (pp.PrecioUno / @IvaMul) ELSE vd.PrecioUnitario END
					- vd.Costo) * vd.Cantidad)
				* CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE @PorComision9500 END)
			END
			/* +  -- Se agrega lo cancelado
			ISNULL(CASE WHEN v.ComisionistaClienteID > 0 THEN
				SUM(((
					CASE com.ListaDePrecios
						WHEN 1 THEN (pp.PrecioUno / @IvaMul)
						WHEN 2 THEN (pp.PrecioDos / @IvaMul)
						WHEN 3 THEN (pp.PrecioTres / @IvaMul)
						WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
						WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
					END
					- vdvd.Costo) * vdvd.Cantidad)
					* CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE @PorComision9500 END)
			ELSE
				SUM(((
					CASE WHEN vdvd.PrecioUnitario > (pp.PrecioUno / @IvaMul) THEN (pp.PrecioUno / @IvaMul) ELSE vdvd.PrecioUnitario END
					- vdvd.Costo) * vdvd.Cantidad)
				* CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE @PorComision9500 END)
			END, 0)
			*/
		) AS Comision
		, 'V' AS Tipo
	FROM
		-- VentaPago vp
		(
			SELECT DISTINCT VentaID FROM VentaPago vp
			WHERE vp.Estatus = 1 AND (vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
		) vp
		-- INNER JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
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
		-- AND vpd.Importe > 0
		-- AND
		 v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)
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
				SUM(((
					CASE com.ListaDePrecios
						WHEN 1 THEN (pp.PrecioUno / @IvaMul)
						WHEN 2 THEN (pp.PrecioDos / @IvaMul)
						WHEN 3 THEN (pp.PrecioTres / @IvaMul)
						WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
						WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
					END
					- vdd.Costo) * vdd.Cantidad)
					* CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE @PorComision9500 END)
			ELSE
				SUM(((
					CASE WHEN vdd.PrecioUnitario > (pp.PrecioUno / @IvaMul) THEN (pp.PrecioUno / @IvaMul) ELSE vdd.PrecioUnitario END
					- vdd.Costo) * vdd.Cantidad)
				* CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE @PorComision9500 END)
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
		AND p.AplicaComision = 1
	GROUP BY
		vd.VentaID
		, v.SucursalID
		, v.RealizoUsuarioID
		, v.ComisionistaClienteID
		, c9.VentaID

END
GO