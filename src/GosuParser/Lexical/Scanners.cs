using GosuParser.Input;

namespace GosuParser.Lexical
{
    public abstract class Scanners<TToken>
    {
        protected abstract TToken ErrorToken(string msg);

        protected abstract Parser<char, TToken> Token { get; }

        protected abstract Parser<char, Unit> Whitespace { get; }

        protected Scanner CreateScanner(string input) => new Scanner(this, new StringReader(input));

        protected Scanner CreateScanner(Reader<char> input) => new Scanner(this, input);

        protected class Scanner : Reader<TToken>
        {
            private readonly Scanners<TToken> _parent;
            private readonly Reader<char> _input;
            private readonly Reader<char> _rest2;
            private readonly Reader<char> _rest1;

            protected TToken ErrorToken(string msg) => _parent.ErrorToken(msg);

            protected Parser<char, TToken> Token => _parent.Token;

            protected Parser<char, Unit> Whitespace => _parent.Whitespace;

            public Scanner(Scanners<TToken> parent, Reader<char> input)
            {
                _parent = parent;
                switch (Whitespace.Run(input))
                {
                    case Parser<char, Unit>.Success success:
                        switch (Token.Run(success.RemainingInput))
                        {
                            case Parser<char, TToken>.Success tokenSuccess:
                                First = tokenSuccess.Value;
                                _rest1 = success.RemainingInput;
                                _rest2 = tokenSuccess.RemainingInput;
                                break;

                            case Parser<char, TToken>.Failure tokenFailure:
                                First = ErrorToken(tokenFailure.FailureText);
                                _rest1 = tokenFailure.RemainingInput;
                                _rest2 = Skip(tokenFailure.RemainingInput);
                                break;
                        }
                        break;

                    case Parser<char, Unit>.Failure failure:
                        First = ErrorToken(failure.FailureText);
                        _rest1 = failure.RemainingInput;
                        _rest2 = Skip(failure.RemainingInput);
                        break;
                }
            }

            private Reader<char> Skip(Reader<char> input) => input.AtEnd ? input : input.Rest;

            public override TToken First { get; }

            public override Reader<TToken> Rest => new Scanner(_parent, _rest2);

            public override bool AtEnd =>
                _input.AtEnd || Whitespace.Run(_input).Match(_ => false, (unit, next) => next.AtEnd);

            public override Position Position => _rest1.Position;
        }
    }
}