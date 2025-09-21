using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;



public class Cosa
{
    public float SpawnX { get; set; }
    public float SpawnY { get; set; }
    public string stringX { get; set; }
    public string stringY { get; set; }
    public int Item1 { get; set; }
    public int Item2 { get; set; }
    public int Item3 { get; set; }
    public int Item4 { get; set; }
    public int Item5 { get; set; }
    public int Item6 { get; set; }

}

public class DBManager : MonoBehaviour
{
    private static DBManager _instance;
    public static DBManager Instance => _instance;

    public static int ActualGameId = 0;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this; // instead of FindObjectOfType
            DontDestroyOnLoad(gameObject); // keep this manager alive
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        DBManager.EnsureDatabase();

    }
    void Start()
    {
        IDbConnection dbconn;
        string conn = "URI=file:" + Application.dataPath + "/Plugins/GameDataBase.db";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        CreateTable();
    }
    public static void EnsureDatabase()
    {
        using (var dbconn = new SqliteConnection("URI=file:" + Application.dataPath + "/Plugins/GameDataBase.db"))
        {
            dbconn.Open();
            using (var dbcmd = dbconn.CreateCommand())
            {
                dbcmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS GameData (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT,
                Data TEXT
            );";
                dbcmd.ExecuteNonQuery();
            }
        }
    }


    public static Cosa GetValues(int id)
    {
        IDbConnection dbconn;
        string conn = "URI=file:" + Application.dataPath + "/Plugins/GameDataBase.db";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        Cosa cosa = null;
        IDbCommand dbcmd = dbconn.CreateCommand();

        string query = $"select SpawnX,SpawnY, Item1,Item2,Item3,Item4,Item5,Item6  from GameData where Id={id}";

        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {

            cosa = new Cosa()
            {
                SpawnX = reader.GetFloat(0),
                SpawnY = reader.GetFloat(1),
                Item1 = reader.GetInt32(2),
                Item2 = reader.GetInt32(3),
                Item3 = reader.GetInt32(4),
                Item4 = reader.GetInt32(5),
                Item5 = reader.GetInt32(6),
                Item6 = reader.GetInt32(7)
            };
        }
        return cosa;
    }

    public static void NewGame()
    {
        IDbConnection dbconn;
        string conn = "URI=file:" + Application.dataPath + "/Plugins/GameDataBase.db";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();
        string query = "INSERT INTO GameData(SpawnX,SpawnY,Item1,Item2,Item3,Item4,Item5,Item6) Values (-25.34,0.68,0,0,0,0,0,0)";
        dbcmd.CommandText = query;
        dbcmd.ExecuteNonQuery();
    }

    public static void DeleteGame(int Id)
    {
        IDbConnection dbconn;
        string conn = "URI=file:" + Application.dataPath + "/Plugins/GameDataBase.db";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();
        string query = $"DELETE FROM GameData WHERE Id = {Id}";
        dbcmd.CommandText = query;
        dbcmd.ExecuteNonQuery();
    }

    public static void CreateTable()
    {
        IDbConnection dbconn;
        string conn = "URI=file:" + Application.dataPath + "/Plugins/GameDataBase.db";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();
        string query = "CREATE TABLE IF NOT EXISTS 'GameData' ('Id'    INTEGER NOT NULL,'SpawnX' REAL NOT NULL, 'SpawnY'    REAL NOT NULL,'Item1' INTEGER NOT NULL,'Item2' INTEGER NOT NULL,'Item3' INTEGER NOT NULL,'Item4' INTEGER NOT NULL,'Item5' INTEGER NOT NULL,'Item6' INTEGER NOT NULL, PRIMARY KEY('Id' AUTOINCREMENT))";

        dbcmd.CommandText = query;
        dbcmd.ExecuteNonQuery();
    }

    public static void SaveData(Cosa cosa)
    {
        IDbConnection dbconn;
        string conn = "URI=file:" + Application.dataPath + "/Plugins/GameDataBase.db";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();
        Debug.Log(cosa.SpawnX);
        string query = $"UPDATE GameData set SpawnX = {cosa.stringX}, SpawnY = {cosa.stringY}, Item1 = {cosa.Item1}, Item2 = {cosa.Item2}, Item3 = {cosa.Item3}, Item4 = {cosa.Item4}, Item5 = {cosa.Item5}, Item6 = {cosa.Item6} WHERE Id = {ActualGameId}";


        dbcmd.CommandText = query;
        dbcmd.ExecuteNonQuery();
    }


    public static void CloseDB()
    {
        IDbConnection dbconn;
        string conn = "URI=file:" + Application.dataPath + "/Plugins/GameDataBase.db";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        dbconn.Close();
    }

    public static int TotalCount()
    {
        IDbConnection dbconn;
        string conn = "URI=file:" + Application.dataPath + "/Plugins/GameDataBase.db";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        int total = 0;
        IDbCommand dbcmd = dbconn.CreateCommand();

        string query = $"select count(*) from GameData";

        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            total = reader.GetInt32(0);
        }
        return total;
    }

    public static List<int> GetAllIds()
    {
        using (var dbconn = new SqliteConnection("URI=file:" + Application.dataPath + "/Plugins/GameDataBase.db"))
        {
            dbconn.Open();
            List<int> ids = new List<int>();
            using (var dbcmd = dbconn.CreateCommand())
            {
                // Always order by Id
                dbcmd.CommandText = "SELECT Id FROM GameData ORDER BY Id ASC;";
                using (var reader = dbcmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ids.Add(reader.GetInt32(0));
                    }
                }
            }
            return ids;
        }
    }


    public static int GetLastId()
    {
        IDbConnection dbconn;
        string conn = "URI=file:" + Application.dataPath + "/Plugins/GameDataBase.db";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        int total = 0;
        IDbCommand dbcmd = dbconn.CreateCommand();

        string query = $"SELECT Id FROM GameData order by Id DESC LIMIT 1;";

        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            total = reader.GetInt32(0);
        }
        return total;
    }


}
