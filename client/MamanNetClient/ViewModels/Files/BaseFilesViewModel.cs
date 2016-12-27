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
using System.Data.Entity.Infrastructure;
using Networking.Network;
using Networking.Utilities;

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
            sharedFile.IsActive = true;
            AddFile(sharedFile);
        }

        public virtual void AddFile(MamaNetFile file)
        {
            AllFiles.Add(file);
            RelevatFilesCollection.Add(file);
        }

        public virtual void _uploadFileByPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;
            var provider = new MetadataFileProvider();
            var fileInfo = new FileInfo(filePath);
            MamaNetFile file;
            using (var stream = fileInfo.OpenRead())
            {
                file = new MamaNetFile(fileInfo.Name, HashUtils.CalculateHash(stream), filePath, (int)fileInfo.Length, isFullAvailable: true)
                {
                    IsActive = true
                };
            }
            AddFile(file);

            var metadata = new MetadataFile(file);
            var fileName = Path.GetFileNameWithoutExtension(filePath);

            provider.Save(metadata, Path.Combine(ConfigurationManager.AppSettings["DonwloadFolderPath"], fileName + ".mamanet"));
            if (ShowPopup != null) ShowPopup(this, "קןבץ Metadata נוצר בהצלחה");
        }

        public virtual bool _canUploadFile(string filePath)
        {
            return true;
        }

        public virtual void _addMetadataFileByPath(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
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
            var indexRelevantCollection = 0;
            if (RelevatFilesCollection != null)
            {
                indexRelevantCollection = RelevatFilesCollection.IndexOf(SelectedFile);
            }

            var indexInAllFiles = AllFiles.IndexOf(SelectedFile);
            if (RelevatFilesCollection != null)
            {
                RelevatFilesCollection.RemoveAt(indexRelevantCollection);
            }

            AllFiles.RemoveAt(indexInAllFiles);
            SelectedFile = null;
            _raiseCommandsCanExecute();
        }

        public virtual bool _canDeleteFile()
        {
            if (SelectedFile == null)
                return false;
            return true;
        }

        public virtual void _disableFile()
        {
            SelectedFile.IsActive = false;
            _raiseCommandsCanExecute();
        }

        public virtual bool _canDisableFile()
        {
            if (SelectedFile != null && SelectedFile.IsActive)
                return true;
            return false;
        }

        public virtual void _activateFile()
        {
            SelectedFile.IsActive = true;
            _raiseCommandsCanExecute();
        }

        public virtual bool _canActivateFile()
        {
            if (SelectedFile != null && !SelectedFile.IsActive)
                return true;
            return false;
        }

        public virtual void _moveUpFile()
        {
            var index = RelevatFilesCollection.IndexOf(SelectedFile);
            RelevatFilesCollection.Move(index, index - 1);
            _raiseCommandsCanExecute();
        }

        public virtual bool _canMoveUpFile()
        {
            if (SelectedFile == null || RelevatFilesCollection.First() == SelectedFile)
                return false;
            return true;
        }

        public virtual void _moveDownFile()
        {
            var index = RelevatFilesCollection.IndexOf(SelectedFile);
            RelevatFilesCollection.Move(index, index + 1);
            _raiseCommandsCanExecute();
        }

        public virtual bool _canMoveDownFile()
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
            StopCommand = new RelayCommand(_disableFile, _canDisableFile);
            PlayCommand = new RelayCommand(_activateFile, _canActivateFile);
            UpCommand = new RelayCommand(_moveUpFile, _canMoveUpFile);
            DownCommand = new RelayCommand(_moveDownFile, _canMoveDownFile);
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
