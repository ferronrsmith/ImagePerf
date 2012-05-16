/*
Copyright 2012 Ferron Smith

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
s
*/ 


using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;

namespace ImagePerf
{
    #region enums
    public enum Direction
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public enum ThumbnailMethod
    {
        Fit,
        Pad,
        Crop
    }
    #endregion

    public class ImageShrink : IDisposable
    {
        #region Private variables

        private Image _image;
        private Graphics _g;
        private Font _font;
        private Color _color;
        private Color _backgroundColor;
        private TextRenderingHint _textrenderinghint = TextRenderingHint.ClearTypeGridFit;

        #endregion


        #region Constructors and destructors

        /// <summary>
        /// Create a ImageShrink from a System.Drawing.Image.
        /// </summary>
        /// <param name="image"></param>
        public ImageShrink(Image image)
        {
            _image = image;
            _g = Graphics.FromImage(_image);
        }

        /// <summary>
        /// Create a ImageShrink with a defined width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public ImageShrink(int width, int height)
        {
            CreateImage(width, height);
        }

        /// <summary>
        /// Create a ImageShrink with a defined width, height and backgroundcolor.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="bgcolor"></param>
        public ImageShrink(int width, int height, Color bgcolor)
        {
            _backgroundColor = bgcolor;
            CreateImage(width, height);
            Clear(bgcolor);
        }

        /// <summary>
        /// Create a ImageShrink by loading an image from either local disk or web.
        /// </summary>
        /// <example>
        /// Open a local image:
        ///     ImageShrink image = new ImageShrink("c:\\images\\test.jpg");
        /// 
        /// Load an image from the web:
        ///     ImageShrink image = new ImageShrink("http://yourdomain.com/test.jpg");
        /// </example>
        /// <param name="filepath">Local filepath or internet URL</param>
        public ImageShrink(string filepath)
        {
            string prefix = filepath.Length > 8 ? filepath.Substring(0, 8).ToLower() : "";

            if (prefix.StartsWith("http://") || prefix.StartsWith("https://"))
            {
                // Load from URL
                LoadImageFromUrl(filepath);
            }
            else
            {
                // Load from local disk
                LoadImage(filepath);
            }
        }

        /// <summary>
        /// Create a ImageShrink from a stream.
        /// </summary>
        /// <param name="stream"></param>
        public ImageShrink(Stream stream)
        {
            LoadImage(stream);
        }

        public void Destroy()
        {
            if (_g != null)
                _g.Dispose();
            if (_image != null)
                _image.Dispose();
        }


        #endregion


        #region Public properties

