/* Script con modificaciones para el módulo de ventas. Archivo 13
 * Creado: 2013/12/12
 * Subido: 2013/12/12
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY
	-- Script	ALTER TABLE Cliente ADD Acumulado DECIMAL(12, 2) NULL
	
	/*
	INSERT INTO Permiso (NombrePermiso, MensajeDeError, FechaRegistro) VALUES
		('Ventas.Ingresos.Ver', 'No tienes permisos para configurar los Ingresos de Caja.', GETDATE())
		, ('Ventas.Egresos.Ver', 'No tienes permisos para configurar los Egresos de Caja.', GETDATE())
		, ('Ventas.FondoDeCaja.Ver', 'No tienes permisos para especificar el Fondo de Caja.', GETDATE())
		, ('Ventas.Cambios.Ver', 'No tienes permisos para cambiar las caracteríasticas de una venta.', GETDATE())
		, ('Ventas.Resguardo.Ver', 'No tienes permisos para hacer Resguardos.')
	*/
	
	CREATE INDEX Ix_NombrePermiso ON Permiso(NombrePermiso)

	ALTER TABLE Cliente DROP COLUMN ApellidoPaterno, ApellidoMaterno, Ciudad, Estado, ePoints
	ALTER TABLE ClienteFacturacion DROP COLUMN Localidad, Municipio, Estado
	
	ALTER TABLE VentaDetalle ALTER COLUMN Cantidad DECIMAL(9, 2) NOT NULL
	ALTER TABLE VentaDevolucionDetalle ALTER COLUMN Cantidad DECIMAL(9, 2) NOT NULL
	ALTER TABLE Cotizacion9500Detalle ALTER COLUMN Cantidad DECIMAL(9, 2) NOT NULL
	ALTER TABLE ReporteDeFaltante ALTER COLUMN CantidadRequerida DECIMAL(9, 2) NOT NULL
	
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

-- DROP VIEW ClientesFacturacionView
CREATE VIEW ClientesFacturacionView AS
	SELECT
		cf.ClienteFacturacionID
		, cf.ClienteID
		, cf.Rfc
		, cf.RazonSocial
		, cf.Calle
		, cf.NumeroExterior
		, cf.NumeroInterior
		, cf.Referencia
		, cf.Colonia
		, cf.CodigoPostal
		, cf.Pais
		, e.NombreEstado AS Estado
		, m.NombreMunicipio AS Municipio
		, c.NombreCiudad AS Localidad
	FROM
		ClienteFacturacion cf
		LEFT JOIN Estado e ON e.EstadoID = cf.EstadoID AND e.Estatus = 1
		LEFT JOIN Municipio m ON m.MunicipioID = cf.MunicipioID AND m.Estatus = 1
		LEFT JOIN Ciudad c ON c.CiudadID = cf.CiudadID AND c.Estatus = 1
	WHERE cf.Estatus = 1
GO

/**/
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
	,Cliente.Credito
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

ALTER VIEW Cotizaciones9500DetalleView AS
	SELECT
		c9d.Cotizacion9500DetalleID
		, c9d.Cotizacion9500ID
		, c9d.ProveedorID
		, c9d.LineaID
		, c9d.MarcaParteID
		, c9d.ParteID
		, p.NumeroParte
		, p.NombreParte
		, c9d.Cantidad
		, c9d.Costo
		, c9d.PrecioAlCliente
	FROM
		Cotizacion9500Detalle c9d
		LEFT JOIN Parte p ON p.ParteID = c9d.ParteID AND p.Estatus = 1
	WHERE
		c9d.Estatus = 1
GO

ALTER VIEW VentasDetalleView AS
	SELECT
		vd.VentaDetalleID
		, vd.VentaID
		, vd.ParteID
		, p.NumeroParte
		, p.NombreParte
		, vd.Cantidad
		, vd.PrecioUnitario
		, vd.Iva
		, m.NombreMedida AS Medida
	FROM
		VentaDetalle vd
		LEFT JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
		LEFT JOIN Medida m ON m.MedidaID = p.MedidaID AND p.Estatus = 1
	WHERE vd.Estatus = 1
