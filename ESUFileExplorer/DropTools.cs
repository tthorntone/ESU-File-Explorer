using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ESUFileExplorer
{
    public class DropTools
    {
        //private bool DropReady;
        private FileSystemWatcher Fsw;
        //private String DropDir;

        private Action<string> DroppedCallback;

        public void Start_DropWatch(Control control)
        {
            String sPath = System.IO.Path.GetTempFileName() + ".DROPTARGET";
            using (StreamWriter sw = new StreamWriter(sPath))
                sw.WriteLine("Placeholder");


            StringCollection s = new StringCollection();
            s.Add(sPath);

            //DropReady = false;
            Fsw = new FileSystemWatcher();
            Fsw.Path = "C:\\";
            Fsw.IncludeSubdirectories = true;
            Fsw.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            Fsw.Filter = "*.DROPTARGET";
            Fsw.Created += Fsw_Created;
            Fsw.EnableRaisingEvents = true;

            // This will copy the dummy file to the destination folder. This way,
            // Once the operation finishes, we'll get the destination dir through
            // the FileSystemWatcher's event.
            DataObject obj = new DataObject(DataFormats.FileDrop, s);
            //obj.SetFileDropList(s);
            control.DoDragDrop(obj, DragDropEffects.Copy);

           // while (!DropReady)
          //      Thread.Sleep(1);

            // All good, we can do our drag operation stuff here (write
            // files to DropDir or something like that).

        }

        // P/invoke stuff

        [DllImport("shell32.dll")]
        static extern void SHChangeNotify(HChangeNotifyEventID wEventId,
                                          HChangeNotifyFlags uFlags,
                                          IntPtr dwItem1,
                                          IntPtr dwItem2);

        [Flags]
        enum HChangeNotifyEventID
        {
            SHCNE_ALLEVENTS = 0x7FFFFFFF,
            SHCNE_ASSOCCHANGED = 0x08000000,
            SHCNE_UPDATEDIR = 0x00001000,
        }

        [Flags]
        public enum HChangeNotifyFlags
        {
            SHCNF_DWORD = 0x0003,
            SHCNF_IDLIST = 0x0000,
        }

        // This will delete the dummy file if it exists before starting the
        // drop operation

        void Fsw_Created(object sender, FileSystemEventArgs e)
        {
            Fsw.Dispose();

            DirectoryInfo d = new FileInfo(e.FullPath).Directory;
            try
            {
                File.Delete(e.FullPath);
            }
            catch
            {
                // If we can't delete the file, release the handle and try again
                GC.Collect(); // Dispose/kill the object. Dispose may work, too
                Thread.Sleep(500);
                File.Delete(e.FullPath);
            }
            d.Refresh();

            // Refresh Windows Explorer
            SHChangeNotify(HChangeNotifyEventID.SHCNE_ALLEVENTS, HChangeNotifyFlags.SHCNF_DWORD, IntPtr.Zero, IntPtr.Zero);
            SHChangeNotify(HChangeNotifyEventID.SHCNE_ASSOCCHANGED, HChangeNotifyFlags.SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
            SHChangeNotify(HChangeNotifyEventID.SHCNE_UPDATEDIR, HChangeNotifyFlags.SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);

            string DropDir = new FileInfo(e.FullPath).Directory.FullName;
            if (!DropDir.EndsWith("\\"))
                DropDir += "\\";

            if(DroppedCallback != null)DroppedCallback(DropDir);

            // All good, we can start dropping our files to the target dir now.
            //DropReady = true;
        }

        public DropTools(Action<string> callback)
        {
            //DropReady = false;
           // Fsw = new FileSystemWatcher();
            if (callback != null)
                DroppedCallback = callback;
        }
    }
}
