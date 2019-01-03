#if NET20 || NET30 || NET35 || NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Dynamic.Utils
{
    // Miscellaneous helpers that don't belong anywhere else
    internal static class Helpers
    {
        internal static T CommonNode<T>(T first, T second, Func<T, T> parent) where T : class
        {
            var cmp = EqualityComparer<T>.Default;
            if (cmp.Equals(first, second))
            {
                return first;
            }
            var set = new HashSet<T>(cmp);
            for (var t = first; t != null; t = parent(t))
            {
                set.Add(t);
            }
            for (var t = second; t != null; t = parent(t))
            {
                if (set.Contains(t))
                {
                    return t;
                }
            }
            return null;
        }

        internal static void IncrementCount<T>(T key, Dictionary<T, int> dict)
        {
            dict.TryGetValue(key, out var count);
            dict[key] = count + 1;
        }
    }
}

#endif