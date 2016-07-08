/* Script con modificaciones para el módulo de ventas. Archivo 17
 * Creado: 2013/12/20
 * Subido: 2013/12/20
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	INSERT INTO Permiso (NombrePermiso, MensajeDeError, FechaRegistro) VALUES
		('Ventas.Comisiones.VerOtrosUsuarios', 'No tienes permisos ver las Comisiones de otros vendedores.', GETDATE())

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



/* *****************************************************************************
** Modificar procedimientos almacenados
***************************************************************************** */

