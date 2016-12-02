using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DAL
{
    public class Sqlite
    {
        private SQLiteConnection m_dbConnection;

        public Sqlite()
        {
            
        }

        public void Connect()
        {
            //D:\git-repos\mamanet\client\MamanNetClient\UI\bin\x86\Debug\MamanNetDB.db
            m_dbConnection = new SQLiteConnection(@"Data Source=..\..\..\..\DAL\MamanNetDB.db;Version=3;");
            m_dbConnection.Open();
        }

        public void Insert(MamanNetFile file)
        {
            //string sql = "insert into highscores (name, score) values ('Me', 9001)";
            //string sql = "insert into highscores (name, score) values ('Me', 3000)";
            //SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            //command.ExecuteNonQuery();
            //sql = "insert into highscores (name, score) values ('Myself', 6000)";
            //command = new SQLiteCommand(sql, m_dbConnection);
            //command.ExecuteNonQuery();
            //sql = "insert into highscores (name, score) values ('And I', 9001)";
            //command = new SQLiteCommand(sql, m_dbConnection);
            //command.ExecuteNonQuery();
        }

        public List<MamanNetFile> GetAllSavedFiles()
        {
            string sql = "select * from File";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            List<MamanNetFile> mamanNetFiles = new List<MamanNetFile>();
            while (reader.Read())
            {
                var file = new MamanNetFile()
                {
                    BytesDownloaded = int.Parse(reader["BytesDownloaded"].ToString()),
                    Name = reader["Name"].ToString(),
                    ID = reader["ID"].ToString(),
                    FileSizeInBytes = int.Parse(reader["FileSizeInBytes"].ToString())
                };
                DownloadStatus fileDownloadStatus;
                FileType fileType;
                Enum.TryParse(reader["DownloadStatus"].ToString(), out fileDownloadStatus);
                FileType.TryParse(reader["type"].ToString(), out fileType);
                file.DownloadStatus = fileDownloadStatus;
                file.Type = fileType;
                mamanNetFiles.Add(file);
            }
            return mamanNetFiles;
        }
    }
}
