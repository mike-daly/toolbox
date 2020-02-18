<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:v="urn:schemas-microsoft-com:vml">
	<xsl:output method="xml" encoding="utf-8"/>
	<xsl:param name="ShowSyncTree">true</xsl:param>
	<xsl:template match="/root/tree"/>
	<xsl:template match="/root/pages">
		<root>
			<xsl:for-each select="page">
				<page>
					<xsl:attribute name="name"><xsl:value-of select="@id"/>.htm</xsl:attribute>
					<xsl:attribute name="title"><xsl:value-of select="@title"/></xsl:attribute>
					<html xmlns:v="urn:schemas-microsoft-com:vml">
						<head>
							<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
							<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
							<link rel="stylesheet">
								<xsl:attribute name="href"><xsl:value-of select="/root/link/@href"/></xsl:attribute>
							</link>
						</head>
						<body id="bodyID" class="dtBODY">
							<xsl:if test="$ShowSyncTree='true'">
								<a href="javascript:void(0)">
									<xsl:attribute name="onclick">parent.frames.left.SynchronizeTree('<xsl:value-of select="@id"/>')</xsl:attribute>Sync tree</a>
								<br/>
								<br/>
							</xsl:if>
							<div id="nsbanner">
								<div id="bannerrow1">
									<table class="bannerparthead" cellspacing="0" id="Table1">
										<tr id="hdr">
											<td class="runninghead">
												<xsl:choose>
													<xsl:when test="string-length(@subtitle)!=0">
														<xsl:value-of select="@subtitle"/>
													</xsl:when>
													<xsl:otherwise>Database Reference</xsl:otherwise>
												</xsl:choose>
											</td>
											<td class="product"/>
										</tr>
									</table>
								</div>
								<div id="TitleRow">
									<h1 class="dtH1">
										<xsl:value-of select="@title"/>
									</h1>
								</div>
							</div>
							<div id="nstext">
								<xsl:for-each select="*">
									<xsl:choose>
										<xsl:when test="name()='item'">
											<xsl:call-template name="DrawItem"/>
										</xsl:when>
										<xsl:when test="name()='keyword'">
											<xsl:call-template name="DrawKeyword"/>
										</xsl:when>
									</xsl:choose>
								</xsl:for-each>
								<p/>
							</div>
						</body>
					</html>
				</page>
			</xsl:for-each>
		</root>
	</xsl:template>
	<xsl:template name="DrawKeyword">
		<a>
			<xsl:attribute name="name"><xsl:value-of select="@anchor"/></xsl:attribute>
		</a>
	</xsl:template>
	<xsl:template name="DrawItem">
		<xsl:if test="@class='subtitle'">
			<a>
				<xsl:attribute name="name"><xsl:value-of select="."/></xsl:attribute>
			</a>
		</xsl:if>
		<div>
			<xsl:attribute name="class"><xsl:value-of select="@class"/></xsl:attribute>
			<xsl:copy-of select="./* | ./text()"/>
		</div>
		<br/>
	</xsl:template>
</xsl:stylesheet>
