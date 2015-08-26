/* Script con modificaciones para el m�dulo de ventas. Archivo 8
*/

/* *****************************************************************************
** Creaci�n y modificaci�n de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	UPDATE Configuracion SET 
		Valor = CONVERT(NVARCHAR(8), CONVERT(INT, Valor))
		, ValorPredeterminado = '1'
	WHERE Nombre = 'Ventas.Folio'
	UPDATE Venta SET Folio = CONVERT(NVARCHAR(8), CONVERT(INT, Folio)) WHERE ISNUMERIC(Folio) = 1

	UPDATE CajaTipoIngreso SET NombreTipoIngreso = UPPER(NombreTipoIngreso)
	UPDATE CajaTipoEgreso SET NombreTipoEgreso = UPPER(NombreTipoEgreso)

	COMMIT TRAN
END TRY
BEGIN CATCH
	PRINT 'Hubo un error al ejecutar el script:'
	PRINT ERROR_MESSAGE()
	ROLLBACK TRAN
	RETURN
END CATCH

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
