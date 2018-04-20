using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace Interfaces
{
    /*
    [Export, PartCreationPolicy(CreationPolicy.Shared)]
    public class ApplicationStatus
    {
        private int hashCode;
       // ApplicationStatus(int hashCode)
       // {
       //     this.hashCode = hashCode;
       //}

        public int HashCode { get { return hashCode; } set { hashCode = value; } }
    }
    */

    [Export, PartCreationPolicy(CreationPolicy.Any)]
    public class ImageData
    {
        private int index;

        private String fileExt;

        private String fullAddress;

        private Action<int, Image> callback;

        private bool localFile;

        private HttpClient downloadClient;

        private CancellationToken cancellationToken;

        //private int localHashCode;

        //public static int currentHashCode;

        public ImageData(int index, String fileExt)
        {
            this.index = index;
            this.fileExt = fileExt;
        }

        public ImageData(int index, String fileExt, String fullAddress, bool localFile) : this(index, fileExt)
        {
            this.fullAddress = fullAddress;
            this.localFile = localFile;
        }

        public ImageData(int index, String fileExt, String fullAddress, /*int localHashCode, */HttpClient downloadClient, CancellationToken cancellationToken, Action<int, Image> callback) : this(index, fileExt, fullAddress, false)
        {
            //this.localHashCode = localHashCode;
            this.downloadClient = downloadClient;
            this.cancellationToken = cancellationToken;
            this.callback = callback;
        }

        //public int LocalHashCode { get { return localHashCode; } }

        //public int CurrentHashCode { get { return currentHashCode; } }

        public int Index { get { return index; } }

        public String FileExt { get { return fileExt; } }

        public String FullAddress { get { return fullAddress; } }

        public bool LocalFile { get { return localFile; } }

        public HttpClient DownloadClient { get { return downloadClient; } }

        public CancellationToken CancellationToken { get { return cancellationToken; } }

        public Action<int, Image> Callback { get { return callback; } }
    }
    


    public interface iImageLoader
    {
        //ImageData imageData { get; }
        Image GetImage(ImageData imageData);
    }

    public interface iLoaderData
    {
        string[] FileTypes { get; }
    }

    public interface iLoader
    {
        Image Load(ImageData imageData);
    }

    /*
    [InheritedExport]
    public interface IHost
    {
        void SendUpdated(string updated);
    }
    */
}