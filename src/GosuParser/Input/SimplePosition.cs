namespace GosuParser.Input
{
    internal class SimplePosition : Position
    {
        public SimplePosition(string currentLine, int line, int column)
        {
            CurrentLine = currentLine;
            Line = line;
            Column = column;
        }

        protected override string CurrentLine { get; }
        public override int Line { get; }
        public override int Column { get; }
    }
}