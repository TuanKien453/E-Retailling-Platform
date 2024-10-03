﻿using System.ComponentModel.DataAnnotations;

namespace E_Retalling_Portal.Models
{
    public class Image
    {
        [Key]
        public int id { get; set; }
        public int productId { get; set; }
        public byte imageData { get; set; }
        public ProductItem productItem { get; set; }
        public Product product { get; set; }
    }
}
