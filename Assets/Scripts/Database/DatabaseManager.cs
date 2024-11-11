using System;
using System.Data;
using System.Data.Common;
using Mono.Data.Sqlite;
using UnityEngine;

public class DatabaseManager
{
    private DbConnection dbConnection;

    //DB를 사용하는 (C#)스크립트 코드의 Start()에서 호출
    public void Connect()
    {
        string connectionString = $"URI=file:{Application.streamingAssetsPath}/wis.db";
        dbConnection = new SqliteConnection(connectionString);
        dbConnection.Open();
    }

    public User login() {
        User user = new User();

        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "SELECT * FROM user WHERE user_id = (SELECT COUNT(user_id) FROM user)";
        
        try
        {
            IDataReader dataReader = dbCommand.ExecuteReader();
            while (dataReader.Read()) 
            {
                user.name = dataReader.GetString(1);
                user.age = dataReader.GetInt32(2);
                user.parent_phone = dataReader.GetString(3);
                user.address = dataReader.GetString(4);
                user.gender = dataReader.GetInt32(5);
                user.registered = dataReader.GetString(6);
            }
        }
        catch (Exception ex) 
        {
            Debug.LogError("로그인 실패 : " + ex);
        }
        finally 
        {
            dbCommand.Dispose(); 
            dbConnection.Close();
        }

        return user;
    }

    public bool register(User user) {
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = $"INSERT INTO user (user_name, age, parent_phone, address, gender, registered) VALUES ('{user.name}', {user.age}, '{user.parent_phone}', '{user.address}', {user.gender}, '{user.registered}')";
        
        try 
        {
            dbCommand.ExecuteNonQuery();
        }
        catch (Exception ex) 
        {
            Debug.LogError("사용자 정보 등록 실패 : " + ex);  // 프리팹이 할당되지 않았을 경우 오류 메시지
        }
        finally 
        { 
            dbCommand.Dispose(); 
            dbConnection.Close();
        } 

        return true;
    }
}
