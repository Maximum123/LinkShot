using System;
using System.Configuration;
using System.IO;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace CapturesQueue.Services
{
    public class CaptureService : ICaptureService
    {
        private readonly Cloudinary _cloudinary;

        public CaptureService()
        {
            var name = ConfigurationManager.AppSettings["Clouddinary.Name"];
            var key = ConfigurationManager.AppSettings["Clouddinary.ApiKey"];
            var secret = ConfigurationManager.AppSettings["Clouddinary.ApiSecret"];
            Account account = new Account(
                name,
                key,
                secret);

             _cloudinary = new Cloudinary(account);
        }

        public string UploadScreenShot(byte[] screenshotBytes)
        {
            if (screenshotBytes == null)
                return String.Empty;
            using (Stream ms = new MemoryStream(screenshotBytes))
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(Guid.NewGuid().ToString(), ms)
                };
                var uploadResult = _cloudinary.Upload(uploadParams);

                return uploadResult.Uri.ToString();
            }
        }
    }
}
