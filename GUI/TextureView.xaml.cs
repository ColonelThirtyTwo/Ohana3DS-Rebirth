using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Ohana3DS_Rebirth.Ohana.RenderBase;

namespace GUI
{
    /// <summary>
    /// Interaction logic for TextureView.xaml
    /// </summary>
    public partial class TextureView : UserControl
    {
        public OTexture Texture { get; private set; }
        public TextureView(OTexture image)
        {
            InitializeComponent();
            Texture = image;

            BitmapImage backing_image;
            using(var memory = new MemoryStream())
            {
                image.texture.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                backing_image = new BitmapImage();
                backing_image.BeginInit();
                backing_image.StreamSource = memory;
                backing_image.CacheOption = BitmapCacheOption.OnLoad;
                backing_image.EndInit();
            }

            Image.BeginInit();
            Image.Source = backing_image;
            Image.EndInit();

            WidthLabel.Content = image.texture.Width + "px";
            HeightLabel.Content = image.texture.Height + "px";
        }
    }
}
