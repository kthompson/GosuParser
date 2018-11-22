namespace GosuParser.Tokens
{
    public sealed class ErrorToken : Token
    {
        public ErrorToken(string msg)
        {
            Text = "*** error: " + msg;
        }

        public override string Text { get; }
    }
}