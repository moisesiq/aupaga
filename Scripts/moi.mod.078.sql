/* Script con modificaciones a la base de datos de Theos. Archivo 078
 * Creado: 2016/08/05
 * Subido: 2016/08/30
 */

DECLARE @ScriptID INT = 78
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)
-- SELECT * FROM ScriptSql ORDER BY ScriptID DESC

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

DROP TABLE ParteComision
CREATE TABLE ParteComision (
	ParteComisionID INT NOT NULL IDENTITY (1, 1) PRIMARY KEY
	, ProveedorID INT NOT NULL FOREIGN KEY REFERENCES Proveedor(ProveedorID)
	, MarcaParteID INT NULL FOREIGN KEY REFERENCES MarcaParte(MarcaParteID)
	, LineaID INT NULL FOREIGN KEY REFERENCES Linea(LineaID)
	, ParteID INT NULL FOREIGN KEY REFERENCES Parte(ParteID)
	, ComisionFija DECIMAL(12, 2) NULL
	, PorcentajeNormal DECIMAL(5, 2) NULL
	, PorcentajeUnArticulo DECIMAL(5, 2) NULL
	, ArticulosEspecial INT NULL
	, PorcentajeArticulosEspecial DECIMAL(5, 2) NULL
	, PorcentajeComplementarios DECIMAL(5, 2) NULL
	
	, PorcentajeReduccionPorRepartidor DECIMAL(5, 2) NULL
	, PorcentajeRepartidor DECIMAL(5, 2) NULL
	, ComisionFijaRepartidor DECIMAL(12, 2) NULL
)

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* ****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO

CREATE VIEW [dbo].[PartesComisionesPrevioView] AS
	WITH _Proveedores AS (
		SELECT
			pc.ParteComisionID
			, pv.ProveedorID
			, 0 AS MarcaID
			, 0 AS LineaID
			, 0 AS ParteID
			, pv.NombreProveedor AS Proveedor
			, NULL AS Marca
			, NULL AS Linea
			, NULL AS Parte
			, pc.ComisionFija
			, pc.PorcentajeNormal
			, pc.PorcentajeUnArticulo
			, pc.ArticulosEspecial
			, pc.PorcentajeArticulosEspecial
			, pc.PorcentajeComplementarios
			, pc.PorcentajeReduccionPorRepartidor
			, pc.PorcentajeRepartidor
			, pc.ComisionFijaRepartidor
		FROM
			Proveedor pv
			LEFT JOIN ParteComision pc ON pc.ProveedorID = pv.ProveedorID AND pc.MarcaParteID IS NULL
				AND pc.LineaID IS NULL AND pc.ParteID IS NULL
		WHERE pv.Estatus = 1
	), _Marcas AS (
		SELECT
			pc.ParteComisionID
			, pv.ProveedorID
			, mp.MarcaParteID AS MarcaID
			, 0 AS LineaID
			, 0 AS ParteID
			, pv.Proveedor
			, mp.NombreMarcaParte AS Marca
			, NULL AS Linea
			, NULL AS Parte
			, pc.ComisionFija
			, pc.PorcentajeNormal
			, pc.PorcentajeUnArticulo
			, pc.ArticulosEspecial
			, pc.PorcentajeArticulosEspecial
			, pc.PorcentajeComplementarios
			, pc.PorcentajeReduccionPorRepartidor
			, pc.PorcentajeRepartidor
			, pc.ComisionFijaRepartidor
		FROM
			ProveedorMarcaParte pmp
			INNER JOIN _Proveedores pv ON pv.ProveedorID = pmp.ProveedorID
			LEFT JOIN MarcaParte mp ON mp.MarcaParteID = pmp.MarcaParteID AND mp.Estatus = 1
			LEFT JOIN ParteComision pc ON pc.ProveedorID = pmp.ProveedorID AND pc.MarcaParteID = pmp.MarcaParteID
				AND pc.LineaID IS NULL AND pc.ParteID IS NULL
		WHERE pmp.Estatus = 1
	), _Lineas AS (
		SELECT
			pc.ParteComisionID
			, mp.ProveedorID
			, mp.MarcaID
			, l.LineaID AS LineaID
			, 0 AS ParteID
			, mp.Proveedor
			, mp.Marca
			, l.NombreLinea AS Linea
			, NULL AS Parte
			, pc.ComisionFija
			, pc.PorcentajeNormal
			, pc.PorcentajeUnArticulo
			, pc.ArticulosEspecial
			, pc.PorcentajeArticulosEspecial
			, pc.PorcentajeComplementarios
			, pc.PorcentajeReduccionPorRepartidor
			, pc.PorcentajeRepartidor
			, pc.ComisionFijaRepartidor
		FROM
			LineaMarcaParte lmp
			INNER JOIN _Marcas mp ON mp.MarcaID = lmp.MarcaParteID
			LEFT JOIN Linea l ON l.LineaID = lmp.LineaID AND l.Estatus = 1
			LEFT JOIN ParteComision pc ON pc.ProveedorID = mp.ProveedorID AND pc.MarcaParteID = lmp.MarcaParteID
				AND pc.LineaID = lmp.LineaID AND pc.ParteID IS NULL
		WHERE lmp.Estatus = 1
	)/* , _Partes AS (
		SELECT
			pc.ParteComisionID
			, l.ProveedorID
			, l.MarcaID
			, l.LineaID
			, p.ParteID
			, l.Proveedor
			, l.Marca
			, l.Linea
			, p.NombreParte AS Parte
			, pc.ComisionFija
			, pc.PorcentajeNormal
			, pc.PorcentajeUnArticulo
			, pc.ArticulosEspecial
			, pc.PorcentajeArticulosEspecial
			, pc.PorcentajeComplementarios
			, pc.PorcentajeReduccionPorRepartidor
			, pc.PorcentajeRepartidor
			, pc.ComisionFijaRepartidor
		FROM
			Parte p
			INNER JOIN _Lineas l ON l.LineaID = p.LineaID
			LEFT JOIN ParteComision pc ON pc.ProveedorID = l.ProveedorID AND pc.MarcaParteID = l.MarcaID
				AND pc.LineaID = l.LineaID AND pc.ParteID = p.ParteID
		WHERE p.Estatus = 1
	)
	*/

	SELECT * FROM _Proveedores
	UNION
	SELECT * FROM _Marcas
	UNION
	SELECT * FROM _Lineas
	/*
	UNION
	SELECT * FROM _Partes
	*/

