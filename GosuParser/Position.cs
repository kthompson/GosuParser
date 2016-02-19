namespace GosuParser
{
    public class Position
    {
        public static readonly Position Initial = new Position(0, 0);

        public int Line { get; }
        public int Column { get; }

        public Position(int line, int column)
        {
            this.Line = line;
            this.Column = column;
        }

        public Position NextColumn() => new Position(this.Line, this.Column + 1);
        public Position NextLine() => new Position(this.Line + 1, 0);
    }
}