namespace LMS.ViewModels.Admin_ViewModels.CourseReview
{
    public class ModuleVm
    {
        public string Name {  get; set; } = string.Empty;

        public List<ModuleItem> Items  = new List<ModuleItem>();
    }

    public class ModuleItem
    {
        public string ItemName { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty ;
        public string Url { get; set; } = string.Empty;
    }

}
