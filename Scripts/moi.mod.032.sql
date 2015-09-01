/* Script con modificaciones a la base de datos de Theos. Archivo 032
 * Creado: 2015/08/31
 * Subido: 2015/09/01
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

UPDATE VentaFacturaDetalle SET Primera = 1 WHERE VentaFacturaDetalleID IN (
SELECT VentaFacturaDetalleID FROM (
	SELECT ROW_NUMBER() OVER (PARTITION BY VentaID ORDER BY VentaFacturaDetalleID) AS Reg
		, VentaFacturaDetalleID FROM VentaFacturaDetalle
	) c WHERE Reg = 1
)

UPDATE ContaConfigAfectacion SET Operacion = 'Venta Contado Factura Directa' WHERE ContaConfigAfectacionID = 1
INSERT INTO ContaConfigAfectacion (Operacion, ContaTipoPolizaID) VALUES
	('Venta Contado Factura Convertida', 1)

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

CREATE VIEW Cotizaciones9500DetalleAvanzadoView AS
	SELECT
		c9d.Cotizacion9500DetalleID
		, c9.Cotizacion9500ID
		, c9.EstatusGenericoID AS Cotizacion9500EstatusID
		, c9d.ProveedorID
		, c9d.LineaID
		, c9d.MarcaParteID AS MarcaID
		, c9d.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, c9d.Cantidad
		, c9d.Costo
		, c9d.PrecioAlCliente
	FROM
		Cotizacion9500Detalle c9d
		LEFT JOIN Cotizacion9500 c9 ON c9.Cotizacion9500ID = c9d.Cotizacion9500ID AND c9.Estatus = 1
		LEFT JOIN Parte p ON p.ParteID = c9d.ParteID AND p.Estatus = 1
	WHERE c9d.Estatus = 1
GO

ALTER VIEW [dbo].[VentasView] AS
	SELECT
		v.VentaID
		, ISNULL(CONVERT(BIT, vf.VentaFacturaID), 0) AS Facturada
		, v.Folio
		, v.FolioIni
		, v.Fecha
		, v.ClienteID
		, c.Nombre AS Cliente
		, v.SucursalID
		, s.NombreSucursal AS Sucursal
		, v.VentaEstatusID
		, ve.Descripcion AS Estatus
		, ISNULL(CONVERT(DECIMAL(12, 2), vd.Subtotal), 0) AS Subtotal
		, ISNULL(CONVERT(DECIMAL(12, 2), vd.Iva), 0) AS Iva
		, ISNULL(CONVERT(DECIMAL(12, 2), vd.Total), 0) AS Total
		, ISNULL(CONVERT(DECIMAL(12, 2), vpt.Importe), 0) AS Pagado
		, v.ACredito
		, v.RealizoUsuarioID AS VendedorID
		, u.NombrePersona AS Vendedor
		, u.NombreUsuario AS VendedorUsuario
		, v.ComisionistaClienteID AS ComisionistaID
		, v.ClienteVehiculoID
		, v.Kilometraje
	FROM
		Venta v
		-- LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		LEFT JOIN VentaEstatus ve ON ve.VentaEstatusID = v.VentaEstatusID
		LEFT JOIN Sucursal s ON s.SucursalID = v.SucursalID AND s.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
		-- LEFT JOIN VentasFacturasDetalleView vfdv ON vfdv.VentaID = v.VentaID -- AND vfdv.EstatusGenericoID != 4
		LEFT JOIN VentaFacturaDetalle vfd ON vfd.VentaID = v.VentaID AND vfd.Primera = 1 AND vfd.Estatus = 1
		LEFT JOIN VentaFactura vf ON vf.VentaFacturaID = vfd.VentaFacturaID AND vf.Estatus = 1
		LEFT JOIN (
			SELECT
				VentaID
				, SUM(PrecioUnitario * Cantidad) AS Subtotal
				, SUM(Iva * Cantidad) AS Iva
				, SUM((PrecioUnitario + Iva) * Cantidad) AS Total
			FROM VentaDetalle
			WHERE Estatus = 1
			GROUP BY VentaID
		) vd ON vd.VentaID = v.VentaID
		LEFT JOIN (
			SELECT
				vp.VentaID
				, SUM(vpd.Importe) AS Importe
			FROM
				VentaPago vp
				LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
			WHERE vp.Estatus = 1
			GROUP BY vp.VentaID
		) vpt ON vpt.VentaID = v.VentaID
		-- LEFT JOIN VentaPago vp ON vp.VentaID = v.VentaID AND vp.Estatus = 1
		-- LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
	WHERE v.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

