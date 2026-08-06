using Microsoft.AspNetCore.SignalR;

namespace TicTacToe.Api.Hubs
{
    public interface IGameClient
    {
        Task ReceiveGameState(object gameState);
        Task SystemNotification(string announcement);
    }

    public class GameHub : Hub<IGameClient>
    {
        public async Task JoinSession(int gameId)
        {
            string groupName = $"Game_{gameId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SystemNotification($"Combatant joined link channel: {Context.ConnectionId}");
        }

        public async Task LeaveSession(int gameId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Game_{gameId}");
        }
    }
}