GO

ALTER VIEW PartesComisionesView AS
	SELECT DISTINCT
		pc.ParteComisionID
		, p.LineaID
		, p.ParteID
		, p.NombreParte AS Parte
		, ISNULL(ISNULL(ISNULL(pc.ComisionFija, pcl.ComisionFija), pcm.ComisionFija), pcp.ComisionFija) AS ComisionFija
		, ISNULL(ISNULL(ISNULL(pc.PorcentajeNormal, pcl.PorcentajeNormal), pcm.PorcentajeNormal), pcp.PorcentajeNormal)
			AS PorcentajeNormal
		, ISNULL(ISNULL(ISNULL(pc.PorcentajeUnArticulo, pcl.PorcentajeUnArticulo), pcm.PorcentajeUnArticulo)
			, pcp.PorcentajeUnArticulo) AS PorcentajeUnArticulo
		, ISNULL(ISNULL(ISNULL(pc.ArticulosEspecial, pcl.ArticulosEspecial), pcm.ArticulosEspecial), pcp.ArticulosEspecial)
			AS ArticulosEspecial
		, ISNULL(ISNULL(ISNULL(pc.PorcentajeArticulosEspecial, pcl.PorcentajeArticulosEspecial), pcm.PorcentajeArticulosEspecial)
			, pcp.PorcentajeArticulosEspecial) AS PorcentajeArticulosEspecial
		, ISNULL(ISNULL(ISNULL(pc.PorcentajeComplementarios, pcl.PorcentajeComplementarios), pcm.PorcentajeComplementarios)
			, pcp.PorcentajeComplementarios) AS PorcentajeComplementarios
		, ISNULL(ISNULL(ISNULL(pc.PorcentajeReduccionPorRepartidor, pcl.PorcentajeReduccionPorRepartidor)
			, pcm.PorcentajeReduccionPorRepartidor), pcp.PorcentajeReduccionPorRepartidor) AS PorcentajeReduccionPorRepartidor
		, ISNULL(ISNULL(ISNULL(pc.PorcentajeRepartidor, pcl.PorcentajeRepartidor), pcm.PorcentajeRepartidor)
			, pcp.PorcentajeRepartidor) AS PorcentajeRepartidor
		, ISNULL(ISNULL(ISNULL(pc.ComisionFijaRepartidor, pcl.ComisionFijaRepartidor), pcm.ComisionFijaRepartidor)
			, pcp.ComisionFijaRepartidor) AS ComisionFijaRepartidor
	FROM
		Parte p
		LEFT JOIN ParteComision pc ON pc.ParteID = p.ParteID
		LEFT JOIN ParteComision pcl ON pcl.LineaID = p.LineaID AND pcl.ParteID IS NULL
		LEFT JOIN ParteComision pcm ON pcm.MarcaParteID = p.MarcaParteID AND pcm.LineaID IS NULL AND pcm.ParteID IS NULL
		LEFT JOIN ParteComision pcp ON pcp.ProveedorID = p.ProveedorID AND pcp.MarcaParteID IS NULL
			AND pcp.LineaID IS NULL AND pcp.ParteID IS NULL
	WHERE p.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

