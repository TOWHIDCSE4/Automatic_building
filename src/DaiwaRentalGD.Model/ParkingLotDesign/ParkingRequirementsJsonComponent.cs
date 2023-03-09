using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.Common;
using DaiwaRentalGD.Scene;
using Newtonsoft.Json.Linq;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// Component for loading parking requirements data from JSON sources
    /// into parking-related <see cref="Component"/>s.
    /// </summary>
    [Serializable]
    public class ParkingRequirementsJsonComponent : Component
    {
        #region Constructors

        public ParkingRequirementsJsonComponent() : base()
        { }

        protected ParkingRequirementsJsonComponent(
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
            if (ParkingLotRequirementsComponent == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(ParkingLotRequirementsComponent)} is not found"
                );
            }

            var jObject = ReadJObject();

            ParkingLotRequirementsComponent.LoadJson(jObject);
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

        public ParkingLotRequirementsComponent
            ParkingLotRequirementsComponent =>
            SceneObject?.GetComponent<ParkingLotRequirementsComponent>();

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

                // Call GetFullPath() to normalize slashes in slashes

                string jsonFileFullPath = Path.GetFullPath(
                    Path.Combine(assemblyDir, JsonFilePath)
                );

                return jsonFileFullPath;
            }
        }

        #endregion

        #region Member variables

        private string _jsonFilePath = Path.Combine(ConfigJsonPath.P3.RootP3Path, ConfigJsonPath.P3.Input, ConfigJsonPath.P3.ParkingRequirements);

        #endregion

        #region Constants

        #endregion
    }
}
