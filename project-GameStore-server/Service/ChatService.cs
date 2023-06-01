using Newtonsoft.Json.Linq;
using project_GameStore_models.Models;
using System.Net.WebSockets;
using System.Text;

namespace project_GameStore_server.Service
{
    internal class ChatService
    {
        public static ChatService GetInstance() => _instance ??= new();
        private static ChatService? _instance;
        private ChatService() { }
        private HashSet<ChatRoom> ChatRooms { get; set; } = new();
        public CancellationToken CreateConnection(Client user, Game game, WebSocket webSocket)
        {
            var room = ChatRooms.FirstOrDefault(x => x.Game.Id == game.Id);
            if (room is null)
                ChatRooms.Add(room = new(game));
            return new ChatUser(user, webSocket, room).SocketToken;
        }

    }
    internal class ChatRoom
    {
        public ChatRoom(Game game)
        {
            Game = game ?? throw new ArgumentNullException(nameof(game));
        }
        public Game Game { get; private set; }
        public HashSet<ChatUser> Users { get; private set; } = new();

        public void SendMessage(object message, ChatUser? source = null)
        {
            Users.RemoveWhere(x => x.SocketToken.IsCancellationRequested || x.Socket.State != WebSocketState.Open);
            foreach (var user in Users)
            {
                if (user != source)
                {
                    user.SendMessage(message);
                }
            }
        }
    }
    internal class ChatUser
    {
        public ChatUser(Client @base, WebSocket socket, ChatRoom chatRoom)
        {
            Base = @base ?? throw new ArgumentNullException(nameof(@base));
            Socket = socket ?? throw new ArgumentNullException(nameof(socket));
            ChatRoom = chatRoom ?? throw new ArgumentNullException(nameof(chatRoom));
            ChatRoom.Users.Add(this);
            Task.Run(ListenerTask, SocketToken);
        }
        public void SendMessage(string message) =>
            Socket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, SocketToken);
        public void SendMessage(object message) =>
            SendMessage(JObject.FromObject(message).ToString());
        private async void ListenerTask()
        {
            while (Socket.State == WebSocketState.Open)
            {
                ArraySegment<byte> buffer = new(new byte[4096]);
                var socketMessage = await Socket.ReceiveAsync(buffer, SocketToken);
                if (SocketToken.IsCancellationRequested || socketMessage.MessageType == WebSocketMessageType.Close)
                    break;

                try
                {
                    var json = JObject.Parse(Encoding.UTF8.GetString(buffer));
                    switch ((string?)json["status"])
                    {
                        case "message":
                            ChatRoom.SendMessage(json.ToString(), this);
                            break;
                        case "close":
                            ChatRoom.SendMessage(new
                            {
                                status = "message",
                                message = $"{Base.Name} is offline!"
                            }, this);
                            break;

                        case "notify":
                        case "photo":
                        case null:
                        default:
                            throw new Exception();
                    }
                }
                catch (Exception)
                {
                    SendMessage(new
                    {

                    });
                }
            }
        }
        public Client Base { get; private set; }
        public WebSocket Socket { get; private set; }
        public ChatRoom ChatRoom { get; private set; }
        private CancellationTokenSource SocketTokenSource { get; set; } = new();
        public CancellationToken SocketToken => SocketTokenSource.Token;


    }
}
