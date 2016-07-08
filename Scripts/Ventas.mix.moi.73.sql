/* Script con modificaciones para el módulo de ventas. Archivo 73
 * Creado: 2014/12/09
 * Subido: 2014/12/21
 */


----------------------------------------------------------------------------------- Código de André
INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion)
	VALUES ('Reportes.ProveedoresPolizas.Salida' , 'D', 'I' ,'Salida donde debe  mostrarse el ticket de una Poliza de Cheque a Proveedor (D - Diseño, P - Pantalla, I - Impresora).')
----------------------------------------------------------------------------------- Código de André


/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	ALTER TABLE Cliente ADD
		CobroPorEnvio BIT NULL
		, ImporteParaCobroPorEnvio DECIMAL(12, 2) NULL
		, ImporteCobroPorEnvio DECIMAL(12, 2) NULL

	ALTER TABLE ParteEquivalente ADD RealizoUsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		CONSTRAINT Df_RealizoUsuarioID DEFAULT 1
	ALTER TABLE ParteEquivalente DROP CONSTRAINT Df_RealizoUsuarioID
	ALTER TABLE ParteCodigoAlterno ADD RealizoUsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		CONSTRAINT Df_RealizoUsuarioID DEFAULT 1
	ALTER TABLE ParteCodigoAlterno DROP CONSTRAINT Df_RealizoUsuarioID
	ALTER TABLE ParteComplementaria ADD RealizoUsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		CONSTRAINT Df_RealizoUsuarioID DEFAULT 1
	ALTER TABLE ParteComplementaria DROP CONSTRAINT Df_RealizoUsuarioID

	-- Control de Cascos
	ALTER TABLE Parte ADD
		EsCasco BIT NULL
		, EsCascoPara INT NULL FOREIGN KEY REFERENCES Linea(LineaID)
		, RequiereCascoDe INT NULL FOREIGN KEY REFERENCES Parte(ParteID)

	/* DROP TABLE CascoRegistroImporte
	DROP TABLE CascoImporte
	DROP TABLE CascoRegistro */
		
	CREATE TABLE CascoRegistro (
		CascoRegistroID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, Fecha DATETIME NOT NULL
		, VentaID INT NOT NULL FOREIGN KEY REFERENCES Venta(VentaID)
		, ParteID INT NOT NULL FOREIGN KEY REFERENCES Parte(ParteID)
		, RecibidoCascoID INT NULL FOREIGN KEY REFERENCES Parte(ParteID)
		, CobroVentaID INT NULL FOREIGN KEY REFERENCES Venta(VentaID)
		, RealizoUsuarioID INT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)

		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME NULL
	)

	CREATE TABLE CascoImporte (
		CascoImporteID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, Fecha DATETIME NOT NULL
		, OrigenID INT NOT NULL FOREIGN KEY REFERENCES CascoRegistro(CascoRegistroID)
		, Importe DECIMAL(12, 2) NOT NULL
		, ImporteUsado DECIMAL(12, 2) NOT NULL
	)

	CREATE TABLE CascoRegistroImporte (
		CascoRegistroImporteID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, CascoRegistroID INT NOT NULL FOREIGN KEY REFERENCES CascoRegistro(CascoRegistroID)
		, CascoImporteID INT NOT NULL FOREIGN KEY REFERENCES CascoImporte(CascoImporteID)
		, Importe DECIMAL(12, 2)
	)

	INSERT INTO Permiso (NombrePermiso, MensajeDeError) VALUES
		('Ventas.ControlDeCascos.Completar', 'No tienes permisos para Completar un registro de Control de Cascos.')
	INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
		('Reportes.ControlDeCascos.Completar.Salida', 'D', 'I', 'Salida donde debe mostrarse el ticket de cuando se completa un Control de Casco (D - Diseño, P - Pantalla, I - Impresora).')

	--
	INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
		('Reportes.DevolucionAProveedor.Salida', 'D', 'I', 'Salida donde debe mostrarse el ticket de una devolución a proveedor / cancelación (D - Diseño, P - Pantalla, I - Impresora).')

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

