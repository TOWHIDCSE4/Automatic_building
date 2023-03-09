using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DaiwaRentalGD.Gui.ViewModels.Model;
using DaiwaRentalGD.Model;
using DaiwaRentalGD.Optimization.Problems;
using DaiwaRentalGD.Optimization.Solvers;
using O3.Commons.UI.ViewModels.Plotting;
using O3.Foundation.UI.ViewModels;
using O3.Nsga.UI.ViewModels;
using DaiwaRentalGD.Gui.Utilities.WorkingJsonFile;
using System.Reflection;
using System.IO;
using DaiwaRentalGD.Gui.Utilities.Configs;
using DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign;
using DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign.TypeC;
using DaiwaRentalGD.Model.Common;

namespace DaiwaRentalGD.Gui.ViewModels.Optimization
{
    /// <summary>
    /// Main view model for working with optimization logic.
    /// </summary>
    public class OptimizationMainViewModel : ViewModelBase
    {
        #region Constructors

        public OptimizationMainViewModel(
            GDModelSceneMainViewModel gdmsmViewModel
        ) : base()
        {
            GDModelSceneMainViewModel = gdmsmViewModel ??
                throw new ArgumentNullException(nameof(gdmsmViewModel));

            InitializeGDModelProblemViewModel();

            InitializeGDModelSolverViewModels();

            InitializeSolutionsPlot2DViewModel();

            InitializeSolutionCollectionViewModel();
        }

        #endregion

        #region Methods

        private void InitializeGDModelProblemViewModel()
        {
            GDModelProblem = new GDModelProblem(
                GDModelSceneMainViewModel.GDModelScene
            );

            GDModelProblemViewModel =
                new GDModelProblemViewModel(GDModelProblem);
        }

        private void InitializeGDModelSolverViewModels()
        {
            GDModelSolver = new GDModelSolver
            {
                GDModelProblem = GDModelProblem
            };

            GDModelSolver.ProgressEventHandler += GDModelSolver_ProgressEventHandler;

            GDModelSolverViewModel =
                new GDModelSolverViewModel(GDModelSolver);

            GDModelSolverSettingsViewModel =
                new Nsga2SolverSettingsViewModel(GDModelSolverViewModel);

            GDModelSolverStatusViewModel =
                new Nsga2SolverStatusViewModel(GDModelSolverViewModel);

            GDModelSolverControlViewModel =
                new Nsga2SolverControlViewModel(GDModelSolverViewModel);
        }

        private void GDModelSolver_ProgressEventHandler(object sender, ProgressEventArgs e)
        {
            // Trigger OnProgress.
            OnProgress(e);         
        }



        private void InitializeSolutionsPlot2DViewModel()
        {
            SolutionsPlot2DViewModel = new Plot2DViewModel();

            SolutionsPlot2DViewModel.AddLayerViewModel(
                new Plot2DAxisLayerViewModel
                {
                    AxisType = Plot2DAxisType.XAxis
                }
            );

            SolutionsPlot2DViewModel.AddLayerViewModel(
                new Plot2DAxisLayerViewModel
                {
                    AxisType = Plot2DAxisType.YAxis
                }
            );

            var populationLayerViewModel =
                new SolutionScatterPlot2DLayerViewModel(
                    GDModelProblem, GDModelSolverViewModel.Population
                )
                {
                    Name = "Population",
                    GetPointCommand = GetSolutionPoint2DCommand
                };

            SolutionsPlot2DViewModel
                .AddLayerViewModel(populationLayerViewModel);

            var populationLayerOptionsViewModel =
                new SolutionScatterPlot2DLayerOptionsViewModel(
                    populationLayerViewModel
                );

            SolutionsPlot2DViewModel.AddLayerOptionsViewModel(
                populationLayerOptionsViewModel
            );

            SolutionsPlot2DViewModel.XAxisViewModel.Dimension =
                populationLayerOptionsViewModel
                .AllDimensions.FirstOrDefault(
                    dimension =>
                    dimension.Tag ==
                    GDModelProblem.FinanceEvaluatorProblemModule
                    .TotalCostYenOutputSpec
                );

            SolutionsPlot2DViewModel.YAxisViewModel.Dimension =
                populationLayerOptionsViewModel
                .AllDimensions.FirstOrDefault(
                    dimension =>
                    dimension.Tag ==
                    GDModelProblem.FinanceEvaluatorProblemModule
                    .TotalRevenueYenPerYearOutputSpec
                );
        }

