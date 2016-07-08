/* Script con modificaciones para el módulo de ventas. Archivo 79
 * Creado: 2015/01/08
 * Subido: 2015/01/12
 */


----------------------------------------------------------------------------------- Código de André

----------------------------------------------------------------------------------- Código de André


/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	

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

ALTER VIEW [dbo].[PartesVentasView] AS
	SELECT
		p.ParteID
		, p.ParteEstatusID
		, p.CodigoBarra
		, p.NumeroParte
		, p.NombreParte
		, p.EsServicio
		, p.Es9500
		, p.AGranel
		, mp.NombreMarcaParte AS Marca
		, pp.PrecioUno
		, pp.PrecioDos
		, pp.PrecioTres
		, pp.PrecioCuatro
		, pp.PrecioCinco
		, pe1.Existencia AS ExistenciaSuc01
		, pe2.Existencia AS ExistenciaSuc02
		, pe3.Existencia AS ExistenciaSuc03
		, pp.Costo
		, pp.CostoConDescuento
	FROM
		Parte p
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID
		LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND pp.Estatus = 1
		LEFT JOIN ParteExistencia pe1 ON pe1.ParteID = p.ParteID AND pe1.SucursalID = 1
			AND pe1.Estatus = 1
		LEFT JOIN ParteExistencia pe2 ON pe2.ParteID = p.ParteID AND pe2.SucursalID = 2
			AND pe2.Estatus = 1
		LEFT JOIN ParteExistencia pe3 ON pe3.ParteID = p.ParteID AND pe3.SucursalID = 3
			AND pe3.Estatus = 1
	WHERE
		p.Estatus = 1
GO

ALTER VIEW [dbo].[PartesBusquedaEnMovimientosView] AS
	SELECT
		Parte.ParteID
		, Parte.ParteEstatusID
		,MarcaParte.MarcaParteID
		,Linea.LineaID
		,Parte.NumeroParte
		,Parte.NombreParte	
		,Linea.NombreLinea AS Linea		
		,MarcaParte.NombreMarcaParte AS Marca
		,PartePrecio.PartePrecioID
		,PartePrecio.Costo
		,PartePrecio.PorcentajeUtilidadUno
		,PartePrecio.PorcentajeUtilidadDos
		,PartePrecio.PorcentajeUtilidadTres
		,PartePrecio.PorcentajeUtilidadCuatro
		,PartePrecio.PorcentajeUtilidadCinco
		,PartePrecio.PrecioUno
		,PartePrecio.PrecioDos
		,PartePrecio.PrecioTres
		,PartePrecio.PrecioCuatro
		,PartePrecio.PrecioCinco
		,Parte.Etiqueta
		,Parte.SoloUnaEtiqueta
		,Parte.NumeroParte
			+ Parte.NombreParte	
			+ Linea.NombreLinea 	
			+ MarcaParte.NombreMarcaParte AS Busqueda
	FROM
		Parte
		INNER JOIN Linea ON Linea.LineaID = Parte.LineaID
		INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = Parte.MarcaParteID
		INNER JOIN PartePrecio ON PartePrecio.ParteID = Parte.ParteID
	WHERE
		Parte.Estatus = 1
GO

ALTER VIEW [dbo].[PartesMaxMinView] AS
	SELECT
		p.ParteID
		, p.ParteEstatusID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, pmm.SucursalID
		, pmm.Calcular
		, pmm.VentasGlobales
		, p.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, p.MarcaParteID
		, mp.NombreMarcaParte AS Marca
		, p.LineaID
		, l.NombreLinea AS Linea
	FROM
		Parte p
		LEFT JOIN ParteMaxMin pmm ON pmm.ParteID = p.ParteID
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
	/* ORDER BY
		pv.NombreProveedor
		, mp.NombreMarcaParte
		, l.NombreLinea
		, p.NombreParte
		
	*/
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

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
	DECLARE @ParteEstActivo INT = 1

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
					AND ParteEstatusID = @ParteEstActivo
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
				AND ParteEstatusID = @ParteEstActivo
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
			AND ParteEstatusID = @ParteEstActivo
			AND (NumeroParte = @Codigo OR CodigoBarra = @Codigo)
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
	
	DECLARE @ParteEstActivo INT = 1
	
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
			p.Estatus = 1
			AND p.ParteEstatusID = @ParteEstActivo
			AND (@Descripcion1 IS NULL OR p.NombreParte LIKE '%' + @Descripcion1 + '%')
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
			p.Estatus = 1
			AND p.ParteEstatusID = @ParteEstActivo
			AND p.NumeroParte = @Codigo
	END

