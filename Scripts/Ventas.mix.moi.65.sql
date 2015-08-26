/* Script con modificaciones para el módulo de ventas. Archivo 65
 * Creado: 2014/10/27
 * Subido: 2014/11/11
 */


----------------------------------------------------------------------------------- Código de André
--//Cliente
Alter table Cliente
ADD SiempreFactura Bit Null Default 0
Alter table Cliente
ADD SiempreVale BIT NULL Default 0
Alter Table Cliente
Add TicketPrecio1 BIT Null Default 0

--//VentaPago
Alter table VentaPago
ADD Vale nvarchar (64) NULL

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
		, c.SiempreFactura
		, c.SiempreVale
		, c.TicketPrecio1
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
		, c.SiempreFactura
		, c.SiempreVale
		, c.TicketPrecio1
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
		, vp.Vale
	FROM
		VentaPago vp
		LEFT JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN VentasFacturasDetalleView vfdv ON vfdv.VentaID = v.VentaID AND vfdv.EstatusGenericoID != 4
	WHERE vp.Estatus = 1
GO

----------------------------------------------------------------------------------- Código de André


/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	CREATE TABLE Caracteristica (
		CaracteristicaID INT NOT NULL IDENTITY(1, 2) PRIMARY KEY
		, Caracteristica NVARCHAR(32) NOT NULL
		, Multiple BIT NULL
		, MultipleOpciones NVARCHAR(256) NULL
	)
	
	CREATE TABLE LineaCaracteristica (
		LineaCaracteristicaID INT NOT NULL IDENTITY(1, 2) PRIMARY KEY
		, LineaID INT NOT NULL FOREIGN KEY REFERENCES Linea(LineaID)
		, CaracteristicaID INT NOT NULL FOREIGN KEY REFERENCES Caracteristica(CaracteristicaID)
	)
	CREATE INDEX Ix_LineaID ON LineaCaracteristica(LineaID)
	
	CREATE TABLE ParteCaracteristica (
		ParteCaracteristicaID INT NOT NULL IDENTITY(1, 2) PRIMARY KEY
		, ParteID INT NOT NULL FOREIGN KEY REFERENCES Parte(ParteID)
		, CaracteristicaID INT NOT NULL FOREIGN KEY REFERENCES Caracteristica(CaracteristicaID)
		, Valor NVARCHAR(64) NULL
	)
	CREATE INDEX Ix_ParteID ON ParteCaracteristica(ParteID)

	ALTER TABLE ParteKardex ALTER COLUMN Folio NVARCHAR(16) NULL
	ALTER TABLE ParteKardex ALTER COLUMN Cantidad DECIMAL(12, 2) NOT NULL

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

-- DROP VIEW LineasCaracteristicasView
CREATE VIEW LineasCaracteristicasView AS
	SELECT
		lc.LineaCaracteristicaID
		, lc.LineaID
		, lc.CaracteristicaID
		, c.Caracteristica
		, c.Multiple
		, c.MultipleOpciones
	FROM
		LineaCaracteristica lc
		LEFT JOIN Caracteristica c ON c.CaracteristicaID = lc.CaracteristicaID
GO

-- DROP VIEW PartesCaracteristicasView
CREATE VIEW PartesCaracteristicasView AS
	SELECT
		pc.ParteCaracteristicaID
		, pc.ParteID
		, pc.CaracteristicaID
		, c.Caracteristica
		, c.Multiple
		, pc.Valor
	FROM
		ParteCaracteristica pc
		LEFT JOIN Caracteristica c ON c.CaracteristicaID = pc.CaracteristicaID
GO

ALTER VIEW [dbo].[PartesExistenciasView] AS
	SELECT
		p.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, p.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, p.MarcaParteID AS MarcaID
		, mp.NombreMarcaParte AS Marca
		, p.LineaID
		, l.NombreLinea AS Linea
		, pp.Costo
		, pp.CostoConDescuento
		, pe.SucursalID
		, pe.Existencia
		-- , SUM(pe.Existencia) AS Existencia
		, MAX(vd.FechaRegistro) AS UltimaVenta
	FROM
		Parte p
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND p.Estatus = 1
		LEFT JOIN VentaDetalle vd ON vd.ParteID = p.ParteID AND vd.Estatus = 1
	WHERE
		p.Estatus = 1
	GROUP BY
		p.ParteID
		, p.NumeroParte
		, p.NombreParte
		, p.ProveedorID
		, pv.NombreProveedor
		, p.MarcaParteID
		, mp.NombreMarcaParte
		, p.LineaID
		, l.NombreLinea
		, pp.Costo
		, pp.CostoConDescuento
		, pe.SucursalID
		, pe.Existencia
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
		, pa.AbcDeVentas AS CriterioAbc
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
		INNER JOIN Parte p ON p.ParteID = c9d.ParteID AND p.Estatus = 1
		LEFT JOIN ParteAbc pa ON pa.ParteID = c9d.ParteID
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
		, pa.AbcDeVentas
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
		, pa.AbcDeVentas AS CriterioABC
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
		, ('(' + u.NombreUsuario + ') ' + rf.Comentario) AS Observacion
		, 'RF' AS Caracteristica
	FROM
		ReporteDeFaltante rf
		LEFT JOIN Parte p ON p.ParteID = rf.ParteID AND p.Estatus = 1
		LEFT JOIN ParteAbc pa ON pa.ParteID = rf.ParteID
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
		LEFT JOIN Usuario u ON u.UsuarioID = rf.RealizoUsuarioID AND u.Estatus = 1
	WHERE
		rf.Estatus = 1
		AND rf.Pedido = 0
		AND rf.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))	
	GROUP BY
		rf.ParteID
		, p.NumeroParte
		, p.NombreParte
		, p.UnidadEmpaque
		, pa.AbcDeVentas
		, pec.ExistenciaMatriz
		, pec.ExistenciaSuc02
		, pec.ExistenciaSuc03
		, pp.Costo
		, p.ProveedorID
		, pv.NombreProveedor
		, pv.Beneficiario
		, rf.Comentario
		, u.NombreUsuario

