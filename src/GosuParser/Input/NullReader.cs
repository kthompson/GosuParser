using System;

namespace GosuParser.Input
{
    public class NullReader<T> : Reader<T>
    {
        public override T First => throw new InvalidOperationException("NullReader.First");
        public override Reader<T> Rest => this;
        public override bool AtEnd => true;
        public override Position Position => NoPosition.Default;
    }
}