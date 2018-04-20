using Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;

namespace Plugin
{
    
    public class DownloadableImage : iLoader
    {
        private Image originalImage;

        [Import(typeof(ApplicationStatus), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ApplicationStatus appStatus;

        private void DownloadImage(ImageData imageData)
        {
            /*
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (o, e) =>
            {
                // System.Threading.Thread.Sleep(5000);

                WebClient webClient = imageData.DownloadClient;

             */
            Image newImage = null;
               
            imageData.DownloadClient.DownloadProgressChanged += (s, e) =>
            {
                if (imageData.LocalHashCode != appStatus.HashCode)
                {
                    imageData.DownloadClient.CancelAsync();
                }
                    
                return;
            };

            imageData.DownloadClient.DownloadDataCompleted += (s, e) =>
            {
                if (e.Cancelled)
                    return;

                using (var ms = new MemoryStream(e.Result))
                {
                    newImage = Image.FromStream(ms);
                }

                imageData.Callback(imageData.Index, newImage);
            };

            imageData.DownloadClient.DownloadDataAsync(new Uri(imageData.FullAddress));
            /*
            try
            {
                using (var ms = new MemoryStream(webClient.DownloadData(imageData.Url)))
                {
                    newImage = Image.FromStream(ms);
                }
            } catch (Exception ex)
            {
                imageData.Callback(imageData.Index, null);
            }
            */

            //    imageData.Callback(imageData.Index, newImage);
            // };

            // bw.RunWorkerAsync();
        }

        public DownloadableImage(Image originalImage)
        {
            this.originalImage = originalImage;
        }


        public Image Load(ImageData imageData)
        {
            DownloadImage(imageData);

            return originalImage;
        }
        
    }
    


    [Export(typeof(iLoader))]
    [ExportMetadata("FileTypes", ".exe", IsMultiple = true)]
    class Executable : iLoader
    {
        public Image Load(ImageData imageData)
        {
            return Properties.Resources.executable;
        }
    }

    [Export(typeof(iLoader))]
    [ExportMetadata("FileTypes", ".zip", IsMultiple = true)]
    [ExportMetadata("FileTypes", ".7z", IsMultiple = true)]
    class Archive : iLoader
    {
        public Image Load(ImageData imageData)
        {
            return Properties.Resources.archive;
        }
    }

    [Export(typeof(iLoader))]
    [ExportMetadata("FileTypes", ".otf", IsMultiple = true)]
    class OTF : iLoader
    {
        public Image Load(ImageData imageData)
        {
            return Properties.Resources.otf;
        }
    }

    [Export(typeof(iLoader))]
    [ExportMetadata("FileTypes", ".png", IsMultiple = true)]
    class PNG : iLoader
    {
        public Image Load(ImageData imageData)
        {
            return Properties.Resources.png;
        }
    }

    [Export(typeof(iLoader))]
    [ExportMetadata("FileTypes", ".pot", IsMultiple = true)]
    [ExportMetadata("FileTypes", ".potx", IsMultiple = true)]
    [ExportMetadata("FileTypes", ".ppt", IsMultiple = true)]
    [ExportMetadata("FileTypes", ".pptx", IsMultiple = true)]
    class PowerPoint : iLoader
    {
        public Image Load(ImageData imageData)
        {
            return Properties.Resources.powerpoint;
        }
    }

    [Export(typeof(iLoader))]
    [ExportMetadata("FileTypes", ".svg", IsMultiple = true)]
    class SVG : iLoader
    {
        public Image Load(ImageData imageData)
        {
            return Properties.Resources.svg;
        }
    }

    [Export(typeof(iLoader))]
    [ExportMetadata("FileTypes", ".wav", IsMultiple = true)]
    class WAV : iLoader
    {
        public Image Load(ImageData imageData)
        {
            return Properties.Resources.wav;
        }
    }

    [Export(typeof(iLoader))]
    [ExportMetadata("FileTypes", ".torrent", IsMultiple = true)]
    class Torrent : iLoader
    {
        public Image Load(ImageData imageData)
        {
            return Properties.Resources.torrent;
        }
    }


    [Export(typeof(iLoader))]
    [ExportMetadata("FileTypes", ".ttf", IsMultiple = true)]
    class TTF : iLoader
    {
        public Image Load(ImageData imageData)
        {
            return Properties.Resources.ttf;
        }
    }

    [Export(typeof(iLoader))]
    [ExportMetadata("FileTypes", ".txt", IsMultiple = true)]
    class Text : iLoader
    {
        public Image Load(ImageData imageData)
        {
            return Properties.Resources.txt;
        }
    }

    [Export(typeof(iLoader))]
    [ExportMetadata("FileTypes", ".jpg", IsMultiple = true)]
    [ExportMetadata("FileTypes", ".bmp", IsMultiple = true)]
    class JPG : DownloadableImage
    {
        JPG() : base(Properties.Resources.jpg){}

        /*
        public Image Load(ImageData imageData)
        {
            return Properties.Resources.jpg;
        }
        */
    }

    [Export(typeof(iLoader))]
    [ExportMetadata("FileTypes", ".ps", IsMultiple = true)]
    [ExportMetadata("FileTypes", ".eps", IsMultiple = true)]
    [ExportMetadata("FileTypes", ".prn", IsMultiple = true)]
    [ExportMetadata("FileTypes", ".pdf", IsMultiple = true)]
    class Acrobat : iLoader
    {
        public Image Load(ImageData imageData)
        {
            //ADD DOWNLOAD
            return Properties.Resources.acrobat;
        }
    }

    [Export(typeof(iLoader))]
    [ExportMetadata("FileTypes", ".doc", IsMultiple = true)]
    [ExportMetadata("FileTypes", ".docx", IsMultiple = true)]
    class Word : iLoader
    {
        public Image Load(ImageData imageData)
        {
            //ADD DOWNLOAD
            return Properties.Resources.word;
        }
    }

    [Export(typeof(iLoader))]
    [ExportMetadata("FileTypes", ".html", IsMultiple = true)]
    class Html : iLoader
    {
        public Image Load(ImageData imageData)
        {
            //ADD DOWNLOAD
            return Properties.Resources.html;
        }
    }

    [Export(typeof(iLoader))]
    [ExportMetadata("FileTypes", ".mp3", IsMultiple = true)]
    [ExportMetadata("FileTypes", ".m4a", IsMultiple = true)]
    [ExportMetadata("FileTypes", ".flac", IsMultiple = true)]
    [ExportMetadata("FileTypes", ".amr", IsMultiple = true)]
    [ExportMetadata("FileTypes", ".wma", IsMultiple = true)]
    class Music : iLoader
    {
        public Image Load(ImageData imageData)
        {
            //ADD DOWNLOAD
            return Properties.Resources.music;
        }
    }


    [Export(typeof(iLoader))]
    [ExportMetadata("FileTypes", ".mp4", IsMultiple = true)]
    [ExportMetadata("FileTypes", ".wmv", IsMultiple = true)]
    [ExportMetadata("FileTypes", ".mkv", IsMultiple = true)]
    [ExportMetadata("FileTypes", ".flv", IsMultiple = true)]
    [ExportMetadata("FileTypes", ".avi", IsMultiple = true)]
    [ExportMetadata("FileTypes", ".m4v", IsMultiple = true)]
    class Video : iLoader
    {
        public Image Load(ImageData imageData)
        {
            //ADD DOWNLOAD
            return Properties.Resources.video;
        }
    }

    [Export(typeof(iLoader))]
    [ExportMetadata("FileTypes", "directory", IsMultiple = true)]
    class Directory : iLoader
    {
        public Image Load(ImageData imageData)
        {
            //ADD DOWNLOAD
            return Properties.Resources.folder;
        }
        
    }



    [Export(typeof(iImageLoader))]
    class MySimpleCalculator : iImageLoader
    {
        [ImportMany]
        IEnumerable<Lazy<iLoader, iLoaderData>> operations;

        //public ImageData imageData { get; }

        public Image GetImage(ImageData imageData)
        {
            foreach (Lazy<iLoader, iLoaderData> i in operations)
            {
                if (i.Metadata.FileTypes.Contains(imageData.FileExt)) return i.Value.Load(imageData);
            }

            return Properties.Resources.empty;
            //return "Operation Not Found!";
        }
    }
}