namespace GosuParser.Input
{
    public class NoPosition : Position
    {
        private NoPosition()
        {
        }

        protected override string CurrentLine => "";

        public override int Line => 0;
        public override int Column => 0;

        public override string ToString()
        {
            return "<undefined position>";
        }

        public static readonly NoPosition Default = new NoPosition();
    }
}