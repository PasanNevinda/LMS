using LMS.Models.Entities;

namespace LMS.ViewModels.CourseDetailsVms
{
    public class ModuleVm
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ModuleContentItemVm> Items { get; set; } = new();

    }
}
