﻿namespace EcommerceAPI.DTOs.Orders
{
    public class UpdateOrderStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public string? AdminNotes { get; set; }
    }
}
