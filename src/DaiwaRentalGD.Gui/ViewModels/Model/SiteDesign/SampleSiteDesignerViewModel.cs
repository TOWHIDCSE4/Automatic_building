using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using DaiwaRentalGD.Model;
using DaiwaRentalGD.Model.Samples;
using DaiwaRentalGD.Model.SiteDesign;

namespace DaiwaRentalGD.Gui.ViewModels.Model.SiteDesign
{
    /// <summary>
    /// View model for choosing from sample
    /// <see cref=DaiwaRentalGD.Model.SiteDesign.SiteDesigner"/>s.
    /// </summary>
    public class SampleSiteDesignerViewModel : SiteDesignerViewModelBase
    {
        #region Constructors

        public SampleSiteDesignerViewModel(GDModelScene gdms) : base(gdms)
        {
            AllSiteDesigners = _allSiteDesigners.AsReadOnly();

            InitializeCommands();
        }

        #endregion

        #region Methods
        private void LoadData()
        {
            SiteDesigner = _allSiteDesigners.FirstOrDefault(
               siteDesigner =>
                   siteDesigner.SiteCreatorComponent.GetType() ==
                   GDModelScene.SiteDesigner.SiteCreatorComponent.GetType()
           ) ?? _allSiteDesigners.FirstOrDefault();
        }
        private void InitializeCommands()
        {
            LoadDataCommand = new RelayCommand
            {
                ExecuteAction = LoadDataAction
            };
        }
        private void LoadDataAction(object parameter)
        {
            try
            {
                _allSiteDesigners.FirstOrDefault(siteDesigner =>siteDesigner.SiteCreatorComponent.GetType() == GDModelScene.SiteDesigner.SiteCreatorComponent.GetType());
                LoadMessage = LoadSuccessMessage;
            }
            catch
            {
                LoadMessage = LoadFailMessage;
                return;
            }

            UpdateGDModelScene();

            NotifyAllGDModelScenePropertiesChanged();
        }

        protected override void NotifyAllGDModelScenePropertiesChanged()
        {
            NotifyPropertyChanged(nameof(SiteDesigner));
            NotifyPropertyChanged(nameof(SampleSiteCreatorComponent));

            NotifyPropertyChanged(nameof(OriginalTrueNorthAngleDegrees));
            NotifyPropertyChanged(nameof(UseOverrideTrueNorthAngle));
            NotifyPropertyChanged(nameof(OverrideTrueNorthAngleDegrees));
            NotifyPropertyChanged(nameof(TrueNorthAngleDegrees));
        }

        #endregion

        #region Properties
        public ICommand LoadDataCommand { get; private set; }
        public override bool IsActivated
        {
            get => base.IsActivated;
            set
            {
                if (value == IsActivated)
                {
                    return;
                }

                base.IsActivated = value;

                if (value)
                {
                    SiteDesigner = AllSiteDesigners.FirstOrDefault();
                }
                else
                {
                    SiteDesigner = new SiteDesigner();
                }
            }
        }

        public override string SiteDesignerName { get; } =
            SampleSiteDesignerName;

        public IReadOnlyList<SiteDesigner> AllSiteDesigners { get; }

        public SiteDesigner SiteDesigner
        {
            get => GDModelScene.SiteDesigner;
            set
            {
                GDModelScene.SiteDesigner = value;

                UpdateGDModelScene();
            }
        }

        public SampleSiteCreatorComponent SampleSiteCreatorComponent =>
            SiteDesigner.SiteCreatorComponent as SampleSiteCreatorComponent;

        public double? OriginalTrueNorthAngleDegrees =>
            SampleSiteCreatorComponent?.OriginalTrueNorthAngleDegrees;

        public bool? UseOverrideTrueNorthAngle
        {
            get => SampleSiteCreatorComponent?.UseOverrideTrueNorthAngle;
            set
            {
                if (SampleSiteCreatorComponent == null)
                {
                    return;
                }

                SampleSiteCreatorComponent.UseOverrideTrueNorthAngle =
                    (bool)value;

                UpdateGDModelScene();
            }
        }

        public double? OverrideTrueNorthAngleDegrees
        {
            get => SampleSiteCreatorComponent?.OverrideTrueNorthAngleDegrees;
            set
            {
                if (SampleSiteCreatorComponent == null)
                {
                    return;
                }

                SampleSiteCreatorComponent.OverrideTrueNorthAngleDegrees =
                    (double)value;

                UpdateGDModelScene();
            }
        }

