using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class SQLiteDB : MonoBehaviour
{
    public static SQLiteDB instance;
    private string dbName = "URI=file:DataBaseHand.db";
    private void Awake()
    {
        instance = this;
    }

    public string NowTable = "Dictionary";
    void Start()
    {
        UpdateDictionary();
        //CreateAlphabetTable();
        // CreateDictionaryTable();
        //Query("DROP TABLE Dictionary");
        //Query("ALTER TABLE Witam2 RENAME TO Witam; ");
        //Query("insert into Dictionary(category) where id = 17 values('szkoła');");
    }

    public void SelectDataHand(string name)
    {
        int rh = GetDataRight(name);
        int lh = GetDataLeft(name);

        if (rh == 1 && lh == 0)
        {
            SelectDataWord(name, 0);
        }
        else if(rh == 0 && lh == 1)
        {
            SelectDataWord(name, 1);
        }
        else if(rh == 1 && lh == 1)
        {
            SelectDataWordForTwoHand(name, 2);
        }
    }

    public int GetDataLeft(string name)
    {
        string leftHand = QueryShowDataLeftHand("SELECT LeftHand FROM " + NowTable + " WHERE name = '" + name + "'");
        int lh = Int32.Parse(leftHand);
        return lh;
    }
    public int GetDataRight(string name)
    {
        string rightHand = QueryShowDataRightHand("SELECT RightHand FROM " + NowTable + " WHERE name = '" + name + "'");
        int rh = Int32.Parse(rightHand);
        return rh;
    }


    public bool check(string nameWord)
    {
       bool b = QueryForDictionaryCheck("SELECT * From " + NowTable + " WHERE name = '" + nameWord + "';", nameWord);
       return b;
    }
   
    public void UpdateDictionary()
    {
        //find all letters and sort them by alphabetical order
        QueryForDictionary("SELECT * FROM Alphabet ORDER BY name;", true); //AlphabetTest
        QueryForDictionary("SELECT * FROM " + NowTable + " ORDER BY name;", false); //DictionaryTest
    }

    public void UpdateDictionarySearchLetter(string name, GameObject alp, GameObject dict)
    {
        foreach (Transform child in alp.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in dict.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        string nameWord = "%" + name + "%";
        QuerySearch("SELECT name FROM Alphabet WHERE name LIKE '" + nameWord + "';", true); //AlphabetTest
        QuerySearch("SELECT name FROM " + NowTable + " WHERE name LIKE '" + nameWord + "';", false); //DictionaryTest

    }
    public void FindByWorld(string category, GameObject dict)
    {
        foreach (Transform child in dict.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        //string nameWord = "%" + category + "%";
        //SELECT * FROM Dictionary WHERE category LIKE '%%';
        QuerySearchCategory("SELECT name FROM " + NowTable + " WHERE category LIKE '%" + category + "%';"); //DictionaryTest

    }

    public void QuerySearch(string q, bool b)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = q;
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader["name"].ToString();
                        if (b)
                        {
                            GameManager.instance.InstantiateWordSlot(name);
                        }else
                        {
                            GameManager.instance.InstantiateDictionaryWordSlot(name);
                        }
                    }
                }
            }
            connection.Close();
        }
    }

    public void QuerySearchCategory(string q)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = q;
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader["name"].ToString();
                        GameManager.instance.InstantiateDictionaryWordSlot(name);

                    }
                }
            }
            connection.Close();
        }
    }

    public string SelectVariations(string nameWord)
    {
        return QueryShowVar("SELECT variations FROM " + NowTable + " WHERE name = '" + nameWord + "';");
    }

    public void DeleteQuery(string nameWord)
    {
        Query("DROP TABLE IF EXISTS '" + nameWord + "';");
        Query("DELETE FROM " + NowTable + " WHERE name = '" + nameWord + "';");
    }

    public void InsertNewSegment(string timeStep, string position, string rotation, string TableName)
    {
        string sql = "INSERT INTO " + TableName + "(timeStep, position, rotation) VALUES('" + timeStep + "', '" + position + "', '" + rotation + "');";
        Query(sql);
    }

    public void SelectData(string TableName)
    {
        for (int i = 1; i <= 25; i++)
        {
            string sql = "SELECT * FROM " + TableName + " WHERE id = " + i + ";";
            QueryShowData(sql);
        }
    }
    void SelectDataWord(string TableName, int state)
    {
        for (int i = 1; i <= 25; i++)
        {
            string sql = "SELECT * FROM " + TableName + " WHERE id = " + i + ";";
            QueryShowDataWord(sql, state);
        }
    }

    void SelectDataWordForTwoHand(string TableName, int state)
    {
        for (int i = 1; i <= 50; i++)
        {
            string sql = "SELECT * FROM " + TableName + " WHERE id = " + i + ";";
            QueryShowDataWord(sql, state);
        }
    }

    public void InsertAlphabet(string name)
    {
        string sql = "INSERT INTO Alphabet(Name) VALUES('" + name + "');"; //AlphabetTest
        Query(sql);
    }

    public void InsertDictionary(string name, string variations, int RH, int LH, string category, int VR = 0)
    {
        string sql = "INSERT INTO " + NowTable + " (RightHand, LeftHand, name, variations, category, VR) VALUES('" + RH + "','" + LH + "','" + name + "','" + variations + "','" + category + "','" + VR + "');";//DictionaryTest  
        Query(sql);
    }

    public int SelectVRData(string name)
    {
        string vr = ToGetVRData("SELECT VR FROM " + NowTable + "  where name = '" + name + "';");
        int vrData = Int32.Parse(vr);
        return vrData;

    }

    public string GetVariations(string name)
    {
        return QueryVariationsDictionary("SELECT variations FROM " + NowTable + " WHERE name = '" + name + "';");
    }


    //for word info
    public string ToGetVRData(string q)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = q;
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string var = reader["VR"].ToString();
                        return var;
                    }
                }
            }
            return "0";
            connection.Close();
        }
    }


    //for word info
    public string QueryVariationsDictionary(string q)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = q;
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string var = reader["variations"].ToString();
                        return var;
                    }
                }
            }
            return "";
            connection.Close();
        }
    }

    public void Search(string str)
    {
        string[] newStr = str.Split(new char[] { ' ', '.', ',', ';', ':', '-' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in newStr)
        {
            Debug.Log(s);
        }

    }
    //for text convert to words
    public string QueryNameVar(string word)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM " + NowTable + "";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader["name"].ToString();
                        string var = reader["variations"].ToString();
                        string temp = name + " " + var;

                        string[] newStr = temp.Split(new char[] { ' ', '.', ',', ';', ':', '-' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string s in newStr)
                        {
                            bool result = s.Equals(word);
                            if (result)
                            {
                                return newStr[0];
                            }
                        }
                        
                    }
                }
            }
            return "";
            connection.Close();
        }
    }


    public string QueryShowDataRightHand(string q)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = q;
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string RightHand = reader["RightHand"].ToString();
                        return RightHand;
                    }
                }
            }
            return "";
            connection.Close();
        }
    }
    public string QueryShowDataLeftHand(string q)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = q;
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string LeftHand = reader["LeftHand"].ToString();
                        return LeftHand;
                    }
                }
            }
            return "";
            connection.Close();
        }
    }


    public string QueryShowVar(string q)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = q;
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string var = reader["variations"].ToString();
                        return var;
                    }
                }
            }
            return "";
            connection.Close();
        }
    }

    public bool QueryForDictionaryCheck(string q, string name)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = q;
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (Convert.ToString(reader["name"]) == name)
                        {
                            //  Debug.Log("i get = " + Convert.ToString(reader["name"]));
                            return true;
                        }
                    }
                }
            }
            connection.Close();
            return false;
        }
    }
    public void CreateTableByName(string NameOfTable)
    {
        Debug.Log("creating....");
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                string sqlcreation = "";

                sqlcreation += "CREATE TABLE IF NOT EXISTS "+ NameOfTable + "(";
                sqlcreation += "id INTEGER NOT NULL ";
                sqlcreation += "PRIMARY KEY AUTOINCREMENT,";
                sqlcreation += "timeStep	TEXT,";
                sqlcreation += "position	TEXT,";
                sqlcreation += "rotation	TEXT";
                sqlcreation += ");";

                command.CommandText = sqlcreation;
                command.ExecuteNonQuery();
            }

            connection.Close();
        }
        Debug.Log("created");
    }

    public void Query(string q)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = q;
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                       // Debug.Log("name: " + reader["name"] + " password: " + reader["password"]);
                    }
                }
            }
            connection.Close();
        }
    }
    //for letter
    public void QueryShowData(string q)
    {
        //Debug.Log("Query = " + q);

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = q;
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        string id = reader["id"].ToString();
                        string timeStep = reader["timeStep"].ToString();
                        string position = reader["position"].ToString();
                        string rotation = reader["rotation"].ToString();

                        //Debug.Log("Info id: = " + id + ", pos " + position);


                        GameManager.instance.SetLetterSegmentFromSQL(id, timeStep, position, rotation);
                    }
                }
            }
            connection.Close();
        }
    }
    //For word
    public void QueryShowDataWord(string q, int state)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = q;
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string id = reader["id"].ToString();
                        string timeStep = reader["timeStep"].ToString();
                        string position = reader["position"].ToString();
                        string rotation = reader["rotation"].ToString();
                        //Debug.Log("Id = " + id + ", time = " + timeStep + ", pos = " + position + ", rot = " + rotation);
                        GameManager.instance.SetWordSegmentFromSQL(id, timeStep, position, rotation, state);
                    }
                }
            }
            connection.Close();
        }
    }

    public void QueryForDictionary(string q, bool b)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = q;
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (b)
                        {
                            GameManager.instance.InstantiateWordSlot(Convert.ToString(reader["name"]));
                        }
                        else
                        {
                            GameManager.instance.InstantiateDictionaryWordSlot(Convert.ToString(reader["name"]));
                        }
                    }
                }
            }
            connection.Close();
        }
    }


    //creation alphabet
    public void CreateAlphabetTable()
    {
        Debug.Log("creating...");
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                string sqlcreation = "";
                sqlcreation += "CREATE TABLE Alphabet (";
                sqlcreation += "id INTEGER NOT NULL ";
                sqlcreation += "PRIMARY KEY AUTOINCREMENT,";
                sqlcreation += "name     TEXT NOT NULL";
                sqlcreation += ");";
                command.CommandText = sqlcreation;
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        Debug.Log("created");

    }
    //create dictionary
    public void CreateDictionaryTable()
    {
        Debug.Log("creating...");
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                string sqlcreation = "";
                sqlcreation += "CREATE TABLE Dictionary (";
                sqlcreation += "id INTEGER NOT NULL ";
                sqlcreation += "PRIMARY KEY AUTOINCREMENT,";
                sqlcreation += "LeftHand	int,";
                sqlcreation += "RightHand	int,";
                sqlcreation += "name     TEXT NOT NULL,";
                sqlcreation += "variations     TEXT NOT NULL";
                sqlcreation += ");";
                command.CommandText = sqlcreation;
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        Debug.Log("created");

    }
}