END
GO

ALTER PROCEDURE [dbo].[pauPartesMovimientosDevoluciones] (
	@ProveedorID INT
	, @Desde DATE
	, @Hasta DATE
	, @Garantias BIT = 0
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
) AS BEGIN
	SET NOCOUNT ON

	DECLARE @OpCompra INT = 1
	DECLARE @EstGenRecibido INT = 8
	DECLARE @ParteEstActivo INT = 1

	SET @Hasta = DATEADD(DAY, 1, @Hasta)

	IF @Garantias = 1 BEGIN
	
		-- Búsqueda por descripción
		IF @Codigo IS NULL BEGIN
			SELECT
				vg.VentaGarantiaID AS Id
				, v.Folio
				, vg.ParteID
				, p.NumeroParte AS NumeroDeParte
				, p.NombreParte AS Descripcion
				, pp.Costo
				, mp.NombreMarcaParte AS Marca
				, l.NombreLinea AS Linea
			FROM
				VentaGarantia vg
				INNER JOIN Parte p ON p.ParteID = vg.ParteID AND p.Estatus = 1
				LEFT JOIN PartePrecio pp ON pp.ParteID = vg.ParteID AND pp.Estatus = 1
				LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
				LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
				LEFT JOIN Venta v ON v.VentaID = vg.VentaID AND v.Estatus = 1
			WHERE
				vg.Estatus = 1
				AND vg.EstatusGenericoID = @EstGenRecibido
				AND p.ProveedorID = @ProveedorID
				AND (
					(@Descripcion1 IS NULL OR p.NombreParte LIKE '%' + @Descripcion1 + '%')
					AND (@Descripcion2 IS NULL OR p.NombreParte LIKE '%' + @Descripcion2 + '%')
					AND (@Descripcion3 IS NULL OR p.NombreParte LIKE '%' + @Descripcion3 + '%')
					AND (@Descripcion4 IS NULL OR p.NombreParte LIKE '%' + @Descripcion4 + '%')
					AND (@Descripcion5 IS NULL OR p.NombreParte LIKE '%' + @Descripcion5 + '%')
					AND (@Descripcion6 IS NULL OR p.NombreParte LIKE '%' + @Descripcion6 + '%')
					AND (@Descripcion7 IS NULL OR p.NombreParte LIKE '%' + @Descripcion7 + '%')
					AND (@Descripcion8 IS NULL OR p.NombreParte LIKE '%' + @Descripcion8 + '%')
					AND (@Descripcion9 IS NULL OR p.NombreParte LIKE '%' + @Descripcion9 + '%')
				)
				AND p.ParteEstatusID = @ParteEstActivo
		
		-- Búsqueda por código			
		END ELSE BEGIN
			SELECT DISTINCT
				vg.VentaGarantiaID AS Id
				, v.Folio
				, vg.ParteID
				, p.NumeroParte AS NumeroDeParte
				, p.NombreParte AS Descripcion
				, pp.Costo
				, mp.NombreMarcaParte AS Marca
				, l.NombreLinea AS Linea
			FROM
				VentaGarantia vg
				INNER JOIN Parte p ON p.ParteID = vg.ParteID AND p.Estatus = 1
				LEFT JOIN PartePrecio pp ON pp.ParteID = vg.ParteID AND pp.Estatus = 1
				LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
				LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
				LEFT JOIN Venta v ON v.VentaID = vg.VentaID AND v.Estatus = 1
			WHERE
				vg.Estatus = 1
				AND vg.EstatusGenericoID = @EstGenRecibido
				AND p.ProveedorID = @ProveedorID
				AND p.NumeroParte = @Codigo
				AND p.ParteEstatusID = @ParteEstActivo
		END
	
	END ELSE BEGIN

		-- Búsqueda por descripción
		IF @Codigo IS NULL BEGIN
			SELECT DISTINCT
				0 AS Id
				, '' AS Folio
				, mid.ParteID
				, p.NumeroParte AS NumeroDeParte
				, p.NombreParte AS Descripcion
				, pp.Costo
				, mp.NombreMarcaParte AS Marca
				, l.NombreLinea AS Linea
			FROM
				MovimientoInventarioDetalle mid
				INNER JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = mid.MovimientoInventarioID AND mi.Estatus = 1
				INNER JOIN Parte p ON p.ParteID = mid.ParteID AND p.Estatus = 1
				LEFT JOIN PartePrecio pp ON pp.ParteID = mid.ParteID AND pp.Estatus = 1
				LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
				LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
			WHERE
				mid.Estatus = 1
				AND mi.TipoOperacionID = @OpCompra
				AND mi.ProveedorID = @ProveedorID
				AND (mi.FechaFactura >= @Desde AND mi.FechaFactura < @Hasta)
				AND (mid.Cantidad - mid.CantidadDevuelta) > 0
				AND (			
					(@Descripcion1 IS NULL OR p.NombreParte LIKE '%' + @Descripcion1 + '%')
					AND (@Descripcion2 IS NULL OR p.NombreParte LIKE '%' + @Descripcion2 + '%')
					AND (@Descripcion3 IS NULL OR p.NombreParte LIKE '%' + @Descripcion3 + '%')
					AND (@Descripcion4 IS NULL OR p.NombreParte LIKE '%' + @Descripcion4 + '%')
					AND (@Descripcion5 IS NULL OR p.NombreParte LIKE '%' + @Descripcion5 + '%')
					AND (@Descripcion6 IS NULL OR p.NombreParte LIKE '%' + @Descripcion6 + '%')
					AND (@Descripcion7 IS NULL OR p.NombreParte LIKE '%' + @Descripcion7 + '%')
					AND (@Descripcion8 IS NULL OR p.NombreParte LIKE '%' + @Descripcion8 + '%')
					AND (@Descripcion9 IS NULL OR p.NombreParte LIKE '%' + @Descripcion9 + '%')
				)
				AND p.ParteEstatusID = @ParteEstActivo
		
		-- Búsqueda por código			
		END ELSE BEGIN
			SELECT DISTINCT
				0 AS Id
				, '' AS Folio
				, mid.ParteID
				, p.NumeroParte AS NumeroDeParte
				, p.NombreParte AS Descripcion
				, pp.Costo
				, mp.NombreMarcaParte AS Marca
				, l.NombreLinea AS Linea
			FROM
				MovimientoInventarioDetalle mid
				INNER JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = mid.MovimientoInventarioID AND mi.Estatus = 1
				INNER JOIN Parte p ON p.ParteID = mid.ParteID AND p.Estatus = 1
				LEFT JOIN PartePrecio pp ON pp.ParteID = mid.ParteID AND pp.Estatus = 1
				LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
				LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
			WHERE
				mid.Estatus = 1
				AND mi.TipoOperacionID = @OpCompra
				AND mi.ProveedorID = @ProveedorID
				AND (mi.FechaFactura >= @Desde AND mi.FechaFactura < @Hasta)
				AND (mid.Cantidad - mid.CantidadDevuelta) > 0
				AND p.NumeroParte = @Codigo
				AND p.ParteEstatusID = @ParteEstActivo
		END
	END

