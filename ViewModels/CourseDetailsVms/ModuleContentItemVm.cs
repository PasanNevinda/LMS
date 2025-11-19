namespace LMS.ViewModels.CourseDetailsVms
{
    public class ModuleContentItemVm
    {
        public int ModuleId { get; set; }
        public int ContentItemId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        public string FilePath { get; set; } = string.Empty;// for links
    }
}
