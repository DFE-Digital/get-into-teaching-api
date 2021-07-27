using AutoMapper;

namespace GetIntoTeachingApi.Utils
{
    public static class AutoMapperUtil
    {
        public static void MapMatchingProperties<TSource, TDestination>(TSource source, TDestination destination)
        {
            var config = new MapperConfiguration(config => config.CreateMap<TSource, TDestination>());

            var mapper = config.CreateMapper();
            mapper.Map(source, destination);
        }
    }
}
