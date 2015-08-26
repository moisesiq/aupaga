--1 view MovimientoInventarioView
--2 view MovimientoInventarioDetalleView
--3 view MovimientoInventarioDescuentoView

/* 1 */
CREATE VIEW MovimientoInventarioView
AS
SELECT
	MovimientoInventario.MovimientoInventarioID
	,MovimientoInventario.TipoOperacionID
	,TipoOperacion.NombreTipoOperacion
	,MovimientoInventario.TipoPagoID	
	,TipoPago.NombreTipoPago
	,MovimientoInventario.ProveedorID	
	,Proveedor.NombreProveedor
	,MovimientoInventario.SucursalOrigenID	
	,s1.NombreSucursal AS SucursalOrigen
	,MovimientoInventario.SucursalDestinoID	
	,s2.NombreSucursal AS SucursalDestino
	,MovimientoInventario.FechaFactura	
	,MovimientoInventario.FechaRecepcion	
	,MovimientoInventario.FolioFactura	
	,MovimientoInventario.AplicaEnMovimientoInventarioID	
	,MovimientoInventario.FechaAplicacion	
	,MovimientoInventario.Subtotal	
	,MovimientoInventario.IVA	
	,MovimientoInventario.ImporteTotal	
	,MovimientoInventario.FueLiquidado	
	,MovimientoInventario.ConceptoMovimiento	
	,MovimientoInventario.Observacion	
	,MovimientoInventario.UsuarioID	
	,Usuario.NombreUsuario
	,MovimientoInventario.FechaRegistro	
	,MovimientoInventario.FechaModificacion	
	,MovimientoInventario.NombreImagen	
FROM
	MovimientoInventario
	INNER JOIN Proveedor ON Proveedor.ProveedorID = MovimientoInventario.ProveedorID
	INNER JOIN TipoOperacion ON TipoOperacion.TipoOperacionID = MovimientoInventario.TipoOperacionID
	INNER JOIN TipoPago ON TipoPago.TipoPagoID = MovimientoInventario.TipoPagoID
	INNER JOIN Sucursal s1 ON s1.SucursalID = MovimientoInventario.SucursalOrigenID
	INNER JOIN Sucursal s2 ON s2.SucursalID = MovimientoInventario.SucursalDestinoID
	INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioID
WHERE
	MovimientoInventario.Estatus = 1
GO

/* 2 */
CREATE VIEW MovimientoInventarioDetalleView
AS
SELECT 
	MovimientoInventarioDetalle.MovimientoInventarioDetalleID
	,MovimientoInventarioDetalle.MovimientoInventarioID
	,MovimientoInventarioDetalle.ParteID
	,Parte.NumeroParte
	,Parte.NombreParte
	,MovimientoInventarioDetalle.Cantidad
	,MovimientoInventarioDetalle.PrecioUnitario
	,MovimientoInventarioDetalle.Importe
	,MovimientoInventarioDetalle.FueDevolucion
	,MovimientoInventarioDetalle.FechaRegistro
FROM 
	MovimientoInventarioDetalle
	INNER JOIN Parte ON Parte.ParteID = MovimientoInventarioDetalle.ParteID
WHERE
	MovimientoInventarioDetalle.Estatus = 1
GO

/* 3 */
CREATE VIEW MovimientoInventarioDescuentoView
AS
SELECT 
	MovimientoInventarioDescuento.MovimientoInventarioDescuentoID
	,MovimientoInventarioDescuento.MovimientoInventarioID
	,MovimientoInventarioDescuento.TipoDescuentoID
	,TipoDescuento.NombreTipoDescuento
	,MovimientoInventarioDescuento.ParteID
	,Parte.NumeroParte
	,Parte.NombreParte
	,MovimientoInventarioDescuento.DescuentoUno
	,MovimientoInventarioDescuento.DescuentoDos
	,MovimientoInventarioDescuento.DescuentoTres
	,MovimientoInventarioDescuento.DescuentoCuatro
	,MovimientoInventarioDescuento.DescuentoCinco
FROM 
	MovimientoInventarioDescuento
	INNER JOIN Parte ON Parte.ParteID = MovimientoInventarioDescuento.ParteID
	INNER JOIN TipoDescuento ON TipoDescuento.TipoDescuentoID = MovimientoInventarioDescuento.TipoDescuentoID
WHERE
	MovimientoInventarioDescuento.Estatus = 1
GO