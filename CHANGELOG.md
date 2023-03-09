CHANGELOG
=========

## v0.5.1 (February 8, 2021)

This release includes a few bug fixes, including one caused by
changed behavior from a previous O3 update.

### Changed

Assemblies `O3.Commons` and `O3.Commons.Evolution` have been updated
with the following changes:

- Changed `RealUniformDistributionDoe.Problem` property setter to
  clear `RealUniformDistributionDoe.InputRanges` property instead of
  populating it with default settings.

- Changed `BlendCxOp.Problem` property setter to
  clear `BlendCxOp.BlendAlphas` property instead of
  populating it with default settings.

- Changed `RealUniformOffsetMutOp.Problem` property setter to
  clear `RealUniformOffsetMutOp.RealUniformOffsets` property instead of
  populating it with default settings.

These updates are currently from a development build, but they will be
incorporated into the next pre-release of O3.

### Fixed

- Fixed the bug of duplicate entries in
  `RealUniformDistributionDoe.InputRanges`,
  `BlendCxOp.BlendAlphas` and `RealUniformOffsetMutOp.RealUniformOffsets`
  properties during the initialization of `BuildingDesignerSolverModule` and
  `ParkingLotDesignerSolverModule` by:

  - Using the behavior of the updated O3 mentioned above

  - Accessing current `Solver.Problem` property from
    `ProblemChangedEventArgs.CurrentProblem` property

- All samples from `DaiwaRentalGD.Optimization.Samples` project that use
  `RealUniformDistributionDoe`, `BlendCxOp` and `RealUniformOffsetMutOp`
  have been updated based on the updated O3 assemblies.

  Please refer to updated source code and comments in
  `Nsga2SolverSampleTests.SetDesignOfExps()`,
  `Nsga2SolverSampleTests.SetCrossoverOp()` and
  `Nsga2SolverSampleTests.SetMutationOp()` for more details.

- Temporarily fixed the bug of `Scene.ReplaceSceneObject()` failure
  when switching to GIS site designer and back to sample site designer
  in the debugging GUI.

## v0.5.0 (January 31, 2021)

This release mainly introduces 2 new Visual Studio projects/assemblies:

- `DaiwaRentalGD.Revit`: A plug-in for converting Revit files from
  Daiwa House's rental housing unit catalog into JSON format that
  the GD prototype can work with.

- `DaiwaRentalGD.Optimization.Samples`: This project provides some
  sample usage of types from O3 in the form of a testing project.

### Added

#### `DaiwaRentalGD.Revit` Assembly

`DaiwaRentalGD.Revit` provides a Revit plug-in that converts Revit files
from Daiwa House's rental housing unit catalog into JSON format that
the GD prototype can work with.

The project/assembly is developed with and tested on Autodesk Revit 2020.

#### `DaiwaRentalGD.Optimization.Samples` Assembly

This project/assembly provides some sample usage of types from O3
in the form of a xUnit.net testing project, which includes:

- Sample usage of foundational classes in O3, such as `Problem` and `Solution`
  (`DaiwaRentalGD.Optimization.Samples.Foundation` namespace)

- A test problem, `BinhAndKorn2Problem` class, which is used by
  various samples in this project
  (`DaiwaRentalGD.Optimization.Samples.Foundation` namespace)

- Samples of working with `Nsga2Solver,` which `GDModelSolver` inherits
  from
  (`DaiwaRentalGD.Optimization.Samples.Nsga` namespace)

- Samples of changing mutation probabilities during optimization and
  comparing the results based on metrics of performance
  (`DaiwaRentalGD.Optimization.Samples.Nsga` namespace)

- Samples of working with metric evaluators from
  `O3.Commons.Metrics` namespace
  (`DaiwaRentalGD.Optimization.Samples.Metrics` namespace)

- Samples of working with termination conditions from O3
  (`DaiwaRentalGD.Optimization.Samples.TerminationConditions` namespace)

- Samples that use and compare different solution archives in O3
  (`DaiwaRentalGD.Optimization.Samples.SolutionArchives` namespace)

