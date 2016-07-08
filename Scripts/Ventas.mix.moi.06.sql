/* Script con modificaciones para el módulo de ventas. Archivo 6
*/

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	-- Facturación
	
	-- DROP TABLE FacturacionConfig
	CREATE TABLE FacturacionConfig (
		FacturacionConfigID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		, RutaCertificado NVARCHAR(256) NOT NULL
		, RutaArchivoKey NVARCHAR(256) NOT NULL
		, ContraseniaArchivoKey NVARCHAR(16) NOT NULL
		, RutaArchivoPfx NVARCHAR(256) NOT NULL
		, ContraseniaArchivoPfx NVARCHAR(16) NOT NULL
		, UsuarioPac NVARCHAR(16) NOT NULL
		, ContraseniaPac NVARCHAR(16) NOT NULL
		, Rfc NVARCHAR(16) NOT NULL
		, Nombre NVARCHAR(64)
		, RegimenesFiscales NVARCHAR(256) NOT NULL
		, Calle NVARCHAR(64) NOT NULL
		, NumeroExterior NVARCHAR(8)
		, NumeroInterior NVARCHAR(8)
		, Referencia NVARCHAR(64)
		, Colonia NVARCHAR(64)
		, CodigoPostal NVARCHAR(8) NOT NULL
		, Localidad NVARCHAR(64) 
		, Municipio NVARCHAR(64) NOT NULL
		, Estado NVARCHAR(64) NOT NULL
		, Pais NVARCHAR(64) NOT NULL
		
		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME
		, Estatus BIT NOT NULL DEFAULT 1
		, Actualizar BIT NOT NULL DEFAULT 1
	)
	INSERT INTO FacturacionConfig (
		[SucursalID]
		,[RutaCertificado]
		,[RutaArchivoKey]
		,[ContraseniaArchivoKey]
		,[RutaArchivoPfx]
		,[ContraseniaArchivoPfx]
		,[UsuarioPac]
		,[ContraseniaPac]
		,[Rfc]
		,[Nombre]
		,[RegimenesFiscales]
		,[Calle]
		,[NumeroExterior]
		,[NumeroInterior]
		,[Referencia]
		,[Colonia]
		,[CodigoPostal]
		,[Localidad]
		,[Municipio]
		,[Estado]
		,[Pais]
		, UsuarioID
	) VALUES (
		1
		, 'FacturacionElectronica\00001000000300932753.cer'
		, 'FacturacionElectronica\CSD_Cd._Guzman_GARL540703AL7_20130927_104352.key'
		, 'Lucre2807'
		, 'FacturacionElectronica\Cancelacion.pfx'
		, '7082ercuL'
		, 'GARL540703AL7'
		, 'neggwmxip'
		, 'GARL540703AL7'
		, 'Autopartes Garibaldi'
		, 'No sé'
		, 'Una Calle'
		, '35'
		, 'E'
		, 'Algo'
		, 'Una Colonia'
		, '12345'
		, 'La Loca'
		, 'El Muni'
		, 'El Esta'
		, 'El Pa'
		, 1
	)

	CREATE TABLE ClienteFacturacion (
		ClienteFacturacionID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, ClienteID INT NOT NULL FOREIGN KEY REFERENCES Cliente(ClienteID)
		, Rfc NVARCHAR(16) NOT NULL
		, RazonSocial NVARCHAR(64)
		, Calle NVARCHAR(64) NOT NULL
		, NumeroExterior NVARCHAR(8)
		, NumeroInterior NVARCHAR(8)
		, Referencia NVARCHAR(64)
		, Colonia NVARCHAR(64)
		, CodigoPostal NVARCHAR(8) NOT NULL
		, Localidad NVARCHAR(64) 
		, Municipio NVARCHAR(64) NOT NULL
		, Estado NVARCHAR(64) NOT NULL
		, Pais NVARCHAR(64) NOT NULL
		
		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME
		, Estatus BIT NOT NULL DEFAULT 1
		, Actualizar BIT NOT NULL DEFAULT 1
	)

	ALTER TABLE Cliente DROP COLUMN
		RFC
		, CalleFacturacion
		, NumeroExteriorFacturacion
		, NumeroInteriorFacturacion
		, ColoniaFacturacion
		, CodigoPostalFacturacion
		, CiudadFacturacion
		, EstadoFacturacion

	ALTER TABLE VentaFactura ADD
		ClienteID INT NOT NULL FOREIGN KEY REFERENCES Cliente(ClienteID)
		, Identificador NVARCHAR(64) NOT NULL

	-- Cotizacion9500
	ALTER TABLE Cotizacion9500 ADD
		ComisionistaClienteID INT NULL FOREIGN KEY REFERENCES Cliente(ClienteID)
		, AnticipoVentaID INT NULL FOREIGN KEY REFERENCES Venta(VentaID)

	-- VentaDevolucion
	UPDATE VentaDevolucion SET EsCancelacion = 0 WHERE EsCancelacion IS NULL
	ALTER TABLE VentaDevolucion ALTER COLUMN EsCancelacion BIT NOT NULL

	-- AutozacionProceso
	INSERT INTO AutorizacionProceso (Descricpion, UsuarioID) VALUES
		('BORRAR GASTO', 1)
		, ('BORRAR OTRO INGRESO', 1)
		, ('9500 PRECIO FUERA DE RANGO', 1)
	UPDATE AutorizacionProceso SET Descricpion = UPPER(Descricpion)
	
	-- Impresión de tickets
	INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
		('Tickets.Leyenda1', 'Comonfort No. 251. Cd. Guzmán, Jal CP 49000', 'Comonfort No. 251. Cd. Guzmán, Jal CP 49000', 'Leyenda fija para utilizar en los tickets impresos.')
		, ('Tickets.Leyenda2', 'GARL-540703-AL7 Nextel 92*81 7573*1', 'GARL-540703-AL7 Nextel 92*81 7573*1', 'Leyenda fija para utilizar en los tickets impresos.')
		, ('Tickets.Leyenda3', 'Tel (341) 41 31163 Fax (341) 41 30734', 'Tel (341) 41 31163 Fax (341) 41 30734', 'Leyenda fija para utilizar en los tickets impresos.')
		, ('Tickets.Leyenda4', 'www.autopartesgaribaldi.mx', 'www.autopartesgaribaldi.mx', 'Leyenda fija para utilizar en los tickets impresos.')
		, ('Tickets.Leyenda5', '', '', 'Leyenda fija para utilizar en los tickets impresos.')
		, ('Tickets.Leyenda6', '¡¡ Gracias por su preferencia !!', '¡¡ Gracias por su preferencia !!', 'Leyenda fija para utilizar en los tickets impresos.')
		, ('Tickets.Leyenda7', 'Le recordamos que es necesario presentar este ticket para hacer válida la Garantía o realizar Devoluciones', 'Le recordamos que es necesario presentar este ticket para hacer válida la Garantía o realizar Devoluciones', 'Leyenda fija para utilizar en los tickets impresos.')
	
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
END CATCH

