using System.Collections.Generic;

namespace BankApp.Configuration
{
    public static class IssuingNetworkSettings
    {
        public static class Visa
        {
            public static class Length
            {
                public const int Thirteen = 13;
                public const int Sixteen = 16;
                public static readonly List<int> AcceptedLengths = new List<int> { Thirteen, Sixteen };
            }

            public static class Prefix
            {
                public static readonly List<string> ValidPrefixes = new List<string> { "4" };
            }
        }

        public static class Mastercard
        {
            public static class Length
            {
                public const int Sixteen = 16;
                public static readonly List<int> AcceptedLengths = new List<int> { Sixteen };
            }

            public static class Prefix
            {
                public static readonly List<string> ValidPrefixes = new List<string> { "51", "52", "53", "54", "55" };
            }
        }
    }
}
