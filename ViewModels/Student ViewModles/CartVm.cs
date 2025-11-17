namespace LMS.ViewModels.Student_ViewModles
{
    public class CartVm
    {
        public int CartId { get; set; }
        public decimal TotalAmount {  get; set; }
        public List<CartItemVm> Items { get; set; } = new List<CartItemVm>();
    }
}
