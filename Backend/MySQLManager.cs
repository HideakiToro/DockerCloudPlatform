using System;
using MySqlConnector;

public class MySQLManager
{
    static MySqlConnection conn;    //Anbindung an MySQL
    public static void connect()
    {
        string connStr = "server=127.0.0.1;user=root;database=mysql;port=3307;password=pass";
        conn = new MySqlConnection(connStr);
        try
        {
            conn.Open();

            try
            {
                SendCommand("CREATE TABLE Users (id int NOT NULL AUTO_INCREMENT, name varchar(255), password varchar(255), PRIMARY KEY(id));");
                SendCommand("CREATE TABLE Containers (id varchar(255) NOT NULL, name varchar(255), userID int, port int, PRIMARY KEY(id));");
                SendCommand("CREATE TABLE Ports (port int, PRIMARY KEY(port));");
                if (SendCommand("SELECT * FROM Ports").Length <= 0)
                {
                    for (int i = 6000; i < 7000; i++)
                    {
                        SendCommand($"INSERT INTO Ports (port) VALUES ({i})");
                    }
                    Console.WriteLine(SendCommand("SELECT * FROM Ports").Length);
                }
            }
            catch { }

            Console.WriteLine("Table Users now usable.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public static bool isOk()
    {
        return conn != null && conn.Ping();
    }

    public static string[] SendCommand(string command)
    {
        try
        {
            string sql = command;
            using var cmd = new MySqlCommand(sql, conn);

            using var reader = cmd.ExecuteReader();

            List<string> result = new List<string>();

            while (reader.Read())
            {
                string line = "";
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    switch (reader.GetDataTypeName(i))
                    {
                        case "INT":
                            line += reader.GetUInt32(i) + ",";
                            break;
                        case "VARCHAR":
                            line += reader.GetString(i) + ",";
                            break;
                    }
                }
                result.Add(line);
            }

            return result.ToArray();
        }
        catch (Exception e)
        {
            return [];
        }
    }
}
