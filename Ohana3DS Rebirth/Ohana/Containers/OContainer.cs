using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Ohana3DS_Rebirth.Ohana.Containers
{
    public class OContainer: IEnumerable<OContainer.FileEntry>, IDisposable
    {
        public class FileEntry
        {
            public FileEntry(OContainer parent, string name, bool compressed, byte[] data)
            {
                Container = parent;
                this.data = data;
                this.name = name;
                loadFromDisk = false;
                fileOffset = 0;
                fileLength = 0;
                doDecompression = compressed;
            }

            public FileEntry(OContainer parent, string name, bool compressed, uint fileOffset, uint fileLength)
            {
                Container = parent;
                data = null;
                this.name = name;
                loadFromDisk = true;
                this.fileOffset = fileOffset;
                this.fileLength = fileLength;
                doDecompression = compressed;
            }

            public string name { get; private set; }
            public byte[] data { get; private set; }

            public bool loadFromDisk { get; private set; }
            public uint fileOffset { get; private set; }
            public uint fileLength { get; private set; }
            public bool doDecompression { get; private set; }

            public OContainer Container { get; private set; }

            /// <summary>
            /// Reads the entry contents, either by returning `data` or by reading it from the backing Stream.
            /// </summary>
            /// <returns></returns>
            public byte[] Load()
            {
                if (data != null)
                    return data;
                var buffer = new byte[fileLength];
                Container.data.Seek(fileOffset, SeekOrigin.Begin);
                Container.data.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        public Stream data { get; private set; }
        private List<FileEntry> content;

        public OContainer(Stream backing_stream=null)
        {
            data = backing_stream;
            content = new List<FileEntry>();
        }

        /// <summary>
        /// Adds a file entry to the container.
        /// </summary>
        /// The entry must have been created with this container object. Additionally, if `FileEntry.loadFromDisk` is set,
        /// this container must have been created with a backing stream.
        /// <param name="entry">Entry to add</param>
        public void Add(FileEntry entry)
        {
            if (entry.Container != this)
                throw new ArgumentException("Tried to add a FileEntry not belonging to this container");
            if (entry.loadFromDisk && data == null)
                throw new ArgumentException("Tried to add a FileEntry with loadFromDisk set to a container that does not have a backing stream");
            content.Add(entry);
        }

        /// <summary>
        /// Returns the entry at the specified index
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public FileEntry this[int i] { get { return content[i]; } }

        /// <summary>
        /// Returns the list of all entries (read only).
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<FileEntry> GetList()
        {
            return new ReadOnlyCollection<FileEntry>(content);
        }

        public IEnumerator<FileEntry> GetEnumerator()
        {
            return ((IEnumerable<FileEntry>)content).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<FileEntry>)content).GetEnumerator();
        }

        public void Dispose()
        {
            if (data != null)
                ((IDisposable)data).Dispose();
        }
    }
}
