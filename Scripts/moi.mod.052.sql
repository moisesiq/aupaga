/* Script con modificaciones a la base de datos de Theos. Archivo 052
 * Creado: 2015/11/27
 * Subido: 2015/12/04
 */

DECLARE @ScriptID INT = 52
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE ParteMaxMinRegla ADD Descripcion NVARCHAR(512) NULL

ALTER TABLE ParteMaxMin ADD ParteMaxMinReglaID INT NULL FOREIGN KEY REFERENCES ParteMaxMinRegla(ParteMaxMinReglaID)

DROP INDEX Ix_VentaDetalle_Estatus ON VentaDetalle
DROP INDEX Ix_VentaDetalle_VentaID ON VentaDetalle
CREATE INDEX Ix_Estatus_ParteID ON VentaDetalle(Estatus, ParteID) INCLUDE (VentaID)

DROP INDEX Ix_PartePrecio_PartePrecioID ON PartePrecio
DROP INDEX Ix_SegunPlan_BusquedaVenta ON PartePrecio
DROP INDEX Ix_Estatus ON PartePrecio
DROP INDEX Ix_ParteID ON PartePrecio
CREATE INDEX Ix_Estatus ON PartePrecio(Estatus) INCLUDE (ParteID)

ALTER TABLE MovimientoInventario ADD EsAgrupador BIT NOT NULL DEFAULT 0
ALTER TABLE MovimientoInventario ADD MovimientoAgrupadorID INT NULL 
	FOREIGN KEY REFERENCES MovimientoInventario(MovimientoInventarioID)

ALTER TABLE ProveedorPolizaDetalle ADD Subtotal DECIMAL(12, 2) NULL
ALTER TABLE ProveedorPolizaDetalle ADD Iva DECIMAL(12, 2) NULL
GO
UPDATE ProveedorPolizaDetalle SET
	Subtotal = CASE WHEN ce.Facturado = 0 THEN ppd.Importe ELSE ROUND(ppd.Importe / 1.16, 2) END
	, Iva = CASE WHEN ce.Facturado = 0 THEN 0 ELSE (ppd.Importe - ROUND(ppd.Importe / 1.16, 2)) END
FROM
	ProveedorPolizaDetalle ppd
	LEFT JOIN CajaEgreso ce ON ce.CajaEgresoID = ppd.CajaEgresoID AND ce.Estatus = 1
ALTER TABLE ProveedorPolizaDetalle ALTER COLUMN Subtotal DECIMAL(12, 2) NOT NULL
ALTER TABLE ProveedorPolizaDetalle ALTER COLUMN Iva DECIMAL(12, 2) NOT NULL
ALTER TABLE ProveedorPolizaDetalle DROP COLUMN Importe

ALTER TABLE ProveedorPoliza DROP COLUMN ImportePago

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

DROP VIEW [dbo].[ProveedorPagosYdevolucionesView]

GO

ALTER VIEW [dbo].[ExistenciasView] AS
	SELECT
		ParteExistencia.ParteExistenciaID
		,ParteExistencia.ParteID
		,Parte.NumeroParte
		, Parte.UnidadEmpaque AS UEmp
		,ParteExistencia.SucursalID
		,Sucursal.NombreSucursal AS Tienda
		,ParteExistencia.Existencia AS Exist
		-- ,ParteExistencia.Maximo AS [Max]
		-- ,ParteExistencia.Minimo AS [Min]
		, pmm.Maximo AS [Max]
		, pmm.Minimo AS [Min]
		, pmm.ParteMaxMinReglaID
		, pmm.FechaCalculo AS FechaMaxMin
		, pmmr.Descripcion AS DescripcionMaxMin
	FROM
		ParteExistencia
		INNER JOIN Parte ON Parte.ParteID = ParteExistencia.ParteID AND Parte.Estatus = 1
		INNER JOIN Sucursal ON Sucursal.SucursalID = ParteExistencia.SucursalID AND Sucursal.Estatus = 1
		LEFT JOIN ParteMaxMin pmm ON pmm.ParteID = ParteExistencia.ParteID AND pmm.SucursalID = ParteExistencia.SucursalID
		LEFT JOIN ParteMaxMinRegla pmmr ON pmmr.ParteMaxMinReglaID = pmm.ParteMaxMinReglaID
	WHERE ParteExistencia.Estatus = 1
