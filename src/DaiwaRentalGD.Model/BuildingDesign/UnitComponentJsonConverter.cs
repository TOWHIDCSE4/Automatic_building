using System;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using Newtonsoft.Json.Linq;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// Converter that creates/loads/saves
    /// <see cref="UnitComponent"/> from/from/to JSON.
    /// </summary>
    [Serializable]
    public class UnitComponentJsonConverter : ISerializable
    {
        #region Constructors

        public UnitComponentJsonConverter()
        { }

        protected UnitComponentJsonConverter(
            SerializationInfo info, StreamingContext context
        )
        { }

        #endregion

        #region Methods

        public virtual UnitComponent CreateFromJson(string json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var jObject = JObject.Parse(json);

            return CreateFromJson(jObject);
        }

        protected internal virtual UnitComponent CreateFromJson(
            JObject jObject
        )
        {
            if (jObject == null)
            {
                throw new ArgumentNullException(nameof(jObject));
            }

            var uc = new UnitComponent();

            LoadJson(uc, jObject);

            return uc;
        }

        public virtual void LoadJson(UnitComponent uc, string json)
        {
            if (uc == null)
            {
                throw new ArgumentNullException(nameof(uc));
            }

            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var jObject = JObject.Parse(json);

            LoadJson(uc, jObject);
        }

        protected internal virtual void LoadJson(
            UnitComponent uc, JObject jObject
        )
        {
            if (uc == null)
            {
                throw new ArgumentNullException(nameof(uc));
            }

            if (jObject == null)
            {
                throw new ArgumentNullException(nameof(jObject));
            }

            uc.RoomHeight =
                (double)jObject[nameof(UnitComponent.RoomHeight)];

            uc.NumOfBedrooms =
                (int)jObject[nameof(UnitComponent.NumOfBedrooms)];

            uc.ClearRoomPlans();

            var roomPlanInPJArray =
                (JArray)jObject[nameof(UnitComponent.RoomPlans)];

            foreach (var roomPlanInPJToken in roomPlanInPJArray)
            {
                var roomPlanInP = CreatePolygonFromJson(roomPlanInPJToken);

                uc.AddRoomPlanP(roomPlanInP);
            }
        }

        public virtual string SaveJson(UnitComponent uc) =>
            SaveJToken(uc).ToString();

        protected internal virtual JObject SaveJToken(UnitComponent uc)
        {
            var jObject = new JObject
            {
                [nameof(UnitComponent.RoomHeight)] = uc.RoomHeight,

                [nameof(UnitComponent.NumOfBedrooms)] = uc.NumOfBedrooms
            };

            var roomPlanInPJTokens =
                uc.RoomPlans.Select(
                    roomPlan => UnitComponent.ConvertPlanToPlanP(roomPlan)
                ).Select(
                    roomPlanInP => SaveJToken(roomPlanInP)
                );

            var roomPlanInPJArray = new JArray(roomPlanInPJTokens);

            jObject[nameof(UnitComponent.RoomPlans)] = roomPlanInPJArray;

            return jObject;
        }

        #region Polygon

        internal Polygon CreatePolygonFromJson(JToken jToken)
        {
            var polygonJArray = (JArray)jToken;

            var points = (
                from pointJToken in polygonJArray
                select CreatePointFromJson(pointJToken)
            ).ToList();

            var polygon = new Polygon(points);

            return polygon;
        }

        internal string SaveJson(Polygon polygon) =>
            SaveJToken(polygon).ToString();

        internal JToken SaveJToken(Polygon polygon)
        {
            var pointJTokens =
                polygon.Points.Select(point => SaveJToken(point));

            var jArray = new JArray(pointJTokens);

            return jArray;
        }

        #endregion

        #region Point

        internal Point CreatePointFromJson(JToken jToken)
        {
            var pointJArray = (JArray)jToken;

            var point = new Point(
                (double)pointJArray[0],
                (double)pointJArray[1],
                (double)pointJArray[2]
            );

            return point;
        }

        internal string SaveJson(Point point) => SaveJToken(point).ToString();

        internal JToken SaveJToken(Point point)
        {
            var jArray = new JArray { point.X, point.Y, point.Z };

            return jArray;
        }

        #endregion

        public virtual void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        { }

        #endregion
    }
}
