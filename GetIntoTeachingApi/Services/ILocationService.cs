namespace GetIntoTeachingApi.Services
{
    public interface ILocationService
    {
        public bool IsValid(string postcode);
        public double DistanceBetween(string originPostcode, string destinationPostcode);
    }
}