GO

ALTER VIEW [dbo].[ProveedoresComprasView] AS
	SELECT
		mi.MovimientoInventarioID
		, mi.ProveedorID
		, mi.FolioFactura AS Factura
		, mi.FechaRecepcion AS Fecha
		, mi.ImporteFactura
		, mi.ImporteTotal
		, ISNULL(SUM(ppd.Subtotal + ppd.Iva), 0.0) AS Abonado
		, (mi.ImporteFactura - ISNULL(SUM(ppd.Subtotal + ppd.Iva), 0.0)) AS Saldo
		, SUM(CASE WHEN ppd.OrigenID IS NULL OR ppd.OrigenID = 1 THEN 0.0 ELSE (ppd.Subtotal + ppd.Iva) END) AS Descuento
		, (
			(mi.ImporteFactura - ISNULL(SUM(ppd.Subtotal + ppd.Iva), 0.0))
			- SUM(CASE WHEN ppd.OrigenID IS NULL OR ppd.OrigenID = 1 THEN 0.0 ELSE (ppd.Subtotal + ppd.Iva) END)
		) AS Final
		, u.NombreUsuario AS Usuario
		-- , ppd.ProveedorPolizaID
		, mi.EsAgrupador
		, mi.MovimientoAgrupadorID
	FROM
		MovimientoInventario mi
		LEFT JOIN ProveedorPolizaDetalle ppd ON ppd.MovimientoInventarioID = mi.MovimientoInventarioID AND ppd.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = mi.UsuarioID AND u.Estatus = 1
	WHERE
		mi.Estatus = 1
		AND mi.TipoOperacionID = 1
	GROUP BY
		mi.MovimientoInventarioID
		, mi.ProveedorID
		, mi.FolioFactura
		, mi.FechaRecepcion
		, mi.ImporteFactura
		, mi.ImporteTotal
		, u.NombreUsuario
		-- , ppd.ProveedorPolizaID
		, mi.EsAgrupador
		, mi.MovimientoAgrupadorID
GO

ALTER VIEW [dbo].[ProveedoresPolizasView] AS
	SELECT
		pp.ProveedorPolizaID
		, pp.ProveedorID
		, p.NombreProveedor AS Proveedor
		, pp.FechaPago
		-- , pp.ImportePago
		, pp.UsuarioID
		, u.NombreUsuario AS Usuario
		, pp.BancoCuentaID
		, bc.NombreDeCuenta
		, pp.TipoFormaPagoID AS FormaDePagoID
		, tfp.NombreTipoFormaPago AS FormaDePago
		, pp.FolioDePago
	FROM
		ProveedorPoliza pp
		LEFT JOIN Proveedor p ON p.ProveedorID = pp.ProveedorID AND p.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = pp.UsuarioID AND u.Estatus = 1
		LEFT JOIN BancoCuenta bc ON bc.BancoCuentaID = pp.BancoCuentaID
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = pp.TipoFormaPagoID AND tfp.Estatus = 1
	WHERE pp.Estatus = 1
GO

ALTER VIEW [dbo].[ProveedoresNotasDeCreditoView] AS
	SELECT
		ndc.ProveedorNotaDeCreditoID
		, ndc.ProveedorID
		, ndc.Folio
		, ndc.Fecha
		, ndc.Subtotal
		, ndc.Iva
		, (ndc.Subtotal + ndc.Iva) AS Total
		, ISNULL(SUM(ppd.Subtotal + ppd.Iva), 0.0) AS Usado
		, ((ndc.Subtotal + ndc.Iva) - ISNULL(SUM(ppd.Subtotal + ppd.Iva), 0.0)) AS Restante
		, ndc.OrigenID
		, ndco.Origen
		, ndc.Observacion
		, ndc.Disponible
	FROM
		ProveedorNotaDeCredito ndc
		LEFT JOIN ProveedorNotaDeCreditoOrigen ndco ON ndco.ProveedorNotaDeCreditoOrigenID = ndc.OrigenID
		LEFT JOIN ProveedorPolizaDetalle ppd ON ppd.NotaDeCreditoID = ndc.ProveedorNotaDeCreditoID AND ppd.Estatus = 1
	GROUP BY
		ndc.ProveedorNotaDeCreditoID
		, ndc.ProveedorID
		, ndc.Folio
		, ndc.Fecha
		, ndc.Subtotal
		, ndc.Iva
		, ndc.OrigenID
		, ndco.Origen
		, ndc.Observacion
		, ndc.Disponible