-- DROP PROCEDURE pauComisiones2
CREATE PROCEDURE pauComisiones2 (
	@ModoID INT
	, @VendedorID INT
	, @Desde DATE = NULL
	, @Hasta DATE = NULL
) AS BEGIN
	SET NOCOUNT ON

	/*
	DECLARE @ModoID INT = 1
	DECLARE @VendedorID INT = 1
	DECLARE @Desde DATE = '2016-01-02'
	DECLARE @Hasta DATE = '2016-01-08'
	-- EXEC pauComisiones @ModoID, @VendedorID, @Desde, @Hasta
	*/

	-- Definición de variables tipo constante
	DECLARE @ModoNormalID INT = 1
	DECLARE @ModoNoPagadasID INT = 2
	
	DECLARE @EstCobradaID INT = 2
	DECLARE @EstPagadaID INT = 3
	DECLARE @EstCanceladaID INT = 4
	DECLARE @EstGenCompletado INT = 3
	DECLARE @EstGenCanceladoDP INT = 13
	DECLARE @NcOrigenGarantia INT = 5

	DECLARE @UsuarioRepartidorID INT = 3
	
	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)
	-- De momento no se toma en cuenta la sucursal
	DECLARE @PorComision DECIMAL(5, 2) = (SELECT (CONVERT(DECIMAL(5, 2), Valor) / 100) FROM Configuracion WHERE Nombre = 'Comisiones.Vendedor.Porcentaje')
	DECLARE @PorComision9500 DECIMAL(5, 2) = ISNULL((SELECT TOP 1 (Porcentaje9500 / 100) FROM MetaVendedor WHERE VendedorID = @VendedorID), 0.00)
	DECLARE @IvaMul DECIMAL(5, 2) = (SELECT (1 + (CONVERT(DECIMAL(5, 2), Valor) / 100)) FROM Configuracion WHERE Nombre = 'IVA')
	--
	DECLARE @TipoUsuarioID INT = (SELECT TipoUsuarioID FROM Usuario WHERE UsuarioID = @VendedorID)
	DECLARE @EsRepartidor BIT = (CASE WHEN @TipoUsuarioID = @UsuarioRepartidorID THEN 1 ELSE 0 END)

	-- Se sacan las ventas que se van a considerar en el proceso, según los pagos de la semana
	;WITH _Ventas AS (
		SELECT
			vp.VentaID
			, CONVERT(BIT, CASE WHEN c9.VentaID IS NULL THEN 0 ELSE 1 END) AS Es9500
			-- Para sacar la fecha del último pago positivo y el importe de lo pagado
			, CASE WHEN v.ACredito = 1 THEN (
				SELECT MAX(vpi.Fecha)
				FROM
					VentaPago vpi
					INNER JOIN VentaPagoDetalle vpdi ON vpdi.VentaPagoID = vpi.VentaPagoID AND vpdi.Estatus = 1
				WHERE
					vpi.Estatus = 1
					AND vpdi.Importe > 0
			) ELSE v.Fecha END AS FechaPagoVenta
			, MAX(vp.Fecha) AS FechaPago
		FROM
			VentaPago vp
			INNER JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
			-- Para saber si ya se había pagado todo, en caso de ser devoluciones
			/* LEFT JOIN (
				SELECT VentaPagoID, SUM(Importe) AS Pagado
				FROM VentaPagoDetalle
				WHERE Estatus = 1 AND Importe > 0
				GROUP BY VentaPagoID
			) vpd ON vpd.VentaPagoID = vp.VentaPagoID
			*/
			LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID
				AND (c9.EstatusGenericoID = @EstGenCompletado OR c9.EstatusGenericoID = @EstGenCanceladoDP)
				AND c9.Estatus = 1
			
			-- Para sacar la fecha del último abono
			/*
			LEFT JOIN (
				SELECT
					vp.VentaID
					, MAX(vp.Fecha) AS Fecha
				FROM
					VentaPago vp
					INNER JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
				WHERE
					vp.Estatus = 1
					AND vpd.Importe > 0
				GROUP BY vp.VentaID
			) vpf ON vpf.VentaID = vd.VentaID
			*/
			
		WHERE
			vp.Estatus = 1
			AND (vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
			-- Se filtra el usuario, se considera también si es usuario repartidor
			AND (
				(@EsRepartidor = 1 AND v.RepartidorID = @VendedorID)
				OR (@EsRepartidor = 0 AND v.RealizoUsuarioID = @VendedorID)
			)
			AND v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)
			-- Se quitan las ventas de los domingos, menos 9500
			AND (DATEPART(DW, v.Fecha) != 1 OR c9.VentaID IS NOT NULL)
			-- Se quitan las ventas pagadas con vales de garantías
			-- No se está seguro de esta parte *********************************** preguntar **********
			-- AND ncg.NotaDeCreditoID IS NULL
		GROUP BY
			vp.VentaID
			, v.Fecha
			, v.ACredito
			, c9.VentaID
	-- Se saca el detalle de las ventas a considerar, para calcular según el número de artículos
	), _PreDetalle AS (
		SELECT
			vd.VentaID
			, vd.ParteID
			-- , vd.Cantidad
			, ((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) AS Importe
			, ((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) AS Utilidad
			, 'V' AS Caracteristica
		FROM
			VentaDetalle vd
			INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
		WHERE
			vd.Estatus = 1
			AND vd.VentaID IN (SELECT VentaID FROM _Ventas)
		UNION
		SELECT
			vd.VentaID
			, vdd.ParteID
			-- , vdd.Cantidad
			, ((vdd.PrecioUnitario + vdd.Iva) * vdd.Cantidad) AS Importe
			, ((vdd.PrecioUnitario - vdd.Costo) * vdd.Cantidad) AS Utilidad
			, 'D' AS Caracteristica
		FROM
			VentaDevolucionDetalle vdd
			INNER JOIN VentaDevolucion vd ON vd.VentaDevolucionID = vdd.VentaDevolucionID AND vd.Estatus = 1
		WHERE
			vdd.Estatus = 1
			AND vd.VentaID IN (SELECT VentaID FROM _Ventas)
	-- Se saca un conjunto de lineas complementarias, para calcular si aplican las líneas complementarias
	), _LineasComp AS (
		SELECT
			pd.VentaID
			, p.LineaID
			, lc.LineaIDComplementaria
		FROM
			_PreDetalle pd
			INNER JOIN Parte p ON p.ParteID = pd.ParteID AND p.Estatus = 1 AND p.AplicaComision = 1
			LEFT JOIN LineaComplementaria lc ON lc.LineaID = p.LineaID
	-- Se sacan las ventas con datos para aplicar el caso correspondiente
	), _Articulos AS (
		SELECT
			pd.VentaID
			, CASE WHEN EXISTS(
				SELECT LineaID FROM _LineasComp WHERE VentaID = pd.VentaID
				INTERSECT
				SELECT LineaIDComplementaria FROM _LineasComp WHERE VentaID = pd.VentaID
			) THEN 1
			ELSE 0
			END AS Complementarias
			, COUNT(pd.ParteID) AS Articulos
		FROM _PreDetalle pd
		GROUP BY pd.VentaID
	)--, _PreFinal AS (
		SELECT
			ve.VentaID
			-- , CASE WHEN pd.Caracteristica = 'V' THEN ve.FechaPagoVenta ELSE ve.FechaPago END AS Fecha
			, ve.FechaPagoVenta AS Fecha
-- , ve.FechaPagoVenta
			, c.Nombre AS Cliente
			, v.Folio
			, SUM(pd.Importe) AS Importe
			, SUM(CASE WHEN v.ACredito = 1 THEN pd.Importe ELSE 0.0 END) AS Cobranza
			, SUM(pd.Utilidad) AS Utilidad
			, -- Se saca la comisión
				CASE WHEN ve.Es9500 = 1 THEN
					SUM(pd.Utilidad * @PorComision9500)
				ELSE
					CASE WHEN @EsRepartidor = 1 THEN
						SUM(pd.Utilidad * (pcv.PorcentajeRepartidor / 100))
					ELSE
						SUM(pd.Utilidad *
							((CASE
								WHEN a.Complementarias = 1 THEN pcv.PorcentajeComplementarios
								WHEN a.Articulos > pcv.ArticulosEspecial THEN pcv.PorcentajeArticulosEspecial
								WHEN a.Articulos = 1 THEN pcv.PorcentajeUnArticulo
								ELSE pcv.PorcentajeNormal
							END
							- (CASE WHEN @EsRepartidor = 1 THEN pcv.PorcentajeReduccionPorRepartidor ELSE 0.0 END)) / 100)
						)
					END
				END AS Comision
			, CASE WHEN @EsRepartidor = 1 THEN SUM(pcv.ComisionFijaRepartidor)
				ELSE SUM(pcv.PorcentajeReduccionPorRepartidor)END AS ComisionFija
			, v.ACredito
			-- , pd.Caracteristica
			, ve.Es9500
			, 'V' AS Caracteristica
		FROM
			_Ventas ve
			INNER JOIN _PreDetalle pd ON pd.VentaID = ve.VentaID
			INNER JOIN _Articulos a ON a.VentaID = ve.VentaID
			LEFT JOIN PartesComisionesView pcv ON pcv.ParteID = pd.ParteID
			LEFT JOIN Venta v ON v.VentaID = ve.VentaID
			LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		WHERE
			(pd.Caracteristica = 'D' OR (ve.FechaPagoVenta >= @Desde AND ve.FechaPagoVenta < @Hasta))
		GROUP BY
			ve.VentaID
			, ve.FechaPago
			, ve.FechaPagoVenta
			, c.Nombre
			, v.Folio
			, v.ACredito
			--, pd.Caracteristica
			, a.Complementarias
			, a.Articulos
			, ve.Es9500
		
		UNION
		
		SELECT
			ve.VentaID
			-- , CASE WHEN pd.Caracteristica = 'V' THEN ve.FechaPagoVenta ELSE ve.FechaPago END AS Fecha
			, ve.FechaPagoVenta AS Fecha
-- , ve.FechaPagoVenta
			, c.Nombre AS Cliente
			, v.Folio
			, SUM(pd.Importe) AS Importe
			, SUM(CASE WHEN v.ACredito = 1 THEN pd.Importe ELSE 0.0 END) AS Cobranza
			, SUM(pd.Utilidad) AS Utilidad
			, -- Se saca la comisión
				CASE WHEN ve.Es9500 = 1 THEN
					SUM(pd.Utilidad * @PorComision9500)
				ELSE
					CASE WHEN @EsRepartidor = 1 THEN
						SUM(pd.Utilidad * (pcv.PorcentajeRepartidor / 100))
					ELSE
						SUM(pd.Utilidad *
							((CASE
								WHEN a.Complementarias = 1 THEN pcv.PorcentajeComplementarios
								WHEN a.Articulos > pcv.ArticulosEspecial THEN pcv.PorcentajeArticulosEspecial
								WHEN a.Articulos = 1 THEN pcv.PorcentajeUnArticulo
								ELSE pcv.PorcentajeNormal
							END
							- (CASE WHEN @EsRepartidor = 1 THEN pcv.PorcentajeReduccionPorRepartidor ELSE 0.0 END)) / 100)
						)
					END
				END AS Comision
			, CASE WHEN @EsRepartidor = 1 THEN SUM(pcv.ComisionFijaRepartidor)
				ELSE SUM(pcv.PorcentajeReduccionPorRepartidor)END AS ComisionFija
			, v.ACredito
			-- , pd.Caracteristica
			, ve.Es9500
			, 'D' AS Caracteristica
		FROM
			_Ventas ve
			INNER JOIN _PreDetalle pd ON pd.VentaID = ve.VentaID
			INNER JOIN _Articulos a ON a.VentaID = ve.VentaID
			LEFT JOIN PartesComisionesView pcv ON pcv.ParteID = pd.ParteID
			LEFT JOIN Venta v ON v.VentaID = ve.VentaID
			LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		WHERE pd.Caracteristica = 'D'
		GROUP BY
			ve.VentaID
			, ve.FechaPago
			, ve.FechaPagoVenta
			, c.Nombre
			, v.Folio
			, v.ACredito
			--, pd.Caracteristica
			, a.Complementarias
			, a.Articulos
			, ve.Es9500
	--)
	
END
GO