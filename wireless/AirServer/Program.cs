using System;
using System.Collections.Generic;
using System.Text;

using AirServer.MySocket;

namespace AirServer.Main
{
    class Program
    {
        static bool runningFlag = true;

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

            switch(str)
            {              
                case "ini":
                case "initialize":
                case "initial":
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
        EndI2C
    }
    
}
