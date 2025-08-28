using LMS.Models.Entities;

namespace LMS.ViewModels.CourseDetailsVms
{
    public class CourseDetailsVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Language Language { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public bool IsOwner { get; set; }

        public List<ModuleVm> Modules { get; set; } = new();
    }
}
