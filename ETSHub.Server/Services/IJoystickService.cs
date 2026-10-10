namespace ETSHub.Server.Services;

/// <summary>
/// Interface for joystick service
/// </summary>

public interface IJoystickService
{
    /// <summary>
    /// set button state
    /// </summary>
    /// <param name="buttonId">Button id</param>
    /// <param name="isPressed">Button state</param>
    void SetButton(int buttonId, bool isPressed);

}