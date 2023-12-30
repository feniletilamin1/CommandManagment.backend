
namespace CommandManagment.backend.Helpers
{
    public static class ImageService
    {
        public static async void LoadPhoto (string path, string fileName, IFormFile photo)
        {
            if(!Directory.Exists(path)) 
                Directory.CreateDirectory(path);

            using FileStream fileStream = new(path + fileName, FileMode.Create);
            await photo.CopyToAsync(fileStream);
        }
    }
}