### Changed

#### O3 v0.1.0-alpha.2

O3 v0.1.0-alpha.2 has been checked in to replace the pre-alpha version
used previously. The updated O3 mainly includes the following
new features:

- **Termination conditions**: 5 types of termination conditions have been
  added:

  - `TimeLimitTermCond`
    (`O3.Commons.TerminationConditions` namespace)
  - `EvaluationCountTermCond`
    (`O3.Commons.TerminationConditions` namespace)
  - `CompositeTermCond`
    (`O3.Commons.TerminationConditions` namespace)
  - `GenerationCountTermCond`
    (`O3.Commons.Evolution.TerminationConditions` namespace)
  - `OutputAdjacencyTermCond`
    (`O3.Commons.Evolution.TerminationConditions` namespace)

- **Solution archives**: Support for solution archives as a type of
  component has been added:

  - `EpsilonParetoSet` class has been updated to
    inherit from the newly added `SolutionArchive` class
  - `ParetoSet` class has been added
  - `Nsga2Sovler` has been updated to work with solution archives

- **Performance of metrics**: Utilities for evaluating metrics of
  performance on solutions have been added:

  - `AggregateMetricEvaluator` (`O3.Commons.Metrics` namespace)
  - `CoverageMetricEvaluator` (`O3.Commons.Metrics` namespace)
  - `SolutionDistanceMetricEvaluator` (`O3.Commons.Metrics` namespace)

## v0.4.1 (November 18, 2020)

This release mainly features integration of an updated version of O3
with support for constraint handling.

### Changed

#### Dependencies

An updated version of O3 which supports constraint handling has been
included and integrated with `DaiwaRentalGD.Optimization` assembly.

#### `DaiwaRentalGD.Optimization` Assembly

Nothing related to the constraint API from O3 is changed in
`DaiwaRentalGD.Optimization` (because the `Constraint` classes was already
part of O3 API but other parts of the previous version of O3 were not
handling the constraints).

However, there are other changes in this assembly due to other O3 API changes.

- `GDModelSceneDataSpec` class now implements the updated `IDataSpec` interface

- `GDModelProblem`, `GDModelProblemModule` and its subclasses,
  `GDModelSolver`, `GDModelSolverModule` and its subclasses have been
  updated to properly support serialization based on their related
  base classes from the updated O3

- `ParkingLotDesignProblemModule`, `SetbackProblemModule` and
  `SlantPlanesProblemModule` classes now use `Solution.Fail()` to mark
  an invalid solution instead of throwing an exception to trigger a failure
  which sets `Solution.State` property to `SolutionState.Failed`

- `GDModelProblemModuleBase` class is renamed to `GDModelProblemModule`

### Removed

#### `DaiwaRentalGD.Optimization` Assembly

- `GDModelSceneProblemModule` class is removed - `GDModelScene` is now
  recreated from a `Solution` by applying solution inputs to
  a `GDModelScene` instance deserialized from JSON in `GDModelProblem`

## v0.4.0 (November 16, 2020)

This release adds serialization support to the GD model using _Workspaces_,
a library that supports serializing object graphs with shared and cyclic
references using a unified front end API.

### Changed

#### `DaiwaRentalGD.Scene` Assembly

- Main types have been updated to implement `IWorkspaceItem` or
 `ISerializable` to support serialization

- `_Workspaces` is added as a package dependency of its Visual Studio project

#### `DaiwaRentalGD.Geometries` Assembly

- Main types have been updated to implement `IWorkspaceItem` or
 `ISerializable` to support serialization

#### `DaiwaRentalGD.Physics` Assembly

- Main types have been updated to implement `IWorkspaceItem` or
 `ISerializable` to support serialization

#### `DaiwaRentalGD.Model` Assembly

- Main types have been updated to implement `IWorkspaceItem` or
 `ISerializable` to support serialization

#### `DaiwaRentalGD.Gui` Assembly

- Load and save menu items in the _Model_ window now use JSON file formats
  from _Workspaces_

