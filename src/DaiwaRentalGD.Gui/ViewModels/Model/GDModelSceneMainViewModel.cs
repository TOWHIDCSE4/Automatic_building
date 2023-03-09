using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DaiwaRentalGD.Gui.Utilities.Configs;
using DaiwaRentalGD.Gui.Utilities.WorkingJsonFile;
using DaiwaRentalGD.Gui.Utilities.WorkingJsonFile.JsonModel;
using DaiwaRentalGD.Gui.ViewModels.Optimization;
using DaiwaRentalGD.Gui.Views.Model;
using DaiwaRentalGD.Model;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.BuildingDesign.TypeA;
using DaiwaRentalGD.Model.BuildingDesign.TypeB;
using DaiwaRentalGD.Model.BuildingDesign.TypeC;
using DaiwaRentalGD.Model.Common;
using DaiwaRentalGD.Model.Samples;
using DaiwaRentalGD.Model.SiteDesign;
using Microsoft.Win32;
using Svg;
using Workspaces.Json;

namespace DaiwaRentalGD.Gui.ViewModels.Model
{
    /// <summary>
    /// View model for working with a
    /// <see cref="GDModelScene"/>, such as
    /// setting the inputs and displaying the data outputs and
    /// 3D visualization of the model.
    /// </summary>
    public class GDModelSceneMainViewModel : GDModelSceneViewModelBase
    {
        #region Constructors

        public GDModelSceneMainViewModel(GDModelScene gdms) : base(gdms)
        {
            GDModelSceneInputsViewModel =
                new GDModelSceneInputsViewModel(gdms);

            GDModelSceneDataViewModel =
                new GDModelSceneDataViewModel(gdms);

            GDModelSceneViewportViewModel =
                new GDModelSceneViewportViewModel(gdms);

            GDModelSceneViewportOptionsViewModel =
                new GDModelSceneViewportOptionsViewModel(
                    GDModelSceneViewportViewModel
                );

            InitializeCommands();
        }

        #endregion

        #region Methods

        private void InitializeCommands()
        {
            CreateNewModelCommand = new RelayCommand
            {
                ExecuteAction = CreateNewModelCommandExecuteAction
            };

            CreateNewOptimizationCommand = new RelayCommand
            {
                ExecuteAction = CreateNewOptimizationCommandExecuteAction
            };

            LoadModelCommand = new RelayCommand
            {
                ExecuteAction = LoadModelCommandExecuteAction
            };

            SaveModelCommand = new RelayCommand
            {
                ExecuteAction = SaveModelCommandExecuteAction
            };

            ExitCommand = new RelayCommand
            {
                ExecuteAction = ExitCommandExecuteAction
            };

            ShowAboutCommand = new RelayCommand
            {
                ExecuteAction = ShowAboutCommandExecuteAction
            };
        }

        private void CreateNewModelCommandExecuteAction(object parameter)
        {
            var gdms = CreateDefaultGDModelScene();

            var viewModel = new GDModelSceneMainViewModel(gdms);
            viewModel.LoadParam();

            WindowService?.ShowWindow(viewModel);
        }

        private void CreateNewOptimizationCommandExecuteAction(
            object parameter
        )
        {
            var omViewModel = new OptimizationMainViewModel(this);
            omViewModel.LoadParam();
            WindowService?.ShowChildWindow(omViewModel);
        }

        private void LoadModelCommandExecuteAction(object parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Load Model",
                FileName = DefaultGDModelFilename,
                DefaultExt = GDModelFilenameExtension,
                Filter = FileDialogFilter,
                CheckFileExists = true,
                CheckPathExists = true
            };

            var dialogResult = openFileDialog.ShowDialog();

            if (dialogResult != true)
            {
                return;
            }

            string gdModelSceneJson;

            using (var fileStream = File.OpenRead(openFileDialog.FileName))
            using (var streamReader = new StreamReader(fileStream))
            {
                gdModelSceneJson = streamReader.ReadToEnd();
            }

            var workspace = JsonWorkspace.FromJson(gdModelSceneJson);
            var gdms = workspace.Load<GDModelScene>(workspace.ItemUids[0]);

            WindowService
                ?.ShowWindow(new GDModelSceneMainViewModel(gdms));

            StatusText = CreateTimestampedStatusText(
                string.Format(
                    "Model loaded from [{0}].",
                    openFileDialog.FileName
                )
            );
        }

