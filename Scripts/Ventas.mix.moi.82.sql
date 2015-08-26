/* Script con modificaciones para el módulo de ventas. Archivo 82
 * Creado: 2015/01/15
 * Subido: 2015/01/22
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	-- 9500
	ALTER TABLE Cotizacion9500 ADD
		BajaUsuarioID INT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, BajaMotivo NVARCHAR(512) NULL

	ALTER TABLE EstatusGenerico ALTER COLUMN Descripcion NVARCHAR(32) NOT NULL
	INSERT INTO EstatusGenerico (Descripcion) VALUES ('VENDIDO'), ('CANCELADO ANTES DE VENDER'), ('CANCELADO DESPUÉS DE VENDIDO')

	--

	INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
		('Pedidos.PorcentajeIncrementoPresupuesto', '5', '5', 'Porcentaje de incremento al importe de presupuesto de Pedidos.')

	INSERT INTO VentaGarantiaAccion (Etapa, Accion) VALUES (0, 'TRANSFERENCIA')

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

-- Clientes
ALTER TABLE ClienteEventoCalendario ADD Fecha DATETIME NULL
GO
UPDATE ClienteEventoCalendario SET Fecha = (DiaEvento + CONVERT(DATETIME, HoraEvento))
ALTER TABLE ClienteEventoCalendario ALTER COLUMN Fecha DATETIME NOT NULL
ALTER TABLE ClienteEventoCalendario DROP COLUMN DiaEvento, HoraEvento
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

ALTER VIEW [dbo].[VentasDetalleAvanzadoView] AS
	SELECT
		vd.VentaDetalleID
		, vd.VentaID
		, v.VentaEstatusID
		, v.SucursalID
		, v.Fecha AS VentaFecha
		, vd.ParteID
		, p.NumeroParte
		, p.NombreParte
		, vd.Costo
		, vd.CostoConDescuento
		, vd.Cantidad
		, vd.PrecioUnitario
		, vd.Iva
	FROM
		VentaDetalle vd
		LEFT JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
	WHERE vd.Estatus = 1
GO

ALTER VIEW [dbo].[Lista9500View] AS
	SELECT
		c9.Cotizacion9500ID
		, c9.EstatusGenericoID
		, eg.Descripcion AS Estatus
		, c9.Fecha
		, pv.NombreProveedor AS Proveedor
		, (l.NombreLinea + ' - ' + mp.NombreMarcaParte) AS LineaMarca
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, c9d.ParteID
		, c9d.Costo
		, c9d.PrecioAlCliente
		, c9.Anticipo
		, c.Nombre AS Cliente
		, s.NombreSucursal AS Sucursal
		, v.NombrePersona AS Vendedor
		, c9.BajaMotivo
		, ub.NombreUsuario AS BajaUsuario
	FROM
		Cotizacion9500 c9
		LEFT JOIN EstatusGenerico eg ON eg.EstatusGenericoID = c9.EstatusGenericoID
		LEFT JOIN (
			SELECT *
			FROM (
				SELECT
					c9d.Cotizacion9500ID
					, c9d.ProveedorID
					, c9d.MarcaParteID
					, c9d.LineaID
					, c9d.ParteID
					, c9d.Costo
					, c9d.PrecioAlCliente
					, ROW_NUMBER() OVER (PARTITION BY c9d.Cotizacion9500ID ORDER BY c9d.Cotizacion9500DetalleID) AS Numero
				FROM Cotizacion9500Detalle c9d
				WHERE c9d.Estatus = 1
			) c9d
			WHERE Numero = 1
		) c9d ON c9d.Cotizacion9500ID = c9.Cotizacion9500ID
		LEFT JOIN Proveedor pv ON pv.ProveedorID = c9d.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = c9d.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = c9d.LineaID AND l.Estatus = 1
		LEFT JOIN Parte p ON p.ParteID = c9d.ParteID AND p.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = c9.ClienteID AND c.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = c9.SucursalID AND s.Estatus = 1
		LEFT JOIN Usuario v ON v.UsuarioID = c9.RealizoUsuarioID AND v.Estatus = 1
		LEFT JOIN Usuario ub ON ub.UsuarioID = c9.BajaUsuarioID AND ub.Estatus = 1
	WHERE
		c9.Estatus = 1
GO

ALTER VIEW [dbo].[VentasGarantiasView] AS
	SELECT
		vg.VentaGarantiaID
		, ISNULL(CONVERT(BIT, CASE WHEN vfdv.FolioFiscal IS NULL THEN 0 ELSE 1 END), 0) AS Facturada
		, vg.VentaID
		, v.Folio AS FolioDeVenta
		, v.Fecha AS FechaDeVenta
		, vg.SucursalID
		, s.NombreSucursal AS Sucursal
		, ISNULL(CONVERT(BIT, v.ACredito), 0) AS VentaACredito
		, vg.Fecha
		, vg.MotivoID
		, vgm.Motivo
		, vg.MotivoObservacion
		, vg.AccionID
		, vga.Accion
		, vg.EstatusGenericoID
		, eg.Descripcion AS Estatus
		, vg.RealizoUsuarioID
		, u.NombrePersona AS Realizo
		, ISNULL(CONVERT(BIT, CASE WHEN ac.AutorizoUsuarioID IS NULL THEN 0 ELSE 1 END), 0) AS Autorizado
		, ISNULL(ua.NombreUsuario, 'PENDIENTE') AS AutorizoUsuario
		-- , CONVERT(DECIMAL(12, 2), vgd.Subtotal) AS Subtotal
		-- , CONVERT(DECIMAL(12, 2), vgd.Iva) AS Iva
		-- , CONVERT(DECIMAL(12, 2), vgd.Total) AS Total
		, (vg.PrecioUnitario + vg.Iva) AS Total
		, vg.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS NombreDeParte
		, mp.NombreMarcaParte AS Marca
		, l.NombreLinea AS Linea
		, vg.Costo
		, vg.PrecioUnitario
		, vg.Iva
		, m.NombreMedida AS Medida
		, p.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, vg.RespuestaID
		, vg.FechaCompletado
		, vg.ObservacionCompletado
		, vg.NotaDeCreditoID
	FROM
		VentaGarantia vg
		LEFT JOIN VentaGarantiaMotivo vgm ON vgm.VentaGarantiaMotivoID = vg.MotivoID
		LEFT JOIN VentaGarantiaAccion vga ON vga.VentaGarantiaAccionID = vg.AccionID
		LEFT JOIN Venta v ON v.VentaID = vg.VentaID AND v.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = vg.SucursalID AND s.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = vg.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN (
			SELECT
				ROW_NUMBER() OVER (PARTITION BY TablaRegistroID ORDER BY FechaAutorizo) AS Registro
				, TablaRegistroID
				, AutorizoUsuarioID
			FROM Autorizacion
			WHERE Tabla = 'VentaGarantia' AND Estatus = 1
		) ac ON ac.TablaRegistroID = vg.VentaGarantiaID AND ac.Registro = 1
		LEFT JOIN Usuario ua ON ua.UsuarioID = ac.AutorizoUsuarioID AND ua.Estatus = 1
		LEFT JOIN VentasFacturasDetalleView vfdv ON vfdv.VentaID = v.VentaID AND vfdv.EstatusGenericoID != 4
		LEFT JOIN Parte p ON p.ParteID = vg.ParteID AND p.Estatus = 1
		LEFT JOIN Medida m ON m.MedidaID = p.MedidaID AND m.Estatus = 1
		LEFT JOIN EstatusGenerico eg ON eg.EstatusGenericoID = vg.EstatusGenericoID
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
	WHERE vg.Estatus = 1
GO

ALTER VIEW [dbo].[MovimientosInventarioNoPagadosView] AS
SELECT 
	MovimientoInventario.MovimientoInventarioID	
	,MovimientoInventario.ProveedorID
	,MovimientoInventario.FolioFactura AS Factura
	,MovimientoInventario.FechaRecepcion AS Fecha
	,MovimientoInventario.ImporteTotal AS Importe
	-- ,CAST(0 AS DECIMAL(18,2)) AS Pagos
	, ppdc.Importe AS Pagos
	,CAST(0 AS DECIMAL(18,2)) AS Aplicados
	,CAST(0 AS DECIMAL(18,2)) AS PorAplicar
	-- ,CAST(0 AS DECIMAL(18,2)) AS Saldo
	, (MovimientoInventario.ImporteTotal - ppdc.Importe) AS Saldo
	,CAST(0 AS DECIMAL(18,2)) AS Pago
FROM 
	MovimientoInventario
	LEFT JOIN (
		SELECT
			MovimientoInventarioID
			, SUM(ImporteMovimiento) AS Importe
		FROM ProveedorPolizaDetalle
		WHERE Estatus = 1
		GROUP BY MovimientoInventarioID
	) ppdc ON ppdc.MovimientoInventarioID = MovimientoInventario.MovimientoInventarioID
WHERE
	MovimientoInventario.FueLiquidado = 0
	AND MovimientoInventario.Estatus = 1
	AND MovimientoInventario.TipoOperacionID = 1
GO

-- DROP VIEW PartesEquivalentesMaxMinView
CREATE VIEW PartesEquivalentesMaxMinView AS
	SELECT
		pe.ParteEquivalenteID
		, pe.GrupoID
		, pe.ParteID
		, pmm.SucursalID
		, pmm.Calcular
		, pmm.Maximo
		, pmm.Minimo
	FROM
		ParteEquivalente pe
		LEFT JOIN ParteMaxMin pmm ON pmm.ParteID = pe.ParteID
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

ALTER PROCEDURE [dbo].[pauVentasPartesPor] (
	@Desde DATE
	, @Hasta DATE
	, @SucursalID INT = NULL
	, @UsuarioID INT = NULL
) AS BEGIN
	SET NOCOUNT ON

	DECLARE @EstPagada INT = 3
	DECLARE @EstCobrada INT = 2
	
	SET @Hasta = DATEADD(DAY, 1, @Hasta)
	
	SELECT
		ISNULL(SUM(CASE WHEN vdc.Partes = 1 THEN 1 ELSE 0 END), 0) AS Uno
		, ISNULL(SUM(CASE WHEN vdc.Partes = 2 THEN 1 ELSE 0 END), 0) AS Dos
		, ISNULL(SUM(CASE WHEN vdc.Partes = 3 THEN 1 ELSE 0 END), 0) AS Tres
		, ISNULL(SUM(CASE WHEN vdc.Partes > 3 THEN 1 ELSE 0 END), 0) AS Mas
	FROM
		Venta v
		LEFT JOIN (
			SELECT VentaID, COUNT(*) AS Partes
			FROM VentaDetalle
			WHERE Estatus = 1
			GROUP BY VentaID
		) vdc ON vdc.VentaID = v.VentaID
	WHERE
		v.Estatus = 1
		AND v.VentaEstatusID IN (@EstCobrada, @EstPagada)
		AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
		AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
		AND (@UsuarioID IS NULL OR v.RealizoUsuarioID = @UsuarioID)
END
GO

ALTER PROCEDURE [dbo].[pauContaCuentasMovimientosTotales] (
	@Desde DATE
	, @Hasta DATE
	, @SucursalID INT = NULL
) AS BEGIN
	SET NOCOUNT ON

	/* EXEC pauContaCuentasMovimientosTotales 1, '2014-08-23', '2014-08-28'
	DECLARE @Desde DATE = '2014-04-01'
	DECLARE @Hasta DATE = '2014-04-30'
	*/

	-- Definición de variables tipo constante
	DECLARE @CdmRepartoDeUtilidades INT = 18

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)

	-- Gastos fijos - SucursalGastoFijo
	SELECT
		ROW_NUMBER() OVER (ORDER BY cca.CuentaAuxiliar) AS Registro
		, CONVERT(BIT, 1) AS GastoFijo
		-- , gf.SucursalID
		, SUM(gf.Importe) AS Importe
		, cca.ContaCuentaAuxiliarID
		, ccm.ContaCuentaDeMayorID
		, cs.ContaSubcuentaID
		, cc.ContaCuentaID
		, NULL AS Egreso
		, cca.CuentaAuxiliar
		, ccm.CuentaDeMayor
		, cs.Subcuenta
		, cc.Cuenta
		, cca.AfectaMetas
		, cca.SumaGastosFijos
		, cca.CalculoSemanal
	FROM
		SucursalGastoFijo gf
		LEFT JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = gf.ContaCuentaAuxiliarID
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = cca.ContaCuentaDeMayorID
		LEFT JOIN ContaSubcuenta cs ON cs.ContaSubcuentaID = ccm.ContaSubcuentaID
		LEFT JOIN ContaCuenta cc ON cc.ContaCuentaID = cs.ContaCuentaID
	WHERE
		(@SucursalID IS NULL OR gf.SucursalID = @SucursalID)
	GROUP BY
		cca.ContaCuentaAuxiliarID
		, ccm.ContaCuentaDeMayorID
		, cs.ContaSubcuentaID
		, cc.ContaCuentaID
		, cca.CuentaAuxiliar
		, ccm.CuentaDeMayor
		, cs.Subcuenta
		, cc.Cuenta
		, cca.AfectaMetas
		, cca.SumaGastosFijos
		, cca.CalculoSemanal

	UNION

	-- Gastos variables - ContaEgreso
	SELECT
		ROW_NUMBER() OVER (ORDER BY cca.CuentaAuxiliar) AS Registro
		, CONVERT(BIT, 0) AS GastoFijo
		-- , cedc.SucursalID
		, SUM(cedc.ImporteDev) AS Importe
		, cca.ContaCuentaAuxiliarID
		, ccm.ContaCuentaDeMayorID
		, cs.ContaSubcuentaID
		, cc.ContaCuentaID
		, ce.Observaciones AS Egreso
		, cca.CuentaAuxiliar
		, ccm.CuentaDeMayor
		, cs.Subcuenta
		, cc.Cuenta
		, cca.AfectaMetas
		, cca.SumaGastosFijos
		, cca.CalculoSemanal
	FROM
		ContaEgreso ce
		INNER JOIN (
			SELECT
				ContaEgresoID
				, SucursalID
				, SUM(Importe) AS ImporteDev
			FROM ContaEgresoDevengado
			WHERE (Fecha >= @Desde AND Fecha < @Hasta)
			GROUP BY ContaEgresoID, SucursalID
		) cedc ON cedc.ContaEgresoID = ce.ContaEgresoID
		INNER JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = ce.ContaCuentaAuxiliarID
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = cca.ContaCuentaDeMayorID
		LEFT JOIN ContaSubcuenta cs ON cs.ContaSubcuentaID = ccm.ContaSubcuentaID
		LEFT JOIN ContaCuenta cc ON cc.ContaCuentaID = cs.ContaCuentaID
	WHERE
		(@SucursalID IS NULL OR cedc.SucursalID = @SucursalID)
		-- Se quitan los egresos que ya se tienen como gastos fijos
		AND ce.ContaCuentaAuxiliarID NOT IN (
			SELECT ContaCuentaAuxiliarID
			FROM SucursalGastoFijo
			WHERE (@SucursalID IS NULL OR SucursalID = @SucursalID)
		)
		-- Se quitan los egresos de la Cuenta de Mayor "Reparto de Utilidades"
		AND cca.ContaCuentaDeMayorID != @CdmRepartoDeUtilidades
	GROUP BY
		cca.ContaCuentaAuxiliarID
		, ccm.ContaCuentaDeMayorID
		, cs.ContaSubcuentaID
		, cc.ContaCuentaID
		, ce.Observaciones
		, cca.CuentaAuxiliar
		, ccm.CuentaDeMayor
		, cs.Subcuenta
		, cc.Cuenta
		, cca.AfectaMetas
		, cca.SumaGastosFijos
		, cca.CalculoSemanal
	
	-- Se asigna el orden
	ORDER BY
		cc.Cuenta
		, cs.Subcuenta
		, ccm.CuentaDeMayor
		, cca.CuentaAuxiliar
		, Egreso