## v0.3.0 (September 22, 2020)

This release mainly includes support for loading certain data from
external JSON sources into the GD model.

### Added

#### `DaiwaRentalGD.Model` Assembly

- Added support for loading financial data from a JSON file.
  By default, the JSON file path is `Data/Finance.json` relative
  to `DaiwaRentalGD.Model` assembly directory.

  The following classes have been added to
  namespace `DaiwaRentalGD.Model.Finance`:

  - `FinanceDataJsonExtensions`: Extension methods for finance-related
    classes.

  - `FinanceDataJsonComponent`: A component that loads JSON into
    other finance-related components. It runs once when being added to
    the scene object and should be added after other finance-related
    components. It can also be used to reload JSON at any time.

- Added support for loading parking requirements data from a JSON file.
  By default, the JSON file path is `Data/ParkingRequirements.json` relative
  to `DaiwaRentalGD.Model` assembly directory.

  The following classes have been added to
  namespace `DaiwaRentalGD.Model.ParkingLotDesign`:

  - `ParkingRequirementsJsonExtensions`: Extension methods for
    parking requirements-related classes.

  - `ParkingRequirementsJsonComponent`: A component that loads JSON into
    other parking requirements-related components. It runs once when being
    added to the scene object and should be added after other
    parking requirements-related components. It can also be used to reload JSON
    at any time.

  - `UnitParkingRequirements` class is added as part of refactoring
    `ParkingLotRequirementsComponent`, which also facilitates the JSON
    features above.

- Added support for loading unit catalog from a JSON file.
  By default, the JSON file path is `Data/UnitCatalog.json` relative
  to `DaiwaRentalGD.Model` assembly directory.

  The following classes have been added to
  namespace `DaiwaRentalGD.Model.BuildingDesign` and subnamespaces:

  - `UnitComponentJsonConverter`: Converter that creates/loads/saves
    `UnitComponent` from/from/to JSON.

  - `CatalogUnitComponentJsonConverter` Converter that creates/loads/saves
    `CatalogUnitComponent` from/from/to JSON.

  - `UnitCatalogComponentJsonConverter`: Converter that creates/loads/saves
    `UnitCatalogComponent` from/from/to JSON.

  - `UnitCatalogJsonComponent`: A component that loads JSON into
    `UnitCatalogComponent`. It runs once when being
    added to the scene object and should be added after
    `UnitCatalogComponent`. It can also be used to reload JSON at any time.

  - `Type{A,B,C}.Type{A,B,C}UnitComponentJsonConverter`: Converter that
    creates/loads/saves `Type{A,B,C}UnitComponent` from/from/to JSON.

  Unlike the implementation of JSON functionality for finance and
  parking requirements, extension methods are not used here because of
  potential confusion and bugs from the lack of true inheritance
  support by extension methods.

- Added property `UnitComponent.TotalRoomPlanArea`

#### `DaiwaRentalGD.Gui` Assembly

- `FinanceInputsView` and `FinanceInputsViewModel` classes have been added for
  working with JSON functionality and presenting detailed financial data.

- `UnitCatalogInputsView`, `UnitCatalogInputsViewModel` and
  other supporting classes have been added for working with JSON functionality
  and presenting detailed unit catalog data.
  `UnitCatalogEntryViewportView` and `UnitCatalogEntryViewportViewModel` have
  been added as part of this for previewing individual unit catalog entries.

#### Data

- 3 sample JSON data files have been added in `data` directory: `Finance.json`,
  `ParkingRequirements.json` and `UnitCatalog.json`. They are automatically
  copied to a subdirectory named `Data` in the build target directory
  of `DaiwaRentalGD.Gui` via a post-build action.

### Changed

#### `DaiwaRentalGD.Model` Assembly

- `UnitFinanceComponent` has been refactored to store `UnitCostEntry`
  and `UnitRevenueEntry` instances directly. `UnitCostEntry` and
  `UnitRevenueEntry` have been changed to reference type (class).

