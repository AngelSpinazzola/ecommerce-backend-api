﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class Order
    {
        public int Id { get; set; }                   
        public int? UserId { get; set; }               
        public int? ShippingAddressId { get; set; }   
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string ShippingAddressType { get; set; }
        public string ShippingStreet { get; set; }
        public string ShippingNumber { get; set; }
        public string ShippingFloor { get; set; }
        public string ShippingApartment { get; set; }
        public string ShippingTower { get; set; }
        public string ShippingBetweenStreets { get; set; }
        public string ShippingPostalCode { get; set; }
        public string ShippingProvince { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingObservations { get; set; }

        // Persona autorizada copiada:
        public string AuthorizedPersonFirstName { get; set; }
        public string AuthorizedPersonLastName { get; set; }
        public string AuthorizedPersonPhone { get; set; }
        public string AuthorizedPersonDni { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } = "pending_payment";
        public string PaymentMethod { get; set; } = "bank_transfer";
        public string PaymentReceiptUrl { get; set; }
        public DateTime? PaymentReceiptUploadedAt { get; set; }
        public DateTime? PaymentApprovedAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string AdminNotes { get; set; }
        public string TrackingNumber { get; set; }
        public string ShippingProvider { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public virtual User User { get; set; }
        public virtual ShippingAddress ShippingAddress { get; set; }    
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
