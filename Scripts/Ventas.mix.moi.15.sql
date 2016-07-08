/* Script con modificaciones para el módulo de ventas. Archivo 15
 * Creado: 2013/12/17
 * Subido: 2013/12/18
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	CREATE UNIQUE INDEX Ix_Nombre_SucursalID ON Configuracion(Nombre, SucursalID)
	ALTER TABLE VentaFacturaDevolucion ADD
		Ack NVARCHAR(36) NULL
		, Procesada BIT NOT NULL CONSTRAINT Df_Procesada DEFAULT 0
		, FechaProcesada DATETIME
	ALTER TABLE VentaFacturaDevolucion DROP CONSTRAINT Df_Procesada

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

CREATE VIEW VentasFacturasView AS
	SELECT
		VentaFacturaID
		, Fecha
		, Serie + Folio AS SerieFolio
	FROM VentaFactura
	WHERE Estatus = 1
GO

/* *****************************************************************************
** Modificar procedimientos almacenados
***************************************************************************** */

