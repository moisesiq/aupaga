/** Script que muestra el resultado del total de los pagos de un cliente por Año **/



SET DATEFIRST 6;

DECLARE
    -- usar 0 para todas las coincidencias sin filtro
	@Anio INT = 2017,
	@Cliente INT = 0

;WITH Resumen As (

SELECT 
DATEPART(week, Fecha) AS Semana, 
SUM(Importe) AS Total
FROM VentasPagosView 
WHERE 
ClienteID = CASE WHEN @Cliente > 0 THEN @Cliente ELSE ClienteID END
AND DATEPART(YYYY,Fecha) = CASE WHEN @Anio > 0 THEN @Anio ELSE DATEPART(YYYY,Fecha) END
AND ACredito = 1
group by Fecha,DATEPART(week, Fecha)
)
--select Semana, SUM(Total)AS Total from Resumen group by Semana order by Semana
select Semana,'$'+convert(varchar(50), CAST(SUM(Total)as money), -1)  AS Total from Resumen group by Semana order by Semana