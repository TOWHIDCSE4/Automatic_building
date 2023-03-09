using System;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// Japanese unit of length, half-ken. Denoted by P.
    /// </summary>
    [Serializable]
    public class LengthP
    {
        #region Constructors

        public LengthP() : this(0.0)
        { }

        public LengthP(double p)
        {
            P = p;
        }

        public LengthP(LengthP lengthP) : this(
            lengthP?.P ??
            throw new ArgumentNullException(nameof(lengthP))
        )
        { }

        #endregion

        #region Methods

        public static LengthP FromM(double m)
        {
            double p = MToP(m);

            return new LengthP(p);
        }

        public static double PToM(double p)
        {
            return p * PToMScale;
        }

        public static double MToP(double m)
        {
            return m / PToMScale;
        }

        #endregion

        #region Properties

        public double P
        {
            get;
            private set;
        }

        public double M
        {
            get => PToM(P);
            private set => P = MToP(value);
        }

        #endregion

        #region Constants

        public const double PToMScale = 0.91;

        #endregion
    }
}
