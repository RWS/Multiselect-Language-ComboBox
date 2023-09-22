using Rws.MultiSelectLanguageComboBox.Models;
using Sdl.MultiSelectComboBox.API;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rws.MultiSelectLanguageComboBox.Services
{
    public class DefaultLanguageInfoService : ILanguageInfoService
    {
        private readonly LanguageItemGroup _allGroup = new LanguageItemGroup(int.MaxValue, "All");
        private readonly ResourceDictionary _resources = new ResourceDictionary { Source = new Uri("pack://application:,,,/Rws.MultiSelectLanguageComboBox;component/Resources/Images.xaml", UriKind.Absolute) };

        public virtual string GetDisplayName(string language)
        {
            return CultureInfo.GetCultureInfo(language).EnglishName;
        }

        public virtual IItemGroup GetItemGroup(string language)
        {
            return _allGroup;
        }

        public virtual ImageSource GetImage(string language)
        {
            language = language.ToLowerInvariant();
            if (_resources.Contains(language)) 
            { 
                return _resources[language] as DrawingImage;
            }

            double width = 24, height = 18;
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                FormattedText label = new FormattedText(
                    language.Substring(0, 2).ToUpperInvariant(),
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(Fonts.SystemTypefaces.First().FaceNames.First().Value),
                    11,
                    Brushes.Black, 
                    1.5);
                Point textPosition = new Point((width - label.Width) / 2, (height - label.Height) / 2);
                drawingContext.DrawRectangle(Brushes.LightGray, null, new Rect(0, 0, width, height));
                drawingContext.DrawText(label, textPosition);
            }

            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)width * 2, (int)height * 2 , 96 * 2, 96 * 2, PixelFormats.Pbgra32);
            bitmap.Render(drawingVisual);

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = BitmapSourceToStream(bitmap);
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

        private static Stream BitmapSourceToStream(BitmapSource source)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            MemoryStream stream = new MemoryStream();
            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(stream);
            return stream;
        }
    }
}
