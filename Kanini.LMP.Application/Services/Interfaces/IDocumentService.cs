using Kanini.LMP.Database.EntitiesDtos.DocumentDtos;
using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IDocumentService
    {
        Task<DocumentUploadDto> UploadDocumentAsync(DocumentUploadRequest request, int userId);
        Task<byte[]> DownloadDocumentAsync(int documentId, int userId);
        Task<(byte[] fileData, string fileName)> DownloadDocumentWithNameAsync(int documentId, int userId);
        Task<IReadOnlyList<DocumentUploadDto>> GetDocumentsByApplicationAsync(int loanApplicationBaseId);
        Task<DocumentUploadDto> VerifyDocumentAsync(DocumentVerificationRequest request, int managerId);
        Task<bool> DeleteDocumentAsync(int documentId, int userId);
        Task<IReadOnlyList<DocumentUploadDto>> GetPendingDocumentsAsync();
    }
}