using System;

namespace GosuParser
{
    public class InputState
    {
        public string[] Lines { get; }
        public Position Position { get; }

        public InputState(string[] lines, Position position)
        {
            Lines = lines;
            Position = position;
        }

        public InputState WithPosition(Position newPosition) => 
            new InputState(Lines, newPosition);

        public string CurrentLine
        {
            get
            {
                var linePos = this.Position.Line;
                if (linePos < this.Lines.Length)
                    return this.Lines[linePos];

                return "end of file";
            }
        }

        public Tuple<InputState, char?> NextChar()
        {
            var linePos = this.Position.Line;
            var colPos = this.Position.Column;

            if (linePos >= this.Lines.Length)
                return NewTransition(null, this);

            var currentLine = this.CurrentLine;
            if (colPos < currentLine.Length)
            {
                var c = currentLine[colPos];
                return NewTransition(c, 
                    WithPosition(
                        Position.NextColumn()));
            }

            return NewTransition('\n',
                WithPosition(Position.NextLine()));
        }

        private static Tuple<InputState, char?> NewTransition(char? c, InputState newState) => 
            Tuple.Create(newState, c);

        public static InputState FromString(string str) => 
            new InputState(StringToLines(str), Position.Initial);

        private static string[] StringToLines(string str) =>
            string.IsNullOrEmpty(str)
                ? new string[0]
                : str.Split(new[] {"\r\n", "\n"}, StringSplitOptions.None);
    }
}