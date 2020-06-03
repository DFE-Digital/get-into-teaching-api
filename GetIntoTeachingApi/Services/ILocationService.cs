namespace GetIntoTeachingApi.Services
{
    public interface ILocationService
    {
        bool IsValid(string postcode);
        double DistanceBetween(string originPostcode, string destinationPostcode);
    }
}
