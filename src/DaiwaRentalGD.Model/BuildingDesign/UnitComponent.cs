using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// A component that describes a unit.
    /// </summary>
    [Serializable]
    public class UnitComponent : Component, IBBox
    {
        #region Constructors

        public UnitComponent() : base()
        { }

        public UnitComponent(UnitComponent uc) : this()
        {
            if (uc == null)
            {
                throw new ArgumentNullException(nameof(uc));
            }

            NumOfBedrooms = uc.NumOfBedrooms;
            RoomHeight = uc.RoomHeight;

            foreach (Polygon roomPlan in uc.RoomPlans)
            {
                Polygon roomPlanCopy = new Polygon(roomPlan);
                AddRoomPlan(roomPlanCopy);
            }
        }

        protected UnitComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _numOfBedrooms = reader.GetValue<int>(nameof(NumOfBedrooms));
            _roomHeight = reader.GetValue<double>(nameof(RoomHeight));
            _roomPlans.AddRange(reader.GetValues<Polygon>(nameof(RoomPlans)));
        }

        #endregion

        #region Methods

        #region Convert between meter and P

        public static Polygon ConvertPlanPToPlan(Polygon planP)
        {
            Polygon plan = new Polygon(planP);

            foreach (Point point in plan.Points)
            {
                point.X = LengthP.PToM(point.X);
                point.Y = LengthP.PToM(point.Y);
                point.Z = LengthP.PToM(point.Z);
            }

            return plan;
        }

        public static Polygon ConvertPlanToPlanP(Polygon plan)
        {
            Polygon planP = new Polygon(plan);

            foreach (Point point in planP.Points)
            {
                point.X = LengthP.MToP(point.X);
                point.Y = LengthP.MToP(point.Y);
                point.Z = LengthP.MToP(point.Z);
            }

            return planP;
        }

        #endregion

        #region Rooms

        public void InsertRoomPlan(int roomIndex, Polygon roomPlan)
        {
            _roomPlans.Insert(
                roomIndex,
                roomPlan ??
                throw new ArgumentNullException(nameof(roomPlan))
            );
        }

        public void InsertRoomPlanP(int roomIndex, Polygon roomPlanP)
        {
            Polygon roomPlan = ConvertPlanPToPlan(roomPlanP);
            InsertRoomPlan(roomIndex, roomPlan);
        }

        public void AddRoomPlan(Polygon roomPlan)
        {
            InsertRoomPlan(_roomPlans.Count, roomPlan);
        }

        public void AddRoomPlanP(Polygon roomPlanP)
        {
            Polygon roomPlan = ConvertPlanPToPlan(roomPlanP);
            AddRoomPlan(roomPlan);
        }

        public void RemoveRoomPlan(int roomIndex)
        {
            _roomPlans.RemoveAt(roomIndex);
        }

        public void ClearRoomPlans()
        {
            _roomPlans.Clear();
        }

        public Mesh GetRoomMesh(int roomIndex)
        {
            Polygon roomPlan = RoomPlans[roomIndex];

            Mesh roomMesh = GeometryUtils.Extrude(roomPlan, RoomHeight);

            return roomMesh;
        }

        #endregion

        #region Others

        public BBox GetBBox()
        {
            List<Polygon> plans = new List<Polygon>();

            plans.AddRange(RoomPlans);

            BBox bbox = BBox.GetBBox(plans);
            bbox.SizeZ = RoomHeight;

            return bbox;
        }

        #endregion

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(NumOfBedrooms), _numOfBedrooms);
            writer.AddValue(nameof(RoomHeight), _roomHeight);
            writer.AddValues(nameof(RoomPlans), _roomPlans);
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
                    throw new ArgumentException(
                        $"{nameof(NumOfBedrooms)} must be non-negative",
                        nameof(value)
                    );
                }

                _numOfBedrooms = value;
                NotifyPropertyChanged();

                NotifyPropertyChanged(nameof(HouseholdType));
                NotifyPropertyChanged(nameof(PlanName));
            }
        }

        public HouseholdType HouseholdType =>
            NumOfBedrooms <= 1 ?
            HouseholdType.SinglePerson : HouseholdType.Family;

        public string PlanName
        {
            get
            {
                switch (NumOfBedrooms)
                {
                    case 0:

                        return "1R/1K/1DK";

                    default:

                        string planName = string.Format(
                            "{0}LDK", NumOfBedrooms
                        );
                        return planName;
                }
            }
        }

        public IReadOnlyList<Polygon> RoomPlans
        {
            get { return _roomPlans; }
        }

        public double RoomHeight
        {
            get => _roomHeight;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentException(
                        $"{nameof(RoomHeight)} must be positive",
                        nameof(value)
                    );
                }

                _roomHeight = value;
                NotifyPropertyChanged();
            }

        }

        public double TotalRoomPlanArea =>
            RoomPlans.Any() ? RoomPlans.Sum(rp => rp.Area) : 0.0;

        #endregion

        #region Member variables

        private readonly List<Polygon> _roomPlans = new List<Polygon>();

        private int _numOfBedrooms = DefaultNumOfBedrooms;
        private double _roomHeight = DefaultRoomHeight;

        #endregion

        #region Constants

        public const int DefaultNumOfBedrooms = 0;

        public const double DefaultRoomHeight = 3.0;

        #endregion
    }
}
