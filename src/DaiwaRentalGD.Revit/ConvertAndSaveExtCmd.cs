using System;
using System.IO;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DaiwaRentalGD.Model.BuildingDesign;
using Microsoft.Win32;

namespace DaiwaRentalGD.Revit
{
    /// <summary>
    /// Revit external command for converting
    /// <see cref="Document"/>s representing Revit unit catalog entries into
    /// <see cref="UnitCatalogComponent"/> and related types and
    /// save them to files.
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    public class ConvertAndSaveExtCmd : IExternalCommand
    {
        #region Constructors

        public ConvertAndSaveExtCmd()
        { }

        #endregion

        #region Methods

        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements
        )
        {
            var document = commandData.Application.ActiveUIDocument.Document;

            var unitCatalogComp = ConvertToUnitCatalogComponent(document);

            string defaultFilename =
                GetDefaultUnitCatalogFilename(document.PathName);

            SaveToFile(unitCatalogComp, defaultFilename);

            return Result.Succeeded;
        }

        public UnitCatalogComponent ConvertToUnitCatalogComponent(
            Document document
        )
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var converter = UnitCatalogEntryConverterLocator.Default
                .GetConverter(document.PathName);

            var catalogUnitComps = converter.Convert(document);

            var unitCatalogComp = new UnitCatalogComponent();

            foreach (var catalogUnitComp in catalogUnitComps)
            {
                unitCatalogComp.AddEntry(catalogUnitComp);
            }

            return unitCatalogComp;
        }

        public string GetDefaultUnitCatalogFilename(string revitDocumentPath)
        {
            if (revitDocumentPath is null)
            {
                throw new ArgumentNullException(nameof(revitDocumentPath));
            }

            var baseEntryName = UnitCatalogEntryConverter.Default
                .GetBaseEntryName(revitDocumentPath);

            string defaultFilename = string.Concat(
                baseEntryName.FullName,
                UnitCatalogJsonComponent.FileExtension
            );

            return defaultFilename;
        }

        public void SaveToFile(
            UnitCatalogComponent unitCatalogComp, string defaultFilename
        )
        {
            if (unitCatalogComp is null)
            {
                throw new ArgumentNullException(nameof(unitCatalogComp));
            }

            if (defaultFilename is null)
            {
                throw new ArgumentNullException(nameof(defaultFilename));
            }

            var saveFileDialog = new SaveFileDialog
            {
                Title = SaveFileDialogTitle,
                FileName = defaultFilename,
                DefaultExt = UnitCatalogJsonComponent.FileExtension,
                Filter = FileDialogFilter,
                OverwritePrompt = true
            };

            var dialogResult = saveFileDialog.ShowDialog();

            if (dialogResult != true)
            {
                return;
            }

            string unitCatalogJson = GetJson(unitCatalogComp);

            using (var fileStream = File.Create(saveFileDialog.FileName))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.WriteLine(unitCatalogJson);
            }
        }

        public string GetJson(UnitCatalogComponent unitCatalogComp)
        {
            if (unitCatalogComp is null)
            {
                throw new ArgumentNullException(nameof(unitCatalogComp));
            }

            var jsonConverter = new UnitCatalogComponentJsonConverter();

            string json = jsonConverter.SaveJson(unitCatalogComp);

            return json;
        }

        #endregion

        #region Properties

        public static string FileDialogFilter { get; } = string.Format(
            "Unit Catalog (*{0}) | *{0}",
            UnitCatalogJsonComponent.FileExtension
        );

        #endregion

        #region Constants

        public const string SaveFileDialogTitle =
            "Save Current Document as Unit Catalog";

        #endregion
    }
}
