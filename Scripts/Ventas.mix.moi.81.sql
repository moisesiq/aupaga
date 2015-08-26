/* Script con modificaciones para el módulo de ventas. Archivo 81
 * Creado: 2015/01/13
 * Subido: 2015/01/14
 */


----------------------------------------------------------------------------------- Código de André

----------------------------------------------------------------------------------- Código de André


/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	-- DROP TABLE CajaCambioDeTurno
	CREATE TABLE CajaCambioDeTurno (
		CajaCambioDeTurnoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		, Fecha DATETIME NOT NULL
		, EntregaUsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, RecibeUsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, Total DECIMAL(12, 2) NOT NULL
		, Conteo DECIMAL(12, 2) NOT NULL
	)

	--
	ALTER TABLE ParteCodigoAlterno ALTER COLUMN CodigoAlterno NVARCHAR(32) NOT NULL
	CREATE INDEX Ix_CodigoAlterno ON ParteCodigoAlterno(CodigoAlterno)

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

ALTER VIEW [dbo].[MovimientoInventarioDetalleView] AS
	SELECT
		MovimientoInventarioDetalle.MovimientoInventarioDetalleID
		,MovimientoInventarioDetalle.MovimientoInventarioID
		, mi.TipoOperacionID
		, mi.FechaRecepcion
		, mi.SucursalDestinoID
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
		LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = MovimientoInventarioDetalle.MovimientoInventarioID
			AND mi.Estatus = 1
		INNER JOIN Parte ON Parte.ParteID = MovimientoInventarioDetalle.ParteID
	WHERE
		MovimientoInventarioDetalle.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

ALTER PROCEDURE [dbo].[pauCuadroDeControlGeneral] (
	@SucursalID INT = NULL
	, @Pagadas BIT
	, @Cobradas BIT
	, @Solo9500 BIT
	, @OmitirDomingo BIT
	-- , @CostoConDescuento BIT
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
		, ISNULL(vpc.Fecha, '') AS Fecha
		, v.SucursalID
		, s.NombreSucursal AS Sucursal
		, v.ClienteID
		, c.Nombre AS Cliente
		, v.RealizoUsuarioID AS VendedorID
		, u.NombreUsuario AS Vendedor
		, l.LineaID
		, l.NombreLinea AS Linea
		, mp.MarcaParteID AS MarcaID
		, mp.NombreMarcaParte AS Marca
		, pv.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, SUM(CASE WHEN (vpc.Fecha >= @Desde AND vpc.Fecha < @Hasta) THEN
			((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) ELSE 0.0 END) AS Actual
		, SUM(CASE WHEN (vpc.Fecha >= @DesdeAnt AND vpc.Fecha < @HastaAnt) THEN
			((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) ELSE 0.0	END) AS Anterior
		, SUM(CASE WHEN (vpc.Fecha >= @Desde AND vpc.Fecha < @Hasta) THEN
			((vd.PrecioUnitario - vd.CostoConDescuento) * vd.Cantidad) ELSE 0.0 END) AS UtilDescActual
		, SUM(CASE WHEN (vpc.Fecha >= @DesdeAnt AND vpc.Fecha < @HastaAnt) THEN
			((vd.PrecioUnitario - vd.CostoConDescuento) * vd.Cantidad) ELSE 0.0 END) AS UtilDescAnterior
		, SUM(CASE WHEN (vpc.Fecha >= @Desde AND vpc.Fecha < @Hasta) THEN (vd.PrecioUnitario + vd.Iva)
			ELSE 0.0 END) AS PrecioActual
		, SUM(CASE WHEN (vpc.Fecha >= @DesdeAnt AND vpc.Fecha < @HastaAnt) THEN (vd.PrecioUnitario + vd.Iva)
			ELSE 0.0 END) AS PrecioAnterior
		, SUM(CASE WHEN (vpc.Fecha >= @Desde AND vpc.Fecha < @Hasta) THEN vd.Costo ELSE 0.0 END) AS CostoActual
		, SUM(CASE WHEN (vpc.Fecha >= @DesdeAnt AND vpc.Fecha < @HastaAnt) THEN vd.Costo ELSE 0.0 END) AS CostoAnterior
		, SUM(CASE WHEN (vpc.Fecha >= @Desde AND vpc.Fecha < @Hasta) THEN vd.CostoConDescuento ELSE 0.0 END) AS CostoDescActual
		, SUM(CASE WHEN (vpc.Fecha >= @DesdeAnt AND vpc.Fecha < @HastaAnt) THEN vd.CostoConDescuento ELSE 0.0 END) AS CostoDescAnterior
		, SUM(CASE WHEN (vpc.Fecha >= @Desde AND vpc.Fecha < @Hasta) THEN 1 ELSE 0 END) AS VentasActual
		, SUM(CASE WHEN (vpc.Fecha >= @DesdeAnt AND vpc.Fecha < @HastaAnt) THEN 1 ELSE 0 END) AS VentasAnterior
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
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
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
		, mp.MarcaParteID
		, mp.NombreMarcaParte
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
		, mp.MarcaParteID AS MarcaID
		, mp.NombreMarcaParte AS Marca
		, pv.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, (SUM(CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta) THEN
			((vdd.PrecioUnitario - vdd.Costo) * vdd.Cantidad) ELSE 0.0 END) * -1) AS Actual
		, (SUM(CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt) THEN
			((vdd.PrecioUnitario - vdd.Costo) * vdd.Cantidad) ELSE 0.0 END) * -1) AS Anterior
		, (SUM(CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta) THEN
			((vdd.PrecioUnitario - vdd.CostoConDescuento) * vdd.Cantidad) ELSE 0.0 END) * -1) AS UtilDescActual
		, (SUM(CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt) THEN
			((vdd.PrecioUnitario - vdd.CostoConDescuento) * vdd.Cantidad) ELSE 0.0 END) * -1) AS UtilDescAnterior
		, (SUM(CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta) THEN (vdd.PrecioUnitario + vdd.Iva)
			ELSE 0.0 END) * -1) AS PrecioActual
		, (SUM(CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt) THEN (vdd.PrecioUnitario + vdd.Iva)
			ELSE 0.0 END) * -1) AS PrecioAnterior
		, (SUM(CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta) THEN vdd.Costo ELSE 0.0 END) * -1) AS CostoActual
		, (SUM(CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt) THEN vdd.Costo ELSE 0.0 END) * -1) AS CostoAnterior
		, (SUM(CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta) THEN vdd.CostoConDescuento ELSE 0.0 END) * -1) AS CostoDescActual
		, (SUM(CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt) THEN vdd.CostoConDescuento ELSE 0.0 END) * -1) AS CostoDescAnterior
		, (SUM(CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta) THEN 1 ELSE 0 END) * -1) AS VentasActual
		, (SUM(CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt) THEN 1 ELSE 0 END) * -1) AS VentasAnterior
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
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
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
		, mp.MarcaParteID
		, mp.NombreMarcaParte
		, pv.ProveedorID
		, pv.NombreProveedor

	/* ORDER BY
		DATEPART(DAYOFYEAR, vp.Fecha)
		, DATEPART(HOUR, vp.Fecha)
	*/

