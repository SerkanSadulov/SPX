using DAL.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class MessagesController : ControllerBase
    {
        private readonly IMessagesDAO _messagesDAO;

        public MessagesController(IMessagesDAO messagesDAO)
        {
            _messagesDAO = messagesDAO;
        }

        [HttpGet]
        public ActionResult<IEnumerable<MessagesEntity>> GetMessages(Guid senderId, Guid receiverId)
        {
            if (senderId == Guid.Empty || receiverId == Guid.Empty)
                return BadRequest("Invalid sender or receiver ID.");

            var messages = _messagesDAO.GetMessages(senderId, receiverId);
            return Ok(messages);
        }
  
        [HttpGet("received/{receiverId:guid}")]
        public ActionResult<IEnumerable<MessagesEntity>> GetMessagesByReceiverID(Guid receiverId)
        {
            if (receiverId == Guid.Empty)
                return BadRequest("Invalid receiver ID.");
            var messages = _messagesDAO.GetMessagesByReceiverID(receiverId);
            return Ok(messages);
        }

        [HttpGet("{userId:guid}")]
        public ActionResult<IEnumerable<Guid>> GetMessagedPeople(Guid userId)
        {
            if (userId != Guid.Empty)
            {
                var messagedPeople = _messagesDAO.GetMessagedPeople(userId);
                return Ok(messagedPeople);
            }
            return BadRequest("Invalid user identifier.");
        }

        [HttpPost]
        public ActionResult<MessagesEntity> PostMessage(MessagesEntity message)
        {
            if (message == null || message.SenderID == Guid.Empty || message.ReceiverID == Guid.Empty)
                return BadRequest("Invalid message entity or IDs.");

            _messagesDAO.SaveMessage(message);
            return CreatedAtAction(nameof(GetMessages), new { senderId = message.SenderID, receiverId = message.ReceiverID }, message);
        }
    }
}
