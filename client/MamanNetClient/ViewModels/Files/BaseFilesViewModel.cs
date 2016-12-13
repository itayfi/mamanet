using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using System.IO;
using Networking.Files;
using DAL;
using System.Security.Cryptography;
using System.Configuration;

namespace ViewModels.Files
{
    public abstract class BaseFilesViewModel : INotifyPropertyChanged
    {
        #region Public Fields
        public ObservableCollection<MamaNetFile> AllFiles { get; set; }
        public ObservableCollection<MamaNetFile> RelevatFilesCollection { get; set; }
        public RelayCommand<MamaNetFile> SelectionChangedCommand { get; set; }
        public RelayCommand<string> UploadFileCommand { get; set; }
        public RelayCommand<string> AddMetadataFileCommand { get; set; }
        public RelayCommand RemoveFileCommand { get; set; }
        public RelayCommand StopCommand { get; set; }
        public RelayCommand PlayCommand { get; set; }
        public RelayCommand UpCommand { get; set; }
        public RelayCommand DownCommand { get; set; }

        public MamaNetFile SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                _selectedFile = value;
                FireChangeEvent("SelectedFile");
            }
        }

        public event EventHandler<string> ShowPopup;
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Fields

        private MamaNetFile _selectedFile;

        #endregion

        #region Commands

        public virtual void _selectionChanged(MamaNetFile file)
        {
            SelectedFile = file;
            _raiseCommandsCanExecute();
        }

        public virtual void AddFile(MetadataFile file)
        {
            MamaNetFile sharedFile = new MamaNetFile(file, ConfigurationManager.AppSettings["DonwloadFolderPath"]);
            AddFile(sharedFile);
        }

        public virtual void AddFile(MamaNetFile file)
        {
            AllFiles.Add(file);
            RelevatFilesCollection.Add(file);
        }

        public virtual void _uploadFileByPath(string filePath)
        {
            if (filePath != null && filePath.Length > 0)
            {
                var provider = new MetadataFileProvider();
                var fileInfo = new FileInfo(filePath);
                MamaNetFile file;
                using (var stream = fileInfo.OpenRead())
                {
                    var md5 = MD5.Create();
                    file = new MamaNetFile(fileInfo.Name, md5.ComputeHash(stream), filePath, (int)fileInfo.Length, isAvailable: true)
                    {
                        IsActive = true
                    };
                }
                AddFile(file);
                var metadata = new MetadataFile(file);
                provider.Save(metadata, fileInfo.Directory.FullName + "\\" + fileInfo.Name + ".mamanet");

                if (ShowPopup != null) ShowPopup(this, @"קובץ הועלה בהצלחה!
קובץ ה-mamanet נמצא באותה תיקייה עם הקובץ המקורי");
            }
        }

        public virtual bool _canUploadFile(string filePath)
        {
            return true;
        }

        public virtual void _addMetadataFileByPath(string filePath)
        {
            if (filePath != null)
            {
                var provider = new MetadataFileProvider();
                AddFile(provider.Load(filePath));

                if (ShowPopup != null) ShowPopup(this, "קובץ התווסף בהצלחה!");
            }
        }

        public virtual bool _canAddFile(string filePath)
        {
            return true;
        }

        public virtual void _deleteFile()
        {
            int indexRelevantCollection = 0;
            if (RelevatFilesCollection != null)
            {
                indexRelevantCollection = RelevatFilesCollection.IndexOf(SelectedFile);
            }

            int indexInAllFiles = AllFiles.IndexOf(SelectedFile);
            if (RelevatFilesCollection != null)
            {
                RelevatFilesCollection.RemoveAt(indexRelevantCollection);
            }

            AllFiles.RemoveAt(indexInAllFiles);
        }

        public virtual bool _canDeleteFile()
        {
            if (SelectedFile == null)
                return false;
            return true;
        }

        public virtual void _stopFile()
        {
            SelectedFile.IsActive = false;
            _raiseCommandsCanExecute();
        }

        public virtual bool _canStopFile()
        {
            if (SelectedFile != null && SelectedFile.DownloadStatus == DownloadStatus.Downloading)
                return true;
            return false;
        }

        public virtual void _playFile()
        {
            SelectedFile.IsActive = false;
            _raiseCommandsCanExecute();
        }

        public virtual bool _canPlayFile()
        {
            if (SelectedFile != null && SelectedFile.DownloadStatus != DownloadStatus.Downloading)
                return true;
            return false;
        }

        public virtual void _upFile()
        {
            var index = RelevatFilesCollection.IndexOf(SelectedFile);
            RelevatFilesCollection.Move(index, index - 1);
            _raiseCommandsCanExecute();
        }

        public virtual bool _canUpFile()
        {
            if (SelectedFile == null || RelevatFilesCollection.First() == SelectedFile)
                return false;
            return true;
        }

        public virtual void _downFile()
        {
            var index = RelevatFilesCollection.IndexOf(SelectedFile);
            RelevatFilesCollection.Move(index, index + 1);
            _raiseCommandsCanExecute();
        }

        public virtual bool _canDownFile()
        {
            if (SelectedFile == null || RelevatFilesCollection.Last() == SelectedFile)
                return false;
            return true;
        }

        #endregion

        #region Methods

        protected BaseFilesViewModel(ObservableCollection<MamaNetFile> allFiles) : this()
        {
            AllFiles = allFiles;
            RelevatFilesCollection = AllFiles;
        }

        protected BaseFilesViewModel()
        {
            AllFiles = new ObservableCollection<MamaNetFile>();
            SelectionChangedCommand = new RelayCommand<MamaNetFile>(_selectionChanged);
            UploadFileCommand = new RelayCommand<string>(_uploadFileByPath, _canUploadFile);
            AddMetadataFileCommand = new RelayCommand<string>(_addMetadataFileByPath, _canAddFile);
            RemoveFileCommand = new RelayCommand(_deleteFile, _canDeleteFile);
            StopCommand = new RelayCommand(_stopFile, _canStopFile);
            PlayCommand = new RelayCommand(_playFile, _canPlayFile);
            UpCommand = new RelayCommand(_upFile, _canUpFile);
            DownCommand = new RelayCommand(_downFile, _canDownFile);
        }

        public void FireChangeEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public abstract void FilterAllFilesToCollectionFiles();

        public virtual void _raiseCommandsCanExecute()
        {
            AddMetadataFileCommand.RaiseCanExecuteChanged();
            RemoveFileCommand.RaiseCanExecuteChanged();
            PlayCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
            UpCommand.RaiseCanExecuteChanged();
            DownCommand.RaiseCanExecuteChanged();
        }
        #endregion

    }
}
