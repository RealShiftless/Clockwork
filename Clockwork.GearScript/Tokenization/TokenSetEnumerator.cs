using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.GearScript.Tokenization
{
    public class TokenSetEnumerator : IEnumerator<TokenDefinition>
    {
        private static int _tokenTypeEnumLength = Enum.GetValues(typeof(TokenType)).Length;

        public TokenDefinition Current => _tokenSet[_type, _index];

        object IEnumerator.Current => Current;

        private TokenSet _tokenSet;

        private TokenType _type;
        private int _index = -1;

        internal TokenSetEnumerator(TokenSet tokenSet)
        {
            _tokenSet = tokenSet;
        }

        public void Dispose()
        {

        }

        public bool MoveNext()
        {
            if (_index + 1 < _tokenSet.Tokens[_type].Count)
            {
                _index++;
            }
            else if ((int)_type + 1 < _tokenTypeEnumLength)
            {
                _index = 0;
                _type++;

                while (_tokenSet.Tokens[_type].Count == 0)
                {
                    _type++;

                    if (_type == TokenType.EndOfFile)
                        return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public void Reset()
        {
            _index = 0;
            _type = TokenType.Literal;
        }
    }
}
