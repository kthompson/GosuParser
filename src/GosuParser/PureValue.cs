namespace GosuParser
{
    public class PureValue<TResult>
    {
        private readonly TResult _value;

        public PureValue(TResult value)
        {
            _value = value;
        }

        public Parser<I, TResult> Parser<I>() => new Parser<I, TResult>(reader => new Parser<I, TResult>.Success(_value, reader));
    }
}