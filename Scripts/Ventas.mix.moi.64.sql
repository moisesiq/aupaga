/* Script con modificaciones para el módulo de ventas. Archivo 64
 * Creado: 2014/10/17
 * Subido: 2014/10/27
 */


----------------------------------------------------------------------------------- Código de André
Alter table Cliente
add HoraDeCobro Time Null

-- Alter table Cliente
-- add FechaDeCobro Date Null
ALTER TABLE Cliente ADD DiaDeCobro INT NULL

insert into Permiso (NombrePermiso,MensajeDeError) values('Administracion.Clientes.Calendario.Ver','No tienes permisos para acceder al calendario de cobro.')

insert into Permiso (NombrePermiso,MensajeDeError) values('Administracion.Clientes.Calendario.Agregar','No tienes permisos agendar en este calendario.')

Alter table Usuario
ADD AlertaCalendarioClientes BIT NULL

CREATE TABLE [dbo].[ClienteEventoCalendario](
	[ClienteEventoCalendarioID] [int] IDENTITY(1,1) NOT NULL,
	[DiaEvento] [date] NOT NULL,
	[HoraEvento] [time](7) NOT NULL,
	[ClienteID] [int] NULL,
	[Evento] [nvarchar](255) NULL,
	Revisado BIT NOT NULL DEFAULT 0
PRIMARY KEY CLUSTERED 
(
	[ClienteEventoCalendarioID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[ClienteEventoCalendario]  WITH CHECK ADD FOREIGN KEY([ClienteID])
REFERENCES [dbo].[Cliente] ([ClienteID])

GO

ALTER VIEW [dbo].[ClientesCreditoView] AS
	WITH _Tiempos AS (
		SELECT
			v.VentaID
			, v.Fecha
			, v.ClienteID
			, DATEDIFF(DAY, v.Fecha, MAX(vp.Fecha)) AS DiasPago
		FROM
			Venta v
			LEFT JOIN VentaPago vp ON vp.VentaID = v.VentaID AND vp.Estatus = 1
		WHERE
			v.Estatus = 1
			AND v.ACredito = 1
		AND v.VentaEstatusID = 3
			AND v.Fecha >= DATEADD(YEAR, -1, GETDATE())
		GROUP BY
			v.VentaID
			, v.Fecha
			, v.ClienteID
	)
	SELECT
		c.ClienteID
		, c.Nombre
		, c.TieneCredito
		, c.LimiteCredito AS LimiteDeCredito
		, c.DiasDeCredito
		, c.Tolerancia
		, c.DiaDeCobro
		, c.HoraDeCobro
		, SUM(ca.Adeudo) AS Adeudo
		, MIN(ca.Fecha) AS FechaPrimerAdeudo
		-- , ISNULL(CONVERT(BIT, CASE WHEN (ca.Adeudo >= c.LimiteCredito OR ca.FechaPrimerAdeudo >= (GETDATE() + c.DiasDeCredito))
		-- 	THEN 1 ELSE 0 END), 0) AS CreditoVencido
		, SUM(CASE WHEN ca.Dias > c.DiasDeCredito THEN ca.Adeudo ELSE 0.0 END) AS AdeudoVencido
		, (SELECT AVG(DiasPago) FROM _Tiempos WHERE ClienteID = c.ClienteID) AS PromedioDePagoAnual
		, (SELECT AVG(DiasPago) FROM _Tiempos WHERE ClienteID = c.ClienteID
			AND Fecha > DATEADD(MONTH, -3, GETDATE())) AS PromedioDePago3Meses
	FROM
		Cliente c
		LEFT JOIN (
			SELECT
				VentaID
				, ClienteID
				, Fecha
				, SUM(Total - Pagado) AS Adeudo
				, DATEDIFF(DAY, Fecha, GETDATE()) AS Dias
			FROM VentasView
			WHERE ACredito = 1 AND (Total - Pagado) > 0 AND VentaEstatusID = 2
			GROUP BY
				VentaID
				, ClienteID
				, Fecha
		) ca ON ca.ClienteID = c.ClienteID
	GROUP BY
		c.ClienteID
		, c.Nombre
		, c.TieneCredito
		, c.LimiteCredito
		, c.DiasDeCredito
		, c.Tolerancia
		, c.DiaDeCobro
		, c.HoraDeCobro
GO
----------------------------------------------------------------------------------- Código de André


/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	ALTER TABLE ReporteDeFaltante ADD Pedido BIT NOT NULL DEFAULT 0

	ALTER TABLE Usuario ADD AlertaPedidos BIT NULL

	CREATE TABLE ParteCaracteristicaTemporal (
		ParteCaracteristicaTemporalID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, ParteID INT NOT NULL FOREIGN KEY REFERENCES Parte(ParteID)
		, Caracteristica NVARCHAR(32) NULL
		
		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME NULL
	)
	CREATE INDEX Ix_ParteID ON ParteCaracteristicaTemporal(ParteID)

	-- Proveedor

	UPDATE Proveedor SET HoraTope = NULL
	ALTER TABLE Proveedor ALTER COLUMN HoraTope TIME NULL

	CREATE TABLE ProveedorEventoCalendario (
		ProveedorEventoCalendarioID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, ProveedorID INT NOT NULL FOREIGN KEY REFERENCES Proveedor(ProveedorID)
		, Fecha DATETIME NOT NULL
		, Evento NVARCHAR(1024) NULL
		, Revisado BIT NOT NULL DEFAULT 0
	)

	-- Kardex

	CREATE TABLE ParteKardexOperacion (
		ParteKardexOperacionID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, Tipo NVARCHAR(1) NOT NULL
		, Operacion NVARCHAR(32) NOT NULL
	)
	INSERT INTO ParteKardexOperacion (Tipo, Operacion) VALUES
		('S', 'VENTA')
		, ('E', 'VENTA CANCELADA')
		, ('E', 'ENTRADA COMPRA')
		, ('S', 'DEVOLUCIÓN A PROVEEDOR')
		, ('E', 'ENTRADA INVENTARIO')
		, ('S', 'SALIDA INVENTARIO')
		, ('E', 'ENTRADA TRASPASO')
		, ('S', 'SALIDA TRASPASO')

	CREATE TABLE ParteKardex (
		ParteKardexID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, ParteID INT NOT NULL FOREIGN KEY REFERENCES Parte(ParteID)
		, OperacionID INT NOT NULL FOREIGN KEY REFERENCES ParteKardexOperacion(ParteKardexOperacionID)
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		, Folio NVARCHAR(16) NOT NULL
		, Fecha DATETIME NOT NULL
		, RealizoUsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, Entidad NVARCHAR(128) NOT NULL
		, Origen NVARCHAR(128) NULL
		, Destino NVARCHAR(128) NULL
		, Cantidad DECIMAL(6, 2) NOT NULL
		, Importe DECIMAL(12, 2) NOT NULL
	)
	CREATE INDEX Ix_ParteID ON ParteKardex(ParteID)

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
** Modificar tablas
***************************************************************************** */



/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

ALTER VIEW [dbo].[VentasView] AS
	SELECT
		v.VentaID
		, ISNULL(CONVERT(BIT, CASE WHEN vfdv.FolioFiscal IS NULL THEN 0 ELSE 1 END), 0) AS Facturada
		, v.Folio
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

ALTER VIEW [dbo].[VentasDevolucionesView] AS
	SELECT
		vd.VentaDevolucionID
		, ISNULL(CONVERT(BIT, CASE WHEN vfdv.FolioFiscal IS NULL THEN 0 ELSE 1 END), 0) AS Facturada
		, vd.VentaID
		, v.Folio
		, v.Fecha AS FechaDeVenta
		, ISNULL(CONVERT(BIT, v.ACredito), 0) AS VentaACredito
		, v.ClienteID
		, c.Nombre AS Cliente
		, vd.Fecha
		, vd.MotivoID
		, vdm.Descripcion AS Motivo
		, vd.Observacion
		, vd.SucursalID
		, s.NombreSucursal AS Sucursal
		, vd.RealizoUsuarioID
		, u.NombrePersona AS Realizo
		, vd.EsCancelacion
		, vd.TipoFormaPagoID AS FormaDePagoID
		, vd.NotaDeCreditoID
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
		LEFT JOIN VentaDevolucionMotivo vdm ON vdm.VentaDevolucionMotivoID = vd.MotivoID
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = vd.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = vd.SucursalID AND s.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
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

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

ALTER PROCEDURE [dbo].[pauPedidosSugeridos] (
	@SucursalID NVARCHAR(10)
) 
AS BEGIN
SET NOCOUNT ON

	DECLARE @EstGenPendiente INT = 2
	DECLARE @EstPedidoNoSurtido INT = 2

	;WITH _Pedidos AS (
		SELECT
			ParteID	
			,CAST(1 AS BIT) AS Sel
			,NumeroParte	
			,NombreParte	
			,UnidadEmpaque	
			,CriterioABC	
			,MaxMatriz	
			,MaxSuc02	
			,MaxSuc03	
			,ExistenciaMatriz	
			,ExistenciaSuc02	
			,ExistenciaSuc03	
			,MaxMatriz - ExistenciaMatriz AS NecesidadMatriz
			,MaxSuc02 - ExistenciaSuc02 AS NecesidadSuc02
			,MaxSuc03 - ExistenciaSuc03 AS NecesidadSuc03
			,(MaxMatriz - ExistenciaMatriz) + (MaxSuc02 - ExistenciaSuc02) + (MaxSuc03 - ExistenciaSuc03) AS Total
			,CEILING((MaxMatriz - ExistenciaMatriz) + (MaxSuc02 - ExistenciaSuc02) + (MaxSuc03 - ExistenciaSuc03) /
			(CASE WHEN Pre.UnidadEmpaque = 0 THEN 1 ELSE Pre.UnidadEmpaque END)) * Pre.UnidadEmpaque AS Pedido	
			,Costo	
			,ProveedorID	
			,NombreProveedor	
			,Beneficiario
			, '' AS Observacion
		FROM (
			SELECT 
				Parte.ParteID
				,Parte.NumeroParte
				,Parte.NombreParte
				,Parte.UnidadEmpaque
				,ParteAbc.AbcDeVentas AS CriterioABC	
				,SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteMaxMin.Maximo ELSE 0.0 END) AS MaxMatriz
				,SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteMaxMin.Maximo ELSE 0.0 END) AS MaxSuc02
				,SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteMaxMin.Maximo ELSE 0.0 END) AS MaxSuc03	
				,SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteExistencia.Existencia ELSE 0.0 END) AS ExistenciaMatriz
				,SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteExistencia.Existencia ELSE 0.0 END) AS ExistenciaSuc02
				,SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteExistencia.Existencia ELSE 0.0 END) AS ExistenciaSuc03
				,PartePrecio.Costo
				,Parte.ProveedorID
				,Proveedor.NombreProveedor
				,Proveedor.Beneficiario	
			FROM 
				Parte
				INNER JOIN ParteAbc ON  ParteAbc.ParteID = Parte.ParteID
				INNER JOIN ParteMaxMin ON ParteMaxMin.ParteID = Parte.ParteID
				INNER JOIN ParteExistencia ON ParteExistencia.ParteID = Parte.ParteID AND ParteExistencia.SucursalID = ParteMaxMin.SucursalID
				INNER JOIN PartePrecio ON PartePrecio.ParteID = Parte.ParteID
				INNER JOIN Proveedor ON Proveedor.ProveedorID = Parte.ProveedorID
			WHERE
				Parte.Estatus = 1 
				AND ParteMaxMin.Maximo > 0
				AND ParteExistencia.Existencia <= ParteMaxMin.Minimo
				AND Parte.ParteID NOT IN (SELECT PedidoDetalle.ParteID FROM PedidoDetalle WHERE PedidoDetalle.Estatus = 1 AND PedidoDetalle.PedidoEstatusID = 2)	
				AND ParteExistencia.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))	
				AND Parte.ParteID NOT IN 
					(SELECT d.ParteID 
					FROM MovimientoInventario m 
					INNER JOIN MovimientoInventarioDetalle d ON m.MovimientoInventarioID = d.MovimientoInventarioID 
					WHERE m.TipoOperacionID = 5 
					AND m.SucursalDestinoID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
					AND m.FechaRecepcion IS NULL 
					AND m.Estatus = 1)			
			GROUP BY
				Parte.ParteID
				,Parte.NumeroParte
				,Parte.NombreParte
				,Parte.UnidadEmpaque
				,ParteAbc.AbcDeVentas
				,PartePrecio.Costo
				,Parte.ProveedorID
				,Proveedor.NombreProveedor
				,Proveedor.Beneficiario	
			) AS Pre
	)
	SELECT
		p.*
		-- Para los que no se deben pedir por la existencia en sus equivalentes
		, CASE WHEN (
			(p.ExistenciaMatriz <= 0 AND pc.ExEquivMatriz > 0)
			OR (p.ExistenciaSuc02 <= 0 AND pc.ExEquivSuc02 > 0)
			OR (p.ExistenciaSuc03 <= 0 AND pc.ExEquivSuc03 > 0)
		) THEN 'NP' ELSE 'MxMn' END AS Caracteristica
	FROM
		_Pedidos p
		LEFT JOIN (
			SELECT
				pi.ParteID
				, SUM(CASE WHEN pe.SucursalID = 1 AND ISNULL(pmm.Calcular, 0) != 1 THEN pe.Existencia ELSE 0.0 END) AS ExEquivMatriz
				, SUM(CASE WHEN pe.SucursalID = 2 AND ISNULL(pmm.Calcular, 0) != 1 THEN pe.Existencia ELSE 0.0 END) AS ExEquivSuc02
				, SUM(CASE WHEN pe.SucursalID = 3 AND ISNULL(pmm.Calcular, 0) != 1 THEN pe.Existencia ELSE 0.0 END) AS ExEquivSuc03
			FROM
				_Pedidos pi
				LEFT JOIN ParteEquivalente pq ON pq.ParteID = pi.ParteID AND pq.Estatus = 1
				LEFT JOIN ParteEquivalente pee ON pee.GrupoID = pq.GrupoID AND pee.ParteID != pq.ParteID AND pee.Estatus = 1
				LEFT JOIN ParteExistencia pe ON pe.ParteID = pee.ParteID AND pe.Estatus = 1
				LEFT JOIN ParteMaxMin pmm ON pmm.ParteID = pe.ParteID and pmm.SucursalID = pe.SucursalID
			GROUP BY pi.ParteID
		) pc ON pc.ParteID = p.ParteID
	WHERE Pedido > 0

	-- Se agregan los 9500
	UNION
	SELECT
		c9d.ParteID
		, CAST(1 AS BIT) AS Sel
		, p.NumeroParte
		, p.NombreParte
		, p.UnidadEmpaque
		, p.CriterioABC
		, NULL AS MaxMatriz
		, NULL AS MaxSuc02
		, NULL AS MaxSuc03
		-- , SUM(CASE WHEN pe.SucursalID = 1 THEN pe.Existencia ELSE 0.0 END) AS ExistenciaMatriz
		-- , SUM(CASE WHEN pe.SucursalID = 2 THEN pe.Existencia ELSE 0.0 END) AS ExistenciaSuc02
		-- , SUM(CASE WHEN pe.SucursalID = 3 THEN pe.Existencia ELSE 0.0 END) AS ExistenciaSuc03
		, pec.ExistenciaMatriz
		, pec.ExistenciaSuc02
		, pec.ExistenciaSuc03
		, SUM(CASE WHEN c9.SucursalID = 1 THEN c9d.Cantidad ELSE 0.0 END) AS NecesidadMatriz
		, SUM(CASE WHEN c9.SucursalID = 2 THEN c9d.Cantidad ELSE 0.0 END) AS NecesidadSuc02
		, SUM(CASE WHEN c9.SucursalID = 3 THEN c9d.Cantidad ELSE 0.0 END) AS NecesidadSuc03
		, SUM(CASE WHEN c9.SucursalID = 1 THEN c9d.Cantidad ELSE 0.0 END)
			+ SUM(CASE WHEN c9.SucursalID = 2 THEN c9d.Cantidad ELSE 0.0 END)
			+ SUM(CASE WHEN c9.SucursalID = 3 THEN c9d.Cantidad ELSE 0.0 END)
		AS Total
		, -- Se calcula el Pedido
			(CEILING((
				SUM(CASE WHEN c9.SucursalID = 1 THEN c9d.Cantidad ELSE 0.0 END)
				+ SUM(CASE WHEN c9.SucursalID = 2 THEN c9d.Cantidad ELSE 0.0 END)
				+ SUM(CASE WHEN c9.SucursalID = 3 THEN c9d.Cantidad ELSE 0.0 END)
			) / (CASE WHEN p.UnidadEmpaque = 0 THEN 1 ELSE p.UnidadEmpaque END)) * p.UnidadEmpaque
			) AS Pedido
		, pp.Costo
		, p.ProveedorID
		, pv.NombreProveedor
		, pv.Beneficiario
		, '' AS Observacion
		, '9500' AS Caracteristica
	FROM
		Cotizacion9500Detalle c9d
		INNER JOIN Cotizacion9500 c9 ON c9.Cotizacion9500ID = c9d.Cotizacion9500ID AND c9.Estatus = 1
		LEFT JOIN PedidoDetalle pd ON pd.ParteID = c9d.ParteID AND pd.PedidoEstatusID = @EstPedidoNoSurtido AND pd.Estatus = 1
		LEFT JOIN Parte p ON p.ParteID = c9d.ParteID AND p.Estatus = 1
		-- LEFT JOIN ParteMaxMin pmm ON pmm.ParteID = c9d.ParteID
		-- LEFT JOIN ParteExistencia pe ON pe.ParteID = c9d.ParteID AND pe.Estatus = 1
		LEFT JOIN (
			SELECT
				ParteID
				, SUM(CASE WHEN SucursalID = 1 THEN Existencia ELSE 0.0 END) AS ExistenciaMatriz
				, SUM(CASE WHEN SucursalID = 2 THEN Existencia ELSE 0.0 END) AS ExistenciaSuc02
				, SUM(CASE WHEN SucursalID = 3 THEN Existencia ELSE 0.0 END) AS ExistenciaSuc03
			FROM ParteExistencia
			WHERE Estatus = 1
			GROUP BY ParteID
		) pec ON pec.ParteID = c9d.ParteID
		LEFT JOIN PartePrecio pp ON pp.ParteID = c9d.ParteID AND pp.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
	WHERE
		c9d.Estatus = 1
		AND c9.EstatusGenericoID = @EstGenPendiente
		AND c9.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))	
		AND pd.PedidoDetalleID IS NULL
	GROUP BY
		c9d.ParteID
		, p.NumeroParte
		, p.NombreParte
		, p.UnidadEmpaque
		, p.CriterioABC
		, pec.ExistenciaMatriz
		, pec.ExistenciaSuc02
		, pec.ExistenciaSuc03
		, pp.Costo
		, p.ProveedorID
		, pv.NombreProveedor
		, pv.Beneficiario

	-- Se agregan los de Reporte de Faltante
	UNION
	SELECT
		rf.ParteID
		, CAST(1 AS BIT) AS Sel
		, p.NumeroParte
		, p.NombreParte
		, p.UnidadEmpaque
		, p.CriterioABC
		, NULL AS MaxMatriz
		, NULL AS MaxSuc02
		, NULL AS MaxSuc03
		, pec.ExistenciaMatriz
		, pec.ExistenciaSuc02
		, pec.ExistenciaSuc03
		, SUM(CASE WHEN rf.SucursalID = 1 THEN rf.CantidadRequerida ELSE 0.0 END) AS NecesidadMatriz
		, SUM(CASE WHEN rf.SucursalID = 2 THEN rf.CantidadRequerida ELSE 0.0 END) AS NecesidadSuc02
		, SUM(CASE WHEN rf.SucursalID = 3 THEN rf.CantidadRequerida ELSE 0.0 END) AS NecesidadSuc03
		, SUM(CASE WHEN rf.SucursalID = 1 THEN rf.CantidadRequerida ELSE 0.0 END)
			+ SUM(CASE WHEN rf.SucursalID = 2 THEN rf.CantidadRequerida ELSE 0.0 END)
			+ SUM(CASE WHEN rf.SucursalID = 3 THEN rf.CantidadRequerida ELSE 0.0 END)
		AS Total
		, -- Se calcula el Pedido
			(CEILING((
				SUM(CASE WHEN rf.SucursalID = 1 THEN rf.CantidadRequerida ELSE 0.0 END)
				+ SUM(CASE WHEN rf.SucursalID = 2 THEN rf.CantidadRequerida ELSE 0.0 END)
				+ SUM(CASE WHEN rf.SucursalID = 3 THEN rf.CantidadRequerida ELSE 0.0 END)
			) / (CASE WHEN p.UnidadEmpaque = 0 THEN 1 ELSE p.UnidadEmpaque END)) * p.UnidadEmpaque
			) AS Pedido
		, pp.Costo
		, p.ProveedorID
		, pv.NombreProveedor
		, pv.Beneficiario
		, rf.Comentario AS Observacion
		, 'RF' AS Caracteristica
	FROM
		ReporteDeFaltante rf
		LEFT JOIN Parte p ON p.ParteID = rf.ParteID AND p.Estatus = 1
		LEFT JOIN (
			SELECT
				ParteID
				, SUM(CASE WHEN SucursalID = 1 THEN Existencia ELSE 0.0 END) AS ExistenciaMatriz
				, SUM(CASE WHEN SucursalID = 2 THEN Existencia ELSE 0.0 END) AS ExistenciaSuc02
				, SUM(CASE WHEN SucursalID = 3 THEN Existencia ELSE 0.0 END) AS ExistenciaSuc03
			FROM ParteExistencia
			WHERE Estatus = 1
			GROUP BY ParteID
		) pec ON pec.ParteID = rf.ParteID
		LEFT JOIN PartePrecio pp ON pp.ParteID = rf.ParteID AND pp.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
	WHERE
		rf.Estatus = 1
		AND rf.Pedido = 0
		AND rf.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))	
	GROUP BY
		rf.ParteID
		, p.NumeroParte
		, p.NombreParte
		, p.UnidadEmpaque
		, p.CriterioABC
		, pec.ExistenciaMatriz
		, pec.ExistenciaSuc02
		, pec.ExistenciaSuc03
		, pp.Costo
		, p.ProveedorID
		, pv.NombreProveedor
		, pv.Beneficiario
		, rf.Comentario

