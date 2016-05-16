<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:MSHelp="http://msdn.microsoft.com/mshelp" >
	
	<!-- html help 2 data island template -->
	<xsl:template match="node()" mode="xml-data-island" priority="1">
		<MSHelp:Attr Name="TargetOS" Value="Windows"/>
	</xsl:template>
	
	<!-- header template -->
	<xsl:template match="ndoc" mode="header-section">
		<style type="text/css">
			.green
			{
				color:green;
			}
		</style>
	</xsl:template>
	
	<!-- section tag templates -->
	<xsl:template match="custom" mode="summary-section">
		<h1 class="green">
			<xsl:value-of select="."/>
		</h1>
	</xsl:template>
	<xsl:template match="mySeeAlso" mode="seealso-section">
		<i class="green">
			<xsl:value-of select="."/>
		</i>
	</xsl:template>	
	
	<!-- inline tag templates -->
	<xsl:template match="null" mode="slashdoc">
		<xsl:text> null reference (Nothing in Visual Basic) </xsl:text>
	</xsl:template>
	<xsl:template match="static" mode="slashdoc">
		<xsl:text> static (Shared in Visual Basic) </xsl:text>
	</xsl:template>
	<xsl:template match="true" mode="slashdoc">
		<b>true</b>
	</xsl:template>
	<xsl:template match="false" mode="slashdoc">
		<b>false</b>
	</xsl:template>
	
	<!-- 
		Because you want only one table per documentation section
		this match pattern will only select the first history node
		for a given item.
		It then applies a template to the "documentation" parent that creates
		the table and then transforms all of the history nodes in a custom fashion
	-->  
	<xsl:template match="history[1]" mode="after-remarks-section">
		<xsl:apply-templates select="parent::documentation" mode="internal"/>
	</xsl:template>
	
	<xsl:template match="documentation[history]" mode="internal">
		<h4 class="dtH4">History Section</h4>
		<div class="tablediv">
			<table class="dtTABLE" cellspacing="0">
				<tr valign="top">
					<th width="10%">Date</th>
					<th width="10%">User</th>
					<th width="10%">SCR</th>
					<th width="70%">Detail</th>
				</tr>
				<xsl:apply-templates select="history" mode="internal"/>
			</table>
		</div>
	</xsl:template>
	
	<xsl:template match="history" mode="internal">
		<tr>
			<td width="10%">
				<xsl:value-of select="date"/>
			</td>
			<td width="10%">
				<xsl:value-of select="user"/>
			</td>
			<td width="10%">
				<xsl:value-of select="scr"/>
			</td>
			<td width="70%">
				<xsl:value-of select="text()"/>
			</td>
		</tr>
	</xsl:template>	
	<!-- -->
</xsl:stylesheet>

  