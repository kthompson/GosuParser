namespace GosuParser.Tokens
{
    public sealed class EndOfFileToken : Token
    {
        private EndOfFileToken()
        {
            Text = "<eof>";
        }

        public override string Text { get; }

        public static readonly EndOfFileToken Default = new EndOfFileToken();
    }
}