END
GO

ALTER PROCEDURE [dbo].[pauPartesMovimientosDevoluciones] (
	@ProveedorID INT
	, @Desde DATE
	, @Hasta DATE
	, @Garantias BIT = 0
	, @Codigo NVARCHAR(32) = NULL
	, @Descripcion1 NVARCHAR(32) = NULL
	, @Descripcion2 NVARCHAR(32) = NULL
	, @Descripcion3 NVARCHAR(32) = NULL
	, @Descripcion4 NVARCHAR(32) = NULL
	, @Descripcion5 NVARCHAR(32) = NULL
	, @Descripcion6 NVARCHAR(32) = NULL
	, @Descripcion7 NVARCHAR(32) = NULL
	, @Descripcion8 NVARCHAR(32) = NULL
	, @Descripcion9 NVARCHAR(32) = NULL
) AS BEGIN
	SET NOCOUNT ON

	DECLARE @OpCompra INT = 1
	DECLARE @EstGenRecibido INT = 9

	SET @Hasta = DATEADD(DAY, 1, @Hasta)

	IF @Garantias = 1 BEGIN
	
		-- Búsqueda por descripción
		IF @Codigo IS NULL BEGIN
			SELECT
				vg.VentaGarantiaID AS Id
				, v.Folio
				, vg.ParteID
				, p.NumeroParte AS NumeroDeParte
				, p.NombreParte AS Descripcion
				, pp.Costo
				, mp.NombreMarcaParte AS Marca
				, l.NombreLinea AS Linea
			FROM
				VentaGarantia vg
				INNER JOIN Parte p ON p.ParteID = vg.ParteID AND p.Estatus = 1
				LEFT JOIN PartePrecio pp ON pp.ParteID = vg.ParteID AND pp.Estatus = 1
				LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
				LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
				LEFT JOIN Venta v ON v.VentaID = vg.VentaID AND v.Estatus = 1
			WHERE
				vg.Estatus = 1
				AND vg.EstatusGenericoID = @EstGenRecibido
				AND p.ProveedorID = @ProveedorID
				AND (			
					(@Descripcion1 IS NULL OR p.NombreParte LIKE '%' + @Descripcion1 + '%')
					AND (@Descripcion2 IS NULL OR p.NombreParte LIKE '%' + @Descripcion2 + '%')
					AND (@Descripcion3 IS NULL OR p.NombreParte LIKE '%' + @Descripcion3 + '%')
					AND (@Descripcion4 IS NULL OR p.NombreParte LIKE '%' + @Descripcion4 + '%')
					AND (@Descripcion5 IS NULL OR p.NombreParte LIKE '%' + @Descripcion5 + '%')
					AND (@Descripcion6 IS NULL OR p.NombreParte LIKE '%' + @Descripcion6 + '%')
					AND (@Descripcion7 IS NULL OR p.NombreParte LIKE '%' + @Descripcion7 + '%')
					AND (@Descripcion8 IS NULL OR p.NombreParte LIKE '%' + @Descripcion8 + '%')
					AND (@Descripcion9 IS NULL OR p.NombreParte LIKE '%' + @Descripcion9 + '%')
				)
		
		-- Búsqueda por código			
		END ELSE BEGIN
			SELECT DISTINCT
				vg.VentaGarantiaID AS Id
				, v.Folio
				, vg.ParteID
				, p.NumeroParte AS NumeroDeParte
				, p.NombreParte AS Descripcion
				, pp.Costo
				, mp.NombreMarcaParte AS Marca
				, l.NombreLinea AS Linea
			FROM
				VentaGarantia vg
				INNER JOIN Parte p ON p.ParteID = vg.ParteID AND p.Estatus = 1
				LEFT JOIN PartePrecio pp ON pp.ParteID = vg.ParteID AND pp.Estatus = 1
				LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
				LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
				LEFT JOIN Venta v ON v.VentaID = vg.VentaID AND v.Estatus = 1
			WHERE
				vg.Estatus = 1
				AND vg.EstatusGenericoID = @EstGenRecibido
				AND p.ProveedorID = @ProveedorID
				AND p.NumeroParte = @Codigo
		END
	
	END ELSE BEGIN

		-- Búsqueda por descripción
		IF @Codigo IS NULL BEGIN
			SELECT DISTINCT
				0 AS Id
				, '' AS Folio
				, mid.ParteID
				, p.NumeroParte AS NumeroDeParte
				, p.NombreParte AS Descripcion
				, pp.Costo
				, mp.NombreMarcaParte AS Marca
				, l.NombreLinea AS Linea
			FROM
				MovimientoInventarioDetalle mid
				INNER JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = mid.MovimientoInventarioID AND mi.Estatus = 1
				INNER JOIN Parte p ON p.ParteID = mid.ParteID AND p.Estatus = 1
				LEFT JOIN PartePrecio pp ON pp.ParteID = mid.ParteID AND pp.Estatus = 1
				LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
				LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
			WHERE
				mid.Estatus = 1
				AND mi.TipoOperacionID = @OpCompra
				AND mi.ProveedorID = @ProveedorID
				AND (mi.FechaFactura >= @Desde AND mi.FechaFactura < @Hasta)
				AND (mid.Cantidad - mid.CantidadDevuelta) > 0
				AND (			
					(@Descripcion1 IS NULL OR p.NombreParte LIKE '%' + @Descripcion1 + '%')
					AND (@Descripcion2 IS NULL OR p.NombreParte LIKE '%' + @Descripcion2 + '%')
					AND (@Descripcion3 IS NULL OR p.NombreParte LIKE '%' + @Descripcion3 + '%')
					AND (@Descripcion4 IS NULL OR p.NombreParte LIKE '%' + @Descripcion4 + '%')
					AND (@Descripcion5 IS NULL OR p.NombreParte LIKE '%' + @Descripcion5 + '%')
					AND (@Descripcion6 IS NULL OR p.NombreParte LIKE '%' + @Descripcion6 + '%')
					AND (@Descripcion7 IS NULL OR p.NombreParte LIKE '%' + @Descripcion7 + '%')
					AND (@Descripcion8 IS NULL OR p.NombreParte LIKE '%' + @Descripcion8 + '%')
					AND (@Descripcion9 IS NULL OR p.NombreParte LIKE '%' + @Descripcion9 + '%')
				)
		
		-- Búsqueda por código			
		END ELSE BEGIN
			SELECT DISTINCT
				0 AS Id
				, '' AS Folio
				, mid.ParteID
				, p.NumeroParte AS NumeroDeParte
				, p.NombreParte AS Descripcion
				, pp.Costo
				, mp.NombreMarcaParte AS Marca
				, l.NombreLinea AS Linea
			FROM
				MovimientoInventarioDetalle mid
				INNER JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = mid.MovimientoInventarioID AND mi.Estatus = 1
				INNER JOIN Parte p ON p.ParteID = mid.ParteID AND p.Estatus = 1
				LEFT JOIN PartePrecio pp ON pp.ParteID = mid.ParteID AND pp.Estatus = 1
				LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
				LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
			WHERE
				mid.Estatus = 1
				AND mi.TipoOperacionID = @OpCompra
				AND mi.ProveedorID = @ProveedorID
				AND (mi.FechaFactura >= @Desde AND mi.FechaFactura < @Hasta)
				AND (mid.Cantidad - mid.CantidadDevuelta) > 0
				AND p.NumeroParte = @Codigo
		END
	END
	
END
GO