using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace SignatureTestApp
{
    /// <summary>
    /// Reads file blocks as streams using memory mapped file
    /// </summary>
    class FileBlockReader : IDisposable
    {
        private readonly long _blockSize;
        private readonly long _length;
        private readonly MemoryMappedFile _mappedFile;

        public FileBlockReader(string filePath, long blockSize)
        {
            if(blockSize <=0)
            {
                throw new ArgumentException("blockSize cannot be less or equals zero", nameof(blockSize));
            }

            _length = new FileInfo(filePath).Length;
            _mappedFile = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open);
            _blockSize = blockSize;
        }

        public void Dispose()
        {
            if (_mappedFile != null)
            {
                _mappedFile.Dispose();
            }
        }

        public IEnumerable<Stream> GetBlocks()
        {
            long offset = 0;
            while (offset < _length)
            {
                long sizeToRead = Math.Min(_blockSize, _length - offset);
                yield return _mappedFile.CreateViewStream(offset, sizeToRead);
                offset += _blockSize;
            }
        }
    }
}
