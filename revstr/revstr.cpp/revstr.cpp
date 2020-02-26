// This is the main project file for VC++ application project 
// generated using an Application Wizard.

#include "stdafx.h"
#include "stdio.h"

#using <mscorlib.dll>

using namespace System;

char* revstr(char* pS) 
{
	char c;
	if (!pS) return NULL; 
	int l = strlen(pS);
	for (int i=0;i<l/2;i++) { //bugbug testing comments and tasks
		c=pS[i];
		pS[i] = pS[l-i-1];
		pS[l-i-1] = c;
	}
	return pS;
}
int _tmain(int argc, char** argv)
{
    // TODO: Please replace the sample code below with your own.
	if (argc>1) {
		int iMaxLen = 0;
		int i;
		for (i=1;i<argc;i++) {				// skip argv[0] -- the exe name
			iMaxLen += strlen(argv[i]) + 1;  // +1 for white space between args
		}
		char* pchAll = new char[iMaxLen];
		strcpy(pchAll, "" ); 

		strcat(pchAll, argv[1]);
		for (i=2;i<argc;i++) {
			strcat(pchAll, " ");				// recover the space padding
			strcat(pchAll, argv[i]);
		}
		printf( "%s\n", revstr(pchAll));
	} else {	// read stdin/write stdout

		char	inBuff[1000];
		while (fgets(inBuff, 1000, stdin) != NULL) {  //BUGBUG -- there is an off-by-one bug here on the last line
			fputs( revstr(inBuff), stdout );
			//fputs( inBuff, stdout );
		}

		//fputs( "\n", stdout );
	}
	return 0;
}