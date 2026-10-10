using System.Text.Json.Serialization;

namespace ETSHub.Server.Models;

/// <summary>

/// </summary>

public class ButtonEvent
{
    [JsonPropertyName("button")]
    public int Button { get; set; }
    
    [JsonPropertyName("action")]
    public string Action { get; set; } = "down";

}
