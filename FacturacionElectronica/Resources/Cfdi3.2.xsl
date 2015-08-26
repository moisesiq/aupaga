<?xml version="1.0" encoding="UTF-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>

	<xsl:template match="/FacturaElectronica">
		<cfdi:Comprobante
			xmlns:cfdi="http://www.sat.gob.mx/cfd/3"
			xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
			xsi:schemaLocation="http://www.sat.gob.mx/cfd/3 http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv32.xsd"
		>

			<!-- Variables de totales -->
			<xsl:variable name="TotalesSubtotal" select="format-number(Subtotal, '0.00')" />
			<xsl:variable name="TotalesIva" select="format-number(Iva, '0.00')" />
			<xsl:variable name="TotalesDescuento" select="format-number(Descuento, '0.00')" />
			<xsl:variable name="TotalesTotal" select="format-number(Total, '0.00')" />

			<!-- Atributos -->
			<xsl:attribute name="version"><xsl:value-of select="Version" /></xsl:attribute>
			<xsl:attribute name="fecha"><xsl:value-of select="substring(Fecha, 1, 19)" /></xsl:attribute>
			<xsl:attribute name="sello">v</xsl:attribute>
			<xsl:attribute name="formaDePago"><xsl:value-of select="FormaDePago" /></xsl:attribute>
			<xsl:attribute name="noCertificado">00000000000000000000</xsl:attribute>
			<xsl:attribute name="certificado">v</xsl:attribute>
			<xsl:attribute name="tipoDeComprobante"><xsl:value-of select="TipoDeComprobante" /></xsl:attribute>

			<xsl:attribute name="metodoDePago"><xsl:value-of select="MetodoDePago" /></xsl:attribute>
			<xsl:attribute name="LugarExpedicion"><xsl:value-of select="LugarDeExpedicion" /></xsl:attribute>

			<xsl:attribute name="subTotal"><xsl:value-of select="$TotalesSubtotal" /></xsl:attribute>
			<xsl:attribute name="descuento"><xsl:value-of select="$TotalesDescuento" /></xsl:attribute>
			<xsl:attribute name="total"><xsl:value-of select="$TotalesTotal" /></xsl:attribute>

			<xsl:if test="Serie">
				<xsl:attribute name="serie"><xsl:value-of select="Serie" /></xsl:attribute>
			</xsl:if>
			<xsl:if test="Folio">
				<xsl:attribute name="folio"><xsl:value-of select="Folio" /></xsl:attribute>
			</xsl:if>

			<!-- Elementos -->

			<!-- Datos del Emisor -->
			<cfdi:Emisor>
				<!-- Atributos -->
				<xsl:attribute name="rfc"><xsl:value-of select="Emisor/RFC" /></xsl:attribute>
				<xsl:if test="Emisor/Nombre">
					<xsl:attribute name="nombre"><xsl:value-of select="Emisor/Nombre" /></xsl:attribute>
				</xsl:if>
				<!-- Elementos -->
				<xsl:for-each select="Emisor/DomicilioFiscal">
					<cfdi:DomicilioFiscal>
						<xsl:call-template name="Ubicacion" />
					</cfdi:DomicilioFiscal>
				</xsl:for-each>
				<xsl:for-each select="Emisor/ExpedidoEn">
					<cfdi:ExpedidoEn>
						<xsl:call-template name="Ubicacion" />
					</cfdi:ExpedidoEn>
				</xsl:for-each>
				<xsl:for-each select="Emisor/RegimenesFiscales/string">
					<cfdi:RegimenFiscal>
						<xsl:attribute name="Regimen"><xsl:value-of select="." /></xsl:attribute>
					</cfdi:RegimenFiscal>
				</xsl:for-each>
			</cfdi:Emisor>

			<!-- Datos del Receptor -->
			<cfdi:Receptor>
				<!-- Atributos -->
				<xsl:attribute name="rfc"><xsl:value-of select="Receptor/RFC" /></xsl:attribute>
				<xsl:if test="Receptor/Nombre">
					<xsl:attribute name="nombre"><xsl:value-of select="Receptor/Nombre" /></xsl:attribute>
				</xsl:if>
				<!-- Elementos -->
				<xsl:for-each select="Receptor/DomicilioFiscal">
					<cfdi:Domicilio>
						<xsl:call-template name="Ubicacion" />
					</cfdi:Domicilio>
				</xsl:for-each>
			</cfdi:Receptor>

			<!-- Conceptos -->
			<cfdi:Conceptos>
				<xsl:for-each select="Conceptos/Concepto">
					<cfdi:Concepto>
						<!-- Atributos -->
						<xsl:if test="Identificador">
							<xsl:attribute name="noIdentificacion"><xsl:value-of select="Identificador" /></xsl:attribute>
						</xsl:if>
						<xsl:attribute name="cantidad"><xsl:value-of select="Cantidad" /></xsl:attribute>
						<xsl:attribute name="unidad"><xsl:value-of select="Unidad" /></xsl:attribute>
						<xsl:attribute name="descripcion"><xsl:value-of select="Descripcion" /></xsl:attribute>
						<xsl:attribute name="valorUnitario"><xsl:value-of select="ValorUnitario" /></xsl:attribute>
						<xsl:attribute name="importe"><xsl:value-of select="format-number(ValorUnitario * Cantidad, '0.00')" /></xsl:attribute>
					</cfdi:Concepto>
				</xsl:for-each>
			</cfdi:Conceptos>

			<!-- Impuestos -->
			<cfdi:Impuestos>
				<xsl:if test="Conceptos/Concepto">  <!-- que exista al menos un concepto -->
					<!-- Atributos -->
					<xsl:attribute name="totalImpuestosTrasladados">
						<xsl:value-of select="$TotalesIva" />
					</xsl:attribute>
					<!-- Elementos -->
					<cfdi:Traslados>
						<cfdi:Traslado>
							<!-- Atributos -->
							<xsl:attribute name="impuesto">IVA</xsl:attribute>
							<xsl:attribute name="importe"><xsl:value-of select="$TotalesIva" /></xsl:attribute>
							<xsl:attribute name="tasa"><xsl:value-of select="TasaDeImpuesto" /></xsl:attribute>
						</cfdi:Traslado>
					</cfdi:Traslados>
				</xsl:if>
			</cfdi:Impuestos>

		</cfdi:Comprobante>
	</xsl:template>

	<xsl:template name="Ubicacion">
		<!-- Atributos -->
		<xsl:attribute name="calle"><xsl:value-of select="Calle" /></xsl:attribute>
		<xsl:if test="NumeroExterior != ''">
			<xsl:attribute name="noExterior"><xsl:value-of select="NumeroExterior" /></xsl:attribute>
		</xsl:if>
		<xsl:if test="NumeroInterior != ''">
			<xsl:attribute name="noInterior"><xsl:value-of select="NumeroInterior" /></xsl:attribute>
		</xsl:if>
		<xsl:if test="Referencia != ''">
			<xsl:attribute name="referencia"><xsl:value-of select="Referencia" /></xsl:attribute>
		</xsl:if>
		<xsl:if test="Colonia != ''">
			<xsl:attribute name="colonia"><xsl:value-of select="Colonia" /></xsl:attribute>
		</xsl:if>
		<xsl:attribute name="codigoPostal"><xsl:value-of select="CodigoPostal" /></xsl:attribute>
		<xsl:if test="Localidad != ''">
			<xsl:attribute name="localidad"><xsl:value-of select="Localidad" /></xsl:attribute>
		</xsl:if>
		<xsl:attribute name="municipio"><xsl:value-of select="Municipio" /></xsl:attribute>
		<xsl:attribute name="estado"><xsl:value-of select="Estado" /></xsl:attribute>
		<xsl:attribute name="pais"><xsl:value-of select="Pais" /></xsl:attribute>
	</xsl:template>

</xsl:stylesheet>