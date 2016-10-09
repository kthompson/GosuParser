using System;

namespace GosuParser
{
    public static class Result
    {
        public static Result<T> Success<T>(T current, InputState input) =>
            new Success<T>(current, input);

        public static Result<T> Failure<T>(string label, string failureText, Position position) =>
            new Failure<T>(label, failureText, position);

        public static Result<TResult> FromFailure<T, TResult>(Failure<T> failure) =>
            new Failure<TResult>(failure.Label, failure.FailureText, failure.Position);

        public static TResult Match<T, TResult>(this Result<T> result, Func<Failure<T>, TResult> failureFunc,
            Func<T, InputState, TResult> successFunc)
        {
            if (result.IsSuccess)
            {
                var success = (Success<T>)result;
                return successFunc(success.Result, success.Input);
            }

            var failure = (Failure<T>)result;
            return failureFunc(failure);
        }
    }

    public abstract class Result<T>
    {
        public abstract bool IsSuccess { get; }
    }

    public sealed class Success<T> : Result<T>
    {
        public Success(T result, InputState input)
        {
            this.Result = result;
            this.Input = input;
        }

        public T Result { get; }
        public InputState Input { get; }
        public override bool IsSuccess => true;
    }

    public sealed class Failure<T> : Result<T>
    {
        public string Label { get; }
        public string FailureText { get; }
        public Position Position { get; }
        public override bool IsSuccess => false;

        public Failure(string label, string failureText, Position position)
        {
            this.Label = label;
            this.FailureText = failureText;
            this.Position = position;
        }

        public override string ToString()
        {
            var errorLine = this.Position.CurrentLine;
            var colPos = this.Position.Column;
            var linePos = this.Position.Line;
            var indent = new string('-', colPos);
            var label = string.IsNullOrEmpty(Label) ? ":" : " " + Label;
            return $"Line:{linePos} Col:{colPos} Error parsing{label}\n{errorLine}\n{indent}^ {this.FailureText}";
        }
    }
}