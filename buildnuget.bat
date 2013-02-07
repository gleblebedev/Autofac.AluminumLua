cd nuget
del /s *.pdb
del /s AluminumLua.dll 
del /s Autofac.Configuration.dll 
del /s Autofac.dll 
del /s Autofac.xml
..\Autofac.AluminumLua\packages\NuGet.CommandLine.2.2.0\tools\NuGet.exe pack Autofac.AluminumLua.nuspec