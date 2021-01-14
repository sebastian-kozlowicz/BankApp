using System.Collections.Generic;

namespace BankApp.Configuration
{
    public static class VisaAcceptedLength
    {
        public const int Thirteen = 13;
        public const int Sixteen = 16;
        public static readonly List<int> AcceptedLengths = new List<int> { Thirteen, Sixteen };
    }
}
