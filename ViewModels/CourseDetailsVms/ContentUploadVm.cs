namespace LMS.ViewModels.CourseDetailsVms
{
    public class ContentUploadVm
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public int CourseId { get; set; }

        // URL returned by TUS resumable upload server
        public string UploadUrl { get; set; }

        // File name of the uploaded file
        public string Name { get; set; }
        public string Type { get; set; }

        // Description provided by the user
        public string Description { get; set; }

        // If used for stage mapping, leave nullable
        //public string StagenameMap { get; set; }

        // Extra content items (optional)
        public List<ModuleContentItemVm> Items { get; set; } = new();
    }
}
