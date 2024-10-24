namespace E_Retalling_Portal.Models.GHNRequestModel
{
    public class OrderItemRequest
    {
        public string Name { get; set; }
        public int code { get; set; }
        public int Weight { get; } = 1000;
        public int Length { get; } = 30;
        public int Width { get; } = 30;
        public int Height { get; } = 30;
        public CategoryRequest Category { get; set; }
    }
}
