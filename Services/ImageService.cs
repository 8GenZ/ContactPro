using ContactPro.Services.Interfaces;


namespace ContactPro.Services
{
    public class ImageService : IImageService
    {
        private readonly string? _defaultImage = "/img/DefaultContactImage.png";
        public string? ConvertByteArrayToFile(byte[]? fileData, string? extension)
        {

            try
            {
                if(fileData == null)
                {
                    return _defaultImage;
                }
                string? imageBase64Data = Convert.ToBase64String(fileData);
                imageBase64Data = string.Format($"data:{extension};base64,{imageBase64Data}");

                return imageBase64Data;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile? file)
        {
            try
            {
                using MemoryStream memoryStream = new MemoryStream();
                await file!.CopyToAsync(memoryStream);
                byte[] byteFile = memoryStream.ToArray();
                memoryStream.Close();
                return byteFile;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
