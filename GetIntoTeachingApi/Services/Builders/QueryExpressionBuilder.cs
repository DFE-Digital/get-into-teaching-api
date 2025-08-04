using Microsoft.Xrm.Sdk.Query;

namespace GetIntoTeachingApi.Services.Builders;

/// <summary>
/// Provides a fluent builder for constructing <see cref="QueryExpression"/> objects.
/// </summary>
public class QueryExpressionBuilder
{
    /// <summary>
    /// Gets or sets the internal <see cref="QueryExpression"/> being built.
    /// </summary>
    private QueryExpression Query { get; set; }

    /// <summary>
    /// Initializes a new <see cref="QueryExpression"/> for the specified entity logical name.
    /// </summary>
    /// <param name="name">The logical name of the entity to query.</param>
    /// <returns>The current <see cref="QueryExpressionBuilder"/> instance for chaining.</returns>
    public QueryExpressionBuilder Create(string name)
    {
        Query = new QueryExpression(name);
        return this;
    }

    /// <summary>
    /// Adds the specified column names to the column set of the query.
    /// </summary>
    /// <param name="columns">An array of column names to retrieve.</param>
    /// <returns>The current <see cref="QueryExpressionBuilder"/> instance for chaining.</returns>
    public QueryExpressionBuilder WithColumns(string[] columns)
    {
        Query.ColumnSet.AddColumns(columns);
        return this;
    }
            
    /// <summary>
    /// Adds a filtering condition to the query criteria.
    /// </summary>
    /// <param name="condition">The <see cref="ConditionExpression"/> to add to the query.</param>
    /// <returns>The current <see cref="QueryExpressionBuilder"/> instance for chaining.</returns>
    public QueryExpressionBuilder WithCondition(ConditionExpression condition)
    {
        Query.Criteria.AddCondition(condition);
        return this;
    }
            
    /// <summary>
    /// Returns the constructed <see cref="QueryExpression"/> object.
    /// </summary>
    /// <returns>The final <see cref="QueryExpression"/>.</returns>
    public QueryExpression Build() => Query;
}