END
GO

ALTER PROCEDURE [dbo].[pauProProveedoresEventosCalendario] AS BEGIN
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

	INSERT INTO ProveedorEventoCalendario (ProveedorID, Fecha, Evento)
	SELECT e.*
	FROM
		_Eventos e
		LEFT JOIN ProveedorEventoCalendario pce ON pce.ProveedorID = e.ProveedorID
			AND CONVERT(DATE, pce.Fecha) = CONVERT(DATE, e.FechaEvento) AND pce.Revisado = 0
	WHERE
		FechaEvento IS NOT NULL
		AND pce.ProveedorEventoCalendarioID IS NULL

END
GO

ALTER PROCEDURE [dbo].[pauKardex] (
	@ParteID INT	
	, @SucursalID NVARCHAR(10)
	, @FechaInicial DATE
	, @FechaFinal DATE
) 
AS BEGIN
SET NOCOUNT ON
	
	--DECLARE @ParteID AS INT = 2301
	--DECLARE @SucursalID AS NVARCHAR(10) =  N'1'
	--DECLARE @FechaInicial AS DATE = '2013-11-17'
	--DECLARE @FechaFinal AS DATE = '2014-02-17'
			
	DECLARE @Iva AS DECIMAL(18,2) = (((SELECT CAST(Valor AS DECIMAL(18,2))FROM Configuracion WHERE Configuracion.ConfiguracionID = 1)/100) + 1)

	SELECT 
		Fecha
		,Folio	
		,Tipo	
		,Operacion	
		,ClienteProveedor	
		,NombreUsuario	
		,Origen	
		,Destino	
		,CAST(Unitario AS DECIMAL(18,2)) AS Unitario
		,Cantidad	
		,ExistenciaNueva
	FROM (
		/* ENTRADA COMPRA */
		SELECT
			MovimientoInventario.FechaRegistro AS Fecha
			,CAST(MovimientoInventario.FolioFactura AS VARCHAR) AS Folio
			,'E' AS Tipo
			,'ENTRADA COMPRA' AS Operacion
			,Proveedor.NombreProveedor AS ClienteProveedor
			,Usuario.NombreUsuario
			,CAST(MovimientoInventario.ProveedorID AS VARCHAR) AS Origen
			,Sucursal.NombreSucursal AS Destino
			,PartePrecio.Costo * @Iva AS Unitario
			,MovimientoInventarioDetalle.Cantidad
			,0.0 AS ExistenciaNueva
			,1 AS Orden
		FROM 
			MovimientoInventario 
			INNER JOIN MovimientoInventarioDetalle ON MovimientoInventarioDetalle.MovimientoInventarioID = MovimientoInventario.MovimientoInventarioID
			INNER JOIN Proveedor ON Proveedor.ProveedorID = MovimientoInventario.ProveedorID
			INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioID 
			INNER JOIN Sucursal ON Sucursal.SucursalID = MovimientoInventario.SucursalDestinoID 
			INNER JOIN PartePrecio ON PartePrecio.ParteID = MovimientoInventarioDetalle.ParteID
		WHERE 
			MovimientoInventario.TipoOperacionID = 1 
			AND MovimientoInventario.Estatus = 1
			AND MovimientoInventarioDetalle.ParteID = @ParteId			
			AND MovimientoInventario.SucursalDestinoID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))

		/* ENTRADA INVENTARIO */
		UNION
		SELECT 
			MovimientoInventario.FechaRegistro AS Fecha
			,CAST(MovimientoInventario.MovimientoInventarioID AS VARCHAR) AS Folio
			,'E' AS Tipo
			,'ENTRADA INVENTARIO' AS Operacion
			,'-----------------' AS ClienteProveedor
			,Usuario.NombreUsuario
			,'-----' AS Origen
			,Sucursal.NombreSucursal AS Destino
			,PartePrecio.Costo * @Iva AS Unitario
			,MovimientoInventarioDetalle.Cantidad
			,0.0 AS ExistenciaNueva
			,2 AS Orden
		FROM 
			MovimientoInventario 
			INNER JOIN MovimientoInventarioDetalle ON MovimientoInventarioDetalle.MovimientoInventarioID = MovimientoInventario.MovimientoInventarioID
			INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioID 
			INNER JOIN Sucursal ON Sucursal.SucursalID = MovimientoInventario.SucursalDestinoID 
			INNER JOIN PartePrecio ON PartePrecio.ParteID = MovimientoInventarioDetalle.ParteID
		WHERE 
			MovimientoInventario.TipoOperacionID = 2
			AND MovimientoInventario.Estatus = 1
			AND MovimientoInventarioDetalle.ParteID = @ParteId			
			AND MovimientoInventario.SucursalDestinoID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
			
		/* SALIDA INVENTARIO */
		UNION
		SELECT 
			MovimientoInventario.FechaRegistro AS Fecha
			,CAST(MovimientoInventario.MovimientoInventarioID AS VARCHAR) AS Folio
			,'S' AS Tipo
			,'SALIDA INVENTARIO' AS Operacion
			,'-----------------' AS ClienteProveedor
			,Usuario.NombreUsuario
			,'-----' AS Origen
			,Sucursal.NombreSucursal AS Destino
			,PartePrecio.Costo * @Iva AS Unitario
			,MovimientoInventarioDetalle.Cantidad
			,0.0 AS ExistenciaNueva
			,6 AS Orden
		FROM 
			MovimientoInventario 
			INNER JOIN MovimientoInventarioDetalle ON MovimientoInventarioDetalle.MovimientoInventarioID = MovimientoInventario.MovimientoInventarioID
			INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioID 
			INNER JOIN Sucursal ON Sucursal.SucursalID = MovimientoInventario.SucursalDestinoID 
			INNER JOIN PartePrecio ON PartePrecio.ParteID = MovimientoInventarioDetalle.ParteID
		WHERE 
			MovimientoInventario.TipoOperacionID = 3
			AND MovimientoInventario.Estatus = 1
			AND MovimientoInventarioDetalle.ParteID = @ParteId
			AND MovimientoInventario.SucursalDestinoID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))

		/* DEVOLUCION A PROVEEDOR */
		UNION
		SELECT 
			MovimientoInventario.FechaRegistro AS Fecha
			,CAST(MovimientoInventario.FolioFactura AS VARCHAR) AS Folio
			,'S' AS Tipo
			,'DEVOLUCION A PROVEEDOR' AS Operacion
			,Proveedor.NombreProveedor AS ClienteProveedor
			,Usuario.NombreUsuario
			,Sucursal.NombreSucursal AS Origen
			,CAST(MovimientoInventario.ProveedorID AS VARCHAR) AS Destino
			,PartePrecio.Costo * @Iva AS Unitario
			,MovimientoInventarioDetalle.Cantidad
			,0.0 AS ExistenciaNueva
			,8 AS Orden
		FROM 
			MovimientoInventario 
			INNER JOIN MovimientoInventarioDetalle ON MovimientoInventarioDetalle.MovimientoInventarioID = MovimientoInventario.MovimientoInventarioID
			INNER JOIN Proveedor ON Proveedor.ProveedorID = MovimientoInventario.ProveedorID
			INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioID 
			INNER JOIN Sucursal ON Sucursal.SucursalID = MovimientoInventario.SucursalDestinoID 
			INNER JOIN PartePrecio ON PartePrecio.ParteID = MovimientoInventarioDetalle.ParteID
		WHERE 
			MovimientoInventario.TipoOperacionID = 4 
			AND MovimientoInventario.Estatus = 1
			AND MovimientoInventarioDetalle.ParteID = @ParteId
			AND MovimientoInventario.SucursalDestinoID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))

		/* TRASPASO SALIDA */
		UNION
		SELECT 
			MovimientoInventario.FechaRegistro AS Fecha
			,CAST(MovimientoInventario.MovimientoInventarioID AS VARCHAR) AS Folio 
			,'S' AS Tipo
			,'TRASPASO SALIDA' AS Operacion
			,'-----------------' AS ClienteProveedor
			,Usuario.NombreUsuario
			,O.NombreSucursal AS Origen
			,D.NombreSucursal AS Destino
			,PartePrecio.Costo * @Iva AS Unitario
			,MovimientoInventarioDetalle.Cantidad
			,0.0 AS ExistenciaNueva
			,7 AS Orden
		FROM 
			MovimientoInventario 
			INNER JOIN MovimientoInventarioDetalle ON MovimientoInventarioDetalle.MovimientoInventarioID = MovimientoInventario.MovimientoInventarioID
			INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioID 
			INNER JOIN Sucursal O ON O.SucursalID = MovimientoInventario.SucursalOrigenID 
			INNER JOIN Sucursal D ON D.SucursalID = MovimientoInventario.SucursalDestinoID 
			INNER JOIN PartePrecio ON PartePrecio.ParteID = MovimientoInventarioDetalle.ParteID
		WHERE 
			MovimientoInventario.TipoOperacionID = 5
			AND MovimientoInventario.Estatus = 1
			AND MovimientoInventarioDetalle.ParteID = @ParteId		
			AND MovimientoInventario.SucursalOrigenID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
			
		/* TRASPASO ENTRADA */
		UNION
		SELECT 
			MovimientoInventario.FechaRecepcion AS Fecha
			,CAST(MovimientoInventario.MovimientoInventarioID AS VARCHAR) AS Folio 
			,'E' AS Tipo
			,'TRASPASO ENTRADA' AS Operacion
			,'-----------------' AS ClienteProveedor
			,Usuario.NombreUsuario
			,O.NombreSucursal AS Origen
			,D.NombreSucursal AS Destino
			,PartePrecio.Costo * @Iva AS Unitario
			,MovimientoInventarioDetalle.Cantidad
			,0.0 AS ExistenciaNueva
			,3 AS Orden
		FROM 
			MovimientoInventario 
			INNER JOIN MovimientoInventarioDetalle ON MovimientoInventarioDetalle.MovimientoInventarioID = MovimientoInventario.MovimientoInventarioID
			INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioRecibioTraspasoID 
			INNER JOIN Sucursal O ON O.SucursalID = MovimientoInventario.SucursalOrigenID 
			INNER JOIN Sucursal D ON D.SucursalID = MovimientoInventario.SucursalDestinoID 
			INNER JOIN PartePrecio ON PartePrecio.ParteID = MovimientoInventarioDetalle.ParteID
		WHERE 
			MovimientoInventario.TipoOperacionID = 5
			AND MovimientoInventario.Estatus = 1
			AND MovimientoInventarioDetalle.ParteID = @ParteId		
			AND MovimientoInventario.FechaRecepcion IS NOT NULL
			AND MovimientoInventario.UsuarioRecibioTraspasoID IS NOT NULL
			AND MovimientoInventario.SucursalDestinoID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))

		/* VENTA (PAGADA Y COBRADA) */
		UNION
		SELECT
			Venta.Fecha AS Fecha		
			,Venta.Folio
			,'S' AS Tipo
			,'VENTAS' AS Operacion
			,Cliente.Nombre AS ClienteProveedor
			,Usuario.NombreUsuario
			,Sucursal.NombreSucursal AS Origen
			,CAST(Venta.ClienteID AS VARCHAR) AS Destino
			,VentaDetalle.PrecioUnitario * @Iva AS Unitario
			,VentaDetalle.Cantidad
			,0.0 AS ExistenciaNueva
			,5 AS Orden
		FROM
			Venta
			LEFT JOIN VentaDetalle ON VentaDetalle.VentaID = Venta.VentaID
			LEFT JOIN Cliente ON Cliente.ClienteID = Venta.ClienteID
			LEFT JOIN Usuario ON Usuario.UsuarioID = Venta.UsuarioID
			LEFT JOIN Sucursal ON Sucursal.SucursalID = Venta.SucursalID
		WHERE
			Venta.Estatus = 1
			AND	Venta.Fecha > '2013-12-12'
			AND Venta.VentaEstatusID IN (2, 3)
			AND VentaDetalle.ParteID = @ParteId
			AND VentaDetalle.Estatus = 1 
			AND Venta.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))

		/* VENTA (CANCELADA) */
		UNION		
		SELECT * FROM (
			SELECT
				Venta.Fecha AS Fecha
				,CAST(Venta.Folio AS VARCHAR) AS Folio
				,'S' AS Tipo
				,'VENTAS' AS Operacion
				,Cliente.Nombre AS ClienteProveedor
				,Usuario.NombreUsuario
				,Sucursal.NombreSucursal AS Origen
				,CAST(Venta.ClienteID AS VARCHAR) AS Destino
				,VentaDevolucionDetalle.PrecioUnitario * @Iva AS Unitario
				,VentaDevolucionDetalle.Cantidad
				,0.0 AS ExistenciaNueva	
				,5 AS Orden		
			FROM
				Venta
				LEFT JOIN VentaDevolucion ON VentaDevolucion.VentaID = Venta.VentaID
				LEFT JOIN VentaDevolucionDetalle ON VentaDevolucionDetalle.VentaDevolucionID = VentaDevolucion.VentaDevolucionID
				LEFT JOIN Cliente ON Cliente.ClienteID = Venta.ClienteID
				LEFT JOIN Usuario ON Usuario.UsuarioID = Venta.UsuarioID
				LEFT JOIN Sucursal ON Sucursal.SucursalID = Venta.SucursalID
			WHERE
				Venta.Estatus = 1
				AND	Venta.Fecha > '2013-12-12'
				AND Venta.VentaEstatusID IN (4, 5)
				AND VentaDevolucionDetalle.ParteID = @ParteId
				AND VentaDevolucion.EsCancelacion = 1
				AND VentaDevolucionDetalle.Estatus = 1
				AND Venta.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
		
			UNION			
			SELECT
				Venta.Fecha AS Fecha
				,CAST(Venta.Folio AS VARCHAR) AS Folio
				,'E' AS Tipo
				,'VENTA CANCELADA' AS Operacion
				,Cliente.Nombre AS ClienteProveedor
				,Usuario.NombreUsuario
				,Sucursal.NombreSucursal AS Origen
				,CAST(Venta.ClienteID AS VARCHAR) AS Destino
				,VentaDevolucionDetalle.PrecioUnitario * @Iva AS Unitario
				,VentaDevolucionDetalle.Cantidad
				,0.0 AS ExistenciaNueva	
				,4 AS Orden			
			FROM
				Venta
				LEFT JOIN VentaDevolucion ON VentaDevolucion.VentaID = Venta.VentaID
				LEFT JOIN VentaDevolucionDetalle ON VentaDevolucionDetalle.VentaDevolucionID = VentaDevolucion.VentaDevolucionID
				LEFT JOIN Cliente ON Cliente.ClienteID = Venta.ClienteID
				LEFT JOIN Usuario ON Usuario.UsuarioID = Venta.UsuarioID
				LEFT JOIN Sucursal ON Sucursal.SucursalID = Venta.SucursalID
			WHERE
				Venta.Estatus = 1
				AND	Venta.Fecha > '2013-12-12'
				AND Venta.VentaEstatusID IN (4, 5)
				AND VentaDevolucionDetalle.ParteID = @ParteId
				AND VentaDevolucion.EsCancelacion = 1
				AND VentaDevolucionDetalle.Estatus = 1
				AND Venta.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
			) AS Canceldas

		/* VENTA (DEVUELTA) */
		UNION
			SELECT * FROM (
			SELECT
				Venta.Fecha AS Fecha
				,CAST(Venta.Folio AS VARCHAR) AS Folio
				,'S' AS Tipo
				,'VENTAS' AS Operacion
				,Cliente.Nombre AS ClienteProveedor
				,Usuario.NombreUsuario
				,Sucursal.NombreSucursal AS Origen
				,CAST(Venta.ClienteID AS VARCHAR) AS Destino
				,VentaDevolucionDetalle.PrecioUnitario * @Iva AS Unitario
				,VentaDevolucionDetalle.Cantidad
				,0.0 AS ExistenciaNueva
				,5 AS Orden	
			FROM
				Venta
				LEFT JOIN VentaDevolucion ON VentaDevolucion.VentaID = Venta.VentaID
				LEFT JOIN VentaDevolucionDetalle ON VentaDevolucionDetalle.VentaDevolucionID = VentaDevolucion.VentaDevolucionID
				LEFT JOIN Cliente ON Cliente.ClienteID = Venta.ClienteID
				LEFT JOIN Usuario ON Usuario.UsuarioID = Venta.UsuarioID
				LEFT JOIN Sucursal ON Sucursal.SucursalID = Venta.SucursalID
			WHERE
				Venta.Estatus = 1
				AND	Venta.Fecha > '2013-12-12'
				AND Venta.VentaEstatusID IN (4, 5)
				AND VentaDevolucionDetalle.ParteID = @ParteId
				AND VentaDevolucion.EsCancelacion = 0
				AND VentaDevolucionDetalle.Estatus = 1
				AND Venta.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
			
			UNION
			
			SELECT
				Venta.Fecha AS Fecha
				,CAST(Venta.Folio AS VARCHAR) AS Folio
				,'E' AS Tipo
				,'VENTA DEVUELTA' AS Operacion
				,Cliente.Nombre AS ClienteProveedor
				,Usuario.NombreUsuario
				,Sucursal.NombreSucursal AS Origen
				,CAST(Venta.ClienteID AS VARCHAR) AS Destino
				,VentaDevolucionDetalle.PrecioUnitario * @Iva AS Unitario
				,VentaDevolucionDetalle.Cantidad
				,0.0 AS ExistenciaNueva
				,9 AS Orden	
			FROM
				Venta
				LEFT JOIN VentaDevolucion ON VentaDevolucion.VentaID = Venta.VentaID
				LEFT JOIN VentaDevolucionDetalle ON VentaDevolucionDetalle.VentaDevolucionID = VentaDevolucion.VentaDevolucionID
				LEFT JOIN Cliente ON Cliente.ClienteID = Venta.ClienteID
				LEFT JOIN Usuario ON Usuario.UsuarioID = Venta.UsuarioID
				LEFT JOIN Sucursal ON Sucursal.SucursalID = Venta.SucursalID
			WHERE
				Venta.Estatus = 1
				AND	Venta.Fecha > '2013-12-12'
				AND Venta.VentaEstatusID IN (4, 5)
				AND VentaDevolucionDetalle.ParteID = @ParteId
				AND VentaDevolucion.EsCancelacion = 0
				AND VentaDevolucionDetalle.Estatus = 1
				AND Venta.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
		) AS Devoluciones
	) AS Kardex
	ORDER BY
		Kardex.Fecha ASC			
		,Kardex.Orden
