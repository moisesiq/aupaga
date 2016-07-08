/* Script con modificaciones para el módulo de ventas. Archivo 49
 * Creado: 2014/07/24
 * Subido: 2014/07/24
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	INSERT INTO Permiso (NombrePermiso, MensajeDeError) VALUES
		('CuadroDeControl.Acceso', 'No tienes permisos para acceder al módulo de Cuadro de Control.')

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

