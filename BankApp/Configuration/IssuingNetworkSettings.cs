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
        }

        public static class Mastercard
        {
            public static class Length
            {
                public const int Sixteen = 16;
                public static readonly List<int> AcceptedLengths = new List<int> { Sixteen };
            }
        }
    }
}
