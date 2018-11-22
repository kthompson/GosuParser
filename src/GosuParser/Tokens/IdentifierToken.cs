namespace GosuParser.Tokens
{
    public class IdentifierToken : Token
    {
        public IdentifierToken(string text)
        {
            this.Text = text;
        }

        public override string Text { get; }

        public override string ToString() => "identifier " + this.Text;
    }
}