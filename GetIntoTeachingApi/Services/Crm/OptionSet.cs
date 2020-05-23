namespace GetIntoTeachingApi.Services.Crm
{
    public class OptionSet
    {
        private const string CandidateMetadataId = "608861bc-50a4-4c5f-a02c-21fe1943e2cf";
        private const string QualificationMetadataId = "9caa6cb4-a7ed-e811-a972-000d3a20838a";
        private const string PastTeachingPositionMetadataId = "74d7265c-42ee-e811-a96e-000d3a206976";
        private const string TeachingEventMetadataId = "400a4c09-a61b-437d-a024-23ca51942804";

        public static OptionSet CandidateInitialTeacherTrainingYears = new OptionSet(CandidateMetadataId, "b93c6ecf-2cbb-e811-a966-000d3a233b72");
        public static OptionSet CandidatePreferredEducationPhases = new OptionSet(CandidateMetadataId, "c21480dc-09bc-e811-a97a-000d3a2065c5");
        public static OptionSet CandidateLocations = new OptionSet(CandidateMetadataId, "4e7556f1-f6c2-e811-a96b-000d3a233b72");
        public static OptionSet QualificationDegreeStatus = new OptionSet(QualificationMetadataId, "bffd758c-0eef-e811-a97a-000d3a233e06");
        public static OptionSet QualificationCategories = new OptionSet(QualificationMetadataId, "75e0dcf0-a7ed-e811-a972-000d3a20838a");
        public static OptionSet QualificationTypes = new OptionSet(QualificationMetadataId, "130f68e3-a9ed-e811-a972-000d3a20838a");
        public static OptionSet PastTeachingPositionEducationPhases = new OptionSet(PastTeachingPositionMetadataId, "4066ee3f-43ee-e811-a96e-000d3a206976");
        public static OptionSet TeachingEventTypes = new OptionSet(TeachingEventMetadataId, "9587094b-5846-e911-a9b0-000d3a2065c5");

        public string EntityMetadataId { get; set; }
        public string AttributeMetadataId { get; set; }
        public string CacheKey => $"{EntityMetadataId}-{AttributeMetadataId}";

        public OptionSet(string entityMetadataId, string attributeMetadataId)
        {
            EntityMetadataId = entityMetadataId;
            AttributeMetadataId = attributeMetadataId;
        }
    }
}