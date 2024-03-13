using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;

namespace UniTinderServer
{
    class DataBase : IDisposable
    {

        private SQLiteConnection _sqliteConnection;

        public DataBase(string connectionString)
        {
            _sqliteConnection = new SQLiteConnection(connectionString);
            _sqliteConnection.Open(); // Открыть соединение при создании экземпляра
        }

        public int GetUserIDByNickname(string Nickname)
        {
            
            using (SQLiteCommand cmd = new SQLiteCommand($"SELECT * FROM User WHERE Nickname = \"{Nickname}\"", _sqliteConnection))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return Convert.ToInt32(reader.GetValue(0));
                        }
                        return -1;
                    }
                    else { Console.WriteLine("Пользователь не существует"); return -1; }
                }
                    
            }
              
        }

        #region Til better times

        public DataTable SelectTable(string tableName)
        {
            return ExecuteQuery($"SELECT * FROM {tableName}");
        }

        public DataTable SelectColumns(string tableName, string[] columns)
        {
            string columnsString = string.Join(", ", columns);
            return ExecuteQuery($"SELECT {columnsString} FROM {tableName}");
        }

        public void InsertRow(string tableName, string[] columns, string[] values)
        {
            string columnsString = string.Join(", ", columns);
            string valuesString = string.Join(", ", values.Select(v => $"'{v}'"));
            ExecuteNonQuery($"INSERT INTO {tableName} ({columnsString}) VALUES ({valuesString})");
        }

        public int DeleteRow(string tableName, string column, string value)
        {
            return ExecuteNonQuery($"DELETE FROM {tableName} WHERE {column} = '{value}'");
        }

        private DataTable ExecuteQuery(string query)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(query, _sqliteConnection))
            {
                using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    return dt;
                }
            }
        }

        private int ExecuteNonQuery(string query)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(query, _sqliteConnection))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        #endregion

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
