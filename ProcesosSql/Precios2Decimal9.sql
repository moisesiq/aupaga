/*
 * Script para ajustar los precios cuando su segundo decimal es un 9
 */

select pp.*
from parteprecio pp where
	ROUND(PrecioUno, 1) != PrecioUno
	OR ROUND(PrecioDos, 1) != PrecioDos
	OR ROUND(PrecioTres, 1) != PrecioTres
	OR ROUND(PrecioCuatro, 1) != PrecioCuatro
	OR ROUND(PrecioCinco, 1) != PrecioCinco
order by PrecioCinco

begin tran
update PartePrecio set PrecioUno = ROUND(PrecioUno, 1) where RIGHT(convert(varchar, preciouno), 1) != '0'
update PartePrecio set PrecioDos = ROUND(PrecioDos, 1) where RIGHT(convert(varchar, PrecioDos), 1) != '0'
update PartePrecio set PrecioTres = ROUND(PrecioTres, 1) where RIGHT(convert(varchar, PrecioTres), 1) != '0'
update PartePrecio set PrecioCuatro = ROUND(PrecioCuatro, 1) where RIGHT(convert(varchar, PrecioCuatro), 1) != '0'
update PartePrecio set PrecioCinco = ROUND(PrecioCinco, 1) where RIGHT(convert(varchar, PrecioCinco), 1) != '0'
select * from PartePrecio where RIGHT(convert(varchar, preciouno), 1) != '0'
rollback tran

begin tran
update PartePrecio set PrecioUno = PrecioUno + 0.01 where right(CONVERT(varchar, preciouno), 1) = '9'
update PartePrecio set PrecioDos = PrecioDos + 0.01 where right(CONVERT(varchar, PrecioDos), 1) = '9'
update PartePrecio set PrecioTres = PrecioTres + 0.01 where right(CONVERT(varchar, PrecioTres), 1) = '9'
update PartePrecio set PrecioCuatro = PrecioCuatro + 0.01 where right(CONVERT(varchar, PrecioCuatro), 1) = '9'
update PartePrecio set PrecioCinco = PrecioCinco + 0.01 where right(CONVERT(varchar, PrecioCinco), 1) = '9'
-- select * from PartePrecio where right(CONVERT(varchar, preciouno), 1) = '9'
rollback tran