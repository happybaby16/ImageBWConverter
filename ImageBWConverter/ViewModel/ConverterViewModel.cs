using ImageBWConverter.ViewModel.Commands;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ImageBWConverter.ViewModel
{
    public class ConverterViewModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        string pathInputImage = string.Empty;
        public string PathInputImage
        {
            get
            {
                return pathInputImage;
            }
            set
            {
                pathInputImage = value;
            }
        }


        public LoadImageCommand LoadInputImageCommand { get; private set; }
        private void LoadImage()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                PathInputImage = openFileDialog.FileName;

                inputImageBitmap = LoadInputImage();
                PropertyChanged(this, new PropertyChangedEventArgs("InputImageBitmap"));

                inputImageSource = BitmapToBitmapImage(inputImageBitmap);
                PropertyChanged(this, new PropertyChangedEventArgs("InputImageSource"));

                outputImageBitmap = ConvertToBW();
                PropertyChanged(this, new PropertyChangedEventArgs("OutputImageBitmap"));
            }
            else
            {
                PathInputImage = string.Empty;
            }
        }

        #region Загрузка выбранного изображения
        Bitmap inputImageBitmap = null;
        public Bitmap InputImageBitmap
        {
            get
            {
                return inputImageBitmap;
            }
        }
        private Bitmap LoadInputImage()
        {
            if (PathInputImage != string.Empty) return new Bitmap(PathInputImage);
            else return null;
        }

        BitmapImage inputImageSource = null;
        public BitmapImage InputImageSource
        {
            get
            {
                return inputImageSource;
            }
        }
        #endregion

        #region Конвертация загруженной картинки
        BitmapImage outputImageBitmap = null;
        public BitmapImage OutputImageBitmap
        {
            get
            {
                return outputImageBitmap;
            }
        }

        private BitmapImage ConvertToBW()
        {
            if (inputImageBitmap != null)
            {
                Bitmap outputImage = new Bitmap(inputImageBitmap.Width, inputImageBitmap.Height);
                for (int i = 0; i < inputImageBitmap.Height; i++)
                {
                    for (int j = 0; j < inputImageBitmap.Width; j++)
                    {
                        Color pixelColor = inputImageBitmap.GetPixel(j, i);
                        byte R, G, B;
                        R = G = B = Convert.ToByte((pixelColor.R + pixelColor.G + pixelColor.B) / 3);
                        Color newPixelColor = Color.FromArgb(255, R, G, B);
                        outputImage.SetPixel(j, i, newPixelColor);
                    }
                }
                return BitmapToBitmapImage(outputImage);
            }
            else return null;
         
        }
        #endregion

        #region Сохранение полученной картинки
        public SaveImageCommand SaveCommand { get; private set; }
        private void SaveImage()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Image";
            dlg.DefaultExt = ".png";
            dlg.Filter = "PNG file (.png)|*.png";

            if (dlg.ShowDialog() == true)
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(OutputImageBitmap));
                using (var stream = new FileStream(dlg.FileName, FileMode.Create))
                {
                    encoder.Save(stream);
                }
            }

            
        }
        #endregion

        private BitmapImage BitmapToBitmapImage(Bitmap bmp)
        {
            if (bmp != null)
            {
                using (var memory = new System.IO.MemoryStream())
                {
                    bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                    memory.Position = 0;

                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    return bitmapImage;
                }
            }
            return null;
            
        }

        public ConverterViewModel()
        {
            LoadInputImageCommand = new LoadImageCommand(LoadImage);
            SaveCommand = new SaveImageCommand(SaveImage);
        }
    }
}