        public double? TrueNorthAngleDegrees =>
            SampleSiteCreatorComponent?.TrueNorthAngleDegrees;


        public double NorthSlantPlanesStartHeight
        {
            get => GDModelScene.SlantPlanes.NorthSlantPlanesComponent.SlopeStartHeight;
            set
            {
                if (SampleSiteCreatorComponent == null)
                {
                    return;
                }
             
                GDModelScene.SlantPlanes.NorthSlantPlanesComponent.SlopeStartHeight = value;
                UpdateGDModelScene();
            }
        }
        
        public double NorthSlantPlanesStartSlope
        {
            get => GDModelScene.SlantPlanes.NorthSlantPlanesComponent.SlopeAngleTangent;
            set
            {
                if (SampleSiteCreatorComponent == null)
                {
                    return;
                }
             
                GDModelScene.SlantPlanes.NorthSlantPlanesComponent.SlopeAngleTangent = value;
                UpdateGDModelScene();
            }
        }
        
        public double RoadSlantPlanesStartHeight
        {
            get => GDModelScene.SlantPlanes.RoadSlantPlanesComponent.SlopeProjectedLength;
            set
            {
                if (SampleSiteCreatorComponent == null)
                {
                    return;
                }
             
                GDModelScene.SlantPlanes.RoadSlantPlanesComponent.SlopeProjectedLength = value;
                UpdateGDModelScene();
            }
        }
        
        public double RoadSlantPlanesStartSlope
        {
            get => GDModelScene.SlantPlanes.RoadSlantPlanesComponent.SlopeAngleTangent;
            set
            {
                if (SampleSiteCreatorComponent == null)
                {
                    return;
                }
             
                GDModelScene.SlantPlanes.RoadSlantPlanesComponent.SlopeAngleTangent = value;
                UpdateGDModelScene();
            }
        }

        public double SiteSlantPlanesStartHeight
        {
            get => GDModelScene.SlantPlanes.AdjacentSiteSlantPlanesComponent.SlopeStartHeight;
            set
            {
                if (SampleSiteCreatorComponent == null)
                {
                    return;
                }

                GDModelScene.SlantPlanes.AdjacentSiteSlantPlanesComponent.SlopeStartHeight = value;
                UpdateGDModelScene();
            }
        }

        public double SiteSlantPlanesStartSlope
        {
            get => GDModelScene.SlantPlanes.AdjacentSiteSlantPlanesComponent.SlopeAngleTangent;
            set
            {
                if (SampleSiteCreatorComponent == null)
                {
                    return;
                }

                GDModelScene.SlantPlanes.AdjacentSiteSlantPlanesComponent.SlopeAngleTangent = value;
                UpdateGDModelScene();
            }
        }
        public double AbsoluteHeightPlane
        {
            get => GDModelScene.SlantPlanes.AbsoluteHeightSlantPlanesComponent.AbsoluteHeightPlane;
            set
            {
                if (SampleSiteCreatorComponent == null)
                {
                    return;
                }

                GDModelScene.SlantPlanes.AbsoluteHeightSlantPlanesComponent.AbsoluteHeightPlane = value;
                UpdateGDModelScene();
            }
        }

        #endregion

        #region Member variables

        public List<SiteDesigner> _allSiteDesigners =
            new List<SiteDesigner>
            {
                new SiteDesigner
                {
                    SiteCreatorComponent = new Sample0SiteCreatorComponent()
                },
                new SiteDesigner
                {
                    SiteCreatorComponent = new Sample1SiteCreatorComponent()
                },
                new SiteDesigner
                {
                    SiteCreatorComponent = new Sample2SiteCreatorComponent()
                },
                new SiteDesigner
                {
                    SiteCreatorComponent = new Sample3SiteCreatorComponent()
                },
                 new SiteDesigner
                {
                    SiteCreatorComponent = new SampleJsonSiteCreatorComponent()
                }
            };
        public string LoadMessage
        {
            get => _loadMessage;
            set
            {
                _loadMessage = value ??
                    throw new System.ArgumentNullException(nameof(value));

                NotifyPropertyChanged();
            }
        }
       

        private string _loadMessage = string.Empty;
        #endregion

        #region Constants

        public const string SampleSiteDesignerName = "Samples";
        public const string LoadSuccessMessage =
          "Site data is loaded.";

        public const string LoadFailMessage =
            "Failed to load site data.";

        #endregion
    }
}
