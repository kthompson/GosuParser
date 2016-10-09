using System;
using System.Collections.Generic;
using System.Linq;

namespace GosuParser
{
    public delegate Result<T> Parser<T>(InputState InputState);

    public static partial class Parser
    {
        public static Parser<T> Create<T>(Func<InputState, Result<T>> fn) => input => fn(input);

        public static Parser<T> Choice<T>(this IEnumerable<Parser<T>> items) =>
            items.Aggregate((parser, parser1) => parser.OrElse(parser1));


        public static Result<T> Run<T>(this Parser<T> parser, InputState input) => parser(input);

        public static Result<T> Run<T>(this Parser<T> parser, string input) =>
            parser(InputState.FromString(input));
        
        public static Parser<Tuple<T1, T2>> AndThen<T1, T2>(this Parser<T1> parser, Parser<T2> parser2) =>
            from p1 in parser
            from p2 in parser2
            select Tuple.Create(p1, p2);

        public static Parser<Tuple<T1, T2, T3>> AndThen<T1, T2, T3>(this Parser<Tuple<T1, T2>> parser, Parser<T3> parser2) =>
            from p1 in parser
            from p2 in parser2
            select Tuple.Create(p1.Item1, p1.Item2, p2);

        public static Parser<Tuple<T1, T2, T3, T4>> AndThen<T1, T2, T3, T4>(
            this Parser<Tuple<T1, T2, T3>> parser, Parser<T4> parser2) =>
                from p1 in parser
                from p2 in parser2
                select Tuple.Create(p1.Item1, p1.Item2, p1.Item3, p2);

        public static Parser<T> OrElse<T>(this Parser<T> parser, Parser<T> parser2) =>
            input => parser(input).Match(
                _ => parser2(input),
                Result.Success);

        public static Parser<TResult> Select<T, TResult>(
            this Parser<T> parser,
            Func<T, TResult> func) =>
                Bind(parser, p =>
                    Return(func(p)));

        public static Parser<TResult> Cast<T, TResult>(this Parser<T> parser) =>
            parser.Select(t => (TResult)(object) t);

        public static Parser<TResult> SelectMany<T, TParser, TResult>(
            this Parser<T> parser,
            Func<T, Parser<TParser>> parserSelector,
            Func<T, TParser, TResult> resultSelector) =>
                Bind(parser, t =>
                    Bind(parserSelector(t), tp =>
                        Return(resultSelector(t, tp))));

        public static Parser<TResult> Bind<T, TResult>(this Parser<T> p, Func<T, Parser<TResult>> f) =>
            input => p(input)
                .Match(
                    failure => Result.FromFailure<T, TResult>(failure),
                    (value1, remainingInput) => f(value1)(remainingInput));

        //returnP <>
        public static Parser<T> Return<T>(this T obj) => input => Result.Success(obj, input);

        private static Func<Parser<T1>, Parser<T2>> Lift2<T1, T2>(Func<T1, T2> f) =>
            pT1 => f.Return().Apply(pT1);

        public static Func<Parser<T1>, Parser<T2>, Parser<T3>> Lift2<T1, T2, T3>(Func<T1, T2, T3> f) =>
            (pT1, pT2) => f.Return().Apply(pT1).Apply(pT2);

        // sequence
        public static Parser<IEnumerable<T>> ToSequence<T>(this IEnumerable<Parser<T>> enumerable)
        {
            var consP = Lift2<T, IEnumerable<T>, IEnumerable<T>>(Extensions.Cons);
            var parsers = enumerable as ICollection<Parser<T>>;
            var list = parsers ?? enumerable.ToList();

            if (list.Count == 0)
                return ((IEnumerable<T>) new List<T>()).Return();

            var head = list.First();
            var tail = list.Skip(1);

            return consP(head, tail.ToSequence());
        }


        private static IEnumerable<T> Yielder<T>(T head, IEnumerator<T> tail)
        {
            yield return head;

            using (tail)
            {
                while (tail.MoveNext())
                {
                    yield return tail.Current;
                }
            }
        }
        private static Success<IEnumerable<T>> ParseZeroOrMore<T>(Parser<T> parser, InputState input) =>
            parser
                .Run(input)
                .Match(
                    _ => new Success<IEnumerable<T>>(new List<T>(), input),
                    (firstValue, inputAfterFirstParse) =>
                    {
                        var result2 = ParseZeroOrMore(parser, inputAfterFirstParse);
                        var values = firstValue.Cons(result2.Result);

                        return new Success<IEnumerable<T>>(values, result2.Input);
                    });

        public static Parser<IEnumerable<T>> ZeroOrMore<T>(this Parser<T> parser) =>
            input => ParseZeroOrMore(parser, input);

        public static Parser<IEnumerable<T>> OneOrMore<T>(this Parser<T> parser) =>
            from head in parser
            from tail in parser.ZeroOrMore()
            select head.Cons(tail);

        public static Parser<IEnumerable<T>> Many<T>(this Parser<T> parser) => 
            OneOrMore(parser);

        public static Parser<T?> Optional<T>(this Parser<T> parser) where T : struct
        {
            var some = parser.Select(x => new T?(x));
            var none = default(T?).Return();

            return some.OrElse(none);
        }

        public static Parser<T1> TakeLeft<T1, T2>(this Parser<T1> parser, Parser<T2> parser2) =>
            from t in parser.AndThen(parser2)
            select t.Item1;

        public static Parser<T2> TakeRight<T1, T2>(this Parser<T1> parser, Parser<T2> parser2) =>
            from t in parser.AndThen(parser2)
            select t.Item2;

        public static Parser<T1> Between<T1, T2, T3>(this Parser<T1> parser, Parser<T2> left, Parser<T3> right) =>
            left.TakeRight(parser)
                .TakeLeft(right);

        public static Parser<IEnumerable<T1>> SepBy1<T1, T2>(this Parser<T1> parser, Parser<T2> sep)
        {
            var manySepThenP =
                ZeroOrMore(
                    TakeRight(sep, parser));

            return
                from tuple in parser.AndThen(manySepThenP)
                let head = tuple.Item1
                let tail = tuple.Item2
                select head.Cons(tail);
        }

        public static Parser<IEnumerable<T1>> SepBy<T1, T2>(this Parser<T1> parser, Parser<T2> sep) =>
            parser.SepBy1(sep)
                .OrElse(((IEnumerable<T1>) new T1[] {}).Return());


        public static Parser<T1> WithLabel<T1>(string label, Parser<T1> parser) =>
            parser.WithLabel(label);

        public static Parser<T1> WithLabel<T1>(this Parser<T1> parser, string label) =>
            input => parser(input)
                .Match(failure => Result.Failure<T1>(label, failure.FailureText, failure.Position),
                    Result.Success);
    }
}