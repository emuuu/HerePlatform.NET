using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;

namespace HerePlatformComponents.Maps.Extension;

internal static class ParameterViewExtensions
{
    internal static bool DidParameterChange<T>(this ParameterView parameters, T parameterValue, [CallerArgumentExpression("parameterValue")] string parameterName = "")
    {
        if (parameters.TryGetValue(parameterName, out T? value))
        {
            return !EqualityComparer<T>.Default.Equals(value, parameterValue);
        }

        return false;
    }
}