END
GO

ALTER PROCEDURE [dbo].[pauKardex] (
	@ParteID INT
	, @Desde DATE
	, @Hasta DATE
	, @SucursalID INT = NULL
) AS BEGIN
	SET NOCOUNT ON
	
	-- Definición de variables tipo constante
	

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(DAY, 1, @Hasta)
	
	SELECT
		pk.ParteKardexID
		, pk.Fecha
		, pk.Folio
		, pko.Tipo
		, pko.Operacion
		, pk.Entidad
		, u.NombreUsuario AS Usuario
		, pk.Origen
		, pk.Destino
		, pk.Importe
		, pk.Cantidad
	FROM
		ParteKardex pk
		LEFT JOIN ParteKardexOperacion pko ON pko.ParteKardexOperacionID = pk.OperacionID
		LEFT JOIN Usuario u ON u.UsuarioID = pk.RealizoUsuarioID AND u.Estatus = 1
	WHERE
		pk.ParteID = @ParteID
		AND (pk.Fecha >= @Desde AND pk.Fecha < @Hasta)
		AND (@SucursalID IS NULL OR pk.SucursalID = @SucursalID)
	ORDER BY
		pk.Fecha
	
END
GO

-- DROP PROCEDURE pauProProveedoresEventosCalendario
CREATE PROCEDURE pauProProveedoresEventosCalendario AS BEGIN
	SET NOCOUNT ON
	
	DECLARE @EstPedidoCancelado INT = 3
	
	DECLARE @Ahora DATETIME = GETDATE()

	;WITH _Eventos AS (
		SELECT
			pv.ProveedorID
			, CONVERT(DATETIME, (CONVERT(NVARCHAR(10), 
			CASE WHEN ISNULL(CONVERT(INT, pv.CalendarioPedido), 0) > 0 THEN
				DATEADD(DAY, CONVERT(INT, pv.CalendarioPedido), MAX(pd.FechaRegistro))
			ELSE
				DATEADD(DAY,
					CASE WHEN CHARINDEX('L', pv.CalendarioPedidoEnDia) > 0 THEN
						CASE WHEN DATEPART(DW, @Ahora) > 2 THEN (7 - DATEPART(DW, @Ahora) + 2) ELSE  (2 - DATEPART(DW, @Ahora)) END
					ELSE CASE WHEN CHARINDEX('Ma', pv.CalendarioPedidoEnDia) > 0 THEN
						CASE WHEN DATEPART(DW, @Ahora) > 3 THEN (7 - DATEPART(DW, @Ahora) + 3) ELSE  (3 - DATEPART(DW, @Ahora)) END
					ELSE CASE WHEN CHARINDEX('Mi', pv.CalendarioPedidoEnDia) > 0 THEN
						CASE WHEN DATEPART(DW, @Ahora) > 4 THEN (7 - DATEPART(DW, @Ahora) + 4) ELSE  (4 - DATEPART(DW, @Ahora)) END
					ELSE CASE WHEN CHARINDEX('J', pv.CalendarioPedidoEnDia) > 0 THEN
						CASE WHEN DATEPART(DW, @Ahora) > 5 THEN (7 - DATEPART(DW, @Ahora) + 5) ELSE  (5 - DATEPART(DW, @Ahora)) END
					ELSE CASE WHEN CHARINDEX('V', pv.CalendarioPedidoEnDia) > 0 THEN
						CASE WHEN DATEPART(DW, @Ahora) > 6 THEN (7 - DATEPART(DW, @Ahora) + 6) ELSE  (6 - DATEPART(DW, @Ahora)) END
					ELSE CASE WHEN CHARINDEX('S', pv.CalendarioPedidoEnDia) > 0 THEN
						CASE WHEN DATEPART(DW, @Ahora) > 7 THEN (7 - DATEPART(DW, @Ahora) + 7) ELSE  (7 - DATEPART(DW, @Ahora)) END
					ELSE NULL END END END END END END
					, @Ahora)
			END, 111) + ' ' + CONVERT(NVARCHAR(8), pv.HoraTope))) AS FechaEvento
			, 'FECHA DE PEDIDO' AS Evento
		FROM
			Proveedor pv
			LEFT JOIN Pedido pd ON pd.ProveedorID = pv.ProveedorID AND pd.PedidoEstatusID != @EstPedidoCancelado AND pd.Estatus = 1
		WHERE
			pv.Estatus = 1
			AND (CONVERT(INT, pv.CalendarioPedido) > 0 OR LEN(pv.CalendarioPedidoEnDia) > 0)
		GROUP BY
			pv.ProveedorID
			, pv.CalendarioPedido
			, pv.CalendarioPedidoEnDia
			, pv.HoraTope
	)

	INSERT INTO ProveedorCalendarioEvento (ProveedorID, Fecha, Evento)
	SELECT e.*
	FROM
		_Eventos e
		LEFT JOIN ProveedorCalendarioEvento pce ON pce.ProveedorID = e.ProveedorID
			AND CONVERT(DATE, pce.Fecha) = CONVERT(DATE, e.FechaEvento) AND pce.Revisado = 0
	WHERE
		FechaEvento IS NOT NULL
		AND pce.ProveedorCalendarioEventoID IS NULL

