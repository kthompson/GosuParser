using System;
using GosuParser.Input;

namespace GosuParser
{
    public class InputState
    {
        public string Text { get; }
        public int Index { get; }
        public int Line { get; }
        public int Column { get; }

        public Position Position => new SimplePosition(GetCurrentLine(), Line, Column);

        public string RemainingInput => GetCurrentLine(0);

        private string GetCurrentLine(int? column = null)
        {
            var offset = Index - (column ?? Column);
            if (offset >= Text.Length)
                return "";

            var pos = Text.IndexOf('\n', offset);

            return (pos == -1
                       ? Text.Substring(offset)
                       : Text.Substring(offset, pos - offset));
        }

        private InputState(string text, int index = 0, int line = 1, int column = 0)
        {
            Text = text;
            Index = index;
            Line = line;
            Column = column;
        }

        private InputState WithPosition(int line, int column) =>
            new InputState(Text, Index + 1, line, column);

        public char? PeekChar() =>
            this.Index >= this.Text.Length
                ? (char?)null
                : this.Text[this.Index];

        public Tuple<InputState, string> ScanEndOfLine()
        {
            var c = PeekChar();
            if (c == null)
            {
                return Tuple.Create(this, (string)null);
            }
            var ch = c.Value;
            switch (ch)
            {
                case '\r':
                    var state = Advance();
                    if (state.PeekChar() == '\n')
                    {
                        return Tuple.Create(state.Advance(true), "\r\n");
                    }

                    return Tuple.Create(state, "\r");

                case '\n':
                    return Tuple.Create(Advance(true), "\n");

                default:
                    if (ch == '\u0085' || ch == '\u2028' || ch == '\u2029')
                    {
                        return Tuple.Create(Advance(), ch.ToString());
                    }

                    return Tuple.Create(this, (string)null);
            }
        }

        public Tuple<InputState, char?> NextChar()
        {
            if (this.Index >= this.Text.Length)
                return NewTransition(null, this);

            var c = this.Text[this.Index];

            return NewTransition(c, Advance(c == '\n'));
        }

        public InputState Advance(bool newLine = false) =>
            !newLine
                ? WithPosition(Line, Column + 1)
                : WithPosition(Line + 1, 0);

        private static Tuple<InputState, char?> NewTransition(char? c, InputState newState) =>
            Tuple.Create(newState, c);

        public static InputState FromString(string str) =>
            new InputState(str ?? "");
    }
}