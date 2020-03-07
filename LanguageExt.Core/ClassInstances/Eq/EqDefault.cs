﻿using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Finds an appropriate Eq from the loaded assemblies, if one can't be found then it
    /// falls back to the standard .NET EqualityComparer<A>.Default.Equals(a,b) method to
    /// provide equality testing.
    /// </summary>
    public struct EqDefault<A> : Eq<A>
    {
        public static readonly EqDefault<A> Inst = default(EqDefault<A>);

        static readonly Func<A, A, bool> eq;

        static EqDefault()
        {
            if (Reflect.IsFunc(typeof(A)))
            {
                eq = GetEq("Try", typeof(EqTry<>)) ?? 
                     GetEq("TryOption", typeof(EqTryOption<>)) ??
                     GetEq("TryAsync", typeof(EqTryAsync<>)) ??
                     GetEq("TryOptionAsync", typeof(EqTryOptionAsync<>)) ??
                     new Func<A, A, bool>((a, b) => ReferenceEquals(a, b));
            }
            else if (Reflect.IsAnonymous(typeof(A)))
            {
                eq = IL.EqualsTyped<A>(false);
            }
            else
            {
                var def = Class<Eq<A>>.Default;
                if (def == null)
                {
                    eq = EqualityComparer<A>.Default.Equals;
                }
                else
                {
                    eq = def.Equals;
                }
            }
        }

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(A a, A b)
        {
            if (isnull(a)) return isnull(b);
            if (isnull(b)) return false;
            if (ReferenceEquals(a, b)) return true;
            return eq(a, b);
        }

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(A x) =>
            default(HashableDefault<A>).GetHashCode(x);
        
        static Func<A, A, bool> GetEq(string name, Type eqType)
        {
            if (typeof(A).FullName.StartsWith($"LanguageExt.{name}`"))
            {
                var genA = typeof(A).GenericTypeArguments[0];
                var tryA = eqType.MakeGenericType(genA);
                var eq = tryA.GetMethod("Equals", new Type[] {typeof(A), typeof(A)});
            
                var lhs = Expression.Parameter(typeof(A), "lhs");
                var rhs = Expression.Parameter(typeof(A), "rhs");

                var lambda = Expression.Lambda<Func<A, A, bool>>(Expression.Call(Expression.Default(tryA), eq, lhs, rhs), lhs, rhs);
                return lambda.Compile();
            }
            else
            {
                return null;
            }
        }
    }
}
