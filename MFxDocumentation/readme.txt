This tool requires the Visio 2013 SDK installed on the compiling box.

    http://www.microsoft.com/en-us/download/details.aspx?id=36825

This tool requires Visio (2013) to be installed locally.

This code has only been casually tested against the 32bit versions of Visio and the SDK.  No testing has been done against 64bit versions.



Some enterprising person could go an add the appropriate libraries to the build.


Known problems:
    can't load an assembly from a share
    can't load the silverlight version of the client library

Manual steps required:
    After the graph is rendered, you will need to do the following:
        - center the object tree
        - move any orphan objects (typically interfaces) out of the middle
            of the chart
        - ungroup the tree
        - move a sub-branch of the tree to make it fit on one page
        - regroup the tree

    You can then save and/or print.
