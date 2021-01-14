using System.Collections.Generic;

namespace BankApp.Configuration
{
    public static class MastercardAcceptedLength
    {
        public const int Sixteen = 16;
        public static readonly List<int> AcceptedLengths = new List<int> { Sixteen };
    }
}
