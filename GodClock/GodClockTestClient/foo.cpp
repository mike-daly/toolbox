#include "windows.h"
#include "stdio.h"
#include "stdafx.h"


int __cdecl main()
{
	SYSTEMTIME stVNow;
	GetSystemTime( &stVNow );

	printf( "GetSystemTime():  %02d:%02d:%02d,%02d:%02d:%02d.%03d\n",
           stVNow.wMonth, stVNow.wDay, (stVNow.wYear % 100),
           stVNow.wHour, stVNow.wMinute, stVNow.wSecond, 
           stVNow.wMilliseconds );

    printf( "choice 11:  %u\n", stVNow.wMilliseconds%11 );
    printf( "choice 10:  %u\n", stVNow.wMilliseconds%10 );
    printf( "choice 9:  %u\n", stVNow.wMilliseconds%9 );
    printf( "choice 8:  %u\n", stVNow.wMilliseconds%8 );
    printf( "choice 7:  %u\n", stVNow.wMilliseconds%7 );
    printf( "choice 6:  %u\n", stVNow.wMilliseconds%6 );
    printf( "choice 5:  %u\n", stVNow.wMilliseconds%5 );
    printf( "choice 4:  %u\n", stVNow.wMilliseconds%4 );
    printf( "choice 3:  %u\n", stVNow.wMilliseconds%3 );
    printf( "choice 2:  %u\n", stVNow.wMilliseconds%2 );


	return 0;
}
