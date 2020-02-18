<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output indent="yes" method="html" omit-xml-declaration="yes"/>
	<xsl:param name="DrawTreeOnly"/>
	<xsl:template match="/root/tree">
		<xsl:choose>
			<xsl:when test="$DrawTreeOnly='true'">
				<xsl:call-template name="DrawTreeOnly"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="DrawAll"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!--
	
	
	just draw the tree
	-->
	<xsl:template name="DrawTreeOnly">
		<html>
			<head>
				<script src="help.js"/>
				<xsl:for-each select="/root/link[@rel='stylesheet']">
					<link rel="stylesheet">
						<xsl:attribute name="href"><xsl:value-of select="@href"/></xsl:attribute>
					</link>
				</xsl:for-each>
			</head>
			<body>
				<table>
					<tr>
						<td nowrap="1">
							<span class="menuItem" onmouseover="event.srcElement.style.color='blue'" onmouseout="event.srcElement.style.color='black'" onclick="CollapseAll()">Collapse all</span>
						</td>
					</tr>
					<tr>
						<td nowrap="1">
							<span class="menuItem" onmouseover="event.srcElement.style.color='blue'" onmouseout="event.srcElement.style.color='black'" onclick="ExpandAll()">Expand all</span>
						</td>
					</tr>
				</table>
				<hr/>
				<xsl:for-each select="page">
					<xsl:call-template name="DrawNode">
						<xsl:with-param name="IsVisible">true</xsl:with-param>
					</xsl:call-template>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
	<!--
	
	
	draw it all!
	-->
	<xsl:template name="DrawAll">
		<html>
			<head>
				<script src="help.js"/>
				<xsl:for-each select="/root/link[@rel='stylesheet']">
					<link rel="stylesheet">
						<xsl:attribute name="href"><xsl:value-of select="@href"/></xsl:attribute>
					</link>
				</xsl:for-each>
			</head>
			<body onload="helpTab[0].click();" style="background-color:menu;">
				<div id="helpTab" class="tab" onclick="tabClick(0);">Contents</div>
				<div id="helpTab" class="tab" onclick="tabClick(1);" style="left:61;background-color:transparent;">Index</div>
				<div id="helpTab" class="tab" onclick="tabClick(2);" style="left:122;background-color:transparent;">Search</div>
				<div style="position:absolute;width:100%;z-index:1;height:2;top:23;left:0;overflow:hidden;border:2 inset;"/>
				<div id="tabContent" style="position:absolute;top:30;display:none;">
					<table>
						<tr>
							<td nowrap="1">
								<span class="menuItem" onmouseover="event.srcElement.style.color='blue'" onmouseout="event.srcElement.style.color='black'" onclick="CollapseAll()">Collapse help topics</span>
							</td>
						</tr>
						<tr>
							<td nowrap="1">
								<span class="menuItem" onmouseover="event.srcElement.style.color='blue'" onmouseout="event.srcElement.style.color='black'" onclick="ExpandAll()">Expand help topics</span>
							</td>
						</tr>
					</table>
					<hr/>
					<xsl:for-each select="page">
						<xsl:call-template name="DrawNode">
							<xsl:with-param name="IsVisible">true</xsl:with-param>
						</xsl:call-template>
					</xsl:for-each>
				</div>
				<div id="tabContent" style="position:absolute;top:30;display:none;">
					<!--<table cellpadding="0" cellspacing="0">-->
					<xsl:for-each select="//keyword | //item[@class='subtitle']">
						<xsl:sort select="." order="ascending" data-type="text"/>
						<xsl:variable name="PageID">
							<xsl:value-of select="../@id"/>
						</xsl:variable>
						<xsl:variable name="anchor">
							<xsl:choose>
								<xsl:when test="name()='keyword'">
									<xsl:value-of select="@anchor"/>
								</xsl:when>
								<xsl:otherwise>#<xsl:value-of select="."/>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:variable>
						<span class="menuItem" onmouseover="event.srcElement.style.color='blue'" onmouseout="event.srcElement.style.color='black'">
							<xsl:attribute name="onclick">parent.frames.right.navigate('<xsl:value-of select="$PageID"/>.htm<xsl:value-of select="$anchor"/>');</xsl:attribute>
							<xsl:value-of select="."/>
						</span>
						<br/>
						<!--
							<tr>
								<td nowrap="1">
									<span class="menuItem" onmouseover="event.srcElement.style.color='blue'" onmouseout="event.srcElement.style.color='black'">
										<xsl:attribute name="onclick">parent.frames.right.navigate('<xsl:value-of select="$PageID"/>.htm<xsl:value-of select="$anchor"/>');</xsl:attribute>
										<xsl:value-of select="."/>
									</span>
								</td>
							</tr>
							-->
					</xsl:for-each>
					<!--</table>-->
				</div>
				<div id="tabContent" style="position:absolute;top:30;display:none;">
					<table>
						<tr>
							<td>
								<input type="text" id="searchFor"/>
							</td>
						</tr>
						<tr>
							<td>
								<input type="button" value="search" id="btnSearch" onclick="SearchFor(document.all.searchFor.value)"/>
							</td>
						</tr>
					</table>
					<div id="searchResults"/>
				</div>
			</body>
			<xml id="xmlIndex" src="indexText.xml"/>
		</html>
	</xsl:template>
	<!--
	
	
	
	this one draws a node in the tree
	-->
	<xsl:template name="DrawNode">
		<xsl:param name="Indent"/>
		<xsl:param name="IsVisible"/>
		<xsl:variable name="PageID">
			<xsl:value-of select="@id"/>
		</xsl:variable>
		<xsl:variable name="ParentID">
			<xsl:value-of select="../@id"/>
		</xsl:variable>
		<xsl:variable name="HasChildren">
			<xsl:choose>
				<xsl:when test="count(./page)!=0">true</xsl:when>
				<xsl:otherwise>false</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="Display">
			<xsl:choose>
				<xsl:when test="string-length(/root/pages/page[@id=$PageID]/@title)!=0">
					<xsl:value-of select="/root/pages/page[@id=$PageID]/@title"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$PageID"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<span>
			<xsl:if test="string-length($ParentID)!=0">
				<xsl:attribute name="parentID"><xsl:value-of select="$ParentID"/></xsl:attribute>
			</xsl:if>
			<xsl:attribute name="id"><xsl:value-of select="$PageID"/></xsl:attribute>
			<xsl:attribute name="style"><xsl:choose><xsl:when test="$IsVisible='true'">display:block</xsl:when><xsl:otherwise>display:none</xsl:otherwise></xsl:choose></xsl:attribute>
			<table border="0" cellpadding="1" cellspacing="0">
				<tr>
					<xsl:value-of select="$Indent" disable-output-escaping="yes"/>
					<xsl:choose>
						<xsl:when test="$HasChildren='true'">
							<td width="15" nowrap="1">
								<image src="images/collapsed.gif" style="cursor:hand">
									<xsl:attribute name="onclick">ToggleShowChildren('<xsl:value-of select="$PageID"/>');</xsl:attribute>
									<xsl:attribute name="PageID"><xsl:value-of select="$PageID"/></xsl:attribute>
								</image>
							</td>
							<td width="15" nowrap="1">
								<img style="cursor:hand">
									<xsl:attribute name="src"><xsl:choose><xsl:when test="string-length(@img)!=0"><xsl:value-of select="@img"/></xsl:when><xsl:otherwise>images/cdir.gif</xsl:otherwise></xsl:choose></xsl:attribute>
									<xsl:attribute name="onclick">ToggleShowChildren('<xsl:value-of select="$PageID"/>');</xsl:attribute>
									<xsl:attribute name="PageID"><xsl:value-of select="$PageID"/></xsl:attribute>
								</img>
							</td>
						</xsl:when>
						<xsl:otherwise>
							<td width="15" nowrap="1">
								<div style="width:15;">
									<xsl:value-of select="' '"/>
								</div>
							</td>
							<td width="15" nowrap="1">
								<img style="cursor:hand">
									<xsl:attribute name="src"><xsl:choose><xsl:when test="string-length(@img)!=0"><xsl:value-of select="@img"/></xsl:when><xsl:otherwise>images/file.gif</xsl:otherwise></xsl:choose></xsl:attribute>
									<xsl:choose>
										<xsl:when test="$DrawTreeOnly='true' and string-length(@onclick)!=0">
											<xsl:attribute name="onclick"><xsl:value-of select="@onclick"/></xsl:attribute>
										</xsl:when>
										<xsl:when test="$DrawTreeOnly='true'">
											<!-- do nothing -->
										</xsl:when>
										<xsl:otherwise>
											<xsl:attribute name="onclick">parent.frames.right.navigate('<xsl:value-of select="$PageID"/>.htm');</xsl:attribute>
										</xsl:otherwise>
									</xsl:choose>
								</img>
							</td>
						</xsl:otherwise>
					</xsl:choose>
					<td nowrap="1">
						<xsl:value-of select="'&amp;nbsp;'" disable-output-escaping="yes"/>
						<span class="menuItem" onmouseover="event.srcElement.style.color='blue'" onmouseout="event.srcElement.style.color='black'">
							<xsl:choose>
								<xsl:when test="$DrawTreeOnly='true' and string-length(@onclick)!=0">
									<xsl:attribute name="onclick"><xsl:value-of select="@onclick"/></xsl:attribute>
								</xsl:when>
								<xsl:when test="$DrawTreeOnly='true'">
									<!-- do nothing -->
								</xsl:when>
								<xsl:otherwise>
									<xsl:attribute name="onclick">parent.frames.right.navigate('<xsl:value-of select="$PageID"/>.htm');</xsl:attribute>
								</xsl:otherwise>
							</xsl:choose>
							<xsl:value-of select="$Display"/>
						</span>
					</td>
				</tr>
			</table>
			<xsl:for-each select="page">
				<xsl:call-template name="DrawNode">
					<xsl:with-param name="Indent">
						<xsl:value-of select="$Indent"/>&lt;td width="15" nowrap="1"&gt;&lt;div style="width:15;"&gt; &lt;/div&gt;&lt;/td&gt;</xsl:with-param>
				</xsl:call-template>
			</xsl:for-each>
		</span>
	</xsl:template>
	<xsl:template match="/root/pages"/>
</xsl:stylesheet>
