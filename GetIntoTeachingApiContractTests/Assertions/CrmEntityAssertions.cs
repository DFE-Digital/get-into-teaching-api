using System;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Extensions;
using FluentAssertions.Primitives;
using Microsoft.Xrm.Sdk;
using MoreLinq;

namespace GetIntoTeachingApiContractTests.Assertions
{
    public class CrmEntityAssertions : ObjectAssertions
    {
        public CrmEntityAssertions(Entity instance) : base(instance)
        {
            Subject = instance;
        }

        protected override string Identifier => "entity";

        public AndConstraint<CrmEntityAssertions> Match(
            Entity reference, string because = "", params object[] becauseArgs)
        {
            if (reference == null)
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .FailWith("You can't match an Entity if you don't pass an actual Entity to match to");
                
                return new AndConstraint<CrmEntityAssertions>(this);
            }
            
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .Given(() => Subject)
                .ForCondition(obj =>
                {
                    var entity = (Entity) obj;
                    
                    entity
                        .Should().BeEquivalentTo(reference, options => options
                                .Excluding(o => o.Attributes)
                                .Excluding(o => o.RelatedEntities)
                            .Using<DateTime>(ctx => 
                                ctx.Subject.Should().BeCloseTo(ctx.Expectation, 5.Seconds()))
                                .WhenTypeIs<DateTime>()
                        );

                    reference.Attributes.Keys
                        .ForEach(CheckAttributesForExpectedValues(entity, reference));
                    
                    if (entity.LogicalName == "contact")
                    {
                        reference.RelatedEntities.Keys
                            .ForEach(CheckReferencesForExpectedValues(entity, reference));
                    }

                    return true;
                })
                .FailWith("Expected {context:entity} to match {0}{reason}.", 
                    _ => reference.LogicalName);

            return new AndConstraint<CrmEntityAssertions>(this);
        }

        private static Action<string> CheckAttributesForExpectedValues(Entity entity, Entity reference)
        {
            return key =>
            {
                var actualValue = entity.Attributes[key];
                var expectedValue = reference.Attributes[key];

                IsEquivalentValue(actualValue, expectedValue);
            };
        }

        private static Action<Relationship> CheckReferencesForExpectedValues(Entity entity, Entity reference)
        {
            return relationship =>
            {
                var actualValue = entity.RelatedEntities[relationship];
                var expectedValue = reference.RelatedEntities[relationship];

                IsEquivalentValue(actualValue.Entities, expectedValue.Entities);
            };
        }

        private static void IsEquivalentValue(object actualValue, object expectedValue)
        {
            actualValue
                .Should().BeEquivalentTo(expectedValue, options => options
                    .Using<DateTime>(ctx =>
                    {
                        var expectation = ctx.Expectation;
                        if (DateTime.MinValue == expectation)
                        {
                            expectation = DateTime.UtcNow;
                        }
                        ctx.Subject.Should().BeCloseTo(expectation, 5.Seconds());
                    })
                    .WhenTypeIs<DateTime>()
                );
        }
    }
}