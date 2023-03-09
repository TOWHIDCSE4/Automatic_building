using System;
using System.Runtime.Serialization;

namespace DaiwaRentalGD.Model.Finance
{
    /// <summary>
    /// An entry specifying the cost of a unit with certain properties.
    /// </summary>
    [Serializable]
    public class UnitCostEntry : ISerializable
    {
        #region Constructors

        public UnitCostEntry()
        { }

        protected UnitCostEntry(
            SerializationInfo info, StreamingContext context
        )
        {
            _numOfBedrooms = info.GetInt32(nameof(NumOfBedrooms));
            CostYen = info.GetDouble(nameof(CostYen));
        }

        #endregion

        #region Methods

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            info.AddValue(nameof(NumOfBedrooms), _numOfBedrooms);
            info.AddValue(nameof(CostYen), CostYen);
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

        #endregion

        #region Member variables

        private int _numOfBedrooms;

        #endregion
    }
}
