using System;
using System.Runtime.Serialization;

namespace DaiwaRentalGD.Model.Finance
{
    /// <summary>
    /// An entry specifying the cost of a unit with certain properties.
    /// </summary>
    [Serializable]
    public class UnitCostsAndRevenuesEntry : ISerializable
    {
        #region Constructors

        public UnitCostsAndRevenuesEntry()
        { }

        protected UnitCostsAndRevenuesEntry(
            SerializationInfo info, StreamingContext context
        )
        {
            _numOfBedrooms = info.GetInt32(nameof(NumOfBedrooms));
            CostYen = info.GetDouble(nameof(CostYen));
            _maxArea = info.GetDouble(nameof(MaxArea));
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
            info.AddValue(nameof(CostYen), CostYen);
            info.AddValue(nameof(MaxArea), _maxArea);
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

        public double CostYen
        {
            get; set;
        }

        public double MaxArea
        {
            get => _maxArea;

            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(MaxArea)} cannot be negative"
                    );
                }

                _maxArea = value;
            }
        }

        public double RevenueYenPerSqmPerMonth
        {
            get; set;
        }
        #endregion

        #region Member variables

        private int _numOfBedrooms;
        private double _maxArea;

        #endregion
    }
}
