First step of a project, always start from an sln, what is it and why is it?
    sln is a list of sub projects or components, describing their relationship towards this project as whole.
    when building the whole solution, or when ide draw solution tree, tools read this list; building a single project alone doesn't need it.
    .cs -> csproj -> sln

command: dotnet new web, create a lease viable template for a website


csproj represents an independently compilable unit — upon build, it turns into a dll (a library project cannot run on its own; projects reference each other). It elaborates what this project is: net version, output type, dependencies, etc. (runtime configuration lives in appsettings / launchSettings, not here)

program.cs code itself, Program.cs is the entry point of csproj

properties/launchSettings.json
configuration under development environment, it wont read under producion

command: dotnet sln add, turns sln from empty list into a meaningful list, and can be read by vs code solution tree