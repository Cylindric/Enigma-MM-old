using System.Net.Sockets;
namespace EnigmaMM
 {
     public class SocketPacket
     {
         public const int BUFFER_SIZE = 64;

         public Socket ThisSocket;
         public int ClientNumber;
         public byte[] DataBuffer = new byte[BUFFER_SIZE];

         public SocketPacket(Socket socket)
         {
             ThisSocket = socket;
         }

         public SocketPacket(Socket socket, int clientNumber)
         {
             ThisSocket = socket;
             ClientNumber = clientNumber;
         }
     }
 }