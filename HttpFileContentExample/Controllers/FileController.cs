using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HttpFileContentExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private const string ContentType = "application/octet-stream";

        [HttpHead("file-content-result")]
        [HttpGet("file-content-result")]
        public async Task<IActionResult> GetFileContentResult([BindRequired, FromQuery] string path)
        {
            var data = await System.IO.File.ReadAllBytesAsync(path);
            return new FileContentResult(data, ContentType) {EnableRangeProcessing = true };
        }

        [HttpHead("file-stream-result")]
        [HttpGet("file-stream-result")]
        public IActionResult GetFileStreamResult([BindRequired, FromQuery] string path)
        {
            var data = System.IO.File.OpenRead(path);
            return new FileStreamResult(data, ContentType) {EnableRangeProcessing = true };
        }

        [HttpHead("physical-file")]
        [HttpGet("physical-file")]
        public IActionResult GetPhysicalFile([BindRequired, FromQuery] string path)
        {
            return PhysicalFile(path, ContentType, true);
        }
    }
}
