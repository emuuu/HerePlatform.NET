using System;
using System.Collections.Generic;
using System.Linq;

namespace HerePlatform.Blazor;

public static class ExceptionExtensions
{
    public static bool HasInnerExceptionsOfType<T>(this Exception ex) where T : Exception => ex.GetInnerExceptionsOfType<T>().Any();

    public static IEnumerable<T> GetInnerExceptionsOfType<T>(this Exception ex) where T : Exception
    {
        var candidates = new[] { ex, ex.InnerException };

        if (ex is AggregateException aggEx)
        {
            var innerExceptions = aggEx.InnerExceptions.ToArray();
            candidates = candidates.Concat(innerExceptions).Where(x => x is not null).ToArray();
        }

        var exceptions = candidates.Select(x => x as T).Where(x => x is not null).Cast<T>().Distinct();
        return exceptions;
    }

    /// <summary>
    /// Returns the leaf exceptions in the tree: every non-wrapper exception reachable
    /// via <see cref="Exception.InnerException"/> and <see cref="AggregateException.InnerExceptions"/>.
    /// An exception with no inner exceptions is itself a leaf.
    /// </summary>
    public static IEnumerable<Exception> GetLeafExceptions(this Exception ex)
    {
        var visited = new HashSet<Exception>(ReferenceEqualityComparer.Instance);
        var stack = new Stack<Exception>();
        stack.Push(ex);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (!visited.Add(current)) continue;

            var children = new List<Exception>();
            if (current is AggregateException aggregate)
            {
                foreach (var inner in aggregate.InnerExceptions)
                {
                    if (inner is not null) children.Add(inner);
                }
            }
            else if (current.InnerException is not null)
            {
                children.Add(current.InnerException);
            }

            if (children.Count == 0)
            {
                yield return current;
            }
            else
            {
                foreach (var child in children) stack.Push(child);
            }
        }
    }

    /// <summary>
    /// Returns true when EVERY leaf exception in the tree is assignable to one of the
    /// given types. Use this to decide whether an exception is safe to suppress during
    /// a Blazor circuit teardown: a mixed tree (e.g. AggregateException containing a
    /// real JSException plus a benign JSDisconnectedException) must NOT be swallowed.
    /// </summary>
    public static bool AllLeavesAreOfType(this Exception ex, params Type[] allowedTypes)
    {
        if (allowedTypes is null || allowedTypes.Length == 0) return false;

        bool sawAny = false;
        foreach (var leaf in ex.GetLeafExceptions())
        {
            sawAny = true;
            var leafType = leaf.GetType();
            bool matched = false;
            foreach (var allowed in allowedTypes)
            {
                if (allowed.IsAssignableFrom(leafType))
                {
                    matched = true;
                    break;
                }
            }
            if (!matched) return false;
        }

        // Defensive default: a pathological cyclic exception graph (constructed
        // via reflection or a custom Exception subclass) can yield zero leaves
        // even though the input is a real exception. Vacuous true would silently
        // suppress it; return false so the exception still propagates.
        return sawAny;
    }
}