GO

ALTER VIEW [dbo].[ProveedoresPolizasDetalleAvanzadoView] AS
	SELECT
		ppd.ProveedorPolizaDetalleID
		, ppd.ProveedorPolizaID
		, ppd.OrigenID
		, ppo.Origen
		, pp.FechaPago AS Fecha
		, ppd.MovimientoInventarioID
		, mi.FolioFactura
		, (ppd.Subtotal + ppd.Iva) AS Importe
		-- , ppd.TipoFormaPagoID
		, tfp.NombreTipoFormaPago AS FormaDePago
		-- , ppd.BancoCuentaID
		, pp.FolioDePago
		, ppd.NotaDeCreditoID
		, ppd.Observacion
		, ppd.Folio
		, ndc.Folio AS NotaDeCredito
		, ndc.OrigenID AS NotaDeCreditoOrigenID
		, ndco.Origen AS NotaDeCreditoOrigen
	FROM
		ProveedorPolizaDetalle ppd
		LEFT JOIN ProveedorPolizaDetalleOrigen ppo ON ppo.ProveedorPolizaDetalleOrigenID = ppd.OrigenID
		LEFT JOIN ProveedorPoliza pp ON pp.ProveedorPolizaID = ppd.ProveedorPolizaID AND pp.Estatus = 1
		LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = ppd.MovimientoInventarioID AND mi.Estatus = 1
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = pp.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN ProveedorNotaDeCredito ndc ON ndc.ProveedorNotaDeCreditoID = ppd.NotaDeCreditoID
		LEFT JOIN ProveedorNotaDeCreditoOrigen ndco ON ndco.ProveedorNotaDeCreditoOrigenID = ndc.OrigenID
	WHERE ppd.Estatus = 1
GO

ALTER VIEW [dbo].[ProveedoresPagosView] AS
	SELECT
		ppd.ProveedorPolizaID
		, pp.FechaPago
		, ppd.MovimientoInventarioID
		, mi.ProveedorID
		, mi.FolioFactura AS Factura
		, mi.FechaRecepcion AS FechaFactura
		, mi.ImporteFactura
		, mi.ImporteTotal
		, (SELECT SUM(Subtotal + Iva) FROM ProveedorPolizaDetalle WHERE MovimientoInventarioID = ppd.MovimientoInventarioID)
			AS AbonadoTotal
		, ISNULL(SUM(ppd.Subtotal), 0.0) AS AbonadoSubtotal
		, ISNULL(SUM(ppd.Iva), 0.0) AS AbonadoIva
		, ISNULL(SUM(ppd.Subtotal + ppd.Iva), 0.0) AS AbonadoImporte
		, SUM(CASE WHEN ppd.OrigenID IS NULL OR ppd.OrigenID = 1 THEN 0.0 ELSE (ppd.Subtotal + ppd.Iva) END) AS Descuento
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

