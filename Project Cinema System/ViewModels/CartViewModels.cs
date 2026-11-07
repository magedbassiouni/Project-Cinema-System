namespace Project_Cinema_System.ViewModels
{
    public class CartItemViewModel
    {
        public int ItemId { get; set; }
        public int ShowtimeId { get; set; }
        public string MovieName { get; set; } = string.Empty;
        public string CinemaName { get; set; } = string.Empty;
        public DateTime ShowtimeDate { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }

    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new();
        public decimal Total => Items.Sum(i => i.LineTotal);
    }

    public class CheckoutViewModel : CartViewModel
    {
        public string? PromotionCode { get; set; }

        // Cash or Online
        public string PaymentMethod { get; set; } = "Online";
    }
}