using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ariphmetic
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Error("Using:\n -e fileName for enc / -d TreeName for dec");
            }
            var fileName = args[1];
            switch (args[0])
            {
                case "-e":
                {
                    var data = File.ReadAllBytes(fileName);
                    Encode(data, new FileInfo(fileName).Name + ".enc");
                    Console.WriteLine("Done, file saved as " + new FileInfo(fileName).Name + ".enc");
                }
                    break;
                case "-d":
                    Decode(new FileInfo(fileName).Name);
                    Console.WriteLine("Done");
                    break;
                default:
                    Error("Using:\n -e fileName for enc / -d Enc fileName for dec");
                    break;
            }
        }

        static void Encode(byte[] input, string fileName)
        {
            var coder = new Coder();
            
            var encodedBits = coder.Encode(input);
            var encodedBytes = coder.EncodeToByteArray(encodedBits);

            File.WriteAllBytes(fileName,encodedBytes);
        }

        static void Decode(string file)
        {
            var coder = new Coder();

            var inputData = File.ReadAllBytes(file);
            var b = coder.EncodeToBitArray(inputData);
            var result = coder.Decode(b);
        

            File.WriteAllBytes("DEC" + file.Remove(file.LastIndexOf(".")),result);


        }

        static void Error(string err)
        {
            Console.WriteLine(err);
            Console.ReadKey();
            Environment.Exit(-1);
        }
    }
}
