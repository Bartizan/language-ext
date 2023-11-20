using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    [Obsolete(Change.UseEffMonadInstead)]
    public static class EitherAsyncGuardExtensions
    {
        public static EitherAsync<L, Unit> ToEitherAsync<L>(this Guard<L, Unit> ma) =>
            ma.Flag
                ? RightAsync<L, Unit>(unit)
                : LeftAsync<L, Unit>(ma.OnFalse());
        
        public static EitherAsync<L, B> SelectMany<L, B>(this Guard<L, Unit> ma, Func<Unit, EitherAsync<L, B>> f) =>
            ma.Flag
                ? f(default)
                : LeftAsync<L, B>(ma.OnFalse());

        public static EitherAsync<L, C> SelectMany<L, B, C>(this Guard<L, Unit> ma, Func<Unit, EitherAsync<L, B>> bind, Func<Unit, B, C> project) =>
            ma.Flag
                ? bind(default).Map(b => project(default, b))
                : LeftAsync<L, C>(ma.OnFalse());

        public static EitherAsync<L, Unit> SelectMany<L, A>(this EitherAsync<L, A> ma, Func<A, Guard<L, Unit>> f) =>
            ma.Bind(a => f(a).ToEitherAsync());

        public static EitherAsync<L, C> SelectMany<L, A, C>(this EitherAsync<L, A> ma, Func<A, Guard<L, Unit>> bind, Func<A, Unit, C> project) =>
            ma.Bind(a => bind(a).ToEitherAsync().Map(_ => project(a, default)));
    }
}
