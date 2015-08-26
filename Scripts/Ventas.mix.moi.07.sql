/* Script con modificaciones para el módulo de ventas. Archivo 7
*/

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	ALTER TABLE Venta ADD ACredito BIT NOT NULL DEFAULT 0

	UPDATE VentaEstatus SET Descripcion = 'Realizada' WHERE VentaEstatusID = 1
	UPDATE VentaEstatus SET Descripcion = 'Cobrada' WHERE VentaEstatusID = 2
	UPDATE VentaEstatus SET Descripcion = 'Pagada' WHERE VentaEstatusID = 3
	UPDATE VentaEstatus SET Descripcion = 'Cancelada' WHERE VentaEstatusID = 4
	UPDATE Venta SET VentaEstatusID =
		CASE VentaEstatusID
			WHEN 1 THEN 3
			WHEN 2 THEN 4
			WHEN 3 THEN 1
		END

	-- Se borra un contraint
	DECLARE @Constraint NVARCHAR(256) = (SELECT name FROM sys.foreign_keys
		WHERE parent_object_id = object_id('VentaPago') AND name LIKE 'FK__VentaPago__TipoP__%')
	DECLARE @Com NVARCHAR(512) = 'ALTER TABLE VentaPago DROP CONSTRAINT ' + @Constraint
	EXEC (@Com)
	--
	ALTER TABLE VentaPago DROP COLUMN TipoPagoID

	EXEC sp_rename 'VentaFactura.Identificador', 'FolioFiscal', 'COLUMN'
	-- Se borra un contraint
	SET @Constraint = (SELECT name FROM sys.foreign_keys
		WHERE parent_object_id = object_id('VentaFactura') AND name LIKE 'FK__VentaFact__Venta__%')
	SET @Com = 'ALTER TABLE VentaFactura DROP CONSTRAINT ' + @Constraint
	EXEC (@Com)
	--
	ALTER TABLE VentaFactura DROP COLUMN VentaID, Factura
	ALTER TABLE VentaFactura ADD
		Serie NVARCHAR(4) NULL
		, Folio NVARCHAR(8) NULL
		, EstatusGenericoID INT NOT NULL FOREIGN KEY REFERENCES EstatusGenerico(EstatusGenericoID)
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		, RealizoUsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, Observacion NVARCHAR(512) NULL

	ALTER TABLE NotaDeCredito ADD
		OrigenVentaID INT NULL FOREIGN KEY REFERENCES Venta(VentaID)
		, UsoVentaID INT NULL FOREIGN KEY REFERENCES Venta(VentaID)

	CREATE TABLE VentaFacturaDetalle (
		VentaFacturaDetalleID INT NOT NULL IDENTITY(1 ,1) PRIMARY KEY
		, VentaFacturaID INT NOT NULL FOREIGN KEY REFERENCES VentaFactura(VentaFacturaID)
		, VentaID INT NOT NULL FOREIGN KEY REFERENCES Venta(VentaID)
		
		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME
		, Estatus BIT NOT NULL DEFAULT 1
		, Actualizar BIT NOT NULL DEFAULT 1
	)	

	DROP TABLE FacturacionConfig

	INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
		('Facturacion.RutaPdf', 'Archivos\Facturas\', 'Archivos\Facturas\', 'Ruta donde se almacenan los archivos Pdf correspondientes a las facturas electrónicas.')
		, ('Facturacion.RutaXml', 'Archivos\Facturas\', 'Archivos\Facturas\', 'Ruta donde se almacenan los archivos Xml correspondientes a las facturas electrónicas.')
		, ('Comisiones.DiaInicial', '6', '6', 'Día de la semana inicial para el cálculo de comisiones de vendedores.')
		, ('Comisiones.DiaFinal', '5', '5', 'Día de la semana final para el cálculo de comisiones de vendedores.')
		, ('Comisiones.Vendedor.Porcentaje', '11', '11', 'Porcentaje de comisión sobre la utilidad que se le da a los vendedores por producto vendido.')
		, ('Comisiones.Vendedor.SueldoFijo', '500', '500', 'Sueldo fijo de los vendedores.')

	INSERT INTO AutorizacionProceso (Descricpion, UsuarioID) VALUES
		('CORTE DE CAJA', 1)
		, ('NOTA DE CRÉDITO', 1)

	INSERT INTO ClienteFacturacion (ClienteID, Rfc, RazonSocial, Calle, CodigoPostal, Municipio, Estado, Pais, UsuarioID) VALUES
		(1, 'MOVT790416R40', 'VENTAS MOSTRADOR', 'Una Calle', '43210', 'CIUDAD GUZMÁN', 'JALISCO', 'MÉXICO', 1)

	COMMIT TRAN
END TRY
BEGIN CATCH
	PRINT 'Hubo un error al ejecutar el script:'
	PRINT ERROR_MESSAGE()
	ROLLBACK TRAN
	RETURN
END CATCH

/* *****************************************************************************
** Modificar tablas
***************************************************************************** */

ALTER TABLE Venta ADD Folio NVARCHAR(16) NULL
GO
UPDATE Venta SET Folio = dbo.fnuLlenarIzquierda(CONVERT(NVARCHAR(7), VentaID), '0', 7)

ALTER TABLE Configuracion ADD SucursalID INT NOT NULL DEFAULT 0
GO
DECLARE @UltID INT = (SELECT MAX(ConfiguracionID) FROM Configuracion)
INSERT INTO Configuracion (SucursalID, Nombre, Valor, Descripcion) VALUES
	(1, 'Ventas.Folio', '0000001', 'Siguiente folio a utilizar al registrar una venta no facturada.')
	, (1, 'Facturacion.Serie', 'MAT', 'Serie a utilizar al facturar una venta.')
	, (1, 'Facturacion.Folio', '1', 'Siguiente folio a utilizar al facturar una venta.')
	, (1, 'Facturacion.RutaCertificado', 'FacturacionElectronica\00001000000300932753.cer', 'Ruta del archivo de certificado (.cer) para facturación electrónica.')
	, (1, 'Facturacion.RutaArchivoKey', 'FacturacionElectronica\CSD_Cd._Guzman_GARL540703AL7_20130927_104352.key', 'Ruta del archivo de llave (.key) para facturación electrónica.')
	, (1, 'Facturacion.ContraseniaArchivoKey', 'Lucre2807', 'Contraseña del archivo de llave (.key). para facturación electrónica.')
	, (1, 'Facturacion.RutaArchivoPfx', 'FacturacionElectronica\Cancelacion.pfx', 'Ruta del archivo tipo .pfx para facturación electrónica.')
	, (1, 'Facturacion.ContraseniaArchivoPfx', '7082ercuL', 'Contraseña del archivo tipo .pfx para facturación electrónica.')
	, (1, 'Facturacion.UsuarioPac', 'GARL540703AL7', 'Usuario para acceder al servicio del PAC. Para facturación electrónica.')
	, (1, 'Facturacion.ContraseniaPac', 'neggwmxip', 'Contraseña para acceder al servicio del PAC. Para facturación electrónica.')
	, (1, 'Facturacion.Rfc', 'GARL540703AL7', 'RFC del emisor para la facturación electrónica.')
	, (1, 'Facturacion.RazonSocial', 'Autopartes Garibaldi', 'Razón social del emisor para la facturación electrónica.')
UPDATE Configuracion SET ValorPredeterminado = Valor WHERE ConfiguracionID > @UltID
UPDATE Configuracion SET
	Valor = dbo.fnuLlenarIzquierda(CONVERT(NVARCHAR(8), (SELECT MAX(VentaID) + 1 FROM Venta)), '0', 7)
WHERE Nombre = 'Ventas.Folio'
INSERT INTO Configuracion (SucursalID, Nombre, Valor, Descripcion) VALUES
	(1, 'Facturacion.RegimenesFiscales', 'No sé', 'Regimenes fiscales del emisor. Para la facturación electrónica.')
	, (1, 'Facturacion.Calle', 'Una Calle', 'Calle de domicilio fiscal del emisor. Para la facturación electrónica.')
	, (1, 'Facturacion.NumeroExterior', '35', 'Número exterior de domicilio fiscal del emisor. Para la facturación electrónica.')
	, (1, 'Facturacion.NumeroInterior', 'E', 'Número interior de domicilio fiscal del emisor. Para la facturación electrónica.')
	, (1, 'Facturacion.Referencia', 'Algo', 'Referencia de domicilio fiscal del emisor. Para la facturación electrónica.')
	, (1, 'Facturacion.Colonia', 'Una Colonia', 'Colonia de domicilio fiscal del emisor. Para la facturación electrónica.')
	, (1, 'Facturacion.CodigoPostal', '12345', 'Código postal de domicilio fiscal del emisor. Para la facturación electrónica.')
	, (1, 'Facturacion.Localidad', 'La Loca', 'Localidad de domicilio fiscal del emisor. Para la facturación electrónica.')
	, (1, 'Facturacion.Municipio', 'El Muni', 'Municipio de domicilio fiscal del emisor. Para la facturación electrónica.')
	, (1, 'Facturacion.Estado', 'El Esta', 'Estado de domicilio fiscal del emisor. Para la facturación electrónica.')
	, (1, 'Facturacion.Pais', 'El Pa', 'País de domicilio fiscal del emisor. Para la facturación electrónica.')

GO

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

CREATE VIEW VentasFacturasDetalleView AS
	SELECT
		vfd.VentaFacturaDetalleID
		, vf.VentaFacturaID
		, vf.Fecha
		, vf.EstatusGenericoID
		, vf.FolioFiscal
		, vf.ClienteID
		, vf.SucursalID
		, vf.Serie
		, vf.Folio
		, vf.Observacion
		, vfd.VentaID
	FROM
		VentaFactura vf
		LEFT JOIN VentaFacturaDetalle vfd ON vfd.VentaFacturaID = vf.VentaFacturaID AND vfd.Estatus = 1
	WHERE vf.Estatus = 1
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
		, ISNULL(vd.Subtotal, 0) AS Subtotal
		, ISNULL(vd.Iva, 0) AS Iva
		, ISNULL(vd.Total, 0) AS Total
		, ISNULL(vpt.Importe, 0) AS Pagado
		, v.ACredito
		, v.RealizoUsuarioID AS VendedorID
		, u.NombrePersona AS Vendedor
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

ALTER VIEW VentasPagosView AS
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
	FROM
		VentaPago vp
		LEFT JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN VentasFacturasDetalleView vfdv ON vfdv.VentaID = v.VentaID AND vfdv.EstatusGenericoID != 4
	WHERE vp.Estatus = 1
GO

ALTER VIEW VentasPagosDetalleView AS
	SELECT
		vpd.VentaPagoDetalleID
		, vpv.VentaID
		, ISNULL(vpv.Facturada, 0) AS Facturada
		, vpv.Folio AS VentaFolio
		, vpv.VentaPagoID
		, vpv.Fecha
		, vpv.Importe AS ImporteTotal
		, vpv.VentaEstatusID
		, vpv.ClienteID
		, vpv.Cliente
		, ISNULL(vpv.ACredito, 0) AS ACredito
		, vpd.TipoFormaPagoID AS FormaDePagoID
		, tfp.NombreTipoFormaPago AS FormaDePago
		, vpd.Importe
		, b.NombreBanco AS Banco
		, vpd.Folio
		, vpd.Cuenta
		, vpd.NotaDeCreditoID
		, vpv.Vendedor
	FROM
		VentaPagoDetalle vpd
		LEFT JOIN VentasPagosView vpv ON vpv.VentaPagoID = vpd.VentaPagoID
		LEFT JOIN Banco b ON b.BancoID = vpd.BancoID AND b.Estatus = 1
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = vpd.TipoFormaPagoID AND tfp.Estatus = 1
		/*
		LEFT JOIN VentaPago vp ON vp.VentaPagoID = vpd.VentaPagoID AND vp.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Banco b ON b.BancoID = vpd.BancoID AND b.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
		*/
	WHERE vpd.Estatus = 1
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
		, u.NombrePersona AS RealizoUsuario
		, vd.EsCancelacion
		, vd.TipoFormaPagoID
		-- , CASE WHEN vd.EsCancelacion = 1 THEN 'CANC-' ELSE '    -DEV' END AS Tipo
		-- , CASE WHEN vd.TipoFormaPagoID = 2 THEN 'EF-' ELSE '  -NC' END AS Salida
		, ISNULL(a.Autorizado, 0) AS Autorizado
		, ISNULL(ua.NombrePersona, 'PENDIENTE') AS Autorizo
		, vdd.Subtotal
		, vdd.Iva
		, vdd.Total
	FROM
		VentaDevolucion vd
		LEFT JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
		LEFT JOIN VentaDevolucionMotivo vdm ON vdm.VentaDevolucionMotivoID = vd.MotivoID AND vdm.Estatus = 1
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

ALTER VIEW ClientesAdeudosView AS
	SELECT
		v.ClienteID
		, v.VentaID
		, v.Fecha
		, v.Total
		, v.Pagado
		, (v.Total - v.Pagado) AS Restante
	FROM VentasView v
	WHERE
		v.VentaEstatusID = 2
		AND v.Total > v.Pagado
GO

CREATE VIEW VentasACreditoView AS
	SELECT
		vv.VentaID
		, vv.ClienteID
		, vv.Folio
		, vv.Fecha
		, (vv.Fecha + c.DiasDeCredito) AS Vencimiento
		, vv.Total
		, vv.Pagado
		, ISNULL(vv.Total - vv.Pagado, 0) AS Restante
	FROM
		VentasView vv
		LEFT JOIN Cliente c ON c.ClienteID = vv.ClienteID AND c.Estatus = 1
	WHERE vv.ACredito = 1
GO

CREATE VIEW NotasDeCreditoView AS
	SELECT
		nc.NotaDeCreditoID
		, nc.FechaDeEmision
		, nc.Importe
		, nc.ClienteID
		, nc.Valida
		, nc.FechaDeUso
		, nc.Observacion
		, u.NombrePersona AS Autorizo
		, v1.Folio AS OrigenVentaFolio
		, v2.Folio AS UsoVentaFolio
	FROM
		NotaDeCredito nc
		LEFT JOIN Autorizacion a ON a.Tabla = 'NotaDeCredito' AND a.TablaRegistroID = nc.NotaDeCreditoID AND a.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = a.UsuarioID AND u.Estatus = 1
		LEFT JOIN Venta v1 ON v1.VentaID = nc.OrigenVentaID AND v1.Estatus = 1
		LEFT JOIN Venta v2 ON v2.VentaID = nc.UsoVentaID AND v2.Estatus = 1
GO

/* *****************************************************************************
** Modificar procedimientos almacenados
***************************************************************************** */
