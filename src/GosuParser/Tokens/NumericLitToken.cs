namespace GosuParser.Tokens
{
    public class NumericLitToken : Token
    {
        public NumericLitToken(string text)
        {
            this.Text = text;
        }

        public override string Text { get; }

        public override string ToString()
        {
            return this.Text;
        }
    }
}