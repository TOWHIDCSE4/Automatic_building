using System;
using System.Runtime.Serialization;

namespace DaiwaRentalGD.Model.Finance
{
    /// <summary>
    /// An entry specifying the revenue from a unit with certain properties.
    /// </summary>
    [Serializable]
    public class UnitRevenueEntry : ISerializable
    {
        #region Constructors

        public UnitRevenueEntry()
        { }

        protected UnitRevenueEntry(
            SerializationInfo info, StreamingContext context
        )
        {
            _numOfBedrooms = info.GetInt32(nameof(NumOfBedrooms));

            _minArea = info.GetDouble(nameof(MinArea));

            RevenueYenPerSqmPerMonth =
                info.GetDouble(nameof(RevenueYenPerSqmPerMonth));
        }

        #endregion

        #region Methods

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            info.AddValue(nameof(NumOfBedrooms), _numOfBedrooms);
            info.AddValue(nameof(MinArea), _minArea);
            info.AddValue(
                nameof(RevenueYenPerSqmPerMonth),
                RevenueYenPerSqmPerMonth
            );
        }

        #endregion

        #region Properties

        public int NumOfBedrooms
        {
            get => _numOfBedrooms;

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(NumOfBedrooms)} cannot be negative"
                    );
                }

                _numOfBedrooms = value;
            }
        }

        public double MinArea
        {
            get => _minArea;

            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(MinArea)} cannot be negative"
                    );
                }

                _minArea = value;
            }
        }

        public double RevenueYenPerSqmPerMonth
        {
            get; set;
        }

        #endregion

        #region Member variables

        private int _numOfBedrooms;
        private double _minArea;

        #endregion
    }
}
