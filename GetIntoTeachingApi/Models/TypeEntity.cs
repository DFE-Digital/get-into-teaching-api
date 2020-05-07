using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Microsoft.Xrm.Sdk;
using System;

namespace GetIntoTeachingApi.Models
{
    public class TypeEntity
    {
        public Guid Id { get; set; }
        public dynamic Value { get; set; }
    }
}
