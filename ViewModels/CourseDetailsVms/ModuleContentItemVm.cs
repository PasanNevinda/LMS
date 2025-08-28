namespace LMS.ViewModels.CourseDetailsVms
{
    public class ModuleContentItemVm
    {
        public int Id { get; set; }
        public int ContentItemId { get; set; }
        public string DisplayName { get; set; }
        public string Kind { get; set; }      // PdfItem / VideoItem / OtherItem
        public string FilePath { get; set; }  // for links
    }
}
