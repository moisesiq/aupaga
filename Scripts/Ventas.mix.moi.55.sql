/* Script con modificaciones para el módulo de ventas. Archivo 55
 * Creado: 2014/09/01
 * Subido: 2014/09/01
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	INSERT INTO Permiso (NombrePermiso, MensajeDeError) VALUES
		('Ventas.Comisiones.VerAdicional', 'No tienes permisos para ver datos adicionales en Comisiones.')

	ALTER TABLE MetaSucursal ADD UtilSucursalMinimo DECIMAL(12, 2) NOT NULL CONSTRAINT Df_UtilSucursalMinimo DEFAULT 0
	ALTER TABLE MetaSucursal DROP CONSTRAINT Df_UtilSucursalMinimo

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

GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

