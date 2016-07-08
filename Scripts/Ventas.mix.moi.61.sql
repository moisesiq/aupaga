/* Script con modificaciones para el módulo de ventas. Archivo 61
 * Creado: 2014/08/24
 * Subido: 2014/08/25
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	INSERT INTO Permiso (NombrePermiso, MensajeDeError) VALUES
		('Administracion.CatalogosPartes.Complementarias.Agregar', 'No tienes permisos para asignar Partes Complementarias.')
		, ('Administracion.CatalogosPartes.Complementarias.Eliminar', 'No tienes permisos para eliminar Partes Complementarias.')

	CREATE TABLE ParteComplementaria (
		ParteComplementariaID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, GrupoID INT NOT NULL
		, ParteID INT NOT NULL FOREIGN KEY REFERENCES Parte(ParteID)
		
		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME NULL
	)

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

CREATE VIEW PartesComplementariasView AS
	SELECT
		pc.ParteComplementariaID
		, pcr.ParteID
		, pc.ParteID AS ParteIDComplementaria
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, pi.NombreImagen
	FROM
		ParteComplementaria pc
		INNER JOIN ParteComplementaria pcr ON pcr.GrupoID = pc.GrupoID AND pcr.ParteID != pc.ParteID
		LEFT JOIN Parte p ON p.ParteID = pc.ParteID AND p.Estatus = 1
		LEFT JOIN ParteImagen pi ON pi.ParteID = pc.ParteID AND pi.Orden = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

