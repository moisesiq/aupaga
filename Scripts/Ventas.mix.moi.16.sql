/* Script con modificaciones para el módulo de ventas. Archivo 16
 * Creado: 2013/12/18
 * Subido: 2013/12/18
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	

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

ALTER VIEW Lista9500View AS
	SELECT
		c9.Cotizacion9500ID AS Folio
		, c9.Fecha
		, pv.NombreProveedor
		, p.NumeroParte
		, p.NombreParte
		, c9d.Costo
		, c9d.PrecioAlCliente
		, c9.Anticipo
		, c.Nombre AS Cliente
		, s.NombreSucursal AS Sucursal
		, v.NombrePersona AS Vendedor
	FROM
		Cotizacion9500 c9
		LEFT JOIN (
			SELECT *
			FROM (
				SELECT
					Cotizacion9500ID
					, ProveedorID
					, ParteID
					, Costo
					, PrecioAlCliente
					, ROW_NUMBER() OVER (PARTITION BY Cotizacion9500ID ORDER BY Cotizacion9500DetalleID) AS Numero
				FROM Cotizacion9500Detalle c9d
				WHERE Estatus = 1
			) c9d
			WHERE Numero = 1
		) c9d ON c9d.Cotizacion9500ID = c9.Cotizacion9500ID
		LEFT JOIN Proveedor pv ON pv.ProveedorID = c9d.ProveedorID AND pv.Estatus = 1
		LEFT JOIN Parte p ON p.ParteID = c9d.ParteID AND p.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = c9.ClienteID AND c.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = c9.SucursalID AND s.Estatus = 1
		LEFT JOIN Usuario v ON v.UsuarioID = c9.RealizoUsuarioID AND v.Estatus = 1
	WHERE
		c9.Estatus = 1
		AND c9.EstatusGenericoID = 2
GO

/* *****************************************************************************
** Modificar procedimientos almacenados
***************************************************************************** */

