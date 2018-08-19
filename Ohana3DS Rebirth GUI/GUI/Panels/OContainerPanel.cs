﻿using System.IO;
using System.Windows.Forms;

using Ohana3DS_Rebirth.Ohana.Containers;
using Ohana3DS_Rebirth.Ohana.Compressions;
using System;

namespace Ohana3DS_Rebirth.GUI
{
    public partial class OContainerPanel : UserControl, IPanel
    {
        private OContainer container;
        public event EventHandler<OpenSubResourceArgs> OpenSubResource;

        public OContainerPanel()
        {
            InitializeComponent();
        }

        public void finalize()
        {
            if (container.data != null)
            {
                container.data.Close();
                container.data = null;
            }
        }

        public void launch(object data)
        {
            container = (OContainer)data;
            FileList.addColumn(new OList.columnHeader(384, "Name"));
            FileList.addColumn(new OList.columnHeader(128, "Size"));
            foreach (OContainer.fileEntry file in container.content)
            {
                OList.listItemGroup item = new OList.listItemGroup();
                item.columns.Add(new OList.listItem(file.name));
                uint length = file.loadFromDisk ? file.fileLength : (uint)file.data.Length;
                item.columns.Add(new OList.listItem(getLength(length)));
                FileList.addItem(item, false);
            }
            FileList.recalcScroll();
            FileList.Refresh();
        }

        private byte[] Read(OContainer.fileEntry file)
        {
            byte[] buffer;

            if (file.loadFromDisk)
            {
                buffer = new byte[file.fileLength];
                container.data.Seek(file.fileOffset, SeekOrigin.Begin);
                container.data.Read(buffer, 0, buffer.Length);
            }
            else
                buffer = file.data;

            if (file.doDecompression) buffer = LZSS_Ninty.decompress(buffer);
            return buffer;
        }

        private void BtnExportAll_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog browserDlg = new FolderBrowserDialog())
            {
                browserDlg.Description = "Export all files";
                if (browserDlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (OContainer.fileEntry file in container.content)
                    {
                        string fileName = Path.Combine(browserDlg.SelectedPath, file.name);
                        string dir = Path.GetDirectoryName(fileName);
                        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                        
                        File.WriteAllBytes(fileName, Read(file));
                    }
                }
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (FileList.SelectedIndex == -1) return;
            using (SaveFileDialog saveDlg = new SaveFileDialog())
            {
                saveDlg.Title = "Export file";
                saveDlg.FileName = container.content[FileList.SelectedIndex].name;
                saveDlg.Filter = "All files|*.*";
                if (saveDlg.ShowDialog() == DialogResult.OK)
                {
                    OContainer.fileEntry file = container.content[FileList.SelectedIndex];
                    File.WriteAllBytes(saveDlg.FileName, Read(file));
                }
            }
        }

        /// <summary>
        /// Opens the selected resource
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open(object sender, EventArgs e)
        {
            if (FileList.SelectedIndex == -1) return;
            OContainer.fileEntry file = container.content[FileList.SelectedIndex];
            byte[] data = Read(file);
            Stream stream = new MemoryStream(data);

            OpenSubResource(this, new OpenSubResourceArgs(file.name, stream));
        }

        private static readonly string[] lengthUnits = { "Bytes", "KB", "MB", "GB", "TB" };

        private string getLength(uint length)
        {
            int i = 0;
            while (length > 0x400 && i < lengthUnits.Length)
            {
                length /= 0x400;
                i++;
            }

            return length.ToString() + " " + lengthUnits[i];
        }
    }
}
