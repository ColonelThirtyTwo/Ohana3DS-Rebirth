using System.Collections.Generic;
using System.IO;

namespace Ohana3DS_Rebirth.Ohana.Containers
{
    public class OContainer
    {
        public struct fileEntry
        {
            public string name;
            public byte[] data;

            public bool loadFromDisk;
            public uint fileOffset;
            public uint fileLength;
            public bool doDecompression;
        }

        public Stream data;
        public List<fileEntry> content;

        public OContainer()
        {
            content = new List<fileEntry>();
        }

        /// <summary>
        /// Loads the contents of a fileEntry, if they need to be loaded from disk.
        /// </summary>
        /// <param name="entry">Entry to load. Assumed to be from this container</param>
        /// <returns>The loaded data</returns>
        public byte[] Load(fileEntry entry)
        {
            if (entry.data != null)
                return entry.data;
            var buffer = new byte[entry.fileLength];
            data.Seek(entry.fileOffset, SeekOrigin.Begin);
            data.Read(buffer, 0, buffer.Length);
            return buffer;
        }
    }
}