END
GO

ALTER PROCEDURE [dbo].[pauProClientesEventosCalendario] AS BEGIN
	SET NOCOUNT ON
	
	DECLARE @Ahora DATETIME = GETDATE()

	;WITH _Eventos AS (
		SELECT
			ccv.ClienteID
			, CONVERT(DATE, DATEADD(DAY,
				CASE WHEN ccv.DiaDeCobro = 2 THEN
					CASE WHEN DATEPART(DW, @Ahora) > 2 THEN (7 - DATEPART(DW, @Ahora) + 2) ELSE  (2 - DATEPART(DW, @Ahora)) END
				ELSE CASE WHEN ccv.DiaDeCobro = 3 THEN
					CASE WHEN DATEPART(DW, @Ahora) > 3 THEN (7 - DATEPART(DW, @Ahora) + 3) ELSE  (3 - DATEPART(DW, @Ahora)) END
				ELSE CASE WHEN ccv.DiaDeCobro = 4 THEN
					CASE WHEN DATEPART(DW, @Ahora) > 4 THEN (7 - DATEPART(DW, @Ahora) + 4) ELSE  (4 - DATEPART(DW, @Ahora)) END
				ELSE CASE WHEN ccv.DiaDeCobro = 5 THEN
					CASE WHEN DATEPART(DW, @Ahora) > 5 THEN (7 - DATEPART(DW, @Ahora) + 5) ELSE  (5 - DATEPART(DW, @Ahora)) END
				ELSE CASE WHEN ccv.DiaDeCobro = 6 THEN
					CASE WHEN DATEPART(DW, @Ahora) > 6 THEN (7 - DATEPART(DW, @Ahora) + 6) ELSE  (6 - DATEPART(DW, @Ahora)) END
				ELSE CASE WHEN ccv.DiaDeCobro = 7 THEN
					CASE WHEN DATEPART(DW, @Ahora) > 7 THEN (7 - DATEPART(DW, @Ahora) + 7) ELSE  (7 - DATEPART(DW, @Ahora)) END
				ELSE NULL END END END END END END 
				, @Ahora))
				+ CONVERT(DATETIME, ccv.HoraDeCobro) AS Fecha
			-- , ccv.HoraDeCobro AS HoraEvento
			, 'FECHA DE COBRO' AS Evento
		FROM
			ClientesCreditoView ccv
		WHERE
			ccv.AdeudoVencido > 0
	)

	INSERT INTO ClienteEventoCalendario (ClienteID, Fecha, Evento)
	SELECT e.*
	FROM
		_Eventos e
		LEFT JOIN ClienteEventoCalendario cec ON cec.ClienteID = e.ClienteID
			AND cec.Fecha = e.Fecha AND cec.Revisado = 0
	WHERE
		e.Fecha IS NOT NULL
		AND cec.ClienteEventoCalendarioID IS NULL

END
GO