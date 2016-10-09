using System;

namespace GosuParser
{
    public class InputState
    {
        public string Text { get; }
        public int Index { get; }
        public Position Position { get; }

        string GetCurrentLine()
        {
            if (Index >= Text.Length)
                return "";

            var pos = Text.IndexOf('\n', Index);

            return (pos == -1
                       ? Text.Substring(Index)
                       : Text.Substring(Index, pos - Index));
        }

        private InputState(string text, int index, Position position = null)
        {
            Text = text;
            Index = index;
            Position = position ?? new Position(GetCurrentLine(), 0, 0);
        }

        private InputState WithPosition(Position newPosition) =>
            new InputState(Text, Index + 1, newPosition);
        
        public Tuple<InputState, char?> NextChar()
        {
            if(this.Index >= this.Text.Length)
                return NewTransition(null, this);
            
            var c = this.Text[this.Index];

            return NewTransition(c,
                c == '\n'
                    ? WithPosition(Position.NextLine(GetCurrentLine()))
                    : WithPosition(Position.NextColumn()))
                ;
        }

        private static Tuple<InputState, char?> NewTransition(char? c, InputState newState) => 
            Tuple.Create(newState, c);

        public static InputState FromString(string str) => 
            new InputState(str ?? "", 0);
    }
}