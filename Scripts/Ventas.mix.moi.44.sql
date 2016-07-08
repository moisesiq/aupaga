/* Script con modificaciones para el m�dulo de ventas. Archivo 44
 * Creado: 2014/05/30
 * Subido: 2014/05/30
 */

/* *****************************************************************************
** Creaci�n y modificaci�n de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	INSERT INTO Permiso (NombrePermiso, MensajeDeError) VALUES
		('Ventas.Ticket.EditarDescripcionPartes', 'No tienes permisos para editar las descripciones de los art�culos para el Ticket.')
		, ('Ventas.Venta.EditarPreciosLibre', 'No tienes permisos para editar los precios de venta fuera del rango de precios.')

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

