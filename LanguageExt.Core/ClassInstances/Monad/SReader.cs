﻿using System;

namespace LanguageExt.ClassInstances
{
    public struct SReader<Env, A> : ReaderMonadValue<Reader<Env, A>, Env, A>
    {
        static readonly Reader<Env, A> bottom = new Reader<Env, A>(_ => (default(A), default(Env), true));

        public (A, Env, bool) Eval(Reader<Env, A> r, Env env) =>
            r.eval(env);

        public Reader<Env, A> Lift((A, Env, bool) value) =>
            new Reader<Env, A>(_ => value);

        public Reader<Env, A> Lift(Func<Env, (A, Env, bool)> f) =>
            new Reader<Env, A>(f);

        public Reader<Env, A> Bottom => bottom;
    }
}