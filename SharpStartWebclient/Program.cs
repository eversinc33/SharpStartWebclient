using System.Runtime.InteropServices;

/*
 * C# version of the BOF at https://github.com/outflanknl/C2-Tool-Collection/blob/main/BOF/StartWebClient/SOURCE/StartWebClient.c
 */

public class Program
{
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public class EVENT_DESCRIPTOR
    {
        [FieldOffset(0)]
        ushort Id = 1;
        [FieldOffset(2)]
        byte Version = 0;
        [FieldOffset(3)]
        byte Channel = 0;
        [FieldOffset(4)]
        byte Level = 4;
        [FieldOffset(5)]
        byte Opcode = 0;
        [FieldOffset(6)]
        ushort Task = 0;
        [FieldOffset(8)]
        long Keyword = 0;
    }

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct EventData
    {
        [FieldOffset(0)]
        internal UInt16 DataPointer;
        [FieldOffset(8)]
        internal uint Size;
        [FieldOffset(12)]
        internal int Reserved;
    }

    [DllImport("Advapi32.dll", SetLastError = true)]
    public static extern uint EventRegister(ref Guid guid, IntPtr EnableCallback, IntPtr CallbackContext, ref long RegHandle);

    [DllImport("Advapi32.dll", SetLastError = true)]
    public static extern unsafe uint EventWrite(long RegHandle, ref EVENT_DESCRIPTOR EventDescriptor, uint UserDataCount, EventData* UserData);

    [DllImport("Advapi32.dll", SetLastError = true)]
    public static extern uint EventUnregister(long RegHandle);

    public static Main(string[] args)
    {
        Guid WebClientTrigger = new Guid(0x22B6D684, 0xFA63, 0x4578, 0x87, 0xC9, 0xEF, 0xFC, 0xBE, 0x66, 0x43, 0xC7);
        long RegistrationHandle = 0;

        if (EventRegister(ref WebClientTrigger, IntPtr.Zero, IntPtr.Zero, ref RegistrationHandle) == 0)
        {
            EVENT_DESCRIPTOR EventDescriptor = new EVENT_DESCRIPTOR();

            unsafe
            {
                EventWrite(RegistrationHandle, ref EventDescriptor, 0, null);
                EventUnregister(RegistrationHandle);
            }

            Console.WriteLine("[*] Webclient should be started now");
        }
        else
        {
            Console.WriteLine("[!] Failed to run");
        }
    }
}
