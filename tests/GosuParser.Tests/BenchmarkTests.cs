using System.Linq;
using BenchmarkDotNet.Running;
using Xunit;

namespace GosuParser.Tests
{
    public class BenchmarkTests
    {
        [Fact]
        public void ParsingIsFast()
        {
            var report = BenchmarkRunner.Run<AnyOfBenchmark>();

            report.ToString();
        }
    }
}