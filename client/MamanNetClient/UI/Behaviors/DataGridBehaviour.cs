using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using ViewModels.Files;
using Networking.Files;
using DAL;

namespace MamaNet.UI.Behaviors
{
    class DataGridBehaviour : Behavior<DataGrid>
    {
        private MetadataFileProvider _fileProvider; 

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Drop += AssociatedObject_Drop;
            if (AssociatedObject.DataContext is DownloadingFilesViewModel)
            {
                AssociatedObject.AllowDrop = true;    
            }
            
            _fileProvider = new MetadataFileProvider();
        }

        void AssociatedObject_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach(var file in files)
                {
                    
                    var metadataFile = _fileProvider.Load(file);
                    (AssociatedObject.DataContext as BaseFilesViewModel).AddFile(metadataFile);
                }
            }
        }
    }
}
