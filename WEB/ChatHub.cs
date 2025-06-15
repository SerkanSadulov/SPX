using DAL.Interfaces;
using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    private readonly IUserDAO _userDAO;

    public ChatHub(IUserDAO userDAO)
    {
        _userDAO = userDAO;
    }

    public async Task JoinRoom(string providerId)
    {
        // Add logic to handle joining a chat room or managing connections
        await Groups.AddToGroupAsync(Context.ConnectionId, providerId);
    }

    public async Task SendMessage(string providerId, string message)
    {
        var senderId = Context.UserIdentifier; // Ensure this is set correctly
        var sender = _userDAO.Select(Guid.Parse(senderId)); // Get sender info

        // Save the message to the database
        // Assume you have a method for this
        SaveMessageToDatabase(sender.UserId, providerId, message);

        // Send the message to the provider
        await Clients.Group(providerId).SendAsync("ReceiveMessage", sender.Username, message);
    }

    private void SaveMessageToDatabase(Guid senderId, string receiverId, string messageContent)
    {
        // Implement your database saving logic here
    }
}