END
GO

ALTER PROCEDURE [dbo].[pauParteBusquedaAvanzadaEnTraspasos] (
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
	, @SucursalOrigenID INT = NULL
	, @SucursalDestinoID INT = NULL
) AS BEGIN
	SET NOCOUNT ON

	DECLARE @IdTipoFuenteMostrador INT = 4

	IF @Codigo IS NULL BEGIN
		SELECT
			Parte.ParteID
			,Parte.NumeroParte
			,Parte.NombreParte
			,Parte.MarcaParteID	
			,Parte.LineaID	
			,Sistema.SistemaID
			,Parte.SubsistemaID
			,ExistenciaOrigen.SucursalID
			,ExistenciaOrigen.Existencia
			,MxMnOrigen.Maximo
			,MxMnOrigen.Minimo
			,ExistenciaDestino.SucursalID AS SucursalDestinoID
			,ExistenciaDestino.Existencia AS DestinoExistencia
			,MxMnDestino.Maximo AS DestinoMaximo
			,MxMnDestino.Minimo AS DestinoMinimo
			,ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) AS Sugerencia
		FROM
			Parte	
			INNER JOIN Subsistema ON Subsistema.SubsistemaID = Parte.SubsistemaID
			INNER JOIN Sistema ON Sistema.SistemaID = Subsistema.SistemaID
			INNER JOIN ParteExistencia AS ExistenciaOrigen ON ExistenciaOrigen.ParteID = Parte.ParteID AND ExistenciaOrigen.SucursalID = @SucursalOrigenID
			INNER JOIN ParteExistencia AS ExistenciaDestino ON ExistenciaDestino.ParteID = Parte.ParteID AND ExistenciaDestino.SucursalID = @SucursalDestinoID
			INNER JOIN ParteMaxMin AS MxMnOrigen ON MxMnOrigen.ParteID = Parte.ParteID AND MxMnOrigen.SucursalID = @SucursalOrigenID
			INNER JOIN ParteMaxMin AS MxMnDestino ON MxMnDestino.ParteID = Parte.ParteID AND MxMnDestino.SucursalID = @SucursalDestinoID
		WHERE
			Parte.Estatus = 1
			AND ExistenciaOrigen.SucursalID = @SucursalOrigenID
			AND ExistenciaDestino.SucursalID = @SucursalDestinoID	
			--AND ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) > 0
			--AND ExistenciaOrigen.Existencia > 0	
			AND Parte.ParteID NOT IN (SELECT d.ParteID FROM MovimientoInventario m INNER JOIN MovimientoInventarioDetalle d ON m.MovimientoInventarioID = d.MovimientoInventarioID WHERE m.TipoOperacionID = 5 AND m.SucursalDestinoID = @SucursalDestinoID AND m.FechaRecepcion IS NULL AND m.Estatus = 1)		
			AND (@Descripcion1 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion1 + '%')
			AND (@Descripcion2 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion2 + '%')
			AND (@Descripcion3 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion3 + '%')
			AND (@Descripcion4 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion4 + '%')
			AND (@Descripcion5 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion5 + '%')
			AND (@Descripcion6 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion6 + '%')
			AND (@Descripcion7 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion7 + '%')
			AND (@Descripcion8 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion8 + '%')
			AND (@Descripcion9 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion9 + '%')
	END ELSE BEGIN
		SELECT
			Parte.ParteID
			,Parte.NumeroParte
			,Parte.NombreParte
			,Parte.MarcaParteID	
			,Parte.LineaID	
			,Sistema.SistemaID
			,Parte.SubsistemaID
			,ExistenciaOrigen.SucursalID
			,ExistenciaOrigen.Existencia
			,MxMnOrigen.Maximo
			,MxMnOrigen.Minimo
			,ExistenciaDestino.SucursalID AS SucursalDestinoID
			,ExistenciaDestino.Existencia AS DestinoExistencia
			,MxMnDestino.Maximo AS DestinoMaximo
			,MxMnDestino.Minimo AS DestinoMinimo
			,ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) AS Sugerencia
		FROM
			Parte	
			INNER JOIN Subsistema ON Subsistema.SubsistemaID = Parte.SubsistemaID
			INNER JOIN Sistema ON Sistema.SistemaID = Subsistema.SistemaID
			INNER JOIN ParteExistencia AS ExistenciaOrigen ON ExistenciaOrigen.ParteID = Parte.ParteID AND ExistenciaOrigen.SucursalID = @SucursalOrigenID
			INNER JOIN ParteExistencia AS ExistenciaDestino ON ExistenciaDestino.ParteID = Parte.ParteID AND ExistenciaDestino.SucursalID = @SucursalDestinoID
			INNER JOIN ParteMaxMin AS MxMnOrigen ON MxMnOrigen.ParteID = Parte.ParteID AND MxMnOrigen.SucursalID = @SucursalOrigenID
			INNER JOIN ParteMaxMin AS MxMnDestino ON MxMnDestino.ParteID = Parte.ParteID AND MxMnDestino.SucursalID = @SucursalDestinoID
		WHERE
			Parte.Estatus = 1
			AND ExistenciaOrigen.SucursalID = @SucursalOrigenID
			AND ExistenciaDestino.SucursalID = @SucursalDestinoID	
			AND (Parte.NumeroParte = @Codigo OR Parte.CodigoBarra = @Codigo)
			AND Parte.ParteID NOT IN (SELECT d.ParteID FROM MovimientoInventario m INNER JOIN MovimientoInventarioDetalle d ON m.MovimientoInventarioID = d.MovimientoInventarioID WHERE m.TipoOperacionID = 5 AND m.SucursalDestinoID = @SucursalDestinoID AND m.FechaRecepcion IS NULL AND m.Estatus = 1)
			--AND ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) > 0
			--AND ExistenciaOrigen.Existencia > 0
	END

