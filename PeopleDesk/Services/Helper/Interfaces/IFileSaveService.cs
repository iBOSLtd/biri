namespace PeopleDesk.Services.Helper.Interfaces
{
    public interface IFileSaveService
    {
        string SaveImage(out string fileName, IFormFile img);
    }
}
