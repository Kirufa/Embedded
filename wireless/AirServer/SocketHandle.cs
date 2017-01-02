using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.InteropServices;



namespace AirServer.MySocket
{

    
    [Serializable]
    public class DataGram
    {        
        public int Type;
        public int DataLength;
        public byte[] Data = new byte[SocketData.MAX_DATASIZE];
        
        //1 client->server i2c data
        //2 server->client i2c data require
        //3 ---
        //4 server->client pwm set
        //5 ---
        //6 server->client i2c data stop

        //i2c data: (ax ay az)(gx gy gz)
        //pwm data: (num period duty)     
    }

    public static class SocketData
    {
        public const int MAX_DATASIZE = 192;

        //TCP
        public static ServerData Server = new ServerData();
        public static List<ClientData> Client = new List<ClientData>();

        //other
        public const int Port = 61357;
        public const int MAX_LISTEN_SIZE = 1;
        public const int THREAD_WAIT_TIME = 1000;   // msec


        public class ServerData
        {
            //Sokcet
            private Socket TCPServer;

            //IPEndPoint
            private IPEndPoint TCPEndPoint;

            //background Thread
            private Thread Accept_Thread;

            //other
            private int ListenCount = MAX_LISTEN_SIZE;
            public static string ServerIP = "192.168.1.101";
            private bool AcceptRunning = true;


            /// <summary>
            /// Initialize TCP Server
            /// </summary>
            public void InitialTCPServer()
            {
                Console.WriteLine("Initializing TCP server.");

                TCPServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                TCPEndPoint = new IPEndPoint(IPAddress.Parse(ServerIP), SocketData.Port);
                EndPoint _EndPoint = (EndPoint)TCPEndPoint;
                TCPServer.Bind(_EndPoint);
                TCPServer.Listen(MAX_LISTEN_SIZE);

                Console.WriteLine("Initialized TCP server.");
                Console.WriteLine("Start TCP accept thread.");
                TCPAcceptInitial();
            }


            private void TCPAcceptInitial()
            {
                Accept_Thread = new Thread(this.Accept_Thread_Run);
                Accept_Thread.IsBackground = true;
                Accept_Thread.Start();
            }


            /// <summary>
            /// Aceept Thread
            /// </summary>
            private void Accept_Thread_Run()
            {
                while (AcceptRunning)
                {
                    if (ListenCount > 0)
                    {
                        Socket _Socket = TCPServer.Accept();
                        IPEndPoint _EP = _Socket.RemoteEndPoint as IPEndPoint;                      
                        ClientData _Client = new ClientData(_Socket);
                        Client.Add(_Client);
                        ListenCount--;

                        Console.WriteLine("Clint " + _EP.Address.ToString() + ":" + _EP.Port.ToString() + " connected.");
                    }
                    Thread.Sleep(THREAD_WAIT_TIME);
                }
            }



        }

        public class ClientData
        {
            private Socket TCP_Client;

            private IPEndPoint EP;

            public int ID;
            public string Name;

            private Thread Receive_Thread;


            public bool ReceiveRunning = true;

            public ClientData(System.Net.Sockets.Socket socket)
            {
                TCP_Client = socket;
                ID = socket.GetHashCode();
                Receive_Thread = new Thread(this.Receive_Thread_Run);
                Receive_Thread.IsBackground = true;
                Receive_Thread.Start();

                EP = (IPEndPoint)socket.RemoteEndPoint;
            }

            ~ClientData()
            {
                TCP_Client.Close();
                this.ReceiveRunning = false;
            }

            public void TCPSend(DataGram _Dgram)
            {
                byte[] _Arr = SocketHandle.DgramToByte(_Dgram);

                try
                {
                   
                    TCP_Client.Send(_Arr, _Arr.Length, SocketFlags.None);
                    //Thread.Sleep(SocketData.THREAD_SLEEP_TIME);
                }
                catch (Exception ex)
                {
                    this.ReceiveRunning = false;
                }

            }

            private void Receive_Thread_Run()
            {
                while (ReceiveRunning)
                {                
                    DataGram _Temp = new DataGram();
                    byte[] _Arr = new byte[MAX_DATASIZE * sizeof(byte) + 2 * sizeof(int)];
                    
             
                    try
                    {
                        TCP_Client.Receive(_Arr, _Arr.Length, SocketFlags.None);
                    }
                    catch (Exception ex)
                    {             
                        break;
                    }
                   
                    _Temp = SocketHandle.ByteToDgram(_Arr, _Arr.Length);
                    switch (_Temp.Type)
                    {
                        case 1:
                            Int16[] data = new short[6];
                            for (int i = 0; i != 6; ++i)
                                BitConverter.ToInt16(_Temp.Data, i * sizeof(Int16));

                            Console.WriteLine(
                                "ax={0} ay={1} az={2} , gx={3} gy={4} gz={5}",
                                data[0], data[1], data[2], data[3], data[4], data[5]);

                            break;
                        default:
                            break;
                    }
                }
            }
        }



    }

    public static class SocketHandle
    {
        public static byte[] StrToByte(string _Str)
        {
            return Encoding.Unicode.GetBytes(_Str);
        }

        public static string ByteToStr(byte[] _Arr, int Len)
        {
            return Encoding.Unicode.GetString(_Arr, 0, Len);
        }

        public static byte[] DgramToByte(DataGram dgram)
        {
            List<byte> byteLst = new List<byte>();
            byteLst.AddRange(BitConverter.GetBytes(dgram.Type));
            byteLst.AddRange(BitConverter.GetBytes(dgram.DataLength));
            byteLst.AddRange(dgram.Data);

            return byteLst.ToArray();
        }

        public static DataGram ByteToDgram(byte[] _Arr, int _Len)
        {
            int index = 0;

            DataGram dgram = new DataGram();

            dgram.Type = BitConverter.ToInt32(_Arr, index);
            index += sizeof(int);
            dgram.DataLength = BitConverter.ToInt32(_Arr, index);
            index += sizeof(int);
            Array.Copy(_Arr, index, dgram.Data, 0, dgram.Data.Length);

            return dgram;
        }
    }

}
