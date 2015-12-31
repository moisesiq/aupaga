/* Script con modificaciones a la base de datos de Theos. Archivo 058
 * Creado: 2015/12/29
 * Subido: 2015/12/30
 */

DECLARE @ScriptID INT = 58
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)
-- SELECT * FROM ScriptSql ORDER BY ScriptID DESC

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE Cliente ADD SiempreTicket BIT NULL

DELETE FROM ParteEquivalente WHERE ParteID IN
	(SELECT ParteID FROM ParteEquivalente GROUP BY ParteID HAVING COUNT(*) > 1)
ALTER TABLE ParteEquivalente ADD CONSTRAINT Un_ParteID UNIQUE (ParteID)

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
		, c.SiempreTicket
		, c.SiempreVale
		, c.TicketPrecio1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

