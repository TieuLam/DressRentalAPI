using DressRental.Application.DTOs.Orders;
using DressRental.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DressRentalAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Kiểm tra váy có available trong khoảng ngày yêu cầu không (đã tính 2 ngày giặt)
    /// POST: api/orders/check-availability
    /// </summary>
    [HttpPost("check-availability")]
    [ProducesResponseType(typeof(CheckAvailabilityResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckAvailability(
        [FromBody] CheckAvailabilityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _orderService.CheckAvailabilityAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Tạo đơn đặt thuê váy mới (Trạng thái ban đầu: PENDING - giữ trong 12h)
    /// POST: api/orders
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.CreateOrderAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
    }

    /// <summary>
    /// Xem thông tin chi tiết order theo ID
    /// GET: api/orders/1
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderById(long id, CancellationToken cancellationToken)
    {
        var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
        if (order == null)
        {
            return NotFound(new { message = $"Không tìm thấy đơn hàng #{id}" });
        }

        return Ok(order);
    }

    /// <summary>
    /// Confirm khách hàng đã nộp tiền cọc (Chuyển trạng thái Order sang CONFIRMED)
    /// POST: api/orders/1/confirm-deposit
    /// </summary>
    [HttpPost("{id:long}/confirm-deposit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmDeposit(
        long id,
        [FromBody] ConfirmDepositRequest request,
        CancellationToken cancellationToken)
    {
        bool success = await _orderService.ConfirmDepositAsync(id, request.Amount, request.PaymentMethod, cancellationToken);
        if (!success)
        {
            return BadRequest(new { message = "Xác nhận tiền cọc thất bại." });
        }

        return Ok(new { message = $"Đã xác nhận đặt cọc thành công cho đơn hàng #{id}. Chiếc váy chính thức được khóa lịch!" });
    }
}

public class ConfirmDepositRequest
{
    public long Amount { get; set; }
    public string PaymentMethod { get; set; } = "CASH"; // "CASH" hoặc "BANK_TRANSFER"
}
