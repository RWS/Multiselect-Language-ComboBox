using Rws.MultiselectLanguageComboBox.Models;
using Sdl.MultiSelectComboBox.API;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rws.MultiselectLanguageComboBox.Services
{
    public class DefaultLanguageInfoService : ILanguageInfoService
    {
        private LanguageItemGroup _allGroup = new LanguageItemGroup(int.MaxValue, "All");

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
            try
            {
                var bitmapImage = new BitmapImage(new Uri($"pack://application:,,,/Rws.MultiselectLanguageComboBox;component/Images/{language}.ico"));
                bitmapImage.Freeze();
                return bitmapImage;
            }
            catch (IOException)
            {
                // Create a DrawingVisual
                DrawingVisual drawingVisual = new DrawingVisual();

                // Define the size of the rectangle
                double width = 24; // Width of the rectangle
                double height = 24; // Height of the rectangle

                // Create a drawing context
                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                {
                    // Create a formatted text object
                    FormattedText label = new FormattedText(
                        language.Substring(0, 2).ToUpperInvariant(),
                        CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface(Fonts.SystemTypefaces.First().FaceNames.First().Value),
                        11,
                        Brushes.Black, 
                        1.5);

                    // Calculate the position to center the text in the rectangle
                    Point textPosition = new Point((width - label.Width) / 2, (height - label.Height) / 2);

                    // Create a rectangle and draw the text
                    drawingContext.DrawRectangle(Brushes.White, new Pen(Brushes.Gray, 1), new Rect(0.5, 3.5, width-1.5, height-7));
                    drawingContext.DrawText(label, textPosition);
                }

                // Render the DrawingVisual to a BitmapImage
                RenderTargetBitmap bitmap = new RenderTargetBitmap((int)width * 2, (int)height * 2 , 96 * 2, 96 * 2, PixelFormats.Pbgra32);
                bitmap.Render(drawingVisual);

                // Create a BitmapImage from the RenderTargetBitmap
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = BitmapSourceToStream(bitmap);
                bitmapImage.EndInit();

                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        // Helper function to convert BitmapSource to a Stream
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
