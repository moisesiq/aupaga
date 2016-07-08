/* Script con modificaciones para el módulo de ventas. Archivo 91
 * Creado: 2015/02/16
 * Subido: 2015/02/19
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	-- Contabilidad

	ALTER TABLE ContaCuenta ADD CuentaSat NVARCHAR(16) NULL
	ALTER TABLE ContaSubcuenta ADD CuentaSat NVARCHAR(16) NULL
	ALTER TABLE ContaCuentaDeMayor ADD CuentaSat NVARCHAR(16) NULL
	ALTER TABLE ContaCuentaAuxiliar ADD CuentaSat NVARCHAR(16) NULL

	CREATE TABLE ContaTipoPoliza (
		ContaTipoPolizaID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, TipoDePoliza NVARCHAR(32) NOT NULL
	)
	INSERT INTO ContaTipoPoliza (TipoDePoliza) VALUES
		('Ingresos')
		, ('Egresos')
		, ('Diario')

	CREATE TABLE ContaPoliza (
		ContaPolizaID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, Fecha DATETIME NOT NULL
		, ContaTipoPolizaID INT NOT NULL FOREIGN KEY REFERENCES ContaTipoPoliza(ContaTipoPolizaID)
		, Concepto NVARCHAR(256) NOT NULL
		, RealizoUsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		, Reportar BIT NOT NULL
		
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME NULL
	)

	CREATE TABLE ContaPolizaDetalle (
		ContaPolizaDetalleID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, ContaPolizaID INT NOT NULL FOREIGN KEY REFERENCES ContaPoliza(ContaPolizaID)
		, ContaCuentaAuxiliarID INT NOT NULL FOREIGN KEY REFERENCES ContaCuentaAuxiliar(ContaCuentaAuxiliarID)
		, Importe DECIMAL(12, 2) NOT NULL
		, EsCargo BIT NOT NULL
		, Referencia NVARCHAR(16) NULL
	)

	CREATE TABLE ContaConfigAfectacion (
		ContaConfigAfectacionID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, Operacion NVARCHAR(64) NOT NULL
		, ContaTipoPolizaID INT NOT NULL FOREIGN KEY REFERENCES ContaTipoPoliza(ContaTipoPolizaID)
	)
	INSERT INTO ContaConfigAfectacion (Operacion, ContaTipoPolizaID) VALUES
		('VentaContadoPago', 1)
		, ('VentaContadoVale', 1)
		, ('VentaCredito', 1)
		, ('DevolucionVentaPago', 1)
		, ('DevolucionVentaVale', 1)
		, ('PagoVentaCredito', 1)

		, ('NotaDeCreditoDescuentoVenta', 1)
		, ('NotaDeCreditoDevolucionVenta', 1)
		, ('NotaDeCreditoDevolucionVentaCredito', 1)

		, ('GarantiaVentaVale', 1)
		, ('GarantiaVentaPago', 1)

		, ('CompraContado', 1)
		, ('CompraCredito', 1)
		, ('PagoCompraCredito', 1)

		, ('EntradaInventario', 1)
		, ('SalidaInventario', 1)

		, ('Resguardo', 1)
		, ('Refuerzo', 1)
		, ('Gasto', 1)
		, ('DepositoBancario', 1)

	CREATE TABLE ContaConfigAfectacionDetalle (
		ContaConfigAfectacionDetalleID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, ContaConfigAfectacionID INT NOT NULL FOREIGN KEY REFERENCES ContaConfigAfectacion(ContaConfigAfectacionID)
		, EsCuentaDeMayor BIT NOT NULL
		, CuentaID INT NOT NULL
		, EsCargo BIT NOT NULL
		, Observacion NVARCHAR(254) NULL
	)

	CREATE TABLE NotaDeCreditoFiscalDetalle (
		NotaDeCreditoFiscalDetalleID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, NotaDeCreditoFiscalID INT NOT NULL FOREIGN KEY REFERENCES NotaDeCreditoFiscal(NotaDeCreditoFiscalID)
		, VentaID INT NOT NULL FOREIGN KEY REFERENCES Venta(VentaID)
		, Descuento DECIMAL(12, 2) NOT NULL
		, IvaDescuento DECIMAL(12, 2) NOT NULL
	)
	
	ALTER TABLE ContaCuentaAuxiliar ADD RelacionID INT NULL

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
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO

ALTER VIEW [dbo].[VentasGarantiasView] AS
	SELECT
		vg.VentaGarantiaID
		, ISNULL(CONVERT(BIT, CASE WHEN vfdv.FolioFiscal IS NULL THEN 0 ELSE 1 END), 0) AS Facturada
		, vg.VentaID
		, v.Folio AS FolioDeVenta
		, v.Fecha AS FechaDeVenta
		, v.ClienteID
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

CREATE VIEW ContaPolizasDetalleView AS
	SELECT
		cpd.ContaPolizaDetalleID
		, cpd.ContaPolizaID
		, cpd.ContaCuentaAuxiliarID
		, cca.CuentaAuxiliar
		, cca.CuentaContpaq
		, cca.CuentaSat
		, cpd.Importe
		, cpd.EsCargo
		, cpd.Referencia
	FROM
		ContaPolizaDetalle cpd
		LEFT JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = cpd.ContaCuentaAuxiliarID
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

ALTER PROCEDURE [dbo].[pauContaCuentasTotales] (
	@Desde DATE
	, @Hasta DATE
) AS BEGIN
	SET NOCOUNT ON

	/* DECLARE @Desde DATE = '2014-04-01'
	DECLARE @Hasta DATE = '2014-04-30'
	*/

	-- Definición de variables tipo constante
	DECLARE @SucMatrizID INT = 1
	DECLARE @Suc02ID INT = 2
	DECLARE @Suc03ID INT = 3

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)

	SELECT
		cc.ContaCuentaID
		, cs.ContaSubcuentaID
		, ccm.ContaCuentaDeMayorID
		, cca.ContaCuentaAuxiliarID
		, cc.Cuenta
		, cs.Subcuenta
		, ccm.CuentaDeMayor
		, cca.CuentaAuxiliar
		, SUM(CASE WHEN ce.EsFiscal = 1 THEN ce.Importe ELSE 0.00 END) AS Fiscal
		, SUM(ce.Importe) AS Total
		, SUM(CASE WHEN ce.SucursalID = @SucMatrizID THEN (ce.Importe 
			- ISNULL(ced2.Importe, 0.00) - ISNULL(ced3.Importe, 0.00)) ELSE ced1.Importe END) AS Matriz
		, SUM(CASE WHEN ce.SucursalID = @Suc02ID THEN (ce.Importe
			- ISNULL(ced1.Importe, 0.00) - ISNULL(ced3.Importe, 0.00)) ELSE ced2.Importe END) AS Suc02
		, SUM(CASE WHEN ce.SucursalID = @Suc03ID THEN (ce.Importe 
			- ISNULL(ced1.Importe, 0.00) - ISNULL(ced2.Importe, 0.00)) ELSE ced3.Importe END) AS Suc03
		, SUM(ISNULL(ced1.Importe, 0.00) + ISNULL(ced2.Importe, 0.00) + ISNULL(ced3.Importe, 0.00)) AS ImporteDev
	FROM
		ContaCuenta cc
		LEFT JOIN ContaSubcuenta cs ON cs.ContaCuentaID = cc.ContaCuentaID
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaSubcuentaID = cs.ContaSubcuentaID
		LEFT JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaDeMayorID = ccm.ContaCuentaDeMayorID
		LEFT JOIN ContaEgreso ce ON ce.ContaCuentaAuxiliarID = cca.ContaCuentaAuxiliarID
			AND (ce.Fecha >= @Desde AND ce.Fecha < @Hasta)
		LEFT JOIN (
			SELECT ContaEgresoID, SUM(Importe) AS Importe FROM ContaEgresoDevengado
			WHERE SucursalID = @SucMatrizID GROUP BY ContaEgresoID, SucursalID
		) ced1 ON ced1.ContaEgresoID = ce.ContaEgresoID
		LEFT JOIN (
			SELECT ContaEgresoID, SUM(Importe) AS Importe FROM ContaEgresoDevengado
			WHERE SucursalID = @Suc02ID GROUP BY ContaEgresoID, SucursalID
		) ced2 ON ced2.ContaEgresoID = ce.ContaEgresoID
		LEFT JOIN (
			SELECT ContaEgresoID, SUM(Importe) AS Importe FROM ContaEgresoDevengado
			WHERE SucursalID = @Suc03ID GROUP BY ContaEgresoID, SucursalID
		) ced3 ON ced3.ContaEgresoID = ce.ContaEgresoID
	GROUP BY
		cc.ContaCuentaID
		, cs.ContaSubcuentaID
		, ccm.ContaCuentaDeMayorID
		, cca.ContaCuentaAuxiliarID
		, cc.Cuenta
		, cs.Subcuenta
		, ccm.CuentaDeMayor
		, cca.CuentaAuxiliar
		, cc.CuentaSat
		, cs.CuentaSat
		, ccm.CuentaSat
		, cca.CuentaSat
	ORDER BY
		(cc.CuentaSat + cc.Cuenta)
		, (cs.CuentaSat + cs.Subcuenta)
		, (ccm.CuentaSat + ccm.CuentaDeMayor)
		, (cca.CuentaSat + cca.CuentaAuxiliar)

END
GO
