using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.Entities.CustomerEntities.JunctionTable;
using Kanini.LMP.Database.EntitiesDtos.DocumentDtos;
using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class DocumentService : IDocumentService
    {
        private readonly ILMPRepository<DocumentUpload, int> _documentRepository;
        private readonly ILMPRepository<ApplicationDocumentLink, int> _documentLinkRepository;

        public DocumentService(
            ILMPRepository<DocumentUpload, int> documentRepository,
            ILMPRepository<ApplicationDocumentLink, int> documentLinkRepository)
        {
            _documentRepository = documentRepository;
            _documentLinkRepository = documentLinkRepository;
        }

        public async Task<DocumentUploadDto> UploadDocumentAsync(DocumentUploadRequest request, int userId)
        {
            using var memoryStream = new MemoryStream();
            await request.File.CopyToAsync(memoryStream);

            var document = new DocumentUpload
            {
                LoanApplicationBaseId = request.LoanApplicationBaseId,
                UserId = userId,
                DocumentName = request.DocumentName,
                DocumentType = request.DocumentType,
                DocumentData = memoryStream.ToArray(),
                UploadedAt = DateTime.UtcNow
            };

            var createdDocument = await _documentRepository.AddAsync(document);

            var documentLink = new ApplicationDocumentLink
            {
                LoanApplicationBaseId = request.LoanApplicationBaseId,
                DocumentId = createdDocument.DocumentId,
                DocumentRequirementType = request.DocumentRequirementType,
                Status = DocumentStatus.Pending,
                LinkedAt = DateTime.UtcNow
            };

            await _documentLinkRepository.AddAsync(documentLink);

            return MapToDto(createdDocument, documentLink);
        }

        public async Task<byte[]> DownloadDocumentAsync(int documentId, int userId)
        {
            var document = await _documentRepository.GetByIdAsync(documentId);
            if (document == null || document.DocumentData == null)
                throw new FileNotFoundException("Document not found");

            return document.DocumentData;
        }

        public async Task<(byte[] fileData, string fileName)> DownloadDocumentWithNameAsync(int documentId, int userId)
        {
            var document = await _documentRepository.GetByIdAsync(documentId);
            if (document == null || document.DocumentData == null)
                throw new FileNotFoundException("Document not found");

            return (document.DocumentData, document.DocumentName);
        }

        public async Task<IReadOnlyList<DocumentUploadDto>> GetDocumentsByApplicationAsync(int loanApplicationBaseId)
        {
            var documentLinks = await _documentLinkRepository.GetAllAsync(dl => dl.LoanApplicationBaseId == loanApplicationBaseId);
            var result = new List<DocumentUploadDto>();

            foreach (var link in documentLinks)
            {
                var document = await _documentRepository.GetByIdAsync(link.DocumentId);
                if (document != null)
                {
                    result.Add(MapToDto(document, link));
                }
            }

            return result;
        }

        public async Task<DocumentUploadDto> VerifyDocumentAsync(DocumentVerificationRequest request, int managerId)
        {
            var documentLink = await _documentLinkRepository.GetAsync(dl => 
                dl.LoanApplicationBaseId == request.LoanApplicationBaseId && 
                dl.DocumentId == request.DocumentId);

            if (documentLink == null)
                throw new ArgumentException("Document link not found");

            documentLink.Status = request.Status;
            documentLink.VerificationNotes = request.VerificationNotes;
            documentLink.VerifiedAt = DateTime.UtcNow;
            documentLink.VerifiedBy = managerId;

            await _documentLinkRepository.UpdateAsync(documentLink);

            var document = await _documentRepository.GetByIdAsync(request.DocumentId);
            return MapToDto(document!, documentLink);
        }

        public async Task<bool> DeleteDocumentAsync(int documentId, int userId)
        {
            var document = await _documentRepository.GetByIdAsync(documentId);
            if (document == null || document.UserId != userId)
                return false;

            await _documentRepository.DeleteAsync(documentId);
            return true;
        }

        public async Task<IReadOnlyList<DocumentUploadDto>> GetPendingDocumentsAsync()
        {
            var pendingLinks = await _documentLinkRepository.GetAllAsync(dl => dl.Status == DocumentStatus.Pending);
            var result = new List<DocumentUploadDto>();

            foreach (var link in pendingLinks)
            {
                var document = await _documentRepository.GetByIdAsync(link.DocumentId);
                if (document != null)
                {
                    result.Add(MapToDto(document, link));
                }
            }

            return result;
        }

        private DocumentUploadDto MapToDto(DocumentUpload document, ApplicationDocumentLink link)
        {
            return new DocumentUploadDto
            {
                DocumentId = document.DocumentId,
                LoanApplicationBaseId = document.LoanApplicationBaseId,
                UserId = document.UserId,
                DocumentName = document.DocumentName,
                DocumentType = document.DocumentType,
                UploadedAt = document.UploadedAt,
                Status = link.Status,
                VerificationNotes = link.VerificationNotes,
                VerifiedAt = link.VerifiedAt,
                VerifiedBy = link.VerifiedBy
            };
        }
    }
}