        public Image Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
            }
        }


        public int Width
        {
            get
            {
                return _image.Width;
            }
        }

        public int Height
        {
            get
            {
                return _image.Height;
            }
        }


        public Color BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }
            set
            {
                _backgroundColor = value;
            }
        }


        public Size Size
        {
            get
            {
                return _image.Size;
            }
        }


        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }


        public TextRenderingHint TextRenderingHint
        {
            get
            {
                return _textrenderinghint;
            }
            set
            {
                _textrenderinghint = value;
            }
        }

        /// <summary>
        /// Check if the current image has an indexed palette.
        /// </summary>
        public bool IndexedPalette
        {
            get
            {
                switch (_image.PixelFormat)
                {
                    case PixelFormat.Undefined:
                    case PixelFormat.Format1bppIndexed:
                    case PixelFormat.Format4bppIndexed:
                    case PixelFormat.Format8bppIndexed:
                    case PixelFormat.Format16bppGrayScale:
                    case PixelFormat.Format16bppArgb1555:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public bool IsPortrait
        {
            get { return Width < Height; }
        }

        public bool IsLandscape
        {
            get { return Width > Height; }
        }

        public bool IsSquare
        {
            get { return Width == Height; }
        }


        #endregion


        #region Common image functions

        /// <summary>
        /// Create an exact copy of this Kaliko.ImageLibrary.ImageShrink
        /// </summary>
        /// <returns></returns>
        public ImageShrink Clone()
        {
            // Create new image from the old one
            ImageShrink newImage = new ImageShrink(_image);

            // Set all private variables to same as the current instance
            newImage._color = _color;
            newImage._font = _font;
            newImage._backgroundColor = _backgroundColor;
            newImage._textrenderinghint = _textrenderinghint;

            return newImage;
        }

        /// <summary>
        /// Instanciate an empty image of the requested resolution.
        /// </summary>
        /// <param name="width">Width of the new image</param>
        /// <param name="height">Height of the new image</param>
        private void CreateImage(int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            _image = bitmap;
            _g = Graphics.FromImage(_image);
        }

        #endregion


        #region Functions for text

        /// <summary>
        /// Load a font for further use.
        /// </summary>
        /// <param name="fileName">Path to font file</param>
        /// <param name="size">Font size</param>
        /// <param name="fontStyle">Font style</param>
        /// <example>
        /// ImageShrink image = new ImageShrink(200, 200);
        /// image.SetFont("c:\\fontpath\\arial.ttf", 12f, FontStyle.Regular);
        /// image.WriteText("Hello world!", 0, 10);
        /// </example>
        public void SetFont(string fileName, float size, FontStyle fontStyle)
        {
            PrivateFontCollection pf = new PrivateFontCollection();
            pf.AddFontFile(fileName);
            _font = new Font(pf.Families[0], size, fontStyle);
        }


        public void WriteText(string txt, int x, int y)
        {
            _g.TextRenderingHint = _textrenderinghint;
            _g.DrawString(txt, _font, new SolidBrush(_color), new Point(x, y));
        }

        #endregion


        #region Functions for loading images (from file, stream or web)

        /// <summary>
        /// Load an image from local disk
        /// </summary>
        /// <param name="fileName">File path</param>
        public void LoadImage(string fileName)
        {
            _image = Image.FromFile(fileName);

            MakeImageNonIndexed();

            _g = Graphics.FromImage(_image);
        }

        /// <summary>
        /// Load an image from a stream object (MemoryStream, Stream etc)
        /// </summary>
        /// <param name="stream">Pointer to stream</param>
        public void LoadImage(Stream stream)
        {
            _image = Image.FromStream(stream);

            MakeImageNonIndexed();

            _g = Graphics.FromImage(_image);
        }

        /// <summary>
        /// Load an image from an URL
        /// </summary>
        /// <param name="url"></param>
        public void LoadImageFromUrl(string url)
        {
            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;

            Stream source = request.GetResponse().GetResponseStream();
            MemoryStream ms = new MemoryStream();

            byte[] data = new byte[256];
            int c = source.Read(data, 0, data.Length);

            while (c > 0)
            {
                ms.Write(data, 0, c);
                c = source.Read(data, 0, data.Length);
            }

            source.Close();
            ms.Position = 0;

            LoadImage(ms);

            ms.Close();
        }

        /// <summary>
        /// Check if image has an indexed palette and if so convert to truecolor
        /// </summary>
        private void MakeImageNonIndexed()
        {
            if (IndexedPalette)
            {
                _image = new Bitmap(new Bitmap(_image));
            }
        }

        #endregion


        #region Primitive drawing functions like clear, fill etc

        public void Clear(Color color)
        {
            _g.Clear(color);
        }


        public void GradientFill(Color colorFrom, Color colorTo)
        {
            GradientFill(new Point(0, 0), new Point(0, _image.Height), colorFrom, colorTo);
        }


        public void GradientFill(Point pointFrom, Point pointTo, Color colorFrom, Color colorTo)
        {
            Brush brush = new LinearGradientBrush(pointFrom, pointTo, colorFrom, colorTo);
            _g.FillRectangle(brush, 0, 0, _image.Width, _image.Height);
        }

        #endregion


        #region Functions for thumbnail creation

        public ImageShrink GetThumbnailImage(int width, int height)
        {
            return GetThumbnailImage(width, height, ThumbnailMethod.Crop);
        }


        public ImageShrink GetThumbnailImage(int width, int height, ThumbnailMethod method)
        {
            ImageShrink image;
            double imageRatio = _image.Width / _image.Height;
            double thumbRatio = width / height;

            if (method == ThumbnailMethod.Crop)
            {
                int imgWidth = width;
                int imgHeight = height;

                if (imageRatio < thumbRatio)
                {
                    imgHeight = (_image.Height * width) / _image.Width;
                }
                else
                {
                    imgWidth = (_image.Width * height) / _image.Height;
                }

                image = new ImageShrink(width, height);
                DrawScaledImage(image._image, _image, (width - imgWidth) / 2, (height - imgHeight) / 2, imgWidth, imgHeight);
                //g.DrawImage(_image, (width - imgWidth) / 2, (height - imgHeight) / 2, imgWidth, imgHeight);
            }
            else if (method == ThumbnailMethod.Pad)
            {
                // Rewritten to fix issue #1. Thanks to Cosmin!
                float hRatio = _image.Height / (float)height;
                float wRatio = _image.Width / (float)width;
                float newRatio = hRatio > wRatio ? hRatio : wRatio;
                int imgHeight = (int)(_image.Height / newRatio);
                int imgWidth = (int)(_image.Width / newRatio);

                image = new ImageShrink(width, height, _backgroundColor);
                DrawScaledImage(image._image, _image, (width - imgWidth) / 2, (height - imgHeight) / 2, imgWidth, imgHeight);
            }
            else
            { // ThumbnailMethod.Fit
                if (imageRatio > thumbRatio)
                {
                    width = (_image.Width * height) / _image.Height;
                }
                else
                {
                    height = (_image.Height * width) / _image.Width;
                }

                image = new ImageShrink(width, height);
                DrawScaledImage(image._image, _image, 0, 0, width, height);
            }

            return image;
        }

        private static void DrawScaledImage(Image destImage, Image sourceImage, int x, int y, int width, int height)
        {
            using (Graphics g = Graphics.FromImage(destImage))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                using (ImageAttributes wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(sourceImage, new Rectangle(x, y, width, height), 0, 0, sourceImage.Width, sourceImage.Height, GraphicsUnit.Pixel, wrapMode);
                    //g.DrawImage(sourceImage, x, y, width, height);
                }
            }
        }

        #endregion


        #region Functions for blitting

        public void BlitImage(string fileName)
        {
            BlitImage(fileName, 0, 0);
        }


        public void BlitImage(string fileName, int x, int y)
        {
            Image mark = Image.FromFile(fileName);
            BlitImage(mark, x, y);
            mark.Dispose();
        }


        public void BlitImage(ImageShrink image)
        {
            BlitImage((Image) image._image, 0, 0);
        }


        public void BlitImage(ImageShrink image, int x, int y)
        {
            BlitImage((Image) image._image, x, y);
        }


        public void BlitImage(Image image)
        {
            BlitImage(image, 0, 0);
        }


        public void BlitImage(Image image, int x, int y)
        {
            _g.DrawImageUnscaled(image, x, y);
        }



        public void BlitFill(string fileName)
        {
            Image mark = Image.FromFile(fileName);
            BlitFill(mark);
            mark.Dispose();
        }


        public void BlitFill(ImageShrink image)
        {
            BlitFill((Image) image._image);
        }


        public void BlitFill(Image image)
        {
            int width = image.Width;
            int height = image.Height;
            int columns = (int)Math.Ceiling((float)_image.Width / width);
            int rows = (int)Math.Ceiling((float)_image.Width / width);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    _g.DrawImageUnscaled(image, x * width, y * height);
                }
            }
        }

        #endregion


        #region Functions for image saving and streaming

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();

            for (int j = 0, l = encoders.Length; j < l; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                {
                    return encoders[j];
                }
            }
            return null;
        }

        public void SaveJpg(Stream stream, long quality)
        {
            EncoderParameters encparam = new EncoderParameters(1);
            encparam.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            ImageCodecInfo ic = GetEncoderInfo("image/jpeg");
            _image.Save(stream, ic, encparam);
        }


        public void SaveJpg(string fileName, long quality)
        {
            EncoderParameters encparam = new EncoderParameters(1);
            encparam.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            ImageCodecInfo ic = GetEncoderInfo("image/jpeg");
            _image.Save(fileName, ic, encparam);
        }


        public void SavePng(Stream stream, long quality)
        {
            EncoderParameters encparam = new EncoderParameters(1);
            encparam.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            ImageCodecInfo ic = GetEncoderInfo("image/png");
            _image.Save(stream, ic, encparam);
        }


        public void SavePng(string fileName, long quality)
        {
            EncoderParameters encparam = new EncoderParameters(1);
            encparam.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            ImageCodecInfo ic = GetEncoderInfo("image/png");
            _image.Save(fileName, ic, encparam);
        }


        public void SaveGif(Stream stream)
        {
            _image.Save(stream, ImageFormat.Gif);
        }


        public void SaveGif(string fileName)
        {
            _image.Save(fileName, ImageFormat.Gif);
        }


        public void SaveBmp(Stream stream)
        {
            _image.Save(stream, ImageFormat.Bmp);
        }


        public void SaveBmp(string fileName)
        {
            _image.Save(fileName, ImageFormat.Bmp);
        }


        public void SaveBmp(Stream stream, ImageFormat format)
        {
            _image.Save(stream, format);
        }


        public void SaveImage(string fileName, ImageFormat format)
        {
            _image.Save(fileName, format);
        }


        #endregion


        #region Functions for filters and bitmap manipulation

        private byte[] _byteArray;
        private bool _disposed;

        /// <summary>
        /// ByteArray matching PixelFormat.Format32bppArgb (bgrA in real life)
        /// </summary>
        public byte[] ByteArray
        {
            get
            {
                if (_byteArray == null)
                {
                    BitmapData data = ((Bitmap)_image).LockBits(new Rectangle(0, 0, _image.Width, _image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    int length = _image.Width * _image.Height * 4;
                    _byteArray = new byte[length];

                    if (data.Stride == _image.Width * 4)
                    {
                        Marshal.Copy(data.Scan0, _byteArray, 0, length);
                    }
                    else
                    {
                        for (int i = 0, l = _image.Height; i < l; i++)
                        {
                            IntPtr p = new IntPtr(data.Scan0.ToInt32() + data.Stride * i);
                            Marshal.Copy(p, _byteArray, i * _image.Width * 4, _image.Width * 4);
                        }
                    }

                    ((Bitmap)_image).UnlockBits(data);
                }
                return _byteArray;
            }
            set
            {
                BitmapData data = ((Bitmap)_image).LockBits(new Rectangle(0, 0, _image.Width, _image.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                if (data.Stride == _image.Width * 4)
                {
                    Marshal.Copy(value, 0, data.Scan0, value.Length);
                }
                else
                {
                    for (int i = 0, l = _image.Height; i < l; i++)
                    {
                        IntPtr p = new IntPtr(data.Scan0.ToInt32() + data.Stride * i);
                        Marshal.Copy(value, i * _image.Width * 4, p, _image.Width * 4);
                    }
                }

                ((Bitmap)_image).UnlockBits(data);
            }
        }

        /// <summary>
        /// Length of the byte array
        /// </summary>
        /// <returns></returns>
        public long ByteSize()
        {
            return this.ByteArray.Length;
        }

        /// <summary>
        /// Compare two images to see which image as a smaller byte array site
        /// </summary>
        /// <param name="image">Image to be Compared</param>
        /// <returns>
        /// returns 0 - images have equal size
        /// returns 1 - current image greater than image being compared to
        /// returns -1 - current images is less than image beging compared to (byte size comparison) 
        /// </returns>
        public int CompareTo(ImageShrink image)
        {
            //pre-cache image size
            long curImage = this.ByteSize();
            long oImage = image.ByteSize();

            if (curImage == oImage)
            {
                return 0;
            }
            else if (curImage < oImage)
            {
                return 1;
            }
            else return -1;
        }

        #endregion


        #region Functions for disposal i.e GC

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_font != null)
                    {
                        _font.Dispose();
                    }
                    if (_g != null)
                    {
                        _g.Dispose();
                    }
                    if (_image != null)
                    {
                        _image.Dispose();
                    }
                }

                _disposed = true;
            }
        }

        ~ImageShrink()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion


        #region Functions for image compression
        /// <summary>
        /// Retrieve the image format for a given extension
        /// </summary>
        /// <param name="filepath">file location path</param>
        /// <returns></returns>
        public static ImageFormat GetImageFormat(string filepath)
        {
            switch (Path.GetExtension(filepath.ToLower()))
            {
                case ".jpg":
                case ".jpeg":
                    return ImageFormat.Jpeg;
                case ".bmp":
                    return ImageFormat.Bmp;
                case ".png":
                    return ImageFormat.Png;
                case ".gif":
                    return ImageFormat.Gif;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Shrink image stream
        /// </summary>
        /// <param name="path">image full path</param>
        /// <param name="stream">input stream for image</param>
        /// <returns></returns>
        public static int ShrinkImage(string path, Stream stream)
        {
            //default WxH for poster area
            const int width = 293, height = 454;
            return ShrinkImage(path, stream, height, width);
        }

        /// <summary>
        /// Shrink image stream and save to the given path
        /// </summary>
        /// <param name="path">image full path</param>
        /// <param name="stream">input stream for image</param>
        /// <param name="thumbHeight">max height</param>
        /// <param name="thumbWidth">max width</param>
        /// <returns></returns>
        public static int ShrinkImage(string path, Stream stream, int thumbHeight, int thumbWidth)
        {

            try
            {
                ImageFormat fileFormat = GetImageFormat(path);
                if (fileFormat == null)
                {
                    // image format not supported or the file may not be an image
                    return -1;
                }

                using (ImageShrink image = new ImageShrink(stream))
                {
                    using (ImageShrink thumb = image.GetThumbnailImage(thumbWidth, thumbHeight, ThumbnailMethod.Fit))
                    {
                        //compare to see which img is smaller
                        int result = image.CompareTo(thumb);

                        if (result < 0)
                        {
                            thumb.SaveImage(path, fileFormat);
                            return 0;
                        }

                        return -1;

                        // no point resaving an already saved image
                    }
                }
            }
            catch
            {
                return -1;
            }
        }
        #endregion
    }
}

