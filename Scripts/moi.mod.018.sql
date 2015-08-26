/* Script con modificaciones a la base de datos de Theos. Archivo 018
 * Creado: 2015/06/08
 * Subido: 2015/05/09
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */



/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

ALTER VIEW [dbo].[Lista9500View] AS
	SELECT
		c9.Cotizacion9500ID
		, c9.EstatusGenericoID
		, eg.Descripcion AS Estatus
		, c9.Fecha
		, pv.NombreProveedor AS Proveedor
		, (l.NombreLinea + ' - ' + mp.NombreMarcaParte) AS LineaMarca
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, c9d.ParteID
		, c9d.Costo
		, c9d.PrecioAlCliente
		, c9.Anticipo
		, c.Nombre AS Cliente
		, c9.SucursalID
		, s.NombreSucursal AS Sucursal
		, c9.RealizoUsuarioID AS VendedorID
		, v.NombrePersona AS Vendedor
		, c9.BajaMotivo
		, ub.NombreUsuario AS BajaUsuario
	FROM
		Cotizacion9500 c9
		LEFT JOIN EstatusGenerico eg ON eg.EstatusGenericoID = c9.EstatusGenericoID
		LEFT JOIN (
			SELECT *
			FROM (
				SELECT
					c9d.Cotizacion9500ID
					, c9d.ProveedorID
					, c9d.MarcaParteID
					, c9d.LineaID
					, c9d.ParteID
					, c9d.Costo
					, c9d.PrecioAlCliente
					, ROW_NUMBER() OVER (PARTITION BY c9d.Cotizacion9500ID ORDER BY c9d.Cotizacion9500DetalleID) AS Numero
				FROM Cotizacion9500Detalle c9d
				WHERE c9d.Estatus = 1
			) c9d
			WHERE Numero = 1
		) c9d ON c9d.Cotizacion9500ID = c9.Cotizacion9500ID
		LEFT JOIN Proveedor pv ON pv.ProveedorID = c9d.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = c9d.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = c9d.LineaID AND l.Estatus = 1
		LEFT JOIN Parte p ON p.ParteID = c9d.ParteID AND p.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = c9.ClienteID AND c.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = c9.SucursalID AND s.Estatus = 1
		LEFT JOIN Usuario v ON v.UsuarioID = c9.RealizoUsuarioID AND v.Estatus = 1
		LEFT JOIN Usuario ub ON ub.UsuarioID = c9.BajaUsuarioID AND ub.Estatus = 1
	WHERE
		c9.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

