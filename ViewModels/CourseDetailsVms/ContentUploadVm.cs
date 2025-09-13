namespace LMS.ViewModels.CourseDetailsVms
{
    public class ContentUploadVm
    {
        public int ModuleId { get; set; }
        public int CourseId { get; set; }
        public List<IFormFile> Files { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string StagenameMap { get; set; }
        public List<ModuleContentItemVm> Items { get; set; } = new();
    }
}
