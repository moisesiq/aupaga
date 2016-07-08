/* Script con modificaciones para el módulo de ventas. Archivo 90
 * Creado: 2015/02/12
 * Subido: 2015/02/16
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	-- Orden de compra
	ALTER TABLE Venta ADD OrdenDeCompra NVARCHAR(64) NULL
	ALTER TABLE VentaPago DROP COLUMN Vale

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

ALTER VIEW [dbo].[VentasPagosView] AS
	SELECT
		vp.VentaPagoID
		, vp.VentaID
		, ISNULL(CONVERT(BIT, CASE WHEN vfdv.FolioFiscal IS NULL THEN 0 ELSE 1 END), 0) AS Facturada
		, v.Folio
		, vp.Fecha
		, ISNULL((SELECT SUM(Importe) FROM VentaPagoDetalle WHERE VentaPagoID = vp.VentaPagoID AND Estatus = 1), 0) AS Importe
		, vp.SucursalID
		, c.ClienteID
		, v.VentaEstatusID
		, ISNULL(v.ACredito, 0) AS ACredito
		-- , v.
		, c.Nombre AS Cliente
		, u.NombrePersona AS Vendedor
		, u.NombreUsuario AS VendedorUsuario
	FROM
		VentaPago vp
		LEFT JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN VentasFacturasDetalleView vfdv ON vfdv.VentaID = v.VentaID AND vfdv.EstatusGenericoID != 4
	WHERE vp.Estatus = 1
GO

ALTER VIEW [dbo].[VentasView] AS
	SELECT
		v.VentaID
		, ISNULL(CONVERT(BIT, CASE WHEN vfdv.FolioFiscal IS NULL THEN 0 ELSE 1 END), 0) AS Facturada
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
		LEFT JOIN VentasFacturasDetalleView vfdv ON vfdv.VentaID = v.VentaID -- AND vfdv.EstatusGenericoID != 4
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

ALTER VIEW [dbo].[ExistenciasView] AS
	SELECT
		ParteExistencia.ParteExistenciaID
		,ParteExistencia.ParteID
		,Parte.NumeroParte
		,ParteExistencia.SucursalID
		,Sucursal.NombreSucursal AS Tienda
		,ParteExistencia.Existencia AS Exist
		-- ,ParteExistencia.Maximo AS [Max]
		-- ,ParteExistencia.Minimo AS [Min]
		, ParteMaxMin.Maximo AS [Max]
		, ParteMaxMin.Minimo AS [Min]
		, Parte.UnidadEmpaque AS UEmp
	FROM
		ParteExistencia
		INNER JOIN Parte ON Parte.ParteID = ParteExistencia.ParteID AND Parte.Estatus = 1
		INNER JOIN Sucursal ON Sucursal.SucursalID = ParteExistencia.SucursalID AND Sucursal.Estatus = 1
		LEFT JOIN ParteMaxMin ON ParteMaxMin.ParteID = ParteExistencia.ParteID
			AND ParteMaxMin.SucursalID = ParteExistencia.SucursalID
	WHERE ParteExistencia.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

