using System;
using System.Collections.Generic;
using System.Linq;
using GosuParser.Tokens;

namespace GosuParser.Lexical
{
    public class StandardLexical : Scanners<Token>
    {
        public StandardLexical()
        {
            this.ReservedWords = new HashSet<string>();
            this.Delimiters = new HashSet<string>();

            _delim = new Lazy<Parser<char, Token>>(BuildDelimiters);
        }

        public HashSet<string> ReservedWords { get; }
        public HashSet<string> Delimiters { get; }

        public Parser<char, char> IdentChar => CharacterParsers.Letter.OrElse(CharacterParsers.Char('_'));

        protected virtual Parser<char, Unit> Comment
        {
            get
            {
                var comment = CharacterParsers.CreateParserForwardedToRef<Unit>(out var setComment);

                var comment1 = CharacterParsers.CharExcept(CharacterParsers.EndOfReader, '*').ZeroOrMore().AndThen(CharacterParsers.String("*/")).Skip();
                var comment2 = CharacterParsers.CharExcept(CharacterParsers.EndOfReader, '*').ZeroOrMore().AndThen(CharacterParsers.Char('*')).AndThen(comment).Skip();

                setComment(comment1.OrElse(comment2));

                return comment;
            }
        }

        protected override Token ErrorToken(string msg) => new ErrorToken(msg);

        protected override Parser<char, Token> Token
        {
            get
            {
                var token1 = IdentChar
                    .AndThen(IdentChar.OrElse(CharacterParsers.Digit).ZeroOrMore()).ToStringParser()
                    .Select(ProcessIndent);

                var numericToken = CharacterParsers.Digit.AndThen(CharacterParsers.Digit.ZeroOrMore()).ToStringParser()
                    .Select(value => (Token)new NumericLitToken(value));

                var stringToken1 = CharacterParsers.CharExcept('\'', '\n', CharacterParsers.EndOfReader).ZeroOrMore().Between(CharacterParsers.Char('\''), CharacterParsers.Char('\'')).ToStringParser()
                    .Select(value => (Token)new StringLitToken(value));

                var stringToken2 = CharacterParsers.CharExcept('"', '\n', CharacterParsers.EndOfReader).ZeroOrMore().Between(CharacterParsers.Char('"'), CharacterParsers.Char('"')).ToStringParser()
                    .Select(value => (Token)new StringLitToken(value));

                var eof = CharacterParsers.Char(CharacterParsers.EndOfReader).Select(value => (Token)EndOfFileToken.Default);

                var badString1 = CharacterParsers.Char('"').TakeRight(CharacterParsers.Failure<Token>("unclosed string literal"));
                var badString2 = CharacterParsers.Char('\'').TakeRight(CharacterParsers.Failure<Token>("unclosed string literal"));

                return new[]
                {
                    token1,
                    numericToken,
                    stringToken1,
                    stringToken2,
                    eof,
                    badString1,
                    badString2,
                    Delim,
                    CharacterParsers.Failure<Token>("illegal character")
                }.Choice();
            }
        }

        private Parser<char, Token> BuildDelimiters() =>
            this.Delimiters.OrderBy(x => x.Length)
                .Select(value => CharacterParsers.String(value).Select(_ => (Token)new KeywordToken(value)))
                .Aggregate(CharacterParsers.Failure<Token>("no matching delimiter"), (left, right) => right.OrElse(left));

        private readonly Lazy<Parser<char, Token>> _delim;
        protected Parser<char, Token> Delim => _delim.Value;

        private Token ProcessIndent(string name) =>
            ReservedWords.Contains(name) ? (Token)new KeywordToken(name) : new IdentifierToken(name);

        protected override Parser<char, Unit> Whitespace =>
            CharacterParsers.WhitespaceChar.Skip()
                .OrElse(CharacterParsers.String("/*").AndThen(Comment).Skip())
                .OrElse(CharacterParsers.String("//").AndThen(CharacterParsers.CharExcept(CharacterParsers.EndOfReader, '\n').ZeroOrMore()).Skip())
                .OrElse(CharacterParsers.String("/*").TakeRight(CharacterParsers.Failure<Unit>("unclosed comment")));
    }
}