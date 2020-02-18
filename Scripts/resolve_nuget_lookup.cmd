@echo on 
@if NOT '_echo_'=='' echo off

rem Issue:  nuget list command does not tell you where a package was found
rem
rem Solution:   get all of the nuget sources in the environment,
rem             select the sources that are enabled
rem             do a nuget list with each enabled source
rem
rem Notes:      all parameters are passed to the nuget list command
rem             and can be used to set Verbosity, -allVersions, prerelease
rem             and any other options you want to add

setlocal enabledelayedexpansion

for /F "tokens=1,* usebackq" %%r in (`nuget sources -Format Short`) do @(
  if '%%r' == 'E' ( set server=%%s )
  if '%%r' == 'EM' ( set server=%%s )

  if "!server!" NEQ "" ( 
        rem strip off the trailing spaces:
        set server=!server:~0,-1!
        echo ^>^>^>^> !server!: 
        nuget list -Source "!server!" %*
  )
  set server=
)

