using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using CsvHelper;

namespace ImagePerf
{
    public class Utils
    {
        static readonly string[] Pics = new[] { "jpg", "jpeg", "png", "gif", "bmp" };
        private const string DefaultBytleFile = "ImageByte.csv", DefaultImgReport = "Image.csv";
        private const int PWidth = 293;
        private const int PHeight = 454;

        /// <summary>
        /// Resize and compress directory images moving the result to another location
        /// </summary>
        /// <param name="src">source folder</param>
        /// <param name="dest">destination folder</param>
        /// <returns>String - process output message</returns>
        public static string ProcessImages(string src, string dest)
        {
            string output;
            int count = 0;
            ImageShrink image = null, thumb = null;

            try
            {
                FileInfo[] posterImgs = FilterImgFiles(src);

                foreach (var file in posterImgs)
                {
                    ImageFormat fileFormat = GetImageFormat(file.Name);

                    if (fileFormat == null) continue;

                    image = new ImageShrink(file.FullName);
                    string thumbPath = Fixurl(dest,file.Name);

                    thumb = image.GetThumbnailImage(PWidth, PHeight, ThumbnailMethod.Fit);

                    thumb.SaveImage(thumbPath, fileFormat);
                    count++;
                }

            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                if (thumb != null) thumb.Destroy();
                if (image != null) image.Destroy();

                PostProcess(src, dest);

                output = "Processing complete : " + count + " images was processed";
            }

            return output;
        }

