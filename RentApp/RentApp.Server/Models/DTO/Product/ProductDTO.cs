﻿using System.Text.Json.Serialization;

namespace RentApp.Server.Models.DTO.Product
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime AddedAt { get; set; }
        public bool Available { get; set; }
        public decimal PricePerDay { get; set; }
        public string UserName { get; set; }
        public string ImagePath { get; set; }
        public string Location { get; set; }
        public double? AverageRating { get; set; } // poate fi null daca nu sunt review-uri
    }
}
