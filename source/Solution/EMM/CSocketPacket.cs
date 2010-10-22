using System.Net.Sockets;
namespace EnigmaMM
 {
     public class CSocketPacket
     {
         public const int BUFFER_SIZE = 32;

         public Socket ThisSocket;
         public int ClientNumber;
         public byte[] DataBuffer = new byte[BUFFER_SIZE];

         public CSocketPacket(Socket socket)
         {
             ThisSocket = socket;
         }

         public CSocketPacket(Socket socket, int clientNumber)
         {
             ThisSocket = socket;
             ClientNumber = clientNumber;
         }
     }
 }