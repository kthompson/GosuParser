using System;
using System.Collections.Generic;
using System.Linq;
using GosuParser.Input;

namespace GosuParser
{
    public static class CharacterParsers
    {
        public static char EndOfReader = (char)0x1a;

        public static Tuple<Parser<char, T>, Action<Parser<char, T>>> CreateParserForwardedToRef<T>()
        {
            var result = CreateParserForwardedToRef<T>(out var assignParser);
            return Tuple.Create(result, assignParser);
        }

        public static Parser<char, T> CreateParserForwardedToRef<T>(out Action<Parser<char, T>> assignParser)
        {
            Parser<char, T> refP = null;

            assignParser = realParser => refP = realParser;

            return new Parser<char, T>(input =>
            {
                if (refP == null)
                    return Parser<char, T>.OfFailure("", "Uninitialized forwarded reference", input);

                return refP.Run(input);
            });
        }

        //public static Parser<int> Index => new Parser<int>(input => new Success<int>(input.Position, input));
        public static Parser<char, int> Line => new Parser<char, int>(input => new Parser<char, int>.Success(input.Position.Line, input));

        public static Parser<char, int> Column => new Parser<char, int>(input => new Parser<char, int>.Success(input.Position.Column, input));

        public static Parser<char, char> AnyOf(string items) =>
            Satisfy(items.Contains).WithLabel($"any of [{items}]");

        public static Parser<char, char> WhitespaceChar =>
              Satisfy(char.IsWhiteSpace);

        public static Parser<char, Unit> SkipWhitespaceChar =>
              SkipSatisfy(char.IsWhiteSpace);

        public static Parser<char, Unit> SkipSpaces => SkipWhitespaceChar.ZeroOrMore().Skip();

        public static Parser<char, string> Spaces =>
            from spaces in WhitespaceChar.ZeroOrMore()
            select new string(spaces.ToArray());

        public static Parser<char, Unit> SkipSpaces1 => WhitespaceChar.OneOrMore().Skip();

        public static Parser<char, string> Spaces1 =>
            from spaces in WhitespaceChar.OneOrMore()
            select new string(spaces.ToArray());

        public static Parser<char, char> Digit =>
            Satisfy(char.IsDigit);

        public static Parser<char, char> Char(char c) =>
            Satisfy(c1 => c1 == c).WithLabel($"'{c}'");

        public static Parser<char, char> CharExcept(params char[] cs) =>
            Satisfy(c1 => !cs.Contains(c1));

        public static Parser<char, Unit> SkipChar(char c) =>
            SkipSatisfy(c1 => c1 == c).WithLabel($"'{c}'");

        public static Parser<char, char> Letter =>
           Satisfy(char.IsLetter).WithLabel("letter");

        public static Parser<char, T> Failure<T>(string message) =>
            new Parser<char, T>(_ => new Parser<char, T>.Failure("", message, new NullReader<char>()));

        public static Parser<char, Unit> EndOfStream => new Parser<char, Unit>(input =>
        {
            var rest = input.Rest;
            return input.AtEnd
                ? Parser<char, Unit>.OfSuccess(Unit.Default, input)
                : Parser<char, Unit>.OfFailure("", $"Unexpected '{rest.First}'", rest);
        });

        public static Parser<char, Unit> SkipSatisfy(Predicate<char> predicate) =>
            new Parser<char, Unit>(input =>
            {
                var c = input.Rest;
                return input.AtEnd
                    ? Parser<char, Unit>.OfFailure("", "No more input", input)
                    : predicate(c.First)
                        ? Parser<char, Unit>.OfSuccess(Unit.Default, c)
                        : Parser<char, Unit>.OfFailure("", $"Unexpected '{c.First}'", c);
            });

        public static Parser<char, char> Satisfy(Predicate<char> predicate) =>
            new Parser<char, char>(input => input.AtEnd
                ? Parser<char, char>.OfFailure("", "No more input", input)
                : (predicate(input.First)
                    ? Parser<char, char>.OfSuccess(input.First, input.Rest)
                    : Parser<char, char>.OfFailure("", $"Unexpected '{input.First}'", input)));

        //public static Parser<string> NewLine => new Parser<string>(input =>
        //{
        //    var result = input.ScanEndOfLine();
        //    return result.Item2 == null
        //        ? Result.Failure<string>("", "", input.Position)
        //        : Result.Success(result.Item2, result.Item1);
        //});

        public static Parser<char, string> ManyChars(char value) =>
            value.Pure().Parser<char>().ZeroOrMore().ToStringParser();

        public static Parser<char, string> ManyChars1(char value) =>
            value.Pure().Parser<char>().OneOrMore().ToStringParser();

        public static Parser<char, Unit> SkipString(string value) =>
            String(value).Skip();

        public static Parser<char, string> String(string value) =>
            value
                .Select(Char)
                .ToSequence()
                .ToStringParser()
                .WithLabel(value);

        public static Parser<char, int> IntParser
        {
            get
            {
                var digits = from dgts in Digits
                             select int.Parse(dgts);

                return (from t in Sign.AndThen(digits)
                        let hasSign = t.Item1
                        let value = t.Item2
                        select hasSign ? -value : value).WithLabel("digit");
            }
        }

        private static Parser<char, string> Digits => Digit.OneOrMore().ToStringParser();

        public static Parser<char, float> Float =>
            from hasSign in Sign
            from leftDigits in Digits
            from _ in Dot
            from rightDigits in Digits
            let f = float.Parse($"{leftDigits}.{rightDigits}")
            select hasSign ? -f : f;

        private static Parser<char, char> Dot => Char('.');

        private static Parser<char, bool> Sign => from ch in Char('-').Optional()
                                                  select ch.HasValue;
    }
}