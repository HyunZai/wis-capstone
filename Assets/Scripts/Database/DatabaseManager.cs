using System.Diagnostics;
//using MySql.Data.MySqlClient;
using UnityEngine;

public class DatabaseManager
{
    // private MySqlConnection connection;

    // public void Connect()
    // {
    //     var config = DatabaseConfigLoader.LoadConfig();
    //     string connStr = $"server={config["db_host"]};user={config["db_user"]};password={config["db_password"]};database={config["db_name"]}";
    //     connection = new MySqlConnection(connStr);
    //     connection.Open();
    // }

    // // DB 연결 해제
    // public void Disconnect()
    // {
    //     if (connection != null)
    //     {
    //         connection.Close();
    //     }
    // }

    // public User login() {
    //     string query = "SELECT * FROM USER WHERE user_id = (SELECT COUNT(user_id) FROM USER)";
    //     MySqlCommand command = new MySqlCommand(query, connection);

    //     User user = null;

    //     try
    //     {
    //         MySqlDataReader reader = command.ExecuteReader();
    //         if (reader.Read()) 
    //         {
    //             user = new User 
    //             {
    //                 user_id = reader.GetInt32("user_id"),
    //                 name = reader.GetString("user_name"),
    //                 age = reader.GetInt32("age"),
    //                 parent_phone = reader.GetString("parent_phone"),
    //                 address = reader.GetString("address"),
    //                 gender = reader.GetInt32("gender"),
    //                 registered = reader.GetString("registered")
    //             };
    //         }

    //         return user;
    //     }
    //     catch (MySqlException ex) {
    //         UnityEngine.Debug.LogError("로그인 실패 : " + ex);  // 프리팹이 할당되지 않았을 경우 오류 메시지
    //     }

    //     return user;
    // }

    // public User register(User user) {
    //     string query = "INSERT INTO USER (user_name, parent_phone, address, gender, registered) VALUES (@user_name, @parent_phone, @address, @gender, @registered)";
    //     MySqlCommand cmd = new MySqlCommand(query, connection);

    //     cmd.Parameters.AddWithValue("@user_name", user.name);
    //     cmd.Parameters.AddWithValue("@parent_phone", user.parent_phone);
    //     cmd.Parameters.AddWithValue("@address", user.address);
    //     cmd.Parameters.AddWithValue("@gender", user.gender);
    //     cmd.Parameters.AddWithValue("@registered", user.registered);

    //     try 
    //     {
    //         cmd.ExecuteNonQuery();
    //         return user;
    //     }
    //     catch (MySqlException ex) 
    //     {
    //         UnityEngine.Debug.LogError("사용자 정보 등록 실패 : " + ex);  // 프리팹이 할당되지 않았을 경우 오류 메시지
    //     }
    //     return null;
    // }
}