END
GO

ALTER PROCEDURE [dbo].[pauParteBusquedaEnTraspasos] (
	@MarcaParteID INT
	, @LineaID INT
	, @ProveedorID INT
	, @SucursalOrigenID INT
	, @SucursalDestinoID INT
	, @Abcs NVARCHAR(20)
) AS BEGIN

	--DECLARE @MarcaParteID INT = 0 --12
	--DECLARE @LineaID INT = 0 --180
	--DECLARE @ProveedorID INT = 0 --10
	--DECLARE @SucursalOrigenID INT = 1 
	--DECLARE @SucursalDestinoID INT = 3
	--DECLARE @Abcs NVARCHAR(20) = 'A'

	DECLARE @PedNoSurtido INT = 2
	DECLARE @OpTraspaso INT = 5

	SELECT
		Parte.ParteID
		,Parte.NumeroParte
		,Parte.NombreParte
		,Parte.MarcaParteID	
		,Parte.LineaID	
		,Sistema.SistemaID
		,Parte.SubsistemaID
		, pv.NombreProveedor AS Proveedor
		,ExistenciaOrigen.SucursalID
		,ExistenciaOrigen.Existencia
		,MxMnOrigen.Maximo
		,MxMnOrigen.Minimo
		,ExistenciaDestino.SucursalID AS SucursalDestinoID
		,ExistenciaDestino.Existencia AS DestinoExistencia
		,MxMnDestino.Maximo AS DestinoMaximo
		,MxMnDestino.Minimo AS DestinoMinimo
		,ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) AS Sugerencia
		,Parte.NumeroParte + Parte.NombreParte AS Busqueda
	FROM
		Parte	
		INNER JOIN Subsistema ON Subsistema.SubsistemaID = Parte.SubsistemaID
		INNER JOIN Sistema ON Sistema.SistemaID = Subsistema.SistemaID
		INNER JOIN ParteExistencia AS ExistenciaOrigen ON ExistenciaOrigen.ParteID = Parte.ParteID AND ExistenciaOrigen.SucursalID = @SucursalOrigenID
		INNER JOIN ParteExistencia AS ExistenciaDestino ON ExistenciaDestino.ParteID = Parte.ParteID AND ExistenciaDestino.SucursalID = @SucursalDestinoID
		INNER JOIN ParteMaxMin AS MxMnOrigen ON MxMnOrigen.ParteID = Parte.ParteID AND MxMnOrigen.SucursalID = @SucursalOrigenID
		INNER JOIN ParteMaxMin AS MxMnDestino ON MxMnDestino.ParteID = Parte.ParteID AND MxMnDestino.SucursalID = @SucursalDestinoID
		INNER JOIN ParteAbc ON ParteAbc.ParteID = Parte.ParteID
		LEFT JOIN Proveedor pv ON pv.ProveedorID = Parte.ProveedorID AND pv.Estatus = 1
		LEFT JOIN PedidoDetalle pd ON pd.ParteID = Parte.ParteID AND pd.PedidoEstatusID = @PedNoSurtido AND pd.Estatus = 1
	WHERE
		Parte.Estatus = 1
		AND ExistenciaOrigen.SucursalID = @SucursalOrigenID
		AND ExistenciaDestino.SucursalID = @SucursalDestinoID	
		AND (@MarcaParteID <= 0 OR Parte.MarcaParteID = @MarcaParteID)
		AND (@LineaID <= 0 OR Parte.LineaID = @LineaID)
		AND (@ProveedorID <= 0 OR Parte.ProveedorID = @ProveedorID)
		AND ParteAbc.AbcDeVentas IN (SELECT * FROM dbo.fnuDividirCadena(@Abcs, ','))
		AND ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) > 0
		AND ExistenciaOrigen.Existencia > 0
		AND ExistenciaDestino.Existencia <= MxMnDestino.Minimo
		AND Parte.ParteID NOT IN (
			SELECT d.ParteID
			FROM
				MovimientoInventario m
				INNER JOIN MovimientoInventarioDetalle d ON m.MovimientoInventarioID = d.MovimientoInventarioID
			WHERE
				m.TipoOperacionID = @OpTraspaso
				AND m.SucursalDestinoID = @SucursalDestinoID
				AND m.FechaRecepcion IS NULL
				AND m.Estatus = 1
		)
		AND pd.PedidoDetalleID IS NULL

