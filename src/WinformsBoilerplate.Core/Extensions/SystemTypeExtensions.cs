using System.Linq.Expressions;
using System.Reflection;
using static WinformsBoilerplate.Core.Constants.Common;

namespace WinformsBoilerplate.Core.Extensions;

/// <summary>
/// Provides extension methods for working with System.Type instances to enhance reflection capabilities.
/// </summary>
public static class SystemTypeExtensions
{
    /// <summary>
    /// Searches for a property in the specified type by its name.
    /// </summary>
    /// <param name="targetType">The type to search for the property.</param>
    /// <param name="actionName">The name of the property to find.</param>
    /// <param name="ignoreCase">If true, performs a case-insensitive search. Default is false.</param>
    /// <returns>
    /// A <see cref="PropertyInfo"/> instance representing the found property, or null if no property
    /// with the specified name exists.
    /// </returns>
    /// <remarks>
    /// This method searches for properties using the binding flags specified in <see cref="Attributes.BINDING_FLAGS"/>.
    /// The search can be either case-sensitive or case-insensitive based on the <paramref name="ignoreCase"/> parameter.
    /// </remarks>
    public static PropertyInfo? FindPropertyInfoByName(this Type targetType, string actionName, bool ignoreCase = false)
    {
        PropertyInfo[] properties = targetType.GetProperties(Attributes.BINDING_FLAGS);

        return ignoreCase
            ? Array.Find(properties, property => string.Equals(property.Name, actionName, StringComparison.CurrentCultureIgnoreCase))
            : Array.Find(properties, property => string.Equals(property.Name, actionName));
    }

    /// <summary>
    /// Extracts a <see cref="PropertyInfo"/> from a lambda expression that targets a property of a specified type.
    /// </summary>
    /// <typeparam name="TSource">The type of the source object.</typeparam>
    /// <param name="expr">The lambda expression to extract the property information from.</param>
    /// <returns>A <see cref="PropertyInfo"/> instance representing the extracted property, or null if the extraction fails.</returns>
    public static PropertyInfo? ExtractPropertyInfo<TSource>(this Expression<Func<TSource, object>> expr)
    {
        return expr.Body switch {
            MemberExpression member => member.Member as PropertyInfo,
            UnaryExpression unary when unary.Operand is MemberExpression member =>
                member.Member as PropertyInfo,
            _ => null
        };
    }
}
