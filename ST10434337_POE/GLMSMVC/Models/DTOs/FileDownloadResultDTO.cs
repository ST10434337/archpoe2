namespace GLMSMVC.Models.DTOs
{
    public class FileDownloadResultDTO // DTO
    {
        public byte[] FileBytes { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = "application/pdf";
        public string FileName { get; set; } = string.Empty;
    }
}
