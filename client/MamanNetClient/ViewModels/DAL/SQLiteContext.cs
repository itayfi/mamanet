using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace ViewModel.DAL
{
    public class SqLiteContext
    {
        public List<File> FileCollection { get; set; }
        private SQLiteConnection m_dbConnection;

        public SqLiteContext()
        {
            
        }

        public void Connect()
        {
            m_dbConnection = new SQLiteConnection(@"Data Source=C:\Users\Idan\Documents\Visual Studio 2013\Projects\MamanNet\MamanNet\bin\Debug\MamanNetDB.db;Version=3;");
            m_dbConnection.Open();
        }

        public void Insert(File file)
        {
            string sql = "insert into highscores (name, score) values ('Me', 9001)";
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

        public List<File> Get()
        {
            string sql = "select * from File";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var file = new File()
                {
                    BytesDownloaded = int.Parse(reader["BytesDownloaded"].ToString()),
                    Name = reader["Name"].ToString(),
                    ID = reader["ID"].ToString(),
                    FileSizeInBytes = int.Parse(reader["FileSizeInBytes"].ToString()),
                    FinishedPercentage = int.Parse(reader["FinishedPercentage"].ToString()),
                    Leechers = int.Parse(reader["leechers"].ToString()),
                    Seeders = int.Parse(reader["seeders"].ToString()),

                };
                var fileDownloadStatus = file.DownloadStatus;
                var fileType = file.Type;
                Enum.TryParse(reader["DownloadStatus"].ToString(), out fileDownloadStatus);
                Enum.TryParse(reader["type"].ToString(), out fileType);
                FileCollection.Add(file);
            }
            return FileCollection;
        }
    }
}