/* *****************************************************************************
** Modificar tablas
***************************************************************************** */



GO

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

ALTER VIEW VentasView AS
	SELECT
		v.VentaID
		, CONVERT(BIT, 0) AS EsFactura
		, dbo.fnuLlenarIzquierda(CONVERT(NVARCHAR(7), v.VentaID), '0', 7) AS Folio
		, v.Fecha
		, v.ClienteID
		, c.Nombre AS Cliente
		, v.SucursalID
		, v.VentaEstatusID
		, ve.Descripcion AS Estatus
		, ISNULL(SUM(vd.PrecioUnitario * vd.Cantidad), 0) AS Subtotal
		, ISNULL(SUM(vd.Iva * vd.Cantidad), 0) AS Iva
		, ISNULL(SUM((vd.PrecioUnitario + vd.Iva) * vd.Cantidad), 0) AS Total
		, ISNULL(vpt.Importe, 0) AS Pagado
		, v.RealizoUsuarioID AS VendedorID
		, u.NombrePersona AS Vendedor
		, v.ComisionistaClienteID AS ComisionistaID
	FROM
		Venta v
		LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		LEFT JOIN VentaEstatus ve ON ve.VentaEstatusID = v.VentaEstatusID AND ve.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
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
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
		-- LEFT JOIN VentaPago vp ON vp.VentaID = v.VentaID AND vp.Estatus = 1
		-- LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
	WHERE v.Estatus = 1
	GROUP BY
		v.VentaID
		, v.Fecha
		, v.ClienteID
		, c.Nombre
		, v.SucursalID
		, v.VentaEstatusID
		, ve.Descripcion
		, vpt.Importe
		, v.RealizoUsuarioID
		, u.NombrePersona
		, v.ComisionistaClienteID
GO

ALTER VIEW VentasPagosView AS
	SELECT
		vp.VentaPagoID
		, vp.VentaID
		, ISNULL(CONVERT(BIT, 0), 0) AS EsFactura
		, dbo.fnuLlenarIzquierda(CONVERT(NVARCHAR(7), vp.VentaID), '0', 7) AS Folio
		, vp.Fecha
		, ISNULL((SELECT SUM(Importe) FROM VentaPagoDetalle WHERE VentaPagoID = vp.VentaPagoID AND Estatus = 1), 0) AS Importe
		, vp.TipoPagoID
		, vp.SucursalID
		, c.ClienteID
		, v.VentaEstatusID
		-- , v.
		, c.Nombre AS Cliente
		, u.NombrePersona AS Vendedor
	FROM
		VentaPago vp
		LEFT JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
	WHERE vp.Estatus = 1
GO

