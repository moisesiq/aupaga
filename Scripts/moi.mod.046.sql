/* Script con modificaciones a la base de datos de Theos. Archivo 046
 * Creado: 2015/11/03
 * Subido: 2015/11/10
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE CorteDetalleHistorico ALTER COLUMN Concepto NVARCHAR(512) NOT NULL

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

ALTER VIEW ProveedoresPagosView AS
	SELECT
		ppd.ProveedorPolizaID
		, pp.FechaPago
		, ppd.MovimientoInventarioID
		, mi.ProveedorID
		, mi.FolioFactura AS Factura
		, mi.FechaRecepcion AS FechaFactura
		, mi.ImporteFactura
		, mi.ImporteTotal
		, (SELECT SUM(Importe) FROM ProveedorPolizaDetalle WHERE MovimientoInventarioID = ppd.MovimientoInventarioID)
			AS Abonadototal
		, ISNULL(SUM(ppd.Importe), 0.0) AS Abonado
		, SUM(CASE WHEN ppd.OrigenID IS NULL OR ppd.OrigenID = 1 THEN 0.0 ELSE ppd.Importe END) AS Descuento
		/* , (
			(mi.ImporteFactura - ISNULL(SUM(ppd.Importe), 0.0))
			- SUM(CASE WHEN ppd.OrigenID IS NULL OR ppd.OrigenID = 1 THEN 0.0 ELSE ppd.Importe END)
		) AS Final
		*/
	FROM
		ProveedorPolizaDetalle ppd
		LEFT JOIN ProveedorPoliza pp ON pp.ProveedorPolizaID = ppd.ProveedorPolizaID AND pp.Estatus = 1
		LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = ppd.MovimientoInventarioID AND mi.Estatus = 1
	WHERE ppd.Estatus = 1
	GROUP BY
		ppd.ProveedorPolizaID
		, pp.FechaPago
		, ppd.MovimientoInventarioID
		, mi.ProveedorID
		, mi.FolioFactura
		, mi.FechaRecepcion
		, mi.ImporteFactura
		, mi.ImporteTotal
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

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

	/* EXEC pauCuadroDeControlGeneral 1, 1, 0, 0, 0, '2015-11-03', '2015-11-03'
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
		-- Se agrega el precio sin iva, para el estado de resultados
		, SUM(CASE WHEN (vpc.Fecha >= @Desde AND vpc.Fecha < @Hasta) THEN (vd.PrecioUnitario * vd.Cantidad)
			ELSE 0.0 END) AS PrecioSinIvaActual
		-- Precio con Iva
		, SUM(CASE WHEN (vpc.Fecha >= @Desde AND vpc.Fecha < @Hasta)
			THEN ((vd.PrecioUnitario + vd.Iva) * vd.Cantidad)
			ELSE 0.0 END) AS PrecioActual
		, SUM(CASE WHEN (vpc.Fecha >= @DesdeAnt AND vpc.Fecha < @HastaAnt)
			THEN ((vd.PrecioUnitario + vd.Iva) * vd.Cantidad)
			ELSE 0.0 END) AS PrecioAnterior
		-- 
		, SUM(CASE WHEN (vpc.Fecha >= @Desde AND vpc.Fecha < @Hasta)
			THEN (vd.Costo * vd.Cantidad) ELSE 0.0 END) AS CostoActual
		, SUM(CASE WHEN (vpc.Fecha >= @DesdeAnt AND vpc.Fecha < @HastaAnt)
			THEN (vd.Costo * vd.Cantidad) ELSE 0.0 END) AS CostoAnterior
		, SUM(CASE WHEN (vpc.Fecha >= @Desde AND vpc.Fecha < @Hasta)
			THEN (vd.CostoConDescuento * vd.Cantidad) ELSE 0.0 END) AS CostoDescActual
		, SUM(CASE WHEN (vpc.Fecha >= @DesdeAnt AND vpc.Fecha < @HastaAnt)
			THEN (vd.CostoConDescuento * vd.Cantidad) ELSE 0.0 END) AS CostoDescAnterior
		, SUM(CASE WHEN (vpc.Fecha >= @Desde AND vpc.Fecha < @Hasta) THEN 1 ELSE 0 END) AS ProductosActual
		, SUM(CASE WHEN (vpc.Fecha >= @DesdeAnt AND vpc.Fecha < @HastaAnt) THEN 1 ELSE 0 END) AS ProductosAnterior
		, CONVERT(BIT, CASE WHEN (vpc.Fecha >= @Desde AND vpc.Fecha < @Hasta) THEN 1 ELSE 0 END) AS EsActual
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
		-- Se agrega el precio sin iva, para el estado de resultados
		, (SUM(CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta) THEN (vdd.PrecioUnitario * vdd.Cantidad)
			ELSE 0.0 END) * -1) AS PrecioSinIvaActual
		-- Precio con Iva
		, (SUM(CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta)
			THEN ((vdd.PrecioUnitario + vdd.Iva) * vdd.Cantidad)
			ELSE 0.0 END) * -1) AS PrecioActual
		, (SUM(CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt)
			THEN ((vdd.PrecioUnitario + vdd.Iva) * vdd.Cantidad)
			ELSE 0.0 END) * -1) AS PrecioAnterior
		--
		, (SUM(CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta) 
			THEN (vdd.Costo * vdd.Cantidad) ELSE 0.0 END) * -1) AS CostoActual
		, (SUM(CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt)
			THEN (vdd.Costo * vdd.Cantidad) ELSE 0.0 END) * -1) AS CostoAnterior
		, (SUM(CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta)
			THEN (vdd.CostoConDescuento * vdd.Cantidad) ELSE 0.0 END) * -1) AS CostoDescActual
		, (SUM(CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt)
			THEN (vdd.CostoConDescuento * vdd.Cantidad) ELSE 0.0 END) * -1) AS CostoDescAnterior
		, (SUM(CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta) THEN 1 ELSE 0 END) * -1) AS ProductosActual
		, (SUM(CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt) THEN 1 ELSE 0 END) * -1) AS ProductosAnterior
		, CONVERT(BIT, CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta) THEN 1 ELSE 0 END) AS EsActual
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

-- DROP PROCEDURE pauContaCuentaAuxiliarPolizas
CREATE PROCEDURE pauContaCuentaAuxiliarPolizas (
	@CuentaAuxiliarID INT
	, @Desde DATE
	, @Hasta DATE
	, @SucursalID INT = NULL
) AS BEGIN
	SET NOCOUNT ON

	SET @Hasta = DATEADD(DAY, 1, @Hasta)

	SELECT
		cpd.ContaPolizaDetalleID
		, cp.Fecha
		, cpd.ContaPolizaID
		, cpd.Referencia
		, cpd.Cargo
		, cpd.Abono
		, s.NombreSucursal AS Sucursal
		, cp.Concepto
		, cp.FueManual
		, cp.Error
	FROM
		ContaPolizaDetalle cpd
		INNER JOIN ContaPoliza cp ON cp.ContaPolizaID = cpd.ContaPolizaID
		LEFT JOIN Sucursal s ON s.SucursalID = cpd.SucursalID AND s.Estatus = 1
	WHERE
		cpd.ContaCuentaAuxiliarID = @CuentaAuxiliarID
		AND (cp.Fecha >= @Desde AND cp.Fecha < @Hasta)
		AND (@SucursalID IS NULL OR cpd.SucursalID = @SucursalID)
	ORDER BY cp.Fecha

END