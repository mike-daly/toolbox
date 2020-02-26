/*

this is an example of a stored procedure with xml comments.  

all you have to do is add xml elements to your code that are 
commented out and dbspecgen will pick them up and parse them.

you can comment them out either comment style.

take a look at the element to see what's available.  

try adding this sproc to a test database and running DBSpecGen 
against it to see how the comments appear in the documentation.

*/

CREATE Procedure [dbo].[sp_SampleStoredProcedure] 
--	<scope>Public</scope> 
--	<summary> 
--		This is just a comment to give a summary of what the procedure is for 
--	</summary> 
--<parameters> 
@p1 INT = NULL, 		--<attributes required="yes" description="this parameter is for x..."/> 
@p2 float = NULL, 		--<attributes required="yes" description="this parameter is for y..."/> 
@p3 nvarchar(3000) = NULL 	--<attributes required="yes" description="this parameter is for z..."/> 
--</parameters> 

AS 

BEGIN 
PRINT 'Code Goes Here...' 
END 

/*
	<returns> 
		<return value="0" description="Success"/> 
		<return value="n" description="Failure. Where n is the error number when not described here"/> 
		<recordset> 
			<column name="ParentOrganizationName" datatype="nvarchar" datalength="20" description="blah blah"/> 
			<column name="IsPrinter" datatype="bit" datalength="" description="blah blah"/> 
		</recordset> 
		<recordset> 
			<column name="OSCode" datatype="int" description="some int"/> 
		</recordset> 
		<recordset> 
			<column name="SiteCode" datatype="int" description="blah blah"/> 
		</recordset> 
		<recordset> 
			<column name="WULanguageCode" datatype="int" description="blah blah"/> 
		</recordset> 
		<recordset> 
			<column name="HWID" datatype="int" description="blah blah"/> 
			<column name="GUID" datatype="int" description="blah blah"/> 
		</recordset> 
	</returns> 

	<historylog> 
		<log revision="1.0" date="08/18/2003" email="jesseh">created</log> 
		<log revision="1.1" date="08/19/2003" email="jesseh">modified</log> 
		<log revision="1.2" date="08/20/2003" email="jesseh">ruined</log> 
		<log revision="1.4" date="08/21/2003" email="jesseh">fixed</log>
		<log revision="1.5" date="08/22/2003" email="jesseh">refactored</log>
		<log revision="1.6" date="08/23/2003" email="jesseh">obfuscated for job security</log>
	</historylog> 

	<samples> 
		<sample>
			<description>here is some sample code</description>
			<code>exec sp_SampleSproc @DriverID=1234</code> 
		</sample>
		<sample>
			<description>some more samples...</description>
			<code>exec sp_SampleSproc @DriverID=5678</code> 
		</sample>
	</samples> 
*/



GO
