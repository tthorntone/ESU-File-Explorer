using Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ESUFileExplorer
{
    public class MEFHelper
    {
        private CompositionContainer _container;

        [Import(typeof(iImageLoader))]
        public iImageLoader imageLoaderPlugin;

        /*
        [Import(typeof(ApplicationStatus), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ApplicationStatus appStatus;
        */

        public MEFHelper(string path = "/")
        {
            Compose(path, true);
        }

        void Compose(string path, bool tryAgain)
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(path)); //@"C:\Users\tthor\Documents\Visual Studio 2015\Projects\Interface\Plugin\bin\Debug\Plugin.dll"
            //catalog.Catalogs.Add(new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly())); // <-- THIS WAS THE MISSING PIECE
            _container = new CompositionContainer(catalog);
            try
            {
                this._container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                // MessageBox.Show("Could Not Find ImageLoader Plugin...", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                string imageLoaderPath = Path.Combine(path, "ImageLoader.dll");
                if (tryAgain)
                {
                    if (!File.Exists(imageLoaderPath))
                    {
                        File.WriteAllBytes(imageLoaderPath, ESUFileExplorer.Properties.DLLs.ImageLoader);
                        File.SetAttributes(imageLoaderPath, FileAttributes.Hidden);
                    }

                    Compose(path, false);
                    return;
                } else
                {
                    //File.Delete(imageLoaderPath);
                }

                MessageBox.Show(compositionException.Message);
            }
            catch (System.Reflection.ReflectionTypeLoadException reflectionException)
            {
                // MessageBox.Show("Error Parsing ImageLoader Plugin...", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show(reflectionException.Message);
            }

        }
    }
}
