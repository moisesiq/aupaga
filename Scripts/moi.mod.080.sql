/* Script modificaciones a la basde de datos de Theos. Archivo 080.
 *Subido: 2017/03/20
 
 *CAMBIOS*
 *Procedimiento almacenado: pauComisiones
 *Procedimeinto almacenado: pauComisionesAgrupado
 
*/

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */


/****** Object:  Table [dbo].[ParteComisionHistorico]    Script Date: 03/20/2017 13:24:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ParteComisionHistorico](
	[ParteComisionHistoricoID] [int] IDENTITY(1,1) NOT NULL,
	[FechaRegistro] [datetime] NOT NULL,
	[VentaID] [int] NULL,
	[ParteID] [int] NULL,
	[ComisionFija] [decimal](12, 2) NULL,
	[PorcentajeNormal] [decimal](5, 2) NULL,
	[PorcentajeUnArticulo] [decimal](5, 2) NULL,
	[ArticulosEspecial] [int] NULL,
	[PorcentajeArticuloEspecial] [decimal](5, 2) NULL,
	[PorcentajeComplementarios] [decimal](5, 2) NULL,
	[PorcentajeReduccionPorRepartidor] [decimal](5, 2) NULL,
	[PorcentajeRepartidor] [decimal](5, 2) NULL,
	[ComisionFijaRepartidor] [decimal](12, 2) NULL,
 CONSTRAINT [PK_ParteComisionHistorico] PRIMARY KEY CLUSTERED 
(
	[ParteComisionHistoricoID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ParteComisionHistorico]  WITH CHECK ADD FOREIGN KEY([ParteID])
REFERENCES [dbo].[Parte] ([ParteID])
GO

ALTER TABLE [dbo].[ParteComisionHistorico]  WITH CHECK ADD FOREIGN KEY([VentaID])
REFERENCES [dbo].[Venta] ([VentaID])
GO





/* ****************************************************************************
** Crear Funciones
***************************************************************************** */


/* ****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */


/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

/******** pauComisiones *********/



ALTER PROCEDURE [dbo].[pauComisiones2test] (
	@ModoID INT
	, @VendedorID INT
	, @SucursalID INT
	, @Desde DATE = NULL
	, @Hasta DATE = NULL
) AS BEGIN
SET NOCOUNT ON

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
--DECLARE @TipoUsuarioID INT = (SELECT TipoUsuarioID FROM Usuario WHERE UsuarioID = @VendedorID)
DECLARE @TipoUsuarioID INT = (SELECT CASE WHEN TipoUsuarioID IS NULL THEN 1 ELSE TipoUsuarioID END AS TipoUsuarioID FROM Usuario WHERE UsuarioID = @VendedorID)
DECLARE @EsRepartidor BIT = (CASE WHEN @TipoUsuarioID = @UsuarioRepartidorID THEN 1 ELSE 0 END)

