using System;
using System.Collections.ObjectModel;
using DaiwaRentalGD.Optimization.Problems;
using O3.Foundation.UI.ViewModels;

namespace DaiwaRentalGD.Gui.ViewModels.Optimization
{
    /// <summary>
    /// View model for
    /// <see cref="DaiwaRentalGD.Optimization.Problems.GDModelProblem"/>.
    /// </summary>
    public class GDModelProblemViewModel : ViewModelBase
    {
        #region Constructors

        public GDModelProblemViewModel(GDModelProblem problem)
        {
            GDModelProblem = problem ??
                throw new ArgumentNullException(nameof(problem));

            InputSpecViewModels =
                new ReadOnlyObservableCollection<InputSpecViewModel>(
                    _inputSpecViewModels
                );

            OutputSpecViewModels =
                new ReadOnlyObservableCollection<OutputSpecViewModel>(
                    _outputSpecViewModels
                );

            InitializeInputSpecViewModels();
            InitializeOutputSpecViewModels();
        }

        #endregion

        #region Methods

        private void InitializeInputSpecViewModels()
        {
            foreach (var inputSpec in GDModelProblem.InputSpecs)
            {
                var viewModel = new InputSpecViewModel(inputSpec);
                _inputSpecViewModels.Add(viewModel);
            }
        }

        private void InitializeOutputSpecViewModels()
        {
            foreach (var outputSpec in GDModelProblem.OutputSpecs)
            {
                var viewModel = new OutputSpecViewModel(outputSpec);
                _outputSpecViewModels.Add(viewModel);
            }
        }

        #endregion

        #region Properties

        public GDModelProblem GDModelProblem { get; }

        public ReadOnlyObservableCollection<InputSpecViewModel>
            InputSpecViewModels
        { get; }

        public ReadOnlyObservableCollection<OutputSpecViewModel>
            OutputSpecViewModels
        { get; }

        #endregion

        #region Member variables

        private readonly ObservableCollection<InputSpecViewModel>
            _inputSpecViewModels =
            new ObservableCollection<InputSpecViewModel>();

        private readonly ObservableCollection<OutputSpecViewModel>
            _outputSpecViewModels =
            new ObservableCollection<OutputSpecViewModel>();

        #endregion
    }
}
