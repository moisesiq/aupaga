/* Script con modificaciones para el módulo de ventas. Archivo 59
 * Creado: 2014/09/22
 * Subido: 2014/09/22
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	ALTER TABLE ContaCuenta ALTER COLUMN Cuenta NVARCHAR(64) NOT NULL
	ALTER TABLE ContaSubcuenta ALTER COLUMN Subcuenta NVARCHAR(64) NOT NULL
	ALTER TABLE ContaCuentaDeMayor ALTER COLUMN CuentaDeMayor NVARCHAR(64) NOT NULL
	ALTER TABLE ContaCuentaAuxiliar ALTER COLUMN CuentaAuxiliar NVARCHAR(64) NOT NULL
	
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
** Modificar tablas
***************************************************************************** */



/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */



/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

