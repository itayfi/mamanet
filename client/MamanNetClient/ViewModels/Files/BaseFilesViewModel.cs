using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Models;

namespace ViewModel.Files
{
    public abstract class BaseFilesViewModel:INotifyPropertyChanged
    {
        #region Public Fields
        public ObservableCollection<MamanNetFile> AllFiles { get; set; }
        public ObservableCollection<MamanNetFile> RelevatFilesCollection { get; set; }
        public RelayCommand<MamanNetFile> SelectionChangedCommand { get; set; }
        public RelayCommand<string> AddFileCommand { get; set; }
        public RelayCommand RemoveFileCommand { get; set; }
        public RelayCommand StopCommand { get; set; }
        public RelayCommand PlayCommand { get; set; }
        public RelayCommand UpCommand { get; set; }
        public RelayCommand DownCommand { get; set; }

        public MamanNetFile SelectedFile
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

        private MamanNetFile _selectedFile;
    
        #endregion

        #region Commands

        public virtual void _selectionChanged(MamanNetFile file)
        {
            SelectedFile = file;
            _rasieCommandsCanExecute();
        }

        public virtual void AddFile(MamanNetFile file)
        {
            AllFiles.Add(file);
            RelevatFilesCollection.Add(file);
        }

        public virtual void _addFileByPath(string filePath)
        {
            if (filePath != null)
            {
                var index1 = filePath.LastIndexOf(@"\", StringComparison.Ordinal);
                var index2 = filePath.LastIndexOf(".", StringComparison.Ordinal);
                MamanNetFile mamanNetFile = new MamanNetFile();
                mamanNetFile.DownloadStatus = DownloadStatus.Paused;
                mamanNetFile.FinishedPercentage = 0;
                mamanNetFile.Name = filePath.Substring(index1 + 1, (index2 - index1) - 1);
                AddFile(mamanNetFile);

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
            SelectedFile.DownloadStatus = DownloadStatus.Paused;
            _rasieCommandsCanExecute();
        }

        public virtual bool _canStopFile()
        {
            if (SelectedFile != null && SelectedFile.DownloadStatus == DownloadStatus.Downloading)
                return true;
            return false;
        }

        public virtual void _playFile()
        {
            SelectedFile.DownloadStatus = DownloadStatus.Downloading;
            _rasieCommandsCanExecute();
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
            _rasieCommandsCanExecute();
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
            _rasieCommandsCanExecute();
        }

        public virtual bool _canDownFile()
        {
            if (SelectedFile == null || RelevatFilesCollection.Last() == SelectedFile)
                return false;
            return true;
        }

        #endregion

        #region Methods

        protected BaseFilesViewModel(ObservableCollection<MamanNetFile> allFiles):this()
        {
            AllFiles = allFiles;
            RelevatFilesCollection = AllFiles;
        }

        protected BaseFilesViewModel()
        {
            AllFiles = new ObservableCollection<MamanNetFile>();
            SelectionChangedCommand = new RelayCommand<MamanNetFile>(_selectionChanged);
            AddFileCommand = new RelayCommand<string>(_addFileByPath, _canAddFile);
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

        public virtual void _rasieCommandsCanExecute()
        {
            AddFileCommand.RaiseCanExecuteChanged();
            RemoveFileCommand.RaiseCanExecuteChanged();
            PlayCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
            UpCommand.RaiseCanExecuteChanged();
            DownCommand.RaiseCanExecuteChanged();
        }
        #endregion

    }
}
