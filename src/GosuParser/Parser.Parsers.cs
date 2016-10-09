using System;
using System.Collections.Generic;
using System.Linq;

namespace GosuParser
{

    public static partial class Parser
    {

        public static Tuple<Parser<T>, Action<Parser<T>>> CreateParserForwardedToRef<T>()
        {
            Action<Parser<T>> assignParser;
            var result = CreateParserForwardedToRef<T>(out assignParser);
            return Tuple.Create(result, assignParser);
        }

        public static Parser<T> CreateParserForwardedToRef<T>(out Action<Parser<T>> assignParser)
        {
            Parser<T> refP = null;

            assignParser = realParser => refP = realParser;

            return input =>
            {
                if (refP == null)
                    return Result.Failure<T>("", "Uninitialized forwarded reference", new Position("", 0, 0));

                return refP(input);
            };
        }
        

        public static Parser<char> AnyOf(string items) =>
            WithLabel(
                $"any of [{items}]",
                Choice(items.Select(Char)));

        public static Parser<char> WhitespaceChar() =>
              Satisfy(char.IsWhiteSpace);

        public static Parser<IEnumerable<char>> Spaces() =>
            ZeroOrMore(WhitespaceChar());

        public static Parser<IEnumerable<char>> Spaces1() =>
            OneOrMore(WhitespaceChar());

        public static Parser<char> Digit() =>
            Satisfy(char.IsDigit);

        public static Parser<char> Char(char c) =>
            Satisfy(c1 => c1 == c).WithLabel($"'{c}'");

        public static Parser<char> Satisfy(Predicate<char> predicate) =>
            input =>
            {
                var result = input.NextChar();
                var c = result.Item2;
                return !c.HasValue
                    ? Result.Failure<char>("", "No more input", input.Position)
                    : (predicate(c.Value)
                        ? Result.Success(c.Value, result.Item1)
                        : Result.Failure<char>("", $"Unexpected '{c.Value}'", input.Position));
            };

        public static Parser<string> ManyChars(char value) =>
            from x in ZeroOrMore(value.Return())
            select new string(x.ToArray());

        public static Parser<string> ManyChars1(char value) =>
            from x in OneOrMore(value.Return())
            select new string(x.ToArray());

        public static Parser<string> String(string value) =>
            WithLabel(value, 
                from list in value.Select(Char).ToSequence()
                select new string(list.ToArray()));


        public static Parser<int> IntParser()
        {
            var digits = from dgts in Digits
                         select int.Parse(dgts);

            return WithLabel("digit",
                from t in Sign.AndThen(digits)
                let hasSign = t.Item1
                let value = t.Item2
                select hasSign ? -value : value);
        }

        private static Parser<string> Digits =>
                from d in OneOrMore(Digit())
                select new string(d.ToArray());

        public static Parser<float> Float => 
            from t in Sign.AndThen(Digits).TakeLeft(Dot).AndThen(Digits)
            let hasSign = t.Item1
            let leftDigits = t.Item2
            let rightDigits = t.Item3
            let f = float.Parse($"{leftDigits}.{rightDigits}")
            select hasSign ? -f : f;

        private static Parser<char> Dot => Char('.');
        private static Parser<bool> Sign => from ch in Optional(Char('-'))
                                             select ch.HasValue;
    }
}
