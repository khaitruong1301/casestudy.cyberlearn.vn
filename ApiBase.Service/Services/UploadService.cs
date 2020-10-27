//using Microsoft.AspNetCore.Http;
//using ApiBase.Service.Constants;
//using ApiBase.Service.ViewModels;
//using System;
//using System.IO;
//using System.Net.Http.Headers;
//using System.Threading.Tasks;

//namespace SoloDevApp.Service.Services
//{
//    public interface IUploadService
//    {
//        Task<ResponseEntity> UploadSingleImageAsync(IFormFile file);
//    }

//    public class UploadService : IUploadService
//    {
//        public async Task<ResponseEntity> UploadSingleImageAsync(IFormFile file)
//        {
//            try
//            {
//                if (file == null || file.Length == 0)
//                    return new ResponseEntity(StatusCodeConstants.BAD_REQUEST);

//                string filePath = await SaveFileAsync(file);

//                return new ResponseEntity(StatusCodeConstants.OK, filePath, MessageConstants.INSERT_SUCCESS);
//            }
//            catch
//            {
//                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, null, MessageConstants.SIGNIN_ERROR);
//            }
//        }
    
//        private async Task<string> SaveFileAsync(IFormFile file)
//        {
//            string folderSave = $"/images/{DateTime.Now.ToString("ddMMyy")}";
//            string folderPath = Path.Combine(
//                Directory.GetCurrentDirectory(), "wwwroot", folderSave);

//            // Tạo folder nếu chưa tồn tại
//            if (!Directory.Exists(folderPath))
//                Directory.CreateDirectory(folderPath);

//            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

//            string path = Path.Combine(folderPath, fileName);

//            using (var stream = new FileStream(path, FileMode.Create))
//            {
//                await file.CopyToAsync(stream);
//            }

//            return $"{folderSave}/{fileName}";
//        }
//    }
//}