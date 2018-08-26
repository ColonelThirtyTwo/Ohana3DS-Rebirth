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
            SetViewportLabel("Double click a container entry to open it");
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
        public void SetViewportTexture(OTexture texture)
        {
            Viewport.Children.Clear();
            DataPanel.Children.Clear();

            var viewport_control = new TextureView(texture);
            Viewport.Children.Add(viewport_control);

            var details_control = new TextureDetails(texture);
            DataPanel.Children.Add(details_control);
        }

        /// <summary>
        /// Closes the container (if any) and clears the viewport.
        /// </summary>
        public void CloseContainer()
        {
            SetViewportLabel("No file opened");
            ArchiveContent.DataContext = null;
            if (RootContainer != null)
            {
                RootContainer.Dispose();
                RootContainer = null;
            }
            CloseMenuItem.IsEnabled = false;
            ArchiveContent.IsEnabled = false;
        }

        private void SetViewportLabel(string contents)
        {
            Viewport.Children.Clear();
            DataPanel.Children.Clear();
            var viewport_label = new Label()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Content = contents
            };
            Viewport.Children.Add(viewport_label);
        }

        private void OpenContainerEntry(ItemEntry entry)
        {
            FileIO.file file;
            try
            {
                var stream = new MemoryStream(entry.ThisItem.Load());
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
                entry.ThisFile = file;
                PopulateContainerEntry((OContainer)file.data, entry);
            }
            else if (file.type == FileIO.formatType.image)
            {
                SetViewportTexture((OTexture)file.data);
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
            OpenFile(dialog.FileName);
        }

        private void OpenFile(string filename)
        {
            FileIO.file file;
            try
            {
                file = FileIO.load(filename);
            }
            catch (IOException err)
            {
                MessageBox.Show(this, "Could not load file: " + err.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            else if (file.type == FileIO.formatType.image || file.type == FileIO.formatType.texture)
            {
                SetViewportTexture((OTexture)file.data);
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
        
        private void TreeViewItem_MouseDoubleClick(object osender, MouseButtonEventArgs e)
        {
            var sender = (TreeViewItem)osender;
            OpenContainerEntry((ItemEntry)sender.DataContext);
        }

        private static T FindParent<T>(DependencyObject obj) where T: DependencyObject
        {
            var parent = LogicalTreeHelper.GetParent(obj);
            if (parent == null)
                return null;
            if (parent is T parent_as_t)
                return parent_as_t;
            return FindParent<T>(parent);
        }

        private void ContainerViewMenuItem(object osender, RoutedEventArgs e)
        {
            var sender = FindParent<ContextMenu>((MenuItem)osender);
            OpenContainerEntry((ItemEntry)sender.DataContext);
        }

        private void ContainerExtractRawMenuItem(object osender, RoutedEventArgs e)
        {
            var sender = FindParent<ContextMenu>((MenuItem)osender);
            var data = (ItemEntry)sender.DataContext;

            var dialog = new SaveFileDialog();
            dialog.FileName = data.ThisItem.name;
            dialog.Filter = "All files (*.*)|*.*";
            if (dialog.ShowDialog() != true)
                return;

            try
            {
                using(var file = File.Create(dialog.FileName))
                {
                    var buffer = data.ThisItem.Load();
                    file.Write(buffer, 0, buffer.Length);
                }
            }
            catch(IOException err)
            {
                MessageBox.Show(this, "Could not save file: "+err.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length != 1)
                return;
            var file = files[0];

            CloseContainer();
            OpenFile(file);
        }

        private void Window_Drag(object sender, DragEventArgs e)
        {
            e.Handled = true;
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length != 1)
            {
                e.Effects = DragDropEffects.None;
                return;
            }
            e.Effects = DragDropEffects.Move;
        }
    }
}
