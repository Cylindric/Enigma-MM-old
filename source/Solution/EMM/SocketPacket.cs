using System.Net.Sockets;
namespace EnigmaMM
 {
     /// <summary>
     /// Simple Class for representing a Socket connection and for passing socket state between sockets.
     /// </summary>
     public class SocketPacket
     {
         /// <summary>
         /// The size of the datapackets to send per chunk (bytes)
         /// </summary>
         public const int BUFFER_SIZE = 64;

         public Socket ThisSocket;
         public int ClientNumber;
         public byte[] DataBuffer = new byte[BUFFER_SIZE];
         public string UserHash;
         public string PassHash;
         public bool Authenticated = false;

         /// <summary>
         /// Initialises a new SocketPacket encapsulating the supplied socket.
         /// </summary>
         /// <param name="socket">The socket to identify</param>
         public SocketPacket(Socket socket)
         {
             ThisSocket = socket;
         }

         /// <summary>
         /// Initialises a new SocketPacket encapsulating the supplied socket and
         /// tags it with the supplied identifier.
         /// </summary>
         /// <param name="socket">The socket to identify</param>
         /// <param name="clientNumber">The identifier to use for the socket</param>
         public SocketPacket(Socket socket, int clientNumber)
         {
             ThisSocket = socket;
             ClientNumber = clientNumber;
         }
     }
 }