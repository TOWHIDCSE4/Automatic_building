using System.IO;
using System.Reflection;
using System.Windows;
using DaiwaRentalGD.Gui.ViewModels.Model;
using DaiwaRentalGD.Gui.Views.Model;
using DaiwaRentalGD.Gui.ViewModels.Optimization;
using DaiwaRentalGD.Gui.Utilities.Configs;
using DaiwaRentalGD.Model.Common;
using System.Linq;
using System;

namespace DaiwaRentalGD.Gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Methods

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            LoadO3Assemblies();

            // P3連携の判定      
            // Where the application executed.
            string p3Dir = GetAppPath();

            if (e.Args.Length == 1 && e.Args[0].ToLower() == "/p3")
            {
                throw new ArgumentException("Path cannot be null. Please specific P3.");
            }
            else if (e.Args.Length > 1 && e.Args[0].ToLower() == "/p3")
            {
                p3Dir = e.Args[1];

                if (!Path.IsPathRooted(p3Dir))
                    p3Dir = Path.Combine(GetAppPath(), p3Dir);

                ConfigJsonPath.P3.HasP3 = true;
            }
            ConfigJsonPath.P3.RootP3Path = p3Dir;

            CheckInputAndCreateOutputDirectory(p3Dir);
            ShowGDModelSceneMainView(ConfigJsonPath.P3.HasP3);
        }

        private void ShowGDModelSceneMainView(bool hasP3)
        {
            var gdmsmView = new GDModelSceneMainView
            {
                GDModelSceneMainViewModel = new GDModelSceneMainViewModel(
                    GDModelSceneMainViewModel.CreateDefaultGDModelScene()
                )
            };

            // Always load params input.json and site.json by defalt even if there is not P3.
            gdmsmView.GDModelSceneMainViewModel.LoadParam();
            gdmsmView.Show();

            // Set up and show Optimization when has P3.
            if (hasP3)
            {
                // P3連携時のOptimizeパラメータ設定
                var omViewModel = new OptimizationMainViewModel(gdmsmView.GDModelSceneMainViewModel);
                gdmsmView.GDModelSceneMainViewModel.WindowService?.ShowChildWindow(omViewModel);
                omViewModel.LoadParam();
                // P3連携時のGD自動実行
                omViewModel.GDModelSolverControlViewModel.SolveToggleCommand.Execute(null);
                omViewModel.ProgressEventHandler += OmViewModel_ProgressEventHandler;
                omViewModel.OptimizationFinished += OmViewModel_OptimizationFinished;
            }
        }

        private void OmViewModel_ProgressEventHandler(object sender, Optimization.Solvers.ProgressEventArgs e)
        {
            // Save progress to json file.
            SaveProgressJson(e.Percent);

            // Close application if the abort req file exists in P3.
            CloseAppIfAbortReqFileExistsInP3();
        }

        private void OmViewModel_OptimizationFinished(object sender, OptimizationEventArgs e)
        {
            // Save progress json.
            SaveProgressJson(100);

            // Close application when the optimization is done.
            Dispatcher.Invoke(new System.Action(() =>
            {
                //  this.Shutdown();
            }));
        }

        private void LoadO3Assemblies()
        {
            LoadLocalAssembly("O3.Commons.dll");
            LoadLocalAssembly("O3.Commons.Evolution.dll");
            LoadLocalAssembly("O3.Commons.Evolution.UI.dll");
            LoadLocalAssembly("O3.Commons.UI.dll");
            LoadLocalAssembly("O3.Foundation.dll");
            LoadLocalAssembly("O3.Foundation.UI.dll");
            LoadLocalAssembly("O3.Nsga.dll");
            LoadLocalAssembly("O3.Nsga.UI.dll");
        }

        private void LoadLocalAssembly(string assemblyFilename)
        {
            string assemblyFullPath = GetAssemblyFullPath(assemblyFilename);
            Assembly.LoadFrom(assemblyFullPath);
        }

        private string GetAssemblyFullPath(string assemblyFilename)
        {
            string appLocation = typeof(App).Assembly.Location;

            var appFileInfo = new FileInfo(appLocation);

            string assemblyFullPath =
                Path.Combine(appFileInfo.DirectoryName, assemblyFilename);

            return assemblyFullPath;
        }

        private void CheckInputAndCreateOutputDirectory(string paramPath)
        {
            if (!Directory.Exists(paramPath))
                throw new System.Exception("The P3 directory doesn't exist.");

            _inputDir = Path.Combine(paramPath, ConfigJsonPath.P3.Input);
            _outputDir = Path.Combine(paramPath, ConfigJsonPath.P3.Output);

            if (!Directory.Exists(_inputDir))
                throw new System.Exception("Input directory doesn't exist.");

            if (!Directory.Exists(_outputDir))
                Directory.CreateDirectory(_outputDir);
        }
        private string GetAppPath()
        {
            string assemblyPath = Assembly.GetAssembly(GetType()).Location;
            return Path.GetDirectoryName(assemblyPath);
        }

        private void SaveProgressJson(int percent)
        {
            string progressJson = "{\"Progress\" : " + percent + "}";
            var progressOutput = Path.Combine(ConfigJsonPath.P3.RootP3Path, ConfigJsonPath.P3.ProgressJson);
            File.WriteAllText(progressOutput, progressJson);
        }

        private void SaveProgressJson(string text)
        {
            string progressJson = "{\"Progress\" : " + "\"" + text + "\"}";
            var progressOutput = Path.Combine(ConfigJsonPath.P3.RootP3Path, ConfigJsonPath.P3.ProgressJson);
            File.WriteAllText(progressOutput, progressJson);
        }

        /// <summary>
        /// Checks if abort.req file exists in the P3.
        /// Writes 'abort' and the closes the application.
        /// </summary>
        /// <returns></returns>
        private void CloseAppIfAbortReqFileExistsInP3()
        {
            var abortReq = Path.Combine(ConfigJsonPath.P3.RootP3Path, ConfigJsonPath.P3.AbortReq);
            if (ConfigJsonPath.P3.HasP3 && File.Exists(abortReq))
            {
                SaveProgressJson("abort");

                Dispatcher.Invoke(new System.Action(() =>
                {
                    this.ShutdownMode = ShutdownMode.OnMainWindowClose;
                    this.Shutdown();                    
                }));
           
                //Application.Current.Dispatcher.Invoke(() =>
                //{
                //    this.Shutdown();
                //});
            }
        }

        #endregion

        #region Constant
        #endregion

        #region Field

        private string _inputDir;
        private string _outputDir;
        #endregion
    }
}
