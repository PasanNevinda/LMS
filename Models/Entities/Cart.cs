namespace LMS.Models.Entities
{
    public class Cart
    {
        public int Id { get; set; }

        public string UserId { get; set; }  // FK to AspNetUsers
        public ApplicationUser User { get; set; }

        public List<CartItem> Items { get; set; } = new();
    }

}