END
GO

-- DROP PROCEDURE pauProClientesEventosCalendario
CREATE PROCEDURE pauProClientesEventosCalendario AS BEGIN
	SET NOCOUNT ON
	
	DECLARE @Ahora DATETIME = GETDATE()

	;WITH _Eventos AS (
		SELECT
			ccv.ClienteID
			, CONVERT(DATE, DATEADD(DAY,
				CASE WHEN ccv.DiaDeCobro = 2 THEN
					CASE WHEN DATEPART(DW, @Ahora) > 2 THEN (7 - DATEPART(DW, @Ahora) + 2) ELSE  (2 - DATEPART(DW, @Ahora)) END
				ELSE CASE WHEN ccv.DiaDeCobro = 3 THEN
					CASE WHEN DATEPART(DW, @Ahora) > 3 THEN (7 - DATEPART(DW, @Ahora) + 3) ELSE  (3 - DATEPART(DW, @Ahora)) END
				ELSE CASE WHEN ccv.DiaDeCobro = 4 THEN
					CASE WHEN DATEPART(DW, @Ahora) > 4 THEN (7 - DATEPART(DW, @Ahora) + 4) ELSE  (4 - DATEPART(DW, @Ahora)) END
				ELSE CASE WHEN ccv.DiaDeCobro = 5 THEN
					CASE WHEN DATEPART(DW, @Ahora) > 5 THEN (7 - DATEPART(DW, @Ahora) + 5) ELSE  (5 - DATEPART(DW, @Ahora)) END
				ELSE CASE WHEN ccv.DiaDeCobro = 6 THEN
					CASE WHEN DATEPART(DW, @Ahora) > 6 THEN (7 - DATEPART(DW, @Ahora) + 6) ELSE  (6 - DATEPART(DW, @Ahora)) END
				ELSE CASE WHEN ccv.DiaDeCobro = 7 THEN
					CASE WHEN DATEPART(DW, @Ahora) > 7 THEN (7 - DATEPART(DW, @Ahora) + 7) ELSE  (7 - DATEPART(DW, @Ahora)) END
				ELSE NULL END END END END END END 
				, @Ahora)) AS DiaEvento
			, ccv.HoraDeCobro AS HoraEvento
			, 'FECHA DE COBRO' AS Evento
		FROM
			ClientesCreditoView ccv
		WHERE
			ccv.AdeudoVencido > 0
	)

	INSERT INTO ClienteEventoCalendario (ClienteID, DiaEvento, HoraEvento, Evento)
	SELECT e.*
	FROM
		_Eventos e
		LEFT JOIN ClienteEventoCalendario cec ON cec.ClienteID = e.ClienteID
			AND cec.DiaEvento = e.DiaEvento AND cec.Revisado = 0
	WHERE
		e.DiaEvento IS NOT NULL
		AND cec.ClienteEventoCalendarioID IS NULL

END
GO