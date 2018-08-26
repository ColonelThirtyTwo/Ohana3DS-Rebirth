using Microsoft.Win32;
using Ohana3DS_Rebirth.Ohana;
using Ohana3DS_Rebirth.Ohana.Containers;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Ohana3DS_Rebirth.Ohana.RenderBase;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private class ItemEntry : INotifyPropertyChanged
        {
            private OContainer.FileEntry _ThisItem;
            public OContainer.FileEntry ThisItem
            {
                get => _ThisItem;
                set {
                    _ThisItem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ThisItem"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
            
            private FileIO.file _ThisFile { get; set; }
            public FileIO.file ThisFile
            {
                get => _ThisFile;
                set
                {
                    _ThisFile = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ThisFile"));
                }
            }

            private List<ItemEntry> _SubEntries;
            public List<ItemEntry> SubEntries
            {
                get => _SubEntries;
                set
                {
                    _SubEntries = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SubEntries"));
                }
            }

            public string Name { get => ThisItem?.name; }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        private OContainer RootContainer;

        public MainWindow()
        {
            InitializeComponent();
            CloseContainer();
        }

        /// <summary>
        /// Opens a container object for viewing, replacing an existing open container.
        /// </summary>
        /// <param name="container"></param>
        public void OpenContainer(OContainer container, string name="Root")
        {
            SetVieportLabel("Double click a container entry to open it");
            RootContainer = container;

            ArchiveContent.DataContext = null;
            var root_data = new ItemEntry();
            PopulateContainerEntry(container, root_data);
            ArchiveContent.DataContext = root_data;

            CloseMenuItem.IsEnabled = true;
            ArchiveContent.IsEnabled = true;
        }

        private void PopulateContainerEntry(OContainer container, ItemEntry item_entry)
        {
            item_entry.ThisFile = new FileIO.file { data = container, type = FileIO.formatType.container };
            item_entry.SubEntries = new List<ItemEntry>(container.Select(entry => new ItemEntry() { ThisItem = entry }));
        }

        /// <summary>
        /// Opens a texture in the viewport.
        /// 
        /// Does not affect the currently loaded container tree.
        /// </summary>
        /// <param name="texture">Texture to view</param>
        public void ViewTexture(OTexture texture)
        {
            Viewport.Children.Clear();
            var viewport_control = new TextureView(texture);
            Viewport.Children.Add(viewport_control);
        }

        /// <summary>
        /// Closes the container (if any) and clears the viewport.
        /// </summary>
        public void CloseContainer()
        {
            SetVieportLabel("No file opened");
            RootContainer = null;
            ArchiveContent.Items.Clear();
            CloseMenuItem.IsEnabled = false;
            ArchiveContent.IsEnabled = false;
        }

        private void SetVieportLabel(string contents)
        {
            Viewport.Children.Clear();
            var viewport_label = new Label()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Content = contents
            };
            Viewport.Children.Add(viewport_label);
        }

        private void OpenContainerEntry(OContainer.FileEntry entry, TreeViewItem item)
        {
            FileIO.file file;
            try
            {
                var stream = new MemoryStream(entry.Load());
                file = FileIO.load(stream);
            }
            catch (IOException err)
            {
                MessageBox.Show(this, "Could not load file: " + err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (ParseException err)
            {
                MessageBox.Show(this, "Could not parse file.\n" + err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (file.type == FileIO.formatType.container)
            {
                PopulateContainerEntry((OContainer)file.data, (ItemEntry)item.DataContext);
            }
            else if (file.type == FileIO.formatType.image)
            {
                ViewTexture((OTexture)file.data);
            }
            else
            {
                MessageBox.Show(this, "Could not load file: unsupported file type: "+file.type, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "All files (*.*)|*.*";
            if (dialog.ShowDialog() != true)
                return;

            FileIO.file file;
            try
            {
                file = FileIO.load(dialog.FileName);
            }
            catch (IOException err)
            {
                MessageBox.Show(this, "Could not load file: " + err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (ParseException err)
            {
                MessageBox.Show(this, "Could not parse file.\n" + err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (file.type == FileIO.formatType.container)
            {
                OpenContainer((OContainer)file.data);
            }
            else if (file.type == FileIO.formatType.image)
            {
                ViewTexture((OTexture)file.data);
            }
            else
            {
                MessageBox.Show(this, "Could not load file: unsupported file type", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CloseContainer();
        }

        private void QuitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OpenTreeViewItem(TreeViewItem sender)
        {

        }

        private void TreeViewItem_MouseDoubleClick(object osender, MouseButtonEventArgs e)
        {
            var sender = (TreeViewItem)osender;
            var str = sender.DataContext == null ? "null" : sender.DataContext.ToString();
            MessageBox.Show(this, "you double clicked on "+ str+"("+sender+")", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
