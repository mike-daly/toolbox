:: first compile the resources
resgen.exe strings.resx DBSpecGen.strings.resources

:: then compile the app
::csc.exe /out:DBSpecGen.exe /debug+ /debug:full /res:DBSpecGen.strings.resources HelpGenner.cs Diagram.cs MultiSelect.cs PieChart.cs UI.cs DBSpecGen.cs DataModel.cs
csc.exe /warn:4 /out:DBSpecGen.exe /res:DBSpecGen.strings.resources HelpGenner.cs Diagram.cs MultiSelect.cs PieChart.cs UI.cs DBSpecGen.cs DataModel.cs


