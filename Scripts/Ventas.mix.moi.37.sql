/* Script con modificaciones para el módulo de ventas. Archivo 37
 * Creado: 2014/04/24
 * Subido: 2014/04/29
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	-- Se borran los contraints y varios campos - EstatusGenerico
	DECLARE @Constraint NVARCHAR(256) = (SELECT TOP 1 name FROM sys.foreign_keys
		WHERE parent_object_id = object_id('EstatusGenerico'))
	DECLARE @Com NVARCHAR(512) = 'ALTER TABLE EstatusGenerico DROP CONSTRAINT ' + @Constraint
	EXEC (@Com)
	SET @Constraint = (SELECT TOP 1 name FROM sys.default_constraints WHERE parent_object_id = object_id('EstatusGenerico'))
	SET @Com = 'ALTER TABLE EstatusGenerico DROP CONSTRAINT ' + @Constraint
	EXEC (@Com)
	SET @Constraint = (SELECT TOP 1 name FROM sys.default_constraints WHERE parent_object_id = object_id('EstatusGenerico'))
	SET @Com = 'ALTER TABLE EstatusGenerico DROP CONSTRAINT ' + @Constraint
	EXEC (@Com)
	SET @Constraint = (SELECT TOP 1 name FROM sys.default_constraints WHERE parent_object_id = object_id('EstatusGenerico'))
	SET @Com = 'ALTER TABLE EstatusGenerico DROP CONSTRAINT ' + @Constraint
	EXEC (@Com)
	ALTER TABLE EstatusGenerico DROP COLUMN UsuarioID, FechaRegistro, FechaModificacion, Estatus, Actualizar

	-- Se borran los contraints y varios campos - VentaDevolucionMotivo
	SET @Constraint = (SELECT TOP 1 name FROM sys.foreign_keys
		WHERE parent_object_id = object_id('VentaDevolucionMotivo'))
	SET @Com = 'ALTER TABLE VentaDevolucionMotivo DROP CONSTRAINT ' + @Constraint
	EXEC (@Com)
	SET @Constraint = (SELECT TOP 1 name FROM sys.default_constraints WHERE parent_object_id = object_id('VentaDevolucionMotivo'))
	SET @Com = 'ALTER TABLE VentaDevolucionMotivo DROP CONSTRAINT ' + @Constraint
	EXEC (@Com)
	SET @Constraint = (SELECT TOP 1 name FROM sys.default_constraints WHERE parent_object_id = object_id('VentaDevolucionMotivo'))
	SET @Com = 'ALTER TABLE VentaDevolucionMotivo DROP CONSTRAINT ' + @Constraint
	EXEC (@Com)
	SET @Constraint = (SELECT TOP 1 name FROM sys.default_constraints WHERE parent_object_id = object_id('VentaDevolucionMotivo'))
	SET @Com = 'ALTER TABLE VentaDevolucionMotivo DROP CONSTRAINT ' + @Constraint
	EXEC (@Com)
	ALTER TABLE VentaDevolucionMotivo DROP COLUMN UsuarioID, FechaRegistro, FechaModificacion, Estatus, Actualizar

	-- Se borran los contraints y varios campos - VentaEstatus
	SET @Constraint = (SELECT TOP 1 name FROM sys.foreign_keys
		WHERE parent_object_id = object_id('VentaEstatus'))
	SET @Com = 'ALTER TABLE VentaEstatus DROP CONSTRAINT ' + @Constraint
	EXEC (@Com)
	SET @Constraint = (SELECT TOP 1 name FROM sys.default_constraints WHERE parent_object_id = object_id('VentaEstatus'))
	SET @Com = 'ALTER TABLE VentaEstatus DROP CONSTRAINT ' + @Constraint
	EXEC (@Com)
	SET @Constraint = (SELECT TOP 1 name FROM sys.default_constraints WHERE parent_object_id = object_id('VentaEstatus'))
	SET @Com = 'ALTER TABLE VentaEstatus DROP CONSTRAINT ' + @Constraint
	EXEC (@Com)
	SET @Constraint = (SELECT TOP 1 name FROM sys.default_constraints WHERE parent_object_id = object_id('VentaEstatus'))
	SET @Com = 'ALTER TABLE VentaEstatus DROP CONSTRAINT ' + @Constraint
	EXEC (@Com)
	DROP INDEX Ix_VentaEstatus_Estatus ON VentaEstatus
	ALTER TABLE VentaEstatus DROP COLUMN UsuarioID, FechaRegistro, FechaModificacion, Estatus, Actualizar

	-- Creación de Índices de Estatus y otros para rendiemiento
	CREATE INDEX Ix_Autorizacion_Estatus ON Autorizacion(Estatus)
	CREATE INDEX Ix_Autorizacion_TablaRegistroID ON Autorizacion(TablaRegistroID)
	CREATE INDEX Ix_AutorizacionProceso_Estatus ON AutorizacionProceso(Estatus)
	CREATE INDEX Ix_CajaEfectivoPorDia_Estatus ON CajaEfectivoPorDia(Estatus)
	CREATE INDEX Ix_CajaEgreso_Estatus ON CajaEgreso(Estatus)
	CREATE INDEX Ix_CajaIngreso_Estatus ON CajaIngreso(Estatus)
	CREATE INDEX Ix_CajaTipoEgreso_Estatus ON CajaTipoEgreso(Estatus)
	CREATE INDEX Ix_CajaTipoIngreso_Estatus ON CajaTipoIngreso(Estatus)
	CREATE INDEX Ix_CajaVistoBueno_CajaTipoEgreso ON CajaVistoBueno(TablaRegistroID)
	CREATE INDEX Ix_ClienteFacturacion_Estatus ON ClienteFacturacion(Estatus)
	-- CREATE INDEX Ix_ClienteFacturacion_ClienteID ON ClienteFacturacion(ClienteID)
	CREATE INDEX Ix_ClienteFlotilla_Estatus ON ClienteFlotilla(Estatus)
	CREATE INDEX Ix_ClienteFlotilla_ClienteID ON ClienteFlotilla(ClienteID)
	CREATE INDEX Ix_CobranzaTicket_VentaPagoID ON CobranzaTicket(VentaPagoID)
	CREATE INDEX Ix_Cotizacion9500_Estatus ON Cotizacion9500(Estatus)
	CREATE INDEX Ix_Cotizacion9500Detalle_Estatus ON Cotizacion9500Detalle(Estatus)
	CREATE INDEX Ix_Cotizacion9500Detalle_Cotizacion9500ID ON Cotizacion9500Detalle(Cotizacion9500ID)
	CREATE INDEX Ix_NotaDeCredito_Estatus ON NotaDeCredito(Estatus)
	CREATE INDEX Ix_ParteMaxMin_Calcular ON ParteMaxMin(Calcular)
	CREATE INDEX Ix_ParteMaxMinRegla_Estatus ON ParteMaxMinRegla(Estatus)
	CREATE INDEX Ix_ReporteDeFaltante_Estatus ON ReporteDeFaltante(Estatus)
	CREATE INDEX Ix_VentaCambio_Estatus ON VentaCambio(Estatus)
	CREATE INDEX Ix_VentaDevolucion_Estatus ON VentaDevolucion(Estatus)
	CREATE INDEX Ix_VentaDevolucion_VentaID ON VentaDevolucion(VentaID)
	CREATE INDEX Ix_VentaDevolucionDetalle_Estatus ON VentaDevolucionDetalle(Estatus)
	CREATE INDEX Ix_VentaDevolucionDetalle_VentaDevolucionID ON VentaDevolucionDetalle(VentaDevolucionID)
	CREATE INDEX Ix_VentaFacturaDevolucion_Estatus ON VentaFacturaDevolucion(Estatus)
	CREATE INDEX Ix_VentaFacturaDevolucion_VentaFacturaID ON VentaFacturaDevolucion(VentaFacturaID)
	CREATE INDEX Ix_VentaFacturaDevolucionDetalle_Estatus ON VentaFacturaDevolucionDetalle(Estatus)
	CREATE INDEX Ix_VentaFacturaDevolucionDetalle_VentaFacturaDevolucionID ON VentaFacturaDevolucionDetalle(VentaFacturaDevolucionID)
	
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
		LEFT JOIN VentaEstatus ve ON ve.VentaEstatusID = v.VentaEstatusID
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

ALTER VIEW VentasDevolucionesView AS
	SELECT
		vd.VentaDevolucionID
		, ISNULL(CONVERT(BIT, CASE WHEN vfdv.FolioFiscal IS NULL THEN 0 ELSE 1 END), 0) AS Facturada
		, vd.VentaID
		, v.Folio
		, v.Fecha AS FechaDeVenta
		, ISNULL(CONVERT(BIT, v.ACredito), 0) AS VentaACredito
		, vd.Fecha
		, vd.MotivoID
		, vdm.Descripcion AS Motivo
		, vd.Observacion
		, vd.SucursalID
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

CREATE VIEW ContaCuentasAuxiliaresView AS
	SELECT
		ca.ContaCuentaAuxiliarID
		-- , CuentaAuxiliar
		, (cm.CuentaDeMayor + ' - ' + ca.CuentaAuxiliar) AS CuentaDeMayorCuentaAuxiliar
		, ca.VisibleEnCaja
	FROM
		ContaCuentaAuxiliar ca
		LEFT JOIN ContaCuentaDeMayor cm ON cm.ContaCuentaDeMayorID = ca.ContaCuentaDeMayorID
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

-- EXEC pauVentasPartesBusqueda @SucursalID = 1, @Descripcion1 = 'camisa', @Descripcion2 = 'car', @Equivalentes = 1
ALTER PROCEDURE pauVentasPartesBusqueda (
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
	, @TipoCilindroID INT = NULL
	, @Largo INT = NULL
	, @Alto INT = NULL
	, @Dientes INT = NULL
	, @Amperes INT = NULL
	, @Watts INT = NULL
	, @Diametro INT = NULL
	, @Astrias INT = NULL
	, @Terminales INT = NULL
	, @Voltios INT = NULL
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
					AND (@TipoCilindroID IS NULL OR p.TipoCilindroID = @TipoCilindroID)
					AND (@Largo IS NULL OR p.Largo = @Largo)
					AND (@Alto IS NULL OR p.Alto = @Alto)
					AND (@Dientes IS NULL OR p.Dientes = @Dientes)
					AND (@Amperes IS NULL OR p.Amperes = @Amperes)
					AND (@Watts IS NULL OR p.Watts = @Watts)
					AND (@Diametro IS NULL OR p.Diametro = @Diametro)
					AND (@Astrias IS NULL OR p.Astrias = @Astrias)
					AND (@Terminales IS NULL OR p.Terminales = @Terminales)
					AND (@Voltios IS NULL OR p.Voltios = @Voltios)
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
				, pe.Existencia
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
				LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
			WHERE e.Fila = 1

		-- Si es búsqueda normal
		END ELSE BEGIN
			SELECT
				p.ParteID
				, p.NumeroParte AS NumeroDeParte
				, p.NombreParte AS Descripcion
				, mp.NombreMarcaParte AS Marca
				, l.NombreLinea AS Linea
				, pe.Existencia
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
				LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
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
				AND (@TipoCilindroID IS NULL OR p.TipoCilindroID = @TipoCilindroID)
				AND (@Largo IS NULL OR p.Largo = @Largo)
				AND (@Alto IS NULL OR p.Alto = @Alto)
				AND (@Dientes IS NULL OR p.Dientes = @Dientes)
				AND (@Amperes IS NULL OR p.Amperes = @Amperes)
				AND (@Watts IS NULL OR p.Watts = @Watts)
				AND (@Diametro IS NULL OR p.Diametro = @Diametro)
				AND (@Astrias IS NULL OR p.Astrias = @Astrias)
				AND (@Terminales IS NULL OR p.Terminales = @Terminales)
				AND (@Voltios IS NULL OR p.Voltios = @Voltios)
				
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
		END
	
	-- Si es búsqueda por código
	END ELSE BEGIN
		SELECT
			p.ParteID
			, p.NumeroParte AS NumeroDeParte
			, p.NombreParte AS Descripcion
			, mp.NombreMarcaParte AS Marca
			, l.NombreLinea AS Linea
			, pe.Existencia
			, pi.NombreImagen AS Imagen
			, (SELECT COUNT(*) FROM ParteImagen WHERE ParteID = p.ParteID AND Estatus = 1) AS CuentaImagenes
		FROM
			Parte p
			LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
			LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
			LEFT JOIN ParteImagen pi ON pi.ParteID = p.ParteID AND pi.Orden = 1
			LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
		WHERE
			p.Estatus = 1
			AND (NumeroParte = @Codigo
			OR CodigoBarra = @Codigo)
	END

END
GO

-- EXEC pauPartesMaxMinEquivalentes '2013-04-01', '2014-03-31'
ALTER PROCEDURE pauPartesMaxMinEquivalentes (
	@Desde DATE
	, @Hasta DATE
	, @ProveedorID INT = NULL
	, @MarcaID INT = NULL
	, @LineaID INT = NULL
) AS BEGIN
	SET NOCOUNT ON

	/* DECLARE @Desde DATE = '2013-01-01'
	DECLARE @Hasta DATE = '2014-01-31'
	*/

	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3
	DECLARE @SucMatrizID INT = 1

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)

	SELECT
		pe.ParteEquivalenteID
		, pe.GrupoID
		, pe.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, pv.NombreProveedor AS Proveedor
		, mr.NombreMarcaParte AS Marca
		, ISNULL(SUM(vd.Cantidad), 0.00) AS Cantidad
		, ISNULL(pp.CostoConDescuento, pp.Costo) AS CostoConDescuento
		, pmm.Calcular
		, pmm.FechaModificacion
	FROM
		ParteEquivalente pe
		INNER JOIN Parte p ON p.ParteID = pe.ParteID AND p.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = pe.ParteID AND pp.Estatus = 1
		LEFT JOIN ParteMaxMin pmm ON pmm.ParteID = pe.ParteID AND pmm.SucursalID = @SucMatrizID
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mr ON mr.MarcaParteID = p.MarcaParteID AND mr.Estatus = 1
		LEFT JOIN VentaDetalle vd ON vd.ParteID = pe.ParteID AND vd.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = vd.VentaID
			AND v.VentaEstatusID = @EstPagadaID
			AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			AND v.Estatus = 1
	WHERE
		(@ProveedorID IS NULL OR p.ProveedorID = @ProveedorID)
		AND (@MarcaID IS NULL OR p.MarcaParteID = @MarcaID)
		AND (@LineaID IS NULL OR p.LineaID = @LineaID)
	GROUP BY
		pe.ParteEquivalenteID
		, pe.GrupoID
		, pe.ParteID
		, p.NumeroParte
		, p.NombreParte
		, pv.NombreProveedor
		, mr.NombreMarcaParte
		, pp.CostoConDescuento
		, pp.Costo
		, pmm.Calcular
		, pmm.FechaModificacion
	ORDER BY
		pe.GrupoID
		, p.NumeroParte
END
GO
