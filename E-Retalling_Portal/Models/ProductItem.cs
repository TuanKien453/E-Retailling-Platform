﻿using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class ProductItem
    {
        [Key]
        public int id { get; set; }
        public int  productId { get; set; }
        [Range(0, 10000)]
        public int quantity { get; set; }
        [Range(0, 10000000)]
        public double price { get; set; }
        public int imageId { get; set; }
        [Required,MaxLength(100)]
        public String attribute { get; set; }
        [MaxLength(60)]
        public string? deleteAt { get; set; }
        public Product? product { get; set; }
        public Image? image { get; set; }
        public List<OrderItem>? orderItems { get; set; }
        public List<ProductDiscount>? productDiscounts { get; set; }


        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