- `ParkingLotRequirementsComponent` has been refactored with the addition of
  `UnitParkingRequirements` class.

#### `DaiwaRentalGD.Gui` Assembly

- `ParkingLotRequirementsView` and `ParkingRequirementsViewModel` have been
  updated to present detailed data related to parking requirements.

- Layouts and styles for other parts of the GUI developer tool
  have been updated.

### Removed

- `SampleFinanceEvaluatorFactory` and
  `SampleParkingLotRequirementsComponentFactory` classes are no longer used
  to create the initial GD model scene in the GUI developer tool.
  These classes themselves are not removed yet, but might be
  in a future version.

### Known Issues

- Unit 1-0707-4 from the sample unit catalog is not rendered correctly in
  the unit catalog entry preview. This likely happened in the tessellation
  routine used to convert `DaiwaRentalGD.Geometries.Mesh` into
  `System.Windows.Media.Media3D.MeshGeometry3D`, which supports
  triangle faces only. There are a pair of degenerate edges in the
  room plan of this unit which might have caused the tessellation to fail.
  However, this issue only affects visualization and does not affect
  the GD model logic. The issue has been tracked and will be fixed
  in a future release.

### Additional Notes

- Formats of the sample JSON data files are subject to change for
  the following reasons:

  - Daiwa House mentioned potential alternative ways to calculate financial
    metrics before, but we did not discussed the details.

  - The JSON-based unit catalog is an intermediate unit catalog format
    which will be exported from the Revit-based unit catalog eventually.
    Also, the unit arrangement logic is not final.
    The information and structure in this JSON unit catalog might change
    depending on what will be extracted from Revit files and future
    updates to unit arrangement logic.

- The length unit for the room plans in `UnitCatalog.json` is P
  (1 P = 0.91 meter approx.). All other geometry-related values,
  if not indicated in the key (e.g. `SizeXInP`), are all in meters.

- If the GUI developer tool fails to load or parse a JSON file, it
  reports the failure without showing details. Diagnostics can be
  retrieved using the debugger at breakpoints. Enhanced feedback will be
  included in a coming release if necessary.

- JSON file paths in the newly added JSON functionality can be changed via
  `DaiwaRentalGD.Model` API, but they are currently read-only in
  the GUI developer tool.

- `nuget.config` at the Visual Studio solution level has been added with
  a local NuGet package source named `local-dev`, which is not used
  in this release. We plan to share updated O3 and other utility libraries as
  NuGet packages in future versions.

- Some classes are still undergoing refactoring processes which will be
  done in upcoming releases.


## v0.2.0 (June 28, 2020)

This release mainly includes the upgraded integration with
a more recent version of O3, the optimization framework used in this project.

### Added

- `DaiwaRentalGD.Gui`: Additional UI components have been added for
  the settings of `GDModelProblem` and `GDModelSolver` in
  `OptimizationMainViewModel`/`OptimizationMainView`.

- `DaiwaRentalGD.Gui`: A table view for solutions from optimization
  has been added to `OptimizationMainViewModel`/`OptimizationMainView`.

### Changed

- `DaiwaRentalGD.Optimization`: This project has been upgraded to work
  with a more recent version of O3. The class `ObjectiveFunc` from
  previous version of O3 and its related classes defined in
  `DaiwaRentalGD.Optimization` have been replaced by `Problem` from
  the current version of O3 and its related classes.

- `O3`: The .NET assemblies of the updated O3 have been checked in
  to `libs` directory (`O3*.dll`). This version of O3 has not been
  assigned a version number.

### Removed

- `DaiwaRentalGD.Gui`: Most classes in namespaces
  `DaiwaRentalGD.Gui.ViewModels.Optimization` and
  `DaiwaRentalGD.Gui.Views.Optimization` have been removed,
  since this project now uses the UI components that come with
  the updated version of O3.


## v0.1.0 (May 21, 2020)

This is the first versioned release of DaiwaRentalGD.

Please refer to materials for past meetings and other documentation
for the Daiwa House Rental Housing GD Prototype project
for more information on what is included in this release.
