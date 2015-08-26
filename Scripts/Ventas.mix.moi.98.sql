/* Script con modificaciones para el módulo de ventas. Archivo 98
 * Creado: 2015/04/03
 * Subido: 2015/04/14
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	-- Catálogo de Partes
	CREATE TABLE ParteError (
		ParteErrorID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, ParteID INT NOT NULL FOREIGN KEY REFERENCES Parte(ParteID)
		, Fecha DATETIME NOT NULL
		, Foto BIT NOT NULL
		, Equivalente BIT NOT NULL
		, Aplicacion BIT NOT NULL
		, Alterno BIT NOT NULL
		, Complemento BIT NOT NULL
		, Otro BIT NOT NULL
		, ComentarioError NVARCHAR(256) NULL
		, ComentarioSolucion NVARCHAR(256) NULL
		, ErrorUsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, SolucionUsuarioID INT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	)

	-- Cobranza
	ALTER TABLE NotaDeCreditoFiscalDetalle ADD
		ListaDePreciosUsada INT NULL
		, ImporteAntes DECIMAL(12, 2) NULL

	-- Contabilidad
	INSERT INTO ContaConfigAfectacion (Operacion, ContaTipoPolizaID) VALUES	('Vale Directo', 1)

	ALTER TABLE BancoCuentaMovimiento ALTER Column Concepto NVARCHAR(128) NULL
	ALTER TABLE BancoCuentaMovimiento ADD
		EsDeVenta BIT NULL
		, FechaConciliado DATETIME NULL
		, ConciliadoUsuarioID INT NULL REFERENCES Usuario(UsuarioID)
		, Observacion NVARCHAR(128) NULL

	ALTER TABLE ContaEgreso ADD BancoCuentaID INT NULL REFERENCES BancoCuenta(BancoCuentaID)

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

-- DROP VIEW PartesErroresView
CREATE VIEW PartesErroresView AS
	SELECT
		pe.ParteErrorID
		, pe.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, pe.Fecha
		, pe.Foto
		, pe.Equivalente
		, pe.Aplicacion
		, pe.Alterno
		, pe.Complemento
		, pe.Otro
		, pe.ComentarioError
		, pe.ComentarioSolucion
		, pe.ErrorUsuarioID
		, eu.NombreUsuario AS UsuarioError
		, pe.SolucionUsuarioID
		, su.NombreUsuario AS UsuarioSolucion
	FROM
		ParteError pe
		LEFT JOIN Parte p ON p.ParteID = pe.ParteID AND p.Estatus = 1
		LEFT JOIN Usuario eu ON eu.UsuarioID = pe.ErrorUsuarioID AND eu.Estatus = 1
		LEFT JOIN Usuario su ON su.UsuarioID = pe.SolucionUsuarioID AND su.Estatus = 1
GO

ALTER VIEW [dbo].[Cotizaciones9500View] AS
	SELECT
		c9.Cotizacion9500ID
		, c9.EstatusGenericoID
		, c9.Fecha
		, c9.SucursalID
		, c9dc.Importe AS Total
		, c9.Anticipo
		, c9.ClienteID
		, c.Nombre AS Cliente
		, c.Celular
		, u.NombrePersona AS Vendedor
		, v.Folio AS AnticipoVentaFolio
	FROM
		Cotizacion9500 c9
		LEFT JOIN (
			SELECT Cotizacion9500ID, SUM(PrecioAlCliente * Cantidad) AS Importe
			FROM Cotizacion9500Detalle
			GROUP BY Cotizacion9500ID
		) c9dc ON c9dc.Cotizacion9500ID = c9.Cotizacion9500ID
		LEFT JOIN Cliente c ON c.ClienteID = c9.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = c9.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = c9.AnticipoVentaID AND v.Estatus = 1
	WHERE c9.Estatus = 1
GO

ALTER VIEW [dbo].[BancosCuentasMovimientosView] AS
	SELECT
		bcm.BancoCuentaMovimientoID
		, bcm.BancoCuentaID
		, bcm.EsIngreso
		, bcm.Fecha
		, bcm.FechaAsignado
		, bcm.SucursalID
		, s.NombreSucursal AS Sucursal
		, bcm.Importe
		, bcm.Concepto
		, bcm.Referencia
		, bcm.TipoFormaPagoID
		, tfp.NombreTipoFormaPago AS FormaDePago
		, bcm.DatosDePago
		, bcm.SaldoAcumulado
		, bcm.Conciliado
		, bcm.FechaConciliado
		, bcm.ConciliadoUsuarioID
		, cu.NombreUsuario AS UsuarioConciliado
		, bcm.Observacion
		, ISNULL(bcm.EsDeVenta, 0) AS EsDeVenta
		, ISNULL(vpdr.Resguardado, 0) AS Resguardado
		, bcm.RelacionID
	FROM
		BancoCuentaMovimiento bcm
		LEFT JOIN Sucursal s ON s.SucursalID = bcm.SucursalID AND s.Estatus = 1
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = bcm.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN Usuario cu ON cu.UsuarioID = bcm.ConciliadoUsuarioID AND cu.Estatus = 1
		LEFT JOIN VentaPagoDetalle vpd ON bcm.EsDeVenta = 1 AND vpd.VentaPagoDetalleID = bcm.RelacionID AND vpd.Estatus = 1
		LEFT JOIN VentaPagoDetalleResguardo vpdr ON vpdr.VentaPagoDetalleID = vpd.VentaPagoDetalleID
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
	DECLARE @SubcuentaGastosGenerales INT = 11

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
		LEFT JOIN ContaSubcuenta cs ON cs.ContaCuentaID = cc.ContaCuentaID AND cs.ContaSubcuentaID = @SubcuentaGastosGenerales
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
	WHERE cs.ContaSubcuentaID IS NOT NULL
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