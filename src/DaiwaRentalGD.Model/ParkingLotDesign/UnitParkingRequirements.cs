using System;
using System.Runtime.Serialization;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// Requirements of car parking and bicycle parking from a unit.
    /// </summary>
    [Serializable]
    public class UnitParkingRequirements : ISerializable
    {
        #region Constructors

        public UnitParkingRequirements(int numOfBedrooms)
        {
            if (numOfBedrooms < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(numOfBedrooms),
                    $"{nameof(NumOfBedrooms)} cannot be negative"
                );
            }

            NumOfBedrooms = numOfBedrooms;
        }

        protected UnitParkingRequirements(
            SerializationInfo info, StreamingContext context
        )
        {
            NumOfBedrooms = info.GetInt32(nameof(NumOfBedrooms));

            _carParkingSpaceMin = info.GetDouble(nameof(CarParkingSpaceMin));
            _carParkingSpaceMax = info.GetDouble(nameof(CarParkingSpaceMax));

            _bicycleParkingSpaceMin =
                info.GetDouble(nameof(BicycleParkingSpaceMin));
            _bicycleParkingSpaceMax =
                info.GetDouble(nameof(BicycleParkingSpaceMax));
        }

        #endregion

        #region Methods

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            info.AddValue(nameof(NumOfBedrooms), NumOfBedrooms);

            info.AddValue(nameof(CarParkingSpaceMin), _carParkingSpaceMin);
            info.AddValue(nameof(CarParkingSpaceMax), _carParkingSpaceMax);

            info.AddValue(
                nameof(BicycleParkingSpaceMin), _bicycleParkingSpaceMin
            );
            info.AddValue(
                nameof(BicycleParkingSpaceMax), _bicycleParkingSpaceMax
            );
        }

        #endregion

        #region Properties

        public int NumOfBedrooms { get; }

        public double CarParkingSpaceMin
        {
            get => _carParkingSpaceMin;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(CarParkingSpaceMin)} cannot be negative"
                    );
                }

                _carParkingSpaceMin = value;
            }
        }

        public double CarParkingSpaceMax
        {
            get => _carParkingSpaceMax;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(CarParkingSpaceMax)} cannot be negative"
                    );
                }

                _carParkingSpaceMax = value;
            }
        }

        public double BicycleParkingSpaceMin
        {
            get => _bicycleParkingSpaceMin;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(BicycleParkingSpaceMin)} cannot be negative"
                    );
                }

                _bicycleParkingSpaceMin = value;
            }
        }

        public double BicycleParkingSpaceMax
        {
            get => _bicycleParkingSpaceMax;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(BicycleParkingSpaceMax)} cannot be negative"
                    );
                }

                _bicycleParkingSpaceMax = value;
            }
        }

        #endregion

        #region Member variables

        private double _carParkingSpaceMin = DefaultCarParkingSpaceMin;
        private double _carParkingSpaceMax = DefaultCarParkingSpaceMax;

        private double _bicycleParkingSpaceMin =
            DefaultBicycleParkingSpaceMin;
        private double _bicycleParkingSpaceMax =
            DefaultBicycleParkingSpaceMax;

        #endregion

        #region Constants

        public const double DefaultCarParkingSpaceMin = 0.0;
        public const double DefaultCarParkingSpaceMax = 0.0;

        public const double DefaultBicycleParkingSpaceMin = 0.0;
        public const double DefaultBicycleParkingSpaceMax = 0.0;

        #endregion
    }
}
