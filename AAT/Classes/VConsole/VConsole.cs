using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace AAT
{
    public class VConsole
    {
        public static int port = 29000;
        public static string ipadresse = "127.0.0.1";

        public static TcpClient ClientInstance = new();

        public static Task TryConnectClient()
        {
            ClientInstance.ExclusiveAddressUse = false;
            return ClientInstance.ConnectAsync(ipadresse, port);

        }

        private static byte[] GetCommandBytes(string Command)
        {
            int PacketSize = 0xD + Command.Length;
            byte[] startBytes = new byte[] { 0x43, 0x4D, 0x4E, 0x44, 0x0, 0xD3, 0x0, 0x0, 0x0, (byte)PacketSize, 0x0, 0x0 };
            // D3 = Protocol = 211
            List<byte> FinalBytes = new List<byte>();

            FinalBytes.AddRange(startBytes.ToList());
            FinalBytes.AddRange(System.Text.ASCIIEncoding.ASCII.GetBytes(Command));
            FinalBytes.Add(0);

            return FinalBytes.ToArray();
        }

        public static async void SendCommand(string Command)
        {

            if (!ClientInstance.Connected)
                try
                {
                    await TryConnectClient();
                }
                catch (Exception)
                {
                    Pages.Editor.CreateMessageDialog(Soundevents.ErrorCodes.CONNECTION_REFUSED);
                    return;
                }
            System.Diagnostics.Debug.WriteLine((Command).Trim().ToLower());
            System.Diagnostics.Debug.WriteLine(ClientInstance.ExclusiveAddressUse);

            ClientInstance.Client.Send(GetCommandBytes((Command).Trim().ToLower()));
        }
    }
}
