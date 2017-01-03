using System;
using System.Collections.Generic;
using System.Text;

using AirServer.MySocket;

namespace AirServer.Main
{
    class Program
    {
        static bool runningFlag = true;
        static List<string> par;

        static void Main(string[] args)
        {
            
            string str;

            Console.WriteLine("Process start.");

            while (runningFlag)
            {
                Console.WriteLine("Waiting instruction...");
                str = Console.ReadLine();               

                Instruction inst = parseStr(str);
                processInst(inst);
            }

            Console.WriteLine("Process end.");
        }

        private static Instruction parseStr(string str)
        {
            Instruction inst;

            str = str.ToLower();
            string[] strs = str.Split(new char[] { ' ' });
            str = strs[0];

            switch(str)
            {              
                case "ini":
                case "initialize":
                case "initial":
                case "init":
                    inst = Instruction.Initialize;
                    break;

                case "end":
                case "e":
                    inst = Instruction.End;
                    break;
                case "si2c":
                    inst = Instruction.StartI2C;
                    break;
                case "ei2c":
                    inst = Instruction.EndI2C;
                    break;
                default:
                    inst = Instruction.None;
                    break;

                case "pwm":
                    if (strs.Length != 4)
                    {
                        inst = Instruction.None;
                        break;
                    }
                    else
                    {
                        par = new List<string>();
                        par.Add(strs[1]);
                        par.Add(strs[2]);
                        par.Add(strs[3]);
                        inst = Instruction.PWMSet;
                        break;
                    }

                   


            }


            return inst;
        }

        private static void processInst(Instruction inst)
        {
            DataGram gram = new DataGram();

            switch (inst)
            {               
                case Instruction.Initialize:
                    SocketData.Server.InitialTCPServer();
                    break;
                case Instruction.End:
                    runningFlag = false;
                    break;
                case Instruction.StartI2C:                  
                    gram.Type = 2;
                    SocketData.Client[0].TCPSend(gram);
                    break;
                case Instruction.EndI2C:                  
                    gram.Type = 6;
                    SocketData.Client[0].TCPSend(gram);
                    break;
                case Instruction.PWMSet:

                    int value, num;

                    if (par[0] != "-p" && par[1] != "-d" && par[1] != "-r")
                    {
                        processInst(Instruction.None);
                        break;
                    }

                    if (!int.TryParse(par[1], out num))
                    {
                        processInst(Instruction.None);
                        break;
                    }

                    if (!int.TryParse(par[2],out value))
                    {
                        processInst(Instruction.None);
                        break;
                    }
                    
                    gram.Type = 4;

                    int index = 0;

                    if (par[1] == "-p")
                        Array.Copy(BitConverter.GetBytes((int)2), 0, gram.Data, index, sizeof(int));
                    else if (par[1] == "-d")
                        Array.Copy(BitConverter.GetBytes((int)1), 0, gram.Data, index, sizeof(int));
                    else if (par[1] == "-r")
                        Array.Copy(BitConverter.GetBytes((int)2), 0, gram.Data, index, sizeof(int));
                    index += sizeof(int);

                    Array.Copy(BitConverter.GetBytes(num), 0, gram.Data, index, sizeof(int));
                    index += sizeof(int);

                    Array.Copy(BitConverter.GetBytes(value), 0, gram.Data, index, sizeof(int));
                    index += sizeof(int);

                    SocketData.Client[0].TCPSend(gram);
                    break;
                default:
                case Instruction.None:
                    Console.WriteLine("Error string.");
                    break;
            }
        }
    }

    public enum Instruction
    {
        Stop,
        Initialize,
        None,
        End,
        StartI2C,
        EndI2C,
        PWMSet            
    }
    
}
