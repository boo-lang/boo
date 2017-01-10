<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<!-- -->
	<xsl:output method="xml" indent="yes" encoding="utf-8" omit-xml-declaration="no" />
	<!-- -->
	<xsl:param name='assembly-name' />
	<!-- -->
	<xsl:template match="node()|@*"/>
	<!--
	 | Identity Template
	 +-->
	<xsl:template match="node()|@*" mode="slashdoc">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()" mode="slashdoc" />
		</xsl:copy>
	</xsl:template>
	<!-- 
	 | Top-level structure
	 +-->
	<xsl:template match="/">
		<xsl:variable name="assy" select="ndoc/assembly[@name=$assembly-name][1]" />
		<doc>
			<assembly>
				<name>
					<xsl:value-of select="$assembly-name" />
				</name>
			</assembly>
			<members>
				<xsl:apply-templates select="$assy/module/namespace/*" mode="types" />
			</members>
		</doc>
	</xsl:template>
	<!-- 
	 | Types 
	 +-->
	<xsl:template match="node()|@*" mode="types"/>
	<xsl:template match="class|structure|interface|enumeration|delegate" mode="types">
		<member>
			<xsl:attribute name="name">
				<xsl:value-of select="@id" />
			</xsl:attribute>
			<xsl:apply-templates select="./documentation/node()" mode="doc" />
		</member>
		<!-- -->
		<xsl:apply-templates select="./*"  mode="members"/>
	</xsl:template>
	<!-- 
	 | Members
	 +-->
	<xsl:template match="node()|@*" mode="members"/>
	<xsl:template match="constructor|field|property|method|operator|event" mode="members">
		<member>
			<xsl:attribute name="name">
				<xsl:value-of select="@id" />
			</xsl:attribute>
			<xsl:apply-templates select="./documentation/node()" mode="doc" />
		</member>
	</xsl:template>
	<!-- 
	 | Doc Tags
	 +-->
	<xsl:template match="node()|@*" mode="doc"/>
	<xsl:template match="summary|param|exception" mode="doc">
			<xsl:copy>
				<xsl:apply-templates select="./node()" mode="slashdoc" />
			</xsl:copy>
	</xsl:template>
</xsl:stylesheet>
