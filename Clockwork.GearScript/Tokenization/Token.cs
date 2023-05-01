using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.GearScript.Tokenization
{
    public class Token
    {
        // Values
        public readonly TokenType TokenType;

        public readonly int StartIndex = 0;
        public readonly int EndIndex = 0;


        // Constructor
        internal Token(TokenType type, int startIndex, int endIndex)
        {
            TokenType = type;

            StartIndex = startIndex;
            EndIndex = endIndex;
        }
    }
}
