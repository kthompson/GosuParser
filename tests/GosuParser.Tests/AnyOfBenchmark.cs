using BenchmarkDotNet.Attributes;
using GosuParser.Input;
using static GosuParser.CharacterParsers;

namespace GosuParser.Tests
{
    public class AnyOfBenchmark
    {
        private readonly Parser<char, char> _digitParser = AnyOf("0123456789");
        private readonly Parser<char, char> _digitParserIsDigit = Satisfy(char.IsDigit);

        private readonly Parser<char, char> _digitParserSwitch = Satisfy(x =>
        {
            switch (x)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return true;

                default:
                    return false;
            }
        });

        private readonly Reader<char> _initialInputState1 = new StringReader("1");
        private readonly Reader<char> _initialInputState9 = new StringReader("9");
        private readonly Reader<char> _initialInputStateA = new StringReader("A");

        [Params("AnyOf", "Switch", "IsDigit")]
        public string Parser;

        private Parser<char, char> GetParser()
        {
            switch (Parser)
            {
                case "AnyOf":
                    return _digitParser;

                case "Switch":
                    return _digitParserSwitch;

                case "IsDigit":
                    return _digitParserIsDigit;

                default:
                    return null;
            }
        }

        [Benchmark]
        public void DigitParsesOne()
        {
            var result = GosuParser.ParserExtensions.Run(GetParser(), _initialInputState1);
        }

        [Benchmark]
        public void DigitParsesNine()
        {
            var result = GosuParser.ParserExtensions.Run(GetParser(), _initialInputState9);
        }

        [Benchmark]
        public void DigitParsesA()
        {
            var result = GosuParser.ParserExtensions.Run(GetParser(), _initialInputStateA);
        }
    }
}