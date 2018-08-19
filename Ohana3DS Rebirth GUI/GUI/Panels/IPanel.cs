using System;
using System.IO;

namespace Ohana3DS_Rebirth.GUI
{
    public class OpenSubResourceArgs: EventArgs
    {
        public string Filename { get; private set; }
        public Stream Stream { get; private set; }
        public OpenSubResourceArgs(string filename, Stream stream)
        {
            this.Filename = filename;
            this.Stream = stream;
        }
    }

    interface IPanel
    {
        void finalize();
        void launch(object data);
        event EventHandler<OpenSubResourceArgs> OpenSubResource;
    }
}
