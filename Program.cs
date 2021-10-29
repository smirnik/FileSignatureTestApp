using System;

namespace SignatureTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                AppParameters.DisplayHelp();
                return;
            }

            var parameters = AppParameters.ParseArgs(args);

            AppDomain.CurrentDomain.UnhandledException += UnhandledException; 

            var signatureGenerator = new FileSignatureGenerator(parameters.FileName, parameters.BlockSize);
            foreach (var blockHash in signatureGenerator.GetSignature())
            {
                Console.WriteLine($"{blockHash.BlockIndex}: {BitConverter.ToString(blockHash.Hash)}");
            }
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception ex = (Exception) args.ExceptionObject;
            Console.WriteLine($"Exception occured during execution: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Environment.Exit(1);
        }
    }
}
