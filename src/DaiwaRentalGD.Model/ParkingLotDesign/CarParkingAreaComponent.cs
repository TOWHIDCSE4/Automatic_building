using System;
using System.Runtime.Serialization;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A component representing a car parking area consisting of
    /// one or more car parking spaces.
    /// </summary>
    [Serializable]
    public class CarParkingAreaComponent : ParkingAreaComponent
    {
        #region Constructors

        public CarParkingAreaComponent() : base()
        {
            SpaceWidth = DefaultSpaceWidth;
            SpaceLength = DefaultSpaceLength;
            NumOfSpaces = DefaultNumOfSpaces;
            IsWidthDirectionInverted = DefaultIsWidthDirectionInverted;
        }

        protected CarParkingAreaComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Constants

        public new const double DefaultSpaceWidth = 2.5;
        public new const double DefaultSpaceLength = 5.0;
        public new const int DefaultNumOfSpaces = 1;
        public new const bool DefaultIsWidthDirectionInverted = false;

        #endregion
    }
}
