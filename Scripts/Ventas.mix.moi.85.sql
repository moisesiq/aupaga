/* Script con modificaciones para el módulo de ventas. Archivo 85
 * Creado: 2015/02/01
 * Subido: 2015/02/01
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	ALTER TABLE ParteExistencia ALTER COLUMN FechaRegistro DATETIME NOT NULL
	ALTER TABLE ParteExistencia ALTER COLUMN FechaModificacion DATETIME NULL
	ALTER TABLE ParteExistencia ADD FOREIGN KEY (UsuarioID) REFERENCES Usuario(UsuarioID)
	
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
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

