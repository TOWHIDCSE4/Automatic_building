using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// Parameters for roadside car parking area placement.
    /// </summary>
    [Serializable]
    public class RoadsideCarParkingAreaParams :
        INotifyPropertyChanged, ISerializable
    {
        #region Constructors

        public RoadsideCarParkingAreaParams()
        { }

        protected RoadsideCarParkingAreaParams(
            SerializationInfo info, StreamingContext context
        )
        {
            _isEnabled = info.GetBoolean(nameof(IsEnabled));
            _offset = info.GetDouble(nameof(Offset));
            _maxOffset = info.GetDouble(nameof(MaxOffset));
        }

        #endregion

        #region Methods

        protected void NotifyPropertyChanged(
            [CallerMemberName] string propertyName = ""
        )
        {
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(propertyName)
            );
        }

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            info.AddValue(nameof(IsEnabled), _isEnabled);
            info.AddValue(nameof(Offset), _offset);
            info.AddValue(nameof(MaxOffset), _maxOffset);
        }

        #endregion

        #region Properties

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public double Offset
        {
            get => _offset;
            set
            {
                _offset = Math.Min(value, MaxOffset);

                NotifyPropertyChanged();
            }
        }

        public double MaxOffset
        {
            get => _maxOffset;
            set
            {
                _maxOffset = value;

                Offset = Math.Min(Offset, value);

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private bool _isEnabled = DefaultIsEnabled;
        private double _offset = DefaultOffset;
        private double _maxOffset = DefaultMaxOffset;

        #endregion

        #region Events

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constants

        public const bool DefaultIsEnabled = true;
        public const double DefaultOffset = 0.0;
        public const double DefaultMaxOffset = 0.0;

        #endregion
    }
}
