using System;
using System.Collections.Generic;

namespace GetIntoTeachingApi.Models
{
    public class MappingInfo
    {
        private readonly Type _type;
        public string Class { get; set; }
        public string LogicalName { get; set; }
        public IDictionary<string, IDictionary<string, string>> Fields { get; } =
            new Dictionary<string, IDictionary<string, string>>();
        public IDictionary<string, IDictionary<string, string>> Relationships { get; } =
            new Dictionary<string, IDictionary<string, string>>();

        public MappingInfo()
        {
        }

        public MappingInfo(Type type)
        {
            _type = type;

            MapClass();
            MapProperties();
        }

        private void MapClass()
        {
            Class = _type.ToString();
            LogicalName = BaseModel.LogicalName(_type);
        }

        private void MapProperties()
        {
            foreach (var property in _type.GetProperties())
            {
                var field = BaseModel.EntityFieldAttribute(property);
                var relationship = BaseModel.EntityRelationshipAttribute(property);

                if (field != null)
                {
                    Fields.Add(property.Name, BaseModel.EntityFieldAttribute(property).ToDictionary());
                }
                else if (relationship != null)
                {
                    Relationships.Add(property.Name, BaseModel.EntityRelationshipAttribute(property).ToDictionary());
                }
            }
        }
    }
}
