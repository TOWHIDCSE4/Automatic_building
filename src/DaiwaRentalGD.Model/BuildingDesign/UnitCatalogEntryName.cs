using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// Contains name information of a unit catalog entry.
    /// </summary>
    [Serializable]
    public class UnitCatalogEntryName : ISerializable
    {
        #region Constructors

        public UnitCatalogEntryName()
        { }

        public UnitCatalogEntryName(UnitCatalogEntryName entryName)
        {
            if (entryName == null)
            {
                throw new ArgumentNullException(nameof(entryName));
            }

            MainType = entryName.MainType;
            SizeXInP = entryName.SizeXInP;
            SizeYInP = entryName.SizeYInP;
            VariantType = entryName.VariantType;
            Index = entryName.Index;
        }

        protected UnitCatalogEntryName(
            SerializationInfo info, StreamingContext context
        )
        {
            _mainType = info.GetInt32(nameof(MainType));
            _sizeXInP = info.GetInt32(nameof(SizeXInP));
            _sizeYInP = info.GetInt32(nameof(SizeYInP));
            _variantType = info.GetInt32(nameof(VariantType));
            _index = info.GetInt32(nameof(Index));
        }

        #endregion

        #region Methods

        public static UnitCatalogEntryName Parse(string fullName)
        {
            Regex regex = new Regex(EntryNamePattern);
            Match match = regex.Match(fullName);

            if (!match.Success)
            {
                throw new ArgumentException(
                    "Invalid full name format",
                    nameof(fullName)
                );
            }

            string mainTypeStr = match.Groups["MainType"].Value;
            int mainType = int.Parse(mainTypeStr);

            string sizeXInPStr = match.Groups["SizeXInP"].Value;
            int sizeXInP = int.Parse(sizeXInPStr);

            string sizeYInPStr = match.Groups["SizeYInP"].Value;
            int sizeYInP = int.Parse(sizeYInPStr);

            string variantTypeStr = match.Groups["VariantType"].Value;
            int variantType = int.Parse(variantTypeStr);

            var entryName = new UnitCatalogEntryName
            {
                MainType = mainType,
                SizeXInP = sizeXInP,
                SizeYInP = sizeYInP,
                VariantType = variantType
            };
            return entryName;
        }

        public override bool Equals(object obj)
        {
            return
                obj is UnitCatalogEntryName entryName &&
                _mainType == entryName._mainType &&
                _sizeXInP == entryName._sizeXInP &&
                _sizeYInP == entryName._sizeYInP &&
                _variantType == entryName._variantType &&
                _index == entryName._index;
        }

        public override int GetHashCode()
        {
            var hashCode = 554719287;

            hashCode = hashCode * -1521134295 + _mainType.GetHashCode();
            hashCode = hashCode * -1521134295 + _sizeXInP.GetHashCode();
            hashCode = hashCode * -1521134295 + _sizeYInP.GetHashCode();
            hashCode = hashCode * -1521134295 + _variantType.GetHashCode();
            hashCode = hashCode * -1521134295 + _index.GetHashCode();

            return hashCode;
        }

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            info.AddValue(nameof(MainType), _mainType);
            info.AddValue(nameof(SizeXInP), _sizeXInP);
            info.AddValue(nameof(SizeYInP), _sizeYInP);
            info.AddValue(nameof(VariantType), _variantType);
            info.AddValue(nameof(Index), _index);
        }

        public static bool operator ==(
            UnitCatalogEntryName left, UnitCatalogEntryName right
        )
        {
            return
                EqualityComparer<UnitCatalogEntryName>
                .Default.Equals(left, right);
        }

        public static bool operator !=(
            UnitCatalogEntryName left, UnitCatalogEntryName right
        )
        {
            return !(left == right);
        }

        #endregion

        #region Properties

        public int MainType
        {
            get => _mainType;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(MainType)} cannot be negative"
                    );
                }

                _mainType = value;
            }
        }

        public int SizeXInP
        {
            get => _sizeXInP;
            set
            {
                if (value < 0 || value > 99)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(SizeXInP)} must be " +
                        "between 0 (inclusive) and 99 (inclusive)"
                    );
                }

                _sizeXInP = value;
            }
        }

        public int SizeYInP
        {
            get => _sizeYInP;
            set
            {
                if (value < 0 || value > 99)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(SizeYInP)} must be " +
                        "between 0 (inclusive) and 99 (inclusive)"
                    );
                }

                _sizeYInP = value;
            }
        }

        public int VariantType
        {
            get => _variantType;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(VariantType)} cannot be negative"
                    );
                }

                _variantType = value;
            }
        }

        public int Index
        {
            get => _index;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(Index)} cannot be negative"
                    );
                }

                _index = value;
            }
        }

        public string FullName
        {
            get
            {
                string fullName = string.Format(
                    "{0}-{1:D2}{2:D2}-{3}",
                    MainType, SizeXInP, SizeYInP, VariantType
                );

                return fullName;
            }
        }

        #endregion

        #region Member variables

        private int _mainType;
        private int _sizeXInP;
        private int _sizeYInP;
        private int _variantType;
        private int _index;

        #endregion

        #region Constants

        public const string EntryNamePattern =
            @"(?<MainType>\d+)-" +
            @"(?<SizeXInP>\d\d)(?<SizeYInP>\d\d)-" +
            @"(?<VariantType>\d+)";

        #endregion
    }
}
