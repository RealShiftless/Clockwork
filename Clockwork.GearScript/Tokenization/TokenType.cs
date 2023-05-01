using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.GearScript.Tokenization
{
    public enum TokenType
    {
        Operator = 0,
        Literal,
        Punctuation,
        Keyword,
        Identifier,
        EndOfFile
    }
}
