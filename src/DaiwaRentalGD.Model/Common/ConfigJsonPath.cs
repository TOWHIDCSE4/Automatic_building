using System;
using System.Collections.Generic;
using System.Text;

namespace DaiwaRentalGD.Model.Common
{
    public static class ConfigJsonPath
    {
        public static class P3
        {
            public const string Input = "Input";
            public const string Output = "Output";
            public const string InputJson = "Input.json";
            public const string FinanceJson = "Finance.json";
            public const string ParkingRequirements = "ParkingRequirements.json";
            public const string Site = "Site.json";
            public static string RootP3Path = string.Empty;
            public static bool HasP3 = false;
            public const string ProgressJson = "Progress.json";
            public const string AbortReq = "abort.req";
        }
    }
}
