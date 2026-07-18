using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Attachment
{
    public interface IAttachmentService
    {
        Task<string?> UploadAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default);
        bool Delete(string folderName, string fileName);
        (Stream stream,string ContentType)? GetFile(string folderName, string fileName);
    }
}
