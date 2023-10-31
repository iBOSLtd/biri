namespace PeopleDesk.Models
{
    public class FeatureCommontDDL
    {
        public long Value { get; set; }
        public string Label { get; set; }
        public string Name { get; set; }
    }
    public class GetCommonDDLViewModel
    {
        public long Value { get; set; }
        public string Label { get; set; }
    }
    public class FeatureCommontWithExtraDDL
    {
        public long Value { get; set; }
        public string Label { get; set; }
        public string Name { get; set; }
        public bool IsForWeb { get; set; }
        public bool IsForApps { get; set; }
    }
}
