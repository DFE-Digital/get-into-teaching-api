using FluentAssertions;
using GetIntoTeachingApi.Services.Builders;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using Xunit;


namespace GetIntoTeachingApiTests.Services.Builders;

public class QueryExpressionBuilderTests
{
    [Fact]
    public void Build_WithColumns_ReturnsConfiguredQueryExpression()
    {
        // arrange / act
        QueryExpression query = 
            new QueryExpressionBuilder()
                .Create("contact")
                .WithColumns(["col1", "col2"])
                .Build();
        
        
        
        // assert
        query.EntityName.Should().Be("contact");
        query.ColumnSet.Columns.Should().HaveCount(2).And.Contain(["col1", "col2"]);
    }
    
    [Fact]
    public void Build_WithCondition_ReturnsConfiguredQueryExpression()
    {
        // arrange
        Guid id = Guid.NewGuid();
        ConditionExpression condition = new("contactid", ConditionOperator.Equal, id);
        
        // act
        QueryExpression query = 
            new QueryExpressionBuilder()
                .Create("contact")
                .WithCondition(condition)
                .Build();
        
        // assert
        query.EntityName.Should().Be("contact");
        query.Criteria.Conditions.Should().HaveCount(1);
        query.Criteria.Conditions.First().AttributeName.Should().Be("contactid");
        query.Criteria.Conditions.First().Operator.Should().Be(ConditionOperator.Equal);
        query.Criteria.Conditions.First().Values.First().Should().Be(id);
    }
}