END
GO

ALTER PROCEDURE [dbo].[pauPartesMaxMin] (
	@SucursalID INT
	, @Desde DATE
	, @Hasta DATE
	, @ProveedorID INT = NULL
	, @MarcaID INT = NULL
	, @LineaID INT = NULL
) AS BEGIN
	SET NOCOUNT ON

	/* DECLARE @SucursalID INT = 1
	DECLARE @Desde DATE = '2013-02-01'
	DECLARE @Hasta DATE = '2014-03-31'
	DECLARE @ProveedorID INT = NULL
	DECLARE @MarcaID INT = NULL
	DECLARE @LineaID INT = NULL
	*/

	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3
	DECLARE @ParteEstActivo INT = 1

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)

	-- Procedimiento
	
	-- Ventas
	;WITH _Ventas AS (
		SELECT
			vd.ParteID
			, CONVERT(DATE, v.Fecha) AS Fecha
			, COUNT(v.VentaID) AS Ventas
			, SUM(vd.Cantidad) AS Cantidad
			, SUM((vd.PrecioUnitario - pp.Costo) * vd.Cantidad) AS Utilidad
		FROM
			VentaDetalle vd
			INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
			LEFT JOIN PartePrecio pp ON pp.ParteID = vd.ParteID AND pp.Estatus = 1
		WHERE
			vd.Estatus = 1
			AND v.VentaEstatusID = @EstPagadaID
			AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			AND v.SucursalID = @SucursalID
		GROUP BY
			vd.ParteID
			, CONVERT(DATE, v.Fecha)

		-- Se agrega lo negado
		UNION ALL
		SELECT
			rf.ParteID
			, CONVERT(DATE, rf.FechaRegistro) AS Fecha
			, COUNT(rf.ReporteDeFaltanteID) AS Ventas
			, SUM(rf.CantidadRequerida) AS Cantidad
			, SUM((pp.PrecioUno - pp.Costo) * rf.CantidadRequerida) AS Utilidad
		FROM
			ReporteDeFaltante rf
			LEFT JOIN PartePrecio pp ON pp.ParteID = rf.ParteID AND pp.Estatus = 1
			-- Para validar que no se haya vendido en el día o en el siguiente
			LEFT JOIN (
				SELECT DISTINCT
					vd.ParteID
					, CONVERT(DATE, v.Fecha) AS Dia
				FROM
					VentaDetalle vd
					INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
				WHERE
					vd.Estatus = 1
					AND v.VentaEstatusID = @EstPagadaID
					AND v.SucursalID = @SucursalID
			) vdc ON vdc.ParteID = rf.ParteID AND (rf.FechaRegistro >= vdc.Dia AND rf.FechaRegistro < DATEADD(d, 2, vdc.Dia))
		WHERE
			rf.Estatus = 1
			AND (rf.FechaRegistro >= @Desde AND rf.FechaRegistro < @Hasta)
			AND rf.SucursalID = @SucursalID
			-- Parte de Venta
			AND vdc.ParteID IS NULL
		GROUP BY
			rf.ParteID
			, CONVERT(DATE, rf.FechaRegistro)
		-- Fin lo negado
	)
	-- Ventas por semana
	, _VentasPorSem AS (
		SELECT
			ParteID
			, DATEPART(WK, Fecha) AS Semana
			, SUM(Cantidad) AS Cantidad
		FROM _Ventas
		GROUP BY
			ParteID
			, DATEPART(WK, Fecha)
	)
	-- Ventas por mes
	, _VentasPorMes AS (
		SELECT
			ParteID
			, MONTH(Fecha) AS Mes
			, SUM(Cantidad) AS Cantidad
		FROM _Ventas
		GROUP BY
			ParteID
			, MONTH(Fecha)
	)

	-- Consulta final
	SELECT
		p.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, p.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, p.MarcaParteID
		, mp.NombreMarcaParte AS Marca
		, p.LineaID
		, l.NombreLinea AS Linea
		, p.UnidadEmpaque
		, p.EsPar
		, p.TiempoReposicion
		, pe.Existencia
		, pa.AbcDeVentas
		, pa.AbcDeUtilidad
		, pa.AbcDeNegocio
		, pa.AbcDeProveedor
		, pa.AbcDeLinea
		, pmm.Fijo
		, pmm.Minimo
		, pmm.Maximo
		, pmm.FechaCalculo

		, ISNULL(SUM(vc.Ventas), 0) AS VentasTotal
		, ISNULL(SUM(vc.Cantidad), 0.00) AS CantidadTotal
		, ISNULL(MAX(vc.Cantidad), 0.00) AS CantidadMaxDia
		, ISNULL(MAX(vcs.Cantidad), 0.00) AS CantidadMaxSem
		, ISNULL(MAX(vcm.Cantidad), 0.00) AS CantidadMaxMes
		, ISNULL(SUM(vc.Utilidad), 0.00) AS UtilidadTotal
		, ISNULL(pmm.VentasGlobales, 0) AS VentasGlobales
	FROM
		Parte p
		INNER JOIN ParteMaxMin pmm ON pmm.ParteID = p.ParteID AND pmm.SucursalID = @SucursalID
		LEFT JOIN _Ventas vc ON vc.ParteID = p.ParteID
		LEFT JOIN (
			SELECT ParteID, MAX(Cantidad) AS Cantidad FROM _VentasPorSem GROUP BY ParteID
		) vcs ON vcs.ParteID = p.ParteID
		LEFT JOIN (
			SELECT ParteID, MAX(Cantidad) AS Cantidad FROM _VentasPorMes GROUP BY ParteID
		) vcm ON vcm.ParteID = p.ParteID
		LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
		LEFT JOIN ParteAbc pa ON pa.ParteID = p.ParteID
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
	WHERE
		p.Estatus = 1
		AND p.ParteEstatusID = @ParteEstActivo
		AND pmm.Calcular = 1
		AND (@ProveedorID IS NULL OR p.ProveedorID = @ProveedorID)
		AND (@MarcaID IS NULL OR p.MarcaParteID = @MarcaID)
		AND (@LineaID IS NULL OR p.LineaID = @LineaID)
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
		, p.UnidadEmpaque
		, p.EsPar
		, p.TiempoReposicion
		, pe.Existencia
		, pa.AbcDeVentas
		, pa.AbcDeUtilidad
		, pa.AbcDeNegocio
		, pa.AbcDeProveedor
		, pa.AbcDeLinea
		, pmm.Fijo
		, pmm.Minimo
		, pmm.Maximo
		, pmm.FechaCalculo
		, pmm.VentasGlobales
	ORDER BY
		VentasTotal DESC

