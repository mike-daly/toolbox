// This is the main project file for VC++ application project 
// generated using an Application Wizard.

#include "stdafx.h"
#include "stdio.h"

#using <mscorlib.dll>

using namespace System;

char* MakeLower(char* pS) 
{
	if (!pS) return NULL; 
	int l = strlen(pS);
	for (int i=0;i<l;i++) { //bugbug testing comments and tasks
		pS[i] = tolower(pS[i]);
	}
	return pS;
}
int _tmain(int argc, char** argv)
{
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
		printf( "%s\n", MakeLower(pchAll));
	} else {	// read stdin/write stdout

		char	inBuff[1000];
		while (fgets(inBuff, 1000, stdin) != NULL) {  //BUGBUG -- there is an off-by-one bug here on the last line
			fputs( MakeLower(inBuff), stdout );
			//fputs( inBuff, stdout );
		}

		//fputs( "\n", stdout );
	}
	return 0;
}
