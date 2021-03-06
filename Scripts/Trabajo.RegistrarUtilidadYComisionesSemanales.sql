/* Script para registrar, cada semana, la utilidad y las comisiones
 * Creado: 2014/05/19
 * Subido: 2014/05/19
 * Por: Moisés
 */

DECLARE @Desde DATE = DATEADD(d, -7, GETDATE())
DECLARE @Hasta DATE = DATEADD(d, -1, GETDATE())
DECLARE @Registro DATE = @Hasta

-- Definición de variables tipo constante
DECLARE @EstPagadaID INT = 3
DECLARE @EstGenCompletado INT = 3

-- Variables calculadas para el proceso
SET @Hasta = DATEADD(d, 1, @Hasta)
-- De momento no se toma en cuenta la sucursal
DECLARE @PorComision DECIMAL(5, 2) = (SELECT (CONVERT(DECIMAL(5, 2), Valor) / 100) FROM Configuracion WHERE Nombre = 'Comisiones.Vendedor.Porcentaje')
DECLARE @PorComision9500 DECIMAL(5, 2) = (SELECT (CONVERT(DECIMAL(5, 2), Valor) / 100) FROM Configuracion WHERE Nombre = 'Comisiones.Vendedor.Porcentaje9500')
DECLARE @IvaMul DECIMAL(5, 2) = (SELECT (1 + (CONVERT(DECIMAL(5, 2), Valor) / 100)) FROM Configuracion WHERE Nombre = 'IVA')

-- Procedimiento
DECLARE @Datos TABLE (
	SucursalID INT
	, VendedorID INT
	, Utilidad DECIMAL(12, 2)
	, UtilidadConDescuento DECIMAL(12, 2)
	, Comision DECIMAL(12, 2)
)
INSERT INTO @Datos
	SELECT
		-- vp.VentaID
		-- , MAX(vp.Fecha) AS Fecha
		v.SucursalID
		, v.RealizoUsuarioID
		-- , SUM((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) AS Importe
		, SUM((vd.PrecioUnitario - pp.Costo) * vd.Cantidad) AS Utilidad
		, SUM((vd.PrecioUnitario - ISNULL(pp.CostoConDescuento, pp.Costo)) * vd.Cantidad) AS UtilidadConDescuento
		, (
			CASE WHEN v.ComisionistaClienteID > 0 THEN
				SUM(((
					CASE com.ListaDePrecios
						WHEN 1 THEN (pp.PrecioUno / @IvaMul)
						WHEN 2 THEN (pp.PrecioDos / @IvaMul)
						WHEN 3 THEN (pp.PrecioTres / @IvaMul)
						WHEN 4 THEN (pp.PrecioCuatro / @IvaMul)
						WHEN 5 THEN (pp.PrecioCinco / @IvaMul)
					END
					- pp.Costo) * vd.Cantidad)
					* CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE @PorComision9500 END)
			ELSE
				SUM(((vd.PrecioUnitario - pp.Costo) * vd.Cantidad)
				* CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE @PorComision9500 END)
			END
		) AS Comision
	FROM
		VentaPago vp
		INNER JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
		INNER JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
		LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		LEFT JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = vd.ParteID AND pp.Estatus = 1
		LEFT JOIN Cliente com ON com.ClienteID = v.ComisionistaClienteID AND com.Estatus = 1
		LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.EstatusGenericoID = @EstGenCompletado
			AND c9.Estatus = 1
	WHERE
		vp.Estatus = 1
		AND (vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
		AND v.VentaEstatusID = @EstPagadaID
	GROUP BY
		vp.VentaID
		, v.SucursalID
		, v.RealizoUsuarioID
		, v.ComisionistaClienteID
		, c9.VentaID

-- Se insertan las utilidades
INSERT INTO MetaUtilidadSucursal (SucursalID, Fecha, Utilidad, UtilidadConDescuento)
	SELECT
		SucursalID
		, @Registro
		, SUM(Utilidad)
		, SUM(UtilidadConDescuento)
	FROM @Datos
	GROUP BY SucursalID

-- Se insertan las comisiones
INSERT INTO MetaComisionVendedor (VendedorID, Fecha, Comision)
	SELECT
		VendedorID
		, @Registro
		, SUM(Comision)
	FROM @Datos
	GROUP BY VendedorID