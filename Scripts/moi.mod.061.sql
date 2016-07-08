/* Script con modificaciones a la base de datos de Theos. Archivo 061
 * Creado: 2016/01/13
 * Subido: 2016/02/15
 */

DECLARE @ScriptID INT = 61
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)
-- SELECT * FROM ScriptSql ORDER BY ScriptID DESC

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
	('Facturacion.ClaveCiec', 'Lucrecia28', '', 'Clave Ciec del Sat.')
	, ('Facturacion.RutaDescargaXmls', 'C:\Theos\Archivos\CFDI Proveedor\{_Anio}\{_Mes}\{_Proveedor}\{_Uuid}.xml', 'C:\Theos\Archivos\CFDI Proveedor\{_Anio}\{_Mes}\{_Proveedor}\{_Uuid}.xml', 'Ruta, con un formato especial, donde se descargarán los archivos xml del Sat.')

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

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
		, c.SiempreTicket
		, c.SiempreVale
		, c.TicketPrecio1
		, SUM(ca.Adeudo) AS Adeudo
		, MIN(ca.Fecha) AS FechaPrimerAdeudo
		-- , ISNULL(CONVERT(BIT, CASE WHEN (ca.Adeudo >= c.LimiteCredito OR ca.FechaPrimerAdeudo >= (GETDATE() + c.DiasDeCredito))
		-- 	THEN 1 ELSE 0 END), 0) AS CreditoVencido
		, SUM(CASE WHEN ca.Dias >= c.DiasDeCredito THEN ca.Adeudo ELSE 0.0 END) AS AdeudoVencido
		, (SELECT AVG(DiasPago) FROM _Tiempos WHERE ClienteID = c.ClienteID) AS PromedioDePagoAnual
		, (SELECT AVG(DiasPago) FROM _Tiempos WHERE ClienteID = c.ClienteID
			-- AND Fecha > DATEADD(MONTH, -3, GETDATE())) AS PromedioDePago3Meses
			-- Por alguna extraña razón, el filtro se debe de hacer con "Fecha <=" y "NOT IN" en vez de "Fecha >", pues si
			-- se hace de esta segunda manera, la consulta se incrementa como 12 segundos en tiempo de ejecución.
			AND VentaID NOT IN (SELECT VentaID FROM _Tiempos WHERE Fecha <= DATEADD(MONTH, -3, GETDATE())))
			AS PromedioDePago3Meses
	FROM
		Cliente c
		LEFT JOIN (
			SELECT
				vi.VentaID
				, vi.ClienteID
				, vi.Fecha
				, SUM(ISNULL(vdi.Total, 0.0) - ISNULL(vpi.Pagado, 0.0)) AS Adeudo
				, DATEDIFF(DAY, vi.Fecha, GETDATE()) AS Dias
			FROM
				Venta vi
				LEFT JOIN (
					SELECT VentaID, SUM((PrecioUnitario + Iva) * Cantidad) AS Total
					FROM VentaDetalle
					WHERE Estatus = 1
					GROUP BY VentaID
				) vdi ON vdi.VentaID = vi.VentaID
				LEFT JOIN (
					SELECT vpi.VentaID, SUM(vpdi.Importe) AS Pagado
					FROM
						VentaPago vpi
						LEFT JOIN VentaPagoDetalle vpdi ON vpdi.VentaPagoID = vpi.VentaPagoID AND vpdi.Estatus = 1
					WHERE vpi.Estatus = 1
					GROUP BY vpi.VentaID
				) vpi ON vpi.VentaID = vi.VentaID
			WHERE
				vi.Estatus = 1
				AND vi.ACredito = 1
				AND VentaEstatusID = 2
			GROUP BY
				vi.VentaID
				, vi.ClienteID
				, vi.Fecha
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
		, c.SiempreTicket
		, c.SiempreVale
		, c.TicketPrecio1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

