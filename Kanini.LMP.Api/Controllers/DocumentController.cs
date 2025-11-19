using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDtos.DocumentDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kanini.LMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost("upload")]
        public async Task<ActionResult<DocumentUploadDto>> UploadDocument([FromForm] DocumentUploadRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var result = await _documentService.UploadDocumentAsync(request, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("download/{documentId}")]
        public async Task<IActionResult> DownloadDocument(int documentId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var (fileData, fileName) = await _documentService.DownloadDocumentWithNameAsync(documentId, userId);
                return File(fileData, "application/octet-stream", fileName);
            }
            catch (FileNotFoundException)
            {
                return NotFound("Document not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("view/{documentId}")]
        public async Task<IActionResult> ViewDocument(int documentId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var (fileData, fileName) = await _documentService.DownloadDocumentWithNameAsync(documentId, userId);
                
                var contentType = GetContentType(fileName);
                Response.Headers.Add("Content-Disposition", "inline");
                
                return File(fileData, contentType);
            }
            catch (FileNotFoundException)
            {
                return NotFound("Document not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".txt" => "text/plain",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }

        [HttpGet("application/{loanApplicationBaseId}")]
        public async Task<ActionResult<IReadOnlyList<DocumentUploadDto>>> GetDocumentsByApplication(int loanApplicationBaseId)
        {
            var documents = await _documentService.GetDocumentsByApplicationAsync(loanApplicationBaseId);
            return Ok(documents);
        }

        [HttpPost("verify")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<DocumentUploadDto>> VerifyDocument([FromBody] DocumentVerificationRequest request)
        {
            try
            {
                var managerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var result = await _documentService.VerifyDocumentAsync(request, managerId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{documentId}")]
        public async Task<IActionResult> DeleteDocument(int documentId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var success = await _documentService.DeleteDocumentAsync(documentId, userId);
            
            if (!success)
                return NotFound("Document not found or unauthorized");
                
            return Ok(new { message = "Document deleted successfully" });
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<IReadOnlyList<DocumentUploadDto>>> GetPendingDocuments()
        {
            var documents = await _documentService.GetPendingDocumentsAsync();
            return Ok(documents);
        }
    }
}