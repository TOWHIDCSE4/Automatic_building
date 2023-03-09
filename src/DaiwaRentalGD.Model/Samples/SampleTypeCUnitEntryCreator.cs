using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.BuildingDesign.TypeC;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Samples
{
    /// <summary>
    /// A helper class that creates sample unit catalog entries of
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.TypeC
    /// .TypeCUnitComponent"/>.
    /// </summary>
    [Serializable]
    public class SampleTypeCUnitEntryCreator : IWorkspaceItem
    {
        #region Constructors

        public SampleTypeCUnitEntryCreator()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected SampleTypeCUnitEntryCreator(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            PositionType =
                reader.GetValue<TypeCUnitPositionType>(nameof(PositionType));

            EntranceType =
                reader.GetValue<TypeCUnitEntranceType>(nameof(EntranceType));

            _roomSizeXInP = reader.GetValue<double>(nameof(RoomSizeXInP));
            _roomSizeYInP = reader.GetValue<double>(nameof(RoomSizeYInP));

            _entryOffsetXInP =
                reader.GetValue<double>(nameof(EntryOffsetXInP));
            _entryOffsetYInP =
                reader.GetValue<double>(nameof(EntryOffsetYInP));

            _numOfBedrooms = reader.GetValue<int>(nameof(NumOfBedrooms));
        }

        #endregion

        #region Methods

        private void UpdateEntryName(TypeCUnitComponent uc)
        {
            uc.EntryName.MainType = TypeCUnitComponent.MainType;

            uc.EntryName.SizeXInP = (int)RoomSizeXInP;
            uc.EntryName.SizeYInP = (int)RoomSizeYInP;

            int variantType = 0;

            if (RoomSizeXInP != (int)RoomSizeXInP)
            {
                variantType += 1;
            }
            if (EntranceType == TypeCUnitEntranceType.South)
            {
                variantType += 2;
            }

            uc.EntryName.VariantType = variantType;

            if (PositionType == TypeCUnitPositionType.FirstFloor)
            {
                uc.EntryName.Index = 0;
            }
            else
            {
                uc.EntryName.Index = 1;
            }

        }

        public void UpdateRoomPlans(TypeCUnitComponent uc)
        {
            Polygon roomPlanP;

            if (EntranceType == TypeCUnitEntranceType.North)
            {
                roomPlanP = new Polygon(
                    new[]
                    {
                        new Point(0.0, 0.0, 0.0),
                        new Point(RoomSizeXInP, 0.0, 0.0),
                        new Point(
                            RoomSizeXInP,
                            RoomSizeYInP - EntryOffsetYInP,
                            0.0
                        ),
                        new Point(
                            RoomSizeXInP - EntryOffsetXInP,
                            RoomSizeYInP - EntryOffsetYInP,
                            0.0
                        ),
                        new Point(
                            RoomSizeXInP - EntryOffsetXInP,
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
                        new Point(RoomSizeXInP - EntryOffsetXInP, 0.0, 0.0),
                        new Point(
                            RoomSizeXInP - EntryOffsetXInP,
                            EntryOffsetYInP,
                            0.0
                        ),
                        new Point(RoomSizeXInP, EntryOffsetYInP, 0.0),
                        new Point(RoomSizeXInP, RoomSizeYInP, 0.0),
                        new Point(0.0, RoomSizeYInP, 0.0),
                    }
                );
            }

            uc.ClearRoomPlans();
            uc.AddRoomPlanP(roomPlanP);
        }

        private void UpdateBalconyAnchorPoint(TypeCUnitComponent uc)
        {
            if (uc.EntranceType == TypeCUnitEntranceType.North)
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
                    RoomSizeXInP - EntryOffsetXInP -
                    DefaultBalconyMarginInP * 2.0
                );
            }
        }


        public TypeCUnitComponent Create()
        {
            TypeCUnitComponent uc = new TypeCUnitComponent
            {
                EntranceType = EntranceType,
                PositionType = PositionType,
                NumOfBedrooms = NumOfBedrooms
            };

            UpdateEntryName(uc);

            UpdateRoomPlans(uc);

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
            writer.AddValue(nameof(RoomSizeXInP), _roomSizeXInP);
            writer.AddValue(nameof(RoomSizeYInP), _roomSizeYInP);
            writer.AddValue(nameof(EntryOffsetXInP), _entryOffsetXInP);
            writer.AddValue(nameof(EntryOffsetYInP), _entryOffsetYInP);
            writer.AddValue(nameof(NumOfBedrooms), _numOfBedrooms);
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public TypeCUnitPositionType PositionType { get; set; }

        public TypeCUnitEntranceType EntranceType { get; set; }

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

        public double EntryOffsetXInP
        {
            get => _entryOffsetXInP;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(EntryOffsetXInP)} cannot be negative"
                    );
                }

                _entryOffsetXInP = value;
            }
        }

        public double EntryOffsetYInP
        {
            get => _entryOffsetYInP;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(EntryOffsetYInP)} cannot be negative"
                    );
                }

                _entryOffsetYInP = value;
            }
        }

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

        #endregion

        #region Member variables

        private double _roomSizeXInP = DefaultRoomSizeXInP;
        private double _roomSizeYInP = DefaultRoomSizeYInP;

        private double _entryOffsetXInP = DefaultEntryOffsetXInP;
        private double _entryOffsetYInP = DefaultEntryOffsetYInP;

        private int _numOfBedrooms = UnitComponent.DefaultNumOfBedrooms;

        #endregion

        #region Constants

        public const double DefaultRoomSizeXInP = 6.0;
        public const double DefaultRoomSizeYInP = 6.0;

        public const double DefaultBalconyMarginInP = 0.5;

        public const double DefaultEntryOffsetXInP = 1.5;
        public const double DefaultEntryOffsetYInP = 1.0;

        #endregion
    }
}
