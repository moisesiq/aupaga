/* Script con modificaciones a la base de datos de Theos. Archivo 026
 * Creado: 2015/07/13
 * Subido: 2015/07/23
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE CajaFacturaGlobal ADD
	FacturadoVales DECIMAL(12, 2) NULL
	, VentaFacturaID INT NULL FOREIGN KEY REFERENCES VentaFactura(VentaFacturaID)

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

ALTER VIEW [dbo].[NominaUsuariosView] AS
	SELECT
		nu.NominaUsuarioID
		, n.NominaID
		, n.Semana
		, n.BancoCuentaID
		, nu.IdUsuario AS UsuarioID
		, u.NombreUsuario AS Usuario
		, nu.SucursalID
		, s.NombreSucursal AS Sucursal
		, nuoc.Total AS TotalOficial
		, nu.SuperoMinimo
		, nu.SueldoFijo
		, nu.SueldoVariable
		, nu.Sueldo9500
		, nu.SueldoMinimo
		, nu.Bono
		, nu.Adicional
		, (nu.Sueldo9500 + nu.Bono + nu.Adicional +
			CASE WHEN nu.SuperoMinimo = 1 THEN (nu.SueldoFijo + nu.SueldoVariable)
			ELSE nu.SueldoMinimo END) AS TotalSueldo
		, (
			(nu.Sueldo9500 + nu.Bono + nu.Adicional +
				CASE WHEN nu.SuperoMinimo = 1 THEN (nu.SueldoFijo + nu.SueldoVariable)
				ELSE nu.SueldoMinimo END)
			- ISNULL(nuoc.Total, 0.0)
			) AS Diferencia
		, nu.Tickets
		, nu.Adelanto
		, nu.MinutosTarde
		, nu.Otros
		, (nu.Tickets + nu.Adelanto + nu.MinutosTarde + nu.Otros) AS TotalDescuentos
		, (
			((nu.Sueldo9500 + nu.Bono + nu.Adicional +
				CASE WHEN nu.SuperoMinimo = 1 THEN (nu.SueldoFijo + nu.SueldoVariable)
				ELSE nu.SueldoMinimo END)
			- nuoc.Total)
			- (nu.Tickets + nu.Adelanto + nu.MinutosTarde + nu.Otros)
			) AS Liquido
	FROM
		NominaUsuario nu
		LEFT JOIN Nomina n ON n.NominaID = nu.NominaID
		LEFT JOIN (
			SELECT
				NominaID
				, IdUsuario
				, SUM(CASE WHEN Suma = 1 THEN Importe ELSE Importe * -1 END) AS Total
			FROM NominaUsuarioOficial
			GROUP BY
				NominaID
				, IdUsuario
		) nuoc ON nuoc.IdUsuario = nu.IdUsuario AND nuoc.NominaID = nu.NominaID
		LEFT JOIN Usuario u ON u.UsuarioID = nu.IdUsuario AND u.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = nu.SucursalID AND s.Estatus = 1
GO

ALTER VIEW [dbo].[VentasPagosDetalleAvanzadoView] AS
	SELECT
		vpd.VentaPagoDetalleID
		, vpd.TipoFormaPagoID AS FormaDePagoID
		, tfp.NombreTipoFormaPago AS FormaDePago
		, vpd.Importe
		, vpd.BancoID
		, b.NombreBanco AS Banco
		, vpd.Folio
		, vpd.Cuenta
		, vpd.NotaDeCreditoID
		, ISNULL(vpdr.Resguardado, 0) AS Resguardado
		, vp.VentaPagoID
		, vp.Fecha
		, vp.SucursalID
		, vp.VentaID
		, SUM(vpd.Importe) AS ImportePago
		, ISNULL(CONVERT(BIT, vf.VentaFacturaID), 0) AS Facturada
		, v.Folio AS FolioVenta
		, v.Fecha AS FechaVenta
		, v.VentaEstatusID
		, v.ClienteID
		, c.Nombre AS Cliente
		, v.ACredito
		, uv.NombreUsuario AS Vendedor
	FROM
		VentaPagoDetalle vpd
		LEFT JOIN VentaPago vp ON vp.VentaPagoID = vpd.VentaPagoID AND vp.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario uv ON uv.UsuarioID = v.RealizoUsuarioID AND uv.Estatus = 1
		LEFT JOIN VentaFacturaDetalle vfd ON vfd.VentaID = v.VentaID AND vfd.Estatus = 1
		LEFT JOIN VentaFactura vf ON vf.VentaFacturaID = vfd.VentaFacturaID AND vf.EstatusGenericoID = 3 AND vf.Estatus = 1
		LEFT JOIN Banco b ON b.BancoID = vpd.BancoID AND b.Estatus = 1
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = vpd.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN VentaPagoDetalleResguardo vpdr ON vpdr.VentaPagoDetalleID = vpd.VentaPagoDetalleID
	WHERE vpd.Estatus = 1
	GROUP BY
		vpd.VentaPagoDetalleID
		, vpd.TipoFormaPagoID
		, tfp.NombreTipoFormaPago
		, vpd.Importe
		, vpd.BancoID
		, b.NombreBanco
		, vpd.Folio
		, vpd.Cuenta
		, vpd.NotaDeCreditoID
		, vpdr.Resguardado
		, vp.VentaPagoID
		, vp.Fecha
		, vp.SucursalID
		, vp.VentaID
		, vf.VentaFacturaID
		, v.Folio
		, v.Fecha
		, v.VentaEstatusID
		, v.ClienteID
		, c.Nombre
		, v.ACredito
		, uv.NombreUsuario
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

ALTER PROCEDURE [dbo].[pauPartesMaster] (
	@Desde DATE
	, @Hasta DATE
	-- , @ProveedorID INT = NULL
	-- , @MarcaID INT = NULL
	-- , @LineaID INT = NULL
	, @Proveedores tpuTablaEnteros READONLY
	, @Marcas tpuTablaEnteros READONLY
	, @Lineas tpuTablaEnteros READONLY
) AS BEGIN
	SET NOCOUNT ON

	/* DECLARE @Desde DATE = '2013-02-01'
	DECLARE @Hasta DATE = '2014-03-31'
	DECLARE @ProveedorID INT = NULL
	DECLARE @MarcaID INT = NULL
	DECLARE @LineaID INT = NULL
	*/

	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)

	-- Procedimiento
	
	SELECT
		p.ParteID
		, p.CodigoBarra AS CodigoDeBara
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, p.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, p.MarcaParteID
		, m.NombreMarcaParte AS Marca
		, p.LineaID
		, l.NombreLinea AS Linea
		, p.MedidaID
		, md.NombreMedida AS UnidadDeMedida
		, p.UnidadEmpaque AS UnidadDeEmpaque
		, p.AplicaComision
		, p.EsServicio
		, p.Etiqueta
		, p.SoloUnaEtiqueta
		, p.EsPar
		, pec.Existencia
		, ISNULL(vc.Ventas, 0) AS Ventas
		, pp.Costo
		, pp.PorcentajeUtilidadUno
		, pp.PrecioUno
		, pp.PorcentajeUtilidadDos
		, pp.PrecioDos
		, pp.PorcentajeUtilidadTres
		, pp.PrecioTres
		, pp.PorcentajeUtilidadCuatro
		, pp.PrecioCuatro
		, pp.PorcentajeUtilidadCinco
		, pp.PrecioCinco
	FROM
		Parte p
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte m ON m.MarcaParteID = p.MarcaParteID AND m.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN Medida md ON md.MedidaID = p.MedidaID AND md.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND pp.Estatus = 1
		LEFT JOIN (
			SELECT
				ParteID
				, SUM(Existencia) AS Existencia
			FROM ParteExistencia
			WHERE Estatus = 1
			GROUP BY ParteID
		) pec ON pec.ParteID = p.ParteID
		LEFT JOIN (
			SELECT
				vd.ParteID
				, COUNT(v.VentaID) AS Ventas
			FROM
				VentaDetalle vd
				INNER JOIN Venta v ON v.VentaID = vd.VentaID
			WHERE
				vd.Estatus = 1
				AND v.Estatus = 1
				AND v.VentaEstatusID = @EstPagadaID
				AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			GROUP BY vd.ParteID
		) vc ON vc.ParteID = p.ParteID
	WHERE
		p.Estatus = 1
		-- AND (@ProveedorID IS NULL OR p.ProveedorID = @ProveedorID)
		-- AND (@MarcaID IS NULL OR p.MarcaParteID = @MarcaID)
		-- AND (@LineaID IS NULL OR p.LineaID = @LineaID)
		AND (NOT EXISTS(SELECT TOP 1 1 FROM @Proveedores) OR p.ProveedorID IN (SELECT Entero FROM @Proveedores))
		AND (NOT EXISTS(SELECT TOP 1 1 FROM @Marcas) OR p.MarcaParteID IN (SELECT Entero FROM @Marcas))
		AND (NOT EXISTS(SELECT TOP 1 1 FROM @Lineas) OR p.LineaID IN (SELECT Entero FROM @Lineas))

END
GO