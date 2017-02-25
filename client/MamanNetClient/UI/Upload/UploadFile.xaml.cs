using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using DAL;
using MamaNet.UI.Utils;
using MamaNet.UI.Views;
using Networking.Files;
using Networking.Utilities;
using ViewModels.Files;
using Path = System.Windows.Shapes.Path;

namespace MamaNet.UI.Upload
{
    /// <summary>
    /// Interaction logic for UploadFile.xaml
    /// </summary>
    public partial class UploadFile
    {
        public string filePath { get; private set; }
        public UploadFileRequest UploadFileRequest { get; private set; }
        protected MainWindow MainWindow { get; private set; }


        public UploadFile(MainWindow mainiWindow)
        {
            InitializeComponent();
            MainWindow = mainiWindow;

            FilePartSizes.Items.Add("2");
            FilePartSizes.Items.Add("768");
            FilePartSizes.Items.Add("1024");
            FilePartSizes.SelectedIndex = 0;

            var hubEndPoints = (ConfigurationManager.GetSection("availableHubs") as EndPointsConfigurationSection);
            foreach (EndPointElement hubElement in hubEndPoints.Instance)
            {
                SelectableEndPoint selectableEndPoint = new SelectableEndPoint() {EndPoint = hubElement.EndPoint, IsSelected = false};
                FileHubs.Items.Add(selectableEndPoint);
            }

            var indedxersEndPoints = (ConfigurationManager.GetSection("availableIndexers") as EndPointsConfigurationSection);
            foreach (EndPointElement hubElement in indedxersEndPoints.Instance)
            {
                SelectableEndPoint selectableEndPoint = new SelectableEndPoint() { EndPoint = hubElement.EndPoint, IsSelected = true };
                FileIndexer.Items.Add(selectableEndPoint);
            }
        }

        public UploadFileRequest GetExposedFileReqest()
        {
            return UploadFileRequest;
        }

        private void OpenFileDialog(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "All Files|*.*";
            fileDialog.ShowDialog();
            fileDialog.Multiselect = false;
            filePath = fileDialog.FileName;
        }

        private async void UploadFileButtonClicked(object sender, RoutedEventArgs e)
        {
            UploadFileRequest = new UploadFileRequest();
            UploadFileRequest.Hubs = new List<string>();
            foreach (SelectableEndPoint hub in FileHubs.Items)
            {
                if (hub.IsSelected)
                {
                    UploadFileRequest.Hubs.Add(hub.EndPoint);
                }
            }
            UploadFileRequest.Indexers = new List<string>();
            foreach (SelectableEndPoint indexer in FileIndexer.Items)
            {
                if (indexer.IsSelected)
                {
                    UploadFileRequest.Indexers.Add(indexer.EndPoint);
                }
            }
            UploadFileRequest.PartSize = int.Parse(FilePartSizes.SelectedValue.ToString());
            UploadFileRequest.Description = FileDescription.Text;
            UploadFileRequest.FilePath = filePath;

            if (!UploadFileRequest.Hubs.Any() || string.IsNullOrWhiteSpace(UploadFileRequest.FilePath))
            {
                MainWindow.ShowPopup(this, "שגיאר בעת ניסיון יצירת קובץ Metadata.");
                return;
            }

            var provider = new MetadataFileProvider();
            var fileInfo = new FileInfo(UploadFileRequest.FilePath);
            MamaNetFile file;
            using (var stream = fileInfo.OpenRead())
            {
                file = new MamaNetFile(fileInfo.Name, HashUtils.CalculateHash(stream), UploadFileRequest.FilePath, (int)fileInfo.Length, isFullAvailable: true, indexer: UploadFileRequest.Indexers.SingleOrDefault(), description: UploadFileRequest.Description,relatedHubs: UploadFileRequest.Hubs.ToArray())
                {
                    IsActive = true
                };
            }

            var metadata = new MetadataFile(file);
            var fileName = System.IO.Path.GetFileNameWithoutExtension(UploadFileRequest.FilePath);

            await provider.SaveAndSend(metadata, System.IO.Path.Combine(ConfigurationManager.AppSettings["DonwloadFolderPath"], fileName + ".mamanet"));
            MainWindow.ShowPopup(this, "קובץ Metadata נוצר בהצלחה");
            this.Close();
        }
    }

    public class SelectableEndPoint
    {
        public bool IsSelected { get; set; }
        public string EndPoint { get; set; }
    }

    public class UploadFileRequest
    {
        public List<string> Hubs { get; set; }
        public List<string> Indexers { get; set; }
        public string Description { get; set; }
        public int PartSize { get; set; }
        public string FilePath { get; set; }
    }
}

