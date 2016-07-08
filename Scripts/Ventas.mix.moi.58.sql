/* Script con modificaciones para el módulo de ventas. Archivo 58
 * Creado: 2014/09/16
 * Subido: 2014/09/18
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
		('Reportes.CajaGastoTicket.Salida', 'D', 'I', 'Salida donde debe mostrarse el ticket de un Gasto de Caja (D - Diseño, P - Pantalla, I - Impresora).')
		, ('Reportes.NotaDeCreditoTicket.Salida', 'D', 'I', 'Salida donde debe mostrarse el ticket de una Nota de Crédito (D - Diseño, P - Pantalla, I - Impresora).')

	ALTER TABLE Autorizacion ADD SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		CONSTRAINT Df_SucursalID DEFAULT 1
	ALTER TABLE Autorizacion DROP CONSTRAINT Df_SucursalID

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

-- NotaDeCredito
ALTER TABLE NotaDeCredito ADD
	Origen NVARCHAR(32) NULL
	, Referencia NVARCHAr(32) NULL
GO
UPDATE NotaDeCredito SET Referencia = OrigenVentaID

-- AutorizacionProceso
ALTER TABLE AutorizacionProceso ADD Permiso NVARCHAR(128) NULL
GO
UPDATE AutorizacionProceso SET Permiso = CASE AutorizacionProcesoID
	WHEN 1 THEN 'Autorizaciones.Ventas.Cobro.CreditoNoAplicable'
	WHEN 2 THEN 'Autorizaciones.Ventas.9500.NoAnticipo'
	WHEN 3 THEN 'Autorizaciones.Ventas.Devoluciones.Agregar'
	WHEN 4 THEN 'Autorizaciones.Ventas.FondoDeCaja.Diferencia'
	WHEN 5 THEN 'Autorizaciones.Ventas.Egresos.Agregar'
	WHEN 6 THEN 'Autorizaciones.Ventas.Ingresos.Agregar'
	WHEN 7 THEN 'Autorizaciones.Ventas.Cambios.Agregar'
	WHEN 8 THEN 'Autorizaciones.Ventas.Refuerzo.Agregar'
	WHEN 9 THEN 'Autorizaciones.Ventas.Resguardo.Agregar'
	WHEN 10 THEN 'Autorizaciones.Ventas.Egresos.Borrar'
	WHEN 11 THEN 'Autorizaciones.Ventas.Ingresos.Borrar'
	WHEN 12 THEN 'Autorizaciones.Ventas.9500.PrecioFueraDeRango'
	WHEN 13 THEN 'Autorizaciones.Ventas.CorteDeCaja.Diferencia'
	WHEN 14 THEN 'Autorizaciones.Ventas.NotasDeCredito.Agregar'
	WHEN 15 THEN 'Autorizaciones.Ventas.Cobro.NotaDeCreditoOtroCliente'
	WHEN 16 THEN 'Autorizaciones.Ventas.Garantia.Agregar'
	END

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

ALTER VIEW [dbo].[CajaEgresosView] AS
	SELECT
		ce.CajaEgresoID
		, ce.CajaTipoEgresoID
		, cte.NombreTipoEgreso AS Tipo
		, ce.Concepto
		, ce.Importe
		, ce.Fecha
		, ce.SucursalID
		, u.NombreUsuario AS Usuario
		, ISNULL(a.Autorizado, 0) AS Autorizado
		, ISNULL(ua.NombreUsuario, 'Pendiente') AS AutorizoUsuario
		, cca.CuentaAuxiliar
	FROM
		CajaEgreso ce
		LEFT JOIN CajaTipoEgreso cte ON cte.CajaTipoEgresoID = ce.CajaTipoEgresoID AND cte.Estatus = 1
		LEFT JOIN Autorizacion a ON a.Tabla = 'CajaEgreso' AND a.TablaRegistroID = ce.CajaEgresoID AND a.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = ce.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN Usuario ua ON ua.UsuarioID = a.AutorizoUsuarioID AND ua.Estatus = 1
		LEFT JOIN ContaEgreso cne ON cne.ContaEgresoID = ce.ContaEgresoID -- AND cne.Estatus = 1
		LEFT JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = cne.ContaCuentaAuxiliarID
	WHERE ce.Estatus = 1
GO

ALTER VIEW [dbo].[NotasDeCreditoView] AS
	SELECT
		nc.NotaDeCreditoID
		, nc.FechaDeEmision
		, nc.Importe
		, nc.ClienteID
		, c.Nombre AS Cliente
		, nc.Valida
		, nc.FechaDeUso
		, nc.Observacion
		, u.NombrePersona AS Autorizo
		, v1.Folio AS OrigenVentaFolio
		, v2.Folio AS UsoVentaFolio
		, nc.Origen
		, nc.Referencia
	FROM
		NotaDeCredito nc
		LEFT JOIN Autorizacion a ON a.Tabla = 'NotaDeCredito' AND a.TablaRegistroID = nc.NotaDeCreditoID AND a.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = nc.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = a.UsuarioID AND u.Estatus = 1
		LEFT JOIN Venta v1 ON v1.VentaID = nc.OrigenVentaID AND v1.Estatus = 1
		LEFT JOIN Venta v2 ON v2.VentaID = nc.UsoVentaID AND v2.Estatus = 1
GO

ALTER VIEW [dbo].[AutorizacionesView] AS
	SELECT
		a.AutorizacionID
		, a.AutorizacionProcesoID
		, ap.Descripcion AS Tipo
		, a.FechaRegistro AS Fecha
		, a.Tabla
		, a.TablaRegistroID
		, a.Autorizado
		, a.FechaAutorizo
		, a.AutorizoUsuarioID
		, a.SucursalID
		, u.NombrePersona AS Autorizo
	FROM
		Autorizacion a
		LEFT JOIN AutorizacionProceso ap ON ap.AutorizacionProcesoID = a.AutorizacionProcesoID AND ap.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = a.AutorizoUsuarioID AND u.Estatus = 1
	WHERE a.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

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