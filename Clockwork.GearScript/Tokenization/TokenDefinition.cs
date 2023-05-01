using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Clockwork.GearScript.Tokenization
{
    public sealed class TokenDefinition
    {
        public readonly string Name;
        public readonly TokenType TokenType;

        public readonly string RegexString;

        internal TokenDefinition(string name, TokenType type, string regexString)
        {
            Name = name;
            TokenType = type;
            RegexString = regexString;
        }

        internal Token[] GetMatches(string input)
        {
            MatchCollection matches = Regex.Matches(input, RegexString);

            Token[] tokens = new Token[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                tokens[i] = new Token(TokenType, match.Index, match.Index + match.Length);
            }

            return tokens;
        }
    }
}
