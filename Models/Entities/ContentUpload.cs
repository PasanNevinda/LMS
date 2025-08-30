namespace LMS.Models.Entities
{
    public class ContentUpload
    {
        public int Id { get; set; }
        public string Title { get; set; }   
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TeacherID { get; set; }
        public string FilePath { get; set; }   // saved path in server
    }   
}
