using System;
using GosuParser.Input;

namespace GosuParser
{
    //public static class Result
    //{
    //    public static Result<T> Success<T>(T current, Reader<char> input) =>
    //        new Success<T>(current, input);

    //    public static Result<T> Failure<T>(string label, string failureText, Position position = null) =>
    //        new Failure<T>(label, failureText, position);

    //    public static Result<TResult> FromFailure<T, TResult>(Failure<T> failure) =>
    //        new Failure<TResult>(failure.Label, failure.FailureText, failure.Position);

    //    public static TResult Match<T, TResult>(this Result<T> result, Func<Failure<T>, TResult> failureFunc,
    //        Func<T, Reader<char>, TResult> successFunc)
    //    {
    //        if (result.IsSuccess)
    //        {
    //            var success = (Success<T>)result;
    //            return successFunc(success.Result, success.RemainingInput);
    //        }

    //        var failure = (Failure<T>)result;
    //        return failureFunc(failure);
    //    }
    //}

    //public abstract class Result<T>
    //{
    //    public abstract bool IsSuccess { get; }
    //}

    //public sealed class Success<T> : Result<T>
    //{
    //    public Success(T result, Reader<char> input)
    //    {
    //        this.Result = result;
    //        this.RemainingInput = input;
    //    }

    //    public T Result { get; }
    //    public Reader<char> RemainingInput { get; }
    //    public override bool IsSuccess => true;

    //    public override string ToString() => $"Success: {Result}";
    //}

    //public sealed class Failure<T> : Result<T>
    //{
    //    public string Label { get; }
    //    public string FailureText { get; }
    //    public Position Position { get; }
    //    public override bool IsSuccess => false;

    //    public Failure(string label, string failureText, Position position = null)
    //    {
    //        this.Label = label;
    //        this.FailureText = failureText;
    //        this.Position = position;
    //    }

    //    public override string ToString()
    //    {
    //        var label = string.IsNullOrEmpty(Label) ? ":" : " " + Label;

    //        if (this.Position != null)
    //        {
    //            var colPos = this.Position.Column;
    //            var linePos = this.Position.Line;
    //            var indent = new string('-', colPos);

    //            return $"Line:{linePos} Col:{colPos} Error parsing{label}\n{this.Position.LongString()} {this.FailureText}";
    //        }

    //        return $"Error parsing{label}: {this.FailureText}";
    //    }
    //}
}