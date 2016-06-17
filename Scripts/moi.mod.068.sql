/* Script con modificaciones a la base de datos de Theos. Archivo 068
 * Creado: 2016/06/02
 * Subido: 2016/06/17
 */

DECLARE @ScriptID INT = 68
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)
-- SELECT * FROM ScriptSql ORDER BY ScriptID DESC

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE Venta ADD Vencimiento DATETIME NULL
GO
UPDATE Venta SET Vencimiento = DATEADD(DAY, c.DiasDeCredito, v.Fecha)
FROM
	Venta v
	LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
WHERE
	v.Estatus = 1
	AND v.ACredito = 1

-- DROP TABLE VentaPagoConTarjeta
CREATE TABLE VentaPagoConTarjeta (
	VentaPagoConTarjetaID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, BancoCuentaID INT NOT NULL FOREIGN KEY REFERENCES BancoCuenta(BancoCuentaID)
	, VentaPagoDetalleID INT NOT NULL FOREIGN KEY REFERENCES VentaPagoDetalle(VentaPagoDetalleID)
	, MesesSinIntereses INT NOT NULL
	, Telefono NVARCHAR(10) NULL
)

INSERT INTO BancoCuenta (NumeroDeCuenta, NombreDeCuenta, BancoID, UsoProveedores, UsoClientes, EsCpcp) VALUES
	('', 'SERFIN', 4, 0, 1, 0)

INSERT INTO TipoDescuento (NombreTipoDescuento, UsuarioID, FechaRegistro) VALUES
	('INDIVIDUAL A FACTURA', 1, GETDATE())

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO

ALTER VIEW [dbo].[PartesExistenciasView] AS
	SELECT		ISNULL(ROW_NUMBER() OVER (ORDER BY p.ParteID, pe.SucursalID), 0) AS Registro		, pe.ParteExistenciaID		, p.ParteID		, p.NumeroParte AS NumeroDeParte		, p.NombreParte AS Descripcion		, p.ProveedorID		, pv.NombreProveedor AS Proveedor		, p.MarcaParteID AS MarcaID		, mp.NombreMarcaParte AS Marca		, p.LineaID		, l.NombreLinea AS Linea		, pp.Costo		, pp.CostoConDescuento		, pe.SucursalID		, pe.Existencia		-- , SUM(pe.Existencia) AS Existencia		, MAX(vd.FechaRegistro) AS UltimaVenta		-- , MAX(mi.FechaRecepcion) AS UltimaCompra		, mov.FechaRecepcion AS UltimaCompra		, mov.FolioFactura	FROM		Parte p		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1		LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.Estatus = 1		LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND p.Estatus = 1		LEFT JOIN VentaDetalle vd ON vd.ParteID = p.ParteID AND vd.Estatus = 1		-- LEFT JOIN MovimientoInventarioDetalle mid ON mid.ParteID = p.ParteID AND mid.Estatus = 1		-- LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = mid.MovimientoInventarioID AND mi.TipoOperacionID = 1 AND mi.Estatus = 1		LEFT JOIN (			SELECT				mid.ParteID				, ROW_NUMBER() over (partition by mid.ParteID order by mi.FechaRecepcion DESC) as Registro				, mi.FechaRecepcion				, mi.FolioFactura			FROM				MovimientoInventario mi				LEFT JOIN MovimientoInventarioDetalle mid ON mid.MovimientoInventarioID = mi.MovimientoInventarioID					AND mid.Estatus = 1			WHERE				mi.Estatus = 1				AND mi.TipoOperacionID = 1			GROUP BY				mid.ParteID				, mi.FechaRecepcion				, mi.FolioFactura		) mov ON mov.ParteID = p.ParteID and mov.Registro = 1	WHERE		p.Estatus = 1		and p.MarcaParteID = 33	GROUP BY		pe.ParteExistenciaID		, p.ParteID		, p.NumeroParte		, p.NombreParte		, p.ProveedorID		, pv.NombreProveedor		, p.MarcaParteID		, mp.NombreMarcaParte		, p.LineaID		, l.NombreLinea		, pp.Costo		, pp.CostoConDescuento		, pe.SucursalID		, pe.Existencia		, mov.FechaRecepcion		, mov.FolioFactura
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

