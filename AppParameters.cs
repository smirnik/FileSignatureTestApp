using System;

namespace SignatureTestApp
{
    /// <summary>
    /// Applicatio parameters
    /// </summary>
    class AppParameters
    {
        const long BlockSizeDefault = 4096;

        public long BlockSize { get; private set; }

        public string FileName { get; private set; }

        private AppParameters(string fileName, long blockSize)
        {
            BlockSize = blockSize;
            FileName = fileName;
        }

        public static AppParameters ParseArgs(string[] args)
        {
            if (args.Length <= 0)
            {
                throw new ArgumentException("At least one parameter should be passed", nameof(args));
            }

            string fileName = args[0];
            long blockSize = BlockSizeDefault;

            if (args.Length > 1)
            {
                if (!long.TryParse(args[1], out blockSize))
                {
                    blockSize = BlockSizeDefault;
                    Console.WriteLine($"Cannot parse block size parameter. Default value of ${BlockSizeDefault} will be used");
                }
            }


            return new AppParameters(fileName, blockSize);
        }

        public static void DisplayHelp()
        {
            Console.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} file_name [block_size]");
        }
    }
}
