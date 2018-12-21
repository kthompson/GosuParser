using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StringReader = GosuParser.Input.StringReader;

namespace GosuParser
{
    public static partial class ParserExtensions
    {
        public static Parser<I, string> ToStringParser<I>(this Parser<I, IEnumerable<char>> p) =>
            p.Select(list => new string(list.ToArray()));

        public static Parser<I, string> ToStringParser<I>(this Parser<I, Tuple<char, IEnumerable<char>>> p) =>
            p.Select(list => new string(list.Item1.Cons(list.Item2).ToArray()));

        public static Parser<I, T> Choice<I, T>(this IEnumerable<Parser<I, T>> items) =>
            new Parser<I, T>(input =>
            {
                var errors = new List<Parser<I, T>.Failure>();
                foreach (var parser in items)
                {
                    var result = parser.Run(input);
                    if (result.IsSuccess)
                        return result;

                    errors.Add((Parser<I, T>.Failure)result);
                }

                var failureLabel = errors
                    .Select(e => e.Label)
                    .Aggregate("", (seed, label) =>
                    {
                        if (string.IsNullOrEmpty(seed))
                            return label;

                        if (string.IsNullOrEmpty(label))
                            return seed;

                        return $"{seed}, {label}";
                    });

                var failureText = errors.Select(x => x.FailureText).FirstOrDefault() ?? "";

                return new Parser<I, T>.Failure($"Choice of [{failureLabel}]", failureText, input);
            });

        public static Parser<char, T>.Result Run<T>(this Parser<char, T> parser, string input) =>
            parser.Run(new StringReader(input));

        public static Parser<I, Tuple<T1, T2>> AndThen<I, T1, T2>(this Parser<I, T1> @this, Parser<I, T2> parser2) =>
            from p1 in @this
            from p2 in parser2
            select Tuple.Create(p1, p2);

        public static Parser<I, Tuple<T1, T2, T3>> AndThen<I, T1, T2, T3>(this Parser<I, Tuple<T1, T2>> parser, Parser<I, T3> parser2) =>
            from p1 in parser
            from p2 in parser2
            select Tuple.Create(p1.Item1, p1.Item2, p2);

        public static Parser<I, Tuple<T1, T2, T3, T4>> AndThen<I, T1, T2, T3, T4>(
            this Parser<I, Tuple<T1, T2, T3>> parser, Parser<I, T4> parser2) =>
                from p1 in parser
                from p2 in parser2
                select Tuple.Create(p1.Item1, p1.Item2, p1.Item3, p2);

        public static Parser<I, TResult> Cast<I, T, TResult>(this Parser<I, T> parser) =>
            parser.Select(t => (TResult)(object)t);

        public static Parser<I, T> LazyReturn<I, T>(Func<T> func) => new Parser<I, T>(input => Parser<I, T>.OfSuccess(func(), input));

        private static Func<Parser<I, T1>, Parser<I, T2>> Lift2<I, T1, T2>(Func<T1, T2> f) =>
            pT1 => f.Pure().Parser<I>().Apply(pT1);

        public static Func<Parser<I, T1>, Parser<I, T2>, Parser<I, T3>> Lift2<I, T1, T2, T3>(Func<T1, T2, T3> f) =>
            (pT1, pT2) => f.Pure().Parser<I>().Apply(pT1).Apply(pT2);

        // sequence
        public static Parser<I, IEnumerable<T>> ToSequence<I, T>(this IEnumerable<Parser<I, T>> enumerable)
        {
            var consP = Lift2<I, T, IEnumerable<T>, IEnumerable<T>>(Extensions.Cons);
            var parsers = enumerable as ICollection<Parser<I, T>>;
            var list = parsers ?? enumerable.ToList();

            if (list.Count == 0)
                return ((IEnumerable<T>)new List<T>()).Pure().Parser<I>();

            var head = list.First();
            var tail = list.Skip(1);

            return consP(head, tail.ToSequence());
        }

        public static Parser<I, T?> Optional<I, T>(this Parser<I, T> parser) where T : struct
        {
            var some = parser.Select(x => new T?(x));
            var none = (default(T?)).Pure();

            return some.OrElse(none);
        }

        public static Parser<I, Unit> Optional<I>(this Parser<I, Unit> parser) =>
            parser.OrElse(Unit.Default.Pure());
    }
}