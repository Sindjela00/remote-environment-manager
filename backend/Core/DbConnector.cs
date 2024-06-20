using System.Configuration;
using System.Data;
using System.Diagnostics;
using MySql.Data.MySqlClient;


public class DB
{
    private static string connStr = "datasource=localhost;user=root;database=diplomski;port=3306;password=root;";
    public static string get_cs()
    {
        return connStr;
    }
    public static List<Room>? ListRooms()
    {
        List<Room> list = new List<Room>();
        using (var connection = new MySqlConnection(connStr))
        {
            try
            {
                connection.Open();
                string sql = "select * from rooms";
                MySqlCommand command = new MySqlCommand(sql, connection);
                MySqlDataReader data_reader = command.ExecuteReader(System.Data.CommandBehavior.Default);
                while (data_reader.Read())
                {
                    list.Add(new Room(data_reader.GetInt32(0), data_reader.GetString(1)));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        return list;
    }
    public static bool AddRoom(string roomName)
    {
        using (var connection = new MySqlConnection(connStr))
        {
            try
            {
                connection.Open();
                string sql = "INSERT INTO rooms(name) VALUES (@name)";

                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@name", roomName);
                int rows_effected = command.ExecuteNonQuery();
                if (rows_effected > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
    public static bool RenameRoom(uint id, string roomName)
    {
        using (var connection = new MySqlConnection(connStr))
        {
            try
            {
                connection.Open();
                string sql = "UPDATE rooms SET name = @name WHERE id = @id";

                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@name", roomName);
                int rows_effected = command.ExecuteNonQuery();
                if (rows_effected > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}

