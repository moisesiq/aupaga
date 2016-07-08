/* Script con modificaciones para el módulo de ventas. Archivo 19
 * Creado: 2014/01/07
 * Subido: 2014/01/08
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
		('Reportes.CajaResguardoTicket.Salida', 'D', 'I', 'Salida donde debe mostrarse el ticket de un resguardo de caja (D - Diseño, P - Pantalla, I - Impresora, N - Nada).')
		, ('Reportes.CajaRefuerzoTicket.Salida', 'D', 'I', 'Salida donde debe mostrarse el ticket de un refuerzo de caja (D - Diseño, P - Pantalla, I - Impresora, N - Nada).')

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