CREATE PROCEDURE pauCuadroDeControlClientesCredito (
	@SucursalID INT = NULL
	, @Pagadas BIT
	, @Cobradas BIT
	, @Solo9500 BIT
	, @OmitirDomingo BIT
	, @Desde DATE = NULL
	, @Hasta DATE = NULL
) AS BEGIN
	SET NOCOUNT ON
	
	/*
	DECLARE @SucursalID INT = NULL
	DECLARE @Pagadas BIT = 1
	DECLARE @Cobradas BIT = 1
	DECLARE @Solo9500 BIT = 0
	DECLARE @OmitirDomingo BIT = 0
	DECLARE @Desde DATE = '2016-01-01'
	DECLARE @Hasta DATE = '2016-12-31'
	EXEC pauCuadroDeControlClientesCredito @SucursalID, @Pagadas, @Cobradas, @Solo9500, @OmitirDomingo, @Desde, @Hasta
	*/
	
	-- Definición de variables tipo constante
	DECLARE @EstCobradaID INT = 2
	DECLARE @EstPagadaID INT = 3
	DECLARE @EstCanceladaID INT = 4
	DECLARE @EstCanceladaSinPagoID INT = 5
	
	-- Variables calculadas para el proceso
	DECLARE @Hoy DATE = GETDATE()
	DECLARE @HastaMas1 DATE = DATEADD(D, 1, @Hasta)
	
	;WITH _Tiempos AS (
		SELECT
			v.VentaID
			, v.Fecha
			, v.ClienteID
			, DATEDIFF(DAY, v.Fecha, MAX(vp.Fecha)) AS DiasPago
		FROM
			Venta v
			LEFT JOIN VentaPago vp ON vp.VentaID = v.VentaID AND vp.Estatus = 1
		WHERE
			v.Estatus = 1
			AND v.ACredito = 1
			AND v.VentaEstatusID = @EstPagadaID
			-- AND v.Fecha >= DATEADD(YEAR, -1, GETDATE())
			AND (v.Fecha >= @Desde AND v.Fecha < @HastaMas1)
		GROUP BY
			v.VentaID
			, v.Fecha
			, v.ClienteID
	), _Ventas AS (
		SELECT
			v.VentaID
			, v.ClienteID
			, SUM((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) AS Total
			, vpc.Pagado
			, (SUM((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) - ISNULL(vpc.Pagado, 0.0)) AS Restante
			, CASE WHEN SUM((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) != 0 THEN 
				((SUM((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) - ISNULL(vpc.Pagado, 0.0))
				/ SUM((vd.PrecioUnitario + vd.Iva) * vd.Cantidad))
				ELSE 0.0 END AS Porcentaje
			, SUM(vd.Costo * vd.Cantidad) AS Costo
			, SUM(vd.CostoConDescuento * vd.Cantidad) AS CostoConDescuento
			, SUM((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) AS Utilidad
			, SUM((vd.PrecioUnitario - vd.CostoConDescuento) * vd.Cantidad) AS UtilidadConDescuento
			, COUNT(vd.ParteID) AS Productos
			, CONVERT(BIT, CASE WHEN @Hoy > v.Vencimiento THEN 1 ELSE 0 END) AS Vencida
		FROM
			Venta v
			LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
			LEFT JOIN (
				SELECT vp.VentaID, SUM(vpd.Importe) AS Pagado
				FROM
					VentaPago vp
					LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
				WHERE vp.Estatus = 1
				GROUP BY vp.VentaID
			) vpc ON vpc.VentaID = v.VentaID
			LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.Estatus = 1
		WHERE
			v.Estatus = 1
			AND v.ACredito = 1
			-- AND v.VentaEstatusID = @EstCobradaID
			-- Filtros de parámetros
			AND (v.Fecha >= @Desde AND v.Fecha < @HastaMas1)
			AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
			AND (
				(@Pagadas = 1 AND (v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)))
				OR (@Cobradas = 1 AND (v.VentaEstatusID IN (@EstCobradaID)))
			)
			AND (@Solo9500 = 0 OR c9.Cotizacion9500ID IS NOT NULL)
			-- Se quitan las ventas de los domingos, si aplica
			AND (@OmitirDomingo = 0 OR DATEPART(DW, v.Fecha) != 1)
		GROUP BY
			v.VentaID
			, v.ClienteID
			, v.Fecha
			, v.Vencimiento
			, vpc.Pagado
	)
	SELECT
		c.ClienteID
		, c.Nombre AS Cliente
		, c.DiasDeCredito
		, SUM(ca.Restante) AS Adeudo
		, SUM(CASE WHEN ca.Vencida = 1 THEN ca.Restante ELSE 0.0 END) AS Vencido
		, SUM(ca.Costo * ca.Porcentaje) AS AdeudoCosto
		, SUM(CASE WHEN ca.Vencida = 1 THEN (ca.Costo * ca.Porcentaje) ELSE 0.0 END) AS VencidoCosto
		, SUM(ca.CostoConDescuento * ca.Porcentaje) AS AdeudoCostoConDescuento
		, SUM(CASE WHEN ca.Vencida = 1 THEN (ca.CostoConDescuento * ca.Porcentaje) ELSE 0.0 END) AS VencidoCostoConDescuento
		, SUM(ca.Utilidad * ca.Porcentaje) AS AdeudoUtilidad
		, SUM(CASE WHEN ca.Vencida = 1 THEN (ca.Utilidad * ca.Porcentaje) ELSE 0.0 END) AS VencidoUtilidad
		, SUM(ca.UtilidadConDescuento * ca.Porcentaje) AS AdeudoUtilidadConDescuento
		, SUM(CASE WHEN ca.Vencida = 1 THEN (ca.UtilidadConDescuento * ca.Porcentaje) ELSE 0.0 END) AS VencidoUtilidadConDescuento
		, SUM(ca.Productos * ca.Porcentaje) AS AdeudoProductos
		, SUM(CASE WHEN ca.Vencida = 1 THEN (ca.Productos * ca.Porcentaje) ELSE 0.0 END) AS VencidoProductos
		, COUNT(ca.VentaID) AS AdeudoVentas
		, SUM(CASE WHEN Vencida = 1 THEN 1 ELSE 0 END) AS VencidoVentas
		
		, (SELECT AVG(DiasPago) FROM _Tiempos WHERE ClienteID = c.ClienteID) AS PromedioDePagoAnual
		, (SELECT AVG(DiasPago) FROM _Tiempos WHERE ClienteID = c.ClienteID
			-- AND Fecha > DATEADD(MONTH, -3, GETDATE())) AS PromedioDePago3Meses
			-- Por alguna extraña razón, el filtro se debe de hacer con "Fecha <=" y "NOT IN" en vez de "Fecha >", pues si
			-- se hace de esta segunda manera, la consulta se incrementa como 12 segundos en tiempo de ejecución.
			AND VentaID NOT IN (SELECT VentaID FROM _Tiempos WHERE Fecha <= DATEADD(MONTH, -3, GETDATE())))
			AS PromedioDePago3Meses
	FROM
		Cliente c
		LEFT JOIN _Ventas ca ON ca.ClienteID = c.ClienteID
	WHERE
		c.Estatus = 1
		AND c.TieneCredito = 1
	GROUP BY
		c.ClienteID
		, c.Nombre
		, c.DiasDeCredito

END
GO

CREATE PROCEDURE pauCuadroDeControlCobranza (
	@SucursalID INT = NULL
	, @ClienteID INT = NULL
	, @Pagadas BIT
	, @Cobradas BIT
	, @Solo9500 BIT
	, @OmitirDomingo BIT
	, @Desde DATE = NULL
	, @Hasta DATE = NULL
) AS BEGIN
	SET NOCOUNT ON

	/*
	DECLARE @SucursalID INT = 1
	DECLARE @ClienteID INT = NULL
	DECLARE @Pagadas BIT = 1
	DECLARE @Cobradas BIT = 1
	DECLARE @Solo9500 BIT = 0
	DECLARE @OmitirDomingo BIT = 0
	DECLARE @Desde DATE = '2016-01-01'
	DECLARE @Hasta DATE = '2016-12-31'
	EXEC pauCuadroDeControlCobranza @SucursalID, @ClienteID, @Pagadas, @Cobradas, @Solo9500, @OmitirDomingo, @Desde, @Hasta
	*/

	-- Definición de variables tipo constante
	DECLARE @EstCobradaID INT = 2
	DECLARE @EstPagadaID INT = 3
	DECLARE @EstCanceladaID INT = 4
	DECLARE @EstCanceladaSinPagoID INT = 5
	
	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(D, 1, @Hasta)
	DECLARE @DesdeAnt DATE = DATEADD(YEAR, -1, @Desde)
	DECLARE @HastaAnt DATE = DATEADD(YEAR, -1, @Hasta)
	
	-- Consulta
	SELECT
		v.VentaID
		, v.Vencimiento
		, v.VentaEstatusID
		, v.ClienteID
		, SUM(CASE WHEN (v.Vencimiento >= @Desde AND v.Vencimiento < @Hasta) THEN
			((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) ELSE 0.0 END) AS Esperado
		, SUM(CASE WHEN (v.Vencimiento >= @Desde AND v.Vencimiento < @Hasta) THEN
			 vpc.Pagado ELSE 0.0 END) AS Pagado
		, CASE WHEN (v.Vencimiento >= @Desde AND v.Vencimiento < @Hasta)
			AND SUM((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) != 0
			THEN SUM(vpc.Pagado) / SUM((vd.PrecioUnitario + vd.Iva) * vd.Cantidad)
			ELSE 0.0 END AS Porcentaje
		, SUM(CASE WHEN (v.Vencimiento >= @Desde AND v.Vencimiento < @Hasta) THEN
			((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) ELSE 0.0 END) AS EsperadoUtilidad
		, SUM(CASE WHEN (v.Vencimiento >= @Desde AND v.Vencimiento < @Hasta) THEN
			((vd.PrecioUnitario - vd.CostoConDescuento) * vd.Cantidad) ELSE 0.0 END) AS EsperadoUtilidadConDescuento
		, SUM(CASE WHEN (v.Vencimiento >= @Desde AND v.Vencimiento < @Hasta)
			THEN (vd.Costo * vd.Cantidad) ELSE 0.0 END) AS EsperadoCosto
		, SUM(CASE WHEN (v.Vencimiento >= @Desde AND v.Vencimiento < @Hasta)
			THEN (vd.CostoConDescuento * vd.Cantidad) ELSE 0.0 END) AS EsperadoCostoConDescuento
		, SUM(CASE WHEN (v.Vencimiento >= @Desde AND v.Vencimiento < @Hasta) THEN 1 ELSE 0 END) AS Productos
		-- Se agregan columnas del año anterior
		, SUM(CASE WHEN (v.Vencimiento >= @DesdeAnt AND v.Vencimiento < @HastaAnt) THEN
			((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) ELSE 0.0 END) AS EsperadoAnt
		, SUM(CASE WHEN (v.Vencimiento >= @DesdeAnt AND v.Vencimiento < @HastaAnt) THEN
			 vpc.Pagado ELSE 0.0 END) AS PagadoAnt
		, CASE WHEN (v.Vencimiento >= @DesdeAnt AND v.Vencimiento < @HastaAnt)
			AND SUM((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) != 0
			THEN SUM(vpc.Pagado) / SUM((vd.PrecioUnitario + vd.Iva) * vd.Cantidad)
			ELSE 0.0 END AS PorcentajeAnt

		, SUM(CASE WHEN (v.Vencimiento >= @DesdeAnt AND v.Vencimiento < @HastaAnt) THEN
			((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) ELSE 0.0 END) AS EsperadoUtilidadAnt
		, SUM(CASE WHEN (v.Vencimiento >= @DesdeAnt AND v.Vencimiento < @HastaAnt) THEN
			((vd.PrecioUnitario - vd.CostoConDescuento) * vd.Cantidad) ELSE 0.0 END) AS EsperadoUtilidadConDescuentoAnt
		, SUM(CASE WHEN (v.Vencimiento >= @DesdeAnt AND v.Vencimiento < @HastaAnt)
			THEN (vd.Costo * vd.Cantidad) ELSE 0.0 END) AS EsperadoCostoAnt
		, SUM(CASE WHEN (v.Vencimiento >= @DesdeAnt AND v.Vencimiento < @HastaAnt)
			THEN (vd.CostoConDescuento * vd.Cantidad) ELSE 0.0 END) AS EsperadoCostoConDescuentoAnt
		, SUM(CASE WHEN (v.Vencimiento >= @DesdeAnt AND v.Vencimiento < @HastaAnt) THEN 1 ELSE 0 END) AS ProductosAnt
		--
		, CONVERT(BIT, CASE WHEN (v.Vencimiento >= @Desde AND v.Vencimiento < @Hasta) THEN 1 ELSE 0 END) AS EsActual
	FROM
		Venta v
		LEFT JOIN (
			SELECT vp.VentaID, SUM(vpd.Importe) AS Pagado
			FROM
				VentaPago vp
				LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
			WHERE vp.Estatus = 1
			GROUP BY vp.VentaID
		) vpc ON vpc.VentaID = v.VentaID
		-- LEFT JOIN VentaPago vp ON vp.VentaID = v.VentaID AND vp.Estatus = 1
		-- LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
		LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
	WHERE
		v.Estatus = 1
		AND v.ACredito = 1
		AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
		AND (@ClienteID IS NULL OR v.ClienteID = @ClienteID)
		AND (
			(v.Vencimiento >= @Desde AND v.Vencimiento < @Hasta)
			OR (v.Vencimiento >= @DesdeAnt AND v.Vencimiento < @HastaAnt)
		)
		-- AND vpd.Importe > 0
		AND (
			(@Pagadas = 1 AND (v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)))
			OR (@Cobradas = 1 AND (v.VentaEstatusID IN (@EstCobradaID)))
		)
		AND (@Solo9500 = 0 OR c9.Cotizacion9500ID IS NOT NULL)
		-- Se quitan las ventas de los domingos, si aplica
		AND (@OmitirDomingo = 0 OR DATEPART(DW, v.Fecha) != 1)
	GROUP BY
		v.VentaID
		, v.Vencimiento
		, v.VentaEstatusID
		, v.ClienteID

END
GO

CREATE PROCEDURE pauCuadroDeControlPorAnio (
	@SucursalID INT = NULL
	, @Pagadas BIT
	, @Cobradas BIT
	, @Solo9500 BIT
	, @OmitirDomingo BIT
) AS BEGIN
	SET NOCOUNT ON

	/*
	DECLARE @SucursalID INT = NULL
	DECLARE @Pagadas BIT = 1
	DECLARE @Cobradas BIT = 1
	DECLARE @Solo9500 BIT = 0
	DECLARE @OmitirDomingo BIT = 0
	EXEC pauCuadroDeControlPorAnio @SucursalID, @Pagadas, @Cobradas, @Solo9500, @OmitirDomingo
	*/

	-- Definición de variables tipo constante
	DECLARE @EstCobradaID INT = 2
	DECLARE @EstPagadaID INT = 3
	DECLARE @EstCanceladaID INT = 4
	DECLARE @EstCanceladaSinPagoID INT = 5
	
	-- Consulta
	SELECT
		DATEPART(YEAR, v.Fecha) AS Anio
		, SUM((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) AS Precio
		, SUM(vd.Costo * vd.Cantidad) AS Costo
		, SUM(vd.CostoConDescuento * vd.Cantidad) AS CostoConDescuento
		, SUM((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) AS Utilidad
		, SUM((vd.PrecioUnitario - vd.CostoConDescuento) * vd.Cantidad) AS UtilidadConDescuento
		, COUNT(vd.ParteID) AS Productos
		, COUNT(v.VentaID) As Ventas
	FROM
		Venta v
		LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		LEFT JOIN (
			SELECT vp.VentaID, SUM(vpd.Importe) AS Pagado
			FROM
				VentaPago vp
				LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
			WHERE vp.Estatus = 1
			GROUP BY vp.VentaID
		) vpc ON vpc.VentaID = v.VentaID
		LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.Estatus = 1
	WHERE
		v.Estatus = 1
		AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
		AND (
			(@Pagadas = 1 AND (v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)))
			OR (@Cobradas = 1 AND (v.VentaEstatusID IN (@EstCobradaID)))
		)
		AND (@Solo9500 = 0 OR c9.Cotizacion9500ID IS NOT NULL)
		-- Se quitan las ventas de los domingos, si aplica
		AND (@OmitirDomingo = 0 OR DATEPART(DW, v.Fecha) != 1)
	GROUP BY
		DATEPART(YEAR, v.Fecha)
	ORDER BY
		Anio

END
GO

CREATE PROCEDURE pauCuadroDeControlPartes (
	@SucursalID INT = NULL
	, @Pagadas BIT
	, @Cobradas BIT
	, @Solo9500 BIT
	, @OmitirDomingo BIT
	, @Desde DATE = NULL
	, @Hasta DATE = NULL
) AS BEGIN
	SET NOCOUNT ON

	/*
	DECLARE @SucursalID INT = NULL
	DECLARE @Pagadas BIT = 1
	DECLARE @Cobradas BIT = 0
	DECLARE @Solo9500 BIT = 0
	DECLARE @OmitirDomingo BIT = 0
	DECLARE @Desde DATE = '2016-01-01'
	DECLARE @Hasta DATE = '2016-12-31'
	EXEC pauCuadroDeControlPartes @SucursalID, @Pagadas, @Cobradas, @Solo9500, @OmitirDomingo, @Desde, @Hasta
	*/

	-- Definición de variables tipo constante
	DECLARE @EstCobradaID INT = 2
	DECLARE @EstPagadaID INT = 3
	DECLARE @EstCanceladaID INT = 4
	DECLARE @EstCanceladaSinPagoID INT = 5
	
	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)
	
	-- Consulta
	SELECT
		vpc.VentaID
		, v.Folio
		, ISNULL(vpc.Fecha, '') AS Fecha
		, v.SucursalID
		, v.ClienteID
		, p.ParteID
		, p.NumeroParte AS NumeroDeParte
		, l.LineaID
		, l.NombreLinea AS Linea
		, mp.MarcaParteID AS MarcaID
		, mp.NombreMarcaParte AS Marca
		, pv.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, p.SubsistemaID
		, ss.NombreSubsistema AS Subsistema
		, ss.SistemaID
		, si.NombreSistema AS Sistema
		
		, SUM((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) AS Utilidad
		, SUM((vd.PrecioUnitario - vd.CostoConDescuento) * vd.Cantidad) AS UtilidadConDescuento
		-- Precio con Iva
		, SUM((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) AS Precio
		, SUM(vd.Costo * vd.Cantidad) AS Costo
		, SUM(vd.CostoConDescuento * vd.Cantidad) AS CostoConDescuento
	FROM
		(
			SELECT
				VentaID
				, MAX(vp.Fecha) AS Fecha
			FROM
				VentaPago vp
				INNER JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
			WHERE
				vp.Estatus = 1
				AND (vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
				-- AND vpd.Importe > 0
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
				AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
		) vpc
		INNER JOIN Venta v ON v.VentaID = vpc.VentaID AND v.Estatus = 1
		LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.Estatus = 1
		-- LEFT JOIN Sucursal s ON s.SucursalID = v.SucursalID AND s.Estatus = 1
		-- LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		-- LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
		INNER JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN Subsistema ss ON ss.SubsistemaID = p.SubsistemaID AND ss.Estatus = 1
		LEFT JOIN Sistema si ON si.SistemaID = ss.SistemaID AND si.Estatus = 1
	WHERE
		-- p.AplicaComision = 1
		(@SucursalID IS NULL OR v.SucursalID = @SucursalID)
		AND (
			(@Pagadas = 1 AND (v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)))
			OR (@Cobradas = 1 AND (v.VentaEstatusID IN (@EstCobradaID)))
		)
		AND (@Solo9500 = 0 OR c9.Cotizacion9500ID IS NOT NULL)
		-- Se quitan las ventas de los domingos, si aplica
		AND (@OmitirDomingo = 0 OR DATEPART(DW, v.Fecha) != 1)
	GROUP BY
		vpc.VentaID
		, v.Folio
		, vpc.Fecha
		, v.SucursalID
		, v.ClienteID
		, p.ParteID
		, p.NumeroParte
		, l.LineaID
		, l.NombreLinea
		, mp.MarcaParteID
		, mp.NombreMarcaParte
		, pv.ProveedorID
		, pv.NombreProveedor
		, p.SubsistemaID
		, ss.NombreSubsistema
		, ss.SistemaID
		, si.NombreSistema

END
GO
