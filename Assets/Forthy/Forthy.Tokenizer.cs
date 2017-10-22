using System;
using System.Collections.Generic;
using System.Text;


/*
 *  Forthy.Tokenizer
 * 
 *  Seperate Tokenizer class to properly handle comment and string parsing.
 * 
 */


public partial class Forthy
{
    public class Tokenizer
    {
        int _head;
        string _source;

        Func<string, bool> _isWordPred;

        public Tokenizer(Func<string, bool> isWordPred)
        {
            _isWordPred = isWordPred;
            return;
        }

        private bool TryMoveNext()
        {
            if (_head < _source.Length)
            {
                ++_head;
                return true;
            }

            return false;
        }

        private bool HasValue(int offset = 0)
        {
            return (_head + offset) >= 0 && (_head + offset) < _source.Length;
        }

        private char GetChar(int offset = 0)
        {
            return _source[_head + offset];
        }

        private void SkipSpace()
        {
            while (HasValue())
            {
                var c = GetChar();

                if (char.IsWhiteSpace(c))
                {
                    TryMoveNext();
                    continue;
                }

                if (c == '#')
                {
                    //  support # style line comment
                    while (HasValue())
                    {
                        TryMoveNext();
                        c = GetChar();
                        if (c == '\r' || c == '\n')
                            break;
                    }
                    continue;
                }

                break;
            }

            return;
        }

        private string MakeString()
        {
            int start = _head;

            char quote = GetChar();
            TryMoveNext();
            while (HasValue())
            {
                var c = GetChar();
                if (c == quote)
                {
                    TryMoveNext();
                    return _source.Substring(start + 1, _head - start - 2);
                }

                TryMoveNext();
            }

            ForthyUtils.Error("unclosed string found");
            return null;
        }

        private string MakeWord()
        {
            int start = _head;

            TryMoveNext();
            while (HasValue())
            {
                var c = GetChar();
                if (char.IsWhiteSpace(c))
                    break;

                TryMoveNext();
            }

            return _source.Substring(start, _head - start);
        }


        public List<Variant> Tokenize(string source)
        {
            _head = 0;
            _source = source;

            var ls = new List<Variant>();

            while (HasValue())
            {
                SkipSpace();

                if (!HasValue())
                    break;

                var c = GetChar();
                if (c == '\'' || c == '"')
                {
                    ls.Add(Variant.Make(MakeString()));
                }
                else
                {
                    var token = MakeWord();
                    if (_isWordPred(token))
                    {
                        ls.Add(Variant.MakeWord(token));
                    }
                    else
                    {
                        Variant variant;
                        bool success = Variant.TryParse(token, out variant);
                        if (!success)
                            throw new ForthyException(string.Format("bad token {0}", token));
                        ls.Add(variant);
                    }
                }
            }

            return ls;
        }
    }
}
