using System;
using System.Collections.Generic;
using GosuParser.Input;

namespace GosuParser
{
    public class Parser<I, T>
    {
        private readonly Func<Reader<I>, Result> func;

        public Parser(Func<Reader<I>, Result> func)
        {
            this.func = func;
        }

        public virtual Result Run(Reader<I> inputState) => func(inputState);

        public Parser<I, TResult> SelectMany<TResult>(Func<T, Parser<I, TResult>> f) =>
            new Parser<I, TResult>(input => this.Run(input).Match(FromFailure<TResult>,
                (value1, remainingInput) => f(value1).Run(remainingInput)));

        public Parser<I, TResult> SelectMany<TParser, TResult>(Func<T, Parser<I, TParser>> parserSelector,
            Func<T, TParser, TResult> resultSelector) =>
            this.SelectMany(t => parserSelector(t).SelectMany(tp => Parser<I, TResult>.Pure(resultSelector(t, tp))));

        public Parser<I, TResult> Select<TResult>(Func<T, TResult> f) =>
            this.SelectMany(value => Parser<I, TResult>.Pure(f(value)));

        public Parser<I, T> OrElse(Parser<I, T> parser2) =>
            new Parser<I, T>(input => Run(input).Match(
                _ => parser2.Run(input),
                (value, reader) => new Success(value, reader)));

        public Parser<I, IEnumerable<T>> ZeroOrOne()
        {
            var some = this.Select(x => new T[] { x });
            var none = (new T[] { }).Pure();

            return some
                .OrElse(none)
                .Select(x => (IEnumerable<T>)x);
        }

        private Parser<I, IEnumerable<T>>.Success ParseZeroOrMore(Reader<I> input) =>
            Run(input)
                .Match(
                    _ => new Parser<I, IEnumerable<T>>.Success(new List<T>(), input),
                    (firstValue, inputAfterFirstParse) =>
                    {
                        var result2 = ParseZeroOrMore(inputAfterFirstParse);
                        var values = firstValue.Cons(result2.Value);

                        return new Parser<I, IEnumerable<T>>.Success(values, result2.Input);
                    });

        public Parser<I, IEnumerable<T>> ZeroOrMore() =>
            new Parser<I, IEnumerable<T>>(this.ParseZeroOrMore);

        public Parser<I, IEnumerable<T>> OneOrMore() =>
            from head in this
            from tail in this.ZeroOrMore()
            select head.Cons(tail);

        public Parser<I, IEnumerable<T>> Many() => OneOrMore();

        public Parser<I, T> Where(Func<T, bool> predicate) =>
            new Parser<I, T>(input => this.Run(input)
                .Match(
                    failure => failure,
                    (value, newState) =>
                        predicate(value)
                            ? OfSuccess(value, newState)
                            : OfFailure("where predicate", "failed predicate")));

        public Parser<I, Unit> Skip() =>
            this.Select(_ => Unit.Default);

        public Parser<I, T> TakeLeft<T2>(Parser<I, T2> parser2) =>
            from t in this
            from _ in parser2
            select t;

        public Parser<I, T2> TakeRight<T2>(Parser<I, T2> parser2) =>
            from _ in this
            from t in parser2
            select t;

        public Parser<I, T> Between<T2, T3>(Parser<I, T2> left, Parser<I, T3> right) =>
            from _ in left
            from t in this
            from __ in right
            select (T)t;

        public Parser<I, IEnumerable<T>> SepBy1<T2>(Parser<I, T2> sep)
        {
            var manySepThenP = sep.TakeRight(this).ZeroOrMore();

            return this.AndThen(manySepThenP).Select(tuple => (IEnumerable<T>)tuple.Item1.Cons(tuple.Item2));
        }

        public Parser<I, IEnumerable<T>> SepBy<T2>(Parser<I, T2> sep) =>
            this.SepBy1(sep)
                .OrElse(((IEnumerable<T>)new T[] { }).Pure());

        public Parser<I, T> WithLabel(string label) =>
            new Parser<I, T>(input => Run(input)
                .Match<Result>(failure => new Failure(label, failure.FailureText, failure.Position), ToSuccess));

        public static Parser<I, T> Pure(T value) => new Parser<I, T>(reader => new Success(value, reader));

        private static Success ToSuccess(T value, Reader<I> reader) => new Success(value, reader);

        private static Parser<I, TResult>.Result FromFailure<TResult>(Failure failure) =>
            new Parser<I, TResult>.Failure(failure.Label, failure.FailureText, failure.Position);

        public static implicit operator Parser<I, T>(PureValue<T> value) => value.Parser<I>();

        public static Result OfSuccess(T value, Reader<I> input) => new Success(value, input);

        public static Result OfFailure(string label, string failureText, Position position = null) => new Failure(label, failureText, position);

        public abstract class Result
        {
            public abstract bool IsSuccess { get; }

            public abstract TResult Match<TResult>(Func<Failure, TResult> failureFunc,
                Func<T, Reader<I>, TResult> successFunc);
        }

        public sealed class Success : Result
        {
            public Success(T value, Reader<I> input)
            {
                this.Value = value;
                this.Input = input;
            }

            public T Value { get; }
            public Reader<I> Input { get; }
            public override bool IsSuccess => true;

            public override TResult Match<TResult>(Func<Failure, TResult> failureFunc, Func<T, Reader<I>, TResult> successFunc)
            {
                return successFunc(this.Value, this.Input);
            }

            public override string ToString() => $"Success: {Value}";
        }

        public sealed class Failure : Result
        {
            public string Label { get; }
            public string FailureText { get; }
            public Position Position { get; }
            public override bool IsSuccess => false;

            public override TResult Match<TResult>(Func<Failure, TResult> failureFunc, Func<T, Reader<I>, TResult> successFunc)
            {
                return failureFunc(this);
            }

            public Failure(string label, string failureText, Position position = null)
            {
                this.Label = label;
                this.FailureText = failureText;
                this.Position = position;
            }

            public override string ToString()
            {
                var label = string.IsNullOrEmpty(Label) ? ":" : " " + Label;

                if (this.Position != null)
                {
                    var colPos = this.Position.Column;
                    var linePos = this.Position.Line;
                    var indent = new string('-', colPos);

                    return $"Line:{linePos} Col:{colPos} Error parsing{label}\n{this.Position.LongString()} {this.FailureText}";
                }

                return $"Error parsing{label}: {this.FailureText}";
            }
        }
    }
}