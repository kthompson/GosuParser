using System.Text;

namespace GosuParser.Input
{
    public abstract class Reader<T>
    {
        public abstract T First { get; }
        public abstract Reader<T> Rest { get; }
        public abstract bool AtEnd { get; }
        public abstract Position Position { get; }

        public virtual Reader<T> Skip(int n)
        {
            var r = this;
            var cnt = n;
            while (cnt > 0)
            {
                r = r.Rest;
                cnt--;
            }

            return r;
        }

        public virtual string GetRemainingInput()
        {
            var sb = new StringBuilder();
            var rest = this;
            while (!rest.AtEnd)
            {
                sb.Append(rest.First);
                rest = rest.Rest;
            }
            sb.Append(rest.First);

            return sb.ToString();
        }
    }
}