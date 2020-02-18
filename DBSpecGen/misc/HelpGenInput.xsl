<xsl:stylesheet version="1.0" xmlns:v="urn:schemas-microsoft-com:vml" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:db="urn:schemas-microsoft-com:dbspecgen" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="db msxsl">
	<xsl:output omit-xml-declaration="yes" method="xml" indent="yes" encoding="utf-8"/>
	<msxsl:script language="JScript" implements-prefix="db">
	
// some lame javascript to get around the fact 
// that all variables are immutable in xslt.

var x;
var y;
var xFocus;
var yFocus;
var zIndexCounter = -100;

function SetValue(sVarName, iVal)
{
	//eval(sVarName + '=' + iVal);
	if(sVarName == 'x') x = iVal;
	else if(sVarName == 'y') y = iVal;
	else if(sVarName == 'xFocus') xFocus = iVal;
	else if(sVarName == 'yFocus') yFocus = iVal;
	else if(sVarName == 'zIndexCounter') zIndexCounter = iVal;
	return "";
}

function GetValue(sVarName)
{
	var retVal = eval(sVarName);
	return retVal;
}

</msxsl:script>
	<!--
	some global variables, passed in from whoever 
	is calling this xslt, with default values
	-->
	<xsl:variable name="ShowAllExtendedProperties">1</xsl:variable>
	<xsl:variable name="CssFile">help.css</xsl:variable>
	<xsl:variable name="IsVML">1</xsl:variable>
	<xsl:variable name="TableColor">blue</xsl:variable>
	<xsl:variable name="ViewColor">green</xsl:variable>
	<xsl:variable name="SprocColor">red</xsl:variable>
	<xsl:variable name="CheckColor">purple</xsl:variable>
	<xsl:variable name="UdfColor">black</xsl:variable>
	<xsl:variable name="TriggerColor">gray</xsl:variable>
	<xsl:variable name="ItemsPerRow">6</xsl:variable>
	<xsl:variable name="MaxLabelLength">12</xsl:variable>
	<xsl:variable name="ShowDbNameOnPageTitles">1</xsl:variable>
	<xsl:variable name="UseMbAsSizeUnit">0</xsl:variable>
	<xsl:variable name="DrawPieCharts">1</xsl:variable>
	<xsl:variable name="ImageMaps"/>
	<!--
	templates
	-->
	<xsl:template match="/database">
		<xsl:variable name="dbName">
			<xsl:value-of select="@name"/>
		</xsl:variable>
		<xsl:variable name="serverName">
			<xsl:value-of select="@server"/>
		</xsl:variable>
		<root>
			<!--
			this is the stylesheet used for the tree, and all the generated help pages.
			-->
			<link rel="stylesheet">
				<xsl:attribute name="href"><xsl:value-of select="$CssFile"/></xsl:attribute>
			</link>
			<xsl:call-template name="drawPages">
				<xsl:with-param name="dbName" select="$dbName"/>
				<xsl:with-param name="serverName" select="$serverName"/>
			</xsl:call-template>
			<xsl:call-template name="drawTree">
				<xsl:with-param name="dbName" select="$dbName"/>
				<xsl:with-param name="serverName" select="$serverName"/>
			</xsl:call-template>
		</root>
	</xsl:template>
	<!--
	
	
	
	
	draw a custom object
	-->
	<xsl:template name="drawCustomObject">
		<page>
			<xsl:attribute name="id"><xsl:value-of select="translate(string(@xtype),'. ','')"/>_<xsl:value-of select="translate(string(@name),'. ','')"/></xsl:attribute>
			<xsl:attribute name="title"><xsl:value-of select="@name"/></xsl:attribute>
			<xsl:attribute name="subtitle">Database Reference</xsl:attribute>
			<item>
				<xsl:for-each select="* | text()">
					<xsl:copy-of select="."/>
				</xsl:for-each>
				<!--
				show the dependencies after all the html that they want to display
				-->
				<xsl:choose>
					<xsl:when test="$IsVML = 1">
						<xsl:call-template name="DrawDependencyGraph"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="DrawDependencies"/>
					</xsl:otherwise>
				</xsl:choose>
			</item>
		</page>
	</xsl:template>
	<!--
	
	draw all the pages 
	-->
	<xsl:template name="drawPages">
		<xsl:param name="serverName"/>
		<xsl:param name="dbName"/>
		<pages>
			<!-- 
            main page for the whole db
            -->
			<xsl:if test="/database/table or /database/procedure or /database/userDefinedType">
				<page id="default">
					<xsl:attribute name="title"><xsl:value-of select="$serverName"/>.<xsl:value-of select="$dbName"/></xsl:attribute>
					<xsl:attribute name="subtitle">Database Reference - <xsl:value-of select="/database/@name"/></xsl:attribute>
					<item>
						<h2 class="dtH2">Documentation</h2>
						<p>Documentation for <xsl:value-of select="$serverName"/>.<xsl:value-of select="$dbName"/>
					 generated on: <xsl:value-of select="translate(/database/@currentTime,'T',' ')"/>
						</p>
						<h2 class="dtH2">Database objects</h2>
						<table class="dtTABLE" cellspacing="0" style="width: 94%">
							<thead>
								<tr valign="top">
									<th>type</th>
									<th>count</th>
								</tr>
							</thead>
							<tbody>
								<tr valign="top">
									<td>
										<xsl:choose>
											<xsl:when test="count(table[(@xtype='U') and not(@name = /database/DBSpecGen/exclude/server[@name=$serverName]/database[@name=$dbName]/table/@name)])">
												<a href="cntTables.htm">tables</a>
											</xsl:when>
											<xsl:otherwise>tables</xsl:otherwise>
										</xsl:choose>
									</td>
									<td>
										<xsl:value-of select="count(/database/table[(@xtype='U')])"/>
									</td>
								</tr>
								<tr valign="top">
									<td>
										<xsl:choose>
											<xsl:when test="count(table[(@xtype='V') and not(@name = /database/DBSpecGen/exclude/server[@name=$serverName]/database[@name=$dbName]/view/@name)])">
												<a href="cntViews.htm">views</a>
											</xsl:when>
											<xsl:otherwise>views</xsl:otherwise>
										</xsl:choose>
									</td>
									<td>
										<xsl:value-of select="count(/database/table[(@xtype='V')])"/>
									</td>
								</tr>
								<tr valign="top">
									<td>
										<xsl:choose>
											<xsl:when test="count(procedure[(@xtype='P') and not(@name = /database/DBSpecGen/exclude/server[@name=$serverName]/database[@name=$dbName]/sproc/@name)]) != 0">
												<a href="cntSprocs.htm">stored procedures</a>
											</xsl:when>
											<xsl:otherwise>stored procedures</xsl:otherwise>
										</xsl:choose>
									</td>
									<td>
										<xsl:value-of select="count(/database/procedure[(@xtype='P')])"/>
									</td>
								</tr>
								<tr valign="top">
									<td>
										<xsl:choose>
											<xsl:when test="count(procedure[(@xtype='FN' or @xtype='IF' or @xtype='TF') and not(@name = /database/DBSpecGen/exclude/server[@name=$serverName]/database[@name=$dbName]/udf/@name)])">
												<a href="cntUDFs.htm">user defined functions</a>
											</xsl:when>
											<xsl:otherwise>user defined functions</xsl:otherwise>
										</xsl:choose>
									</td>
									<td>
										<xsl:value-of select="count(/database/procedure[@xtype='FN' or @xtype='IF' or @xtype='TF'])"/>
									</td>
								</tr>
								<tr valign="top">
									<td>
										<xsl:choose>
											<xsl:when test="count(/database/userDefinedType) != 0">
												<a href="cntUDTs.htm">user defined types</a>
											</xsl:when>
											<xsl:otherwise>user defined types</xsl:otherwise>
										</xsl:choose>
									</td>
									<td>
										<xsl:value-of select="count(/database/userDefinedType)"/>
									</td>
								</tr>
							</tbody>
						</table>
						<xsl:if test="/database/extendedProperty[not(@objType) and not(@objName) and not(@parentObj)]">
							<h2 class="dtH2">Extended properties</h2>
							<table class="dtTABLE" cellspacing="0" style="width: 94%">
								<thead>
									<tr valign="top">
										<th>name</th>
										<th>value</th>
									</tr>
								</thead>
								<tbody>
									<xsl:for-each select="/database/extendedProperty[not(@objType) and not(objName) and not(parentObj)]">
										<tr valign="top">
											<td>
												<xsl:value-of select="@propName"/>
											</td>
											<td>
												<xsl:value-of select="@value"/>
											</td>
										</tr>
									</xsl:for-each>
								</tbody>
							</table>
						</xsl:if>
						<h2 class="dtH2">Database users</h2>
						<table class="dtTABLE" cellspacing="0" style="width: 94%">
							<thead>
								<tr valign="top">
									<th>username</th>
									<th>group</th>
								</tr>
							</thead>
							<tbody>
								<xsl:for-each select="/database/user">
									<tr valign="top">
										<td>
											<xsl:value-of select="@username"/>
										</td>
										<td>
											<xsl:value-of select="@group"/>
										</td>
									</tr>
								</xsl:for-each>
							</tbody>
						</table>
						<xsl:if test="count(/database/permission[string-length(@object)=0 and string-length(@owner)=0])!=0">
							<h2 class="dtH2">Global permissions</h2>
							<table class="dtTABLE" cellspacing="0" style="width: 94%">
								<thead>
									<tr valign="top">
										<th>grantor</th>
										<th>grantee</th>
										<th>allow</th>
										<th>action</th>
									</tr>
								</thead>
								<tbody>
									<xsl:for-each select="/database/permission[string-length(@object)=0 and string-length(@owner)=0]">
										<tr valign="top">
											<td>
												<xsl:value-of select="@grantor"/>
											</td>
											<td>
												<xsl:value-of select="@grantee"/>
											</td>
											<td>
												<xsl:choose>
													<xsl:when test="@protecttype='deny'">
														<font color="red">
															<xsl:value-of select="@protecttype"/>
														</font>
													</xsl:when>
													<xsl:otherwise>
														<font color="green">
															<xsl:value-of select="@protecttype"/>
														</font>
													</xsl:otherwise>
												</xsl:choose>
											</td>
											<td>
												<xsl:value-of select="@action"/>
											</td>
										</tr>
									</xsl:for-each>
								</tbody>
							</table>
						</xsl:if>
						<h2 class="dtH2">Database files</h2>
						<table class="dtTABLE" cellspacing="0" style="width: 94%">
							<thead>
								<tr valign="top">
									<th>database</th>
									<th>file</th>
									<th>file group</th>
									<th>size</th>
									<th>max size</th>
									<th>growth</th>
									<th>usage</th>
								</tr>
							</thead>
							<tbody>
								<xsl:for-each select="/database/file">
									<tr valign="top">
										<td>
											<xsl:value-of select="@dbname"/>
										</td>
										<td>
											<xsl:value-of select="@filename"/>
										</td>
										<td>
											<xsl:choose>
												<xsl:when test="@filegroup">
													<xsl:value-of select="@filegroup"/>
												</xsl:when>
												<xsl:otherwise>&#160;</xsl:otherwise>
											</xsl:choose>
										</td>
										<td>
											<xsl:choose>
												<xsl:when test="$UseMbAsSizeUnit=1">
													<xsl:value-of select="format-number(@sizeKB div 1000,'###,###,###,##0.###')"/> mb
												</xsl:when>
												<xsl:otherwise>
													<xsl:value-of select="format-number(@sizeKB,'###,###,###,##0')"/> kb
												</xsl:otherwise>
											</xsl:choose>
										</td>
										<td>
											<xsl:value-of select="@maxsize"/>
										</td>
										<td>
											<xsl:value-of select="@growth"/>
										</td>
										<td>
											<xsl:value-of select="@usage"/>
										</td>
									</tr>
								</xsl:for-each>
							</tbody>
						</table>
						<h2 class="dtH2">Database properties</h2>
						<table class="dtTABLE" cellspacing="0" style="width: 94%">
							<thead>
								<tr valign="top">
									<th>property</th>
									<th>value</th>
								</tr>
							</thead>
							<tbody>
								<tr valign="top">
									<td>name</td>
									<td>
										<xsl:value-of select="/database/@name"/>
									</td>
								</tr>
								<tr valign="top">
									<td>server</td>
									<td>
										<xsl:value-of select="/database/@server"/>
									</td>
								</tr>
								<tr valign="top">
									<td>size</td>
									<td>
										<xsl:value-of select="/database/@sizeMB"/> mb
							                </td>
								</tr>
								<tr valign="top">
									<td>owner</td>
									<td>
										<xsl:value-of select="/database/@owner"/>
									</td>
								</tr>
								<tr valign="top">
									<td>created</td>
									<td>
										<xsl:value-of select="/database/@created"/>
									</td>
								</tr>
								<tr valign="top">
									<td>status</td>
									<td>
										<xsl:value-of select="/database/@status"/>
									</td>
								</tr>
								<tr valign="top">
									<td>compat level</td>
									<td>
										<xsl:value-of select="/database/@compatLevel"/>
									</td>
								</tr>
								<tr valign="top">
									<td>updateability</td>
									<td>
										<xsl:value-of select="/database/@updateability"/>
									</td>
								</tr>
								<tr valign="top">
									<td>user access</td>
									<td>
										<xsl:value-of select="/database/@userAccess"/>
									</td>
								</tr>
								<tr valign="top">
									<td>recovery</td>
									<td>
										<xsl:value-of select="/database/@recovery"/>
									</td>
								</tr>
								<tr valign="top">
									<td>version</td>
									<td>
										<xsl:value-of select="/database/@version"/>
									</td>
								</tr>
								<tr valign="top">
									<td>collation</td>
									<td>
										<xsl:value-of select="/database/@collation"/>
									</td>
								</tr>
								<tr valign="top">
									<td>SQL sort order</td>
									<td>
										<xsl:value-of select="/database/@sqlSortOrder"/>
									</td>
								</tr>
								<tr valign="top">
									<td>auto close</td>
									<td>
										<xsl:choose>
											<xsl:when test="/database/@isAutoClose=1">yes</xsl:when>
											<xsl:otherwise>no</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
								<tr valign="top">
									<td>auto shrink</td>
									<td>
										<xsl:choose>
											<xsl:when test="/database/@isAutoShrink=1">yes</xsl:when>
											<xsl:otherwise>no</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
								<tr valign="top">
									<td>in standby</td>
									<td>
										<xsl:choose>
											<xsl:when test="/database/@isInStandby=1">yes</xsl:when>
											<xsl:otherwise>no</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
								<tr valign="top">
									<td>torn page detection</td>
									<td>
										<xsl:choose>
											<xsl:when test="/database/@isTornPageDetectionEnabled=1">yes</xsl:when>
											<xsl:otherwise>no</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
								<tr valign="top">
									<td>ansi null default</td>
									<td>
										<xsl:choose>
											<xsl:when test="/database/@isAnsiNullDefault=1">yes</xsl:when>
											<xsl:otherwise>no</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
								<tr valign="top">
									<td>ansi nulls</td>
									<td>
										<xsl:choose>
											<xsl:when test="/database/@isAnsiNullsEnabled=1">yes</xsl:when>
											<xsl:otherwise>no</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
								<tr valign="top">
									<td>ansi padding</td>
									<td>
										<xsl:choose>
											<xsl:when test="/database/@isAnsiPaddingEnabled=1">yes</xsl:when>
											<xsl:otherwise>no</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
								<tr valign="top">
									<td>ansi warnings</td>
									<td>
										<xsl:choose>
											<xsl:when test="/database/@isAnsiWarningsEnabled=1">yes</xsl:when>
											<xsl:otherwise>no</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
								<tr valign="top">
									<td>arithmetic abort</td>
									<td>
										<xsl:choose>
											<xsl:when test="/database/@isArithmeticAbortEnabled=1">yes</xsl:when>
											<xsl:otherwise>no</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
								<tr valign="top">
									<td>auto create statistics</td>
									<td>
										<xsl:choose>
											<xsl:when test="/database/@isAutoCreateStatistics=1">yes</xsl:when>
											<xsl:otherwise>no</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
								<tr valign="top">
									<td>auto update statistics</td>
									<td>
										<xsl:choose>
											<xsl:when test="/database/@isAutoUpdateStatistics=1">yes</xsl:when>
											<xsl:otherwise>no</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
								<tr valign="top">
									<td>close cursors on commit</td>
									<td>
										<xsl:choose>
											<xsl:when test="/database/@isCloseCursorsOnCommitEnabled=1">yes</xsl:when>
											<xsl:otherwise>no</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
								<tr valign="top">
									<td>full text</td>
									<td>
										<xsl:choose>
											<xsl:when test="/database/@isFullTextEnabled=1">yes</xsl:when>
											<xsl:otherwise>no</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
								<tr valign="top">
									<td>local cursors default</td>
									<td>
										<xsl:choose>
											<xsl:when test="/database/@isLocalCursorsDefault=1">yes</xsl:when>
											<xsl:otherwise>no</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
								<tr valign="top">
									<td>null concat</td>
									<td>
										<xsl:choose>
											<xsl:when test="/database/@isNullConcat=1">yes</xsl:when>
											<xsl:otherwise>no</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
								<tr valign="top">
									<td>numeric round abort</td>
									<td>
										<xsl:choose>
											<xsl:when test="/database/@isNumericRoundAbortEnabled=1">yes</xsl:when>
											<xsl:otherwise>no</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
								<tr valign="top">
									<td>quoted identifiers</td>
									<td>
										<xsl:choose>
											<xsl:when test="/database/@isQuotedIdentifiersEnabled=1">yes</xsl:when>
											<xsl:otherwise>no</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
								<tr valign="top">
									<td>recursive triggers</td>
									<td>
										<xsl:choose>
											<xsl:when test="/database/@isRecursiveTriggersEnabled=1">yes</xsl:when>
											<xsl:otherwise>no</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
								<tr valign="top">
									<td>published</td>
									<td>
										<xsl:choose>
											<xsl:when test="/database/@isPublished=1">yes</xsl:when>
											<xsl:otherwise>no</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
								<tr valign="top">
									<td>subscribed</td>
									<td>
										<xsl:choose>
											<xsl:when test="/database/@isSubscribed=1">yes</xsl:when>
											<xsl:otherwise>no</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
								<tr valign="top">
									<td>sync with backup</td>
									<td>
										<xsl:choose>
											<xsl:when test="/database/@isSyncWithBackup=1">yes</xsl:when>
											<xsl:otherwise>no</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
							</tbody>
						</table>
						<h2 class="dtH2">Server properties</h2>
						<table class="dtTABLE" cellspacing="0" style="width: 94%">
							<thead>
								<tr valign="top">
									<th>name</th>
									<th>value</th>
								</tr>
							</thead>
							<tbody>
								<xsl:for-each select="/database/serverProperty">
									<tr valign="top">
										<td>
											<xsl:value-of select="@name"/>
										</td>
										<td>
											<xsl:value-of select="@runValue"/>
										</td>
									</tr>
								</xsl:for-each>
							</tbody>
						</table>
					</item>
				</page>
			</xsl:if>
			<!--
			custom objects
			-->
			<xsl:if test="object">
				<xsl:for-each select="/database/object[not(@xtype=preceding-sibling::object/@xtype)]">
					<xsl:variable name="xtype" select="@xtype"/>
					<page>
						<xsl:attribute name="id">cnt<xsl:value-of select="translate(@xtype,'. ','')"/></xsl:attribute>
						<xsl:attribute name="title"><xsl:value-of select="/database/DBSpecGen/definitions/object[@xtype=$xtype]/@plural"/></xsl:attribute>
						<xsl:attribute name="subtitle">Database Reference</xsl:attribute>
						<item>
							<h2 class="dtH2">
								<xsl:value-of select="/database/DBSpecGen/definitions/object[@xtype=$xtype]/@plural"/> overview</h2>
							<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
								<thead>
									<tr valign="top">
										<th>name</th>
										<th>dependents</th>
										<th>depends on</th>
									</tr>
								</thead>
								<tbody>
									<xsl:for-each select="/database/object[@xtype=$xtype]">
										<xsl:sort select="@name"/>
										<xsl:variable name="objName">
											<xsl:value-of select="@name"/>
										</xsl:variable>
										<tr valign="top">
											<td>
												<a>
													<xsl:attribute name="href"><xsl:value-of select="translate(@xtype,'. ','')"/>_<xsl:value-of select="translate(@name,'. ','')"/>.htm</xsl:attribute>
													<xsl:value-of select="@name"/>
												</a>
											</td>
											<td align="right">
												<xsl:value-of select="count(/database/dependency[@objName=$objName and @xtype=$xtype and @dependsOnObj])"/>
											</td>
											<td align="right">
												<xsl:value-of select="count(/database/dependency[@refXtype=$xtype and @dependsOnObj=$objName])"/>
											</td>
										</tr>
									</xsl:for-each>
								</tbody>
							</table>
						</item>
					</page>
				</xsl:for-each>
				<xsl:for-each select="object">
					<xsl:call-template name="drawCustomObject"/>
				</xsl:for-each>
			</xsl:if>
			<!--
    			tables
    			-->
			<xsl:if test="
            (count(table[(@xtype='U') 
             and not(@name = /database/DBSpecGen/exclude/server[@name=$serverName]/database[@name=$dbName]/table/@name)]) != 0)
             or
            (count(table[(@xtype='V') 
             and not(@name = /database/DBSpecGen/exclude/server[@name=$serverName]/database[@name=$dbName]/view/@name)]) != 0)
             ">
				<page id="allTableColumns">
					<xsl:attribute name="title"><xsl:value-of select="/database/@name"/> table columns</xsl:attribute>
					<xsl:attribute name="subtitle">Database Reference - <xsl:value-of select="/database/@name"/></xsl:attribute>
					<item>
						<h2 class="dtH2">All table columns</h2>
						<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
							<thead>
								<tr valign="top">
									<th>table</th>
									<th>column</th>
									<th>definition</th>
									<th>comment</th>
								</tr>
							</thead>
							<tbody>
								<xsl:for-each select="table">
									<xsl:sort select="@xtype" data-type="text"/>
									<xsl:sort select="@name" data-type="text"/>
									<xsl:variable name="tableName">
										<xsl:value-of select="@name"/>
									</xsl:variable>
									<xsl:variable name="Excluded">
										<xsl:call-template name="IsExcluded">
											<xsl:with-param name="xtype" select="@xtype"/>
											<xsl:with-param name="ObjName" select="$tableName"/>
										</xsl:call-template>
									</xsl:variable>
									<xsl:if test="$Excluded != '1'">
										<xsl:for-each select="column">
											<xsl:variable name="colName">
												<xsl:value-of select="@name"/>
											</xsl:variable>
											<xsl:variable name="CurrentType">
												<xsl:value-of select="@type"/>
											</xsl:variable>
											<tr valign="top">
												<td>
													<a>
														<xsl:attribute name="href"><xsl:choose><xsl:when test="../@xtype='U'">tbl_</xsl:when><xsl:otherwise>vw_</xsl:otherwise></xsl:choose><xsl:value-of select="translate($tableName,'. ','')"/>.htm</xsl:attribute>
														<xsl:value-of select="$tableName"/>
													</a>
												</td>
												<td>
													<xsl:value-of select="@name"/>
												</td>
												<td>
													<xsl:value-of select="@type"/>
													<xsl:if test="@length">(<xsl:value-of select="@length"/>)</xsl:if>
													<xsl:if test="@allowNull='1'"> NULL</xsl:if>
													<xsl:if test="@defaultValue"> = <xsl:value-of select="@defaultValue"/>
													</xsl:if>
												</td>
												<td>
													<xsl:choose>
														<xsl:when test="string-length(/database/extendedProperty[@objType='COLUMN' and @parentObj=$tableName and @objName=$colName and @propName='MS_Description']/@value)!=0">
															<xsl:value-of select="/database/extendedProperty[@objType='COLUMN' and @parentObj=$tableName and @objName=$colName and @propName='MS_Description']/@value"/>
														</xsl:when>
														<xsl:otherwise>&#160;</xsl:otherwise>
													</xsl:choose>
												</td>
											</tr>
										</xsl:for-each>
									</xsl:if>
								</xsl:for-each>
							</tbody>
						</table>
					</item>
				</page>
				<page id="cntTables">
					<xsl:attribute name="title"><xsl:value-of select="/database/@name"/> tables</xsl:attribute>
					<xsl:attribute name="subtitle">Database Reference - <xsl:value-of select="/database/@name"/></xsl:attribute>
					<item>
						<h2 class="dtH2">Table overview</h2>
						<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
							<thead>
								<tr valign="top">
									<th>name</th>
									<th>created</th>
									<th>columns</th>
									<th>rows</th>
									<th>indexes</th>
									<th>triggers</th>
									<th>constraints</th>
									<th>ref to</th>
									<th>ref from</th>
									<th>dependents</th>
									<th>row size</th>
									<th>data</th>
									<th>indexes</th>
									<th>comment</th>
								</tr>
							</thead>
							<tbody>
								<xsl:for-each select="table[(@xtype='U')]">
									<xsl:sort select="@name" data-type="text"/>
									<xsl:variable name="tableName">
										<xsl:value-of select="@name"/>
									</xsl:variable>
									<xsl:variable name="IsExcluded">
										<xsl:call-template name="IsExcluded">
											<xsl:with-param name="xtype" select="@xtype"/>
											<xsl:with-param name="ObjName" select="$tableName"/>
										</xsl:call-template>
									</xsl:variable>
									<xsl:if test="$IsExcluded != '1'">
										<tr valign="top">
											<td>
												<a>
													<xsl:attribute name="href">tbl_<xsl:value-of select="translate(@name,'. ','')"/>.htm</xsl:attribute>
													<xsl:value-of select="@name"/>
												</a>
											</td>
											<td>
												<xsl:value-of select="@created"/>
											</td>
											<td align="right">
												<xsl:value-of select="count(column)"/>
											</td>
											<td align="right">
												<xsl:value-of select="format-number(@rowcount,'###,###,###,##0')"/>
											</td>
											<td align="right">
												<xsl:value-of select="count(index)"/>
											</td>
											<td align="right">
												<xsl:value-of select="count(trigger)"/>
											</td>
											<td align="right">
												<xsl:value-of select="count(constraint)"/>
											</td>
											<td align="right">
												<xsl:value-of select="count(constraint[@isForeignKey=1])"/>
											</td>
											<td align="right">
												<xsl:value-of select="count(/database/table[(@xtype='U') and @name!=$tableName]/constraint[@isForeignKey=1 and @refTable=$tableName])"/>
											</td>
											<td align="right">
												<xsl:value-of select="count(/database/dependency[@objName=$tableName and (@xtype='U') and @dependsOnObj])"/>
											</td>
											<td align="right">
												<xsl:value-of select="format-number(sum(column[@type!='text' and @type!='ntext' and @type!='image']/@bytes),'###,##0')"/> b
											            </td>
											<td align="right">
												<xsl:choose>
													<xsl:when test="$UseMbAsSizeUnit=1">
														<xsl:choose>
															<xsl:when test="@datasizeKB &gt; 0">
																<xsl:value-of select="format-number(@datasizeKB div 1000,'###,###,###,##0.###')"/> mb</xsl:when>
															<xsl:otherwise>0 mb</xsl:otherwise>
														</xsl:choose>
													</xsl:when>
													<xsl:otherwise>
														<xsl:value-of select="format-number(@datasizeKB,'###,###,###,##0')"/> kb
													</xsl:otherwise>
												</xsl:choose>
											</td>
											<td align="right">
												<xsl:choose>
													<xsl:when test="$UseMbAsSizeUnit=1">
														<xsl:choose>
															<xsl:when test="@indexsizeKB &gt; 0">
																<xsl:value-of select="format-number(@indexsizeKB div 1000,'###,###,###,##0.###')"/> mb</xsl:when>
															<xsl:otherwise>0 mb</xsl:otherwise>
														</xsl:choose>
													</xsl:when>
													<xsl:otherwise>
														<xsl:choose>
															<xsl:when test="@indexsizeKB &gt; 0">
																<xsl:value-of select="format-number(@indexsizeKB ,'###,###,###,##0')"/> kb</xsl:when>
															<xsl:otherwise>0 kb</xsl:otherwise>
														</xsl:choose>
													</xsl:otherwise>
												</xsl:choose>
											</td>
											<td>
												<xsl:choose>
													<xsl:when test="/database/extendedProperty[@objType='TABLE' and @propName='MS_Description' and @objName=$tableName]/@value">
														<xsl:value-of select="/database/extendedProperty[@objType='TABLE' and @propName='MS_Description' and @objName=$tableName]/@value"/>
													</xsl:when>
													<xsl:otherwise>&#160;</xsl:otherwise>
												</xsl:choose>
											</td>
										</tr>
									</xsl:if>
								</xsl:for-each>
							</tbody>
						</table>
						<xsl:if test="$DrawPieCharts=1">
							<br/>
							<br/>
							<center>
								<img alt="Largest tables by size on disk.  Click the labels to navigate." border="0" usemap="#Largesttablesbysizeondisk" src="tablesizes.gif"/>
								<img alt="Largest tables by row count.  Click the labels to navigate." border="0" usemap="#Largesttablesbyrowcount" src="tablerows.gif"/>
								<img alt="Largest indexes by size on disk.  Click the labels to navigate." border="0" usemap="#Largestindexesbysizeondisk" src="indexsizes.gif"/>
							</center>
							<xsl:value-of select="$ImageMaps" disable-output-escaping="yes"/>
						</xsl:if>
					</item>
				</page>
				<xsl:for-each select="table[(@xtype='U')]">
					<xsl:sort select="@name" data-type="text"/>
					<xsl:variable name="IsExcluded">
						<xsl:call-template name="IsExcluded">
							<xsl:with-param name="xtype" select="@xtype"/>
							<xsl:with-param name="ObjName" select="@name"/>
						</xsl:call-template>
					</xsl:variable>
					<xsl:if test="$IsExcluded!='1'">
						<xsl:call-template name="drawTable"/>
					</xsl:if>
				</xsl:for-each>
			</xsl:if>
			<!--
			views
			-->
			<xsl:if test="count(table[(@xtype='V') and not(@name = /database/DBSpecGen/exclude/server[@name=$serverName]/database[@name=$dbName]/view/@name)]) != 0">
				<page id="cntViews">
					<xsl:attribute name="title"><xsl:value-of select="/database/@name"/> views</xsl:attribute>
					<xsl:attribute name="subtitle">Database Reference - <xsl:value-of select="/database/@name"/></xsl:attribute>
					<item>
						<h2 class="dtH2">View overview</h2>
						<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
							<thead>
								<tr valign="top">
									<th>name</th>
									<th>created</th>
									<th>columns</th>
									<th>code length</th>
									<th>dependents</th>
									<th>depends on</th>
									<th>row size</th>
									<th>comment</th>
								</tr>
							</thead>
							<tbody>
								<xsl:for-each select="table[(@xtype='V')]">
									<xsl:sort select="@name" data-type="text"/>
									<xsl:variable name="tableName">
										<xsl:value-of select="@name"/>
									</xsl:variable>
									<xsl:variable name="IsExcluded">
										<xsl:call-template name="IsExcluded">
											<xsl:with-param name="xtype" select="@xtype"/>
											<xsl:with-param name="ObjName" select="$tableName"/>
										</xsl:call-template>
									</xsl:variable>
									<xsl:if test="$IsExcluded!='1'">
										<tr valign="top">
											<td>
												<a>
													<xsl:attribute name="href">vw_<xsl:value-of select="translate(@name,'. ','')"/>.htm</xsl:attribute>
													<xsl:value-of select="@name"/>
												</a>
											</td>
											<td>
												<xsl:value-of select="@created"/>
											</td>
											<td align="right">
												<xsl:value-of select="count(column)"/>
											</td>
											<td align="right">
												<xsl:value-of select="format-number(string-length(code),'###,###,###,###,##0')"/>
											</td>
											<td align="right">
												<xsl:value-of select="count(/database/dependency[@objName=$tableName and (@xtype='V') and @dependsOnObj])"/>
											</td>
											<td align="right">
												<xsl:value-of select="count(/database/dependency[(@refXtype='V') and @dependsOnObj=$tableName])"/>
											</td>
											<td align="right">
												<xsl:value-of select="format-number(sum(column[@type!='text' and @type!='ntext' and @type!='image']/@bytes),'###,###,##0')"/> b
											</td>
											<td>
												<xsl:choose>
													<xsl:when test="/database/extendedProperty[@objType='VIEW' and @propName='MS_Description' and @objName=$tableName]/@value">
														<xsl:value-of select="/database/extendedProperty[@objType='VIEW' and @propName='MS_Description' and @objName=$tableName]/@value"/>
													</xsl:when>
													<xsl:otherwise>&#160;</xsl:otherwise>
												</xsl:choose>
											</td>
										</tr>
									</xsl:if>
								</xsl:for-each>
							</tbody>
						</table>
						<xsl:if test="$DrawPieCharts=1">
							<br/>
							<br/>
							<center>
								<img alt="Largest views by code length.  Click the labels to navigate." border="0" usemap="#Largestviewsbycodelength" src="viewcomplexity.gif"/>
							</center>
							<xsl:value-of select="$ImageMaps" disable-output-escaping="yes"/>
						</xsl:if>
					</item>
				</page>
				<xsl:for-each select="table[(@xtype='V')]">
					<xsl:sort select="@name" data-type="text"/>
					<xsl:variable name="IsExcluded">
						<xsl:call-template name="IsExcluded">
							<xsl:with-param name="xtype" select="@xtype"/>
							<xsl:with-param name="ObjName" select="@name"/>
						</xsl:call-template>
					</xsl:variable>
					<xsl:if test="$IsExcluded!='1'">
						<xsl:call-template name="drawTable"/>
					</xsl:if>
				</xsl:for-each>
			</xsl:if>
			<!--
            all procedure parameters
            -->
			<xsl:if test="
            (count(procedure[(@xtype='P') 
             and not(@name = /database/DBSpecGen/exclude/server[@name=$serverName]/database[@name=$dbName]/sproc/@name)]) != 0) 
             or 
            (count(procedure[(@xtype='FN' or @xtype='IF' or @xtype='TF') 
             and not(@name = /database/DBSpecGen/exclude/server[@name=$serverName]/database[@name=$dbName]/udf/@name)]) != 0)">
				<page id="allProdecureParameters">
					<xsl:attribute name="title"><xsl:value-of select="/database/@name"/> procedure parameters</xsl:attribute>
					<xsl:attribute name="subtitle">Database Reference - <xsl:value-of select="/database/@name"/></xsl:attribute>
					<item>
						<h2 class="dtH2">All procedure parameters</h2>
						<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
							<thead>
								<tr valign="top">
									<th>procedure</th>
									<th>type</th>
									<th>parameter</th>
									<th>datatype</th>
									<th>length</th>
									<th>in/out</th>
								</tr>
							</thead>
							<tbody>
								<xsl:for-each select="procedure">
									<xsl:sort select="string-length(@xtype)"/>
									<xsl:sort select="@xtype"/>
									<xsl:sort select="@name" data-type="text"/>
									<xsl:variable name="objName">
										<xsl:value-of select="@name"/>
									</xsl:variable>
									<xsl:variable name="IsExcluded">
										<xsl:call-template name="IsExcluded">
											<xsl:with-param name="xtype" select="@xtype"/>
											<xsl:with-param name="ObjName" select="$objName"/>
										</xsl:call-template>
									</xsl:variable>
									<xsl:if test="$IsExcluded!='1'">
										<xsl:for-each select="param">
											<tr valign="top">
												<td>
													<a>
														<xsl:attribute name="href"><xsl:choose><xsl:when test="../@xtype='P'">sp_</xsl:when><xsl:otherwise>udf_</xsl:otherwise></xsl:choose><xsl:value-of select="translate($objName,'. ','')"/>.htm</xsl:attribute>
														<xsl:value-of select="$objName"/>
													</a>
												</td>
												<td>
													<xsl:value-of select="../@xtype"/>
												</td>
												<td>
													<xsl:choose>
														<xsl:when test="string-length(@name)!=0">
															<xsl:value-of select="@name"/>
														</xsl:when>
														<xsl:otherwise>&#160;</xsl:otherwise>
													</xsl:choose>
												</td>
												<td>
													<xsl:value-of select="@datatype"/>
												</td>
												<td align="right">
													<xsl:choose>
														<xsl:when test="string-length(@length)!=0">
															<xsl:value-of select="@length"/>
														</xsl:when>
														<xsl:otherwise>&#160;</xsl:otherwise>
													</xsl:choose>
												</td>
												<td>
													<xsl:value-of select="@inOut"/>
												</td>
											</tr>
										</xsl:for-each>
									</xsl:if>
								</xsl:for-each>
							</tbody>
						</table>
					</item>
				</page>
			</xsl:if>
			<!--
			sprocs
			-->
			<xsl:if test="count(procedure[(@xtype='P') and not(@name = /database/DBSpecGen/exclude/server[@name=$serverName]/database[@name=$dbName]/sproc/@name)]) != 0">
				<page id="cntSprocs">
					<xsl:attribute name="title"><xsl:value-of select="/database/@name"/> stored procedures</xsl:attribute>
					<xsl:attribute name="subtitle">Database Reference - <xsl:value-of select="/database/@name"/></xsl:attribute>
					<item>
						<h2 class="dtH2">Stored procedure overview</h2>
						<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
							<thead>
								<tr valign="top">
									<th>name</th>
									<th>created</th>
									<th>params</th>
									<th>code length</th>
									<th>dependents</th>
									<th>depends on</th>
									<th>comment</th>
								</tr>
							</thead>
							<tbody>
								<xsl:for-each select="procedure[(@xtype='P')]">
									<xsl:sort select="@name" data-type="text"/>
									<xsl:variable name="objName">
										<xsl:value-of select="@name"/>
									</xsl:variable>
									<xsl:variable name="IsExcluded">
										<xsl:call-template name="IsExcluded">
											<xsl:with-param name="xtype" select="@xtype"/>
											<xsl:with-param name="ObjName" select="$objName"/>
										</xsl:call-template>
									</xsl:variable>
									<xsl:if test="$IsExcluded!='1'">
										<tr valign="top">
											<td>
												<a>
													<xsl:attribute name="href">sp_<xsl:value-of select="translate(@name,'. ','')"/>.htm</xsl:attribute>
													<xsl:value-of select="@name"/>
												</a>
											</td>
											<td>
												<xsl:value-of select="@created"/>
											</td>
											<td align="right">
												<xsl:value-of select="count(param)"/>
											</td>
											<td align="right">
												<xsl:value-of select="format-number(string-length(code),'###,###,###,###,##0')"/>
											</td>
											<td align="right">
												<xsl:value-of select="count(/database/dependency[@objName=$objName and (@xtype='P') and @dependsOnObj])"/>
											</td>
											<td align="right">
												<xsl:value-of select="count(/database/dependency[(@refXtype='P') and @dependsOnObj=$objName])"/>
											</td>
											<td>
												<xsl:choose>
													<xsl:when test="/database/extendedProperty[@objType='PROCEDURE' and @propName='MS_Description' and @objName=$objName]/@value">
														<xsl:value-of select="/database/extendedProperty[@objType='PROCEDURE' and @propName='MS_Description' and @objName=$objName]/@value"/>
													</xsl:when>
													<xsl:otherwise>&#160;</xsl:otherwise>
												</xsl:choose>
											</td>
										</tr>
									</xsl:if>
								</xsl:for-each>
							</tbody>
						</table>
						<xsl:if test="$DrawPieCharts=1">
							<br/>
							<br/>
							<center>
								<img alt="Largest stored procedures by code length.  Click the labels to navigate." border="0" usemap="#Largeststoredproceduresbycodelength" src="sproccomplexity.gif"/>
							</center>
							<xsl:value-of select="$ImageMaps" disable-output-escaping="yes"/>
						</xsl:if>
					</item>
				</page>
				<xsl:for-each select="procedure[(@xtype='P')]">
					<xsl:sort select="@name" data-type="text"/>
					<xsl:variable name="IsExcluded">
						<xsl:call-template name="IsExcluded">
							<xsl:with-param name="xtype" select="@xtype"/>
							<xsl:with-param name="ObjName" select="@name"/>
						</xsl:call-template>
					</xsl:variable>
					<xsl:if test="$IsExcluded!='1'">
						<xsl:call-template name="drawProcedure"/>
					</xsl:if>
				</xsl:for-each>
			</xsl:if>
			<!--
			user defined functions
			-->
			<xsl:if test="count(procedure[(@xtype='FN' or @xtype='IF' or @xtype='TF') and not(@name = /database/DBSpecGen/exclude/server[@name=$serverName]/database[@name=$dbName]/udf/@name)]) != 0">
				<page id="cntUDFs">
					<xsl:attribute name="title"><xsl:value-of select="/database/@name"/> user defined functions</xsl:attribute>
					<xsl:attribute name="subtitle">Database Reference - <xsl:value-of select="/database/@name"/></xsl:attribute>
					<item>
						<h2 class="dtH2">User defined function overview</h2>
						<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
							<thead>
								<tr valign="top">
									<th>name</th>
									<th>created</th>
									<th>code length</th>
									<th>dependents</th>
									<th>depends on</th>
									<th>comment</th>
								</tr>
							</thead>
							<tbody>
								<xsl:for-each select="procedure[@xtype='FN' or @xtype='IF' or @xtype='TF']">
									<xsl:sort select="@name" data-type="text"/>
									<xsl:variable name="objName">
										<xsl:value-of select="@name"/>
									</xsl:variable>
									<xsl:variable name="IsExcluded">
										<xsl:call-template name="IsExcluded">
											<xsl:with-param name="xtype" select="@xtype"/>
											<xsl:with-param name="ObjName" select="$objName"/>
										</xsl:call-template>
									</xsl:variable>
									<xsl:if test="$IsExcluded!='1'">
										<tr valign="top">
											<td>
												<a>
													<xsl:attribute name="href">udf_<xsl:value-of select="translate(@name,'. ','')"/>.htm</xsl:attribute>
													<xsl:value-of select="@name"/>
												</a>
											</td>
											<td>
												<xsl:value-of select="@created"/>
											</td>
											<td align="right">
												<xsl:value-of select="format-number(string-length(code),'###,###,###,###,##0')"/>
											</td>
											<td align="right">
												<xsl:value-of select="count(/database/dependency[@objName=$objName and (@xtype='FN' or @xtype='IF' or @xtype='TF') and @dependsOnObj])"/>
											</td>
											<td align="right">
												<xsl:value-of select="count(/database/dependency[(@refXtype='FN' or @refXtype='IF' or @refXtype='TF') and @dependsOnObj=$objName])"/>
											</td>
											<td>
												<xsl:choose>
													<xsl:when test="/database/extendedProperty[@objType='FUNCTION' and @propName='MS_Description' and @objName=$objName]/@value">
														<xsl:value-of select="/database/extendedProperty[@objType='FUNCTION' and @propName='MS_Description' and @objName=$objName]/@value"/>
													</xsl:when>
													<xsl:otherwise>&#160;</xsl:otherwise>
												</xsl:choose>
											</td>
										</tr>
									</xsl:if>
								</xsl:for-each>
							</tbody>
						</table>
						<xsl:if test="$DrawPieCharts=1">
							<br/>
							<br/>
							<center>
								<img alt="Largest user defined functions by code length.  Click the labels to navigate." border="0" usemap="#Largestuserdefinedfunctionsbycodelength" src="udfcomplexity.gif"/>
							</center>
							<xsl:value-of select="$ImageMaps" disable-output-escaping="yes"/>
						</xsl:if>
					</item>
				</page>
				<xsl:for-each select="procedure[@xtype='FN' or @xtype='IF' or @xtype='TF']">
					<xsl:variable name="IsExcluded">
						<xsl:call-template name="IsExcluded">
							<xsl:with-param name="xtype" select="@xtype"/>
							<xsl:with-param name="ObjName" select="@name"/>
						</xsl:call-template>
					</xsl:variable>
					<xsl:if test="$IsExcluded!='1'">
						<xsl:call-template name="drawProcedure"/>
					</xsl:if>
				</xsl:for-each>
			</xsl:if>
			<!--
			user defined types - we don't exclude udts.
			-->
			<xsl:if test="userDefinedType">
				<page id="cntUDTs">
					<xsl:attribute name="title"><xsl:value-of select="/database/@name"/> user defined data types</xsl:attribute>
					<xsl:attribute name="subtitle">Database Reference - <xsl:value-of select="/database/@name"/></xsl:attribute>
					<item>
						<h2 class="dtH2">User defined type overview</h2>
						<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
							<thead>
								<tr valign="top">
									<th>name</th>
									<th>base type</th>
									<th>length</th>
									<th>nulls</th>
								</tr>
							</thead>
							<tbody>
								<xsl:for-each select="userDefinedType">
									<tr valign="top">
										<td>
											<a>
												<xsl:attribute name="href">udt_<xsl:value-of select="translate(@name,'. ','')"/>.htm</xsl:attribute>
												<xsl:value-of select="@name"/>
											</a>
										</td>
										<td>
											<xsl:value-of select="@baseType"/>
										</td>
										<td>
											<xsl:choose>
												<xsl:when test="@baseType='text' or @baseType='ntext' or @baseType='image'">(n/a)</xsl:when>
												<xsl:otherwise>
													<xsl:value-of select="@length"/>
												</xsl:otherwise>
											</xsl:choose>
										</td>
										<td>
											<xsl:choose>
												<xsl:when test="@allowNull=0">no</xsl:when>
												<xsl:otherwise>yes</xsl:otherwise>
											</xsl:choose>
										</td>
									</tr>
								</xsl:for-each>
							</tbody>
						</table>
					</item>
				</page>
				<xsl:for-each select="userDefinedType">
					<xsl:call-template name="drawUserDefinedType"/>
				</xsl:for-each>
			</xsl:if>
		</pages>
	</xsl:template>
	<!--
	
	
	
	draw the html describing a user defined type
	-->
	<xsl:template name="drawUserDefinedType">
		<xsl:variable name="dbName">
			<xsl:value-of select="/database/@name"/>
		</xsl:variable>
		<xsl:variable name="serverName">
			<xsl:value-of select="/database/@server"/>
		</xsl:variable>
		<page>
			<xsl:attribute name="id">udt_<xsl:value-of select="translate(string(@name),'. ','')"/></xsl:attribute>
			<xsl:attribute name="title"><xsl:value-of select="@name"/></xsl:attribute>
			<xsl:attribute name="subtitle">Database Reference - <xsl:value-of select="/database/@name"/></xsl:attribute>
			<keyword>
				<xsl:value-of select="@name"/>
			</keyword>
			<xsl:variable name="typeName">
				<xsl:value-of select="@name"/>
			</xsl:variable>
			<item>
				<h2 class="dtH2">Description</h2>
				<xsl:choose>
					<xsl:when test="string-length(@comment)!=0">
						<xsl:value-of select="@comment"/>
					</xsl:when>
					<xsl:otherwise>none</xsl:otherwise>
				</xsl:choose>
				<h2 class="dtH2">Properties</h2>
				<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
					<thead>
						<tr valign="top">
							<th>name</th>
							<th>base type</th>
							<th>length</th>
							<th>nulls</th>
							<th>default</th>
							<th>rule</th>
						</tr>
					</thead>
					<tbody>
						<tr valign="top">
							<td>
								<xsl:value-of select="@name"/>
							</td>
							<td>
								<xsl:value-of select="@baseType"/>
							</td>
							<td>
								<xsl:choose>
									<xsl:when test="@baseType='text' or @baseType='ntext' or @baseType='image'">(n/a)</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="@length"/>
									</xsl:otherwise>
								</xsl:choose>
							</td>
							<td>
								<xsl:choose>
									<xsl:when test="@allowNull=0">no</xsl:when>
									<xsl:otherwise>yes</xsl:otherwise>
								</xsl:choose>
							</td>
							<td>
								<xsl:choose>
									<xsl:when test="@default">
										<xsl:value-of select="@default"/>
									</xsl:when>
									<xsl:otherwise>&#160;</xsl:otherwise>
								</xsl:choose>
							</td>
							<td>
								<xsl:choose>
									<xsl:when test="@rule">
										<xsl:value-of select="@rule"/>
									</xsl:when>
									<xsl:otherwise>&#160;</xsl:otherwise>
								</xsl:choose>
							</td>
						</tr>
					</tbody>
				</table>
				<!--
        				usage for this udt
        				-->
				<xsl:if test="/database/table[(@xtype='U') and not(@name = /database/DBSpecGen/exclude/server[@name=$serverName]/database[@name=$dbName]/table/@name)]/column[@type=$typeName]">
					<h2 class="dtH2">Usage of <xsl:value-of select="$typeName"/>
					</h2>
					<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
						<thead>
							<tr valign="top">
								<th>used by</th>
								<th>type</th>
							</tr>
						</thead>
						<tbody>
							<xsl:for-each select="/database/table[(@xtype='U') and not(@name = /database/DBSpecGen/exclude/server[@name=$serverName]/database[@name=$dbName]/table/@name)]/column[@type=$typeName]">
								<tr valign="top">
									<td>
										<a>
											<xsl:attribute name="href">tbl_<xsl:value-of select="../@name"/>.htm</xsl:attribute>
											<xsl:value-of select="../@name"/>
										</a>
									</td>
									<td>table</td>
								</tr>
							</xsl:for-each>
							<xsl:for-each select="/database/table[(@xtype='V') and not(@name = /database/DBSpecGen/exclude/server[@name=$serverName]/database[@name=$dbName]/view/@name)]/column[@type=$typeName]">
								<tr valign="top">
									<td>
										<a>
											<xsl:attribute name="href">vw_<xsl:value-of select="../@name"/>.htm</xsl:attribute>
											<xsl:value-of select="../@name"/>
										</a>
									</td>
									<td>view</td>
								</tr>
							</xsl:for-each>
						</tbody>
					</table>
				</xsl:if>
				<!--
				extended properties
				-->
				<xsl:if test="/database/extendedProperty[@objName=$typeName or @parentObj=$typeName]">
					<h2 class="dtH2">Extended properties</h2>
					<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
						<thead>
							<tr valign="top">
								<th>parent object</th>
								<th>object</th>
								<th>type</th>
								<th>property name</th>
								<th>value</th>
							</tr>
						</thead>
						<tbody>
							<xsl:for-each select="/database/extendedProperty[@objName=$typeName or @parentObj=$typeName]">
								<tr valign="top">
									<td>
										<xsl:choose>
											<xsl:when test="@parentObj">
												<xsl:value-of select="@parentObj"/>
											</xsl:when>
											<xsl:otherwise>&#160;</xsl:otherwise>
										</xsl:choose>
									</td>
									<td>
										<xsl:value-of select="@objName"/>
									</td>
									<td>
										<xsl:value-of select="@objType"/>
									</td>
									<td>
										<xsl:value-of select="@propName"/>
									</td>
									<td>
										<xsl:value-of select="@value"/>
									</td>
								</tr>
							</xsl:for-each>
						</tbody>
					</table>
				</xsl:if>
			</item>
		</page>
	</xsl:template>
	<!--
	
	
	
	
	
	
	
	
	
	
	
	
	draws the html describing a table or view
	-->
	<xsl:template name="drawTable">
		<xsl:variable name="tableName">
			<xsl:value-of select="@name"/>
		</xsl:variable>
		<page>
			<xsl:attribute name="id"><xsl:choose><xsl:when test="(@xtype='U')">tbl_</xsl:when><xsl:when test="(@xtype='V')">vw_</xsl:when></xsl:choose><xsl:value-of select="translate(string(@name),'. ','')"/></xsl:attribute>
			<xsl:attribute name="title"><xsl:value-of select="@name"/></xsl:attribute>
			<xsl:attribute name="subtitle">Database Reference - <xsl:value-of select="/database/@name"/></xsl:attribute>
			<keyword>
				<xsl:value-of select="@name"/>
			</keyword>
			<item>
				<h2 class="dtH2">Description</h2>
				<xsl:choose>
					<xsl:when test="string-length(codeComments/summary)!=0">
						<pre class="comment">
							<xsl:value-of select="codeComments/summary"/>
						</pre>
					</xsl:when>
					<xsl:when test="string-length(/database/extendedProperty[@objName=$tableName and @propName='MS_Description']/@value)!=0">
						<xsl:value-of select="/database/extendedProperty[@objName=$tableName and @propName='MS_Description']/@value"/>
					</xsl:when>
					<xsl:otherwise>none</xsl:otherwise>
				</xsl:choose>
				<xsl:if test="@rowcount">
					<br/>
					<br/>
					As of <xsl:value-of select="translate(/database/@currentTime,'T',' ')"/>, the <xsl:value-of select="@name"/>
					<xsl:choose>
						<xsl:when test="(@xtype='U')"> table</xsl:when>
						<xsl:when test="(@xtype='V')"> view</xsl:when>
					</xsl:choose> had <xsl:value-of select="format-number(@rowcount,'###,###,###,###,###,###')"/> rows.  
				</xsl:if>
				<xsl:if test="@datasizeKB">
					<br/>
					<br/>
					The data in <xsl:value-of select="@name"/> occupy 			
					<xsl:choose>
						<xsl:when test="$UseMbAsSizeUnit=1">
							<xsl:value-of select="format-number(@datasizeKB div 1000,'###,###,###,##0.###')"/> mb.
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="format-number(@datasizeKB,'###,###,###,##0')"/> kb.
						</xsl:otherwise>
					</xsl:choose>
					<br/>
					<br/>
					The indexes in <xsl:value-of select="@name"/> occupy 			
					<xsl:choose>
						<xsl:when test="$UseMbAsSizeUnit=1">
							<xsl:value-of select="format-number(@indexsizeKB div 1000,'###,###,###,##0.###')"/> mb.
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="format-number(@indexsizeKB,'###,###,###,##0')"/> kb.
						</xsl:otherwise>
					</xsl:choose>
				</xsl:if>
				<br/>
				<br/>
				The maximum size of each row in <xsl:value-of select="@name"/> is <xsl:value-of select="format-number(sum(column[@type!='text' and @type!='ntext' and @type!='image']/@bytes),'###,##0')"/> bytes.  			
				<h2 class="dtH2">Columns</h2>
				<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
					<thead>
						<tr valign="top">
							<th>column</th>
							<th>datatype</th>
							<th>length</th>
							<th>bytes</th>
							<xsl:if test="(@xtype='U')">
								<th>default</th>
							</xsl:if>
							<th>nulls</th>
							<xsl:if test="(@xtype='U')">
								<th>PK</th>
								<th>FK</th>
								<th>UQ</th>
							</xsl:if>
							<th>comment</th>
						</tr>
					</thead>
					<tbody>
						<xsl:for-each select="column">
							<xsl:variable name="colName">
								<xsl:value-of select="@name"/>
							</xsl:variable>
							<tr valign="top">
								<td>
									<xsl:value-of select="@name"/>
								</td>
								<td>
									<xsl:variable name="CurrentType">
										<xsl:value-of select="@type"/>
									</xsl:variable>
									<xsl:choose>
										<xsl:when test="/database/userDefinedType[@name=$CurrentType]">
											<a>
												<xsl:attribute name="href">udt_<xsl:value-of select="translate(string($CurrentType),'. ','')"/>.htm</xsl:attribute>
												<xsl:value-of select="$CurrentType"/>
											</a>
										</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="$CurrentType"/>
										</xsl:otherwise>
									</xsl:choose>
								</td>
								<td align="right">
									<xsl:choose>
										<xsl:when test="@type='text' or @type='ntext' or @type='image'">(n/a)</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="@length"/>
										</xsl:otherwise>
									</xsl:choose>
								</td>
								<td align="right">
									<xsl:choose>
										<xsl:when test="@type='text' or @type='ntext' or @type='image'">(n/a)</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="@bytes"/>
										</xsl:otherwise>
									</xsl:choose>
								</td>
								<xsl:if test="../@xtype='U'">
									<td align="right">
										<xsl:choose>
											<xsl:when test="string-length(@defaultValue)!=0">
												<xsl:value-of select="@defaultValue"/>
											</xsl:when>
											<xsl:otherwise>&#160;</xsl:otherwise>
										</xsl:choose>
									</td>
								</xsl:if>
								<td>
									<xsl:choose>
										<xsl:when test="@allowNull=0">no</xsl:when>
										<xsl:otherwise>yes</xsl:otherwise>
									</xsl:choose>
								</td>
								<xsl:if test="../@xtype='U'">
									<td>
										<xsl:choose>
											<xsl:when test="count(../constraint[@colName=$colName and @isPrimaryKey=1])!=0">yes</xsl:when>
											<!--
													for composite keys
													-->
											<xsl:when test="../constraint[(contains(@colName,concat($colName,',')) or contains(@colName,concat(', ',$colName))) and @isPrimaryKey=1]">composite PK</xsl:when>
											<xsl:otherwise>&#160;</xsl:otherwise>
										</xsl:choose>
									</td>
									<td>
										<xsl:choose>
											<xsl:when test="../constraint[contains(@colName,$colName) and @isForeignKey=1]">
												<xsl:if test="../constraint[@colName=$colName and @isForeignKey=1]">to <a>
														<xsl:variable name="IsExcluded">
															<xsl:call-template name="IsExcluded">
																<xsl:with-param name="xtype" select="'U'"/>
																<xsl:with-param name="ObjName" select="../constraint[@colName=$colName and @isForeignKey=1]/@refTable"/>
															</xsl:call-template>
														</xsl:variable>
														<xsl:choose>
															<xsl:when test="$IsExcluded='1'">
																<xsl:attribute name="href">javascript:void(0);</xsl:attribute>
															</xsl:when>
															<xsl:otherwise>
																<xsl:attribute name="href">tbl_<xsl:value-of select="translate(../constraint[@colName=$colName and @isForeignKey=1]/@refTable,'. ','')"/>.htm</xsl:attribute>
															</xsl:otherwise>
														</xsl:choose>
														<xsl:value-of select="../constraint[@colName=$colName and @isForeignKey=1]/@refTable"/>
													</a>.<xsl:value-of select="../constraint[@colName=$colName and @isForeignKey=1]/@refColumn"/>
															&#160;
													</xsl:if>
												<!--
												for composite keys
												-->
												<xsl:if test="../constraint[(contains(@colName,concat($colName,',')) or contains(@colName,concat(', ',$colName))) and @isForeignKey=1]">composite FK to <a>
														<xsl:variable name="IsExcluded">
															<xsl:call-template name="IsExcluded">
																<xsl:with-param name="xtype" select="'U'"/>
																<xsl:with-param name="ObjName" select="../constraint[(contains(@colName,concat($colName,',')) or contains(@colName,concat(', ',$colName))) and @isForeignKey=1]/@refTable"/>
															</xsl:call-template>
														</xsl:variable>
														<xsl:choose>
															<xsl:when test="$IsExcluded='1'">
																<xsl:attribute name="href">javascript:void(0)</xsl:attribute>
															</xsl:when>
															<xsl:otherwise>
																<xsl:attribute name="href">tbl_<xsl:value-of select="translate(../constraint[(contains(@colName,concat($colName,',')) or contains(@colName,concat(', ',$colName))) and @isForeignKey=1]/@refTable,'. ','')"/>.htm</xsl:attribute>
															</xsl:otherwise>
														</xsl:choose>
														<xsl:value-of select="../constraint[(contains(@colName,concat($colName,',')) or contains(@colName,concat(', ',$colName))) and @isForeignKey=1]/@refTable"/>
													</a> (<xsl:value-of select="../constraint[(contains(@colName,concat($colName,',')) or contains(@colName,concat(', ',$colName))) and @isForeignKey=1]/@refColumn"/>)
												</xsl:if>
											</xsl:when>
											<xsl:otherwise>&#160;</xsl:otherwise>
										</xsl:choose>
									</td>
									<td>
										<xsl:choose>
											<xsl:when test="../constraint[contains(@colName,$colName) and @isUnique=1]">
												<xsl:if test="../constraint[@colName=$colName and @isUnique=1]">yes</xsl:if>
												<!--
														for composite keys
														-->
												<xsl:if test="../constraint[(contains(@colName,concat($colName,',')) or contains(@colName,concat(', ',$colName))) and @isUnique=1]">composite UQ (<xsl:value-of select="../constraint[(contains(@colName,concat($colName,',')) or contains(@colName,concat(', ',$colName))) and @isUnique=1]/@colName"/>)</xsl:if>
											</xsl:when>
											<xsl:otherwise>&#160;</xsl:otherwise>
										</xsl:choose>
									</td>
								</xsl:if>
								<td>
									<xsl:choose>
										<xsl:when test="string-length(/database/extendedProperty[@objType='COLUMN' and @parentObj=$tableName and @objName=$colName and @propName='MS_Description']/@value)!=0">
											<xsl:value-of select="/database/extendedProperty[@objType='COLUMN' and @parentObj=$tableName and @objName=$colName and @propName='MS_Description']/@value"/>
										</xsl:when>
										<xsl:otherwise>&#160;</xsl:otherwise>
									</xsl:choose>
								</td>
							</tr>
						</xsl:for-each>
					</tbody>
				</table>
				<!--
				who references this table?  who does it reference?
				-->
				<xsl:choose>
					<xsl:when test="$IsVML = 1">
						<xsl:call-template name="DrawReferenceGraph"/>
					</xsl:when>
				</xsl:choose>
				<!--
				does this table have a trigger?
				-->
				<xsl:if test="trigger">
					<a name="TRIGGERS"/>
					<h2 class="dtH2">Triggers</h2>
					<ul>
						<xsl:for-each select="trigger">
							<li>
								<a href="javascript:void(0)">
									<xsl:attribute name="onclick">document.all.trg<xsl:value-of select="translate(string(@name),'. ','')"/>.style.display=(document.all.trg<xsl:value-of select="translate(string(@name),'. ','')"/>.style.display=='block'?'none':'block')</xsl:attribute>
									<xsl:value-of select="@name"/>
								</a>
								<pre class="code" style="display:none;">
									<xsl:attribute name="id">trg<xsl:value-of select="translate(string(@name),'. ','')"/></xsl:attribute>
									<xsl:value-of select="code"/>
								</pre>
							</li>
						</xsl:for-each>
					</ul>
				</xsl:if>
				<!--
				does this table have a check constraint?
				-->
				<xsl:if test="constraint[@hasCheck=1]">
					<h2 class="dtH2">Check constraints</h2>
					<ul>
						<xsl:for-each select="constraint[@hasCheck=1]">
							<li>
								<a href="javascript:void(0)">
									<xsl:attribute name="onclick">document.all.chk<xsl:value-of select="translate(string(@name),'. ','')"/>.style.display=(document.all.chk<xsl:value-of select="translate(string(@name),'. ','')"/>.style.display=='block'?'none':'block')</xsl:attribute>
									<xsl:value-of select="@name"/>
								</a>
								<pre class="code" style="display:none;">
									<xsl:attribute name="id">chk<xsl:value-of select="translate(string(@name),'. ','')"/></xsl:attribute>
									<xsl:value-of select="code"/>
								</pre>
							</li>
						</xsl:for-each>
					</ul>
				</xsl:if>
				<!--
				does this table have an index?
				-->
				<xsl:if test="index">
					<h2 class="dtH2">Indexes</h2>
					<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
						<thead>
							<tr valign="top">
								<th>name</th>
								<th>description</th>
								<th>keys</th>
							</tr>
						</thead>
						<tbody>
							<xsl:for-each select="index">
								<tr valign="top">
									<td>
										<xsl:value-of select="@name"/>
									</td>
									<td>
										<xsl:value-of select="@description"/>
									</td>
									<td>
										<xsl:value-of select="@keys"/>
									</td>
								</tr>
							</xsl:for-each>
						</tbody>
					</table>
				</xsl:if>
				<!--
				history log
				-->
				<xsl:if test="codeComments/historylog/log">
					<h2 class="dtH2">History</h2>
					<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
						<thead>
							<tr valign="top">
								<th>revision</th>
								<th>date</th>
								<th>email</th>
								<th>comment</th>
							</tr>
						</thead>
						<tbody>
							<xsl:for-each select="codeComments/historylog/log">
								<tr valign="top">
									<td>
										<xsl:value-of select="@revision"/>
									</td>
									<td>
										<xsl:value-of select="@date"/>
									</td>
									<td>
										<xsl:value-of select="@email"/>
									</td>
									<td>
										<xsl:value-of select="."/>
									</td>
								</tr>
							</xsl:for-each>
						</tbody>
					</table>
				</xsl:if>
				<!--
				extended properties
				-->
				<xsl:if test="
					(
						/database/extendedProperty[@objName=$tableName or @parentObj=$tableName] 
						and $ShowAllExtendedProperties=1
					)
					or 
					(
						/database/extendedProperty[@propName!='MS_Description' and (@objName=$tableName or @parentObj=$tableName)] 
						and $ShowAllExtendedProperties=0
					)">
					<h2 class="dtH2">Extended properties</h2>
					<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
						<thead>
							<tr valign="top">
								<th>parent object</th>
								<th>object</th>
								<th>type</th>
								<th>property name</th>
								<th>value</th>
							</tr>
						</thead>
						<tbody>
							<xsl:for-each select="/database/extendedProperty[
									($ShowAllExtendedProperties=1 or ($ShowAllExtendedProperties=0 and @propName!='MS_Description')) 
									and (@objName=$tableName or @parentObj=$tableName)]">
								<tr valign="top">
									<td>
										<xsl:choose>
											<xsl:when test="@parentObj">
												<xsl:value-of select="@parentObj"/>
											</xsl:when>
											<xsl:otherwise>&#160;</xsl:otherwise>
										</xsl:choose>
									</td>
									<td>
										<xsl:value-of select="@objName"/>
									</td>
									<td>
										<xsl:value-of select="@objType"/>
									</td>
									<td>
										<xsl:value-of select="@propName"/>
									</td>
									<td>
										<xsl:value-of select="@value"/>
									</td>
								</tr>
							</xsl:for-each>
						</tbody>
					</table>
				</xsl:if>
				<!--
				permissions
				-->
				<xsl:if test="/database/permission[@object=$tableName]">
					<h2 class="dtH2">Permissions</h2>
					<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
						<thead>
							<tr valign="top">
								<th>grantor</th>
								<th>grantee</th>
								<th>allow</th>
								<th>action</th>
								<th>column</th>
							</tr>
						</thead>
						<tbody>
							<xsl:for-each select="/database/permission[@object=$tableName]">
								<tr valign="top">
									<td>
										<xsl:value-of select="@grantor"/>
									</td>
									<td>
										<xsl:value-of select="@grantee"/>
									</td>
									<td>
										<xsl:choose>
											<xsl:when test="@protecttype='deny'">
												<font color="red">
													<xsl:value-of select="@protecttype"/>
												</font>
											</xsl:when>
											<xsl:otherwise>
												<font color="green">
													<xsl:value-of select="@protecttype"/>
												</font>
											</xsl:otherwise>
										</xsl:choose>
									</td>
									<td>
										<xsl:value-of select="@action"/>
									</td>
									<td>
										<xsl:choose>
											<xsl:when test="string-length(@column)!=0">
												<xsl:value-of select="@column"/>
											</xsl:when>
											<xsl:otherwise>&#160;</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
							</xsl:for-each>
						</tbody>
					</table>
				</xsl:if>
				<!--
				show the dependencies
				-->
				<xsl:choose>
					<xsl:when test="$IsVML = 1">
						<xsl:call-template name="DrawDependencyGraph"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="DrawDependencies"/>
					</xsl:otherwise>
				</xsl:choose>
				<!--
				view definition
				-->
				<xsl:if test="(@xtype='V')">
					<h2 class="dtH2">Code</h2>
					<a href="javascript:void(0)">
						<xsl:attribute name="onclick">document.all.code<xsl:value-of select="translate(string(@name),'. ','')"/>.style.display=(document.all.code<xsl:value-of select="translate(string(@name),'. ','')"/>.style.display=='block'?'none':'block')</xsl:attribute>Show/hide view definition:</a>
					<pre style="display:none;" class="code">
						<xsl:attribute name="id">code<xsl:value-of select="translate(string(@name),'. ','')"/></xsl:attribute>
						<xsl:value-of select="code"/>
					</pre>
				</xsl:if>
			</item>
		</page>
	</xsl:template>
	<!--
	
	
	
	
	
	
	
	
	
	
	
	
	draw a procedure page...
	-->
	<xsl:template name="drawProcedure">
		<page>
			<xsl:attribute name="id"><xsl:choose><xsl:when test="(@xtype='P')">sp_</xsl:when><xsl:when test="@xtype='FN' or @xtype='IF' or @xtype='TF'">udf_</xsl:when></xsl:choose><xsl:value-of select="translate(string(@name),'. ','')"/></xsl:attribute>
			<xsl:attribute name="title"><xsl:value-of select="@name"/></xsl:attribute>
			<xsl:attribute name="subtitle">Database Reference - <xsl:value-of select="/database/@name"/></xsl:attribute>
			<keyword>
				<xsl:value-of select="@name"/>
			</keyword>
			<xsl:variable name="procName">
				<xsl:value-of select="@name"/>
			</xsl:variable>
			<xsl:variable name="xtype">
				<xsl:value-of select="@xtype"/>
			</xsl:variable>
			<item>
				<xsl:if test="@xtype='FN' or @xtype='IF' or @xtype='TF'">
					<h2 class="dtH2">Type</h2>
					<xsl:choose>
						<xsl:when test="@xtype='FN'">Scalar function</xsl:when>
						<xsl:when test="@xtype='IF'">Inline table-valued function</xsl:when>
						<xsl:when test="@xtype='TF'">Multistatement table-valued function</xsl:when>
					</xsl:choose>
				</xsl:if>
				<xsl:choose>
					<xsl:when test="codeComments">
						<h2 class="dtH2">Description</h2>
						<xsl:choose>
							<xsl:when test="codeComments/summary">
								<pre class="comment">
									<xsl:value-of select="codeComments/summary"/>
								</pre>
							</xsl:when>
							<xsl:otherwise>none</xsl:otherwise>
						</xsl:choose>
						<h2 class="dtH2">Parameters</h2>
						<xsl:choose>
							<xsl:when test="codeComments/parameters/attributes | codeComments/parameters/param">
								<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
									<thead>
										<tr valign="top">
											<th>name</th>
											<th>datatype</th>
											<th>length</th>
											<th>In/Out</th>
											<th>required</th>
											<th>comment</th>
										</tr>
									</thead>
									<tbody>
										<xsl:if test="$xtype='FN'">
											<tr valign="top">
												<td>return value</td>
												<td>
													<xsl:value-of select="/database/procedure[@name=$procName]/param[position()=1]/@datatype"/>
												</td>
												<td>
													<xsl:choose>
														<xsl:when test="/database/procedure[@name=$procName]/param[position()=1]/@length">
															<xsl:value-of select="/database/procedure[@name=$procName]/param[position()=1]/@length"/>
														</xsl:when>
														<xsl:otherwise>&#160;</xsl:otherwise>
													</xsl:choose>
												</td>
												<td>
													<xsl:choose>
														<xsl:when test="/database/procedure[@name=$procName]/param[position()=1]/@inOut">
															<xsl:value-of select="/database/procedure[@name=$procName]/param[position()=1]/@inOut"/>
														</xsl:when>
														<xsl:otherwise>&#160;</xsl:otherwise>
													</xsl:choose>
												</td>
												<td>(n/a)</td>
												<td>&#160;</td>
											</tr>
										</xsl:if>
										<xsl:for-each select="codeComments/parameters/attributes | codeComments/parameters/param">
											<xsl:variable name="pos">
												<xsl:choose>
													<xsl:when test="$xtype='FN'">
														<xsl:value-of select="position() + 1"/>
													</xsl:when>
													<xsl:otherwise>
														<xsl:value-of select="position()"/>
													</xsl:otherwise>
												</xsl:choose>
											</xsl:variable>
											<tr valign="top">
												<td>
													<xsl:value-of select="/database/procedure[@name=$procName]/param[position()=$pos]/@name"/>
												</td>
												<td>
													<xsl:value-of select="/database/procedure[@name=$procName]/param[position()=$pos]/@datatype"/>
												</td>
												<td>
													<xsl:choose>
														<xsl:when test="/database/procedure[@name=$procName]/param[position()=$pos]/@length">
															<xsl:value-of select="/database/procedure[@name=$procName]/param[position()=$pos]/@length"/>
														</xsl:when>
														<xsl:otherwise>&#160;</xsl:otherwise>
													</xsl:choose>
												</td>
												<td>
													<xsl:choose>
														<xsl:when test="/database/procedure[@name=$procName]/param[position()=$pos]/@inOut">
															<xsl:value-of select="/database/procedure[@name=$procName]/param[position()=$pos]/@inOut"/>
														</xsl:when>
														<xsl:otherwise>&#160;</xsl:otherwise>
													</xsl:choose>
												</td>
												<td>
													<xsl:choose>
														<xsl:when test="@required">
															<xsl:value-of select="@required"/>
														</xsl:when>
														<xsl:otherwise>&#160;</xsl:otherwise>
													</xsl:choose>
												</td>
												<td>
													<xsl:choose>
														<xsl:when test="@description">
															<xsl:value-of select="@description"/>
														</xsl:when>
														<xsl:otherwise>&#160;</xsl:otherwise>
													</xsl:choose>
												</td>
											</tr>
										</xsl:for-each>
									</tbody>
								</table>
							</xsl:when>
							<xsl:otherwise>none</xsl:otherwise>
						</xsl:choose>
						<h2 class="dtH2">Scope</h2>
						<xsl:choose>
							<xsl:when test="codeComments/scope">
								<xsl:value-of select="codeComments/scope"/>
							</xsl:when>
							<xsl:otherwise>none</xsl:otherwise>
						</xsl:choose>
						<xsl:choose>
							<xsl:when test="codeComments/returns">
								<h2 class="dtH2">Return values</h2>
								<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
									<thead>
										<tr valign="top">
											<th>int value</th>
											<th>description</th>
										</tr>
									</thead>
									<tbody>
										<xsl:for-each select="codeComments/returns/return">
											<tr valign="top">
												<td>
													<xsl:value-of select="@value"/>
												</td>
												<td>
													<xsl:value-of select="@description"/>
												</td>
											</tr>
										</xsl:for-each>
									</tbody>
								</table>
								<xsl:for-each select="codeComments/returns/recordset">
									<xsl:if test="position()=1">
										<h2 class="dtH2">Return record sets</h2>
									</xsl:if>
									<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
										<thead>
											<tr valign="top">
												<th>column name</th>
												<th>type</th>
												<th>length</th>
												<th>description</th>
											</tr>
										</thead>
										<tbody>
											<xsl:for-each select="column">
												<tr valign="top">
													<td>
														<xsl:choose>
															<xsl:when test="string-length(@name)!=0">
																<xsl:value-of select="@name"/>
															</xsl:when>
															<xsl:otherwise>&#160;</xsl:otherwise>
														</xsl:choose>
													</td>
													<td>
														<xsl:choose>
															<xsl:when test="string-length(@datatype)!=0">
																<xsl:value-of select="@datatype"/>
															</xsl:when>
															<xsl:otherwise>&#160;</xsl:otherwise>
														</xsl:choose>
													</td>
													<td>
														<xsl:choose>
															<xsl:when test="string-length(@datalength)!=0">
																<xsl:value-of select="@datalength"/>
															</xsl:when>
															<xsl:otherwise>&#160;</xsl:otherwise>
														</xsl:choose>
													</td>
													<td>
														<xsl:choose>
															<xsl:when test="string-length(@description)!=0">
																<xsl:value-of select="@description"/>
															</xsl:when>
															<xsl:otherwise>&#160;</xsl:otherwise>
														</xsl:choose>
													</td>
												</tr>
											</xsl:for-each>
										</tbody>
									</table>
									<br/>
								</xsl:for-each>
							</xsl:when>
							<xsl:otherwise>
								<span class="dbComment">not available</span>
							</xsl:otherwise>
						</xsl:choose>
						<h2 class="dtH2">History</h2>
						<xsl:choose>
							<xsl:when test="codeComments/historylog">
								<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
									<thead>
										<tr valign="top">
											<th>revision</th>
											<th>date</th>
											<th>alias</th>
											<th>comment</th>
										</tr>
									</thead>
									<tbody>
										<xsl:for-each select="codeComments/historylog/log">
											<tr valign="top">
												<td>
													<xsl:value-of select="@revision"/>
												</td>
												<td>
													<xsl:value-of select="@date"/>
												</td>
												<td>
													<xsl:value-of select="@email"/>
												</td>
												<td>
													<xsl:value-of select="."/>
												</td>
											</tr>
										</xsl:for-each>
									</tbody>
								</table>
							</xsl:when>
							<xsl:otherwise>
								<span class="dbComment">not available</span>
							</xsl:otherwise>
						</xsl:choose>
						<h2 class="dtH2">Usage</h2>
						<xsl:choose>
							<xsl:when test="codeComments/samples/sample">
								<xsl:for-each select="codeComments/samples/sample">
									<xsl:value-of select="description"/>
									<pre class="code" style="display:block">
										<xsl:value-of select="code"/>
									</pre>
								</xsl:for-each>
							</xsl:when>
							<xsl:otherwise>
								<span class="dbComment">not available</span>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
					<xsl:otherwise>
						<h2 class="dtH2">Description</h2>
						<xsl:choose>
							<xsl:when test="string-length(/database/extendedProperty[@objName=$procName and @propName='MS_Description']/@value)!=0">
								<xsl:value-of select="/database/extendedProperty[@objName=$procName and @propName='MS_Description']/@value"/>
							</xsl:when>
							<xsl:otherwise>none</xsl:otherwise>
						</xsl:choose>
						<h2 class="dtH2">Parameters</h2>
						<xsl:choose>
							<xsl:when test="param">
								<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
									<thead>
										<tr valign="top">
											<th>name</th>
											<th>datatype</th>
											<th>length</th>
											<th>In/Out</th>
										</tr>
									</thead>
									<tbody>
										<xsl:for-each select="param">
											<xsl:sort select="@ordinal" order="ascending"/>
											<tr valign="top">
												<td>
													<xsl:choose>
														<xsl:when test="string-length(@name)!=0">
															<xsl:value-of select="@name"/>
														</xsl:when>
														<xsl:otherwise>return value</xsl:otherwise>
													</xsl:choose>
												</td>
												<td>
													<xsl:value-of select="@datatype"/>
												</td>
												<td align="right">
													<xsl:choose>
														<xsl:when test="string-length(@length)!=0">
															<xsl:value-of select="@length"/>
														</xsl:when>
														<xsl:otherwise>&#160;</xsl:otherwise>
													</xsl:choose>
												</td>
												<td>
													<xsl:value-of select="@inOut"/>
												</td>
											</tr>
										</xsl:for-each>
									</tbody>
								</table>
								<br/>
								<br/>
							</xsl:when>
							<xsl:otherwise>none</xsl:otherwise>
						</xsl:choose>
						<xsl:if test="error">
							<xsl:value-of select="error"/>
						</xsl:if>
					</xsl:otherwise>
				</xsl:choose>
				<!--
				extended properties
				-->
				<xsl:if test="
					(
						/database/extendedProperty[@objName=$procName or @parentObj=$procName] 
						and $ShowAllExtendedProperties=1
					)
					or 
					(
						/database/extendedProperty[@propName!='MS_Description' and (@objName=$procName or @parentObj=$procName)] 
						and $ShowAllExtendedProperties=0
					)">
					<h2 class="dtH2">Extended properties</h2>
					<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
						<thead>
							<tr valign="top">
								<th>parent object</th>
								<th>object</th>
								<th>type</th>
								<th>property name</th>
								<th>value</th>
							</tr>
						</thead>
						<tbody>
							<xsl:for-each select="/database/extendedProperty[
									($ShowAllExtendedProperties=1 or ($ShowAllExtendedProperties=0 and @propName!='MS_Description')) 
									and (@objName=$procName or @parentObj=$procName)]">
								<tr valign="top">
									<td>
										<xsl:choose>
											<xsl:when test="@parentObj">
												<xsl:value-of select="@parentObj"/>
											</xsl:when>
											<xsl:otherwise>&#160;</xsl:otherwise>
										</xsl:choose>
									</td>
									<td>
										<xsl:value-of select="@objName"/>
									</td>
									<td>
										<xsl:value-of select="@objType"/>
									</td>
									<td>
										<xsl:value-of select="@propName"/>
									</td>
									<td>
										<xsl:value-of select="@value"/>
									</td>
								</tr>
							</xsl:for-each>
						</tbody>
					</table>
				</xsl:if>
				<!--
				permissions
				-->
				<xsl:if test="/database/permission[@object=$procName]">
					<h2 class="dtH2">Permissions</h2>
					<table class="dtTABLE" cellspacing="0" style="WIDTH: 94%">
						<thead>
							<tr valign="top">
								<th>grantor</th>
								<th>grantee</th>
								<th>allow</th>
								<th>action</th>
							</tr>
						</thead>
						<tbody>
							<xsl:for-each select="/database/permission[@object=$procName]">
								<tr valign="top">
									<td>
										<xsl:value-of select="@grantor"/>
									</td>
									<td>
										<xsl:value-of select="@grantee"/>
									</td>
									<td>
										<xsl:choose>
											<xsl:when test="@protecttype='deny'">
												<font color="red">
													<xsl:value-of select="@protecttype"/>
												</font>
											</xsl:when>
											<xsl:otherwise>
												<font color="green">
													<xsl:value-of select="@protecttype"/>
												</font>
											</xsl:otherwise>
										</xsl:choose>
									</td>
									<td>
										<xsl:value-of select="@action"/>
									</td>
								</tr>
							</xsl:for-each>
						</tbody>
					</table>
				</xsl:if>
				<!--
				show the dependencies
				-->
				<xsl:choose>
					<xsl:when test="$IsVML = 1">
						<xsl:call-template name="DrawDependencyGraph"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="DrawDependencies"/>
					</xsl:otherwise>
				</xsl:choose>
				<xsl:if test="codeComments/logic">
					<h2 class="dtH2">Logic</h2>
					<xsl:for-each select="codeComments/logic">
						<pre class="comment">
							<xsl:value-of select="."/>
						</pre>
						<br/>
					</xsl:for-each>
				</xsl:if>
				<!--
				show the code
				-->
				<h2 class="dtH2">Code</h2>
				<a href="javascript:void(0)">
					<xsl:attribute name="onclick">document.all.code<xsl:value-of select="translate(string(@name),'. ','')"/>.style.display=(document.all.code<xsl:value-of select="translate(string(@name),'. ','')"/>.style.display=='block'?'none':'block')</xsl:attribute>Show/hide code for <xsl:value-of select="@name"/>:</a>
				<pre style="display:none;" class="code">
					<xsl:attribute name="id">code<xsl:value-of select="translate(string(@name),'. ','')"/></xsl:attribute>
					<xsl:value-of select="code"/>
				</pre>
			</item>
		</page>
	</xsl:template>
	<!--
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	for drawing a non-vml dependency list
	-->
	<xsl:template name="DrawDependencies">
		<xsl:variable name="objName">
			<xsl:value-of select="@name"/>
		</xsl:variable>
		<xsl:variable name="objType">
			<xsl:value-of select="@xtype"/>
		</xsl:variable>
		<xsl:if test="/database/dependency[(@objName=$objName and @xtype=$objType and string-length(@dependsOnObj)!=0) or 
		                                   (@dependsOnObj=$objName and @refXtype=$objType and string-length(@objName)!=0)]">
			<tr valign="top">
				<td>
					<h2 class="dtH2">Dependencies</h2>
					<xsl:if test="/database/dependency[@dependsOnObj=$objName and @refXtype=$objType and string-length(@objName)!=0]">
						<span class="dbComment">
							<xsl:value-of select="@name"/> depends on: </span>
					</xsl:if>
					<table>
						<xsl:for-each select="/database/dependency[@dependsOnObj=$objName and @refXtype=$objType and string-length(@objName)!=0]">
							<xsl:sort select="@xtype"/>
							<xsl:sort select="@objName"/>
							<xsl:variable name="dep">
								<xsl:value-of select="@objName"/>
							</xsl:variable>
							<xsl:if test="/database/*[@name=$dep] or /database/table/constraint[@name=$dep]">
								<tr valign="top">
									<td>
										<xsl:choose>
											<xsl:when test="@refXtype='C'">
												<xsl:value-of select="$dep"/>
											</xsl:when>
											<xsl:when test=" (@refXtype='U') or 
																	(@refXtype='V') or 
																	(@refXtype='P') or 
																	@refXtype='FN' or 
																	@refXtype='TF' or 
																	@refXtype='IF'">
												<a>
													<xsl:variable name="IsExcluded">
														<xsl:call-template name="IsExcluded">
															<xsl:with-param name="xtype" select="@xtype"/>
															<xsl:with-param name="ObjName" select="$dep"/>
														</xsl:call-template>
													</xsl:variable>
													<xsl:choose>
														<xsl:when test="$IsExcluded='1'">
															<xsl:attribute name="href">javascript:void(0)</xsl:attribute>
														</xsl:when>
														<xsl:otherwise>
															<xsl:attribute name="href"><xsl:choose><xsl:when test="(@xtype='U')">tbl_</xsl:when><xsl:when test="(@xtype='V')">vw_</xsl:when><xsl:when test="(@xtype='P')">sp_</xsl:when><xsl:when test="@xtype='FN'">udf_</xsl:when></xsl:choose><xsl:value-of select="translate(string($dep),'. ','')"/>.htm</xsl:attribute>
														</xsl:otherwise>
													</xsl:choose>
													<xsl:value-of select="$dep"/>
												</a>
											</xsl:when>
											<xsl:otherwise>
												<!-- 
												unknown refxtype.  must be a custom object 
												-->
												<a>
													<xsl:attribute name="href"><xsl:value-of select="@xtype"/>_<xsl:value-of select="translate(string($dep),'. ','')"/>.htm</xsl:attribute>
													<xsl:value-of select="$dep"/>
												</a>
											</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
							</xsl:if>
						</xsl:for-each>
					</table>
				</td>
			</tr>
		</xsl:if>
		<xsl:if test="/database/dependency[@objName=$objName and @xtype=$objType and string-length(@dependsOnObj)!=0]">
			<tr valign="top">
				<td>
					<span class="dbComment">The following objects depend on <xsl:value-of select="@name"/>: </span>
					<table>
						<xsl:for-each select="/database/dependency[@objName=$objName and @xtype=$objType and string-length(@dependsOnObj)!=0]">
							<xsl:sort select="@refXtype"/>
							<xsl:sort select="@dependsOnObj"/>
							<xsl:variable name="dep">
								<xsl:value-of select="@dependsOnObj"/>
							</xsl:variable>
							<xsl:if test="/database/table[@name=$dep] or
												/database/procedure[@name=$dep] or
												/database/table/constraint[@name=$dep and @type='CHECK'] or 
												/database/object[@name=$dep]">
								<tr valign="top">
									<td>
										<xsl:choose>
											<xsl:when test="(@refXtype='U') or (@refXtype='V') or (@refXtype='P') or @refXtype='FN' or @refXtype='TF' or @refXtype='IF'">
												<a>
													<xsl:variable name="IsExcluded">
														<xsl:call-template name="IsExcluded">
															<xsl:with-param name="xtype" select="@refXtype"/>
															<xsl:with-param name="ObjName" select="$dep"/>
														</xsl:call-template>
													</xsl:variable>
													<xsl:choose>
														<xsl:when test="$IsExcluded='1'">
															<xsl:attribute name="href">javascript:void(0)</xsl:attribute>
														</xsl:when>
														<xsl:otherwise>
															<xsl:attribute name="href"><xsl:choose><xsl:when test="(@refXtype='U')">tbl_</xsl:when><xsl:when test="(@refXtype='V')">vw_</xsl:when><xsl:when test="(@refXtype='P')">sp_</xsl:when><xsl:when test="@refXtype='FN' or @refXtype='TF' or @refXtype='IF'">udf_</xsl:when></xsl:choose><xsl:value-of select="translate(string($dep),'. ','')"/>.htm</xsl:attribute>
														</xsl:otherwise>
													</xsl:choose>
													<xsl:value-of select="$dep"/>
												</a>
											</xsl:when>
											<xsl:when test="@refXtype='C'">
												<xsl:value-of select="$dep"/>
											</xsl:when>
											<xsl:otherwise>
												<!-- 
												unknown refxtype.  must be a custom object 
												-->
												<a>
													<xsl:attribute name="href"><xsl:value-of select="@refXtype"/>_<xsl:value-of select="translate(string($dep),'. ','')"/>.htm</xsl:attribute>
													<xsl:value-of select="$dep"/>
												</a>
											</xsl:otherwise>
										</xsl:choose>
									</td>
								</tr>
							</xsl:if>
						</xsl:for-each>
					</table>
				</td>
			</tr>
		</xsl:if>
	</xsl:template>
	<!--
	
	
	for drawing a nifty little legend in the upper left corner.
	-->
	<xsl:template name="DrawLegend">
		<table style="font-size:100%">
			<tbody>
				<tr>
					<td colSpan="2">
						<b style="font-size: 115%">Legend</b>
					</td>
				</tr>
				<tr>
					<td>
						<v:shape>
							<xsl:attribute name="title">table</xsl:attribute>
							<xsl:attribute name="fillcolor"><xsl:value-of select="$TableColor"/></xsl:attribute>
							<xsl:attribute name="strokecolor"><xsl:value-of select="$TableColor"/></xsl:attribute>
							<xsl:attribute name="style">behavior:url(#default#VML);top:20;left:20;width:10;height:10;</xsl:attribute>
							<xsl:attribute name="path">m 500,500 l 500,-500, -500,-500, -500,500, 500,500 xe</xsl:attribute>
							<v:fill method="linear sigma" angle="-135" type="gradient" style="behavior:url(#default#VML);"/>
						</v:shape>
					</td>
					<td>
						<span title="table" style="top:15px;left:30px">table</span>
					</td>
				</tr>
				<tr>
					<td>
						<v:shape>
							<xsl:attribute name="title">view</xsl:attribute>
							<xsl:attribute name="fillcolor"><xsl:value-of select="$ViewColor"/></xsl:attribute>
							<xsl:attribute name="strokecolor"><xsl:value-of select="$ViewColor"/></xsl:attribute>
							<xsl:attribute name="style">behavior:url(#default#VML);top:40;left:20;width:10;height:10;</xsl:attribute>
							<xsl:attribute name="path">m 500,500 l 500,-500, -500,-500, -500,500, 500,500 xe</xsl:attribute>
							<v:fill method="linear sigma" angle="-135" type="gradient" style="behavior:url(#default#VML);"/>
						</v:shape>
					</td>
					<td>
						<span title="view" style="top:35px;left:30px">view</span>
					</td>
				</tr>
				<tr>
					<td>
						<v:shape>
							<xsl:attribute name="title">sproc</xsl:attribute>
							<xsl:attribute name="fillcolor"><xsl:value-of select="$SprocColor"/></xsl:attribute>
							<xsl:attribute name="strokecolor"><xsl:value-of select="$SprocColor"/></xsl:attribute>
							<xsl:attribute name="style">behavior:url(#default#VML);top:55;left:20;width:10;height:10;</xsl:attribute>
							<xsl:attribute name="path">m 500,500 l 500,-500, -500,-500, -500,500, 500,500 xe</xsl:attribute>
							<v:fill method="linear sigma" angle="-135" type="gradient" style="behavior:url(#default#VML);"/>
						</v:shape>
					</td>
					<td>
						<span title="sproc" style="top:50px;left:30px">sproc</span>
					</td>
				</tr>
				<tr>
					<td>
						<v:shape>
							<xsl:attribute name="title">udf</xsl:attribute>
							<xsl:attribute name="fillcolor"><xsl:value-of select="$UdfColor"/></xsl:attribute>
							<xsl:attribute name="strokecolor"><xsl:value-of select="$UdfColor"/></xsl:attribute>
							<xsl:attribute name="style">behavior:url(#default#VML);top:70;left:20;width:10;height:10;</xsl:attribute>
							<xsl:attribute name="path">m 500,500 l 500,-500, -500,-500, -500,500, 500,500 xe</xsl:attribute>
							<v:fill method="linear sigma" angle="-135" type="gradient" style="behavior:url(#default#VML);"/>
						</v:shape>
					</td>
					<td>
						<span title="udf" style="top:65px;left:30px">udf</span>
					</td>
				</tr>
				<tr>
					<td>
						<v:shape>
							<xsl:attribute name="title">constraint</xsl:attribute>
							<xsl:attribute name="fillcolor"><xsl:value-of select="$CheckColor"/></xsl:attribute>
							<xsl:attribute name="strokecolor"><xsl:value-of select="$CheckColor"/></xsl:attribute>
							<xsl:attribute name="style">behavior:url(#default#VML);top:85;left:20;width:10;height:10;</xsl:attribute>
							<xsl:attribute name="path">m 500,500 l 500,-500, -500,-500, -500,500, 500,500 xe</xsl:attribute>
							<v:fill method="linear sigma" angle="-135" type="gradient" style="behavior:url(#default#VML);"/>
						</v:shape>
					</td>
					<td>
						<span title="constraint" style="top:80px;left:30px">constraint</span>
					</td>
				</tr>
				<tr>
					<td>
						<v:shape>
							<xsl:attribute name="title">trigger</xsl:attribute>
							<xsl:attribute name="fillcolor"><xsl:value-of select="$TriggerColor"/></xsl:attribute>
							<xsl:attribute name="strokecolor"><xsl:value-of select="$TriggerColor"/></xsl:attribute>
							<xsl:attribute name="style">behavior:url(#default#VML);top:85;left:20;width:10;height:10;</xsl:attribute>
							<xsl:attribute name="path">m 500,500 l 500,-500, -500,-500, -500,500, 500,500 xe</xsl:attribute>
							<v:fill method="linear sigma" angle="-135" type="gradient" style="behavior:url(#default#VML);"/>
						</v:shape>
					</td>
					<td>
						<span title="trigger" style="top:80px;left:30px">trigger</span>
					</td>
				</tr>
				<xsl:for-each select="/database/DBSpecGen/definitions/object">
					<xsl:sort select="@name"/>
					<tr>
						<td>
							<v:shape>
								<xsl:attribute name="title"><xsl:value-of select="@name"/></xsl:attribute>
								<xsl:attribute name="fillcolor"><xsl:value-of select="@color"/></xsl:attribute>
								<xsl:attribute name="strokecolor"><xsl:value-of select="@color"/></xsl:attribute>
								<xsl:attribute name="style">behavior:url(#default#VML);top:85;left:20;width:10;height:10;</xsl:attribute>
								<xsl:attribute name="path">m 500,500 l 500,-500, -500,-500, -500,500, 500,500 xe</xsl:attribute>
								<v:fill method="linear sigma" angle="-135" type="gradient" style="behavior:url(#default#VML);"/>
							</v:shape>
						</td>
						<td>
							<span style="top:80px;left:30px">
								<xsl:attribute name="title"><xsl:value-of select="@name"/></xsl:attribute>
								<xsl:value-of select="@name"/>
							</span>
						</td>
					</tr>
				</xsl:for-each>
			</tbody>
		</table>
	</xsl:template>
	<!--
	
	
	
	
	this template is used for drawing the little globes representing the entities in the diagram,
	as well as the little lines connecting the globes.
	-->
	<xsl:template name="DrawEntity">
		<xsl:param name="xParentPos"/>
		<xsl:param name="yParentPos"/>
		<xsl:param name="xPos"/>
		<xsl:param name="yPos"/>
		<xsl:param name="Color"/>
		<xsl:param name="DisplayText"/>
		<xsl:param name="ObjectName"/>
		<xsl:param name="IsFocus"/>
		<xsl:param name="IsBold"/>
		<xsl:param name="zIndex"/>
		<xsl:param name="LocalMaxLabelLength"/>
		<xsl:param name="xtype"/>
		<xsl:param name="Title"/>
		<xsl:param name="LineTitle"/>
		<xsl:param name="UrlPrefix"/>
		<xsl:param name="FillColor">
			<xsl:choose>
				<xsl:when test="string-length($Color)!=0">$Color</xsl:when>
				<xsl:when test="($xtype='U')">
					<xsl:value-of select="$TableColor"/>
				</xsl:when>
				<xsl:when test="($xtype='V')">
					<xsl:value-of select="$ViewColor"/>
				</xsl:when>
				<xsl:when test="($xtype='P')">
					<xsl:value-of select="$SprocColor"/>
				</xsl:when>
				<xsl:when test="$xtype='C'">
					<xsl:value-of select="$CheckColor"/>
				</xsl:when>
				<xsl:when test="$xtype='FN' or $xtype='IF' or $xtype='TF'">
					<xsl:value-of select="$UdfColor"/>
				</xsl:when>
				<xsl:when test="$xtype='TR'">
					<xsl:value-of select="$TriggerColor"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="/database/DBSpecGen/definitions/object[@xtype=$xtype]/@color"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:param>
		<a>
			<xsl:choose>
				<xsl:when test="$xtype='C'">
					<xsl:attribute name="name">check<xsl:value-of select="translate($ObjectName,'. ','')"/></xsl:attribute>
				</xsl:when>
				<xsl:when test="$xtype='TR'">
					<!-- need to find out what table this trigger belongs to. -->
					<xsl:variable name="tableName">
						<xsl:value-of select="/database/table/trigger[@name=$ObjectName]/../@name"/>
					</xsl:variable>
					<xsl:variable name="tableType">
						<xsl:value-of select="/database/table/trigger[@name=$ObjectName]/../@xtype"/>
					</xsl:variable>
					<xsl:variable name="IsExcluded">
						<xsl:call-template name="IsExcluded">
							<xsl:with-param name="xtype" select="$tableType"/>
							<xsl:with-param name="ObjName" select="$tableName"/>
						</xsl:call-template>
					</xsl:variable>
					<xsl:choose>
						<xsl:when test="$IsExcluded='1'">
							<xsl:attribute name="href">javascript:void(0)</xsl:attribute>
						</xsl:when>
						<xsl:otherwise>
							<xsl:attribute name="href"><xsl:value-of select="$UrlPrefix"/><xsl:choose><xsl:when test="$tableType='U'">tbl_</xsl:when><xsl:when test="$tableType='V'">vw_</xsl:when></xsl:choose><xsl:value-of select="translate($tableName,'. ','')"/>.htm#TRIGGERS</xsl:attribute>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:when>
				<xsl:when test="($xtype='U') or ($xtype='V') or ($xtype='P') or $xtype='FN' or $xtype='TF' or $xtype='IF'">
					<xsl:variable name="IsExcluded">
						<xsl:call-template name="IsExcluded">
							<xsl:with-param name="xtype" select="$xtype"/>
							<xsl:with-param name="ObjName" select="$ObjectName"/>
						</xsl:call-template>
					</xsl:variable>
					<xsl:choose>
						<xsl:when test="$IsExcluded != '1'">
							<xsl:attribute name="href"><xsl:value-of select="$IsExcluded"/><xsl:value-of select="$UrlPrefix"/><xsl:choose><xsl:when test="($xtype='U')">tbl_</xsl:when><xsl:when test="($xtype='V')">vw_</xsl:when><xsl:when test="($xtype='P')">sp_</xsl:when><xsl:when test="$xtype='FN' or $xtype='IF' or $xtype='TF'">udf_</xsl:when></xsl:choose><xsl:value-of select="translate($ObjectName,'. ','')"/>.htm</xsl:attribute>
						</xsl:when>
						<xsl:otherwise>
							<xsl:attribute name="href">javascript:void(0)</xsl:attribute>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:when>
				<xsl:otherwise>
					<!-- 
					must be a custom object 
					-->
					<xsl:attribute name="href"><xsl:value-of select="$UrlPrefix"/><xsl:value-of select="translate($xtype,'. ','')"/>_<xsl:value-of select="translate($ObjectName,'. ','')"/>.htm</xsl:attribute>
				</xsl:otherwise>
			</xsl:choose>
			<v:shape>
				<xsl:choose>
					<xsl:when test="string-length($Title)!=0">
						<xsl:attribute name="title"><xsl:value-of select="$Title"/></xsl:attribute>
					</xsl:when>
					<xsl:otherwise>
						<xsl:attribute name="title"><xsl:choose><xsl:when test="$xtype='TR'">trigger: </xsl:when><xsl:when test="($xtype='U')">table: </xsl:when><xsl:when test="($xtype='V')">view: </xsl:when><xsl:when test="($xtype='P')">stored procedure: </xsl:when><xsl:when test="$xtype='C'">constraint: </xsl:when><xsl:when test="$xtype='FN' or $xtype='IF' or $xtype='TF'">user defined function: </xsl:when><xsl:otherwise><xsl:value-of select="/database/DBSpecGen/definitions/object[@xtype=$xtype]/@name"/>: </xsl:otherwise></xsl:choose><xsl:value-of select="$DisplayText"/></xsl:attribute>
					</xsl:otherwise>
				</xsl:choose>
				<xsl:attribute name="fillcolor"><xsl:value-of select="$FillColor"/></xsl:attribute>
				<xsl:attribute name="strokecolor"><xsl:value-of select="$FillColor"/></xsl:attribute>
				<xsl:attribute name="style">position:absolute;cursor:hand;z-index:<xsl:value-of select="$zIndex"/>;behavior:url(#default#VML);top:<xsl:value-of select="$yPos"/>;left:<xsl:value-of select="$xPos"/>;width:10;height:10;</xsl:attribute>
				<xsl:choose>
					<xsl:when test="$IsFocus='true'">
						<xsl:attribute name="path">m 1000,1000 l 1000,-1000, -1000,-1000, -1000,1000, 1000,1000 xe</xsl:attribute>
					</xsl:when>
					<xsl:otherwise>
						<xsl:attribute name="path">m 750,750 l 750,-750, -750,-750, -750,750, 750,750 xe</xsl:attribute>
					</xsl:otherwise>
				</xsl:choose>
				<v:fill method="linear sigma" angle="-135" type="gradient" style="behavior:url(#default#VML);"/>
				<span>
					<xsl:choose>
						<xsl:when test="string-length($Title)!=0">
							<xsl:attribute name="title"><xsl:value-of select="$Title"/></xsl:attribute>
						</xsl:when>
						<xsl:otherwise>
							<xsl:attribute name="title"><xsl:choose><xsl:when test="$xtype='TR'">trigger: </xsl:when><xsl:when test="($xtype='U')">table: </xsl:when><xsl:when test="($xtype='V')">view: </xsl:when><xsl:when test="($xtype='P')">stored procedure: </xsl:when><xsl:when test="$xtype='C'">constraint: </xsl:when><xsl:when test="$xtype='FN' or $xtype='IF' or $xtype='TF'">user defined function: </xsl:when></xsl:choose><xsl:value-of select="$DisplayText"/></xsl:attribute>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:attribute name="style">position:relative;<xsl:choose><xsl:when test="$IsFocus='true'">top:22;left:0;</xsl:when><xsl:otherwise>top:17;left:0;</xsl:otherwise></xsl:choose>cursor:hand;font-family:verdana;font-size:10;z-index:<xsl:value-of select="$zIndex - 1"/>;background-color:white;</xsl:attribute>
					<xsl:choose>
						<xsl:when test="string-length($DisplayText) &gt; $LocalMaxLabelLength">
							<xsl:value-of select="concat(substring($DisplayText,1,$LocalMaxLabelLength),'...')"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="$DisplayText"/>
						</xsl:otherwise>
					</xsl:choose>
				</span>
			</v:shape>
		</a>
		<!-- draw the connecting lines -->
		<xsl:if test="string-length($xParentPos)!=0 and string-length($yParentPos)!=0">
			<v:shape strokeweight="1px" style="position:absolute;behavior:url(#default#VML);z-index:-1000;height:1000;width:1000;top:0;left:0;">
				<xsl:attribute name="strokecolor">gray</xsl:attribute>
				<xsl:attribute name="path">m <xsl:value-of select="$xParentPos"/>,<xsl:value-of select="$yParentPos"/> l <xsl:value-of select="$xPos"/>,<xsl:value-of select="$yPos"/> xe</xsl:attribute>
				<xsl:if test="string-length($LineTitle)!=0">
					<xsl:attribute name="title"><xsl:value-of select="$LineTitle"/></xsl:attribute>
				</xsl:if>
			</v:shape>
		</xsl:if>
	</xsl:template>
	<!--
	
	
	
	
	
	
	
	
	
	
	for drawing a graph of an objects dependencies in VML
	-->
	<xsl:template name="DrawDependencyGraph">
		<xsl:variable name="objName">
			<xsl:value-of select="@name"/>
		</xsl:variable>
		<xsl:variable name="NumParents">
			<xsl:value-of select="count(/database/dependency[@dependsOnObj=$objName and string-length(@objName)!=0])"/>
		</xsl:variable>
		<xsl:variable name="NumChildren">
			<xsl:value-of select="count(/database/dependency[@objName=$objName and string-length(@dependsOnObj)!=0])"/>
		</xsl:variable>
		<xsl:variable name="dbName">
			<xsl:value-of select="/database/@name"/>
		</xsl:variable>
		<xsl:variable name="serverName">
			<xsl:value-of select="/database/@server"/>
		</xsl:variable>
		<xsl:if test="$NumParents &gt; 0 or $NumChildren &gt; 0">
			<xsl:variable name="PageWidth">700</xsl:variable>
			<xsl:variable name="ySpacer">50</xsl:variable>
			<xsl:variable name="xOffset">-40</xsl:variable>
			<xsl:variable name="yOffset">-10</xsl:variable>
			<!-- calculate the x,y position of the focused object -->
			<xsl:value-of select="db:SetValue('xFocus', $PageWidth div 2 + $xOffset)"/>
			<xsl:value-of select="db:SetValue('yFocus', ceiling($NumParents div $ItemsPerRow) * $ySpacer + $yOffset)"/>
			<h2 class="dtH2">Dependency graph</h2>
			<table style="font-size:100%">
				<tr valign="top">
					<td valign="top" align="center">
						<v:group coordsize="1000px,1000px" coordorig="0px,0px">
							<xsl:attribute name="style">position:relative;top:20px;left:0px;width:<xsl:value-of select="$PageWidth"/>;height:<xsl:value-of select="(ceiling($NumChildren div $ItemsPerRow) + ceiling($NumParents div $ItemsPerRow) + 1) * $ySpacer"/></xsl:attribute>
							<!-- draw parents -->
							<xsl:if test="$NumParents &gt; 0">
								<xsl:variable name="NumParentCols">
									<xsl:choose>
										<xsl:when test="$NumParents &gt; $ItemsPerRow">
											<xsl:value-of select="$ItemsPerRow"/>
										</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="$NumParents"/>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:variable>
								<!-- calculate spacer for x-spacing -->
								<xsl:variable name="xSpacer">
									<xsl:value-of select="$PageWidth div $NumParentCols"/>
								</xsl:variable>
								<xsl:value-of select="db:SetValue('x', ($PageWidth div 2) - ($xSpacer * (($NumParentCols - 1) div 2)) + $xOffset)"/>
								<xsl:value-of select="db:SetValue('y', number($yOffset) + 0)"/>
								<xsl:for-each select="/database/dependency[@dependsOnObj=$objName and string-length(@objName)!=0]">
									<xsl:sort select="@xtype"/>
									<xsl:sort select="@objName"/>
									<xsl:if test="(position() - 1) mod $ItemsPerRow = 0 and position() &gt; $ItemsPerRow">
										<xsl:value-of select="db:SetValue('x',($PageWidth div 2) - ($xSpacer * (($NumParentCols - 1) div 2)) + $xOffset)"/>
										<xsl:value-of select="db:SetValue('y',db:GetValue('y') + $ySpacer)"/>
									</xsl:if>
									<xsl:variable name="UrlPrefix">
										<xsl:choose>
											<xsl:when test="(@xtype='C') or @xtype='TR' or (@xtype='U') or (@xtype='V') or (@xtype='P') or @xtype='FN' or @xtype='TF' or @xtype='IF'">
												<xsl:choose>
													<xsl:when test="string-length($dbName)!=0 and string-length($serverName)!=0">../<xsl:value-of select="translate($serverName,'\','.')"/>.<xsl:value-of select="$dbName"/>/</xsl:when>
													<xsl:otherwise>../<xsl:value-of select="translate(@server,'\','.')"/>.<xsl:value-of select="@database"/>/</xsl:otherwise>
												</xsl:choose>
											</xsl:when>
											<xsl:otherwise>
												<xsl:value-of select="'../ExternalObjects/'"/>
											</xsl:otherwise>
										</xsl:choose>
									</xsl:variable>
									<xsl:value-of select="db:SetValue('zIndexCounter', db:GetValue('zIndexCounter')-1)"/>
									<xsl:call-template name="DrawEntity">
										<xsl:with-param name="xPos" select="floor(db:GetValue('x'))"/>
										<xsl:with-param name="yPos" select="floor(db:GetValue('y'))"/>
										<xsl:with-param name="xParentPos" select="db:GetValue('xFocus')"/>
										<xsl:with-param name="yParentPos" select="db:GetValue('yFocus')"/>
										<xsl:with-param name="IsFocus">false</xsl:with-param>
										<xsl:with-param name="DisplayText" select="@objName"/>
										<xsl:with-param name="ObjectName" select="@objName"/>
										<xsl:with-param name="xtype" select="@xtype"/>
										<xsl:with-param name="LocalMaxLabelLength" select="$MaxLabelLength"/>
										<xsl:with-param name="UrlPrefix" select="$UrlPrefix"/>
										<xsl:with-param name="zIndex">10</xsl:with-param>
									</xsl:call-template>
									<xsl:value-of select="db:SetValue('x',db:GetValue('x') + $xSpacer)"/>
								</xsl:for-each>
							</xsl:if>
							<!-- draw focused object -->
							<xsl:call-template name="DrawEntity">
								<xsl:with-param name="xPos" select="db:GetValue('xFocus')"/>
								<xsl:with-param name="yPos" select="db:GetValue('yFocus')"/>
								<xsl:with-param name="IsFocus">true</xsl:with-param>
								<xsl:with-param name="DisplayText" select="$objName"/>
								<xsl:with-param name="ObjectName" select="$objName"/>
								<xsl:with-param name="xtype" select="@xtype"/>
								<xsl:with-param name="LocalMaxLabelLength" select="255"/>
								<xsl:with-param name="zIndex">10</xsl:with-param>
							</xsl:call-template>
							<!-- draw children -->
							<xsl:if test="$NumChildren &gt; 0">
								<xsl:variable name="NumChildCols">
									<xsl:choose>
										<xsl:when test="$NumChildren &gt; $ItemsPerRow">
											<xsl:value-of select="$ItemsPerRow"/>
										</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="$NumChildren"/>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:variable>
								<!-- calculate spacer for x-spacing -->
								<xsl:variable name="xSpacer">
									<xsl:value-of select="$PageWidth div $NumChildCols"/>
								</xsl:variable>
								<xsl:value-of select="db:SetValue('x', ($PageWidth div 2) - ($xSpacer * (($NumChildCols - 1) div 2)) + $xOffset)"/>
								<xsl:value-of select="db:SetValue('y', db:GetValue('yFocus') + $ySpacer)"/>
								<xsl:for-each select="/database/dependency[@objName=$objName and string-length(@dependsOnObj)!=0]">
									<xsl:sort select="@refXtype"/>
									<xsl:sort select="@dependsOnObj"/>
									<xsl:if test="(position() - 1) mod $ItemsPerRow = 0 and position() &gt; $ItemsPerRow">
										<xsl:value-of select="db:SetValue('x',($PageWidth div 2) - ($xSpacer * (($NumChildCols - 1) div 2)) + $xOffset)"/>
										<xsl:value-of select="db:SetValue('y',db:GetValue('y') + $ySpacer)"/>
									</xsl:if>
									<xsl:variable name="UrlPrefix">
										<xsl:choose>
											<xsl:when test="(@refXtype='C') or @refXtype='TR' or (@refXtype='U') or (@refXtype='V') or (@refXtype='P') or @refXtype='FN' or @refXtype='TF' or @refXtype='IF'">
												<xsl:choose>
													<xsl:when test="string-length($dbName)!=0 and string-length($serverName)!=0">../<xsl:value-of select="translate($serverName, '\', '.')"/>.<xsl:value-of select="$dbName"/>/</xsl:when>
													<xsl:otherwise>../<xsl:value-of select="translate(@server, '\', '.')"/>.<xsl:value-of select="@database"/>/</xsl:otherwise>
												</xsl:choose>
											</xsl:when>
											<xsl:otherwise>
												<xsl:value-of select="'../ExternalObjects/'"/>
											</xsl:otherwise>
										</xsl:choose>
									</xsl:variable>
									<xsl:value-of select="db:SetValue('zIndexCounter', db:GetValue('zIndexCounter')-1)"/>
									<xsl:call-template name="DrawEntity">
										<xsl:with-param name="xPos" select="floor(db:GetValue('x'))"/>
										<xsl:with-param name="yPos" select="floor(db:GetValue('y'))"/>
										<xsl:with-param name="xParentPos" select="db:GetValue('xFocus')"/>
										<xsl:with-param name="yParentPos" select="db:GetValue('yFocus')"/>
										<xsl:with-param name="IsFocus">false</xsl:with-param>
										<xsl:with-param name="DisplayText" select="@dependsOnObj"/>
										<xsl:with-param name="ObjectName" select="@dependsOnObj"/>
										<xsl:with-param name="xtype" select="@refXtype"/>
										<xsl:with-param name="LocalMaxLabelLength" select="$MaxLabelLength"/>
										<xsl:with-param name="UrlPrefix" select="$UrlPrefix"/>
										<xsl:with-param name="zIndex">10</xsl:with-param>
									</xsl:call-template>
									<xsl:value-of select="db:SetValue('x',db:GetValue('x') + $xSpacer)"/>
								</xsl:for-each>
							</xsl:if>
						</v:group>
					</td>
					<td valign="top" width="10%">
						<xsl:call-template name="DrawLegend"/>
					</td>
				</tr>
			</table>
		</xsl:if>
	</xsl:template>
	<!--
	
	
	
	
	
	
	
	
	draw a pk/fk reference graph for a table in VML
	-->
	<xsl:template name="DrawReferenceGraph">
		<xsl:variable name="objName">
			<xsl:value-of select="@name"/>
		</xsl:variable>
		<xsl:variable name="NumParents">
			<xsl:value-of select="count(/database/table[@name=$objName]/constraint[@isForeignKey=1])"/>
		</xsl:variable>
		<xsl:variable name="NumChildren">
			<xsl:value-of select="count(/database/table/constraint[@isForeignKey=1 and @refTable=$objName])"/>
		</xsl:variable>
		<xsl:if test="$NumParents &gt; 0 or $NumChildren &gt; 0">
			<xsl:variable name="PageWidth">700</xsl:variable>
			<xsl:variable name="ySpacer">50</xsl:variable>
			<xsl:variable name="xOffset">-40</xsl:variable>
			<xsl:variable name="yOffset">-10</xsl:variable>
			<!-- calculate the x,y position of the focused object -->
			<xsl:value-of select="db:SetValue('xFocus', ($PageWidth div 2) + $xOffset)"/>
			<xsl:value-of select="db:SetValue('yFocus', ((ceiling($NumParents div $ItemsPerRow) * $ySpacer) + $yOffset))"/>
			<h2 class="dtH2">Foreign key graph</h2>
			<table>
				<tr valign="top">
					<td valign="top" align="center">
						<v:group coordsize="1000px,1000px" coordorig="0px,0px">
							<xsl:attribute name="style">position:relative;top:20px;left:0px;width:<xsl:value-of select="$PageWidth"/>;height:<xsl:value-of select="(ceiling($NumChildren div $ItemsPerRow) + ceiling($NumParents div $ItemsPerRow) + 1) * $ySpacer"/></xsl:attribute>
							<!-- draw parents -->
							<xsl:if test="$NumParents &gt; 0">
								<xsl:variable name="NumParentCols">
									<xsl:choose>
										<xsl:when test="$NumParents &gt; $ItemsPerRow">
											<xsl:value-of select="$ItemsPerRow"/>
										</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="$NumParents"/>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:variable>
								<!-- calculate spacer for x-spacing -->
								<xsl:variable name="xSpacer">
									<xsl:value-of select="$PageWidth div $NumParentCols"/>
								</xsl:variable>
								<xsl:value-of select="db:SetValue('x', ($PageWidth div 2) - ($xSpacer * (($NumParentCols - 1) div 2)) + $xOffset)"/>
								<xsl:value-of select="db:SetValue('y', number($yOffset) + 0)"/>
								<xsl:for-each select="/database/table[@name=$objName]/constraint[@isForeignKey=1]">
									<xsl:sort select="@refTable" data-type="text"/>
									<xsl:if test="(position() - 1) mod $NumParentCols = 0 and position() &gt; $NumParentCols">
										<xsl:value-of select="db:SetValue('x', (($PageWidth div 2) - ($xSpacer * (($NumParentCols - 1) div 2)) + $xOffset))"/>
										<xsl:value-of select="db:SetValue('y', (number(db:GetValue('y')) + $ySpacer))"/>
									</xsl:if>
									<xsl:value-of select="db:SetValue('zIndexCounter', db:GetValue('zIndexCounter')-1)"/>
									<xsl:call-template name="DrawEntity">
										<xsl:with-param name="xPos" select="floor(db:GetValue('x'))"/>
										<xsl:with-param name="yPos" select="floor(db:GetValue('y'))"/>
										<xsl:with-param name="xParentPos" select="db:GetValue('xFocus')"/>
										<xsl:with-param name="yParentPos" select="db:GetValue('yFocus')"/>
										<xsl:with-param name="IsFocus">false</xsl:with-param>
										<xsl:with-param name="DisplayText" select="@refTable"/>
										<xsl:with-param name="ObjectName" select="@refTable"/>
										<xsl:with-param name="xtype" select="'U'"/>
										<xsl:with-param name="LocalMaxLabelLength" select="$MaxLabelLength"/>
										<xsl:with-param name="zIndex">10</xsl:with-param>
										<xsl:with-param name="Title">
											<xsl:value-of select="concat(../@name,'.',@colName,' --&gt; ',@refTable,'.',@refColumn)"/>
										</xsl:with-param>
										<xsl:with-param name="LineTitle">
											<xsl:value-of select="concat(../@name,'.',@colName,' --&gt; ',@refTable,'.',@refColumn)"/>
										</xsl:with-param>
									</xsl:call-template>
									<xsl:value-of select="db:SetValue('x',db:GetValue('x') + $xSpacer)"/>
								</xsl:for-each>
							</xsl:if>
							<!-- draw focused object -->
							<xsl:call-template name="DrawEntity">
								<xsl:with-param name="xPos" select="db:GetValue('xFocus')"/>
								<xsl:with-param name="yPos" select="db:GetValue('yFocus')"/>
								<xsl:with-param name="IsFocus">true</xsl:with-param>
								<xsl:with-param name="DisplayText" select="$objName"/>
								<xsl:with-param name="ObjectName" select="$objName"/>
								<xsl:with-param name="xtype" select="'U'"/>
								<xsl:with-param name="LocalMaxLabelLength" select="255"/>
								<xsl:with-param name="zIndex">10</xsl:with-param>
							</xsl:call-template>
							<!-- draw children -->
							<xsl:if test="$NumChildren &gt; 0">
								<xsl:variable name="NumChildCols">
									<xsl:choose>
										<xsl:when test="$NumChildren &gt; $ItemsPerRow">
											<xsl:value-of select="$ItemsPerRow"/>
										</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="$NumChildren"/>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:variable>
								<!-- calculate spacer for x-spacing -->
								<xsl:variable name="xSpacer">
									<xsl:value-of select="$PageWidth div $NumChildCols"/>
								</xsl:variable>
								<xsl:value-of select="db:SetValue('x', ($PageWidth div 2) - ($xSpacer * (($NumChildCols - 1) div 2)) + $xOffset)"/>
								<xsl:value-of select="db:SetValue('y', number(db:GetValue('yFocus')) + $ySpacer)"/>
								<xsl:for-each select="/database/table/constraint[@isForeignKey=1 and @refTable=$objName]">
									<xsl:sort select="../@name"/>
									<xsl:if test="(position() - 1) mod $ItemsPerRow = 0 and position() &gt; $ItemsPerRow">
										<xsl:value-of select="db:SetValue('x', number(($PageWidth div 2) - ($xSpacer * (($NumChildCols - 1) div 2)) + $xOffset))"/>
										<xsl:value-of select="db:SetValue('y', number(number(db:GetValue('y')) + $ySpacer))"/>
									</xsl:if>
									<xsl:value-of select="db:SetValue('zIndexCounter', db:GetValue('zIndexCounter')-1)"/>
									<xsl:call-template name="DrawEntity">
										<xsl:with-param name="xPos" select="floor(db:GetValue('x'))"/>
										<xsl:with-param name="yPos" select="floor(db:GetValue('y'))"/>
										<xsl:with-param name="xParentPos" select="db:GetValue('xFocus')"/>
										<xsl:with-param name="yParentPos" select="db:GetValue('yFocus')"/>
										<xsl:with-param name="IsFocus">false</xsl:with-param>
										<xsl:with-param name="DisplayText" select="../@name"/>
										<xsl:with-param name="ObjectName" select="../@name"/>
										<xsl:with-param name="xtype" select="'U'"/>
										<xsl:with-param name="LocalMaxLabelLength" select="$MaxLabelLength"/>
										<xsl:with-param name="zIndex">10</xsl:with-param>
										<xsl:with-param name="Title">
											<xsl:value-of select="concat(../@name,'.',@colName,' --&gt; ',@refTable,'.',@refColumn)"/>
										</xsl:with-param>
										<xsl:with-param name="LineTitle">
											<xsl:value-of select="concat(../@name,'.',@colName,' --&gt; ',@refTable,'.',@refColumn)"/>
										</xsl:with-param>
									</xsl:call-template>
									<xsl:value-of select="db:SetValue('x',db:GetValue('x') + $xSpacer)"/>
								</xsl:for-each>
							</xsl:if>
						</v:group>
					</td>
				</tr>
			</table>
		</xsl:if>
	</xsl:template>
	<!--
	
	
	
	
	
	
	
	see if an object is excluded from the documentation
	-->
	<xsl:template name="IsExcluded">
		<xsl:param name="ServerName">
			<xsl:value-of select="/database/@server"/>
		</xsl:param>
		<xsl:param name="DatabaseName">
			<xsl:value-of select="/database/@name"/>
		</xsl:param>
		<xsl:param name="ObjName"/>
		<xsl:param name="xtype"/>
		<xsl:for-each select="/database/DBSpecGen/exclude/server[@name=$ServerName]/database[@name=$DatabaseName]">
			<xsl:choose>
				<xsl:when test="$xtype='U' and table[@name=$ObjName]">1</xsl:when>
				<xsl:when test="$xtype='V' and view[@name=$ObjName]">1</xsl:when>
				<xsl:when test="$xtype='P' and sproc[@name=$ObjName]">1</xsl:when>
				<xsl:when test="$xtype='FN' and udf[@name=$ObjName]">1</xsl:when>
				<xsl:when test="$xtype='IF' and udf[@name=$ObjName]">1</xsl:when>
				<xsl:when test="$xtype='TF' and udf[@name=$ObjName]">1</xsl:when>
			</xsl:choose>
		</xsl:for-each>
	</xsl:template>
	<!--
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	draws the tree used for the html output.  
	not needed for chm output.
	-->
	<xsl:template name="drawTree">
		<xsl:param name="dbName"/>
		<xsl:param name="serverName"/>
		<tree>
			<xsl:if test="count(table[(@xtype='U') and not(@name = /database/DBSpecGen/exclude/server[@name=$serverName]/database[@name=$dbName]/table/@name)])">
				<page id="cntTables">
					<xsl:attribute name="title"><xsl:value-of select="/database/@name"/> tables</xsl:attribute>
					<xsl:for-each select="table[(@xtype='U')]">
						<xsl:sort select="@name" data-type="text"/>
						<xsl:if test="not(@name = /database/DBSpecGen/exclude/server[@name=$serverName]/database[@name=$dbName]/table/@name)">
							<page>
								<xsl:attribute name="id">tbl_<xsl:value-of select="translate(string(@name),'. ','')"/></xsl:attribute>
								<xsl:attribute name="title"><xsl:value-of select="@name"/></xsl:attribute>
							</page>
						</xsl:if>
					</xsl:for-each>
				</page>
			</xsl:if>
			<xsl:if test="count(table[(@xtype='V') and not(@name = /database/DBSpecGen/exclude/server[@name=$serverName]/database[@name=$dbName]/view/@name)])">
				<page id="cntViews">
					<xsl:attribute name="title"><xsl:value-of select="/database/@name"/> views</xsl:attribute>
					<xsl:for-each select="table[(@xtype='V')]">
						<xsl:sort select="@name" data-type="text"/>
						<xsl:if test="not(@name = /database/DBSpecGen/exclude/server[@name=$serverName]/database[@name=$dbName]/view/@name)">
							<page>
								<xsl:attribute name="id">vw_<xsl:value-of select="translate(string(@name),'. ','')"/></xsl:attribute>
								<xsl:attribute name="title"><xsl:value-of select="@name"/></xsl:attribute>
							</page>
						</xsl:if>
					</xsl:for-each>
				</page>
			</xsl:if>
			<xsl:if test="count(procedure[(@xtype='P') and not(@name = /database/DBSpecGen/exclude/server[@name=$serverName]/database[@name=$dbName]/sproc/@name)])">
				<page id="cntSprocs">
					<xsl:attribute name="title"><xsl:value-of select="/database/@name"/> stored procedures</xsl:attribute>
					<xsl:for-each select="procedure[(@xtype='P')]">
						<xsl:sort select="@name" data-type="text"/>
						<xsl:if test="not(@name = /database/DBSpecGen/exclude/server[@name=$serverName]/database[@name=$dbName]/sproc/@name)">
							<page>
								<xsl:attribute name="id">sp_<xsl:value-of select="translate(string(@name),'. ','')"/></xsl:attribute>
								<xsl:attribute name="title"><xsl:value-of select="@name"/></xsl:attribute>
							</page>
						</xsl:if>
					</xsl:for-each>
				</page>
			</xsl:if>
			<xsl:if test="count(procedure[(@xtype='FN' or @xtype='IF' or @xtype='TF') and not(@name = /database/DBSpecGen/exclude/server[@name=$serverName]/database[@name=$dbName]/udf/@name)])">
				<page id="cntUDFs">
					<xsl:attribute name="title"><xsl:value-of select="/database/@name"/> user defined functions</xsl:attribute>
					<xsl:for-each select="procedure[@xtype='FN' or @xtype='IF' or @xtype='TF']">
						<xsl:sort select="@name" data-type="text"/>
						<xsl:if test="not(@name = /database/DBSpecGen/exclude/server[@name=$serverName]/database[@name=$dbName]/udf/@name)">
							<page>
								<xsl:attribute name="id">udf_<xsl:value-of select="translate(string(@name),'. ','')"/></xsl:attribute>
								<xsl:attribute name="title"><xsl:value-of select="@name"/></xsl:attribute>
							</page>
						</xsl:if>
					</xsl:for-each>
				</page>
			</xsl:if>
			<xsl:if test="userDefinedType">
				<page id="cntUDTs">
					<xsl:attribute name="title"><xsl:value-of select="/database/@name"/> user defined data types</xsl:attribute>
					<xsl:for-each select="userDefinedType">
						<xsl:sort select="@name" data-type="text"/>
						<page>
							<xsl:attribute name="id">udt_<xsl:value-of select="translate(string(@name),'. ','')"/></xsl:attribute>
							<xsl:attribute name="title"><xsl:value-of select="@name"/></xsl:attribute>
						</page>
					</xsl:for-each>
				</page>
			</xsl:if>
		</tree>
	</xsl:template>
</xsl:stylesheet>
<!--
			
			table issues
			
			<page id="tableIssues">
				<xsl:attribute name="title"><xsl:value-of select="$serverName"/>.<xsl:value-of select="$dbName"/> table issues</xsl:attribute>
				<item>
					<table border="1" cellpadding="2" cellspacing="0">
						<xsl:for-each select="/database/table[(@xtype='U') and count(constraint[@isPrimaryKey='1'])=0]">
							<tr valign="top">
								<td>
									<a>
										<xsl:attribute name="href">tbl_<xsl:value-of select="translate(@name,'. ','')"/>.htm</xsl:attribute>
										<xsl:value-of select="@name"/>
									</a>
								</td>
								<td>no primary key</td>
							</tr>
						</xsl:for-each>
						<xsl:for-each select="/database/table/column">
							<xsl:if test="@allowNull=1">
								<tr valign="top">
									<td>
										<a>
											<xsl:attribute name="href">tbl_<xsl:value-of select="translate(../@name,'. ','')"/>.htm</xsl:attribute>
											<xsl:value-of select="../@name"/>
										</a>.<xsl:value-of select="@name"/>
									</td>
									<td>column allows NULL</td>
								</tr>
							</xsl:if>
						</xsl:for-each>
					</table>
				</item>
			</page>
			
			sproc issues
			
			<page id="sprocIssues">
				<xsl:attribute name="title"><xsl:value-of select="$serverName"/>.<xsl:value-of select="$dbName"/> stored procedure issues</xsl:attribute>
				<item>
					<table>
						
						dynamic sql usage
						
						<xsl:for-each select="/database/procedure/code">
							<xsl:if test="contains(translate(.,'EXC ','exc'), 'exec(') or contains(translate(.,'SPEXCUTQL','spexcutql'), 'sp_executesql')">
								<tr valign="top">
									<td>
										<a>
											<xsl:attribute name="href"><xsl:choose><xsl:when test="../@xtype='P'">sp_</xsl:when><xsl:otherwise>udf_</xsl:otherwise></xsl:choose><xsl:value-of select="translate(../@name,'. ','')"/>.htm</xsl:attribute>
											<xsl:value-of select="../@name"/>
										</a>
									</td>
									<td>dynamic sql usage</td>
								</tr>
							</xsl:if>
						</xsl:for-each>
						
						cursor usage
						
						<xsl:for-each select="/database/procedure/code">
							<xsl:if test="contains(translate(., 'DECLARE', 'declare'), 'declare') and contains(translate(., 'CURSOR', 'cursor'), 'cursor')">
								<tr valign="top">
									<td>
										<a>
											<xsl:attribute name="href"><xsl:choose><xsl:when test="../@xtype='P'">sp_</xsl:when><xsl:otherwise>udf_</xsl:otherwise></xsl:choose><xsl:value-of select="translate(../@name,'. ','')"/>.htm</xsl:attribute>
											<xsl:value-of select="../@name"/>
										</a>
									</td>
									<td>cursor usage</td>
								</tr>
							</xsl:if>
						</xsl:for-each>
						
						select * usage
						
						<xsl:for-each select="/database/procedure/code">
							<xsl:if test="contains(translate(., 'SELECT', 'select'), 'select *')">
								<tr valign="top">
									<td>
										<a>
											<xsl:attribute name="href"><xsl:choose><xsl:when test="../@xtype='P'">sp_</xsl:when><xsl:otherwise>udf_</xsl:otherwise></xsl:choose><xsl:value-of select="translate(../@name,'. ','')"/>.htm</xsl:attribute>
											<xsl:value-of select="../@name"/>
										</a>
									</td>
									<td>select * usage</td>
								</tr>
							</xsl:if>
						</xsl:for-each>
					</table>
				</item>
			</page>
			-->
