using System;
using System.IO;
using System.Text;
using System.Threading;

namespace JacoChatConfigGenerator
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the JacoChat Server Configuration Generator.\nIf you mistakenly opened this, please type ctrl-c now.");
            Thread.Sleep(1000);

            Console.Write("Please enter the host ip for your server.\n" +
                "If you only want to test on your computer enter 127.0.0.1\n" +
                "If you wish to test this on a LAN enter the computer's LAN address\n" +
                "If this is running on a VPS type the public IP Address of it.\n\n" +
                              "<IP> ");
            string ip = Console.ReadLine();
            Console.Clear();


            Console.Write("Please enter the port number you wish to host on.\n" +
                "Note: If you are runnning on a VPS you may need to add an IPTables rule.\n\n" +
                "<PORT>");
            int port = Convert.ToInt32(Console.ReadLine());
            Console.Clear();

            Console.Write("Please enter the method you wish to have debugger output sent.\n" +
                "The options are stdout to print to the console or a filepath.\n\n" +
                "<OUTPUT_MODE>");
            string outputMode = Console.ReadLine();
            Console.Clear();

            Console.Write("Enter the path including the file name where the config should output to.\n\n" +
                "<OUTPUT_PATH>");
            string outputPath = Console.ReadLine();
            Console.Clear();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# Created with JacoChatConfigGenerator.exe. https://github.com/JacobMisirian/JacoChat");
            sb.AppendLine("# This is the host ip for the server.");
            sb.AppendLine("HostName " + ip);
            sb.AppendLine("# This is the port for the server.");
            sb.AppendLine("Port " + port);
            sb.AppendLine("# This is the output mode for debugger output.");
            sb.AppendLine("OutputMode " + outputMode);

            File.WriteAllText(outputPath, sb.ToString());
        }
    }
}
