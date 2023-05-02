using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.GearScript.Tokenization
{
    public class TokenSet : IEnumerable<TokenDefinition>
    {
        // Statics
        internal static TokenDefinition IdentifierDefinition = new TokenDefinition("Identifier", TokenType.Identifier, @"\b[a-zA-Z_][\w]*\b");


        // Values
        internal Dictionary<TokenType, List<TokenDefinition>> Tokens = new Dictionary<TokenType, List<TokenDefinition>>();
        private Dictionary<string, TokenPtr> _tokenDict;

        private int _count;


        // Properties
        public int Count => _count;


        // Indexer
        public TokenDefinition this[TokenType tokenType, int i]
        {
            get
            {
                return Tokens[tokenType][i];
            }
        }
        public TokenDefinition this[string tokenName]
        {
            get
            {
                TokenPtr ptr = _tokenDict[tokenName];
                return Tokens[ptr.Type][ptr.Id];
            }
        }


        // Constructor
        /// <summary>
        /// Defines the standard set of literals, this includes literals, punctuation, operators and the end of file token types
        /// </summary>
        public TokenSet()
        {
            // Define Token Types
            foreach (TokenType tokenType in Enum.GetValues(typeof(TokenType)))
            {
                Tokens.Add(tokenType, new List<TokenDefinition>());
            }

            // Operators
            DefineOperator("Clockwork.GearScript.Operators.Assign", @"=");

            DefineOperator("Clockwork.GearScript.Operators.Add", @"+");
            DefineOperator("Clockwork.GearScript.Operators.Subtract", @"-");
            DefineOperator("Clockwork.GearScript.Operators.Multiply", @"*");
            DefineOperator("Clockwork.GearScript.Operators.Divide", @"/");
            DefineOperator("Clockwork.GearScript.Operators.Modulo", @"%");
            DefineOperator("Clockwork.GearScript.Operators.Exponent", @"^");

            DefineOperator("Clockwork.GearScript.Operators.AddAssign", @"+=");
            DefineOperator("Clockwork.GearScript.Operators.SubtractAssign", @"-=");
            DefineOperator("Clockwork.GearScript.Operators.MultiplyAssign", @"*=");
            DefineOperator("Clockwork.GearScript.Operators.DivideAssign", @"/=");
            DefineOperator("Clockwork.GearScript.Operators.ModuloAssign", @"%=");
            DefineOperator("Clockwork.GearScript.Operators.ExponentAssign", @"^=");

            DefineOperator("Clockwork.GearScript.Operators.And", @"&&");
            DefineOperator("Clockwork.GearScript.Operators.Or", @"||");
            DefineOperator("Clockwork.GearScript.Operators.Not", @"!");

            DefineOperator("Clockwork.GearScript.Operators.Equal", @"==");
            DefineOperator("Clockwork.GearScript.Operators.NotEqual", @"!=");

            DefineOperator("Clockwork.GearScript.Operators.Greater", @">");
            DefineOperator("Clockwork.GearScript.Operators.Lesser", @"<");
            DefineOperator("Clockwork.GearScript.Operators.GreaterOrEqual", @">=");
            DefineOperator("Clockwork.GearScript.Operators.LesserOrEqual", @"<=");

            // Literals
            DefineLiteral("Clockwork.GearScript.Literals.Number", @"-?\d+(\.\d+)?");
            DefineLiteral("Clockwork.GearScript.Literals.String", @""".*?""");
            //DefineLiteral("Clockwork.GearScript.Literals.Vector", @"");
            DefineLiteral("Clockwork.GearScript.Literals.Color", @"#([0-9A-Fa-f]{6}|[0-9A-Fa-f]{8})");

            // Literals
            DefineToken("Clockwork.GearScript.NumberLiteral", TokenType.Literal, @"-?\d+(\.\d+)?");
            DefineToken("Clockwork.GearScript.StringLiteral", TokenType.Literal, @""".*?""");
            DefineToken("Clockwork.GearScript.VectorLiteral", TokenType.Literal, @"<\s*-?\d+(\.\d+)?\s*,\s*-?\d+(\.\d+)?\s*>");
            DefineToken("Clockwork.GearScript.Color.Literal", TokenType.Literal, @"#([0-9A-Fa-f]{6}|[0-9A-Fa-f]{8})");

            // Punctuation
            DefineToken("Parenthesis", TokenType.Punctuation, @"\(|\)");
            DefineToken("Braces", TokenType.Punctuation, @"\{|\}");
            DefineToken("Brackets", TokenType.Punctuation, @"\[|\]");
            DefineToken("Dot", TokenType.Punctuation, @".");
            DefineToken("Comma", TokenType.Punctuation, @",");
            DefineToken("Colon", TokenType.Punctuation, @":");
            DefineToken("Semicolon", TokenType.Punctuation, @";");

            // Operators
            DefineToken("Add", TokenType.Operator, @"+");
            DefineToken("Subtract", TokenType.Operator, @"-");
            DefineToken("Multiply", TokenType.Operator, @"*");
            DefineToken("Divide", TokenType.Operator, @"/");
            DefineToken("Modulo", TokenType.Operator, @"%");

            DefineToken("Add Assign", TokenType.Operator, @"+=");
            DefineToken("Subtract Assign", TokenType.Operator, @"-=");
            DefineToken("Multiply Assign", TokenType.Operator, @"*=");
            DefineToken("Divide Assign", TokenType.Operator, @"/=");
            DefineToken("Modulo Assign", TokenType.Operator, @"%=");

            DefineToken("And", TokenType.Operator, @"&&");
            DefineToken("Or", TokenType.Operator, @"||");
            DefineToken("Not", TokenType.Operator, @"!");
            DefineToken("Equal", TokenType.Operator, @"==");
            DefineToken("Not Equal", TokenType.Operator, @"!=");
            DefineToken("Greater", TokenType.Operator, @">");
            DefineToken("Lesser", TokenType.Operator, @"<");
            DefineToken("Greather Equal", TokenType.Operator, @">=");
            DefineToken("Lesser Equal", TokenType.Operator, @"<=");
        }


        // Func
        public void DefineLiteral(string tokenName, string regexString)
        {
            DefineToken(tokenName, TokenType.Literal, regexString);
        }
        public void DefinePunctuation(string tokenName, string regexString)
        {
            DefineToken(tokenName, TokenType.Punctuation, regexString);
        }
        public void DefineOperator(string tokenName, string regexString)
        {
            DefineToken(tokenName, TokenType.Operator, regexString);
        }
        public void DefineKeyword(string tokenName, string regexString)
        {
            DefineToken(tokenName, TokenType.Keyword, regexString);
        }

        public bool Contains(string tokenName)
        {
            return _tokenDict.ContainsKey(tokenName);
        }

        private void DefineToken(string tokenName, TokenType tokenType, string regexString)
        {
            if (_tokenDict.ContainsKey(tokenName))
                throw new ArgumentException("Token of name " + tokenName + " was already defined!");

            Tokens[tokenType].Add(new TokenDefinition(tokenName, tokenType, regexString));
            _count++;
        }


        // Interface
        public IEnumerator<TokenDefinition> GetEnumerator()
        {
            return new TokenSetEnumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        // Operators
        public static TokenSet operator +(TokenSet left, TokenSet right)
        {

        }


        // Structs
        private struct TokenPtr
        {
            public readonly TokenType Type;
            public readonly int Id;

            private TokenPtr(TokenType tokenType, int tokenName)
            {
                Type = tokenType;
                Id = tokenName;
            }
        }
    }
}
