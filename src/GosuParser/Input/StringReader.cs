namespace GosuParser.Input
{
    public class StringReader : Reader<char>
    {
        public static char EndOfReader = (char)0x1a;

        public string Source { get; }
        public int Offset { get; }

        public StringReader(string source, int offset = 0)
        {
            Source = source;
            Offset = offset;
        }

        public override Reader<char> Skip(int n) => new StringReader(Source, Offset + n);

        public override char First => AtEnd ? EndOfReader : Source[Offset];
        public override Reader<char> Rest => AtEnd ? this : new StringReader(this.Source, Offset + 1);
        public override Position Position => new OffsetPosition(Source, Offset);
        public override bool AtEnd => Offset >= Source.Length;

        public override string GetRemainingInput()
        {
            return Source.Substring(Offset);
        }

        public override string ToString()
        {
            var c = AtEnd ? "" : $"'{First}', ...";
            return $"StringReader({c})";
        }
    }
}