using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using ETSHub.Server.Models;
using ETSHub.Server.Services;
using QRCoder;

var builder = WebApplication.CreateBuilder(args);

// set Kestrel and listen on interfaces at a port
const int port = 5001;
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, port);
});

// Register joystick service: use real VJoy on Windows, otherwise fallback to Mock
if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
{
    builder.Services.AddSingleton<IJoystickService, VJoyService>();
}
else
{
    builder.Services.AddSingleton<IJoystickService, MockJoystickService>();
}


var app = builder.Build();

// Configure static files (HTML, CSS, JS from wwwroot) and WebSockets
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseWebSockets();

// Get joystick service from dependency injection container
var joystick = app.Services.GetRequiredService<IJoystickService>();

// WebSocket route for incoming iPad controller commands
app.Map("/ws", async context => 
{

    if (context.WebSockets.IsWebSocketRequest)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        Console.WriteLine("[IPAD] is connected via WebSocket");

        var buffer = new byte[1024 * 4];

        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                    "Closing", CancellationToken.None);
                    
                    Console.WriteLine("[IPAD] is disconnected");
                    break;
                } 

                var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);

                try {
                    var buttonEvent = JsonSerializer.Deserialize<ButtonEvent>(messageJson);
                    if (buttonEvent != null) {
                        bool isPressed = buttonEvent.Action.Equals("down", StringComparison.OrdinalIgnoreCase);
                        joystick.SetButton(buttonEvent.Button, isPressed);
                    }
                } catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing ButtonEvent: {ex.Message}");
                }   
        }
    }

    else {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
});

// Resolve local IP and print QR code for easy iPad pairing
string localIp = GetLocalIpAddress();
string serverUrl = $"http://{localIp}:{port}";

PrintQrCode(serverUrl);

app.Run();

// Helper method to find primary local IPv4 address
static string GetLocalIpAddress()
{
    try {
        using var socket = new Socket(AddressFamily.InterNetwork,
        SocketType.Dgram, 0);
        socket.Connect("8.8.8.8", 65530);
        var endPoint = socket.LocalEndPoint as IPEndPoint;
        return endPoint?.Address.ToString() ?? "127.0.0.1";
    }
    catch 
    {
        return "127.0.0.1";
    }
}

// Helper method to print ASCII QR code in terminal

static void PrintQrCode(string url) {
    Console.WriteLine("\n===================================");
    Console.WriteLine("Server is running");
    Console.WriteLine("Scan QR-Code or open this Link");
    Console.WriteLine($"{url}\n");

    Console.WriteLine("===================================");

    using var qrGenerator = new QRCodeGenerator();
    using var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.L);
    var qrCode = new AsciiQRCode(qrCodeData);
    string asciiArt = qrCode.GetGraphic(1, drawQuietZones: false);
    Console.WriteLine(asciiArt);
    Console.WriteLine("===================================\n");
}