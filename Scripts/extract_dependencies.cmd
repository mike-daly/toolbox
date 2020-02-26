@echo off
rem  script to crawl through the *proj files
rem  and attempt to create a packages.config file.
rem  with the right version information.
rem
rem  there are still some odd cases (with numbers in the
rem  assembly name, etc) where it does not quite work,
rem  so you have to go and edit a few files by hand


for /f "usebackq delims==" %%f in (`where /r . *.csproj *ccproj *sfproj`) do @(
    @echo === Processing %%f ===
    @del %%~pf\more.packages.config
    (
      @type %~dp0\packages.config.head
      @echo ^<packages^>
       findstr /sp \(Pkg %%f | sed "s/^.*(Pkg/Pkg/" | cut -d^) -f1 | sed "s/^Pkg\([a-zA-Z_]*\)_\([0-9_]*\)$/    <package id=QUOTE\1QUOTE version=QUOTE\2QUOTE targetFramework=QUOTEnet452QUOTE \/>/" | sort | uniq | sed "s,_,.,g" | sed "s,QUOTE,\\\",g"
      @echo ^</packages^>
    )  >>%%~pf\more.packages.config
  )

