using System;
using System.Collections.Generic;

namespace GosuParser.Input
{
    internal class OffsetPosition : Position
    {
        public string Source { get; }
        public int Offset { get; }
        private readonly Lazy<int[]> _index;
        private readonly Lazy<int> _line;

        public OffsetPosition(string source, int offset)
        {
            Source = source;
            Offset = offset;
            _index = new Lazy<int[]>(CreateIndex);
            _line = new Lazy<int>(CreateLine);
        }

        private int CreateLine()
        {
            // use binary search to determine the line we are on
            var index = Index;
            var lo = 0;
            var hi = index.Length - 1;
            while (lo + 1 < hi)
            {
                var mid = (hi + lo) / 2;
                if (Offset < index[mid])
                {
                    hi = mid;
                }
                else
                {
                    lo = mid;
                }
            }

            return lo + 1;
        }

        /// <summary>
        /// Create an index with all of the start of lines
        /// </summary>
        /// <returns></returns>
        private int[] CreateIndex()
        {
            var lineStarts = new List<int> { 0 };
            for (int i = 0; i < Source.Length; i++)
            {
                if (Source[i] == '\n')
                {
                    lineStarts.Add(i + 1);
                }
            }
            lineStarts.Add(Source.Length);
            return lineStarts.ToArray();
        }

        protected override string CurrentLine
        {
            get
            {
                var index = _index.Value;
                var lineStart = index[Line - 1];
                var lineEnd = index[Line];
                var endIndex = lineStart < lineEnd && Source[lineEnd - 1] == '\n' ? lineEnd - 1 : lineEnd;
                return Source.Substring(lineStart, endIndex - lineStart);
            }
        }

        public int[] Index => _index.Value;
        public override int Line => _line.Value;

        public override int Column => Offset - Index[Line - 1] + 1;
    }
}