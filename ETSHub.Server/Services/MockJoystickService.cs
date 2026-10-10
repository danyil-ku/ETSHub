namespace ETSHub.Server.Services;

public class MockJoystickService : IJoystickService
{
    public void SetButton(int buttonId, bool isPressed)
    {
        string status = isPressed ? "pressed" : "released";
        Console.WriteLine($"[MOCK JOYSTICK] Button {buttonId} -> {status}");
    }

}