ALTER VIEW [dbo].[CajaEgresosProveedoresView] AS
	SELECT
		ce.CajaEgresoID
		, ce.Fecha
		, ce.Concepto
		, ce.Subtotal
		, ce.Iva
		, ce.Importe AS Total
		, ISNULL(SUM(ppd.Subtotal + ppd.Iva), 0.0) AS Usado
		, (ce.Importe - ISNULL(SUM(ppd.Subtotal + ppd.Iva), 0.0)) AS Restante
		, cne.ContaCuentaAuxiliarID
		, ce.Facturado
		, ce.AfectadoEnProveedores
	FROM
		CajaEgreso ce
		LEFT JOIN ProveedorPolizaDetalle ppd ON ppd.CajaEgresoID = ce.CajaEgresoID AND ppd.Estatus = 1
		LEFT JOIN ContaEgreso cne ON cne.ContaEgresoID = ce.ContaEgresoID
	GROUP BY
		ce.CajaEgresoID
		, ce.Fecha
		, ce.Concepto
		, ce.Subtotal
		, ce.Iva
		, ce.Importe
		, cne.ContaCuentaAuxiliarID
		, ce.Facturado
		, ce.AfectadoEnProveedores
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

ALTER PROCEDURE [dbo].[pauPartesMaxMin] (
	@SucursalID INT
	, @Desde DATE
	, @Hasta DATE
	, @Proveedores tpuTablaEnteros READONLY
	, @Marcas tpuTablaEnteros READONLY
	, @Lineas tpuTablaEnteros READONLY
) AS BEGIN
	SET NOCOUNT ON

	/*
	DECLARE @SucursalID INT = 1
	DECLARE @Desde DATE = '2015-01-01'
	DECLARE @Hasta DATE = '2015-12-31'
	DECLARE @Proveedores tpuTablaEnteros
	DECLARE @Marcas tpuTablaEnteros
	DECLARE @Lineas tpuTablaEnteros
	EXEC pauPartesMaxMin @SucursalID, @Desde, @Hasta
	*/

	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3
	DECLARE @ParteEstActivo INT = 1

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)

	-- Procedimiento
	
	-- Ventas
	;WITH _Ventas AS (
		SELECT
			vd.ParteID
			, CONVERT(DATE, v.Fecha) AS Fecha
			, COUNT(v.VentaID) AS Ventas
			, SUM(vd.Cantidad) AS Cantidad
			, SUM((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) AS Utilidad
		FROM
			VentaDetalle vd
			INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
			-- LEFT JOIN PartePrecio pp ON pp.ParteID = vd.ParteID AND pp.Estatus = 1
			left join Parte p on p.ParteID = vd.ParteID and p.Estatus = 1
		WHERE
			vd.Estatus = 1
			AND v.VentaEstatusID = @EstPagadaID
			AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			AND v.SucursalID = @SucursalID
		GROUP BY
			vd.ParteID
			, CONVERT(DATE, v.Fecha)

		-- Se agrega lo negado
		UNION ALL
		SELECT
			rf.ParteID
			, CONVERT(DATE, rf.FechaRegistro) AS Fecha
			, COUNT(rf.ReporteDeFaltanteID) AS Ventas
			, SUM(rf.CantidadRequerida) AS Cantidad
			, SUM((pp.PrecioUno - pp.Costo) * rf.CantidadRequerida) AS Utilidad
		FROM
			ReporteDeFaltante rf
			LEFT JOIN PartePrecio pp ON pp.ParteID = rf.ParteID AND pp.Estatus = 1
			-- Para validar que no se haya vendido en el día o en el siguiente
			LEFT JOIN (
				SELECT DISTINCT
					vd.ParteID
					, CONVERT(DATE, v.Fecha) AS Dia
				FROM
					VentaDetalle vd
					INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
				WHERE
					vd.Estatus = 1
					AND v.VentaEstatusID = @EstPagadaID
					AND v.SucursalID = @SucursalID
			) vdc ON vdc.ParteID = rf.ParteID AND (rf.FechaRegistro >= vdc.Dia AND rf.FechaRegistro < DATEADD(d, 2, vdc.Dia))
		WHERE
			rf.Estatus = 1
			AND (rf.FechaRegistro >= @Desde AND rf.FechaRegistro < @Hasta)
			AND rf.SucursalID = @SucursalID
			-- Parte de Venta
			AND vdc.ParteID IS NULL
		GROUP BY
			rf.ParteID
			, CONVERT(DATE, rf.FechaRegistro)
		-- Fin lo negado
	)
	-- Ventas por semana
	, _VentasPorSem AS (
		SELECT
			ParteID
			, DATEPART(WK, Fecha) AS Semana
			, SUM(Cantidad) AS Cantidad
		FROM _Ventas
		GROUP BY
			ParteID
			, DATEPART(WK, Fecha)
	)
	-- Ventas por mes
	, _VentasPorMes AS (
		SELECT
			ParteID
			, MONTH(Fecha) AS Mes
			, SUM(Cantidad) AS Cantidad
		FROM _Ventas
		GROUP BY
			ParteID
			, MONTH(Fecha)
	)

	-- Consulta final
	SELECT
		p.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, p.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, p.MarcaParteID
		, mp.NombreMarcaParte AS Marca
		, p.LineaID
		, l.NombreLinea AS Linea
		, p.UnidadEmpaque
		, p.EsPar
		, p.TiempoReposicion
		, pe.Existencia
		, pa.AbcDeVentas
		, pa.AbcDeUtilidad
		, pa.AbcDeNegocio
		, pa.AbcDeProveedor
		, pa.AbcDeLinea
		, pmm.Fijo
		, pmm.Minimo
		, pmm.Maximo
		, pmm.FechaCalculo
		, pmm.ParteMaxMinReglaID
		, pmmr.Descripcion AS DescripcionCalculo
		
		, ISNULL(SUM(vc.Ventas), 0) AS VentasTotal
		, ISNULL(SUM(vc.Cantidad), 0.00) AS CantidadTotal
		, ISNULL(MAX(vc.Cantidad), 0.00) AS CantidadMaxDia
		, ISNULL(MAX(vcs.Cantidad), 0.00) AS CantidadMaxSem
		, ISNULL(MAX(vcm.Cantidad), 0.00) AS CantidadMaxMes
		, ISNULL(SUM(vc.Utilidad), 0.00) AS UtilidadTotal
		, ISNULL(pmm.VentasGlobales, 0) AS VentasGlobales
	FROM
		Parte p
		INNER JOIN ParteMaxMin pmm ON pmm.ParteID = p.ParteID AND pmm.SucursalID = @SucursalID
		LEFT JOIN _Ventas vc ON vc.ParteID = p.ParteID
		LEFT JOIN (
			SELECT ParteID, MAX(Cantidad) AS Cantidad FROM _VentasPorSem GROUP BY ParteID
		) vcs ON vcs.ParteID = p.ParteID
		LEFT JOIN (
			SELECT ParteID, MAX(Cantidad) AS Cantidad FROM _VentasPorMes GROUP BY ParteID
		) vcm ON vcm.ParteID = p.ParteID
		LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
		LEFT JOIN ParteAbc pa ON pa.ParteID = p.ParteID
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN ParteMaxMinRegla pmmr ON pmmr.ParteMaxMinReglaID = pmm.ParteMaxMinReglaID
	WHERE
		p.Estatus = 1
		AND p.ParteEstatusID = @ParteEstActivo
		AND pmm.Calcular = 1
		-- AND (@ProveedorID IS NULL OR p.ProveedorID = @ProveedorID)
		AND (NOT EXISTS(SELECT TOP 1 1 FROM @Proveedores) OR p.ProveedorID IN (SELECT Entero FROM @Proveedores))
		AND (NOT EXISTS(SELECT TOP 1 1 FROM @Marcas) OR p.MarcaParteID IN (SELECT Entero FROM @Marcas))
		AND (NOT EXISTS(SELECT TOP 1 1 FROM @Lineas) OR p.LineaID IN (SELECT Entero FROM @Lineas))
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
		, p.UnidadEmpaque
		, p.EsPar
		, p.TiempoReposicion
		, pe.Existencia
		, pa.AbcDeVentas
		, pa.AbcDeUtilidad
		, pa.AbcDeNegocio
		, pa.AbcDeProveedor
		, pa.AbcDeLinea
		, pmm.Fijo
		, pmm.Minimo
		, pmm.Maximo
		, pmm.FechaCalculo
		, pmm.ParteMaxMinReglaID
		, pmmr.Descripcion
		, pmm.VentasGlobales
	ORDER BY
		VentasTotal DESC

END
GO