using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using DaiwaRentalGD.Scene;
using Newtonsoft.Json.Linq;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// Component for loading unit catalog from JSON sources.
    /// </summary>
    [Serializable]
    public class UnitCatalogJsonComponent : Component
    {
        #region Constructors

        public UnitCatalogJsonComponent()
        { }

        protected UnitCatalogJsonComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            _jsonFilePath = info.GetString(nameof(JsonFilePath));

            UnitCatalogJsonConverter =
                info.GetValue<UnitCatalogComponentJsonConverter>(
                    nameof(UnitCatalogJsonConverter)
                );
        }

        #endregion

        #region Methods

        public void Load()
        {
            if (UnitCatalogComponent == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(UnitCatalogComponent)} is not found"
                );
            }

            var jObject = ReadJObject();

            UnitCatalogJsonConverter.LoadJson(UnitCatalogComponent, jObject);
        }

        public string ReadJson()
        {
            string json = File.ReadAllText(JsonFileFullPath);
            //Assembly.GetExecutingAssembly().Location;
            //Assembly.GetEntryAssembly().Location;
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

            info.AddValue(nameof(JsonFilePath), _jsonFilePath);
            info.AddValue(
                nameof(UnitCatalogJsonConverter), UnitCatalogJsonConverter
            );
        }

        #endregion

        #region Properties

        public UnitCatalogComponent UnitCatalogComponent =>
            SceneObject?.GetComponent<UnitCatalogComponent>();

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

                string assemblyPath = Assembly.GetEntryAssembly().Location;

                string assemblyDir = Path.GetDirectoryName(assemblyPath);

                // Call GetFullPath() to normalize slashes in paths

                string jsonFileFullPath = Path.GetFullPath(
                    Path.Combine(assemblyDir, JsonFilePath)
                );

                return jsonFileFullPath;
            }
        }

        public UnitCatalogComponentJsonConverter UnitCatalogJsonConverter
        { get; } = new UnitCatalogComponentJsonConverter();

        #endregion

        #region Member variables

        private string _jsonFilePath = DefaultJsonFilePath;

        #endregion

        #region Constants

        public const string DefaultJsonFilePath = "Data/UnitCatalog.json";

        public const string FileExtension = ".json";

        #endregion
    }
}
