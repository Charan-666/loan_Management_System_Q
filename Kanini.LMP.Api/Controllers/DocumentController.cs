using Kanini.LMP.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        private readonly ILoanApplicationService _loanApplicationService;

        public DocumentController(ILoanApplicationService loanApplicationService)
        {
            _loanApplicationService = loanApplicationService;
        }

        [HttpPost("upload/{loanApplicationId}/{userId}")]
        public async Task<ActionResult> UploadDocument(int loanApplicationId, int userId, IFormFile file, [FromForm] string documentType)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            if (string.IsNullOrEmpty(documentType))
                return BadRequest("Document type is required");

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var documentData = memoryStream.ToArray();

            var documentId = await _loanApplicationService.UploadDocumentAsync(
                loanApplicationId, 
                userId, 
                file.FileName, 
                documentType, 
                documentData);

            return Ok(new { DocumentId = documentId, Message = "Document uploaded successfully" });
        }

        [HttpPost("upload-multiple/{loanApplicationId}/{userId}")]
        public async Task<ActionResult> UploadMultipleDocuments(int loanApplicationId, int userId, List<IFormFile> files, [FromForm] List<string> documentTypes)
        {
            if (files == null || !files.Any())
                return BadRequest("No files uploaded");

            if (documentTypes == null || files.Count != documentTypes.Count)
                return BadRequest("Document types must match number of files");

            var uploadedDocuments = new List<object>();

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var documentType = documentTypes[i];

                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var documentData = memoryStream.ToArray();

                var documentId = await _loanApplicationService.UploadDocumentAsync(
                    loanApplicationId, 
                    userId, 
                    file.FileName, 
                    documentType, 
                    documentData);

                uploadedDocuments.Add(new { DocumentId = documentId, FileName = file.FileName, DocumentType = documentType });
            }

            return Ok(new { UploadedDocuments = uploadedDocuments, Message = $"{files.Count} documents uploaded successfully" });
        }
    }
}