using System.Runtime.InteropServices;

namespace ETSHub.Server.Services;

public class VJoyService : IJoystickService
{
    private const uint DeviceId = 1;

    // Импорт функций из системной библиотеки vJoyInterface.dll
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
                Console.WriteLine($"[vJoy] Устройство #{DeviceId} успешно захвачено!");
            }
            else
            {
                Console.WriteLine($"⚠️ [vJoy] Не удалось захватить устройство #{DeviceId}. Проверьте Configure vJoy.");
            }
        }
    }

    public void SetButton(int buttonId, bool isPressed)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Устанавливаем кнопку в vJoy: true = нажата, false = отпущена
            SetBtn(isPressed, DeviceId, (byte)buttonId);
        }

        Console.WriteLine($"🎮 [VJOY] Button {buttonId} -> {(isPressed ? "PRESSED" : "RELEASED")}");
    }
}
