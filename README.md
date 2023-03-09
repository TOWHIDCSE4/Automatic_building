DaiwaRentalGD
=============
_Version 0.5.1_

This is the repository for the Daiwa House Rental Housing GD Prototype
(referred to as the Prototype below).

## Development Environment

The Prototype is developed in C# using Visual Studio 2019.
The target frameworks of the Visual Studio projects are
either .NET Standard 2.0 or .NET Core 3.1.

It is ideal to have the aforementioned IDE and SDK installed
in the development environment. In case they are not available,
it is possible to adapt the source code to different environment settings,
which is out of the scope of this text.

## Repository Structure

The files and directories below are specified using relative paths based on
the root directory of this repository.

- `DaiwaRentalGD.sln`: The source code for the Prototype is organized under
  this single Visual Studio solution. All Visual Studio projects under
  this solution are in SDK style and located in `src` and `tests` directories.

- `src`: Contains Visual Studio projects for the libraries and executables of
  the Prototype, each in its own subdirectory.
  Please refer to _Software Architecture Specification_ for more information
  on these projects and their corresponding top-level modules.

- `tests`: Contains Visual Studio unit test projects for the projects in `src`.
  Each test project is named after the project it tests with suffix `.Tests`.
  Due to the prototyping nature, the test coverage is focused on key logic and
  currently there is no unit test for `DaiwaRentalGD.Gui`.
  Also, `DaiwaRentalGD.Optimization.Samples` project, which provides samples
  of working with O3, is also located in `tests` directory.

- `libs`: Contains external dependencies in binary formats, such as O3.

## Dependencies

- The dependencies needed for building the Visual Studio solution come in
  three categories:

  - Direct assembly references: These assemblies are located in `libs`
    directory mentioned above, such as O3 assemblies

  - NuGet packages from nuget.org: Third-party dependencies are
    retrieved from nuget.org, such as _MathNET.Numerics_

  - NuGet packages from local source: Dependencies developed by
    Autodesk Research internally and used in this GD prototype, such as
    _Workspaces_

Only the last category of dependencies need to be set up manually as follows.

As specified in `nuget.config`, the local NuGet source is named `local-dev`
and points to `packages` directory, which is not tracked by Git.
Please obtain the NuGet packages in this category and add them to
`local-dev` source. For instance, to add `Workspaces 0.3.1` from PowerShell,
use the following command:

```
nuget add .\Workspaces.0.3.1.nupkg -source .\packages\
```

As of this version, packages that need to be installed to the local source
include:

- _Workspaces_: a library that supports serializing object graphs with
  shared and cyclic references using a unified front end API

## Naming Conventions

The source code of the Prototype follows
[the .NET naming guidlines recommended by Microsoft](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines)
wherever applicable.

## Version Control and Releases

The version control of this repository follows the guidance from
[Git Flow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow).
All releases will be made on `master` branch as commits with tags.

Although this is a prototype, [Semantic Versioning](https://semver.org/)
is used for organizing the releases of the Prototype.

For the purpose of project handover (as opposed to further development),
to simplify the process of sharing the source code without requiring
participating developers to set up additional accounts,
the source code is released as a repository with `master` branch only
in a shared folder on Autodesk OneDrive.

(Technically, this repository is set up as a Git remote locally
on the Autodesk developer's machine and synced using Microsoft OneDrive
client application. This ensures that Daiwa House and AppTec have access to
the latest source code releases during the handover process.)

## Work in Progress

As of this release:

- Please disregard all `.Tests` projects since a lot of the unit tests
  are outdated. They will be updated in a future release.

- `DaiwaRentalGD.Optimization` has a framework established with
  some functionality added. More will be added in a future release.
  It will be refactored too.

- XML comments will be added to the following projects in future releases:

    - `DaiwaRentalGD.Model`
    - `DaiwaRentalGD.Optimization`

  Due to the prototyping nature and schedule, only limited XML comments
  will be added to `DaiwaRentalGD.Gui`.

## Documentation

Here is a list of documents for the Prototype development
(currently or to be) available in the shared folder on Autodesk OneDrive:

- _Phase 3 Overview_
- _Software Requirements Specification_
- _Software Architecture Specification_
- _Notes on Selected Algorithms_
- XML documentation comments