        /// <summary>
        /// Traverses the src and dest after processing to ensure the file with the lesser size was synced
        /// </summary>
        /// <param name="src">source path</param>
        /// <param name="dest">destination path</param>
        /// <returns>String - process notification</returns>
        public static string PostProcess(string src, string dest)
        {
            string output;
            
            try
            {
                // filter only images
                FileInfo[] posterImgs = FilterImgFiles(src);
                FileInfo[] thumbImgs = FilterImgFiles(dest);

                for (int i = 0; i < thumbImgs.Length; i++)
                {
                    long orgSize = posterImgs[i].Length;
                    long thbSize = thumbImgs[i].Length;

                    if (orgSize > thbSize) continue;

                    thumbImgs[i].Delete();
                    posterImgs[i].CopyTo(thumbImgs[i].FullName);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                output = "process complete";
            }

            return output;            
        }

        #region Report Generation Function

        /// <summary>
        /// Generate a report of all images
        /// Comparitive anaylysis of the images reduced size and their original to see byte difference
        /// </summary>
        /// <param name="src">uncompressed images folder</param>
        /// <param name="dest">compressed images folder</param>
        /// <returns>String - process output message</returns>
        public static string GenerateReport(string src, string dest)
        {
            List<ImageData> records = new List<ImageData>();
            string output;
            
            try
            {
                FileInfo[] posterImgs = FilterImgFiles(src);
                FileInfo[] thumbImgs = FilterImgFiles(dest);

                if (posterImgs.Length != thumbImgs.Length)
                    return "folders does not seem to be in sync";

                for (int i = 0; i < thumbImgs.Length; i++)
                {
                    long orgSize = posterImgs[i].Length;
                    long thbSize = thumbImgs[i].Length;

                    records.Add(new ImageData
                                    {
                                        OrgSize = orgSize / 1024, //convert to KB
                                        OriginalFileName = posterImgs[i].Name,
                                        ThumbSize = thbSize / 1024,
                                        ThumbFileName = thumbImgs[i].Name,
                                        PercentageDiff = CalcPercentage(thbSize,orgSize) + " %"
                                    });
                }

            }
            catch(Exception e)
            {
                return e.Message;
            }
            finally
            {
               WriteCsv(records, Fixurl(src,DefaultImgReport));
               output = records.Count + " records processed";
            }

            return output;
        }

        /// <summary>
        /// The following function generates a byte size comparison report of compressed files vs unprocessed files
        /// </summary>
        /// <param name="directory">directory</param>
        /// <returns>String - process output message</returns>
        public static string GenerateByteArrayReport(string directory)
        {

            List<ImageData> records = new List<ImageData>();
            string output = string.Empty;
            ImageShrink image = null, thumb = null;
            try
            {
                string[] files = Directory.GetFiles(directory);

                foreach (var file in files)
                {
                    ImageFormat fileFormat = GetImageFormat(file);

                    if (fileFormat == null) continue;

                    string fileName = Path.GetFileName(file);
                    image = new ImageShrink(file);
                    thumb = image.GetThumbnailImage(PWidth, PHeight, ThumbnailMethod.Fit);

                    long orgSize = image.ByteArray.Length;
                    long thbSize = thumb.ByteArray.Length;

                    records.Add(new ImageData
                    {
                        OrgSize = orgSize, //convert to KB
                        OriginalFileName = fileName,
                        ThumbSize = thbSize,
                        ThumbFileName = "thumb_" + fileName,
                        PercentageDiff = CalcPercentage(thbSize,orgSize) + " %"
                    });
                }

            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                if (image != null) image.Destroy();
                if (thumb != null) thumb.Destroy();
                WriteCsv(records, Fixurl(directory,DefaultBytleFile));
                output = records.Count + " records processed";
            }

            return output;
        }

        #endregion


        #region Utility Function

        /// <summary>
        /// Retrieves the imageformat for a given file extension
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static ImageFormat GetImageFormat(string filename)
        {
            switch (Path.GetExtension(filename.ToLower()))
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
        /// Calculate the percentage of value portion of the whole
        /// </summary>
        /// <param name="portion">portion used</param>
        /// <param name="total">total size</param>
        /// <returns></returns>
        public static double CalcPercentage(long portion, long total)
        {
            return ((double)(total - portion) / total) * 100;
        }

        /// <summary>
        /// Fixed paths not ending with \ 
        /// </summary>
        /// <param name="path">path</param>
        /// <returns></returns>
        public static string Fixurl(string path)
        {
            return path.EndsWith("\\") ? path : path + "\\";
        }

        /// <summary>
        /// Fixed paths not ending with \ 
        /// </summary>
        /// <param name="desc">path to folder</param>
        /// <param name="fileName">fileName to be added to the end</param>
        /// <returns></returns>
        public static string Fixurl(string desc, string fileName)
        {
            return Fixurl(desc) + fileName;
        }

        /// <summary>
        /// Returns a filtered array of Image types FileInfo
        /// </summary>
        /// <param name="location">folder path</param>
        /// <returns>Collection of file information for the filtered images</returns>
        public static FileInfo[] FilterImgFiles(string location)
        {
            return FilterImgFiles(new DirectoryInfo(location));
        }

        /// <summary>
        /// Returns a filtered array of Image types FileInfo
        /// </summary>
        /// <param name="dInfo">Directory Information</param>
        /// <returns>Collection of file information for the filtered images</returns>
        public static FileInfo[] FilterImgFiles(DirectoryInfo dInfo)
        {
            return dInfo.GetFiles().Where(a => Pics.Any(a.Name.ToLower().Contains)).ToArray();
        }

        /// <summary>
        /// Writes a collection of records to a csv file to the location path provided
        /// </summary>
        /// <param name="records">Collection of records</param>
        /// <param name="location">destination path</param>
        private static void WriteCsv<T>(IEnumerable<T> records, string location) where T : class
        {
            if (records == null || !records.Any())
                return;

            CsvWriter csv = null;
            try
            {
                csv = new CsvWriter(new StreamWriter(location));
                csv.WriteRecords(records);
            }
            finally
            {
                if (csv != null) csv.Dispose();
            }
        }

        /// <summary>
        /// Writes to logfile.
        /// </summary>
        /// <param name="filename">filename</param>
        /// <param name="content">The content.</param>
        /// <param name="append">if set to <c>true</c> [append].</param>
        public static void WriteToLogfile(string filename, string content, bool append)
        {
            using (StreamWriter file = new StreamWriter(filename, append))
            {
                file.Write(content);
            }
        }

        #endregion


        #region ImageData struct
        internal class ImageData
        {
            public string OriginalFileName { get; set; }

            public long OrgSize { get; set; }

            public string ThumbFileName { get; set; }

            public long ThumbSize { get; set; }

            public string PercentageDiff { get; set; }
        }
        #endregion

    }
}
