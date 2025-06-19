using System.Linq.Expressions;

namespace WinformsBoilerplate.Core.Entities.Systems;

/// <summary>
/// Represents a partial update operation for a specific property of a source object.
/// </summary>
/// <typeparam name="TSource">The type of the source object being partially updated.</typeparam>
/// <typeparam name="TValue">The type of the value being set.</typeparam>
/// <param name="expr">The lambda expression used to select the property to update.</param>
/// <param name="value">The new value to set for the selected property.</param>
public class PartialLogic<TSource, TValue>(Expression<Func<TSource, TValue>> expr, TValue value)
{
    /// <summary>
    /// The lambda expression used to select the property to update.
    /// </summary>
    public Expression<Func<TSource, TValue>> Expr { get; set; } = expr;

    /// <summary>
    /// The new value to set for the selected property.
    /// </summary>
    public TValue Value { get; set; } = value;
}
