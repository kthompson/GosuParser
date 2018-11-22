namespace GosuParser.Tokens
{
    public class KeywordToken : Token
    {
        public KeywordToken(string chars)
        {
            this.Text = chars;
        }

        public override string Text { get; }

        public override string ToString() => "'" + this.Text + "'";
    }
}