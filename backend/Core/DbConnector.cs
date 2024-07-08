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
    public static List<Machine>? ListMachines()
    {
        List<Machine> list = new List<Machine>();
        using (var connection = new MySqlConnection(connStr))
        {
            try
            {
                connection.Open();
                string sql = "select * from machines";
                MySqlCommand command = new MySqlCommand(sql, connection);
                MySqlDataReader data_reader = command.ExecuteReader(System.Data.CommandBehavior.Default);
                while (data_reader.Read())
                {
                    int id = data_reader.GetInt32(0);
                    string name = data_reader.GetString(1);
                    string hostname = data_reader.GetString(2);
                    string ipv4 = data_reader.GetString(3);
                    string ipv6 = data_reader.GetString(4);
                    int port = data_reader.GetInt32(5);
                    int posx = data_reader.GetInt32(6);
                    int posy = data_reader.GetInt32(7);
                    int roomId = data_reader.GetInt32(8);
                    list.Add(new Machine(id, name, hostname, ipv4, ipv6, port, posx, posy, roomId));
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
    public static List<Machine>? ListMachines(int roomId)
    {
        List<Machine> list = new List<Machine>();
        using (var connection = new MySqlConnection(connStr))
        {
            try
            {
                connection.Open();
                string sql = "select * from machines where roomId = @roomId";
                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@roomId", roomId);
                MySqlDataReader data_reader = command.ExecuteReader(System.Data.CommandBehavior.Default);
                while (data_reader.Read())
                {
                    int id = data_reader.GetInt32(0);
                    string name = data_reader.GetString(1);
                    string hostname = data_reader.GetString(2);
                    string ipv4 = data_reader.GetString(3);
                    string ipv6 = data_reader.GetString(4);
                    int port = data_reader.GetInt32(5);
                    int posx = data_reader.GetInt32(6);
                    int posy = data_reader.GetInt32(7);
                    list.Add(new Machine(id, name, hostname, ipv4, ipv6, port, posx, posy, roomId));
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
    public static List<Machine>? ListMachines(List<int> ids)
    {
        List<Machine> list = new List<Machine>();
        using (var connection = new MySqlConnection(connStr))
        {
            try
            {
                string ids_params = "";
                int brojac = 0;
                foreach (var id in ids)
                {
                    ids_params += "@id"+ brojac.ToString() + " ,";
                    brojac++;
                }
                string sql = "select * from machines where id IN (" + ids_params.Substring(0,  ids_params.Length - 1) +")";
                connection.Open();
                MySqlCommand command = new MySqlCommand(sql, connection);
                brojac = 0;
                foreach (var id in ids)
                {
                    ids_params = "@id"+ brojac.ToString();
                    command.Parameters.AddWithValue(ids_params, id);
                    brojac++;
                }
                MySqlDataReader data_reader = command.ExecuteReader(System.Data.CommandBehavior.Default);
                while (data_reader.Read())
                {
                    int id = data_reader.GetInt32(0);
                    string name = data_reader.GetString(1);
                    string hostname = data_reader.GetString(2);
                    string ipv4 = data_reader.GetString(3);
                    string ipv6 = data_reader.GetString(4);
                    int port = data_reader.GetInt32(5);
                    int posx = data_reader.GetInt32(6);
                    int posy = data_reader.GetInt32(7);
                    int roomId = data_reader.GetInt32(8);
                    list.Add(new Machine(id, name, hostname, ipv4, ipv6, port, posx, posy, roomId));
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
    public static bool AddMachines(Machine machine)
    {
        using (var connection = new MySqlConnection(connStr))
        {
            try
            {
                connection.Open();
                string sql = "INSERT INTO machines(name, hostname, ipv4, ipv6, port, posx, posy, roomid) VALUES (@name, @hostname, @ipv4, @ipv6, @port, @posx, @posy, @roomid)";

                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@name", machine.name);
                command.Parameters.AddWithValue("@hostname", machine.hostname);
                command.Parameters.AddWithValue("@ipv4", machine.ipv4);
                command.Parameters.AddWithValue("@ipv6", machine.ipv6);
                command.Parameters.AddWithValue("@port", machine.port);
                command.Parameters.AddWithValue("@posx", machine.posx);
                command.Parameters.AddWithValue("@posy", machine.posy);
                command.Parameters.AddWithValue("@roomid", machine.roomId);

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
    public static bool EditMachines(int id, Machine machine)
    {
        using (var connection = new MySqlConnection(connStr))
        {
            try
            {
                connection.Open();
                string sql = "UPDATE machines SET name = @name , hostname = @hostname, ipv4 = @ipv4, ipv6 = @ipv6, port = @port, posx = @posx, posy = @posy, roomid = @roomid WHERE id = @id";

                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@name", machine.name);
                command.Parameters.AddWithValue("@hostname", machine.hostname);
                command.Parameters.AddWithValue("@ipv4", machine.ipv4);
                command.Parameters.AddWithValue("@ipv6", machine.ipv6);
                command.Parameters.AddWithValue("@port", machine.port);
                command.Parameters.AddWithValue("@posx", machine.posx);
                command.Parameters.AddWithValue("@posy", machine.posy);
                command.Parameters.AddWithValue("@roomid", machine.roomId);
                command.Parameters.AddWithValue("@id", id);

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

