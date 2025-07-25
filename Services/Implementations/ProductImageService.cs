﻿using EcommerceAPI.Data;
using EcommerceAPI.DTOs.Products;
using EcommerceAPI.Models;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Services.Implementations
{
    public class ProductImageService : IProductImageService
    {
        private readonly IProductImageRepository _productImageRepository;
        private readonly IProductRepository _productRepository;
        private readonly IFileService _fileService;

        public ProductImageService(
            IProductImageRepository productImageRepository,
            IProductRepository productRepository,
            IFileService fileService)
        {
            _productImageRepository = productImageRepository;
            _productRepository = productRepository;
            _fileService = fileService;
        }

        public async Task<IEnumerable<ProductImageDto>> GetProductImagesAsync(int productId)
        {
            var images = await _productImageRepository.GetByProductIdAsync(productId);
            return images.Select(img => new ProductImageDto
            {
                Id = img.Id,
                ProductId = img.ProductId,
                ImageUrl = img.ImageUrl,
                DisplayOrder = img.DisplayOrder,
                IsMain = img.IsMain,
                CreatedAt = img.CreatedAt
            });
        }

        public async Task<IEnumerable<ProductImageDto>> CreateProductImagesAsync(CreateProductImageDto createDto)
        {
            var createdImages = new List<ProductImage>();

            // Verifica que el producto exista
            var product = await _productRepository.GetByIdAsync(createDto.ProductId);
            if (product == null)
                throw new ArgumentException("Producto no encontrado");

            // Obtiene el siguiente orden de display correctamente
            var existingImages = await _productImageRepository.GetByProductIdAsync(createDto.ProductId);
            int nextDisplayOrder = 0;
            if (existingImages.Any())
            {
                nextDisplayOrder = existingImages.Max(img => img.DisplayOrder) + 1;
            }

            Console.WriteLine($"🔍 Imágenes existentes: {existingImages.Count()}");
            Console.WriteLine($"🔍 Próximo DisplayOrder: {nextDisplayOrder}");

            // Procesa archivos de imagen
            if (createDto.ImageFiles != null && createDto.ImageFiles.Length > 0)
            {
                for (int i = 0; i < createDto.ImageFiles.Length; i++)
                {
                    var imageFile = createDto.ImageFiles[i];
                    var imageUrl = await _fileService.SaveImageAsync(imageFile);

                    var productImage = new ProductImage
                    {
                        ProductId = createDto.ProductId,
                        ImageUrl = imageUrl,
                        DisplayOrder = nextDisplayOrder + i,
                        IsMain = createDto.MainImageIndex.HasValue && createDto.MainImageIndex.Value == i 
                    };

                    Console.WriteLine($"🖼️ Creando imagen con DisplayOrder: {productImage.DisplayOrder}");

                    var created = await _productImageRepository.CreateAsync(productImage);
                    createdImages.Add(created);
                }
            }

            // Procesa URLs de imagen
            if (createDto.ImageUrls != null && createDto.ImageUrls.Length > 0)
            {
                int urlStartOrder = nextDisplayOrder + (createDto.ImageFiles?.Length ?? 0);
                for (int i = 0; i < createDto.ImageUrls.Length; i++)
                {
                    var imageUrl = createDto.ImageUrls[i];
                    if (!string.IsNullOrWhiteSpace(imageUrl) && imageUrl != "string")
                    {
                        var productImage = new ProductImage
                        {
                            ProductId = createDto.ProductId,
                            ImageUrl = imageUrl,
                            DisplayOrder = urlStartOrder + i,
                            IsMain = false
                        };

                        var created = await _productImageRepository.CreateAsync(productImage);
                        createdImages.Add(created);
                    }
                }
            }

            //if (createdImages.Any() && createDto.MainImageIndex.HasValue &&
            //    createDto.MainImageIndex.Value >= 0 &&
            //    createDto.MainImageIndex.Value < createdImages.Count)
            //{
            //    var mainImageIndex = createDto.MainImageIndex.Value;
            //    await SetMainImageAsync(createDto.ProductId, createdImages[mainImageIndex].Id);
            //}

            return createdImages.Select(img => new ProductImageDto
            {
                Id = img.Id,
                ProductId = img.ProductId,
                ImageUrl = img.ImageUrl,
                DisplayOrder = img.DisplayOrder,
                IsMain = img.IsMain,
                CreatedAt = img.CreatedAt
            });
        }

        public async Task<bool> DeleteProductImageAsync(int productId, int imageId)
        {
            var productImage = await _productImageRepository.GetByIdAsync(imageId);
            if (productImage == null || productImage.ProductId != productId)
                return false;

            // Elimina archivo si es local
            if (!string.IsNullOrEmpty(productImage.ImageUrl) && !productImage.ImageUrl.StartsWith("http"))
            {
                await _fileService.DeleteImageAsync(productImage.ImageUrl);
            }

            var wasMain = productImage.IsMain;
            var deleted = await _productImageRepository.DeleteAsync(imageId);

            // Si era la imagen principal, establece otra como principal
            if (deleted && wasMain)
            {
                var remainingImages = await _productImageRepository.GetByProductIdAsync(productId);
                var firstImage = remainingImages.OrderBy(img => img.DisplayOrder).FirstOrDefault();
                if (firstImage != null)
                {
                    await SetMainImageAsync(productId, firstImage.Id);
                }
            }

            return deleted;
        }

        public async Task<bool> SetMainImageAsync(int productId, int imageId)
        {
            return await _productImageRepository.SetMainImageAsync(productId, imageId);
        }

        public async Task<bool> UpdateImagesOrderAsync(int productId, UpdateProductImagesOrderDto updateOrderDto)
        {
            // Verifica que todas las imágenes pertenezcan al producto
            var existingImages = await _productImageRepository.GetByProductIdAsync(productId);
            var imageIds = existingImages.Select(img => img.Id).ToHashSet();

            var imageOrders = new Dictionary<int, int>();
            foreach (var imageOrder in updateOrderDto.Images)
            {
                if (!imageIds.Contains(imageOrder.ImageId))
                    return false; // Imagen no pertenece al producto

                imageOrders[imageOrder.ImageId] = imageOrder.DisplayOrder;
            }

            return await _productImageRepository.UpdateMultipleDisplayOrderAsync(imageOrders);
        }

        public async Task<ProductImageDto?> GetProductImageAsync(int productId, int imageId)
        {
            var productImage = await _productImageRepository.GetByIdAsync(imageId);
            if (productImage == null || productImage.ProductId != productId)
                return null;

            return new ProductImageDto
            {
                Id = productImage.Id,
                ProductId = productImage.ProductId,
                ImageUrl = productImage.ImageUrl,
                DisplayOrder = productImage.DisplayOrder,
                IsMain = productImage.IsMain,
                CreatedAt = productImage.CreatedAt
            };
        }
    }
}
