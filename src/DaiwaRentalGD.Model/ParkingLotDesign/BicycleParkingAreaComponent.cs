using System;
using System.Runtime.Serialization;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A component representing a bicycle parking area consisting of
    /// one or more bicycle parking spaces.
    /// </summary>
    [Serializable]
    public class BicycleParkingAreaComponent : ParkingAreaComponent
    {
        #region Constructors

        public BicycleParkingAreaComponent() : base()
        {
            SpaceWidth = DefaultSpaceWidth;
            SpaceLength = DefaultSpaceLength;
            NumOfSpaces = DefaultNumOfSpaces;
            IsWidthDirectionInverted = DefaultIsWidthDirectionInverted;
        }

        protected BicycleParkingAreaComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Constants

        public new const double DefaultSpaceWidth = 0.5;
        public new const double DefaultSpaceLength = 2.0;
        public new const int DefaultNumOfSpaces = 4;
        public new const bool DefaultIsWidthDirectionInverted = false;

        #endregion
    }
}