END
GO

CREATE PROCEDURE [dbo].[pauParteKardex] (
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

ALTER PROCEDURE [dbo].[pauVentasPartesBusqueda] (
	@SucursalID INT
	, @Codigo NVARCHAR(32) = NULL
	, @Descripcion1 NVARCHAR(32) = NULL
	, @Descripcion2 NVARCHAR(32) = NULL
	, @Descripcion3 NVARCHAR(32) = NULL
	, @Descripcion4 NVARCHAR(32) = NULL
	, @Descripcion5 NVARCHAR(32) = NULL
	, @Descripcion6 NVARCHAR(32) = NULL
	, @Descripcion7 NVARCHAR(32) = NULL
	, @Descripcion8 NVARCHAR(32) = NULL
	, @Descripcion9 NVARCHAR(32) = NULL
	, @SistemaID INT = NULL
	, @LineaID INT = NULL
	, @MarcaID INT = NULL
	
	/* , @TipoCilindroID INT = NULL
	, @Largo INT = NULL
	, @Alto INT = NULL
	, @Dientes INT = NULL
	, @Amperes INT = NULL
	, @Watts INT = NULL
	, @Diametro INT = NULL
	, @Astrias INT = NULL
	, @Terminales INT = NULL
	, @Voltios INT = NULL
	*/
	, @Caracteristicas BIT = NULL
	, @Car01 NVARCHAR(64) = NULL
	, @Car02 NVARCHAR(64) = NULL
	, @Car03 NVARCHAR(64) = NULL
	, @Car04 NVARCHAR(64) = NULL
	, @Car05 NVARCHAR(64) = NULL
	, @Car06 NVARCHAR(64) = NULL
	, @Car07 NVARCHAR(64) = NULL
	, @Car08 NVARCHAR(64) = NULL
	, @Car09 NVARCHAR(64) = NULL
	, @Car10 NVARCHAR(64) = NULL
	, @Car11 NVARCHAR(64) = NULL
	, @Car12 NVARCHAR(64) = NULL
	, @Car13 NVARCHAR(64) = NULL
	, @Car14 NVARCHAR(64) = NULL
	, @Car15 NVARCHAR(64) = NULL
	, @Car16 NVARCHAR(64) = NULL
	, @Car17 NVARCHAR(64) = NULL

	, @CodigoAlterno NVARCHAR(32) = NULL

	, @VehiculoModeloID INT = NULL -- Se debe incluir el ModeloID para que el filtro por vehículo tenga efecto
	, @VehiculoAnio INT = NULL
	, @VehiculoMotorID INT = NULL
	
	, @Equivalentes BIT = NULL
) AS BEGIN
	SET NOCOUNT ON

	DECLARE @IdTipoFuenteMostrador INT = 4

	IF @Codigo IS NULL BEGIN
		-- Si la búsqueda incluye Equivalentes
		IF @Equivalentes = 1 BEGIN
			;WITH _Partes AS (
				SELECT
					p.ParteID
					, pe.GrupoID
				FROM
					Parte p
					LEFT JOIN ParteEquivalente pe ON pe.ParteID = p.ParteID AND pe.Estatus = 1
					LEFT JOIN (
						SELECT
							pcc.ParteID
							, MAX(CASE WHEN pcc.Registro = 1 THEN pcc.Valor ELSE NULL END) AS Car01
							, MAX(CASE WHEN pcc.Registro = 2 THEN pcc.Valor ELSE NULL END) AS Car02
							, MAX(CASE WHEN pcc.Registro = 3 THEN pcc.Valor ELSE NULL END) AS Car03
							, MAX(CASE WHEN pcc.Registro = 4 THEN pcc.Valor ELSE NULL END) AS Car04
							, MAX(CASE WHEN pcc.Registro = 5 THEN pcc.Valor ELSE NULL END) AS Car05
							, MAX(CASE WHEN pcc.Registro = 6 THEN pcc.Valor ELSE NULL END) AS Car06
							, MAX(CASE WHEN pcc.Registro = 7 THEN pcc.Valor ELSE NULL END) AS Car07
							, MAX(CASE WHEN pcc.Registro = 8 THEN pcc.Valor ELSE NULL END) AS Car08
							, MAX(CASE WHEN pcc.Registro = 9 THEN pcc.Valor ELSE NULL END) AS Car09
							, MAX(CASE WHEN pcc.Registro = 10 THEN pcc.Valor ELSE NULL END) AS Car10
							, MAX(CASE WHEN pcc.Registro = 11 THEN pcc.Valor ELSE NULL END) AS Car11
							, MAX(CASE WHEN pcc.Registro = 12 THEN pcc.Valor ELSE NULL END) AS Car12
							, MAX(CASE WHEN pcc.Registro = 13 THEN pcc.Valor ELSE NULL END) AS Car13
							, MAX(CASE WHEN pcc.Registro = 14 THEN pcc.Valor ELSE NULL END) AS Car14
							, MAX(CASE WHEN pcc.Registro = 15 THEN pcc.Valor ELSE NULL END) AS Car15
							, MAX(CASE WHEN pcc.Registro = 16 THEN pcc.Valor ELSE NULL END) AS Car16
							, MAX(CASE WHEN pcc.Registro = 17 THEN pcc.Valor ELSE NULL END) AS Car17
						FROM
							(SELECT
								ROW_NUMBER() OVER (PARTITION BY lc.LineaID ORDER BY lc.CaracteristicaID) AS Registro
								, pc.ParteID
								, pc.CaracteristicaID
								, pc.Valor
							FROM
								ParteCaracteristica pc
								LEFT JOIN Parte p ON p.ParteID = pc.ParteID AND p.Estatus = 1
								LEFT JOIN LineaCaracteristica lc ON lc.LineaID = p.LineaID
									AND lc.CaracteristicaID = pc.CaracteristicaID
							) pcc
						GROUP BY pcc.ParteID
				) pcc ON pcc.ParteID = p.ParteID
					AND (@Car01 IS NULL OR pcc.Car01 = @Car01)
					AND (@Car02 IS NULL OR pcc.Car02 = @Car02)
					AND (@Car03 IS NULL OR pcc.Car03 = @Car03)
					AND (@Car04 IS NULL OR pcc.Car04 = @Car04)
					AND (@Car05 IS NULL OR pcc.Car05 = @Car05)
					AND (@Car06 IS NULL OR pcc.Car06 = @Car06)
					AND (@Car07 IS NULL OR pcc.Car07 = @Car07)
					AND (@Car08 IS NULL OR pcc.Car08 = @Car08)
					AND (@Car09 IS NULL OR pcc.Car09 = @Car09)
					AND (@Car10 IS NULL OR pcc.Car10 = @Car10)
					AND (@Car11 IS NULL OR pcc.Car11 = @Car11)
					AND (@Car12 IS NULL OR pcc.Car12 = @Car12)
					AND (@Car13 IS NULL OR pcc.Car13 = @Car13)
					AND (@Car14 IS NULL OR pcc.Car14 = @Car14)
					AND (@Car15 IS NULL OR pcc.Car15 = @Car15)
					AND (@Car16 IS NULL OR pcc.Car16 = @Car16)
					AND (@Car17 IS NULL OR pcc.Car17 = @Car17)
				WHERE
					p.Estatus = 1
					AND (@Descripcion1 IS NULL OR p.NombreParte LIKE '%' + @Descripcion1 + '%')
					AND (@Descripcion2 IS NULL OR p.NombreParte LIKE '%' + @Descripcion2 + '%')
					AND (@Descripcion3 IS NULL OR p.NombreParte LIKE '%' + @Descripcion3 + '%')
					AND (@Descripcion4 IS NULL OR p.NombreParte LIKE '%' + @Descripcion4 + '%')
					AND (@Descripcion5 IS NULL OR p.NombreParte LIKE '%' + @Descripcion5 + '%')
					AND (@Descripcion6 IS NULL OR p.NombreParte LIKE '%' + @Descripcion6 + '%')
					AND (@Descripcion7 IS NULL OR p.NombreParte LIKE '%' + @Descripcion7 + '%')
					AND (@Descripcion8 IS NULL OR p.NombreParte LIKE '%' + @Descripcion8 + '%')
					AND (@Descripcion9 IS NULL OR p.NombreParte LIKE '%' + @Descripcion9 + '%')
					AND (@SistemaID IS NULL OR p.SubsistemaID IN (
						SELECT SubsistemaID
						FROM Subsistema
						WHERE SistemaID = @SistemaID AND Estatus = 1
					))
					AND (@LineaID IS NULL OR p.LineaID = @LineaID)
					AND (@MarcaID IS NULL OR p.MarcaParteID = @MarcaID)
					
					/* AND (@TipoCilindroID IS NULL OR p.TipoCilindroID = @TipoCilindroID)
					AND (@Largo IS NULL OR p.Largo = @Largo)
					AND (@Alto IS NULL OR p.Alto = @Alto)
					AND (@Dientes IS NULL OR p.Dientes = @Dientes)
					AND (@Amperes IS NULL OR p.Amperes = @Amperes)
					AND (@Watts IS NULL OR p.Watts = @Watts)
					AND (@Diametro IS NULL OR p.Diametro = @Diametro)
					AND (@Astrias IS NULL OR p.Astrias = @Astrias)
					AND (@Terminales IS NULL OR p.Terminales = @Terminales)
					AND (@Voltios IS NULL OR p.Voltios = @Voltios)
					*/
					AND (@Caracteristicas IS NULL OR pcc.ParteID IS NOT NULL)
					
					AND (@CodigoAlterno IS NULL OR p.ParteID IN (
						SELECT DISTINCT ParteID
						FROM ParteCodigoAlterno
						WHERE CodigoAlterno LIKE '%' + @CodigoAlterno + '%' AND Estatus = 1
					))
					AND (@VehiculoModeloID IS NULL OR p.ParteID IN (
						SELECT ParteID
						FROM ParteVehiculo
						WHERE
							TipoFuenteID <> @IdTipoFuenteMostrador
							AND ModeloID = @VehiculoModeloID
							AND (@VehiculoMotorID IS NULL OR MotorID = @VehiculoMotorID)
							AND (@VehiculoAnio IS NULL OR Anio = @VehiculoAnio)
					))
			)

			, _Equivalentes AS (
				SELECT
					ISNULL(pe.ParteID, p.ParteID) AS ParteID
					, ROW_NUMBER() OVER(PARTITION BY p.ParteID ORDER BY 
						CASE WHEN pex.Existencia > 0 THEN 1 ELSE 2 END
						, pp.PrecioUno DESC
					) AS Fila
				FROM
					_Partes p
					LEFT JOIN ParteEquivalente pe ON pe.GrupoID = p.GrupoID AND pe.Estatus = 1
					LEFT JOIN PartePrecio pp ON pp.ParteID = pe.ParteID AND pp.Estatus = 1
					LEFT JOIN ParteExistencia pex ON pex.ParteID = pe.ParteID 
						AND pex.SucursalID = @SucursalID AND pex.Estatus = 1
			)

			SELECT DISTINCT
				e.ParteID
				, p.NumeroParte AS NumeroDeParte
				, p.NombreParte AS Descripcion
				, mp.NombreMarcaParte AS Marca
				, l.NombreLinea AS Linea
				, SUM(pe.Existencia) AS Existencia
				, SUM(CASE WHEN pe.SucursalID = @SucursalID THEN pe.Existencia ELSE 0.00 END) AS ExistenciaLocal
				, pic.NombreImagen AS Imagen
				, pic.CuentaImagenes
			FROM
				_Equivalentes e
				LEFT JOIN Parte p ON p.ParteID = e.ParteID AND p.Estatus = 1
				LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
				LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
				LEFT JOIN (
					SELECT
						ParteID
						, MAX(CASE WHEN Orden = 1 THEN NombreImagen ELSE '' END) AS NombreImagen
						, COUNT(*) AS CuentaImagenes
					FROM ParteImagen
					WHERE Estatus = 1
					GROUP BY ParteID
				) pic ON pic.ParteID = p.ParteID
				-- LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
				LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.Estatus = 1
			WHERE e.Fila = 1
			GROUP BY
				e.ParteID
				, p.NumeroParte
				, p.NombreParte
				, mp.NombreMarcaParte
				, l.NombreLinea
				, pic.NombreImagen
				, pic.CuentaImagenes

		-- Si es búsqueda normal
		END ELSE BEGIN
			SELECT
				p.ParteID
				, p.NumeroParte AS NumeroDeParte
				, p.NombreParte AS Descripcion
				, mp.NombreMarcaParte AS Marca
				, l.NombreLinea AS Linea
				-- , pe.Existencia
				, SUM(pe.Existencia) AS Existencia
				, SUM(CASE WHEN pe.SucursalID = @SucursalID THEN pe.Existencia ELSE 0.00 END) AS ExistenciaLocal
				, pic.NombreImagen AS Imagen
				, pic.CuentaImagenes
			FROM
				Parte p
				LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
				LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
				-- LEFT JOIN Subsistema ss ON ss.SubsistemaID = p.SubsistemaID AND ss.Estatus = 1
				-- LEFT JOIN ParteImagen pi ON pi.ParteID = p.ParteID AND pi.Orden = 1 AND pi.Estatus = 1
				LEFT JOIN (
					SELECT
						ParteID
						, MAX(CASE WHEN Orden = 1 THEN NombreImagen ELSE '' END) AS NombreImagen
						, COUNT(*) AS CuentaImagenes
					FROM ParteImagen
					WHERE Estatus = 1
					GROUP BY ParteID
				) pic ON pic.ParteID = p.ParteID
				-- LEFT JOIN ParteCodigoAlterno pca ON pca.ParteID = p.ParteID AND pca.Estatus = 1
				-- LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
				LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.Estatus = 1
				LEFT JOIN (
					SELECT
						pcc.ParteID
						, MAX(CASE WHEN pcc.Registro = 1 THEN pcc.Valor ELSE NULL END) AS Car01
						, MAX(CASE WHEN pcc.Registro = 2 THEN pcc.Valor ELSE NULL END) AS Car02
						, MAX(CASE WHEN pcc.Registro = 3 THEN pcc.Valor ELSE NULL END) AS Car03
						, MAX(CASE WHEN pcc.Registro = 4 THEN pcc.Valor ELSE NULL END) AS Car04
						, MAX(CASE WHEN pcc.Registro = 5 THEN pcc.Valor ELSE NULL END) AS Car05
						, MAX(CASE WHEN pcc.Registro = 6 THEN pcc.Valor ELSE NULL END) AS Car06
						, MAX(CASE WHEN pcc.Registro = 7 THEN pcc.Valor ELSE NULL END) AS Car07
						, MAX(CASE WHEN pcc.Registro = 8 THEN pcc.Valor ELSE NULL END) AS Car08
						, MAX(CASE WHEN pcc.Registro = 9 THEN pcc.Valor ELSE NULL END) AS Car09
						, MAX(CASE WHEN pcc.Registro = 10 THEN pcc.Valor ELSE NULL END) AS Car10
						, MAX(CASE WHEN pcc.Registro = 11 THEN pcc.Valor ELSE NULL END) AS Car11
						, MAX(CASE WHEN pcc.Registro = 12 THEN pcc.Valor ELSE NULL END) AS Car12
						, MAX(CASE WHEN pcc.Registro = 13 THEN pcc.Valor ELSE NULL END) AS Car13
						, MAX(CASE WHEN pcc.Registro = 14 THEN pcc.Valor ELSE NULL END) AS Car14
						, MAX(CASE WHEN pcc.Registro = 15 THEN pcc.Valor ELSE NULL END) AS Car15
						, MAX(CASE WHEN pcc.Registro = 16 THEN pcc.Valor ELSE NULL END) AS Car16
						, MAX(CASE WHEN pcc.Registro = 17 THEN pcc.Valor ELSE NULL END) AS Car17
					FROM
						(SELECT
							ROW_NUMBER() OVER (PARTITION BY lc.LineaID ORDER BY lc.CaracteristicaID) AS Registro
							, pc.ParteID
							, pc.CaracteristicaID
							, pc.Valor
						FROM
							ParteCaracteristica pc
							LEFT JOIN Parte p ON p.ParteID = pc.ParteID AND p.Estatus = 1
							LEFT JOIN LineaCaracteristica lc ON lc.LineaID = p.LineaID
								AND lc.CaracteristicaID = pc.CaracteristicaID
						) pcc
					GROUP BY pcc.ParteID
				) pcc ON pcc.ParteID = p.ParteID
					AND (@Car01 IS NULL OR pcc.Car01 = @Car01)
					AND (@Car02 IS NULL OR pcc.Car02 = @Car02)
					AND (@Car03 IS NULL OR pcc.Car03 = @Car03)
					AND (@Car04 IS NULL OR pcc.Car04 = @Car04)
					AND (@Car05 IS NULL OR pcc.Car05 = @Car05)
					AND (@Car06 IS NULL OR pcc.Car06 = @Car06)
					AND (@Car07 IS NULL OR pcc.Car07 = @Car07)
					AND (@Car08 IS NULL OR pcc.Car08 = @Car08)
					AND (@Car09 IS NULL OR pcc.Car09 = @Car09)
					AND (@Car10 IS NULL OR pcc.Car10 = @Car10)
					AND (@Car11 IS NULL OR pcc.Car11 = @Car11)
					AND (@Car12 IS NULL OR pcc.Car12 = @Car12)
					AND (@Car13 IS NULL OR pcc.Car13 = @Car13)
					AND (@Car14 IS NULL OR pcc.Car14 = @Car14)
					AND (@Car15 IS NULL OR pcc.Car15 = @Car15)
					AND (@Car16 IS NULL OR pcc.Car16 = @Car16)
					AND (@Car17 IS NULL OR pcc.Car17 = @Car17)
			WHERE
				p.Estatus = 1
				AND (@Descripcion1 IS NULL OR p.NombreParte LIKE '%' + @Descripcion1 + '%')
				AND (@Descripcion2 IS NULL OR p.NombreParte LIKE '%' + @Descripcion2 + '%')
				AND (@Descripcion3 IS NULL OR p.NombreParte LIKE '%' + @Descripcion3 + '%')
				AND (@Descripcion4 IS NULL OR p.NombreParte LIKE '%' + @Descripcion4 + '%')
				AND (@Descripcion5 IS NULL OR p.NombreParte LIKE '%' + @Descripcion5 + '%')
				AND (@Descripcion6 IS NULL OR p.NombreParte LIKE '%' + @Descripcion6 + '%')
				AND (@Descripcion7 IS NULL OR p.NombreParte LIKE '%' + @Descripcion7 + '%')
				AND (@Descripcion8 IS NULL OR p.NombreParte LIKE '%' + @Descripcion8 + '%')
				AND (@Descripcion9 IS NULL OR p.NombreParte LIKE '%' + @Descripcion9 + '%')
				
				AND (@SistemaID IS NULL OR p.SubsistemaID IN (
					SELECT SubsistemaID
					FROM Subsistema
					WHERE SistemaID = @SistemaID AND Estatus = 1
				))
				AND (@LineaID IS NULL OR p.LineaID = @LineaID)
				AND (@MarcaID IS NULL OR p.MarcaParteID = @MarcaID)
				
				/* AND (@TipoCilindroID IS NULL OR p.TipoCilindroID = @TipoCilindroID)
				AND (@Largo IS NULL OR p.Largo = @Largo)
				AND (@Alto IS NULL OR p.Alto = @Alto)
				AND (@Dientes IS NULL OR p.Dientes = @Dientes)
				AND (@Amperes IS NULL OR p.Amperes = @Amperes)
				AND (@Watts IS NULL OR p.Watts = @Watts)
				AND (@Diametro IS NULL OR p.Diametro = @Diametro)
				AND (@Astrias IS NULL OR p.Astrias = @Astrias)
				AND (@Terminales IS NULL OR p.Terminales = @Terminales)
				AND (@Voltios IS NULL OR p.Voltios = @Voltios)
				*/
				AND (@Caracteristicas IS NULL OR pcc.ParteID IS NOT NULL)
				
				AND (@CodigoAlterno IS NULL OR p.ParteID IN (
					SELECT DISTINCT ParteID
					FROM ParteCodigoAlterno
					WHERE CodigoAlterno LIKE '%' + @CodigoAlterno + '%' AND Estatus = 1
				))
				
				AND (@VehiculoModeloID IS NULL OR p.ParteID IN (
					SELECT ParteID
					FROM ParteVehiculo
					WHERE
						TipoFuenteID <> @IdTipoFuenteMostrador
						AND ModeloID = @VehiculoModeloID
						AND (@VehiculoMotorID IS NULL OR MotorID = @VehiculoMotorID)
						AND (@VehiculoAnio IS NULL OR Anio = @VehiculoAnio)
				))
			GROUP BY
				p.ParteID
				, p.NumeroParte
				, p.NombreParte
				, mp.NombreMarcaParte
				, l.NombreLinea
				-- , pe.Existencia
				, pic.NombreImagen
				, pic.CuentaImagenes
		END
	
	-- Si es búsqueda por código
	END ELSE BEGIN
		SELECT
			p.ParteID
			, p.NumeroParte AS NumeroDeParte
			, p.NombreParte AS Descripcion
			, mp.NombreMarcaParte AS Marca
			, l.NombreLinea AS Linea
			-- , pe.Existencia
			, SUM(pe.Existencia) AS Existencia
				, SUM(CASE WHEN pe.SucursalID = @SucursalID THEN pe.Existencia ELSE 0.00 END) AS ExistenciaLocal
			, pi.NombreImagen AS Imagen
			, (SELECT COUNT(*) FROM ParteImagen WHERE ParteID = p.ParteID AND Estatus = 1) AS CuentaImagenes
		FROM
			Parte p
			LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
			LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
			LEFT JOIN ParteImagen pi ON pi.ParteID = p.ParteID AND pi.Orden = 1
			-- LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
			LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.Estatus = 1
		WHERE
			p.Estatus = 1
			AND (NumeroParte = @Codigo
			OR CodigoBarra = @Codigo)
		GROUP BY
			p.ParteID
			, p.NumeroParte
			, p.NombreParte
			, mp.NombreMarcaParte
			, l.NombreLinea
			-- , pe.Existencia
			, pi.NombreImagen
	END

END
GO