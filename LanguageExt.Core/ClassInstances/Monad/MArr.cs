﻿using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using System;
using System.Linq;
using System.Diagnostics.Contracts;
using LanguageExt;
using System.Collections.Generic;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Array type-class instance
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    public struct MArr<A> :
        Monad<Arr<A>, A>,
        Eq<Arr<A>>,
        Ord<Arr<A>>,
        Monoid<Arr<A>>
   {
        public static readonly MArr<A> Inst = default(MArr<A>);

        [Pure]
        public Arr<A> Append(Arr<A> x, Arr<A> y) =>
            x.ConcatFast(y).ToArray();

        [Pure]
        public MB Bind<MONADB, MB, B>(Arr<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            traverse<MArr<A>, MONADB, Arr<A>, MB, A, B>(ma, f);

        [Pure]
        public MB BindAsync<MONADB, MB, B>(Arr<A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            traverseSyncAsync<MArr<A>, MONADB, Arr<A>, MB, A, B>(ma, f);

        [Pure]
        public Func<Unit, int> Count(Arr<A> fa) =>
            _ => fa.Count();

        [Pure]
        public Arr<A> Empty() =>
            Arr<A>.Empty;

        [Pure]
        public bool Equals(Arr<A> x, Arr<A> y) =>
            ((System.Collections.IStructuralEquatable)x.Value).Equals(y.Value, EqualityComparer<A>.Default);

        [Pure]
        public int Compare(Arr<A> x, Arr<A> y) =>
            ((System.Collections.IStructuralComparable)x.Value).CompareTo(y.Value, Comparer<A>.Default);

        [Pure]
        public Arr<A> Fail(object err = null) =>
            Empty();

        [Pure]
        public Func<Unit, S> Fold<S>(Arr<A> fa, S state, Func<S, A, S> f) =>
            _ => fa.Fold(state, f);

        [Pure]
        public Func<Unit, S> FoldBack<S>(Arr<A> fa, S state, Func<S, A, S> f) =>
            _ => fa.FoldBack(state, f);

        [Pure]
        public Arr<A> Plus(Arr<A> ma, Arr<A> mb) =>
            ma + mb;

        [Pure]
        public Arr<A> Return(Func<Unit, A> f) =>
            Arr.create(f(unit));

        [Pure]
        public Arr<A> Return(A x) =>
            Return(_ => x);

        [Pure]
        public Arr<A> Zero() =>
            Empty();

        [Pure]
        public int GetHashCode(Arr<A> x) =>
            x.GetHashCode();

        [Pure]
        public Arr<A> Run(Func<Unit, Arr<A>> f) =>
            f(unit);

        [Pure]
        public Arr<A> BindReturn(Unit _, Arr<A> fmb) =>
            fmb;

        [Pure]
        public Arr<A> Apply(Func<A, A, A> f, Arr<A> fa, Arr<A> fb) =>
            new Arr<A>(
                from a in fa
                from b in fb
                select f(a, b));
    }
}
