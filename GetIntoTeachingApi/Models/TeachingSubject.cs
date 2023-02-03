using System;
using Microsoft.Xrm.Sdk;
using System.ComponentModel.DataAnnotations.Schema;

namespace GetIntoTeachingApi.Models
{
    public class TeachingSubject
    {
        public static readonly Guid PrimaryTeachingSubjectId = new("b02655a1-2afa-e811-a981-000d3a276620");

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid? Id { get; set; }
        public string Value { get; set; }

        public TeachingSubject()
        {
        }

        public TeachingSubject(Entity entity)
        {
            Id = entity.Id;
            Value = entity.GetAttributeValue<string>("dfe_name");
        }
    }
}
