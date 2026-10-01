using DressRental.Application.DTOs.Products;
using DressRental.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DressRentalAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Lấy danh sách mẫu váy trưng bày (lọc theo danh mục, màu sắc, size)
        /// GET: api/products?categoryId=1&color=Trắng&size=M
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), 200)]
        public async Task<IActionResult> GetProducts([FromQuery] long? categoryId, [FromQuery] string? size, [FromQuery] string? color, CancellationToken cancellationToken)
        {
            var products = await _productService.GetProductsAsync(categoryId, size, color, cancellationToken);
            return Ok(products);
        }

        /// <summary>
        /// Xem chi tiết một mẫu váy (bao gồm bộ ảnh và các chiếc váy thực tế theo size)
        /// GET: api/products/1001
        /// </summary>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ProductDto), 200)]
        public async Task<IActionResult> GetProductById(long id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetProductByIdAsync(id, cancellationToken);
            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm" });
            }
            return Ok(product);
        }
    }
}
