using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Xml.Serialization;



namespace AirServer.MySocket
{

    
    [Serializable]
    public class DataGram
    {       
        public byte[] Data = new byte[SocketData.MAX_DATASIZE];
        public int DataLength;
        public int Type;
        //0: check alive, client <-> server
        //1: TCP Text Data, client -> server 
        //2: TCP Text Data, server -> client 
        //3: TCP UDP Image Size, follow the UDP Image, client -> server 
        //4: TCP UDP Image Size, follow the UDP Image, server -> client
        //5: UDP Image, client -> server
        //6: UDP Image, server -> client
        //7: TCP send Name client -> server
        //20: TCP System message server -> client
    }

    public static class SocketData
    {
        public const int MAX_DATASIZE = 1024;

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
            public static string ServerIP = "192.168.0.106";
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
                   // //send to all online member
                   // DataGram _Temp = new DataGram();
                   // byte[] _Arr = new byte[RECEIVE_LENGTH];
                   // byte[] _Len = new byte[4];
                   // int RecvSize;
                   // try
                   // {
                   //     TCP_Client.Receive(_Len, 4, SocketFlags.None);
                   //     Thread.Sleep(THREAD_SLEEP_TIME);
                   //     RecvSize = TCP_Client.Receive(_Arr, BitConverter.ToInt32(_Len, 0), SocketFlags.None);
                   //     Thread.Sleep(THREAD_SLEEP_TIME);
                   // }
                   // catch (Exception ex)
                   // {
                   //     Form_Server.AddText(Name, ID.ToString(), "Except", ex.ToString());
                   //     break;
                   // }
                   //
                   // _Temp = Socket.ByteToDgram(_Arr, RecvSize);
                   // switch (_Temp.Type)
                   // {
                   //     case 0:
                   //         break;
                   //     case 1:
                   //         string _Str = ByteToStr(_Temp.Data, _Temp.DataLength);
                   //         Form_Server.AddText(Name, ID.ToString(), "Recieve", _Str);
                   //         _Temp.Type = 2;
                   //         SocketHandle.SendToAllClient(_Temp, false);
                   //
                   //         break;
                   //     case 3:
                   //         //  MessageBox.Show(_Temp.Type.ToString());                    
                   //         // int _N = Convert.ToInt32(ByteToStr(_Temp.Data, _Temp.DataLength));
                   //         //MessageBox.Show();
                   //         SocketHandle.ReSendBitmap(_Temp, this.TCP_Client);
                   //         break;
                   //     case 5:
                   //         break;
                   //     case 7:
                   //         string _Str2 = ByteToStr(_Temp.Name, _Temp.NameLength);
                   //         this.Name = _Str2;
                   //         Form_Server.AddText(Name, ID.ToString(), "Name", "Recieve Name.");
                   //         _Temp.Type = 20;
                   //         SocketHandle.SendToAllClient(_Temp, false);
                   //         break;
                   //     default:
                   //         break;
                   // }
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
            MemoryStream _Ms = new MemoryStream();
            XmlSerializer _Xs = new XmlSerializer(typeof(DataGram));
            _Xs.Serialize(_Ms, dgram);
            return _Ms.ToArray();
        }

        public static DataGram ByteToDgram(byte[] _Arr, int _Len)
        {
            MemoryStream _Ms = new MemoryStream(_Arr, 0, _Len);
            XmlSerializer _Xs = new XmlSerializer(typeof(DataGram));
            //  MessageBox.Show(_Ms.ToArray().Length.ToString());
            /*
            using (FileStream fs = new FileStream("123.xml", FileMode.Create))
            {
                fs.Write(_Ms.ToArray(), 0, (int)_Ms.Length);
                fs.Close();
            }
            */


            DataGram _Ret = (DataGram)_Xs.Deserialize(_Ms);
            return _Ret;
        }
    }

}
