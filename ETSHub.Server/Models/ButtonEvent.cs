using System.Text.Json.Serialization;

namespace ETSHub.Server.Models;

/// <summary>
/// Button event received from the web client
/// </summary>
public class ButtonEvent
{
    [JsonPropertyName("button")]
    public int Button { get; set; }
    
    [JsonPropertyName("action")]
    public string Action { get; set; } = "down"; // "down" or "up"
}
