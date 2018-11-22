namespace GosuParser
{
    public static class PureExtensions
    {
        public static PureValue<TResult> Pure<TResult>(this TResult value) => new PureValue<TResult>(value);
    }
}