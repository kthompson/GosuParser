using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GosuParser
{
    public static partial class Parser
    {
        public static Parser<char> AnyOf(string items) =>
            WithLabel(
                $"any of [{items}]",
                Choice(items.Select(Char)));

        public static Parser<char> WhitespaceChar() =>
              Satisfy(char.IsWhiteSpace, "whitespace");

        public static Parser<IEnumerable<char>> Spaces() =>
            ZeroOrMore(WhitespaceChar());

        public static Parser<IEnumerable<char>> Spaces1() =>
            OneOrMore(WhitespaceChar());

        public static Parser<char> Digit() =>
            Satisfy(char.IsDigit, "digit");

        public static Parser<char> Char(char c) =>
            Satisfy(c1 => c1 == c, $"'{c}'");

        public static Parser<char> Satisfy(Predicate<char> predicate, string label) =>
            Create(label,
                input =>
                {
                    var result = input.NextChar();
                    var c = result.Item2;
                    var remainingInput = result.Item1;
                    if (!c.HasValue)
                    {
                        var pos = ParserPosition.FromInputState(input);
                        return Result.Failure<char>(label, "No more input", pos);
                    }

                    var first = c.Value;
                    if (!predicate(first))
                    {
                        var pos = ParserPosition.FromInputState(input);
                        return Result.Failure<char>(label, $"Unexpected '{first}'", pos);
                    }

                    return Result.Success(first, remainingInput);
                });

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

            return
                from t in Sign.AndThen(digits)
                let hasSign = t.Item1
                let value = t.Item2
                select hasSign ? -value : value;
        }

        private static Parser<string> Digits =>
                from d in OneOrMore(Digit())
                select new string(d.ToArray());

        public static Parser<float> Float => 
            from t in Sign.AndThen(Digits).TakeLeft(Dot).AndThen(Digits)
            let hasSign = t.Item1.Item1
            let leftDigits = t.Item1.Item2
            let rightDigits = t.Item2
            let f = float.Parse($"{leftDigits}.{rightDigits}")
            select hasSign ? -f : f;

        private static Parser<char> Dot => Char('.');
        private static Parser<bool> Sign => from ch in Optional(Char('-'))
                                             select ch.HasValue;
    }
}