-- DROP VIEW PartesAvancesView
CREATE VIEW PartesAvancesView AS
	SELECT
		pc.*
		, pv.NombreProveedor AS Proveedor
		, mp.NombreMarcaParte AS Marca
		, l.NombreLinea AS Linea
	FROM
		(
			SELECT
				p.ParteID
				, p.NumeroParte AS NumeroDeParte
				, p.NombreParte AS Descripcion
				, p.ProveedorID
				, p.MarcaParteID
				, p.LineaID
				, p.FechaRegistro
				, (SELECT TOP 1 1 FROM ParteImagen WHERE ParteID = p.ParteID AND Estatus = 1) AS Fotos
				, (SELECT TOP 1 1 FROM ParteEquivalente WHERE GrupoID = (SELECT GrupoID FROM ParteEquivalente
					WHERE ParteID = p.ParteID AND Estatus = 1)) AS Equivalentes
				, (SELECT TOP 1 1 FROM ParteVehiculo WHERE ParteID = p.ParteID AND Estatus = 1) AS Aplicaciones
				, (SELECT TOP 1 1 FROM ParteCodigoAlterno WHERE ParteID = p.ParteID AND Estatus = 1) AS Alternos
				, (SELECT TOP 1 1 FROM ParteComplementaria WHERE ParteID = p.ParteID) AS Complementarios
				, (SELECT TOP 1 1 FROM ParteCaracteristica WHERE ParteID = p.ParteID) AS Caracteristicas
			FROM Parte p
			WHERE p.Estatus = 1
		) pc
		LEFT JOIN Proveedor pv ON pv.ProveedorID = pc.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = pc.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = pc.LineaID AND l.Estatus = 1
GO

ALTER VIEW [dbo].[PartesView] AS
	SELECT -- TOP 1000000000000
		p.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, p.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, p.MarcaParteID AS MarcaID
		, mp.NombreMarcaParte AS Marca
		, p.LineaID
		, l.NombreLinea AS Linea
		/* ,MAX(CASE 
		WHEN (Linea.Alto = 1 AND Parte.Alto IS NULL) OR 
			(Linea.Diametro = 1 AND Parte.Diametro IS NULL) OR
			(Linea.Largo = 1 AND Parte.Largo IS NULL) OR
			(Linea.Dientes = 1 AND Parte.Dientes IS NULL) OR
			(Linea.Astrias = 1 AND Parte.Astrias IS NULL) OR
			(Linea.Sistema = 1 AND Parte.ParteSistemaID IS NULL) OR	
			(Linea.Amperaje = 1 AND Parte.Amperes IS NULL) OR
			(Linea.Voltaje = 1 AND Parte.Voltios IS NULL) OR
			(Linea.Watts = 1 AND Parte.Watts IS NULL) OR
			(Linea.Ubicacion = 1 AND Parte.ParteUbicacionID IS NULL) OR
			(Linea.Terminales = 1 AND Parte.Terminales IS NULL) THEN 'SI' ELSE 'NO' END) AS FaltanCaracteristicas
		*/
		-- , '' AS FaltanCaracteristicas
		, p.ParteEstatusID
		-- , MAX(CASE WHEN Parte.ParteID = ParteVehiculo.ParteID THEN 'SI' ELSE 'NO' END) AS Aplicacion
		-- , MAX(CASE WHEN Parte.ParteID = ParteEquivalente.ParteID THEN 'SI' ELSE 'NO' END) AS Equivalente
		, p.FechaRegistro
		, p.NumeroParte
			+ p.NombreParte
			+ mp.NombreMarcaParte
			+ l.NombreLinea AS Busqueda
	FROM 
		Parte p
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1 
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		-- LEFT JOIN ParteEstatus pe ON pe.ParteEstatusID = p.ParteEstatusID AND pe.Estatus = 1
		LEFT JOIN ParteVehiculo pa ON pa.ParteID = p.ParteID AND pa.Estatus = 1
		LEFT JOIN ParteEquivalente pe ON pe.ParteID = p.ParteID	AND pe.Estatus = 1
	WHERE p.Estatus = 1
	/* GROUP BY
		Parte.ParteID
		,Parte.NumeroParte 
		,Parte.NombreParte
		,MarcaParte.NombreMarcaParte
		,ParteEstatus.ParteEstatusID
		,Linea.NombreLinea
		,Parte.LineaID
		,Parte.FechaRegistro
	ORDER BY p.NombreParte
	*/
