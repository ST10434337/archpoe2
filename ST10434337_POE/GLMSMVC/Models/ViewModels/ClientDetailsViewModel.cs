namespace GLMSMVC.Models.ViewModels
{
    public class ClientDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ContactDetails { get; set; }
        public string Region { get; set; } = string.Empty;
    }
}
