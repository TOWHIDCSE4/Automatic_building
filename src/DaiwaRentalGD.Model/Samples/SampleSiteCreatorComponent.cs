using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.SiteDesign;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Samples
{
    /// <summary>
    /// Base class for sample
    /// <see cref="DaiwaRentalGD.Model.SiteDesign.SiteCreatorComponent"/>.
    /// </summary>
    [Serializable]
    public abstract class SampleSiteCreatorComponent : SiteCreatorComponent
    {
        #region Constructors

        protected SampleSiteCreatorComponent()
        { }

        protected SampleSiteCreatorComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _useOverrideTrueNorthAngle =
                reader.GetValue<bool>(nameof(UseOverrideTrueNorthAngle));
            _originalTrueNorthAngle =
                reader.GetValue<double>(nameof(OriginalTrueNorthAngle));
            _overrideTrueNorthAngle =
                reader.GetValue<double>(nameof(OverrideTrueNorthAngle));
        }

        #endregion

        #region Methods

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(
                nameof(UseOverrideTrueNorthAngle),
                _useOverrideTrueNorthAngle
            );
            writer.AddValue(
                nameof(OriginalTrueNorthAngle),
                _originalTrueNorthAngle
            );
            writer.AddValue(
                nameof(OverrideTrueNorthAngle),
                _overrideTrueNorthAngle
            );
        }

        #endregion

        #region Properties

        public bool UseOverrideTrueNorthAngle
        {
            get => _useOverrideTrueNorthAngle;
            set
            {
                _useOverrideTrueNorthAngle = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(TrueNorthAngle));
                NotifyPropertyChanged(nameof(TrueNorthAngleDegrees));
            }
        }

        public double OriginalTrueNorthAngle
        {
            get => _originalTrueNorthAngle;
            set
            {
                _originalTrueNorthAngle = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(OriginalTrueNorthAngleDegrees));
                NotifyPropertyChanged(nameof(TrueNorthAngle));
                NotifyPropertyChanged(nameof(TrueNorthAngleDegrees));
            }
        }

        public double OriginalTrueNorthAngleDegrees =>
            OriginalTrueNorthAngle / Math.PI * 180.0;

        public double OverrideTrueNorthAngle
        {
            get => _overrideTrueNorthAngle;
            set
            {
                _overrideTrueNorthAngle = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(OverrideTrueNorthAngleDegrees));
                NotifyPropertyChanged(nameof(TrueNorthAngle));
                NotifyPropertyChanged(nameof(TrueNorthAngleDegrees));
            }
        }

        public double OverrideTrueNorthAngleDegrees
        {
            get => OverrideTrueNorthAngle / Math.PI * 180.0;
            set => OverrideTrueNorthAngle = value / 180.0 * Math.PI;
        }

        public double TrueNorthAngle =>
            UseOverrideTrueNorthAngle ?
            OverrideTrueNorthAngle : OriginalTrueNorthAngle;

        public double TrueNorthAngleDegrees =>
            TrueNorthAngle / Math.PI * 180.0;

        #endregion

        #region Member variables

        private bool _useOverrideTrueNorthAngle =
            DefaultUseOverrideTrueNorthAngle;

        private double _originalTrueNorthAngle;

        private double _overrideTrueNorthAngle =
            DefaultOverrideTrueNorthAngle;

        #endregion

        #region Constants

        public const bool DefaultUseOverrideTrueNorthAngle = false;
        public const double DefaultOverrideTrueNorthAngle = 0.0;

        #endregion
    }
}
