using System;
using System.Linq;

namespace GosuParser.Input
{
    public abstract class Position : IComparable<Position>
    {
        protected abstract string CurrentLine { get; }

        public abstract int Line { get; }
        public abstract int Column { get; }

        public override string ToString() => $"{Line}:{Column}";

        public string LongString() => $"{CurrentLine}\n{string.Join("", CurrentLine.Take(Column - 1).Select(c => c == '\t' ? c : ' '))}^";

        public virtual int CompareTo(Position other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;

            var lineComparison = Line.CompareTo(other.Line);
            if (lineComparison != 0) return lineComparison;

            return Column.CompareTo(other.Column);
        }
    }
}