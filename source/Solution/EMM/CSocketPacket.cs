namespace EnigmaMM
{
    public class CSocketPacket
    {
        public const int BufferSize = 32;
        public System.Net.Sockets.Socket thisSocket;
        public byte[] dataBuffer = new byte[BufferSize];
    }
}
