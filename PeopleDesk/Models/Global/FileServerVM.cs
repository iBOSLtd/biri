namespace PeopleDesk.Models.Global
{
    public class FileServerVM
    {
    }

    public class FileValidationError
    {
        public string? FileName { get; set; }
        public string? Message { get; set; }
    }

    public class FileResponse
    {
        public long globalFileUrlId { get; set; }
        public string? fileName { get; set; }
    }

    public class BaseSixtyFourDTO
    {
        public string Data { get; set; }
        public string FileName { get; set; }
    }
}