-- Se sacan las ventas que se van a considerar en el proceso, según los pagos de la semana
IF @ModoID = @ModoNormalID BEGIN
	IF @TipoUsuarioID = 1 OR @TipoUsuarioID = 2 BEGIN
		;WITH _Ventas AS (
			SELECT	
				vp.VentaID,
				v.Folio,
				MAX(vp.Fecha) AS Fecha,
				SUM(vpd.Importe) AS Importe,
				(
					SELECT 
						CASE 
							WHEN SUM((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) IS NULL THEN	
								0.0
							ELSE
								SUM((vd.PrecioUnitario - vd.Costo) * vd.Cantidad)
						END
					FROM VentaDetalle vd
						INNER JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
					WHERE 
					vd.Estatus = 1 AND 
					vd.VentaID = vp.VentaID AND p.AplicaComision = 1
				)
				AS Utilidad,
				(
					SELECT 
						CASE 
							WHEN SUM((vdd.PrecioUnitario - vdd.Costo) * vdd.Cantidad) IS NULL THEN
								0.0
							ELSE 
								SUM((vdd.PrecioUnitario - vdd.Costo) * vdd.Cantidad) 
						END
					FROM
						VentaDevolucion vd
						INNER JOIN VentaDevolucionDetalle vdd ON vdd.VentaDevolucionID = vd.VentaDevolucionID AND vdd.Estatus = 1
						INNER JOIN Parte p ON p.ParteID = vdd.ParteID AND p.Estatus = 1
					WHERE
						vdd.Estatus = 1 AND 
						vd.VentaID = vp.VentaID AND p.AplicaComision = 1
				)
				AS UtilidadDev,
				v.ACredito,
				'V' AS Caracteristica,
				CONVERT(BIT, CASE WHEN c9.VentaID IS NULL THEN 0 ELSE 1 END) AS Es9500		
			FROM
				VentaPago vp
				INNER JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
				INNER JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
				LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND (c9.EstatusGenericoID = @EstGenCompletado OR c9.EstatusGenericoID = @EstGenCanceladoDP) AND c9.Estatus = 1
				LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
				LEFT JOIN NotaDeCredito nc ON nc.NotaDeCreditoID = vpd.NotaDeCreditoID AND nc.OrigenID = @NcOrigenGarantia
			WHERE 
				vp.Estatus = 1
				AND (vp.Fecha >= @Desde AND vp.Fecha <= @Hasta)
				AND vpd.Importe > 0
				AND v.RealizoUsuarioID = @VendedorID
				AND v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)
				AND v.SucursalID = @SucursalID
				AND (DATEPART(DW, v.Fecha) != 1 OR c9.VentaID IS NOT NULL)
				AND nc.NotaDeCreditoID IS NULL
			GROUP BY
				vp.VentaID,
				c9.VentaID,
				v.Folio,
				v.ACredito		
		)
		,
		_DetalleVenta AS (
			SELECT 
				vd.VentaID,
				CASE WHEN vdd.VentaID = v.VentaID THEN 'VD' ELSE 'V' END AS Caracteristica,
				vd.parteId
			FROM
				VentaDetalle vd
				INNER JOIN _Ventas v ON v.VentaID = vd.VentaID
				LEFT JOIN Venta v1 ON v1.VentaID = v.VentaID
				LEFT JOIN VentaDevolucion vdd ON vdd.VentaID = vd.VentaID
			WHERE
				vd.VentaID = v.VentaID
			GROUP BY
				vd.VentaID--,
				,v.VentaID
				,vdd.VentaID
				,vd.ParteID
		)

		,
		_VentasDev AS (
			SELECT 
				vd.VentaID,
				vd.VentaDevolucionID,
				CONVERT(BIT, CASE WHEN c9.VentaID IS NULL THEN 0 ELSE 1 END) AS Es9500,
				vd.Fecha,
				v.Folio,
				SUM((vdd.PrecioUnitario + vdd.Iva) * vdd.Cantidad) AS Importe,
				(SELECT SUM((vddi.PrecioUnitario - vddi.Costo) * vddi.Cantidad)
						FROM VentaDevolucion vdi
						INNER JOIN VentaDevolucionDetalle vddi ON vddi.VentaDevolucionID = vdi.VentaDevolucionID AND vddi.Estatus = 1
						INNER JOIN Parte p ON p.ParteID = vddi.ParteID AND p.Estatus = 1
						WHERE vdi.Estatus = 1 AND vdi.VentaID = vd.VentaID AND vdi.Fecha = vd.Fecha  AND p.AplicaComision = 1)
				AS Utilidad,
				c.Nombre AS Cliente
				,0.0 AS Comision,
				0.0 As ComisionFija,
				0.0 as Cobranza	
				,'D' AS Caracteristica
			FROM
				VentaDevolucion vd
				LEFT JOIN VentaDevolucionDetalle vdd ON vdd.VentaDevolucionID = vd.VentaDevolucionID AND vdd.Estatus = 1
				INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
				inner join Parte p on p.ParteID = vdd.ParteID and p.Estatus = 1
				LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID
				AND (c9.EstatusGenericoID = @EstGenCompletado OR c9.EstatusGenericoID = @EstGenCanceladoDP)
				AND c9.Estatus = 1
				LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
			WHERE 
				vd.Estatus = 1
				AND (
					(vd.Fecha >= @Desde AND vd.Fecha < @Hasta)
					--OR vd.VentaID IN (SELECT DISTINCT VentaID FROM _Ventas WHERE Fecha < @Desde)
				)
				--AND (vd.Fecha >= @Desde AND vd.Fecha < @Hasta)
				--AND v.Fecha < @Desde
				AND v.RealizoUsuarioID = @VendedorID
				AND v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)
				--AND p.AplicaComision = 1
				--AND p.AplicaComision = 1
				-- Se quitan las ventas de los domingos
				--AND (DATEPART(DW, v.Fecha) != 1 OR c9.VentaID IS NOT NULL)
			GROUP BY
				vd.VentaDevolucionID
				, vd.VentaID
				, vd.Fecha
				, c.Nombre
				, v.Folio
				, v.ACredito
				, v.ComisionistaClienteID
				, c9.VentaID
		)
		,
		_DetalleDev AS(
		SELECT
			vd.VentaID,
			'D' AS Caracteristica,
			vdd.ParteID AS ParteID
		FROM
			VentaDevolucionDetalle vdd
			INNER JOIN _VentasDev vd ON vd.VentaDevolucionID = vdd.VentaDevolucionID
			--LEFT JOIN Venta v ON v.VentaID = vd.VentaID
		--WHERE
		GROUP BY
			vd.VentaID,
			vdd.ParteID

		)
		,_LineasComp AS (
			SELECT
				d.VentaID,
				p.ParteID,
				lc.ParteIDComplementaria
			FROM
				_DetalleVenta d
				INNER JOIN Parte p ON p.ParteID = d.ParteID AND p.Estatus = 1 AND p.AplicaComision = 1
				LEFT JOIN ParteComplementaria lc ON lc.ParteID = p.ParteID
		)
		,
		_Articulos AS (
			SELECT
				d.VentaID,
				CASE WHEN EXISTS(
					SELECT ParteID FROM _LineasComp WHERE VentaID = d.VentaID
						INTERSECT
					SELECT ParteIDComplementaria FROM _LineasComp WHERE VentaID = d.VentaID
					) 
				THEN 
					1
				ELSE 
					0
				END AS Complementarias,
				COUNT(d.ParteID) AS Articulos
			FROM 
				_DetalleVenta d
			GROUP BY 
				d.VentaID
		)
		,

		_PreComision AS(
			SELECT 
				ve.VentaID,
				--d.ParteID,
				(
				CASE 
				WHEN ve.Es9500 = 1 THEN
					((vd.PrecioUnitario - vd.Costo)* vd.Cantidad) * @PorComision9500
				--WHEN a.Articulos > 1 THEN
				ELSE
					CASE
						WHEN 
							@EsRepartidor = 1 
						THEN
							(
								((vd.PrecioUnitario - vd.Costo)* vd.Cantidad) * (pcv.PorcentajeRepartidor / 100)
							)
						ELSE
							(
								((
								CASE WHEN v.ComisionistaClienteID > 0 THEN 
									CASE c.ListaDePrecios
										WHEN 1 THEN (pp.PrecioUno / @IvaMul)
										WHEN 2 THEN (pp.PrecioDos / @IvaMul)
										WHEN 3 THEN (pp.PrecioTres / @IvaMul)
										WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
										WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
									END
								ELSE 
									CASE 
									WHEN vd.PrecioUnitario > (pp.PrecioUno / @IvaMul) THEN 
										(pp.PrecioUno /@IvaMul)
									ELSE	
										vd.PrecioUnitario
									END
								END
								--((vd.PrecioUnitario - vd.Costo)* vd.Cantidad) *
									- vd.Costo)* vd.Cantidad) *
									(
									CASE WHEN ve.VentaID = pch.VentaID THEN
										(
											CASE
												WHEN a.Complementarias = 1  THEN 
													pch.PorcentajeComplementarios 
												WHEN a.Articulos > pcv.ArticulosEspecial THEN 
													pch.PorcentajeArticuloEspecial
												WHEN a.Articulos = 1 THEN 
													pch.PorcentajeUnArticulo 
												ELSE 
													pch.PorcentajeNormal 
											END
												- (
											CASE WHEN v.RepartidorID > 0 THEN 
												pch.PorcentajeReduccionPorRepartidor 
											ELSE 
												0.0 
											END
													)
												) / 100
										--)
									ELSE
										(
											CASE
												WHEN a.Complementarias = 1  THEN 
													pcv.PorcentajeComplementarios 
												WHEN a.Articulos > pcv.ArticulosEspecial THEN 
													pcv.PorcentajeArticulosEspecial
												WHEN a.Articulos = 1 THEN 
													pcv.PorcentajeUnArticulo 
												ELSE 
													pcv.PorcentajeNormal 
											END
												- (
											CASE WHEN v.RepartidorID > 0 THEN 
												pcv.PorcentajeReduccionPorRepartidor 
											ELSE 
												0.0 
											END
													)
												) / 100
										--)
									END
									)
							)
					END 
				END 
				)
				AS ComisionIndividual,
				CASE 
					WHEN @EsRepartidor = 1 THEN 
						--SUM
						(pcv.ComisionFijaRepartidor)
					--WHEN ve.Es9500 = 1 THEN
					--	 0.0
					ELSE 
						--SUM(pcv.PorcentajeReduccionPorRepartidor)
						--SUM
						(pcv.ComisionFija)
				END AS ComisionFija,
				d.ParteID
			FROM
				_Ventas ve
				LEFT JOIN _DetalleVenta d ON d.VentaID = ve.VentaID
				LEFT JOIN _Articulos a ON a.VentaID = d.VentaID
				LEFT JOIN PartesComisionesView pcv ON pcv.ParteID = d.ParteID
				LEFT JOIN ParteComisionHistorico pch ON pch.ParteID = d.ParteID and pch.VentaID = ve.VentaID
				LEFT JOIN VentaDetalle vd ON vd.VentaID = d.VentaID
				LEFT JOIN VentaDevolucion vdi ON vdi.VentaID = ve.VentaID
				LEFT JOIN VentaDevolucionDetalle vddi ON vddi.VentaDevolucionID = vdi.VentaDevolucionID and vddi.Estatus = 1
				LEFT JOIN Venta v ON v.VentaID = d.VentaID
				LEFT JOIN Cliente c ON c.ClienteID = v.ComisionistaClienteID AND c.Estatus = 1
				LEFT JOIN PartePrecio pp ON pp.ParteID = d.ParteID
			WHERE
				d.VentaID = ve.VentaID
				and vd.ParteID = d.ParteID
				--and vd.Estatus = 1 and a.Articulos > 1
			GROUP BY
				--ve.VentaID, v.ComisionistaClienteID, vd.VentaID
				d.ParteID,
				ve.VentaID,
				ve.Es9500,
				pcv.ComisionFija,
				pcv.ComisionFijaRepartidor,
				pcv.PorcentajeNormal,
				pcv.PorcentajeRepartidor,
				pcv.PorcentajeComplementarios,
				pcv.PorcentajeArticulosEspecial,
				pcv.PorcentajeUnArticulo,
				pcv.PorcentajeReduccionPorRepartidor,
				pcv.ArticulosEspecial,
				pch.ComisionFija,
				pch.PorcentajeNormal,
				pch.PorcentajeRepartidor,
				pch.PorcentajeComplementarios,
				pch.PorcentajeArticuloEspecial,
				pch.PorcentajeUnArticulo,
				pch.PorcentajeReduccionPorRepartidor,
				pch.ArticulosEspecial,
				a.Articulos,
				a.Complementarias,
				vd.PrecioUnitario,
				vd.Costo,
				vd.Cantidad,
				pp.PrecioUno,
				pp.PrecioCinco,
				pp.PrecioCuatro,
				pp.PrecioDos,
				pp.PrecioTres,
				v.ComisionistaClienteID,
				c.ListaDePrecios,
				pch.VentaID,
				v.RepartidorID
			----	ve.Es9500
		)

		--select * from _PreComision
		,

		_PreComisionDev AS(
			SELECT 
				ve.VentaID,
				SUM
				(
				CASE 
				WHEN ve.Es9500 = 1 THEN
					((vd.PrecioUnitario - vd.Costo)* vd.Cantidad) * @PorComision9500
				ELSE
					CASE
						WHEN 
							@EsRepartidor = 1 
						THEN
							(
								((vd.PrecioUnitario - vd.Costo)* vd.Cantidad) * (pcv.PorcentajeRepartidor / 100)
							)
						ELSE
							(
								((
									CASE WHEN v.ComisionistaClienteID > 0 THEN 
										CASE c.ListaDePrecios
											WHEN 1 THEN (pp.PrecioUno / @IvaMul)
											WHEN 2 THEN (pp.PrecioDos / @IvaMul)
											WHEN 3 THEN (pp.PrecioTres / @IvaMul)
											WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
											WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
										END
									ELSE 
										CASE 
										WHEN vd.PrecioUnitario > (pp.PrecioUno / @IvaMul) THEN 
											(pp.PrecioUno /@IvaMul)
										ELSE	
											vd.PrecioUnitario
										END
									END
										- vd.Costo)* vd.Cantidad) *
									(
										
											CASE WHEN ve.VentaID = pch.VentaID THEN
										(
											CASE
												WHEN a.Complementarias = 1  THEN 
													pch.PorcentajeComplementarios 
												WHEN a.Articulos > pcv.ArticulosEspecial THEN 
													pch.PorcentajeArticuloEspecial
												WHEN a.Articulos = 1 THEN 
													pch.PorcentajeUnArticulo 
												ELSE 
													pch.PorcentajeNormal 
											END
												- (
											CASE WHEN v.RepartidorID > 0 THEN 
												pch.PorcentajeReduccionPorRepartidor 
											ELSE 
												0.0 
											END
													)
												) / 100
										--)
									ELSE
										(
											CASE
												WHEN a.Complementarias = 1  THEN 
													pcv.PorcentajeComplementarios 
												WHEN a.Articulos > pcv.ArticulosEspecial THEN 
													pcv.PorcentajeArticulosEspecial
												WHEN a.Articulos = 1 THEN 
													pcv.PorcentajeUnArticulo 
												ELSE 
													pcv.PorcentajeNormal 
											END
												- (
											CASE WHEN v.RepartidorID > 0 THEN 
												pcv.PorcentajeReduccionPorRepartidor 
											ELSE 
												0.0 
											END
													)
												) / 100
										--)
									END
										)
							)
					END 
				END 
				)
				AS ComisionIndividual,
				CASE 
					WHEN @EsRepartidor = 1 THEN 
						SUM(pcv.ComisionFijaRepartidor)
					ELSE 
						SUM(pcv.ComisionFija)
				END AS ComisionFija,
				ve.Fecha AS Fecha
			FROM
				_VentasDev ve
				LEFT JOIN _DetalleDev d ON d.VentaID = ve.VentaID
				LEFT JOIN _Articulos a ON a.VentaID = ve.VentaID
				LEFT JOIN PartesComisionesView pcv ON pcv.ParteID = d.ParteID
				LEFT JOIN ParteComisionHistorico pch ON pch.ParteID = d.ParteID and pch.VentaID = ve.VentaID
				LEFT JOIN VentaDevolucionDetalle vd ON vd.VentaDevolucionID = ve.VentaDevolucionID
				LEFT JOIN Venta v ON v.VentaID = ve.VentaID
				LEFT JOIN Cliente c ON c.ClienteID = v.ComisionistaClienteID AND c.Estatus = 1
				LEFT JOIN PartePrecio pp ON pp.ParteID = d.ParteID
			WHERE
				d.VentaID = ve.VentaID
				and vd.ParteID = d.ParteID
			GROUP BY
				ve.VentaID,
				vd.VentaDevolucionID,
				ve.Fecha,
				ve.Cliente,
				ve.Folio,
				ve.Importe,
				ve.Cobranza,
				ve.Utilidad,
				ve.Es9500
		)
		SELECT
			pc.VentaID,
			ve.Fecha AS Fecha,
			c.Nombre AS Cliente,
			ve.Folio,
			CASE WHEN ve.ACredito = 1 THEN 0.00 ELSE Importe END AS Importe,
			CASE WHEN ve.ACredito = 1 THEN Importe ELSE 0.00 END AS Cobranza,
			CASE WHEN ve.Utilidad = 0.0 THEN ve.UtilidadDev ELSE ve.Utilidad END AS Utilidad,
			SUM(pc.ComisionIndividual) AS Comision,
			--SUM(pc.ComisionFija) AS ComisionFija,
			CASE WHEN SUM(pc.ComisionFija) IS NULL THEN 0.0 ELSE SUM(pc.ComisionFija) END AS ComisionFija,
			ve.ACredito,
			ve.Es9500,
			(SELECT DISTINCT
				CASE WHEN vdd.VentaID = v.VentaID THEN 'VD' ELSE 'V' END
			FROM
				VentaDetalle vd
				INNER JOIN _Ventas v ON v.VentaID = pc.VentaID
				LEFT JOIN Venta v1 ON v1.VentaID = v.VentaID
				LEFT JOIN VentaDevolucion vdd ON vdd.VentaID = pc.VentaID
			WHERE
				vd.VentaID = v.VentaID
			GROUP BY
				vd.VentaID--,
				,v.VentaID
				,vdd.VentaID
				,vd.ParteID) AS Caracteristica
		FROM
			_Ventas ve
			LEFT JOIN 
			_PreComision pc ON pc.VentaID = ve.VentaID
			LEFT JOIN Venta v ON v.VentaID = ve.VentaID
			LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		WHERE
			pc.VentaID = ve.VentaID
		GROUP BY
			pc.VentaID,
			ve.Fecha,
			c.Nombre,
			ve.Folio,
			ve.Importe,
			ve.Es9500,
			ve.ACredito,
			ve.Utilidad,
			ve.UtilidadDev

		UNION 
		SELECT 
			vd.VentaID,
			vd.Fecha,
			vd.Cliente,
			vd.Folio,
			vd.Importe * -1 AS Importe,
			vd.Cobranza * -1 AS Cobranza,
			(CASE WHEN vd.Utilidad IS NULL THEN 0.0 ELSE vd.Utilidad * -1 END) AS Utilidad,
			(pcd.ComisionIndividual) * -1 AS Comision,
			CASE WHEN
			(
				SELECT 
					SUM(ComisionFija)
				FROM
				_PreComisionDev pcd
				WHERE
					pcd.VentaID = vd.VentaID and pcd.Fecha = vd.Fecha
			) * -1 IS NULL 
			THEN 0.0 
			ELSE
			(
				SELECT 
					SUM(ComisionFija)
				FROM
				_PreComisionDev pcd
				WHERE
					pcd.VentaID = vd.VentaID and pcd.Fecha = vd.Fecha
			) * -1
			END
			 AS ComisionFija,
			
			CONVERT(BIT,0) as ACredito,
			CONVERT(BIT,vd.Es9500) AS Es9500
			,'D' AS Caracteristica
		FROM 
			_VentasDev vd
			LEFT JOIN _DetalleDev d ON d.VentaID = vd.VentaID
			LEFT JOIN _PreComisionDev pcd ON pcd.VentaID = vd.VentaID AND pcd.Fecha = vd.Fecha
		where
		vd.Fecha = vd.Fecha
		group by
		vd.VentaID,
		vd.VentaDevolucionID,
		vd.Fecha,
		vd.Cliente,
		vd.Folio,
		vd.Importe,
		vd.Cobranza,
		vd.Utilidad,
		vd.Es9500
		,pcd.ComisionIndividual
		order by
			Fecha
			,Caracteristica 
		END







		------------------------------------------------------------------------------------------------------------------------------------------------









	IF @TipoUsuarioID = 3 BEGIN
		;WITH _Ventas AS (
			SELECT	
				vp.VentaID,
				v.Folio,
				MAX(vp.Fecha) AS Fecha,
				SUM(vpd.Importe) AS Importe,
				(
					SELECT 
						CASE 
							WHEN SUM((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) IS NULL THEN	
								0.0
							ELSE
								SUM((vd.PrecioUnitario - vd.Costo) * vd.Cantidad)
						END
					FROM VentaDetalle vd
						INNER JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
					WHERE 
					--vd.Estatus = 1 AND 
					vd.VentaID = vp.VentaID AND p.AplicaComision = 1
				)
				AS Utilidad,
				(
					SELECT 
						CASE 
							WHEN SUM((vdd.PrecioUnitario - vdd.Costo) * vdd.Cantidad) IS NULL THEN
								0.0
							ELSE 
								SUM((vdd.PrecioUnitario - vdd.Costo) * vdd.Cantidad) 
						END
					FROM
						VentaDevolucion vd
						INNER JOIN VentaDevolucionDetalle vdd ON vdd.VentaDevolucionID = vd.VentaDevolucionID AND vdd.Estatus = 1
						INNER JOIN Parte p ON p.ParteID = vdd.ParteID AND p.Estatus = 1
					WHERE
						vdd.Estatus = 1 AND 
						vd.VentaID = vp.VentaID AND p.AplicaComision = 1
				)
				AS UtilidadDev,
				v.ACredito,
				'V' AS Caracteristica,
				CONVERT(BIT, CASE WHEN c9.VentaID IS NULL THEN 0 ELSE 1 END) AS Es9500		
			FROM
				VentaPago vp
				INNER JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
				INNER JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
				LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND (c9.EstatusGenericoID = @EstGenCompletado OR c9.EstatusGenericoID = @EstGenCanceladoDP) AND c9.Estatus = 1
				LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
				LEFT JOIN NotaDeCredito nc ON nc.NotaDeCreditoID = vpd.NotaDeCreditoID AND nc.OrigenID = @NcOrigenGarantia
			WHERE 
				vp.Estatus = 1
				AND (vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
				AND v.RepartidorID = @VendedorID
				AND v.SucursalID = @SucursalID
				AND vpd.Importe > 0
				AND v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)
				AND (DATEPART(DW, v.Fecha) != 1 OR c9.VentaID IS NOT NULL)
			GROUP BY
				vp.VentaID,
				c9.VentaID,
				v.Folio,
				v.ACredito		
		)
		,
		_DetalleVenta AS (
			SELECT 
				vd.VentaID,
				CASE WHEN vdd.VentaID = v.VentaID THEN 'VD' ELSE 'V' END AS Caracteristica,
				vd.parteId
			FROM
				VentaDetalle vd
				INNER JOIN _Ventas v ON v.VentaID = vd.VentaID
				LEFT JOIN Venta v1 ON v1.VentaID = v.VentaID
				LEFT JOIN VentaDevolucion vdd ON vdd.VentaID = vd.VentaID
			WHERE
				vd.VentaID = v.VentaID
			GROUP BY
				vd.VentaID--,
				,v.VentaID
				,vdd.VentaID
				,vd.ParteID
		)

		,
		_VentasDev AS (
			SELECT 
				vd.VentaID,
				vd.VentaDevolucionID,
				CONVERT(BIT, CASE WHEN c9.VentaID IS NULL THEN 0 ELSE 1 END) AS Es9500,
				vd.Fecha,
				v.Folio,
				SUM((vdd.PrecioUnitario + vdd.Iva) * vdd.Cantidad) AS Importe,
				(SELECT SUM((vddi.PrecioUnitario - vddi.Costo) * vddi.Cantidad)
						FROM VentaDevolucion vdi
						INNER JOIN VentaDevolucionDetalle vddi ON vddi.VentaDevolucionID = vdi.VentaDevolucionID AND vddi.Estatus = 1
						INNER JOIN Parte p ON p.ParteID = vddi.ParteID AND p.Estatus = 1
						WHERE vdi.Estatus = 1 AND vdi.VentaID = vd.VentaID AND vdi.Fecha = vd.Fecha  AND p.AplicaComision = 1)
				AS Utilidad,
				c.Nombre AS Cliente
				,0.0 AS Comision,
				0.0 As ComisionFija,
				0.0 as Cobranza	
				,'D' AS Caracteristica
			FROM
				VentaDevolucion vd
				LEFT JOIN VentaDevolucionDetalle vdd ON vdd.VentaDevolucionID = vd.VentaDevolucionID AND vdd.Estatus = 1
				INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
				inner join Parte p on p.ParteID = vdd.ParteID and p.Estatus = 1
				LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID
				AND (c9.EstatusGenericoID = @EstGenCompletado OR c9.EstatusGenericoID = @EstGenCanceladoDP)
				AND c9.Estatus = 1
				LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
			WHERE 
				vd.Estatus = 1
				AND (
					(vd.Fecha >= @Desde AND vd.Fecha < @Hasta)
				)
				AND v.RepartidorID = @VendedorID
				AND v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)
				--AND p.AplicaComision = 1
			GROUP BY
				vd.VentaDevolucionID
				, vd.VentaID
				, vd.Fecha
				, c.Nombre
				, v.Folio
				, v.ACredito
				, v.ComisionistaClienteID
				, c9.VentaID
		),
		_DetalleDev AS(
		SELECT
			vd.VentaID,
			'D' AS Caracteristica,
			vdd.ParteID AS ParteID
		FROM
			VentaDevolucionDetalle vdd
			INNER JOIN _VentasDev vd ON vd.VentaDevolucionID = vdd.VentaDevolucionID
		GROUP BY
			vd.VentaID,
			vdd.ParteID

		),
		_LineasComp AS (
			SELECT
				d.VentaID,
				p.ParteID,
				lc.ParteIDComplementaria
			FROM
				_DetalleVenta d
				INNER JOIN Parte p ON p.ParteID = d.ParteID AND p.Estatus = 1 AND p.AplicaComision = 1
				LEFT JOIN ParteComplementaria lc ON lc.ParteID = p.ParteID
		),
		_Articulos AS (
			SELECT
				d.VentaID,
				CASE WHEN EXISTS(
					SELECT ParteID FROM _LineasComp WHERE VentaID = d.VentaID
						INTERSECT
					SELECT ParteIDComplementaria FROM _LineasComp WHERE VentaID = d.VentaID
					) 
				THEN 
					1
				ELSE 
					0
				END AS Complementarias,
				COUNT(d.ParteID) AS Articulos
			FROM 
				_DetalleVenta d
			GROUP BY 
				d.VentaID
		)
		--select * from _Articulos
		,

		_PreComision AS(
			SELECT 
				ve.VentaID,
				(
				CASE 
				WHEN ve.Es9500 = 1 THEN
					((vd.PrecioUnitario - vd.Costo)* vd.Cantidad) * @PorComision9500
				ELSE
					CASE
						WHEN 
							@EsRepartidor = 1 
						THEN
							(
								((vd.PrecioUnitario - vd.Costo)* vd.Cantidad) * (pcv.PorcentajeRepartidor / 100)
							)
						ELSE
							(
								((
								CASE WHEN v.ComisionistaClienteID > 0 THEN 
									CASE c.ListaDePrecios
										WHEN 1 THEN (pp.PrecioUno / @IvaMul)
										WHEN 2 THEN (pp.PrecioDos / @IvaMul)
										WHEN 3 THEN (pp.PrecioTres / @IvaMul)
										WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
										WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
									END
								ELSE 
									CASE 
									WHEN vd.PrecioUnitario > (pp.PrecioUno / @IvaMul) THEN 
										(pp.PrecioUno /@IvaMul)
									ELSE	
										vd.PrecioUnitario
									END
								END
									- vd.Costo)* vd.Cantidad) *
									(
									CASE WHEN ve.VentaID = pch.VentaID THEN
										(
											CASE
												WHEN a.Complementarias = 1  THEN 
													pch.PorcentajeComplementarios 
												WHEN a.Articulos > pcv.ArticulosEspecial THEN 
													pch.PorcentajeArticuloEspecial
												WHEN a.Articulos = 1 THEN 
													pch.PorcentajeUnArticulo 
												ELSE 
													pch.PorcentajeNormal 
											END
												- (
											CASE WHEN v.RepartidorID > 0 THEN 
												pch.PorcentajeReduccionPorRepartidor 
											ELSE 
												0.0 
											END
													)
												) / 100
										--)
									ELSE
										(
											CASE
												WHEN a.Complementarias = 1  THEN 
													pcv.PorcentajeComplementarios 
												WHEN a.Articulos > pcv.ArticulosEspecial THEN 
													pcv.PorcentajeArticulosEspecial
												WHEN a.Articulos = 1 THEN 
													pcv.PorcentajeUnArticulo 
												ELSE 
													pcv.PorcentajeNormal 
											END
												- (
											CASE WHEN v.RepartidorID > 0 THEN 
												pcv.PorcentajeReduccionPorRepartidor 
											ELSE 
												0.0 
											END
													)
												) / 100
										--)
									END
									)
							)
					END 
				END 
				)
				AS ComisionIndividual,
				CASE 
					WHEN @EsRepartidor = 1 THEN 
						(pcv.ComisionFijaRepartidor)
					ELSE 
						(pcv.ComisionFija)
				END AS ComisionFija,
				d.ParteID
			FROM
				_Ventas ve
				LEFT JOIN _DetalleVenta d ON d.VentaID = ve.VentaID
				LEFT JOIN _Articulos a ON a.VentaID = d.VentaID
				LEFT JOIN PartesComisionesView pcv ON pcv.ParteID = d.ParteID
				LEFT JOIN ParteComisionHistorico pch ON pch.ParteID = d.ParteID and pch.VentaID = ve.VentaID
				LEFT JOIN VentaDetalle vd ON vd.VentaID = d.VentaID
				LEFT JOIN VentaDevolucion vdi ON vdi.VentaID = ve.VentaID
				LEFT JOIN VentaDevolucionDetalle vddi ON vddi.VentaDevolucionID = vdi.VentaDevolucionID and vddi.Estatus = 1
				LEFT JOIN Venta v ON v.VentaID = d.VentaID
				LEFT JOIN Cliente c ON c.ClienteID = v.ComisionistaClienteID AND c.Estatus = 1
				LEFT JOIN PartePrecio pp ON pp.ParteID = d.ParteID
			WHERE
				d.VentaID = ve.VentaID
				and vd.ParteID = d.ParteID
			GROUP BY
				d.ParteID,
				ve.VentaID,
				ve.Es9500,
				pcv.ComisionFija,
				pcv.ComisionFijaRepartidor,
				pcv.PorcentajeNormal,
				pcv.PorcentajeRepartidor,
				pcv.PorcentajeComplementarios,
				pcv.PorcentajeArticulosEspecial,
				pcv.PorcentajeUnArticulo,
				pcv.PorcentajeReduccionPorRepartidor,
				pcv.ArticulosEspecial,
				pch.ComisionFija,
				pch.PorcentajeNormal,
				pch.PorcentajeRepartidor,
				pch.PorcentajeComplementarios,
				pch.PorcentajeArticuloEspecial,
				pch.PorcentajeUnArticulo,
				pch.PorcentajeReduccionPorRepartidor,
				pch.ArticulosEspecial,
				a.Articulos,
				a.Complementarias,
				vd.PrecioUnitario,
				vd.Costo,
				vd.Cantidad,
				pp.PrecioUno,
				pp.PrecioCinco,
				pp.PrecioCuatro,
				pp.PrecioDos,
				pp.PrecioTres,
				v.ComisionistaClienteID,
				c.ListaDePrecios,
				pch.VentaID,
				v.RepartidorID
			----	ve.Es9500
		)

		--select * from _PreComision
		,

		_PreComisionDev AS(
			SELECT 
				ve.VentaID,
				SUM
				(
				CASE 
				WHEN ve.Es9500 = 1 THEN
					((vd.PrecioUnitario - vd.Costo)* vd.Cantidad) * @PorComision9500
				ELSE
					CASE
						WHEN 
							@EsRepartidor = 1 
						THEN
							(
								((vd.PrecioUnitario - vd.Costo)* vd.Cantidad) * (pcv.PorcentajeRepartidor / 100)
							)
						ELSE
							(
								((
									CASE WHEN v.ComisionistaClienteID > 0 THEN 
										CASE c.ListaDePrecios
											WHEN 1 THEN (pp.PrecioUno / @IvaMul)
											WHEN 2 THEN (pp.PrecioDos / @IvaMul)
											WHEN 3 THEN (pp.PrecioTres / @IvaMul)
											WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
											WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
										END
									ELSE 
										CASE 
										WHEN vd.PrecioUnitario > (pp.PrecioUno / @IvaMul) THEN 
											(pp.PrecioUno /@IvaMul)
										ELSE	
											vd.PrecioUnitario
										END
									END
										- vd.Costo)* vd.Cantidad) *
									(
										
											CASE WHEN ve.VentaID = pch.VentaID THEN
										(
											CASE
												WHEN a.Complementarias = 1  THEN 
													pch.PorcentajeComplementarios 
												WHEN a.Articulos > pcv.ArticulosEspecial THEN 
													pch.PorcentajeArticuloEspecial
												WHEN a.Articulos = 1 THEN 
													pch.PorcentajeUnArticulo 
												ELSE 
													pch.PorcentajeNormal 
											END
												- (
											CASE WHEN v.RepartidorID > 0 THEN 
												pch.PorcentajeReduccionPorRepartidor 
											ELSE 
												0.0 
											END
													)
												) / 100
										--)
									ELSE
										(
											CASE
												WHEN a.Complementarias = 1  THEN 
													pcv.PorcentajeComplementarios 
												WHEN a.Articulos > pcv.ArticulosEspecial THEN 
													pcv.PorcentajeArticulosEspecial
												WHEN a.Articulos = 1 THEN 
													pcv.PorcentajeUnArticulo 
												ELSE 
													pcv.PorcentajeNormal 
											END
												- (
											CASE WHEN v.RepartidorID > 0 THEN 
												pcv.PorcentajeReduccionPorRepartidor 
											ELSE 
												0.0 
											END
													)
												) / 100
										--)
									END
										)
							)
					END 
				END 
				)
				AS ComisionIndividual,
				CASE 
					WHEN @EsRepartidor = 1 THEN 
						SUM(pcv.ComisionFijaRepartidor)
					--WHEN ve.Es9500 = 1 THEN
					--	 0.0
					ELSE 
						--SUM(pcv.PorcentajeReduccionPorRepartidor)
						SUM(pcv.ComisionFija)
				END AS ComisionFija,
				ve.Fecha AS Fecha
			FROM
				_VentasDev ve
				LEFT JOIN _DetalleDev d ON d.VentaID = ve.VentaID
				LEFT JOIN _Articulos a ON a.VentaID = ve.VentaID
				LEFT JOIN PartesComisionesView pcv ON pcv.ParteID = d.ParteID
				LEFT JOIN ParteComisionHistorico pch ON pch.ParteID = d.ParteID and pch.VentaID = ve.VentaID
				LEFT JOIN VentaDevolucionDetalle vd ON vd.VentaDevolucionID = ve.VentaDevolucionID
				LEFT JOIN Venta v ON v.VentaID = ve.VentaID
				LEFT JOIN Cliente c ON c.ClienteID = v.ComisionistaClienteID AND c.Estatus = 1
				LEFT JOIN PartePrecio pp ON pp.ParteID = d.ParteID
			WHERE
				d.VentaID = ve.VentaID
				and vd.ParteID = d.ParteID
			GROUP BY
				ve.VentaID,
				vd.VentaDevolucionID,
				ve.Fecha,
				ve.Cliente,
				ve.Folio,
				ve.Importe,
				ve.Cobranza,
				ve.Utilidad,
				ve.Es9500
		)
		SELECT
			pc.VentaID,
			ve.Fecha AS Fecha,
			c.Nombre AS Cliente,
			ve.Folio,
			CASE WHEN ve.ACredito = 1 THEN 0.00 ELSE Importe END AS Importe,
			CASE WHEN ve.ACredito = 1 THEN Importe ELSE 0.00 END AS Cobranza,
			CASE WHEN ve.Utilidad = 0.0 THEN ve.UtilidadDev ELSE ve.Utilidad END AS Utilidad,
			SUM(pc.ComisionIndividual) AS Comision,
			SUM(pc.ComisionFija) AS ComisionFija,
			ve.ACredito,
			ve.Es9500,
			(SELECT DISTINCT
				CASE WHEN vdd.VentaID = v.VentaID THEN 'VD' ELSE 'V' END
			FROM
				VentaDetalle vd
				INNER JOIN _Ventas v ON v.VentaID = pc.VentaID
				LEFT JOIN Venta v1 ON v1.VentaID = v.VentaID
				LEFT JOIN VentaDevolucion vdd ON vdd.VentaID = pc.VentaID
			WHERE
				vd.VentaID = v.VentaID
			GROUP BY
				vd.VentaID--,
				,v.VentaID
				,vdd.VentaID
				,vd.ParteID) AS Caracteristica
		FROM
			_Ventas ve
			LEFT JOIN 
			_PreComision pc ON pc.VentaID = ve.VentaID
			LEFT JOIN Venta v ON v.VentaID = ve.VentaID
			LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		WHERE
			pc.VentaID = ve.VentaID
		GROUP BY
			pc.VentaID,
			ve.Fecha,
			c.Nombre,
			ve.Folio,
			ve.Importe,
			ve.Es9500,
			ve.ACredito,
			ve.Utilidad,
			ve.UtilidadDev--,


		UNION 
		SELECT 
			vd.VentaID,
			vd.Fecha,
			vd.Cliente,
			vd.Folio,
			vd.Importe * -1 AS Importe,
			vd.Cobranza * -1 AS Cobranza,
			(CASE WHEN vd.Utilidad IS NULL THEN 0.0 ELSE vd.Utilidad * -1 END) AS Utilidad,
			(pcd.ComisionIndividual) * -1 AS Comision,
			(
				SELECT 
					SUM(ComisionFija)
				FROM
				_PreComisionDev pcd
				WHERE
					pcd.VentaID = vd.VentaID and pcd.Fecha = vd.Fecha
			) * -1 AS ComisionFija,
			CONVERT(BIT,0) as ACredito,
			CONVERT(BIT,vd.Es9500) AS Es9500
			,'D' AS Caracteristica
		FROM 
			_VentasDev vd
			LEFT JOIN _DetalleDev d ON d.VentaID = vd.VentaID
			LEFT JOIN _PreComisionDev pcd ON pcd.VentaID = vd.VentaID AND pcd.Fecha = vd.Fecha
		where
		vd.Fecha = vd.Fecha
		group by
		vd.VentaID,
		vd.VentaDevolucionID,
		vd.Fecha,
		vd.Cliente,
		vd.Folio,
		vd.Importe,
		vd.Cobranza,
		vd.Utilidad,
		vd.Es9500
		,pcd.ComisionIndividual
		order by
			Fecha
			,Caracteristica 
		
	END