GO

ALTER VIEW [dbo].[ClientesDatosView] AS
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
		, c.CobroPorEnvio
		, c.ImporteParaCobroPorEnvio
		, c.ImporteCobroPorEnvio
	FROM
		Cliente c
		LEFT JOIN Municipio m ON m.MunicipioID = c.MunicipioID AND m.Estatus = 1
		LEFT JOIN Ciudad cd ON cd.CiudadID = c.CiudadID AND cd.Estatus = 1
		LEFT JOIN Estado e ON e.EstadoID = c.EstadoID AND e.Estatus = 1
		LEFT JOIN ClienteFacturacion cf ON cf.ClienteID = c.ClienteID AND cf.Estatus = 1
	WHERE c.Estatus = 1

GO

-- DROP VIEW CascosRegistrosView
CREATE VIEW CascosRegistrosView AS
	SELECT
		cr.CascoRegistroID
		, cr.Fecha
		, cr.VentaID
		, v.Folio AS FolioDeVenta
		, v.ClienteID
		, v.SucursalID
		, v.VentaEstatusID
		, c.Nombre AS Cliente
		, cr.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, (vd.PrecioUnitario + vd.Iva) AS Precio
		, pc.NumeroParte AS NumeroDeParteDeCasco
		, pc.NombreParte AS DescripcionDeCasco
		, pr.NumeroParte AS NumeroDeParteRecibido
		, vc.Folio AS FolioDeCobro
		, STUFF((
			SELECT
				(', ' + CONVERT(NVARCHAR, ci.OrigenID))
			FROM
				CascoRegistroImporte cri
				LEFT JOIN CascoImporte ci ON ci.CascoImporteID = cri.CascoImporteID
			WHERE cri.CascoRegistroID = cr.CascoRegistroID
			FOR XML PATH('')
			), 1, 2, '') AS CascoImporte
	FROM
		CascoRegistro cr
		LEFT JOIN Venta v ON v.VentaID = cr.VentaID AND v.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Parte p ON p.ParteID = cr.ParteID AND p.Estatus = 1
		LEFT JOIN Parte pc ON pc.ParteID = p.RequiereCascoDe AND pc.Estatus = 1
		LEFT JOIN Parte pr ON pr.ParteID = cr.RecibidoCascoID AND pr.Estatus = 1
		LEFT JOIN Venta vc ON vc.VentaID = cr.CobroVentaID AND vc.Estatus = 1
		LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.ParteID = cr.ParteID AND vd.Estatus = 1
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
	DECLARE @OpeTraspaso INT = 5

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
			, CASE WHEN MinMatriz >= ExistenciaMatriz THEN MaxMatriz - ExistenciaMatriz - TraspasoMatriz ELSE 0.0 END AS NecesidadMatriz
			, CASE WHEN MinSuc02 >= ExistenciaSuc02 THEN MaxSuc02 - ExistenciaSuc02 - TraspasoSuc02 ELSE 0.0 END AS NecesidadSuc02
			, CASE WHEN MinSuc03 >= ExistenciaSuc03 THEN MaxSuc03 - ExistenciaSuc03 - TraspasoSuc03 ELSE 0.0 END AS NecesidadSuc03
			-- , MaxMatriz - ExistenciaMatriz AS NecesidadMatriz
			-- , MaxSuc02 - ExistenciaSuc02 AS NecesidadSuc02
			-- , MaxSuc03 - ExistenciaSuc03 AS NecesidadSuc03
			-- ,(MaxMatriz - ExistenciaMatriz) + (MaxSuc02 - ExistenciaSuc02) + (MaxSuc03 - ExistenciaSuc03) AS Total
			-- ,CEILING((MaxMatriz - ExistenciaMatriz) + (MaxSuc02 - ExistenciaSuc02) + (MaxSuc03 - ExistenciaSuc03) /
			-- (CASE WHEN Pre.UnidadEmpaque = 0 THEN 1 ELSE Pre.UnidadEmpaque END)) * Pre.UnidadEmpaque AS Pedido
			,Costo
			, CostoConDescuento
			,ProveedorID
			,NombreProveedor
			,Beneficiario
			, '' AS Observacion
		FROM (
			SELECT 
				p.ParteID
				, p.NumeroParte
				, p.NombreParte
				, p.UnidadEmpaque
				,ParteAbc.AbcDeVentas AS CriterioABC	
				, SUM(CASE WHEN pe.SucursalID = 1 THEN pmm.Maximo ELSE 0.0 END) AS MaxMatriz
				, SUM(CASE WHEN pe.SucursalID = 2 THEN pmm.Maximo ELSE 0.0 END) AS MaxSuc02
				, SUM(CASE WHEN pe.SucursalID = 3 THEN pmm.Maximo ELSE 0.0 END) AS MaxSuc03	
				, SUM(CASE WHEN pmm.SucursalID = 1 THEN pmm.Minimo ELSE 0.0 END) AS MinMatriz
				, SUM(CASE WHEN pmm.SucursalID = 2 THEN pmm.Minimo ELSE 0.0 END) AS MinSuc02
				, SUM(CASE WHEN pmm.SucursalID = 3 THEN pmm.Minimo ELSE 0.0 END) AS MinSuc03
				, SUM(CASE WHEN pe.SucursalID = 1 THEN pe.Existencia ELSE 0.0 END) AS ExistenciaMatriz
				, SUM(CASE WHEN pe.SucursalID = 2 THEN pe.Existencia ELSE 0.0 END) AS ExistenciaSuc02
				, SUM(CASE WHEN pe.SucursalID = 3 THEN pe.Existencia ELSE 0.0 END) AS ExistenciaSuc03
				, SUM(CASE WHEN tra.SucursalID = 1 THEN tra.Cantidad ELSE 0.0 END) AS TraspasoMatriz
				, SUM(CASE WHEN tra.SucursalID = 2 THEN tra.Cantidad ELSE 0.0 END) AS TraspasoSuc02
				, SUM(CASE WHEN tra.SucursalID = 3 THEN tra.Cantidad ELSE 0.0 END) AS TraspasoSuc03
				,PartePrecio.Costo
				, PartePrecio.CostoConDescuento
				, p.ProveedorID
				,Proveedor.NombreProveedor
				,Proveedor.Beneficiario	
			FROM 
				Parte p
				INNER JOIN ParteAbc ON  ParteAbc.ParteID = p.ParteID
				INNER JOIN ParteMaxMin pmm ON pmm.ParteID = p.ParteID
				INNER JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = pmm.SucursalID
				INNER JOIN PartePrecio ON PartePrecio.ParteID = p.ParteID
				INNER JOIN Proveedor ON Proveedor.ProveedorID = p.ProveedorID
				LEFT JOIN (
					SELECT
						mi.SucursalDestinoID AS SucursalID
						, mid.ParteID
						, mid.Cantidad
					FROM
						MovimientoInventario mi
						INNER JOIN MovimientoInventarioDetalle mid ON mid.MovimientoInventarioID = mi.MovimientoInventarioID AND mid.Estatus = 1
					WHERE
						mi.Estatus = 1
						AND mi.TipoOperacionID = @OpeTraspaso
						AND mi.FechaRecepcion IS NULL
				) tra ON tra.ParteID = p.ParteID AND tra.SucursalID = pmm.SucursalID
			WHERE
				p.Estatus = 1 
				AND pmm.Maximo > 0
				-- AND pe.Existencia <= pmm.Minimo
				AND p.ParteID NOT IN (SELECT PedidoDetalle.ParteID FROM PedidoDetalle WHERE PedidoDetalle.Estatus = 1 AND PedidoDetalle.PedidoEstatusID = 2)	
				AND pe.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
				/* Ya se incluyen aún las partes que estén en traspaso
				AND Parte.ParteID NOT IN 
					(SELECT d.ParteID 
					FROM MovimientoInventario m 
					INNER JOIN MovimientoInventarioDetalle d ON m.MovimientoInventarioID = d.MovimientoInventarioID 
					WHERE m.TipoOperacionID = 5 
					AND m.SucursalDestinoID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
					AND m.FechaRecepcion IS NULL 
					AND m.Estatus = 1)
				*/
			GROUP BY
				p.ParteID
				, p.NumeroParte
				, p.NombreParte
				, p.UnidadEmpaque
				,ParteAbc.AbcDeVentas
				,PartePrecio.Costo
				, PartePrecio.CostoConDescuento
				, p.ProveedorID
				,Proveedor.NombreProveedor
				,Proveedor.Beneficiario	
			) AS Pre
		
		WHERE
			(ExistenciaMatriz <= MinMatriz
			OR ExistenciaSuc02 <= MinSuc02
			OR ExistenciaSuc03 <= MinSuc03)
			-- Se sacan los que su necesidad se pueda cubrir con la existencia de matriz
			AND (ExistenciaMatriz - MinMatriz) <= ((MaxSuc02 - ExistenciaSuc02) + (MaxSuc03 - ExistenciaSuc03))
	)
	SELECT
		p.ParteID
		,CAST(1 AS BIT) AS Sel
		, p.NumeroParte
		, p.NombreParte
		, p.UnidadEmpaque
		, p.CriterioABC
		,MaxMatriz
		,MaxSuc02	
		,MaxSuc03	
		,ExistenciaMatriz	
		,ExistenciaSuc02	
		,ExistenciaSuc03	
		, NecesidadMatriz
		, NecesidadSuc02
		, NecesidadSuc03
		, p.NecesidadMatriz + p.NecesidadSuc02 + p.NecesidadSuc03 AS Total
		, CEILING((p.NecesidadMatriz + p.NecesidadSuc02 + p.NecesidadSuc03) /
			(CASE WHEN pt.UnidadEmpaque = 0 THEN 1 ELSE pt.UnidadEmpaque END)) * pt.UnidadEmpaque AS Pedido	
		,Costo	
		, CostoConDescuento
		, p.ProveedorID
		,NombreProveedor	
		,Beneficiario
		, '' AS Observacion
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
		LEFT JOIN Parte pt ON pt.ParteID = p.ParteID AND pt.Estatus = 1
	WHERE
		CEILING((p.NecesidadMatriz + p.NecesidadSuc02 + p.NecesidadSuc03) /
			(CASE WHEN pt.UnidadEmpaque = 0 THEN 1 ELSE pt.UnidadEmpaque END)) * pt.UnidadEmpaque
		> 0

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
		, (SUM(CASE WHEN c9.SucursalID = 1 THEN c9d.Cantidad ELSE 0.0 END)
			- SUM(ISNULL(CASE WHEN c9.SucursalID = 1 THEN pec.ExistenciaMatriz ELSE 0.0 END, 0.0))) AS NecesidadMatriz
		, (SUM(CASE WHEN c9.SucursalID = 2 THEN c9d.Cantidad ELSE 0.0 END) 
			- SUM(ISNULL(CASE WHEN c9.SucursalID = 2 THEN pec.ExistenciaSuc02 ELSE 0.0 END, 0.0))) AS NecesidadSuc02
		, (SUM(CASE WHEN c9.SucursalID = 3 THEN c9d.Cantidad ELSE 0.0 END)
			- SUM(ISNULL(CASE WHEN c9.SucursalID = 3 THEN pec.ExistenciaSuc03 ELSE 0.0 END, 0.0))) AS NecesidadSuc03
		, SUM(CASE WHEN c9.SucursalID = 1 THEN c9d.Cantidad ELSE 0.0 END)
			+ SUM(CASE WHEN c9.SucursalID = 2 THEN c9d.Cantidad ELSE 0.0 END)
			+ SUM(CASE WHEN c9.SucursalID = 3 THEN c9d.Cantidad ELSE 0.0 END)
			- SUM(ISNULL(CASE WHEN c9.SucursalID = 1 THEN pec.ExistenciaMatriz ELSE 0.0 END, 0.0))
			- SUM(ISNULL(CASE WHEN c9.SucursalID = 2 THEN pec.ExistenciaSuc02 ELSE 0.0 END, 0.0))
			- SUM(ISNULL(CASE WHEN c9.SucursalID = 3 THEN pec.ExistenciaSuc03 ELSE 0.0 END, 0.0))
		AS Total
		, -- Se calcula el Pedido
			(CEILING((
				SUM(CASE WHEN c9.SucursalID = 1 THEN c9d.Cantidad ELSE 0.0 END)
				+ SUM(CASE WHEN c9.SucursalID = 2 THEN c9d.Cantidad ELSE 0.0 END)
				+ SUM(CASE WHEN c9.SucursalID = 3 THEN c9d.Cantidad ELSE 0.0 END)
			) / (CASE WHEN p.UnidadEmpaque = 0 THEN 1 ELSE p.UnidadEmpaque END)) * p.UnidadEmpaque
			) AS Pedido
		, pp.Costo
		, pp.CostoConDescuento
		, p.ProveedorID
		, pv.NombreProveedor
		, pv.Beneficiario
		, (CONVERT(VARCHAR, c9.Fecha, 103) + ' ' + CONVERT(VARCHAR(5), c9.Fecha, 114)
			+ ' - ' + u.NombreUsuario) AS Observacion
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
		LEFT JOIN Usuario u ON u.UsuarioID = c9.RealizoUsuarioID AND u.Estatus = 1
	WHERE
		c9d.Estatus = 1
		AND c9.EstatusGenericoID = @EstGenPendiente
		AND c9.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))	
		AND pd.PedidoDetalleID IS NULL
	GROUP BY
		c9d.ParteID
		, c9.Fecha
		, p.NumeroParte
		, p.NombreParte
		, p.UnidadEmpaque
		, pa.AbcDeVentas
		, pec.ExistenciaMatriz
		, pec.ExistenciaSuc02
		, pec.ExistenciaSuc03
		, pp.Costo
		, pp.CostoConDescuento
		, p.ProveedorID
		, pv.NombreProveedor
		, pv.Beneficiario
		, u.NombreUsuario
	HAVING
		(SUM(CASE WHEN c9.SucursalID = 1 THEN c9d.Cantidad ELSE 0.0 END) - ISNULL(pec.ExistenciaMatriz, 0.0)) > 0
		OR (SUM(CASE WHEN c9.SucursalID = 2 THEN c9d.Cantidad ELSE 0.0 END) - ISNULL(pec.ExistenciaSuc02, 0.0)) > 0
		OR (SUM(CASE WHEN c9.SucursalID = 3 THEN c9d.Cantidad ELSE 0.0 END) - ISNULL(pec.ExistenciaSuc03, 0.0)) > 0

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
		, pp.CostoConDescuento
		, p.ProveedorID
		, pv.NombreProveedor
		, pv.Beneficiario
		, (CONVERT(VARCHAR, rf.FechaRegistro, 103) + ' ' + CONVERT(VARCHAR(5), rf.FechaRegistro, 114)
			+ ' - ' + u.NombreUsuario + ' - ' + rf.Comentario) AS Observacion
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
		, rf.FechaRegistro
		, p.NumeroParte
		, p.NombreParte
		, p.UnidadEmpaque
		, pa.AbcDeVentas
		, pec.ExistenciaMatriz
		, pec.ExistenciaSuc02
		, pec.ExistenciaSuc03
		, pp.Costo
		, pp.CostoConDescuento
		, p.ProveedorID
		, pv.NombreProveedor
		, pv.Beneficiario
		, rf.Comentario
		, u.NombreUsuario

