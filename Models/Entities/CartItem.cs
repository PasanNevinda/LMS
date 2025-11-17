namespace LMS.Models.Entities
{
    public class CartItem
    {
        public int Id { get; set; }

        public int CartId { get; set; }
        public Cart Cart { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

    }

}
