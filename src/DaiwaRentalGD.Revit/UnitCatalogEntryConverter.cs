using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using DaiwaRentalGD.Model.BuildingDesign;

namespace DaiwaRentalGD.Revit
{
    /// <summary>
    /// Converts <see cref="Document"/> from Revit unit catalog
    /// into <see cref="CatalogUnitComponent"/> and related types.
    /// </summary>
    public class UnitCatalogEntryConverter
    {
        #region Constructors

        public UnitCatalogEntryConverter()
        { }

        #endregion

        #region Methods

        public virtual IReadOnlyList<CatalogUnitComponent> Convert(
            Document document
        )
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var rooms = RoomConverter.GetRooms(document);

            var roomGroupsByLevel = RoomConverter.GroupRoomsByLevel(rooms);

            var unitComps = new List<CatalogUnitComponent>();

            for (int index = 0; index < roomGroupsByLevel.Count; ++index)
            {
                var unitComp = Convert(document, roomGroupsByLevel, index);

                unitComps.Add(unitComp);
            }

            return unitComps;
        }

        protected virtual CatalogUnitComponent Convert(
            Document document,
            List<(string levelName, List<Room> rooms)> roomGroups,
            int index
        )
        {
            var unitComp = Create();

            var baseEntryName = GetBaseEntryName(document.PathName);

            unitComp.EntryName = new UnitCatalogEntryName(baseEntryName)
            {
                Index = index
            };

            var rooms = roomGroups[index].rooms;

            var roomPlans = RoomConverter.GetUnionedRoomPlans(rooms);

            var roomPlanPs = RoomConverter
                .ConvertToShiftedQuantizedPlanPs(roomPlans);

            foreach (var roomPlanP in roomPlanPs)
            {
                unitComp.AddRoomPlanP(roomPlanP);
            }

            var roomHeights =
                rooms.Select(RoomConverter.GetRoomHeight).ToList();

            unitComp.RoomHeight = roomHeights.Max();

            return unitComp;
        }

        protected virtual CatalogUnitComponent Create() =>
            new CatalogUnitComponent();

        public virtual UnitCatalogEntryName GetBaseEntryName(
            string revitDocumentPath
        )
        {
            // Eample `revitDocumentFilename`:
            // 000-00-30809-01-00_モデル.rvt

            string revitDocumentFilename =
                Path.GetFileName(revitDocumentPath);

            // Example `stringParts`:
            //   [0]    [1]    [2]     [3]   [4]
            // { 000,   00,    30809,  01,   00_モデル.rvt }

            var stringParts = revitDocumentFilename.Split('-');

            // Example `entryNameString`:
            // 3-0809-01

            string entryNameString = string.Join(
                "-",
                stringParts[2].Substring(0, 1),
                stringParts[2].Substring(1),
                stringParts[3]
            );

            var entryName = UnitCatalogEntryName.Parse(entryNameString);

            return entryName;
        }

        #endregion

        #region Properties

        public static UnitCatalogEntryConverter Default { get; } =
            new UnitCatalogEntryConverter();

        public RoomConverter RoomConverter
        {
            get => _roomConverter;
            set => _roomConverter = value ??
                throw new ArgumentNullException(nameof(value));
        }

        #endregion

        #region Fields

        private RoomConverter _roomConverter = RoomConverter.Default;

        #endregion
    }
}
