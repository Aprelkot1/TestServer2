using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TestKot
{
    public partial class MainWindow : Window
    {
      
        public void OnStart()
        {
            MysqlReader("CREATE TABLE if not exists Tests (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, testName TEXT NOT NULL)", 0);
            MysqlReader("CREATE TABLE if not exists Questions (testName INTEGER NOT NULL,id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, question TEXT NOT NULL, type TEXT NOT NULL,FOREIGN KEY (testName) REFERENCES Tests (id) ON DELETE CASCADE)", 0);
            MysqlReader("CREATE TABLE if not exists Answers (question INTEGER NOT NULL, answer TEXT NOT NULL, isRight TEXT NOT NULL,FOREIGN KEY (question) REFERENCES Questions (id) ON DELETE CASCADE)", 0);
            MysqlReader("CREATE TABLE if not exists UserResult (userName TEXT NOT NULL, test TEXT NOT NULL, result TEXT NOT NULL)", 0);
        }
        public List<string> MysqlReader(string commandText, int value)
        {
            using (var connection = new SqliteConnection("Data Source=TestKot.db"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand();
                command.Connection = connection;
                command.CommandText = commandText;
                if (!commandText.Contains("SELECT"))
                {
                    command.ExecuteNonQuery();
                    return null;
                }
                else
                {
                    List<string> resultReader = new List<string>();
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows) // если есть данные
                        {
                            while (reader.Read())   // построчно считываем данные
                            {
                               resultReader.Add(reader.GetString(value));
                            }
                        }
                    }
                    return resultReader;
                }
            }
        }
    }
}
