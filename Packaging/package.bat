set var=0.1.8

c:\LocalNugetPackages\nuget pack Hermes.Core.nuspec -Version %var%
c:\LocalNugetPackages\nuget pack Hermes.Messaging.nuspec -Version %var%
c:\LocalNugetPackages\nuget pack Hermes.EntityFramework.nuspec -Version %var%
c:\LocalNugetPackages\nuget pack Hermes.OrmLite.nuspec -Version %var%
c:\LocalNugetPackages\nuget pack Hermes.ServiceHost.nuspec -Version %var%

move *.nupkg c:\LocalNugetPackages\

pause