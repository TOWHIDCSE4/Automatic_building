using System;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace DaiwaRentalGD.Model.BuildingDesign.TypeC
{
    /// <summary>
    /// Converter that creates/loads/saves
    /// <see cref="TypeCUnitComponent"/> from/from/to JSON.
    /// </summary>
    [Serializable]
    public class TypeCUnitComponentJsonConverter :
        CatalogUnitComponentJsonConverter
    {
        #region Constructors

        public TypeCUnitComponentJsonConverter() : base()
        { }

        protected TypeCUnitComponentJsonConverter(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Methods

        protected internal override UnitComponent CreateFromJson(
            JObject jObject
        )
        {
            if (jObject == null)
            {
                throw new ArgumentNullException(nameof(jObject));
            }

            var ucTypeC = new TypeCUnitComponent();

            LoadJson(ucTypeC, jObject);

            return ucTypeC;
        }

        protected internal override void LoadJson(
            UnitComponent uc, JObject jObject
        )
        {
            base.LoadJson(uc, jObject);

            if (!(uc is TypeCUnitComponent ucTypeC))
            {
                return;
            }

            // PositionType

            string positionTypeText =
                (string)jObject[nameof(TypeCUnitComponent.PositionType)];

            if (!Enum.TryParse<TypeCUnitPositionType>(
                positionTypeText, out var positionType
            ))
            {
                throw new ArgumentException(
                    "Invalid position type",
                    nameof(jObject)
                );
            }

            ucTypeC.PositionType = positionType;

            // EntranceType

            string entranceTypeText =
                (string)jObject[nameof(TypeCUnitComponent.EntranceType)];

            if (!Enum.TryParse<TypeCUnitEntranceType>(
                entranceTypeText, out var entranceType
            ))
            {
                throw new ArgumentException(
                    "Invalid entrance type",
                    nameof(jObject)
                );
            }

            ucTypeC.EntranceType = entranceType;

            // BalconyAnchorPoint

            var balconyAnchorPointJToken =
                jObject[nameof(TypeCUnitComponent.BalconyAnchorPoint)];

            var balconyAnchorPoint =
                CreatePointFromJson(balconyAnchorPointJToken);

            ucTypeC.BalconyAnchorPoint.Vector = balconyAnchorPoint.Vector;

            // BalconyLength

            ucTypeC.BalconyLength =
                (double)jObject[nameof(TypeCUnitComponent.BalconyLength)];
        }

        protected internal override JObject SaveJToken(UnitComponent uc)
        {
            var jObject = base.SaveJToken(uc);

            if (!(uc is TypeCUnitComponent ucTypeC))
            {
                return jObject;
            }

            // PositionType

            jObject[nameof(TypeCUnitComponent.PositionType)] =
                ucTypeC.PositionType.ToString();

            // EntranceType

            jObject[nameof(TypeCUnitComponent.EntranceType)] =
                ucTypeC.EntranceType.ToString();

            // BalconyAnchorPoint

            jObject[nameof(TypeCUnitComponent.BalconyAnchorPoint)] =
               SaveJToken(ucTypeC.BalconyAnchorPoint);

            // BalconyLength

            jObject[nameof(TypeCUnitComponent.BalconyLength)] =
                ucTypeC.BalconyLength;

            return jObject;
        }

        #endregion
    }
}
