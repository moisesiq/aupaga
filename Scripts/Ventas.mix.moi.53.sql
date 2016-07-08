/* Script con modificaciones para el módulo de ventas. Archivo 53
 * Creado: 2014/08/19
 * Subido: 2014/08/19
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	CREATE TABLE MetaSucursal (
		MetaSucursalID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		, UtilSucursal DECIMAL(12, 2) NOT NULL
		, UtilGerente DECIMAL(12, 2) NOT NULL
		, UtilVendedor DECIMAL(12, 2) NOT NULL
	)
	INSERT INTO MetaSucursal (SucursalID, UtilSucursal, UtilGerente, UtilVendedor) VALUES
		(1, 40000, 16000, 12000)
		, (2, 18000, 7200, 5400)
		, (3, 18000, 7200, 5400)
	
	DROP TABLE MetaGeneral

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

ALTER TABLE MetaVendedor ADD
	MetaConsiderar9500 BIT NOT NULL CONSTRAINT Df_MetaConsiderar9500 DEFAULT 0
	, EsGerente BIT NOT NULL CONSTRAINT Df_EsGerente DEFAULT 0
	, SueldoMinimo DECIMAL(12, 2) NOT NULL CONSTRAINT Df_SueldoMinimo DEFAULT 0
	, SueldoMeta DECIMAL(12, 2) NOT NULL CONSTRAINT Df_SueldoMeta DEFAULT 0
	, PorcentajeDeUtil DECIMAL(5, 2) NULL
	, IncrementoUtil DECIMAL(12, 2) NULL
	, IncrementoFijo DECIMAL(12, 2) NULL
	, Porcentaje9500 DECIMAL(5, 2) NULL
ALTER TABLE MetaVendedor DROP CONSTRAINT Df_MetaConsiderar9500, Df_EsGerente, Df_SueldoMinimo, Df_SueldoMeta
GO
UPDATE MetaVendedor SET SueldoMeta = SueldoFijo
ALTER TABLE MetaVendedor DROP COLUMN SueldoFijo

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

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

