namespace GetIntoTeachingApi.Services
{
    public interface IPostcodeService
    {
        public bool IsValid(string postcode);
        public double DistanceBetween(string originPostcode, string destinationPostcode);
    }
}
