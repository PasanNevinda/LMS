namespace LMS.ViewModels.Admin_ViewModels
{
    public class RecentCourcesVm
    {
        public string Name { get; set; } = string.Empty;
        public string ImgUrl { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime Submitted { get; set; }
        public string Instructor { get; set; } = string.Empty;
        public int Price { get; set; }
        public string Email {  get; set; } = string.Empty;
    }
}
