/* Script con modificaciones para el módulo de ventas. Archivo 25
 * Creado: 2014/02/17
 * Subido: 2014/02/23
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	CREATE TABLE ParteMaxMin (
		ParteMaxMinID INT NOT NULL IDENTITY(1, 1)
		, ParteID INT NOT NULL FOREIGN KEY REFERENCES Parte(ParteID)
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		, Maximo INT NULL
		, Minimo INT NULL
		, FechaCalculo DATETIME NULL
		, Calcular BIT NULL
		, VentasGlobales BIT NULL
		
		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME NULL
		
		, CONSTRAINT Ix_ParteID_SucursalID PRIMARY KEY (ParteID, SucursalID)
	)
	INSERT INTO ParteMaxMin (ParteID, SucursalID, UsuarioID)
		SELECT p.ParteID, s.SucursalID, 1 FROM Parte p LEFT JOIN Sucursal s ON 1 = 1

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

GO

CREATE VIEW PartesMaxMinView AS
	SELECT
		p.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, pmm.SucursalID
		, pmm.Calcular
		, pmm.VentasGlobales
		, pv.NombreProveedor AS Proveedor
		, mp.NombreMarcaParte AS Marca
		, l.NombreLinea AS Linea
	FROM
		Parte p
		LEFT JOIN ParteMaxMin pmm ON pmm.ParteID = p.ParteID
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
	/* ORDER BY
		pv.NombreProveedor
		, mp.NombreMarcaParte
		, l.NombreLinea
		, p.NombreParte
		
	*/
GO

/* *****************************************************************************
** Modificar procedimientos almacenados
***************************************************************************** */

