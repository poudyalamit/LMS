using LMS.Data.Migrations;
using LMS.Models.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using System.IO;

namespace LMS.Utils
{
    public class Upload ()
    {
        public async Task<string> UploadFile(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".docx", ".pptx" };
            if (!allowedExtensions.Contains(extension.ToLower()))
                return BadRequest( "File type not allowed.");

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest("File size exceeds 5MB.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var savedFilePath = Path.Combine("uploads", fileName);
            return savedFilePath;

        }

        public async Task<string> EditFile(IFormFile file, string? oldFilePath = null)
        {
            var extension = Path.GetExtension(file.FileName);
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".docx", ".pptx" };
            if (!allowedExtensions.Contains(extension.ToLower()))
                throw new Exception("File type not allowed.");

            if (file.Length > 5 * 1024 * 1024)
                throw new Exception("File size exceeds 5MB.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            if (!string.IsNullOrEmpty(oldFilePath))
            {
                var oldFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldFilePath);
                if (System.IO.File.Exists(oldFullPath))
                {
                    System.IO.File.Delete(oldFullPath);
                }
            }

            return Path.Combine("uploads", fileName);
        }

        public void DeleteFile(string filepath)
        {
            if (!string.IsNullOrEmpty(filepath))
            {
                var oldFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filepath);
                if (System.IO.File.Exists(oldFullPath))
                {
                    System.IO.File.Delete(oldFullPath);
                }
            }
        }

        private string BadRequest(string v)
        {
            throw new NotImplementedException();
        }
    }
}