        private void SaveModelCommandExecuteAction(object parameter)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Save Model",
                FileName = DefaultGDModelFilename,
                DefaultExt = GDModelFilenameExtension,
                Filter = FileDialogFilter,
                OverwritePrompt = true
            };

            var dialogResult = saveFileDialog.ShowDialog();

            if (dialogResult != true)
            {
                return;
            }

            var workspace = new JsonWorkspace();
            workspace.Save(GDModelScene);
            string gdModelSceneJson = workspace.ToJson();

            using (var fileStream = File.Create(saveFileDialog.FileName))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.WriteLine(gdModelSceneJson);
            }

            StatusText = CreateTimestampedStatusText(
                string.Format(
                    "Model saved to [{0}].",
                    saveFileDialog.FileName
                )
            );
        }

        private void ExitCommandExecuteAction(object parameter)
        {
            Application.Current.Shutdown();
        }

        private void ShowAboutCommandExecuteAction(object parameter)
        {
            MessageBox.Show(
                "Daiwa Rental GD Developer Tool" + "\n" +
                "Version 0.5.0" + "\n" +
                "\n" +
                "This program is part of " +
                "Daiwa House Rental Housing GD Prototype." + "\n" +
                "\n" +
                "\x00a9 2021 Autodesk Research",
                "About Daiwa Rental GD Developer Tool"
            );
        }

        public static string CreateTimestampedStatusText(string message)
        {
            return string.Format(
                "[{0}] {1}",
                DateTime.Now,
                message
            );
        }

        public static GDModelScene CreateDefaultGDModelScene()
        {
            var gdms = new GDModelScene();           

            // Unit catalog

            var unitCatalogFactory = new SampleUnitCatalogFactory();

            gdms.UnitCatalog = unitCatalogFactory.Create();
            gdms.UnitCatalog.FilterUnitCatalogEntriesByFinance(gdms.FinanceEvaluator.UnitFinanceComponent.CostAndrevenueEntries);

            // Site designer

            gdms.SiteDesigner = new SiteDesigner
            {
                SiteCreatorComponent = new Sample0SiteCreatorComponent()
            };

            // Building designer
            if (gdms.UnitCatalog.UnitCatalogComponent.Entries.Any(x => x is TypeBUnitComponent))
            {
                gdms.BuildingDesigner = new TypeBBuildingDesigner
                {
                    RoofCreatorComponent = new GableRoofCreatorComponent()
                };
            }
            else if (gdms.UnitCatalog.UnitCatalogComponent.Entries.Any(x => x is TypeAUnitComponent))
            {
                gdms.BuildingDesigner = new TypeABuildingDesigner
                {
                    RoofCreatorComponent = new GableRoofCreatorComponent()
                };
            }
            else if (gdms.UnitCatalog.UnitCatalogComponent.Entries.Any(x => x is TypeCUnitComponent))
            {
                gdms.BuildingDesigner = new TypeCBuildingDesigner
                {
                    RoofCreatorComponent = new GableRoofCreatorComponent()
                };
            }


            // Update

            gdms.ExecuteUpdate();

            return gdms;
        }      

        
        // P3向けにSolutionをJSON形式に保存する
        public void SaveSolutionForP3(string output)
        {
            // Saves output json file.
            var outputLayout = new GDOutputJsonLayout(this);
            outputLayout.GetData();
            var jsonSave = $"{output}.json";
            outputLayout.SaveJson(jsonSave);

            // Saves SVG file.
            var svgSave = $"{output}.svg";
            var svgDoc = GDModelSceneViewportViewModel.CreateSVG();
            svgDoc.Write(svgSave);

        }

        public void LoadParam()
        {
            var inputJsonPath = Path.Combine(ConfigJsonPath.P3.RootP3Path, ConfigJsonPath.P3.Input, ConfigJsonPath.P3.InputJson);
            var settingJsonInput = new GDSettingJsonInput(inputJsonPath, this);
            settingJsonInput.Process();

            var settingJsonSite = new GDSettingJsonSite(this);
            settingJsonSite.Process();
        }

        #endregion

        #region Properties

        public GDModelSceneInputsViewModel GDModelSceneInputsViewModel
        { get; }

        public GDModelSceneDataViewModel GDModelSceneDataViewModel
        { get; }

        public GDModelSceneViewportViewModel GDModelSceneViewportViewModel
        { get; }

        public GDModelSceneViewportOptionsViewModel
            GDModelSceneViewportOptionsViewModel
        { get; }

        public ICommand CreateNewModelCommand { get; private set; }

        public ICommand CreateNewOptimizationCommand { get; private set; }

        public ICommand LoadModelCommand { get; private set; }

        public ICommand SaveModelCommand { get; private set; }

        public ICommand ExitCommand { get; private set; }

        public ICommand ShowAboutCommand { get; private set; }

        public string StatusText
        {
            get => _statusText;
            private set
            {
                _statusText = value;
                NotifyPropertyChanged();
            }
        }

        public IViewModelWindowService WindowService { get; set; }

        public static string FileDialogFilter { get; } = string.Format(
            "GD Model (*{0}) | *{0}",
            GDModelFilenameExtension
        );

        #endregion

        #region Member variables

        private string _statusText = "";

        #endregion

        #region Constants

        public const string DefaultGDModelFilename = "GD Model";
        public const string GDModelFilenameExtension = ".gd-model";

        #endregion
    }
}