END
GO

ALTER PROCEDURE [dbo].[pauPartesBusquedaEnMovimientos]
(
	@Codigo NVARCHAR(32) = NULL
	, @Descripcion1 NVARCHAR(32) = NULL
	, @Descripcion2 NVARCHAR(32) = NULL
	, @Descripcion3 NVARCHAR(32) = NULL
	, @Descripcion4 NVARCHAR(32) = NULL
	, @Descripcion5 NVARCHAR(32) = NULL
	, @Descripcion6 NVARCHAR(32) = NULL
	, @Descripcion7 NVARCHAR(32) = NULL
	, @Descripcion8 NVARCHAR(32) = NULL
	, @Descripcion9 NVARCHAR(32) = NULL
) AS BEGIN
	SET NOCOUNT ON
	IF @Codigo IS NULL BEGIN
		SELECT
			p.ParteID
			, p.NumeroParte AS NumeroDeParte
			, p.NombreParte AS Descripcion
			, l.NombreLinea AS Linea
			, mp.NombreMarcaParte AS Marca
			, pp.Costo
			, 1.0 AS Cantidad
		FROM
			Parte p
			LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND pp.Estatus =	1
			LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
			LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		WHERE
			(@Descripcion1 IS NULL OR p.NombreParte LIKE '%' + @Descripcion1 + '%')
			AND (@Descripcion2 IS NULL OR p.NombreParte LIKE '%' + @Descripcion2 + '%')
			AND (@Descripcion3 IS NULL OR p.NombreParte LIKE '%' + @Descripcion3 + '%')
			AND (@Descripcion4 IS NULL OR p.NombreParte LIKE '%' + @Descripcion4 + '%')
			AND (@Descripcion5 IS NULL OR p.NombreParte LIKE '%' + @Descripcion5 + '%')
			AND (@Descripcion6 IS NULL OR p.NombreParte LIKE '%' + @Descripcion6 + '%')
			AND (@Descripcion7 IS NULL OR p.NombreParte LIKE '%' + @Descripcion7 + '%')
			AND (@Descripcion8 IS NULL OR p.NombreParte LIKE '%' + @Descripcion8 + '%')
			AND (@Descripcion9 IS NULL OR p.NombreParte LIKE '%' + @Descripcion9 + '%')
	END ELSE BEGIN
		SELECT
			p.ParteID
			, p.NumeroParte AS NumeroDeParte
			, p.NombreParte AS Descripcion
			, l.NombreLinea AS Linea
			, mp.NombreMarcaParte AS Marca
			, pp.Costo
			, 1.0 AS Cantidad
		FROM
			Parte p
			LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND pp.Estatus =	1
			LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
			LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		WHERE
			p.NumeroParte = @Codigo
	END

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
								ROW_NUMBER() OVER (PARTITION BY pcd.ParteID ORDER BY lc.CaracteristicaID) AS Registro
								, pcd.ParteID
								, lc.CaracteristicaID
								, pc.Valor
							FROM
								(SELECT DISTINCT ParteID FROM ParteCaracteristica) pcd
								LEFT JOIN Parte p on p.ParteID = pcd.ParteID AND p.Estatus = 1
								LEFT JOIN LineaCaracteristica lc on lc.LineaID = p.LineaID
								LEFT JOIN ParteCaracteristica pc on pc.ParteID = pcd.ParteID 
									AND pc.CaracteristicaID = lc.CaracteristicaID
							) pcc
						GROUP BY pcc.ParteID
				) pcc ON pcc.ParteID = p.ParteID
					AND (@Car01 IS NULL OR pcc.Car01 LIKE '%' + @Car01 + '%')
					AND (@Car02 IS NULL OR pcc.Car02 LIKE '%' + @Car02 + '%')
					AND (@Car03 IS NULL OR pcc.Car03 LIKE '%' + @Car03 + '%')
					AND (@Car04 IS NULL OR pcc.Car04 LIKE '%' + @Car04 + '%')
					AND (@Car05 IS NULL OR pcc.Car05 LIKE '%' + @Car05 + '%')
					AND (@Car06 IS NULL OR pcc.Car06 LIKE '%' + @Car06 + '%')
					AND (@Car07 IS NULL OR pcc.Car07 LIKE '%' + @Car07 + '%')
					AND (@Car08 IS NULL OR pcc.Car08 LIKE '%' + @Car08 + '%')
					AND (@Car09 IS NULL OR pcc.Car09 LIKE '%' + @Car09 + '%')
					AND (@Car10 IS NULL OR pcc.Car10 LIKE '%' + @Car10 + '%')
					AND (@Car11 IS NULL OR pcc.Car11 LIKE '%' + @Car11 + '%')
					AND (@Car12 IS NULL OR pcc.Car12 LIKE '%' + @Car12 + '%')
					AND (@Car13 IS NULL OR pcc.Car13 LIKE '%' + @Car13 + '%')
					AND (@Car14 IS NULL OR pcc.Car14 LIKE '%' + @Car14 + '%')
					AND (@Car15 IS NULL OR pcc.Car15 LIKE '%' + @Car15 + '%')
					AND (@Car16 IS NULL OR pcc.Car16 LIKE '%' + @Car16 + '%')
					AND (@Car17 IS NULL OR pcc.Car17 LIKE '%' + @Car17 + '%')
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
							ROW_NUMBER() OVER (PARTITION BY pcd.ParteID ORDER BY lc.CaracteristicaID) AS Registro
							, pcd.ParteID
							, lc.CaracteristicaID
							, pc.Valor
						FROM
							(SELECT DISTINCT ParteID FROM ParteCaracteristica) pcd
							LEFT JOIN Parte p on p.ParteID = pcd.ParteID AND p.Estatus = 1
							LEFT JOIN LineaCaracteristica lc on lc.LineaID = p.LineaID
							LEFT JOIN ParteCaracteristica pc on pc.ParteID = pcd.ParteID 
								AND pc.CaracteristicaID = lc.CaracteristicaID
						) pcc
					GROUP BY pcc.ParteID
				) pcc ON pcc.ParteID = p.ParteID
					AND (@Car01 IS NULL OR pcc.Car01 LIKE '%' + @Car01 + '%')
					AND (@Car02 IS NULL OR pcc.Car02 LIKE '%' + @Car02 + '%')
					AND (@Car03 IS NULL OR pcc.Car03 LIKE '%' + @Car03 + '%')
					AND (@Car04 IS NULL OR pcc.Car04 LIKE '%' + @Car04 + '%')
					AND (@Car05 IS NULL OR pcc.Car05 LIKE '%' + @Car05 + '%')
					AND (@Car06 IS NULL OR pcc.Car06 LIKE '%' + @Car06 + '%')
					AND (@Car07 IS NULL OR pcc.Car07 LIKE '%' + @Car07 + '%')
					AND (@Car08 IS NULL OR pcc.Car08 LIKE '%' + @Car08 + '%')
					AND (@Car09 IS NULL OR pcc.Car09 LIKE '%' + @Car09 + '%')
					AND (@Car10 IS NULL OR pcc.Car10 LIKE '%' + @Car10 + '%')
					AND (@Car11 IS NULL OR pcc.Car11 LIKE '%' + @Car11 + '%')
					AND (@Car12 IS NULL OR pcc.Car12 LIKE '%' + @Car12 + '%')
					AND (@Car13 IS NULL OR pcc.Car13 LIKE '%' + @Car13 + '%')
					AND (@Car14 IS NULL OR pcc.Car14 LIKE '%' + @Car14 + '%')
					AND (@Car15 IS NULL OR pcc.Car15 LIKE '%' + @Car15 + '%')
					AND (@Car16 IS NULL OR pcc.Car16 LIKE '%' + @Car16 + '%')
					AND (@Car17 IS NULL OR pcc.Car17 LIKE '%' + @Car17 + '%')
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