GO

ALTER VIEW VentasDevolucionesDetalleView AS
	SELECT
		vdd.VentaDevolucionDetalleID
		, vdd.VentaDevolucionID
		, vdd.ParteID
		, p.NumeroParte
		, p.NombreParte
		, vdd.Cantidad
		, vdd.PrecioUnitario
		, vdd.Iva
		, m.NombreMedida
	FROM
		VentaDevolucionDetalle vdd
		LEFT JOIN Parte p ON p.ParteID = vdd.ParteID AND p.Estatus = 1
		LEFT JOIN Medida m ON m.MedidaID = p.MedidaID AND m.Estatus = 1
	WHERE vdd.Estatus = 1
GO

ALTER VIEW VentasDevolucionesView AS
	SELECT
		vd.VentaDevolucionID
		, ISNULL(CONVERT(BIT, CASE WHEN vfdv.FolioFiscal IS NULL THEN 0 ELSE 1 END), 0) AS Facturada
		, vd.VentaID
		, v.Folio
		, v.Fecha AS FechaDeVenta
		, vd.Fecha
		, vd.MotivoID
		, vdm.Descripcion AS Motivo
		, vd.Observacion
		, vd.SucursalID
		, vd.RealizoUsuarioID
		, u.NombrePersona AS Realizo
		, vd.EsCancelacion
		, vd.TipoFormaPagoID AS FormaDePagoID
		, tfp.NombreTipoFormaPago AS FormaDePago
		-- , CASE WHEN vd.EsCancelacion = 1 THEN 'CANC-' ELSE '    -DEV' END AS Tipo
		-- , CASE WHEN vd.TipoFormaPagoID = 2 THEN 'EF-' ELSE '  -NC' END AS Salida
		, ISNULL(a.Autorizado, 0) AS Autorizado
		, ISNULL(ua.NombreUsuario, 'PENDIENTE') AS AutorizoUsuario
		, CONVERT(DECIMAL(12, 2), vdd.Subtotal) AS Subtotal
		, CONVERT(DECIMAL(12, 2), vdd.Iva) AS Iva
		, CONVERT(DECIMAL(12, 2), vdd.Total) AS Total
	FROM
		VentaDevolucion vd
		LEFT JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
		LEFT JOIN VentaDevolucionMotivo vdm ON vdm.VentaDevolucionMotivoID = vd.MotivoID AND vdm.Estatus = 1
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = vd.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = vd.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN Autorizacion a ON a.Tabla = 'VentaDevolucion' AND a.TablaRegistroID = vd.VentaDevolucionID AND a.Estatus = 1
		LEFT JOIN Usuario ua ON ua.UsuarioID = a.AutorizoUsuarioID AND ua.Estatus = 1
		LEFT JOIN VentasFacturasDetalleView vfdv ON vfdv.VentaID = v.VentaID AND vfdv.EstatusGenericoID != 4
		LEFT JOIN (
			SELECT
				VentaDevolucionID
				, SUM(PrecioUnitario * Cantidad) AS Subtotal
				, SUM(Iva * Cantidad) AS Iva
				, SUM((PrecioUnitario + Iva) * Cantidad) AS Total
			FROM VentaDevolucionDetalle
			WHERE Estatus = 1
			GROUP BY VentaDevolucionID
		) vdd ON vdd.VentaDevolucionID = vd.VentaDevolucionID
	WHERE vd.Estatus = 1
GO

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
		LEFT JOIN VentasFacturasDetalleView vfdv ON vfdv.VentaID = v.VentaID AND vfdv.EstatusGenericoID != 4
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
** Modificar procedimientos almacenados
***************************************************************************** */

