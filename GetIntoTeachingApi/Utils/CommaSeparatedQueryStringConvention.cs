using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Attributes;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace GetIntoTeachingApi.Utils
{
    public class CommaSeparatedQueryStringConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            foreach (var parameter in action.Parameters)
            {
                if (parameter.Attributes.OfType<CommaSeparatedAttribute>().Any() && !parameter.Action.Filters.OfType<CommaSeparatedQueryStringAttribute>().Any())
                {
                    var attributeNames = parameter.Attributes.OfType<CommaSeparatedAttribute>().First().AttributeNames;

                    if (attributeNames != null)
                    {
                        AddToActionFilterAsCommaSeparated(parameter, attributeNames);
                    }
                    else
                    {
                        AddToActionFilterAsCommaSeparated(parameter, new string[] { parameter.ParameterName });
                    }
                }
            }
        }

        private static void AddToActionFilterAsCommaSeparated(ParameterModel parameter, string[] keys)
        {
            parameter.Action.Filters.Add(new CommaSeparatedQueryStringAttribute(keys, ","));
        }
    }
}
