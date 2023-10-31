using PeopleDesk.Services.Helper.Interfaces;

namespace PeopleDesk.Services.Helper
{
    public class FileSaveService : IFileSaveService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public FileSaveService(IWebHostEnvironment _hostingEnvironment)
        {
            this._hostingEnvironment = _hostingEnvironment;
        }
        public string SaveImage(out string fileName, IFormFile img)
        {
            string message = "success";

            var extention = Path.GetExtension(img.FileName);
            fileName = Path.Combine("UploadImages", DateTime.Now.Ticks + extention);

            var path = Path.Combine(_hostingEnvironment.WebRootPath, fileName);
            try
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    img.CopyTo(stream);
                }
            }
            catch
            {
                message = "can not upload image";
            }

            return message;
        }


    }
}
