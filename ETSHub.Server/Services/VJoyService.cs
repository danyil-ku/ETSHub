using System.Runtime.InteropServices;

namespace ETSHub.Server.Services;

/// <summary>
/// Real vJoy driver service for Windows
/// </summary>
public class VJoyService : IJoystickService
{
    private const uint DeviceId = 1;

    // Import native functions from vJoyInterface.dll
    [DllImport("vJoyInterface.dll", EntryPoint = "AcquireVJD")]
    private static extern bool AcquireVJD(uint rID);

    [DllImport("vJoyInterface.dll", EntryPoint = "RelinquishVJD")]
    private static extern void RelinquishVJD(uint rID);

    [DllImport("vJoyInterface.dll", EntryPoint = "SetBtn")]
    private static extern bool SetBtn(bool value, uint rID, byte nBtn);

    public VJoyService()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            bool acquired = AcquireVJD(DeviceId);
            if (acquired)
            {
                Console.WriteLine($"[vJoy] Device #{DeviceId} successfully acquired!");
            }
            else
            {
                Console.WriteLine($"⚠️ [vJoy] Failed to acquire device #{DeviceId}. Please verify Configure vJoy.");
            }
        }
    }

    public void SetButton(int buttonId, bool isPressed)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Set vJoy button state: true = pressed, false = released
            SetBtn(isPressed, DeviceId, (byte)buttonId);
        }

        Console.WriteLine($"🎮 [VJOY] Button {buttonId} -> {(isPressed ? "PRESSED" : "RELEASED")}");
    }
}
