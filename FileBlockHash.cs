namespace SignatureTestApp
{
    public class FileBlockHash
    {
        public int BlockIndex { get; private set; }
    
        public byte[] Hash { get; private set; }

        public FileBlockHash(int index, byte[] hash)
        {
            BlockIndex = index;
            Hash = hash;
        }
    }
}
