namespace GetIntoTeachingApi.Services.Crm
{
    public class Lookup
    {
        public static Lookup Country = new Lookup("dfe_countries", "dfe_countryid");
        public static Lookup TeachingSubject = new Lookup("dfe_teachingsubjectlists", "dfe_teachingsubjectlistid");

        public string EntityName { get; set; }
        public string IdAttribute { get; set; }
        public string CacheKey => $"{EntityName}-{IdAttribute}";

        public Lookup(string entityName, string idAttribute)
        {
            EntityName = entityName;
            IdAttribute = idAttribute;
        }
    }
}
