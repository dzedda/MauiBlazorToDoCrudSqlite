using MauiBlazorToDo.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBlazorToDo.Data
{
    public class Datasource
    {
        public static string dataSourceString = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"todo.db");

        public async static Task InitDB()
        {
            if (!File.Exists(dataSourceString))
            {
                SqliteConnection myConnection;
                SqliteCommand myCommand;
                myConnection = new SqliteConnection("Data Source=" + dataSourceString);
                //creo il command 
                myCommand = new SqliteCommand("CREATE TABLE tblTodos (Id INTEGER  PRIMARY KEY AUTOINCREMENT,Title TEXT,Due   DATETIME, Done  BOOL DEFAULT (false));");
                myCommand.Connection = myConnection;

                myConnection.Open();
                var resp = await myCommand.ExecuteNonQueryAsync();
                myConnection.Close();
            }
        }

        public  async Task<int> AddTodo(ToDoItem item)
        {
            await InitDB();
            using (SqliteConnection myConn = new SqliteConnection("Data Source=" + dataSourceString))
            {
                SqliteCommand myCommand = new SqliteCommand("INSERT INTO tblTodos (Title,Due,Done) VALUES (@par1,@par2,@par3);  SELECT last_insert_rowid();");
                myCommand.Connection = myConn;
                SqliteParameter myPar = new SqliteParameter("@par1", item.Title);
                SqliteParameter myPar2 = new SqliteParameter("@par2", item.Due);
                SqliteParameter myPar3 = new SqliteParameter("@par3", item.Done);
                myCommand.Parameters.Add(myPar);
                myCommand.Parameters.Add(myPar2);
                myCommand.Parameters.Add(myPar3);
                myConn.Open();                
                item.Id =Convert.ToInt32(await myCommand.ExecuteScalarAsync());

               
                myConn.Close();
                return item.Id;
            }

        }

        public async Task<List<ToDoItem>> GetTodos()
        {
            await InitDB();
            List < ToDoItem > myList = new List<ToDoItem> ();
            using (SqliteConnection myConn = new SqliteConnection("Data Source=" + dataSourceString))
            {
                SqliteCommand myCommand = new SqliteCommand("SELECT * FROM tblTodos;");
                myCommand.Connection = myConn;
                
                myConn.Open();
               
                    SqliteDataReader mydr = await myCommand.ExecuteReaderAsync();
                    while (mydr.Read())
                    {
                        var item = new ToDoItem()
                        {
                            Id = Convert.ToInt32(mydr["id"]),
                            Title = mydr["Title"].ToString(),
                            Due =DateTime.Parse(mydr["Due"].ToString()),
                            Done = Convert.ToBoolean(mydr["Done"])
                        };
                        myList.Add(item);
                    }
              
                
                


                myConn.Close();
                return myList;
            }
        }

        public async Task UpdateTodo(ToDoItem item)
        {
            await InitDB();
            using (SqliteConnection myConn = new SqliteConnection("Data Source=" + dataSourceString))
            {
                SqliteCommand myCommand = new SqliteCommand("UPDATE tblTodos SET Title=@par1, Due=@par2, Done =@par3 WHERE Id=@par4;");
                myCommand.Connection = myConn;
                SqliteParameter myPar = new SqliteParameter("@par1", item.Title);
                SqliteParameter myPar2 = new SqliteParameter("@par2", item.Due);
                SqliteParameter myPar3 = new SqliteParameter("@par3", item.Done);
                SqliteParameter myPar4 = new SqliteParameter("@par4", item.Id);
                myCommand.Parameters.Add(myPar);
                myCommand.Parameters.Add(myPar2);
                myCommand.Parameters.Add(myPar3);
                myCommand.Parameters.Add(myPar4);
                myConn.Open();
                await myCommand.ExecuteScalarAsync();


                myConn.Close();
                
            }
        }
        public async Task DeleteTodo(ToDoItem item)
        {
            await InitDB();
            using (SqliteConnection myConn = new SqliteConnection("Data Source=" + dataSourceString))
            {
                SqliteCommand myCommand = new SqliteCommand("DELETE FROM tblTodos WHERE Id=@par4;");
                myCommand.Connection = myConn;
                SqliteParameter myPar4 = new SqliteParameter("@par4", item.Id);
                myCommand.Parameters.Add(myPar4);
                myConn.Open();
                await myCommand.ExecuteScalarAsync();

                myConn.Close();

            }
        }
    }
}
