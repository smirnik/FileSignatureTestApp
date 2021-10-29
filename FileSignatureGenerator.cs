using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;

namespace SignatureTestApp
{
    /// <summary>
    /// Generates file signature
    /// </summary>
    public class FileSignatureGenerator
    {
        private readonly string _filePath;
        private readonly long _blockSize;
        private readonly ConcurrentDictionary<long, byte[]> _blockHashesBuffer = new();
        private AutoResetEvent _hashCalculatedResetEvent = new(false);
        private bool _isHashCalculationFinished;

        public FileSignatureGenerator(string filePath, long blockSize)
        {
            _filePath = filePath;
            _blockSize = blockSize;
        }

        public IEnumerable<FileBlockHash> GetSignature()
        {
            int nextBlockIndex = 0;
            CalculateHashesAsync();
            
            //Return hashes in order
            do
            {
                while (_blockHashesBuffer.TryRemove(nextBlockIndex, out var hash))
                {
                    yield return new FileBlockHash(nextBlockIndex, hash);
                    nextBlockIndex++;
                }

                _hashCalculatedResetEvent.WaitOne();
            } while (!_isHashCalculationFinished);
        }

        private void CalculateHashesAsync()
        {
            Thread hashCalcThread = new Thread(() =>
            {
                using FileBlockReader reader = new(_filePath, _blockSize);
                using (ThreadQueue worker = new(Environment.ProcessorCount))
                {
                    int count = 0;
                    foreach (var block in reader.GetBlocks())
                    {
                        var index = count;
                        worker.Enqueue(() =>
                        {
                            using var sha256 = SHA256.Create();
                            _blockHashesBuffer.TryAdd(index, sha256.ComputeHash(block));
                            _hashCalculatedResetEvent.Set();
                        });
                        count++;
                    }
                }
                _isHashCalculationFinished = true;
                _hashCalculatedResetEvent.Set();
            });

            hashCalcThread.Start();
        }
    }
}
