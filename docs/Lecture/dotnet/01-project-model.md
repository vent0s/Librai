First step of a project, always start from an sln, what is it and why is it?
    sln is a list of sub projects or components, describing their relationship towards this project as whole.
    each time we compile, we build, and each time ide draw solution tree, it has to get to this file and fetch the blueprint of this project.
    .cs -> csproj -> sln

command: dotnet new web, create a lease viable template for a website


csproj representing a self encapsulating project, upon build, it turns into a dll. It elaborates what is this project, its environment, net version, types, dependencies, etc

program.cs code itself, Program.cs is the entry point of csproj

properties/launchSettings.json
configuration under development environment, it wont read under producion

command: dotnet sln add, turns sln from empty list into a meaningful list, and can be read by vs code solution tree