END
GO

ALTER PROCEDURE [dbo].[pauPartesMaxMinEquivalentes] (
	@Desde DATE
	, @Hasta DATE
	, @SucursalID INT
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
	DECLARE @ParteEstActivo INT = 1

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
		-- , ISNULL(SUM(vd.Cantidad), 0.00) AS Cantidad
		, ISNULL(COUNT(v.VentaID), 0) AS Ventas
		, ISNULL(pp.CostoConDescuento, pp.Costo) AS CostoConDescuento
		, px.Existencia
		, pmm.Maximo
		, pmm.Minimo
		, pmm.Calcular
		, pmm.FechaModificacion
	FROM
		ParteEquivalente pe
		INNER JOIN Parte p ON p.ParteID = pe.ParteID AND p.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = pe.ParteID AND pp.Estatus = 1
		LEFT JOIN ParteExistencia px ON px.ParteID = pe.ParteID AND px.SucursalID = @SucursalID AND px.Estatus = 1
		LEFT JOIN ParteMaxMin pmm ON pmm.ParteID = pe.ParteID AND pmm.SucursalID = @SucursalID
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mr ON mr.MarcaParteID = p.MarcaParteID AND mr.Estatus = 1
		LEFT JOIN VentaDetalle vd ON vd.ParteID = pe.ParteID AND vd.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = vd.VentaID
			AND v.SucursalID = @SucursalID
			AND v.VentaEstatusID = @EstPagadaID
			AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			AND v.Estatus = 1
	WHERE
		p.ParteEstatusID = @ParteEstActivo
		AND (@ProveedorID IS NULL OR p.ProveedorID = @ProveedorID)
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
		, px.Existencia
		, pmm.Maximo
		, pmm.Minimo
		, pmm.Calcular
		, pmm.FechaModificacion
	ORDER BY
		pe.GrupoID
		, p.NumeroParte
END
GO

ALTER PROCEDURE [dbo].[pauPartesMaxMinSinCalcular] (
	@Desde DATE
	, @Hasta DATE
) AS BEGIN
	SET NOCOUNT ON

	/* DECLARE @Desde DATE = '2013-01-01'
	DECLARE @Hasta DATE = '2014-01-31'
	*/

	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3
	DECLARE @ParteEstActivo INT = 1

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)
	
	SELECT
		pmm.ParteID
		, s.NombreSucursal AS Sucursal
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, pv.NombreProveedor AS Proveedor
		, mr.NombreMarcaParte AS Marca
		, l.NombreLinea AS Linea
		, ISNULL(SUM(vd.Cantidad), 0.00) AS Cantidad
	FROM
		ParteMaxMin pmm
		INNER JOIN Parte p ON p.ParteID = pmm.ParteID AND p.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = pmm.SucursalID AND s.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mr ON mr.MarcaParteID = p.MarcaParteID AND mr.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN VentaDetalle vd ON vd.ParteID = pmm.ParteID AND vd.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = vd.VentaID
			AND v.VentaEstatusID = @EstPagadaID
			AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			AND v.Estatus = 1
	WHERE
		p.ParteEstatusID = @ParteEstActivo
		AND pmm.Calcular = 0
	GROUP BY
		pmm.ParteID
		, s.NombreSucursal
		, p.NumeroParte
		, p.NombreParte
		, pv.NombreProveedor
		, mr.NombreMarcaParte
		, l.NombreLinea
	ORDER BY
		pv.NombreProveedor
		, mr.NombreMarcaParte
		, l.NombreLinea
		, p.NombreParte

END
GO