using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("orders")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrderController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("place-order/{userId}")]
    [Authorize(Roles = "Admin,Seller,Customer")]
    public IActionResult PlaceOrder(int userId)
    {
        var UserId = User.FindFirst("id").Value;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (UserId == null)
        {
            return Unauthorized();
        }

        if (!int.TryParse(UserId, out var tokenUserId))
        {
            return Unauthorized("Invalid user ID in token.");
        }

        if (userId != tokenUserId && userRole != "Admin")
        {
            return Forbid();
        }
        var success = _orderService.PlaceOrder(userId);
        if (success)
        {
            return Ok(new { Message = "Order created successfully" });
        }
        return BadRequest(new { Message = "Order could not be placed. Please check your balance, cart and stock availability." });
    }

    [HttpGet("user-history/{userId}")]
    [Authorize(Roles = "Admin,Seller,Customer")]
    public IActionResult GetUserOrderHistory(int userId)
    {
        var UserId = User.FindFirst("id").Value;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (UserId == null)
        {
            return Unauthorized();
        }

        if (!int.TryParse(UserId, out var tokenUserId))
        {
            return Unauthorized("Invalid user ID in token.");
        }

        if (userId != tokenUserId && userRole != "Admin")
        {
            return Forbid();
        }
        var orders = _orderService.GetUserOrderHistory(userId);
        if (orders == null || !orders.Any())
        {
            return NotFound(new { Message = "No past orders found." });
        }
        return Ok(orders);
    }
}
