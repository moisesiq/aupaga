/* Script con modificaciones para el módulo de ventas. Archivo 22
 * Creado: 2014/01/21
 * Subido: 2014/01/21
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	ALTER TABLE Cliente DROP COLUMN Credito

	ALTER TABLE Cotizacion9500 ADD VentaID INT NULL FOREIGN KEY REFERENCES Venta(VentaID)

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

ALTER VIEW VentasView AS
	SELECT
		v.VentaID
		, ISNULL(CONVERT(BIT, CASE WHEN vfdv.FolioFiscal IS NULL THEN 0 ELSE 1 END), 0) AS Facturada
		, v.Folio
		, v.Fecha
		, v.ClienteID
		, c.Nombre AS Cliente
		, v.SucursalID
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
	FROM
		Venta v
		-- LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		LEFT JOIN VentaEstatus ve ON ve.VentaEstatusID = v.VentaEstatusID AND ve.Estatus = 1
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

ALTER VIEW ClientesCreditoView AS
	SELECT
		c.ClienteID
		, c.Nombre
		, c.TieneCredito
		, c.LimiteCredito AS LimiteDeCredito
		, c.DiasDeCredito
		, c.Tolerancia
		, ca.Adeudo
		, ca.FechaPrimerAdeudo
		, ISNULL(CONVERT(BIT, CASE WHEN (ca.Adeudo >= c.LimiteCredito OR ca.FechaPrimerAdeudo >= (GETDATE() + c.DiasDeCredito))
			THEN 1 ELSE 0 END), 0) AS CreditoVencido
	FROM
		Cliente c
		LEFT JOIN (
			SELECT
				ClienteID
				, MIN(Fecha) AS FechaPrimerAdeudo
				, SUM(Total - Pagado) AS Adeudo
			FROM VentasView
			WHERE ACredito = 1 AND (Total - Pagado) > 0 AND VentaEstatusID = 2
			GROUP BY ClienteID
		) ca ON ca.ClienteID = c.ClienteID
GO

ALTER VIEW ClientesDatosView AS
	SELECT
		c.ClienteID
		, c.Nombre
		, c.Calle + ' ' + c.NumeroExterior
			+ CASE WHEN ISNULL(c.NumeroInterior, '') = '' THEN '' ELSE ' - ' END
			+ ISNULL(c.NumeroInterior, '') AS Direccion
		, c.Colonia
		, c.CodigoPostal
		, m.NombreMunicipio AS Municipio
		, cd.NombreCiudad AS Ciudad
		, e.NombreEstado AS Estado
		, c.Telefono
		, c.Celular
		, cf.Rfc
		, c.TipoFormaPagoID
		, c.BancoID
		, c.CuentaBancaria
		, c.ListaDePrecios
		, c.TieneCredito
		, c.LimiteCredito AS LimiteDeCredito
		, c.Tolerancia
	FROM
		Cliente c
		LEFT JOIN Municipio m ON m.MunicipioID = c.MunicipioID AND m.Estatus = 1
		LEFT JOIN Ciudad cd ON cd.CiudadID = c.CiudadID AND cd.Estatus = 1
		LEFT JOIN Estado e ON e.EstadoID = c.EstadoID AND e.Estatus = 1
		LEFT JOIN ClienteFacturacion cf ON cf.ClienteID = c.ClienteID AND cf.Estatus = 1
	WHERE c.Estatus = 1
GO

ALTER VIEW [dbo].[ClientesView]
AS
SELECT
	Cliente.ClienteID
	,ClienteFacturacion.Rfc
	,Cliente.Nombre
	,Cliente.Calle
	,Cliente.NumeroInterior
	,Cliente.NumeroExterior
	,Cliente.Colonia
	,Cliente.CodigoPostal
	,Cliente.EstadoID
	,Estado.NombreEstado
	,Cliente.MunicipioID
	,Municipio.NombreMunicipio
	,Cliente.CiudadID
	,Ciudad.NombreCiudad
	,Cliente.Telefono
	,Cliente.Celular
	,Cliente.Particular
	,Cliente.Nextel
	,Cliente.ListaDePrecios
	,ISNULL(Cliente.TieneCredito, 0) AS TieneCredito
	-- ,Cliente.Credito
	,Cliente.DiasDeCredito
	,Cliente.TipoFormaPagoID
	,TipoFormaPago.NombreTipoFormaPago
	,Cliente.BancoID
	,Banco.NombreBanco
	,Cliente.TipoClienteID
	,TipoCliente.NombreTipoCliente
	,Cliente.CuentaBancaria
	,ISNULL(Cliente.Tolerancia, 0) AS Tolerancia
	,Cliente.LimiteCredito
	,ISNULL(Cliente.EsClienteComisionista, 0) AS EsClienteComisionista
	,ISNULL(Cliente.EsTallerElectrico, 0) AS EsTallerElectrico
	,ISNULL(Cliente.EsTallerMecanico, 0) AS EsTallerMecanico
	,ISNULL(Cliente.EsTallerDiesel, 0) AS EsTallerDiesel
	,Cliente.ClienteComisionistaID
	,Com.Nombre AS NombreClienteComisionista
	,Cliente.FechaRegistro
	,Cliente.Alias
	,dbo.fnuClienteCreditoDeuda(Cliente.ClienteID, 1) AS DeudaActual
	,dbo.fnuClienteCreditoDeuda(Cliente.ClienteID, 2) AS DeudaVencido
	,dbo.fnuClientePromedioPago(Cliente.ClienteID, 1) AS PromedioPagoTotal
	,dbo.fnuClientePromedioPago(Cliente.ClienteID, 2) AS PromedioPagoTresMeses
	,Cliente.Acumulado AS AcumuladoH
	,dbo.fnuClienteAcumulado(Cliente.ClienteID) AS Acumulado
FROM
	Cliente
	LEFT JOIN ClienteFacturacion ON ClienteFacturacion.ClienteID = Cliente.ClienteID
	INNER JOIN Estado ON Estado.EstadoID = Cliente.EstadoID
	INNER JOIN Municipio ON Municipio.MunicipioID = Cliente.MunicipioID
	INNER JOIN Ciudad ON Ciudad.CiudadID = Cliente.CiudadID
	LEFT JOIN TipoFormaPago ON TipoFormaPago.TipoFormaPagoID = Cliente.TipoFormaPagoID
	LEFT JOIN Banco ON Banco.BancoID = Cliente.BancoID
	LEFT JOIN TipoCliente ON TipoCliente.TipoClienteID = Cliente.TipoClienteID
	LEFT JOIN Cliente Com ON Com.ClienteComisionistaID = Cliente.ClienteID
WHERE
	Cliente.Estatus = 1
GO

/* *****************************************************************************
** Modificar procedimientos almacenados
***************************************************************************** */

