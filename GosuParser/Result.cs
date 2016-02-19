using System;

namespace GosuParser
{
    public static class Result
    {
        public static Result<T> Success<T>(T current, InputState input) =>
            new Success<T>(current, input);

        public static Result<T> Failure<T>(string label, string message, ParserPosition position) =>
            new Failure<T>(label, message, position);

        public static TResult Match<T, TResult>(this Result<T> result, Func<string, string, ParserPosition, TResult> failureFunc,
            Func<T, InputState, TResult> successFunc)
        {
            if (result.IsSuccess)
            {
                var success = (Success<T>)result;
                return successFunc(success.Result, success.Input);
            }

            var failure = (Failure<T>)result;
            return failureFunc(failure.FailureLabel, failure.FailureText, failure.Position);
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

    public class ParserPosition
    {
        public string CurrentLine { get; }
        public int Line { get; }
        public int Column { get; }

        public ParserPosition(string currentLine, int line, int column)
        {
            CurrentLine = currentLine;
            Line = line;
            Column = column;
        }

        public static ParserPosition FromInputState(InputState input) =>
            new ParserPosition(
                input.CurrentLine,
                input.Position.Line,
                input.Position.Column);
    }

    public sealed class Failure<T> : Result<T>
    {
        public string FailureLabel { get; }
        public string FailureText { get; }
        public ParserPosition Position { get; }
        public override bool IsSuccess => false;

        public Failure(string label, string failure, ParserPosition position)
        {
            this.FailureLabel = label;
            this.FailureText = failure;
            this.Position = position;
        }

        public override string ToString()
        {
            var errorLine = this.Position.CurrentLine;
            var colPos = this.Position.Column;
            var linePos = this.Position.Line;
            var failureCaret = $"{new string('-', colPos)}^ {this.FailureText}";
            return $"Line:{linePos} Col:{colPos} Error parsing {FailureLabel}\n{errorLine}\n{failureCaret}";
        }
    }
}