ALTER VIEW VentasPagosDetalleView AS
	SELECT
		vpd.VentaPagoDetalleID
		, vpv.VentaID
		, ISNULL(vpv.EsFactura, 0) AS EsFactura
		, vpv.Folio AS VentaFolio
		, vpv.VentaPagoID
		, vpv.Fecha
		, vpv.Importe AS ImporteTotal
		, vpv.ClienteID
		, vpv.Cliente
		, vpv.TipoPagoID AS TipoDePagoID
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
		-- LEFT JOIN TipoPago tp ON tp.TipoPagoID = vp.TipoPagoID AND tp.Estatus = 1
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

ALTER VIEW ClientesDatosView AS
	SELECT
		c.ClienteID
		, RTRIM(c.Nombre + ' ' + ISNULL(c.ApellidoPaterno, '') + ' ' + ISNULL(c.ApellidoMaterno, '')) AS Nombre
		, c.Calle + ' ' + c.NumeroExterior
			+ CASE WHEN ISNULL(c.NumeroInterior, '') = '' THEN '' ELSE ' - ' END
			+ ISNULL(c.NumeroInterior, '') AS Direccion
		, c.Colonia
		, c.CodigoPostal
		, c.Ciudad
		, c.Telefono
		, c.Celular
		, cf.Rfc
		
		, c.ListaDePrecios
		, c.TieneCredito
		, c.ePoints
	FROM
		Cliente c
		LEFT JOIN ClienteFacturacion cf ON cf.ClienteID = c.ClienteID AND cf.Estatus = 1
	WHERE c.Estatus = 1
GO

ALTER VIEW VentasDevolucionesView AS
	SELECT
		vd.VentaDevolucionID
		, CONVERT(BIT, 0) AS EsFactura
		, dbo.fnuLlenarIzquierda(CONVERT(NVARCHAR(7), vd.VentaID), '0', 7) AS Folio
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
		, vdd.Total
	FROM
		VentaDevolucion vd
		LEFT JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
		LEFT JOIN VentaDevolucionMotivo vdm ON vdm.VentaDevolucionMotivoID = vd.MotivoID AND vdm.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = vd.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN Autorizacion a ON a.Tabla = 'VentaDevolucion' AND a.TablaRegistroID = vd.VentaDevolucionID AND a.Estatus = 1
		LEFT JOIN Usuario ua ON ua.UsuarioID = a.AutorizoUsuarioID AND ua.Estatus = 1
		LEFT JOIN (
			SELECT
				VentaDevolucionID
				, SUM((PrecioUnitario + Iva) * Cantidad) AS Total
			FROM VentaDevolucionDetalle
			WHERE Estatus = 1
			GROUP BY VentaDevolucionID
		) vdd ON vdd.VentaDevolucionID = vd.VentaDevolucionID
	WHERE vd.Estatus = 1
GO

CREATE VIEW VentasDevolucionesDetalleView AS
	SELECT
		vdd.VentaDevolucionDetalleID
		, vdd.VentaDevolucionID
		, vdd.ParteID
		, p.NumeroParte
		, p.NombreParte
		, vdd.Cantidad
		, vdd.PrecioUnitario
		, vdd.Iva
	FROM
		VentaDevolucionDetalle vdd
		LEFT JOIN Parte p ON p.ParteID = vdd.ParteID AND p.Estatus = 1
	WHERE vdd.Estatus = 1
GO

ALTER VIEW Lista9500View AS
	SELECT
		c9.Cotizacion9500ID AS Folio
		, c9.Fecha
		, p.NumeroParte
		, p.NombreParte
		, c9.Anticipo
		, (c.Nombre + ' ' + c.ApellidoPaterno + ' ' + c.ApellidoMaterno) AS Nombre
		, s.NombreSucursal AS Sucursal
		, v.NombrePersona AS Vendedor
	FROM
		Cotizacion9500 c9
		LEFT JOIN (
			SELECT
				Cotizacion9500ID
				, ParteID
			FROM (
				SELECT
					Cotizacion9500ID
					, ParteID
					, ROW_NUMBER() OVER (PARTITION BY Cotizacion9500ID ORDER BY Cotizacion9500DetalleID)
						AS Numero
				FROM Cotizacion9500Detalle c9d
				WHERE Estatus = 1
			) c9d
			WHERE Numero = 1
		) c9d ON c9d.Cotizacion9500ID = c9.Cotizacion9500ID
		LEFT JOIN Parte p ON p.ParteID = c9d.ParteID AND p.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = c9.ClienteID AND c.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = c9.SucursalID AND s.Estatus = 1
		LEFT JOIN Usuario v ON v.UsuarioID = c9.RealizoUsuarioID AND v.Estatus = 1
	WHERE
		c9.Estatus = 1
		AND c9.EstatusGenericoID = 2
GO

DROP VIEW MotoresBasicoView
GO

CREATE VIEW MotoresAniosView AS
	SELECT
		mt.ModeloID
		, mt.MotorID
		, mt.NombreMotor
		, ma.Anio
	FROM
		Motor mt
		LEFT JOIN MotorAnio ma ON ma.MotorID = mt.MotorID AND ma.Estatus = 1
	WHERE mt.Estatus = 1
GO

/* *****************************************************************************
** Modificar procedimientos almacenados
***************************************************************************** */
