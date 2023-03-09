using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.BuildingDesign.TypeB;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Samples
{
    /// <summary>
    /// A helper class that creates sample unit catalog entries of
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.TypeB
    /// .TypeBUnitComponent"/>.
    /// </summary>
    [Serializable]
    public class SampleTypeBUnitEntryCreator : IWorkspaceItem
    {
        #region Constructors

        public SampleTypeBUnitEntryCreator()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected SampleTypeBUnitEntryCreator(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            LayoutType =
                reader.GetValue<TypeBUnitLayoutType>(nameof(LayoutType));

            _numOfBedrooms = reader.GetValue<int>(nameof(NumOfBedrooms));

            _roomSizeXInP = reader.GetValue<double>(nameof(RoomSizeXInP));
            _roomSizeYInP = reader.GetValue<double>(nameof(RoomSizeYInP));

            _stairSizeXInP = reader.GetValue<double>(nameof(StairSizeXInP));
            _stairSizeYInP = reader.GetValue<double>(nameof(StairSizeYInP));
        }

        #endregion

        #region Methods

        private void UpdateEntryName(TypeBUnitComponent uc)
        {
            uc.EntryName.MainType = TypeBUnitComponent.MainType;

            uc.EntryName.SizeXInP = (int)RoomSizeXInP;
            uc.EntryName.SizeYInP = (int)RoomSizeYInP;

            int variantType = 0;

            if (RoomSizeXInP != (int)RoomSizeXInP)
            {
                variantType += 1;
            }
            if (LayoutType == TypeBUnitLayoutType.Stair)
            {
                variantType += 2;
            }

            uc.EntryName.VariantType = variantType;
        }

        private void UpdateRoomPlans(TypeBUnitComponent uc)
        {
            Polygon roomPlanP;

            if (LayoutType == TypeBUnitLayoutType.Basic)
            {
                roomPlanP = new Polygon(
                    new[]
                    {
                        new Point(0.0, 0.0, 0.0),
                        new Point(RoomSizeXInP, 0.0, 0.0),
                        new Point(RoomSizeXInP, RoomSizeYInP, 0.0),
                        new Point(0.0, RoomSizeYInP, 0.0),
                    }
                );
            }
            else
            {
                roomPlanP = new Polygon(
                    new[]
                    {
                        new Point(0.0, 0.0, 0.0),
                        new Point(RoomSizeXInP, 0.0, 0.0),
                        new Point(
                            RoomSizeXInP,
                            RoomSizeYInP - StairSizeYInP,
                            0.0
                        ),
                        new Point(
                            RoomSizeXInP - StairSizeXInP,
                            RoomSizeYInP - StairSizeYInP,
                            0.0
                        ),
                        new Point(
                            RoomSizeXInP - StairSizeXInP,
                            RoomSizeYInP,
                            0.0
                        ),
                        new Point(0.0, RoomSizeYInP, 0.0),
                    }
                );
            }

            uc.ClearRoomPlans();
            uc.AddRoomPlanP(roomPlanP);
        }

        private void UpdateStaircaseAnchorPoint(TypeBUnitComponent uc)
        {
            if (uc.LayoutType == TypeBUnitLayoutType.Stair)
            {
                uc.StaircaseAnchorPoint.X =
                    LengthP.PToM(RoomSizeXInP - StairSizeXInP);

                uc.StaircaseAnchorPoint.Y =
                    LengthP.PToM(RoomSizeYInP - StairSizeYInP);
            }
        }

        private void UpdateCorridorAnchorPoint(TypeBUnitComponent uc)
        {
            uc.CorridorAnchorPoint.X = 0.0;
            uc.CorridorAnchorPoint.Y = LengthP.PToM(RoomSizeYInP);
        }

        private void UpdateBalconyAnchorPoint(TypeBUnitComponent uc)
        {
            uc.BalconyAnchorPoint.X = DefaultBalconyMarginInP;
            uc.BalconyAnchorPoint.Y = 0.0;
            uc.BalconyLength =
                LengthP.PToM(RoomSizeXInP - DefaultBalconyMarginInP * 2.0);
        }

        public TypeBUnitComponent Create()
        {
            TypeBUnitComponent uc = new TypeBUnitComponent
            {
                LayoutType = LayoutType,
                NumOfBedrooms = NumOfBedrooms
            };

            UpdateEntryName(uc);

            UpdateRoomPlans(uc);

            UpdateStaircaseAnchorPoint(uc);

            UpdateCorridorAnchorPoint(uc);

            UpdateBalconyAnchorPoint(uc);

            return uc;
        }

        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            Enumerable.Empty<IWorkspaceItem>();

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(LayoutType), LayoutType);
            writer.AddValue(nameof(NumOfBedrooms), _numOfBedrooms);
            writer.AddValue(nameof(RoomSizeXInP), _roomSizeXInP);
            writer.AddValue(nameof(RoomSizeYInP), _roomSizeYInP);
            writer.AddValue(nameof(StairSizeXInP), _stairSizeXInP);
            writer.AddValue(nameof(StairSizeYInP), _stairSizeYInP);
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public TypeBUnitLayoutType LayoutType { get; set; }

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
            }
        }

        public double RoomSizeXInP
        {
            get => _roomSizeXInP;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(RoomSizeXInP)} must be positive"
                    );
                }

                _roomSizeXInP = value;
            }
        }

        public double RoomSizeYInP
        {
            get => _roomSizeYInP;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(RoomSizeYInP)} must be positive"
                    );
                }

                _roomSizeYInP = value;
            }
        }

        public double StairSizeXInP
        {
            get => _stairSizeXInP;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(StairSizeXInP)} must be positive"
                    );
                }

                _stairSizeXInP = value;
            }
        }

        public double StairSizeYInP
        {
            get => _stairSizeYInP;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(StairSizeYInP)} must be positive"
                    );
                }

                _stairSizeYInP = value;
            }
        }

        #endregion

        #region Member variables

        private double _roomSizeXInP = DefaultRoomSizeXInP;
        private double _roomSizeYInP = DefaultRoomSizeYInP;

        private double _stairSizeXInP = DefaultStairSizeXInP;
        private double _stairSizeYInP = DefaultStairSizeYInP;

        private int _numOfBedrooms = UnitComponent.DefaultNumOfBedrooms;

        #endregion

        #region Constants

        public const double DefaultRoomSizeXInP = 6.0;
        public const double DefaultRoomSizeYInP = 6.0;

        public const double DefaultStairSizeXInP = 3.0;
        public const double DefaultStairSizeYInP = 4.0;

        public const double DefaultBalconyMarginInP = 0.0;

        #endregion
    }
}
