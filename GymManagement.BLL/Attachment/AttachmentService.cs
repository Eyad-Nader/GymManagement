using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Attachment
{
    public class AttachmentService : IAttachmentService
    {
        private readonly ILogger<AttachmentService> _logger;
        private readonly long MaxLengthFile = 5 * 1024 * 1024;
        private readonly IWebHostEnvironment _env;
        private readonly string[] _allowedExtensions = [".jpg",".jpeg",".png"];
        public AttachmentService(ILogger<AttachmentService> logger , IWebHostEnvironment env) 
        {
            _logger = logger;
            _env = env;
        }

        public async Task<string?> UploadAsync(Stream fs, string fileName, string folderName, CancellationToken ct = default)
        {
            if (fs == null || !fs.CanRead) return null;
            if (fs.Length < 0) return null;

            if (fs.Length > MaxLengthFile) 
            {
                _logger.LogError($"File is too Largr {fs.Length} Bytes");
                return null;
            }

            var extension =  Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension)||!_allowedExtensions.Contains(extension))
            {
                _logger.LogError($"{extension} Is Not Allowed Extention");
                return null;
            }

            var folderPath = Path.Combine(_env.ContentRootPath, folderName);
            Directory.CreateDirectory(folderPath);  
            
            var uniqueName = $"{Guid.NewGuid()}{fileName}";
            var fullPath = Path.Combine(folderPath, uniqueName);
            try
            {
                using var outputStream = File.Create(fullPath);
                await fs.CopyToAsync(outputStream, ct);
                return uniqueName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,$"Failed To Upload File{fileName} error:{ex.Message}");
                return null;
            } 
        }

        public bool Delete(string FolderName, string fileName)
        {
            var fullPath = Path.Combine(_env.ContentRootPath, FolderName, fileName);
            try{

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            return false;
            }catch(Exception ex)
            {
                _logger.LogError(ex,$"Failed To Delete File{fileName} error:{ex.Message}");
                return false;
            }
        }

        public (Stream, string)? GetFile(string FolderName, string fileName)
        {
            if(string.IsNullOrWhiteSpace(fileName)|| string.IsNullOrWhiteSpace(FolderName)) return null;
            var fullPath = Path.Combine(_env.ContentRootPath, FolderName, fileName);
            
            if (!File.Exists(fullPath)) return null;
            var stream = new FileStream(fullPath,FileMode.Open,FileAccess.Read);
            var extention = Path.GetExtension(fullPath).ToLower();
            var ContetnType = extention switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
            return (stream, ContetnType);

        }
        
    }
}
