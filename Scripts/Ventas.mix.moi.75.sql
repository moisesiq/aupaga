/* Script con modificaciones para el módulo de ventas. Archivo 75
 * Creado: 2014/12/22
 * Subido: 2014/12/23
 */


----------------------------------------------------------------------------------- Código de André

----------------------------------------------------------------------------------- Código de André


/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY
	
	ALTER TABLE Permiso ADD UNIQUE (NombrePermiso)

	IF (SELECT COUNT(*) FROM TipoMovimiento) = 3 BEGIN
		DBCC CHECKIDENT(TipoMovimiento, RESEED, 3)
		INSERT INTO TipoMovimiento (NombreTipoMovimiento, UsuarioID, FechaRegistro) VALUES ('AJUSTE', 1, GETDATE())
		INSERT INTO TipoOperacion (TipoMovimientoID, NombreTipoOperacion, UsuarioID, FechaRegistro) VALUES
			(4, 'AJUSTE KÁRDEX', 1, GETDATE())
	END ELSE BEGIN
		PRINT 'Error Faltal al verificar una tabla. Intervención manual requerida.'
	END

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

-- DROP VIEW PartesDiferenciasEnExistenciaView
CREATE VIEW PartesDiferenciasEnExistenciaView AS
	SELECT * FROM (
		SELECT
			pk.ParteID
			, pk.SucursalID
			, s.NombreSucursal AS Sucursal
			, p.NumeroParte as NumeroDeParte
			, p.NombreParte AS Descripcion
			, pk.ExistenciaNueva AS ExistenciaKardex
			, pe.Existencia
			, (pk.ExistenciaNueva - pe.Existencia) AS Diferencia
			, ROW_NUMBER() OVER (PARTITION BY pk.ParteID, pk.SucursalID ORDER BY pk.Fecha DESC) AS Orden
		 from
		  ParteKardex pk
		  LEFT JOIN ParteExistencia pe on pe.ParteID = pk.ParteID AND pe.SucursalID = pk.SucursalID AND pe.Estatus = 1
		  LEFT JOIN Parte p on p.ParteID = pk.ParteID AND p.Estatus = 1
		  LEFT JOIN Sucursal s ON s.SucursalID = pk.SucursalID AND s.Estatus = 1
	) c
	WHERE c.Orden = 1 AND c.ExistenciaKardex != c.Existencia
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

