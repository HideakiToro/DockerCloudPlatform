using System;
using MySqlConnector;

public class UserConnector
{
    static MySqlConnection conn;    //Anbindung an MySQL
    public static void connect()
    {
        string connStr = "server=127.0.0.1;user=root;database=mysql;port=6000;password=pass";
        conn = new MySqlConnection(connStr);
        try
        {
            conn.Open();
            SendCommand("CREATE TABLE Users (id int NOT NULL AUTO_INCREMENT, name varchar(255), password varchar(255), PRIMARY KEY(id));");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public static bool isOk()
    {
        return conn != null;
    }

    public static MySqlDataReader SendCommand(string command)
    {
        Console.WriteLine($"\n----- {command.Split(';')[0]} -----\n");
        try
        {
            string sql = command;
            using var cmd = new MySqlCommand(sql, conn);

            using var reader = cmd.ExecuteReader();

            return reader;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return null;
        }
    }
}
