using System;

// ReSharper disable RedundantDelegateCreation

namespace GosuParser
{
    public static partial class ParserExtensions
    {
        // applyP <*>
        public static Parser<I, TResult> Apply<I, T1, TResult>(this Parser<I, Func<T1, TResult>> funcParser, Parser<I, T1> xParser) =>
            from func in funcParser
            from x in xParser
            select func(x);

        public static Parser<I, Func<T2, TResult>> Apply<I, T1, T2, TResult>(this Parser<I, Func<T1, T2, TResult>> funcParser, Parser<I, T1> xParser) =>
            from func in funcParser
            from x in xParser
            select new Func<T2, TResult>((t2) => func(x, t2));

        public static Parser<I, Func<T2, T3, TResult>> Apply<I, T1, T2, T3, TResult>(this Parser<I, Func<T1, T2, T3, TResult>> funcParser, Parser<I, T1> xParser) =>
            from func in funcParser
            from x in xParser
            select new Func<T2, T3, TResult>((t2, t3) => func(x, t2, t3));

        public static Parser<I, Func<T2, T3, T4, TResult>> Apply<I, T1, T2, T3, T4, TResult>(this Parser<I, Func<T1, T2, T3, T4, TResult>> funcParser, Parser<I, T1> xParser) =>
            from func in funcParser
            from x in xParser
            select new Func<T2, T3, T4, TResult>((t2, t3, t4) => func(x, t2, t3, t4));

        public static Parser<I, Func<T2, T3, T4, T5, TResult>> Apply<I, T1, T2, T3, T4, T5, TResult>(this Parser<I, Func<T1, T2, T3, T4, T5, TResult>> funcParser, Parser<I, T1> xParser) =>
            from func in funcParser
            from x in xParser
            select new Func<T2, T3, T4, T5, TResult>((t2, t3, t4, t5) => func(x, t2, t3, t4, t5));

        public static Parser<I, Func<T2, T3, T4, T5, T6, TResult>> Apply<I, T1, T2, T3, T4, T5, T6, TResult>(this Parser<I, Func<T1, T2, T3, T4, T5, T6, TResult>> funcParser, Parser<I, T1> xParser) =>
            from func in funcParser
            from x in xParser
            select new Func<T2, T3, T4, T5, T6, TResult>((t2, t3, t4, t5, t6) => func(x, t2, t3, t4, t5, t6));

        public static Parser<I, Func<T2, T3, T4, T5, T6, T7, TResult>> Apply<I, T1, T2, T3, T4, T5, T6, T7, TResult>(this Parser<I, Func<T1, T2, T3, T4, T5, T6, T7, TResult>> funcParser, Parser<I, T1> xParser) =>
            from func in funcParser
            from x in xParser
            select new Func<T2, T3, T4, T5, T6, T7, TResult>((t2, t3, t4, t5, t6, t7) => func(x, t2, t3, t4, t5, t6, t7));

        public static Parser<I, Func<T2, T3, T4, T5, T6, T7, T8, TResult>> Apply<I, T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Parser<I, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> funcParser, Parser<I, T1> xParser) =>
            from func in funcParser
            from x in xParser
            select new Func<T2, T3, T4, T5, T6, T7, T8, TResult>((t2, t3, t4, t5, t6, t7, t8) => func(x, t2, t3, t4, t5, t6, t7, t8));
    }
}