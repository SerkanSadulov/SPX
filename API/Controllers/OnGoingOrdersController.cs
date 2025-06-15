using DAL.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OngoingOrdersController : ControllerBase
    {
        private readonly IOngoingOrderDAO _ongoingOrderDAO;

        public OngoingOrdersController(IOngoingOrderDAO ongoingOrderDAO)
        {
            _ongoingOrderDAO = ongoingOrderDAO;
        }


        [HttpGet("ByProvider/{providerId:guid}")]
        public ActionResult<IEnumerable<OngoingOrderEntity>> GetByProvider(Guid providerId)
        {
            if (providerId != Guid.Empty)
            {
                var orders = _ongoingOrderDAO.SelectByProvider(providerId);
                if (orders != null && orders.Any())
                {
                    return Ok(orders);
                }
                return NotFound("No orders found for the provider.");
            }
            return BadRequest("Invalid provider identifier.");
        }

        [HttpGet("ByUser/{userId:guid}")]
        public ActionResult<IEnumerable<OngoingOrderEntity>> GetByUser(Guid userId)
        {
            if (userId != Guid.Empty)
            {
                var orders = _ongoingOrderDAO.SelectByUser(userId);
                if (orders != null && orders.Any())
                {
                    return Ok(orders);
                }
                return Ok(new List<OngoingOrderEntity>());
            }
            return BadRequest("Invalid user identifier.");
        }


        [HttpPost]
        public ActionResult<OngoingOrderEntity> Post(OngoingOrderEntity order)
        {
            if (order == null || order.OrderID == Guid.Empty)
                return BadRequest("Invalid order entity or OrderID.");

            _ongoingOrderDAO.Insert(order);
            return CreatedAtAction(nameof(GetByProvider), new { providerId = order.ProviderID }, order);
        }

        [HttpPut("UpdateStatus/{orderId:guid}")]
        public ActionResult UpdateStatus(Guid orderId, [FromBody] string status)
        {
            if (orderId == Guid.Empty || string.IsNullOrWhiteSpace(status))
                return BadRequest("Invalid order identifier or status.");

            _ongoingOrderDAO.UpdateStatus(orderId, status);
            return NoContent();
        }
    }
}