        private void InitializeSolutionCollectionViewModel()
        {
            SolutionCollectionViewModel =
                new SolutionCollectionViewModel(GDModelProblem);

            SolutionCollectionViewModel.GetSolutionCommandText =
                (viewModel) => "Open...";

            SolutionCollectionViewModel.GetSolutionCommand =
                (viewModel) =>
                {
                    return new RelayCommand
                    {
                        ExecuteAction = (_) =>
                        {
                            var solution = viewModel.Solution;

                            var problem = (GDModelProblem)solution.Problem;

                            var gdms =
                                problem.CreateSceneFromSolution(solution);

                            ShowGDModelSceneModelMainView(gdms);
                        }
                    };
                };

            GDModelSolver.StatusChanged += (sender, e) =>
            {
                UpdateSolutionCollectionViewModelSolutions();

                /* P3 連携：GD完了ハンドラ*/
                if (e.CurrentStatus == O3.Foundation.SolverStatus.Finished)
                {
                    int sasCount = SolutionCollectionViewModel.SolutionRowViewModels.Count();
                    var summary = new GDOutputSummaryJson();
                    foreach (SolutionRowViewModel srvm in SolutionCollectionViewModel.SolutionRowViewModels)
                    {
                        O3.Foundation.Solution sol = srvm.Solution;
                        var problem = (GDModelProblem)sol.Problem;
                        var gdms = problem.CreateSceneFromSolution(sol);
                        //表示する場合
                        //ShowGDModelSceneModelMainView(gdms);
                        var gdmsmViewModel = new GDModelSceneMainViewModel(gdms);
                        SetNumberofFloors(gdmsmViewModel.GDModelSceneInputsViewModel.BuildingInputsViewModel.BuildingDesignerViewModel);
                        var solutionName = srvm.SolutionName.Replace(". ", "_").Replace(", ", "_");
                        var output = Path.Combine(ConfigJsonPath.P3.RootP3Path, ConfigJsonPath.P3.Output, solutionName);
                        gdmsmViewModel.SaveSolutionForP3(output);
                        summary.GetData(gdmsmViewModel, solutionName);
                    }
                    var summaryJsonSave = Path.Combine(ConfigJsonPath.P3.RootP3Path, ConfigJsonPath.P3.Output, "Output_Summary.json");
                    summary.SaveJson(summaryJsonSave);                  

                    // Auto trigger OnOptimizationFinished event when optimization is done.
                    // Customs OptimizationEventArgs in the future.
                    OnOptimizationFinished(new OptimizationEventArgs());
                }
            };
        }
        /// <summary>
        /// SetNumberofFloors =2 if is TypeCBuildingDesignerViewModel
        /// </summary>
        /// <param name="typeViewModel">BuildingDesignerViewModelBase</param>
        private void SetNumberofFloors(BuildingDesignerViewModelBase typeViewModel)
        {
            if (!(typeViewModel is TypeCBuildingDesignerViewModel))
                return;
            var typeCUnitVM = (TypeCUnitArrangerViewModel)((TypeCBuildingDesignerViewModel)typeViewModel).UnitArrangerViewModel;
            if (typeCUnitVM == null)
                return;
            if (typeCUnitVM.NumOfFloors > 2)
                typeCUnitVM.NumOfFloors = 2;
        }
        private string GetAppPath()
        {
            string assemblyPath = Assembly.GetAssembly(GetType()).Location;
            return Path.GetDirectoryName(assemblyPath);
        }
        private ICommand GetSolutionPoint2DCommand(
            SolutionPoint2DViewModel viewModel
        )
        {
            var command = new RelayCommand
            {
                ExecuteAction = (_) =>
                {
                    var solution = viewModel.Solution;

                    var problem = (GDModelProblem)solution.Problem;

                    var gdms = problem.CreateSceneFromSolution(solution);

                    ShowGDModelSceneModelMainView(gdms);
                }
            };

            return command;
        }

        private void ShowGDModelSceneModelMainView(GDModelScene gdms)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (gdms == null)
                {
                    return;
                }

                var gdmsmViewModel = new GDModelSceneMainViewModel(gdms);

                WindowService?.ShowWindow(gdmsmViewModel);
            });
        }

        private void UpdateSolutionCollectionViewModelSolutions()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                SolutionCollectionViewModel
                    .SetSolutions(GDModelSolver.Population);
            });
        }
        public void LoadParam()
        {
            GDModelSolverViewModel.MaxGeneration = 4;
            GDModelSolverViewModel.PopulationSize = 10;

            var inputJsonPath = Path.Combine(ConfigJsonPath.P3.RootP3Path, ConfigJsonPath.P3.Input, ConfigJsonPath.P3.InputJson);
            var settingJsonInput = new GDSettingJsonInput(inputJsonPath, this);
            settingJsonInput.Process();
        }

        private void OnOptimizationFinished(OptimizationEventArgs e)
        {
            OptimizationFinished?.Invoke(this, e);
        }
       
        private void OnProgress(ProgressEventArgs e)
        {
            ProgressEventHandler?.Invoke(this, e);
        }        

        #endregion

        #region Properties
        public event EventHandler<OptimizationEventArgs> OptimizationFinished;

        public event EventHandler<ProgressEventArgs> ProgressEventHandler;
        public GDModelSceneMainViewModel GDModelSceneMainViewModel { get; }

        public GDModelProblemViewModel GDModelProblemViewModel
        { get; private set; }

        public Nsga2SolverViewModel GDModelSolverViewModel
        { get; private set; }

        public Nsga2SolverSettingsViewModel GDModelSolverSettingsViewModel
        { get; private set; }

        public Nsga2SolverStatusViewModel GDModelSolverStatusViewModel
        { get; private set; }

        public Nsga2SolverControlViewModel GDModelSolverControlViewModel
        { get; private set; }

        public Plot2DViewModel SolutionsPlot2DViewModel
        { get; private set; }

        public SolutionCollectionViewModel SolutionCollectionViewModel
        { get; private set; }

        public IViewModelWindowService WindowService { get; set; }

        private GDModelProblem GDModelProblem { get; set; }

        private GDModelSolver GDModelSolver { get; set; }

        #endregion
    }
    public class OptimizationEventArgs : EventArgs
    {
        public OptimizationEventArgs() { }
    }
}
