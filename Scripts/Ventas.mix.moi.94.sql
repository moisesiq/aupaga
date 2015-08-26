/* Script con modificaciones para el módulo de ventas. Archivo 94
 * Creado: 2015/03/11
 * Subido: 2015/03/16
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	-- Contabilidad

	CREATE TABLE BancoCuenta (
		BancoCuentaID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, NumeroDeCuenta NVARCHAR(16) NOT NULL
		, NombreDeCuenta NVARCHAR(32) NOT NULL
		, BancoID INT NOT NULL FOREIGN KEY REFERENCES Banco(BancoID)
		, UsoProveedores BIT NOT NULL
		, UsoClientes BIT NOT NULL
	)
	INSERT INTO BancoCuenta(NumeroDeCuenta, NombreDeCuenta, BancoID, UsoProveedores, UsoClientes) VALUES
		('02200647349', 'SCOTIABANK', 6, 1, 1)
		, ('70012621289', 'BANAMEX', 2, 0, 1)
		, ('6274670060133069', 'SCOTIALINE', 6, 0, 0)
		, ('4552550088909799', 'TARJETA CREDITO', 2, 0, 0)

	DECLARE @Const NVARCHAR(128) =
		(SELECT TOP 1 name FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('ProveedorPoliza') AND name LIKE '%_Cuent%')
	DECLARE @Com NVARCHAR(128) = 'ALTER TABLE ProveedorPoliza DROP CONSTRAINT ' + @Const
	EXEC (@Com)
	EXEC sp_rename 'ProveedorPoliza.CuentaBancariaID', 'BancoCuentaID', 'COLUMN'
	ALTER TABLE ProveedorPoliza ADD CONSTRAINT FK_BancoCuentaID FOREIGN KEY (BancoCuentaID) REFERENCES BancoCuenta(BancoCuentaID)
	DROP TABLE CuentaBancaria

	-- DROP TABLE BancoCuentaMovimiento
	CREATE TABLE BancoCuentaMovimiento (
		BancoCuentaMovimientoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, BancoCuentaID INT NULL FOREIGN KEY REFERENCES Banco(BancoID)
		, EsIngreso BIT NOT NULL
		, Fecha DATETIME NOT NULL
		, FechaAsignado DATETIME NULL
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		, Importe DECIMAL(12, 2) NOT NULL
		, Concepto NVARCHAR(64) NULL
		, Referencia NVARCHAR(16) NULL
		, TipoFormaPagoID INT NULL FOREIGN KEY REFERENCES TipoFormaPago(TipoFormaPagoID)
		, DatosDePago NVARCHAR(64) NULL
		, SaldoAcumulado DECIMAL(12, 2) NOT NULL
		, Conciliado BIT NOT NULL
		, RelacionID INT NULL
	)

	CREATE TABLE ContaPolizaError (
		ContaPolizaErrorID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, Fecha DATETIME NOT NULL
		, ContaTipoPolizaID INT NOT NULL FOREIGN KEY REFERENCES ContaTipoPoliza(ContaTipoPolizaID)
		, Concepto NVARCHAR(256) NOT NULL
		, RealizoUsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		, Detalle NVARCHAR(1024) NULL
	)

	-- Proveedores
	
	CREATE TABLE MovimientoInventarioDevolucionOrigen (
		MovimientoInventarioDevolucionOrigenID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, Origen NVARCHAR(32) NOT NULL
	)
	INSERT INTO MovimientoInventarioDevolucionOrigen (Origen) VALUES
		('DEVOLUCIÓN')
		, ('GARANTÍA')
		, ('DESCUENTO')

	ALTER TABLE MovimientoInventario ADD DevolucionOrigenID INT NULL
		FOREIGN KEY REFERENCES MovimientoInventarioDevolucionOrigen(MovimientoInventarioDevolucionOrigenID)

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

-- DROP VIEW BancosCuentasMovimientosView
CREATE VIEW BancosCuentasMovimientosView AS
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
		, bcm.RelacionID
	FROM
		BancoCuentaMovimiento bcm
		LEFT JOIN Sucursal s ON s.SucursalID = bcm.SucursalID AND s.Estatus = 1
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = bcm.TipoFormaPagoID AND tfp.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