END




IF @ModoID = @ModoNoPagadasID BEGIN
	;WITH _Ventas AS (
		SELECT	
			v.VentaID,
			v.Folio,
			MAX(v.Fecha) AS Fecha,
			SUM(vdc.Importe) AS Importe,
			(
				SELECT 
					CASE 
						WHEN SUM((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) IS NULL THEN	
							0.0
						ELSE
							SUM((vd.PrecioUnitario - vd.Costo) * vd.Cantidad)
					END
				FROM VentaDetalle vd
					INNER JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
				WHERE 
				--vd.Estatus = 1 AND 
				vd.VentaID = v.VentaID AND p.AplicaComision = 1
			)
			AS Utilidad,
			(
				SELECT 
					CASE 
						WHEN SUM((vdd.PrecioUnitario - vdd.Costo) * vdd.Cantidad) IS NULL THEN
							0.0
						ELSE 
							SUM((vdd.PrecioUnitario - vdd.Costo) * vdd.Cantidad) 
					END
				FROM
					VentaDevolucion vd
					INNER JOIN VentaDevolucionDetalle vdd ON vdd.VentaDevolucionID = vd.VentaDevolucionID AND vdd.Estatus = 1
					INNER JOIN Parte p ON p.ParteID = vdd.ParteID AND p.Estatus = 1
				WHERE
					vdd.Estatus = 1 AND 
					vd.VentaID = v.VentaID AND p.AplicaComision = 1
			)
			AS UtilidadDev,
			v.ACredito,
			'V' AS Caracteristica,
			CONVERT(BIT, CASE WHEN c9.VentaID IS NULL THEN 0 ELSE 1 END) AS Es9500		
		FROM
		
					Venta v
				LEFT JOIN (
					SELECT
						VentaId
						, SUM((PrecioUnitario + Iva) * Cantidad) AS Importe
					FROM VentaDetalle
					WHERE Estatus = 1
					GROUP BY VentaID
				) vdc ON vdc.VentaID = v.VentaID
				LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.EstatusGenericoID = @EstGenCompletado
						AND c9.Estatus = 1
				LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1

		WHERE 
			v.Estatus = 1
			AND v.RealizoUsuarioID = @VendedorID
			AND v.VentaEstatusID = @EstCobradaID
			AND (DATEPART(DW, v.Fecha) != 1 OR c9.VentaID IS NOT NULL)
		GROUP BY
			v.VentaID,
			c9.VentaID,
			v.Folio,	
			v.ACredito
	)
	,
	_DetalleVenta AS (
		SELECT 
			vd.VentaID,
			CASE WHEN vdd.VentaID = v.VentaID THEN 'VD' ELSE 'V' END AS Caracteristica,
			vd.parteId
		FROM
			VentaDetalle vd
			INNER JOIN _Ventas v ON v.VentaID = vd.VentaID
			LEFT JOIN Venta v1 ON v1.VentaID = v.VentaID
			LEFT JOIN VentaDevolucion vdd ON vdd.VentaID = vd.VentaID
		WHERE
			vd.VentaID = v.VentaID
		GROUP BY
			vd.VentaID--,
			,v.VentaID
			,vdd.VentaID
			,vd.ParteID
	)
	,_LineasComp AS (
		SELECT
			d.VentaID,
			p.ParteID,
			lc.ParteIDComplementaria
		FROM
			_DetalleVenta d
			INNER JOIN Parte p ON p.ParteID = d.ParteID AND p.Estatus = 1 AND p.AplicaComision = 1
			LEFT JOIN ParteComplementaria lc ON lc.ParteID = p.ParteID
	)
	--select * from _LineasComp
	,
	_Articulos AS (
		SELECT
			d.VentaID,
			CASE WHEN EXISTS(
				SELECT ParteID FROM _LineasComp WHERE VentaID = d.VentaID
					INTERSECT
				SELECT ParteIDComplementaria FROM _LineasComp WHERE VentaID = d.VentaID
				) 
			THEN 
				1
			ELSE 
				0
			END AS Complementarias,
			COUNT(d.ParteID) AS Articulos
		FROM 
			_DetalleVenta d
		GROUP BY 
			d.VentaID
	)
	--select * from _Articulos
	,

	_PreComision AS(
		SELECT 
			ve.VentaID,
			--d.ParteID,
			(
			CASE 
			WHEN ve.Es9500 = 1 THEN
				((vd.PrecioUnitario - vd.Costo)* vd.Cantidad) * @PorComision9500
			--WHEN a.Articulos > 1 THEN
			ELSE
				CASE
					WHEN 
						@EsRepartidor = 1 
					THEN
						(
							((vd.PrecioUnitario - vd.Costo)* vd.Cantidad) * (pcv.PorcentajeRepartidor / 100)
						)
					ELSE
						(
							((
							CASE WHEN v.ComisionistaClienteID > 0 THEN 
								CASE c.ListaDePrecios
									WHEN 1 THEN (pp.PrecioUno / @IvaMul)
									WHEN 2 THEN (pp.PrecioDos / @IvaMul)
									WHEN 3 THEN (pp.PrecioTres / @IvaMul)
									WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
									WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
								END
							ELSE 
								CASE 
								WHEN vd.PrecioUnitario > (pp.PrecioUno / @IvaMul) THEN 
									(pp.PrecioUno /@IvaMul)
								ELSE	
									vd.PrecioUnitario
								END
							END
							--((vd.PrecioUnitario - vd.Costo)* vd.Cantidad) *
								- vd.Costo)* vd.Cantidad) *
								(
								CASE WHEN ve.VentaID = pch.VentaID THEN
									(
										CASE
											WHEN a.Complementarias = 1  THEN 
												pch.PorcentajeComplementarios 
											WHEN a.Articulos > pcv.ArticulosEspecial THEN 
												pch.PorcentajeArticuloEspecial
											WHEN a.Articulos = 1 THEN 
												pch.PorcentajeUnArticulo 
											ELSE 
												pch.PorcentajeNormal 
										END
											- (
										CASE WHEN v.RepartidorID > 0 THEN 
											pch.PorcentajeReduccionPorRepartidor 
										ELSE 
											0.0 
										END
												)
											) / 100
									--)
								ELSE
									(
										CASE
											WHEN a.Complementarias = 1  THEN 
												pcv.PorcentajeComplementarios 
											WHEN a.Articulos > pcv.ArticulosEspecial THEN 
												pcv.PorcentajeArticulosEspecial
											WHEN a.Articulos = 1 THEN 
												pcv.PorcentajeUnArticulo 
											ELSE 
												pcv.PorcentajeNormal 
										END
											- (
										CASE WHEN v.RepartidorID > 0 THEN 
											pcv.PorcentajeReduccionPorRepartidor 
										ELSE 
											0.0 
										END
												)
											) / 100
									--)
								END
								)
						)
				END 
			END 
			)
			AS ComisionIndividual,
			CASE 
				WHEN @EsRepartidor = 1 THEN 
					--SUM
					(pcv.ComisionFijaRepartidor)
				--WHEN ve.Es9500 = 1 THEN
				--	 0.0
				ELSE 
					--SUM(pcv.PorcentajeReduccionPorRepartidor)
					--SUM
					(pcv.ComisionFija)
			END AS ComisionFija,
			d.ParteID
		FROM
			_Ventas ve
			LEFT JOIN _DetalleVenta d ON d.VentaID = ve.VentaID
			LEFT JOIN _Articulos a ON a.VentaID = d.VentaID
			LEFT JOIN PartesComisionesView pcv ON pcv.ParteID = d.ParteID
			LEFT JOIN ParteComisionHistorico pch ON pch.ParteID = d.ParteID and pch.VentaID = ve.VentaID
			LEFT JOIN VentaDetalle vd ON vd.VentaID = d.VentaID
			LEFT JOIN VentaDevolucion vdi ON vdi.VentaID = ve.VentaID
			LEFT JOIN VentaDevolucionDetalle vddi ON vddi.VentaDevolucionID = vdi.VentaDevolucionID and vddi.Estatus = 1
			LEFT JOIN Venta v ON v.VentaID = d.VentaID
			LEFT JOIN Cliente c ON c.ClienteID = v.ComisionistaClienteID AND c.Estatus = 1
			LEFT JOIN PartePrecio pp ON pp.ParteID = d.ParteID
		WHERE
			d.VentaID = ve.VentaID
			and vd.ParteID = d.ParteID
			--and vd.Estatus = 1 and a.Articulos > 1
		GROUP BY
			--ve.VentaID, v.ComisionistaClienteID, vd.VentaID
			d.ParteID,
			ve.VentaID,
			ve.Es9500,
			pcv.ComisionFija,
			pcv.ComisionFijaRepartidor,
			pcv.PorcentajeNormal,
			pcv.PorcentajeRepartidor,
			pcv.PorcentajeComplementarios,
			pcv.PorcentajeArticulosEspecial,
			pcv.PorcentajeUnArticulo,
			pcv.PorcentajeReduccionPorRepartidor,
			pcv.ArticulosEspecial,
			pch.ComisionFija,
			pch.PorcentajeNormal,
			pch.PorcentajeRepartidor,
			pch.PorcentajeComplementarios,
			pch.PorcentajeArticuloEspecial,
			pch.PorcentajeUnArticulo,
			pch.PorcentajeReduccionPorRepartidor,
			pch.ArticulosEspecial,
			a.Articulos,
			a.Complementarias,
			vd.PrecioUnitario,
			vd.Costo,
			vd.Cantidad,
			pp.PrecioUno,
			pp.PrecioCinco,
			pp.PrecioCuatro,
			pp.PrecioDos,
			pp.PrecioTres,
			v.ComisionistaClienteID,
			c.ListaDePrecios,
			pch.VentaID,
			v.RepartidorID
		----	ve.Es9500
	)
	SELECT
		--ve.VentaID,
		pc.VentaID,
		ve.Fecha AS Fecha,
		--ve.Fecha AS Fecha,
		c.Nombre AS Cliente,
		ve.Folio,
		CASE WHEN ve.ACredito = 1 THEN 0.00 ELSE Importe END AS Importe,
		CASE WHEN ve.ACredito = 1 THEN Importe ELSE 0.00 END AS Cobranza,
		CASE WHEN ve.Utilidad = 0.0 THEN ve.UtilidadDev ELSE ve.Utilidad END AS Utilidad,
		--(pc.ComisionIndividual - CASE WHEN a.Articulos > 1 THEN ISNULL(pcd.ComisionIndividual, 0.00)ELSE 0 END) AS Comision,
		SUM(pc.ComisionIndividual) AS Comision,
		SUM(pc.ComisionFija) AS ComisionFija,
		ve.ACredito,
		ve.Es9500,
		--pd.Caracteristica AS Caracteristica
		(SELECT DISTINCT
			CASE WHEN vdd.VentaID = v.VentaID THEN 'VD' ELSE 'V' END
		FROM
			VentaDetalle vd
			INNER JOIN _Ventas v ON v.VentaID = pc.VentaID
			LEFT JOIN Venta v1 ON v1.VentaID = v.VentaID
			LEFT JOIN VentaDevolucion vdd ON vdd.VentaID = pc.VentaID
		WHERE
			vd.VentaID = v.VentaID
		GROUP BY
			vd.VentaID--,
			,v.VentaID
			,vdd.VentaID
			,vd.ParteID) AS Caracteristica,
		1 AS Orden
	FROM
		_Ventas ve
		LEFT JOIN 
		_PreComision pc ON pc.VentaID = ve.VentaID
		LEFT JOIN Venta v ON v.VentaID = ve.VentaID
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
	WHERE
		pc.VentaID = ve.VentaID
		--and vd.Estatus = 1
		--and ve.Fecha >= @Desde AND ve.Fecha <@Hasta
	GROUP BY
		pc.VentaID,
		ve.Fecha,
		c.Nombre,
		ve.Folio,
		ve.Importe,
		ve.Es9500,
		ve.ACredito,
		ve.Utilidad,
		ve.UtilidadDev--,
END


END

GO


/*-----------------------------------------------------------------------------------------*/