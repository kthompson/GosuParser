namespace GosuParser.Tokens
{
    public class StringLitToken : Token
    {
        public StringLitToken(string text)
        {
            this.Text = text;
        }

        public override string Text { get; }

        public override string ToString() => "'" + this.Text + "'";
    }
}