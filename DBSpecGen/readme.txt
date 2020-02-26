I got some requests to share out the source, so here it is.  I aim to please. :}   

Beware: the source located here will be replaced whenever I prop a new version.  
I put the source for older versions in the boneyard directory.

If you add some nice features, all I ask is that you tell me about it so that 
I may incorporate them for everyone to enjoy.

Look in strings.resx for the giant sql query that the tool executes to get the 
data it uses to build the docs.

The misc directory contains some xsl used to produce the pretty html output from 
the xml returned by the sql query, as well as some html/js stuff.

You should be able to build it by altering make.bat to point to resgen.exe and 
csc.exe as appropriate and typing "make" at the command line.
 
Send questions/comments to jesseh.

thanks,
Jesse 


