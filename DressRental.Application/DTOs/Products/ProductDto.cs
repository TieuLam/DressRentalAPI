using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DressRental.Application.DTOs.Products
{
    public class ProductDto
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Color { get; set; }

        public long OriginalPrice { get; set; }

        public long Price3Days { get; set; }

        public long Price7Days { get; set; }

        public int DepositRate { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? ThumbnailImageUrl { get; set; }
    }
    public class ProductDetailDto : ProductDto
    {
        public List<string> ImageUrls { get; set; } = new List<string>();
        public List<ProductItemDto> AvailableItems { get; set; } = new List<ProductItemDto>();
    }

    public class ProductItemDto
    {
        public long Id { get; set; }

        public string ItemCode { get; set; } = null!;

        public string Size { get; set; } = null!;

        public string CurrentCondition { get; set; } = null!;

        public string Status { get; set; } = null!;
    }
}
