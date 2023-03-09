using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.Common;
using DaiwaRentalGD.Scene;
using Newtonsoft.Json.Linq;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Finance
{
    /// <summary>
    /// Component for loading financial data from JSON sources
    /// into finance-related <see cref="Component"/>s.
    /// </summary>
    [Serializable]
    public class FinanceDataJsonComponent : Component
    {
        #region Constructors

        public FinanceDataJsonComponent() : base()
        { }

        protected FinanceDataJsonComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _jsonFilePath = reader.GetValue<string>(nameof(JsonFilePath));
        }

        #endregion

        #region Methods

        public void Load()
        {
            if (UnitFinanceComponent == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(UnitFinanceComponent)} is not found"
                );
            }

            if (ParkingLotFinanceComponent == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(ParkingLotFinanceComponent)} is not found"
                );
            }

            var jObject = ReadJObject();

            var unitFinanceJObject = (JObject)jObject[UnitFinanceKey];

            UnitFinanceComponent.LoadJson(unitFinanceJObject);

            var parkingLotFinanceJObject =
                (JObject)jObject[ParkingLotFinanceKey];

            ParkingLotFinanceComponent.LoadJson(parkingLotFinanceJObject);
        }

        public string ReadJson()
        {
            string json = File.ReadAllText(JsonFilePath);
            return json;
        }

        private JObject ReadJObject()
        {
            string json = ReadJson();

            var jObject = JObject.Parse(json);

            return jObject;
        }

        protected override void OnAdded()
        {
            base.OnAdded();

            Load();
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(JsonFilePath), _jsonFilePath);
        }

        #endregion

        #region Properties

        public UnitFinanceComponent UnitFinanceComponent =>
            SceneObject?.GetComponent<UnitFinanceComponent>();

        public ParkingLotFinanceComponent ParkingLotFinanceComponent =>
            SceneObject?.GetComponent<ParkingLotFinanceComponent>();

        public string JsonFilePath
        {
            get => _jsonFilePath;
            set
            {
                _jsonFilePath = value ??
                    throw new ArgumentNullException(nameof(value));
            }
        }

        public string JsonFileFullPath
        {
            get
            {
                if (Path.IsPathRooted(JsonFilePath))
                {
                    return JsonFilePath;
                }

                string assemblyPath =
                    Assembly.GetAssembly(GetType()).Location;

                string assemblyDir = Path.GetDirectoryName(assemblyPath);

                // Call GetFullPath() to normalize slashes in paths

                string jsonFileFullPath = Path.GetFullPath(
                    Path.Combine(assemblyDir, JsonFilePath)
                );

                return jsonFileFullPath;
            }
        }

        #endregion

        #region Member variables

        private string _jsonFilePath = Path.Combine(ConfigJsonPath.P3.RootP3Path, ConfigJsonPath.P3.Input, ConfigJsonPath.P3.FinanceJson);
        #endregion

        #region Constants

        public const string ParkingLotFinanceKey = "Parking_Lot";

        public const string UnitFinanceKey = "Units";

        #endregion
    }
}
