using GLMSMVC.Models.DTOs;
using GLMSMVC.Repositories.Interfaces;
using GLMSMVC.Services.Interfaces;
using GLMSMVC.Services.Results;

namespace GLMSMVC.Services
{
    public class ContractFileService : IContractFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment; // For wwwRoot
        private readonly IContractRepository _contractRepository;

        public ContractFileService(IWebHostEnvironment webHostEnvironment, IContractRepository contractRepository)
        {
            _webHostEnvironment = webHostEnvironment;
            _contractRepository = contractRepository;
        }

        // UPLOAD
        public async Task<ServiceResult<string>> UploadFileAsync(int contractId, IFormFile? file) //(DotNet Tech, 2023)
        {
            var contract = await _contractRepository.GetByIdAsync(contractId);
            if (contract == null)
            {
                return ServiceResult<string>.Failure("Contract not found.");
            }

            var validationResult = ValidatePdfFile(file);
            if (!validationResult.IsSuccess)
            {
                return ServiceResult<string>.Failure(validationResult.Errors);
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Delete old file for this contract if one already exists
            if (!string.IsNullOrWhiteSpace(contract.FilePath))
            {
                var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, 
                    contract.FilePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{contractId}.pdf";
            var fullPath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file!.CopyToAsync(stream);
            }

            var relativePath = Path.Combine("uploads", uniqueFileName).Replace("\\", "/");

            // Update Contract Path
            contract.FilePath = relativePath;
            _contractRepository.Update(contract);
            await _contractRepository.SaveChangesAsync();

            return ServiceResult<string>.Success(relativePath);
        }

        // DOWNLOAD
        public async Task<ServiceResult<FileDownloadResultDTO>> DownloadFileAsync(int contractId)//(DotNet Tech, 2023)
        {
            var contract = await _contractRepository.GetByIdAsync(contractId);
            if (contract == null)
            {
                return ServiceResult<FileDownloadResultDTO>.Failure("Contract not found.");
            }

            if (string.IsNullOrWhiteSpace(contract.FilePath))
            {
                return ServiceResult<FileDownloadResultDTO>.Failure("No file found for this contract.");
            }

            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath,
                contract.FilePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (!File.Exists(fullPath))
            {
                return ServiceResult<FileDownloadResultDTO>.Failure("File does not exist on the server.");
            }

            var fileBytes = await File.ReadAllBytesAsync(fullPath);

            var result = new FileDownloadResultDTO
            {
                FileBytes = fileBytes,
                ContentType = "application/pdf",
                FileName = Path.GetFileName(fullPath)
            };

            return ServiceResult<FileDownloadResultDTO>.Success(result);
        }

        // Return 1 or all errors from validation
        private ServiceResult<bool> ValidatePdfFile(IFormFile? file) // (Milton Coding, 2022)
        {
            var errors = new List<string>();

            if (file == null)
            {
                errors.Add("No file was uploaded.");
                return ServiceResult<bool>.Failure(errors);
            }

            if (file.Length == 0)
            {
                errors.Add("Uploaded file is empty.");
            }

            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            if (extension != ".pdf")
            {
                errors.Add("Only PDF files (.pdf) are allowed.");
            }

            var contentType = file.ContentType?.ToLowerInvariant();
            if (contentType != "application/pdf")
            {
                errors.Add("Invalid file type. Only PDF files are allowed.");
            }
            
            if (errors.Any())
            {
                return ServiceResult<bool>.Failure(errors);
            }

            return ServiceResult<bool>.Success(true);
        }
    }
}

/*
 DotNet Tech. 2023. ASP.NET Web API | File Upload and Download | .NET 7.0 [video online]. Avaliable at: <https://youtu.be/bN64VNISNw0?si=WAwiYL2RJSi-qktd> [Accessed 22 April 2026].
 Milton Coding. 2022. ASP.Net FileUpload File Extension Validation | File upload in asp.net c# [video online]. Avaliable at: <https://youtu.be/SaMxzqpGQ0Q?si=ywLWApTeOUjOKKsV> [Accessed 22 April 2026].

 */