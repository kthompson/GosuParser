namespace GosuParser
{
    public class Position
    {
        public string CurrentLine { get; }
        public int Line { get; }
        public int Column { get; }

        public string RemainingInput =>
            Column > CurrentLine.Length
                ? ""
                : CurrentLine.Substring(Column);

        public Position(string currentLine, int line, int column)
        {
            this.Line = line;
            this.Column = column;
            this.CurrentLine = currentLine;
        }

        public Position NextColumn() => new Position(this.CurrentLine, this.Line, this.Column + 1);
        public Position NextLine(string currentLine) => new Position(currentLine, this.Line + 1, 0);
    }
}