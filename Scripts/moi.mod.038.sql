/* Script con modificaciones a la base de datos de Theos. Archivo 038
 * Creado: 2015/10/09
 * Subido: 2015/10/09
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

CREATE TABLE Duenio (
	DuenioID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, Duenio NVARCHAR(128) NOT NULL
)
INSERT INTO Duenio (Duenio) VALUES ('DON ISIDRO'), ('ISIDRO')

CREATE TABLE ContaEgresoDevengadoEspecial (
	ContaEgresoDevengadoEspecialID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, ContaEgresoID INT NOT NULL FOREIGN KEY REFERENCES ContaEgreso(ContaEgresoID)
	, Fecha DATETIME NOT NULL
	, DuenioID INT NOT NULL FOREIGN KEY REFERENCES Duenio(DuenioID)
	, Importe DECIMAL(12, 2) NOT NULL
)

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

CREATE VIEW ContaEgresosDevengadoEspecialCuentasView AS
	SELECT
		ede.ContaEgresoDevengadoEspecialID
		, ede.ContaEgresoID
		, ede.DuenioID
		, d.Duenio
		, cc.ContaCuentaID
		, cc.Cuenta
		, cs.ContaSubcuentaID
		, cs.Subcuenta
		, ccm.ContaCuentaDeMayorID
		, ccm.CuentaDeMayor
		, cca.ContaCuentaAuxiliarID
		, cca.CuentaAuxiliar
		, ede.Fecha
		, ede.Importe AS ImporteDev
	FROM
		ContaEgresoDevengadoEspecial ede
		INNER JOIN ContaEgreso ce ON ce.ContaEgresoID = ede.ContaEgresoID
		INNER JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = ce.ContaCuentaAuxiliarID
		LEFT JOIN Duenio d ON d.DuenioID = ede.DuenioID
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = cca.ContaCuentaDeMayorID
		LEFT JOIN ContaSubcuenta cs ON cs.ContaSubcuentaID = ccm.ContaSubcuentaID
		LEFT JOIN ContaCuenta cc ON cc.ContaCuentaID = cs.ContaCuentaID
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

