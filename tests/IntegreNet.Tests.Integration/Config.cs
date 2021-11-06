using System;

namespace IntegreNet.Tests.Integration
{
    internal static class Config
    {
        public static bool IsCi => Environment.GetEnvironmentVariable("CI") == "true";
        public static string IntegreUrl => IsCi ? "http://integresql:5000/api/" : "http://localhost:6432/api/";
    }
}