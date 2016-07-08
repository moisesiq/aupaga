/* Script con modificaciones a la base de datos de Theos. Archivo 006
 * Creado: 2015/05/01
 * Subido: 2015/05/05
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

-- Nómina

CREATE TABLE Nomina (
	NominaID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, Semana DATE NOT NULL
	, Fecha DATETIME NOT NULL
	, BancoCuentaID INT NOT NULL FOREIGN KEY REFERENCES BancoCuenta(BancoCuentaID)
)

-- DROP TABLE NominaUsuario
CREATE TABLE NominaUsuario (
	NominaUsuarioID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, NominaID INT NOT NULL FOREIGN KEY REFERENCES Nomina(NominaID)
	, IdUsuario INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
	-- , TotalOficial DECIMAL(12, 2) NOT NULL
	, SueldoFijo DECIMAL(12, 2) NOT NULL
	, SueldoVariable DECIMAL(12, 2) NOT NULL
	, Sueldo9500 DECIMAL(12, 2) NOT NULL
	, SueldoMinimo DECIMAL(12, 2) NOT NULL
	, Bono DECIMAL(12, 2) NOT NULL
	, Adicional DECIMAL(12, 2) NOT NULL
	-- , TotalSueldo DECIMAL(12, 2) NOT NULL
	, Tickets DECIMAL(12, 2) NOT NULL
	, Adelanto DECIMAL(12, 2) NOT NULL
	, MinutosTarde DECIMAL(12, 2) NOT NULL
	, Otros DECIMAL(12, 2) NOT NULL
	-- , TotalDescuentos DECIMAL(12, 2) NOT NULL
	-- , Liquido DECIMAL(12, 2) NOT NULL
)

-- DROP TABLE NominaUsuarioOficial
CREATE TABLE NominaUsuarioOficial (
	NominaUsuarioOficialID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, NominaID INT NOT NULL FOREIGN KEY REFERENCES Nomina(NominaID)
	, IdUsuario INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	, ContaCuentaDeMayorID INT NOT NULL FOREIGN KEY REFERENCES ContaCuentaDeMayor(ContaCuentaDeMayorID)
	, Importe DECIMAL(12, 2) NOT NULL
	, Suma BIT NOT NULL
)

DELETE FROM NominaOficialCuenta
ALTER TABLE NominaOficialCuenta ADD
	ContaCuentaDeMayorID INT NOT NULL FOREIGN KEY REFERENCES ContaCuentaDeMayor(ContaCuentaDeMayorID)
EXEC pauBorrarLlaveForanea 'NominaOficialCuenta', '_NominaOfi_'
ALTER TABLE NominaOficialCuenta DROP COLUMN ContaCuentaAuxiliarID

DELETE FROM UsuarioNominaOficial
ALTER TABLE UsuarioNominaOficial ADD
	ContaCuentaDeMayorID INT NOT NULL FOREIGN KEY REFERENCES ContaCuentaDeMayor(ContaCuentaDeMayorID)
EXEC pauBorrarLlaveForanea 'UsuarioNominaOficial', '_UsuarioNo_'
ALTER TABLE UsuarioNominaOficial DROP COLUMN ContaCuentaAuxiliarID

DELETE FROM UsuarioNomina
ALTER TABLE UsuarioNomina ADD SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)

INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
	('Reportes.Nomina.Salida', 'D', 'I', 'Salida donde debe mostrarse el la nómina de cada usuario (D - Diseño, P - Pantalla, I - Impresora).')

-- Contabilidad
INSERT INTO ContaConfigAfectacion (Operacion, ContaTipoPolizaID) VALUES
	('Nómina oficial', 1)

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO

-- DROP VIEW NominaUsuariosView
CREATE VIEW NominaUsuariosView AS
	SELECT
		nu.NominaUsuarioID
		, n.NominaID
		, n.Semana
		, n.BancoCuentaID
		, nu.IdUsuario AS UsuarioID
		, u.NombreUsuario AS Usuario
		, nu.SucursalID
		, s.NombreSucursal AS Sucursal
		, nuoc.Total AS TotalOficial
		, nu.SueldoFijo
		, nu.SueldoVariable
		, nu.Sueldo9500
		, nu.SueldoMinimo
		, nu.Bono
		, nu.Adicional
		, (nu.SueldoFijo + nu.SueldoVariable + nu.Sueldo9500 + nu.SueldoMinimo + nu.Bono + nu.Adicional) AS TotalSueldo
		, ((nu.SueldoFijo + nu.SueldoVariable + nu.Sueldo9500 + nu.SueldoMinimo + nu.Bono + nu.Adicional) - nuoc.Total)
			AS Diferencia
		, nu.Tickets
		, nu.Adelanto
		, nu.MinutosTarde
		, nu.Otros
		, (nu.Tickets + nu.Adelanto + nu.MinutosTarde + nu.Otros) AS TotalDescuentos
		, (((nu.SueldoFijo + nu.SueldoVariable + nu.Sueldo9500 + nu.SueldoMinimo + nu.Bono + nu.Adicional) - nuoc.Total)
			- (nu.Tickets + nu.Adelanto + nu.MinutosTarde + nu.Otros)) AS Liquido
	FROM
		NominaUsuario nu
		LEFT JOIN Nomina n ON n.NominaID = nu.NominaID
		LEFT JOIN (
			SELECT
				NominaID
				, IdUsuario
				, SUM(CASE WHEN Suma = 1 THEN Importe ELSE Importe * -1 END) AS Total
			FROM NominaUsuarioOficial
			GROUP BY
				NominaID
				, IdUsuario
		) nuoc ON nuoc.IdUsuario = nu.IdUsuario AND nuoc.NominaID = nu.NominaID
		LEFT JOIN Usuario u ON u.UsuarioID = nu.IdUsuario AND u.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = nu.SucursalID AND s.Estatus = 1
GO

CREATE VIEW NominaUsuariosOficialView AS
	SELECT
		nuo.NominaUsuarioOficialID
		, nuo.NominaID
		, nuo.IdUsuario AS UsuarioID
		, nuo.ContaCuentaDeMayorID
		, ccm.CuentaDeMayor
		, nuo.Importe
		, nuo.Suma
	FROM
		NominaUsuarioOficial nuo
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = nuo.ContaCuentaDeMayorID
GO

ALTER VIEW [dbo].[NominaOficialCuentasView] AS
	SELECT
		noc.NominaOficialCuentaID
		, noc.ContaCuentaDeMayorID
		, ccm.CuentaDeMayor
		, noc.Suma
	FROM
		NominaOficialCuenta noc
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = noc.ContaCuentaDeMayorID
GO

ALTER VIEW [dbo].[UsuariosNominaView] AS
	SELECT
		un.UsuarioNominaID
		, un.IdUsuario AS UsuarioID
		, u.NombreUsuario AS Usuario
		, un.SucursalID
		, s.NombreSucursal AS Sucursal
		, un.SueldoFijo
	FROM
		UsuarioNomina un
		INNER JOIN Usuario u ON u.UsuarioID = un.IdUsuario AND u.Activo = 1 AND u.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = un.SucursalID AND s.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

