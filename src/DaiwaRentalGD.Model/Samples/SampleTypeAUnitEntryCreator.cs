using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.BuildingDesign.TypeA;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Samples
{
    /// <summary>
    /// A helper class that creates sample unit catalog entries of
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.TypeA
    /// .TypeAUnitComponent"/>.
    /// </summary>
    [Serializable]
    public class SampleTypeAUnitEntryCreator : IWorkspaceItem
    {
        #region Constructors

        public SampleTypeAUnitEntryCreator()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected SampleTypeAUnitEntryCreator(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            PositionType =
                reader.GetValue<TypeAUnitPositionType>(nameof(PositionType));
            EntranceType =
                reader.GetValue<TypeAUnitEntranceType>(nameof(EntranceType));
            _numOfBedrooms = reader.GetValue<int>(nameof(NumOfBedrooms));
            _roomSizeXInP = reader.GetValue<double>(nameof(RoomSizeXInP));
            _roomSizeYInP = reader.GetValue<double>(nameof(RoomSizeYInP));
            _fullStairSizeXInP =
                reader.GetValue<double>(nameof(FullStairSizeXInP));
            _stairSizeYInP = reader.GetValue<double>(nameof(StairSizeYInP));
        }

        #endregion

        #region Methods

        private void UpdateEntryName(TypeAUnitComponent uc)
        {
            uc.EntryName.MainType = TypeAUnitComponent.MainType;

            uc.EntryName.SizeXInP = (int)RoomSizeXInP;
            uc.EntryName.SizeYInP = (int)RoomSizeYInP;

            int variantType = 0;

            if (RoomSizeXInP != (int)RoomSizeXInP)
            {
                variantType += 1;
            }
            if (EntranceType == TypeAUnitEntranceType.South)
            {
                variantType += 2;
            }
            if (PositionType == TypeAUnitPositionType.End)
            {
                variantType += 4;
            }

            uc.EntryName.VariantType = variantType;
        }

        private void UpdateRoomPlans(TypeAUnitComponent uc)
        {
            Polygon roomPlanP;

            if (uc.EntranceType == TypeAUnitEntranceType.North)
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
            else
            {
                roomPlanP = new Polygon(
                    new[]
                    {
                        new Point(0.0, 0.0, 0.0),
                        new Point(RoomSizeXInP - StairSizeXInP, 0.0, 0.0),
                        new Point(
                            RoomSizeXInP - StairSizeXInP,
                            StairSizeYInP,
                            0.0
                        ),
                        new Point(RoomSizeXInP, StairSizeYInP, 0.0),
                        new Point(RoomSizeXInP, RoomSizeYInP, 0.0),
                        new Point(0.0, RoomSizeYInP, 0.0),
                    }
                );
            }

            uc.ClearRoomPlans();
            uc.AddRoomPlanP(roomPlanP);
        }

        private void UpdateStaircaseAnchorPoint(TypeAUnitComponent uc)
        {
            if (uc.EntranceType == TypeAUnitEntranceType.North)
            {
                if (uc.PositionType == TypeAUnitPositionType.Basic)
                {
                    uc.StaircaseAnchorPoint.X =
                        LengthP.PToM(RoomSizeXInP - FullStairSizeXInP / 2.0);
                    uc.StaircaseAnchorPoint.Y =
                        LengthP.PToM(RoomSizeYInP - StairSizeYInP);
                }
                else
                {
                    uc.StaircaseAnchorPoint.X =
                        LengthP.PToM(FullStairSizeXInP);
                    uc.StaircaseAnchorPoint.Y =
                        LengthP.PToM(RoomSizeYInP - StairSizeYInP);
                }
            }
            else
            {
                if (uc.PositionType == TypeAUnitPositionType.Basic)
                {
                    uc.StaircaseAnchorPoint.X =
                        LengthP.PToM
                        (RoomSizeXInP - FullStairSizeXInP / 2.0);
                    uc.StaircaseAnchorPoint.Y =
                        LengthP.PToM(StairSizeYInP);
                }
                else
                {
                    uc.StaircaseAnchorPoint.X =
                        LengthP.PToM(FullStairSizeXInP);
                    uc.StaircaseAnchorPoint.Y =
                        LengthP.PToM(StairSizeYInP);
                }
            }
        }

        private void UpdateBalconyAnchorPoint(TypeAUnitComponent uc)
        {
            if (uc.EntranceType == TypeAUnitEntranceType.North)
            {
                if (uc.PositionType == TypeAUnitPositionType.Basic)
                {
                    uc.BalconyAnchorPoint.X = DefaultBalconyMarginInP;
                    uc.BalconyAnchorPoint.Y = 0.0;
                    uc.BalconyLength = LengthP.PToM(
                        RoomSizeXInP - DefaultBalconyMarginInP * 2.0
                    );
                }
                else
                {
                    uc.BalconyAnchorPoint.X = DefaultBalconyMarginInP;
                    uc.BalconyAnchorPoint.Y = 0.0;
                    uc.BalconyLength = LengthP.PToM(
                        RoomSizeXInP - DefaultBalconyMarginInP * 2.0
                    );
                }
            }
            else
            {
                if (uc.PositionType == TypeAUnitPositionType.Basic)
                {
                    uc.BalconyAnchorPoint.X = DefaultBalconyMarginInP;
                    uc.BalconyAnchorPoint.Y = 0.0;
                    uc.BalconyLength = LengthP.PToM(
                        RoomSizeXInP - StairSizeXInP -
                        DefaultBalconyMarginInP * 2.0
                    );
                }
                else
                {
                    uc.BalconyAnchorPoint.X = DefaultBalconyMarginInP;
                    uc.BalconyAnchorPoint.Y = 0.0;
                    uc.BalconyLength = LengthP.PToM(
                        RoomSizeXInP - StairSizeXInP -
                        DefaultBalconyMarginInP * 2.0
                    );
                }
            }
        }

        public TypeAUnitComponent Create()
        {
            TypeAUnitComponent uc = new TypeAUnitComponent
            {
                EntranceType = EntranceType,
                PositionType = PositionType,
                NumOfBedrooms = NumOfBedrooms
            };

            UpdateEntryName(uc);

            UpdateRoomPlans(uc);

            UpdateStaircaseAnchorPoint(uc);

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

            writer.AddValue(nameof(PositionType), PositionType);
            writer.AddValue(nameof(EntranceType), EntranceType);
            writer.AddValue(nameof(NumOfBedrooms), _numOfBedrooms);
            writer.AddValue(nameof(RoomSizeXInP), _roomSizeXInP);
            writer.AddValue(nameof(RoomSizeYInP), _roomSizeYInP);
            writer.AddValue(nameof(FullStairSizeXInP), _fullStairSizeXInP);
            writer.AddValue(nameof(StairSizeYInP), _stairSizeYInP);
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public TypeAUnitPositionType PositionType { get; set; }

        public TypeAUnitEntranceType EntranceType { get; set; }

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

        public double FullStairSizeXInP
        {
            get => _fullStairSizeXInP;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(FullStairSizeXInP)} must be positive"
                    );
                }

                _fullStairSizeXInP = value;
            }
        }

        public double StairSizeXInP =>
            PositionType == TypeAUnitPositionType.Basic ?
            FullStairSizeXInP / 2.0 : FullStairSizeXInP;

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

        private double _fullStairSizeXInP = DefaultFullStairSizeXInP;
        private double _stairSizeYInP = DefaultStairSizeYInP;

        private int _numOfBedrooms = UnitComponent.DefaultNumOfBedrooms;

        #endregion

        #region Constants

        public const double DefaultRoomSizeXInP = 6.0;
        public const double DefaultRoomSizeYInP = 6.0;

        public const double DefaultFullStairSizeXInP = 3.0;
        public const double DefaultStairSizeYInP = 4.0;

        public const double DefaultBalconyMarginInP = 0.5;

        #endregion
    }
}
