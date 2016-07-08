BEGIN TRAN

DECLARE @EntradaTras INT = 7
DECLARE @SalidaTras INT = 8
DECLARE @CcaInventario INT = 353

INSERT INTO ContaPoliza (Fecha, ContaTipoPolizaID, Concepto, RealizoUsuarioID, SucursalID, Reportar
, RelacionTabla, RelacionID, Origen, FueManual)
	SELECT
		pk.Fecha
		, 3
		, ('TRASPASO ORIGEN ' + pk.Origen + ' DESTINO ' + pk.Destino)
		, 1
		, CASE WHEN OperacionID = @SalidaTras THEN
			CASE pk.Origen
				WHEN 'MATRIZ' THEN 1
				WHEN 'Suc02' THEN 2
				WHEN 'Suc03' THEN 3
			END
		ELSE
			CASE pk.Destino
				WHEN 'MATRIZ' THEN 1
				WHEN 'Suc02' THEN 2
				WHEN 'Suc03' THEN 3
			END
		END
		, 0
		, SUM(pk.Importe) AS ImporteCosto
		, mi.MovimientoInventarioID
		, 'dmod'
		, CASE WHEN pk.OperacionID = @EntradaTras THEN 1 ELSE 0 END
	FROM
		ParteKardex pk
		-- LEFT JOIN MovimientoInventarioDetalle mid ON mid.MovimientoInventarioID = pk.Folio AND mid.Estatus = 1
		LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = pk.Folio AND mi.Estatus = 1
	WHERE
		pk.OperacionID IN (@EntradaTras, @SalidaTras)
		AND (pk.Fecha >= '2015-06-01' AND pk.Fecha < '2015-07-16')
	GROUP BY
		pk.Fecha
		, pk.Origen
		, pk.Destino
		, pk.OperacionID
		, pk.SucursalID
		, mi.MovimientoInventarioID

INSERT INTO ContaPolizaDetalle (ContaPolizaID, ContaCuentaAuxiliarID, Referencia, Cargo, Abono)
	SELECT
		cp.ContaPolizaID
		, @CcaInventario
		, CASE WHEN cp.FueManual = 1 THEN ur.NombreUsuario ELSE us.NombreUsuario END
		, CASE WHEN cp.FueManual = 1 THEN cp.RelacionTabla ELSE '0.0' END AS Cargo
		, CASE WHEN cp.FueManual = 1 THEN '0.0' ELSE cp.RelacionTabla END AS Abono
	FROM
		ContaPoliza cp
		LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = cp.RelacionID AND mi.Estatus = 1
		LEFT JOIN Usuario us ON us.UsuarioID = mi.UsuarioSolicitoTraspasoID AND us.Estatus = 1
		LEFT JOIN Usuario ur ON ur.UsuarioID = mi.UsuarioRecibioTraspasoID AND ur.Estatus = 1
	WHERE Origen = 'dmod'

select * from ContaPoliza where Origen = 'dmod'
select * from ContaPolizaDetalle where ContaPolizaID in (select ContaPolizaID from ContaPoliza where Origen = 'dmod')

UPDATE ContaPoliza SET
	Origen = NULL
	, RelacionTabla = 'MovimientoInventario'
	, FueManual = NULL
WHERE Origen = 'dmod'

ROLLBACK TRAN