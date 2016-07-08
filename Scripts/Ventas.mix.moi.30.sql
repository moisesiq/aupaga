/* Script con modificaciones para el m�dulo de ventas. Archivo 30
 * Creado: 2014/02/14
 * Subido: 2014/03/14
 */

/* *****************************************************************************
** Creaci�n y modificaci�n de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	ALTER TABLE VentaDevolucionDetalle ALTER COLUMN PrecioUnitario DECIMAL(14, 4) NOT NULL
	ALTER TABLE VentaDevolucionDetalle ALTER COLUMN Iva DECIMAL(14, 4) NOT NULL
	
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

