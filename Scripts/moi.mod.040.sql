/* Script con modificaciones a la base de datos de Theos. Archivo 040
 * Creado: 2015/10/14
 * Subido: 2015/10/16
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */



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

ALTER PROCEDURE [dbo].[pauPartesMaster] (
	@Desde DATE
	, @Hasta DATE
	-- , @ProveedorID INT = NULL
	-- , @MarcaID INT = NULL
	-- , @LineaID INT = NULL
	, @Proveedores tpuTablaEnteros READONLY
	, @Marcas tpuTablaEnteros READONLY
	, @Lineas tpuTablaEnteros READONLY
) AS BEGIN
	SET NOCOUNT ON

	/*
	DECLARE @Desde DATE = '2013-02-01'
	DECLARE @Hasta DATE = '2014-03-31'
	DECLARE @ProveedorID INT = NULL
	DECLARE @MarcaID INT = NULL
	DECLARE @LineaID INT = NULL
	*/

	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)

	-- Procedimiento
	
	SELECT
		p.ParteID
		, p.CodigoBarra AS CodigoDeBara
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, p.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, p.MarcaParteID
		, m.NombreMarcaParte AS Marca
		, p.LineaID
		, l.NombreLinea AS Linea
		, p.MedidaID
		, md.NombreMedida AS UnidadDeMedida
		, p.UnidadEmpaque AS UnidadDeEmpaque
		, p.TiempoReposicion AS TiempoDeReposicion
		, p.AplicaComision
		, p.EsServicio
		, p.Etiqueta
		, p.SoloUnaEtiqueta
		, p.EsPar
		, pec.Existencia
		, ISNULL(vc.Ventas, 0) AS Ventas
		, pp.Costo
		, pp.CostoConDescuento
		, pp.PorcentajeUtilidadUno
		, pp.PrecioUno
		, pp.PorcentajeUtilidadDos
		, pp.PrecioDos
		, pp.PorcentajeUtilidadTres
		, pp.PrecioTres
		, pp.PorcentajeUtilidadCuatro
		, pp.PrecioCuatro
		, pp.PorcentajeUtilidadCinco
		, pp.PrecioCinco
	FROM
		Parte p
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte m ON m.MarcaParteID = p.MarcaParteID AND m.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN Medida md ON md.MedidaID = p.MedidaID AND md.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND pp.Estatus = 1
		LEFT JOIN (
			SELECT
				ParteID
				, SUM(Existencia) AS Existencia
			FROM ParteExistencia
			WHERE Estatus = 1
			GROUP BY ParteID
		) pec ON pec.ParteID = p.ParteID
		LEFT JOIN (
			SELECT
				vd.ParteID
				, COUNT(v.VentaID) AS Ventas
			FROM
				VentaDetalle vd
				INNER JOIN Venta v ON v.VentaID = vd.VentaID
			WHERE
				vd.Estatus = 1
				AND v.Estatus = 1
				AND v.VentaEstatusID = @EstPagadaID
				AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			GROUP BY vd.ParteID
		) vc ON vc.ParteID = p.ParteID
	WHERE
		p.Estatus = 1
		-- AND (@ProveedorID IS NULL OR p.ProveedorID = @ProveedorID)
		-- AND (@MarcaID IS NULL OR p.MarcaParteID = @MarcaID)
		-- AND (@LineaID IS NULL OR p.LineaID = @LineaID)
		AND (NOT EXISTS(SELECT TOP 1 1 FROM @Proveedores) OR p.ProveedorID IN (SELECT Entero FROM @Proveedores))
		AND (NOT EXISTS(SELECT TOP 1 1 FROM @Marcas) OR p.MarcaParteID IN (SELECT Entero FROM @Marcas))
		AND (NOT EXISTS(SELECT TOP 1 1 FROM @Lineas) OR p.LineaID IN (SELECT Entero FROM @Lineas))

END
GO

ALTER PROCEDURE [dbo].[pauContaCuentasPorSemana] (
	@Desde DATE
	, @Hasta DATE
	, @AfectaMetas BIT = NULL
) AS BEGIN
	SET NOCOUNT ON
	
	/* EXEC pauContaCuentasPorSemana '2014-01-01', '2014-12-31'
	DECLARE @Desde DATE = '2014-01-01'
	DECLARE @Hasta DATE = '2014-12-31'
	*/

	-- Definición de variables tipo constante
	DECLARE @SubcuentaGastosFinancieros INT = 10
	DECLARE @SubcuentaGastosGenerales INT = 11

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
		, cca.PeriodicidadMes
		, cca.FinSemanalizar
		-- , ced.Fecha
		-- , DATEPART(WEEK, ced.Fecha) AS Semana
		-- , CONVERT(DATE, CASE WHEN DATEPART(DW, ced.Fecha) = 7 THEN ced.Fecha
		-- 	ELSE DATEADD(DAY, (DATEPART(DW, ced.Fecha) * -1), ced.Fecha) END) AS DiaIni
		, ced.Fecha
		, SUM(ced.Importe) AS ImporteDev
		-- , ce.Observaciones AS Egreso
		-- , cca.CalculoSemanal
		-- , cca.DiasMovimiento
		-- , (ISNULL(cca.DiasMovimiento, 7) / 7) AS Semanas
		-- , SUM(CASE WHEN cca.CalculoSemanal = 1 THEN ((ced.Importe / cca.DivisorDia) * 7) ELSE ced.Importe END) AS Importe
	FROM
		ContaEgresoDevengado ced
		INNER JOIN ContaEgreso ce ON ce.ContaEgresoID = ced.ContaEgresoID
		INNER JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = ce.ContaCuentaAuxiliarID

		LEFT JOIN Sucursal s ON s.SucursalID = ced.SucursalID AND s.Estatus = 1
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = cca.ContaCuentaDeMayorID
		LEFT JOIN ContaSubcuenta cs ON cs.ContaSubcuentaID = ccm.ContaSubcuentaID
			AND cs.ContaSubcuentaID IN (@SubcuentaGastosGenerales, @SubcuentaGastosFinancieros)
		LEFT JOIN ContaCuenta cc ON cc.ContaCuentaID = cs.ContaCuentaID
	WHERE
		(ced.Fecha >= @Desde AND ced.Fecha < @Hasta)
		AND cs.ContaSubcuentaID IS NOT NULL
		AND (@AfectaMetas IS NULL OR @AfectaMetas = 0 OR cca.AfectaMetas = @AfectaMetas)
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
		, cca.PeriodicidadMes
		, cca.FinSemanalizar
		, ced.Fecha
		-- , DATEPART(WEEK, ced.Fecha)
		-- , CONVERT(DATE, CASE WHEN DATEPART(DW, ced.Fecha) = 7 THEN ced.Fecha
		--	ELSE DATEADD(DAY, (DATEPART(DW, ced.Fecha) * -1), ced.Fecha) END)
		-- , cca.DiasMovimiento
	ORDER BY
		Sucursal
		, Cuenta
		, Subcuenta
		, CuentaDeMayor
		, CuentaAuxiliar
		, ced.Fecha DESC

END
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