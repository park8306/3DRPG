using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System;
using System.Linq;

public class MySQL : MonoBehaviour
{
    static public MySQL instance;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update 
    void Start()
    {
        FirstUserDataSelect();
        MonsterDataSelect();
        ItemDataSelect();
        UserItemLoad();
        LevelDataSelect();
        QuestDataSelect();
        initQuestDictionary();
    }

    // 유저 아이템 가져오기
    // 유저 아이템 데이터베이스에는 id만 있고 id를 이용해 아이템 정보를 넣어줌
    private void UserItemLoad()
    {
        string connStr = "Server=localhost;Port=3306;Database=rpgdata;Uid=root;Pwd=!Imsua0312;";

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            conn.Open();
            string sql = "SELECT * FROM useritems";

            //ExecuteReader를 이용하여
            //연결 모드로 데이타 가져오기
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                UserItem userItemDB = new UserItem();
                int itemID;
                itemID = int.Parse(rdr["ItemID"].ToString());
                userItemDB.userItemCount = int.Parse(rdr["ItemCount"].ToString());
                allItems.TryGetValue(itemID, out ItemDB itemValue);
                userItemDB.userItemInfo = itemValue;

                UserDB.instance.userInventoryItems.Add(userItemDB);
            }
            rdr.Close();
            conn.Close();
        }
    }
    // 유저 아이템 추가
    public void UserItemInsert(int itemID)
    {
        string connStr = "Server=localhost;Port=3306;Database=rpgdata;Uid=root;Pwd=!Imsua0312;";

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            conn.Open();
            // A.I는 NULL을 넣어주면 알아서 증가한다
            MySqlCommand cmd = new MySqlCommand($"INSERT INTO useritems VALUES (NULL,{itemID}, 1)", conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
    // 물약같은 아이템은 아이템 수정을 통해 갯수를 바꿔준다
    public void UserItemUpdate(int itemID, bool isCountIncrease)
    {
        string connStr = "Server=localhost;Port=3306;Database=rpgdata;Uid=root;Pwd=!Imsua0312;";

        // 증가시켜주는 부분
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            conn.Open();
            if(isCountIncrease)
            {
                MySqlCommand cmd = new MySqlCommand($"Update useritems SET ItemCount = ItemCount+1 Where ItemID = {itemID}", conn);
                cmd.ExecuteNonQuery();
            }
            else
            {
                MySqlCommand cmd = new MySqlCommand($"Update useritems SET ItemCount = ItemCount-1 Where ItemID = {itemID}", conn);
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }
    }

    public void UpdateUserData()
    {
        string connStr = "Server=localhost;Port=3306;Database=rpgdata;Uid=root;Pwd=!Imsua0312;";

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            conn.Open();
            string sql = $"UPDATE user SET gold='{UserDB.instance.playerGold}'," +
                $" gem='{UserDB.instance.playerGem}'," +
                $" Level='{UserDB.instance.userLv}'," +
                $" Exp='{UserDB.instance.userExp}' ";

            //ExecuteReader를 이용하여
            //연결 모드로 데이타 가져오기
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();

            conn.Close();
        }
    }

    private void FirstUserDataSelect()
    {
        string connStr = "Server=localhost;Port=3306;Database=rpgdata;Uid=root;Pwd=!Imsua0312;";

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            conn.Open();
            string sql = "SELECT * FROM user";

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                UserDB.instance.userId = rdr["userID"].ToString();
                UserDB.instance.userName = rdr["userName"].ToString();
                UserDB.instance.playerGold = int.Parse(rdr["Gold"].ToString());
                UserDB.instance.playerGem = int.Parse(rdr["Gem"].ToString());
                UserDB.instance.userExp = int.Parse(rdr["Exp"].ToString());
                UserDB.instance.userLv = int.Parse(rdr["Level"].ToString());
                Debug.Log($"{UserDB.instance.playerGold}");
            }
            rdr.Close();
            conn.Close();
        }
    }

    private void LevelDataSelect()
    {
        string connStr = "Server=localhost;Port=3306;Database=rpgdata;Uid=root;Pwd=!Imsua0312;";

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            conn.Open();
            string sql = "SELECT * FROM level";

            //ExecuteReader를 이용하여
            //연결 모드로 데이타 가져오기
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                LvDB lvDB = new LvDB();
                lvDB.level = int.Parse(rdr["Level"].ToString());
                lvDB.maxExp = int.Parse(rdr["MaxExp"].ToString());
                LvDBs.Add(lvDB);
            }
            rdr.Close();
            conn.Close();
        }
    }

    public void MonsterDataSelect()
    {
        string connStr = "Server=localhost;Port=3306;Database=rpgdata;Uid=root;Pwd=!Imsua0312;";

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            conn.Open();
            string sql = "SELECT * FROM monster";

            //ExecuteReader를 이용하여
            //연결 모드로 데이타 가져오기
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            
            while (rdr.Read())
            {
                MonsterDB monsterDB = new MonsterDB();
                monsterDB.monsterID = int.Parse( rdr["monsterID"].ToString());
                monsterDB.monsterName = rdr["monsterName"].ToString();
                monsterDB.monsterHP = int.Parse(rdr["monsterHP"].ToString());
                monsterDB.monsterDmg = int.Parse(rdr["monsterDmg"].ToString());
                monsterDB.monsterGold = int.Parse(rdr["monsterGold"].ToString());
                monsterDB.monsterExp = int.Parse(rdr["monsterExp"].ToString());

                monsters.Add(monsterDB);
            }
            rdr.Close();
            conn.Close();
        }
    }

    private void ItemDataSelect()
    {
        string connStr = "Server=localhost;Port=3306;Database=rpgdata;Uid=root;Pwd=!Imsua0312;";

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            conn.Open();
            string sql = "SELECT * FROM item";

            //ExecuteReader를 이용하여
            //연결 모드로 데이타 가져오기
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                ItemDB newDB = new ItemDB();
                newDB.itemID = int.Parse(rdr["ItemID"].ToString());
                newDB.itemName = rdr["ItemName"].ToString();
                newDB.itemExplain = rdr["ItemExplain"].ToString();
                newDB.iconName = rdr["IconName"].ToString();
                newDB.itemType = EnumUtil<ItemType>.Parse(rdr["ItemType"].ToString());
                newDB.itemAmount = int.Parse(rdr["ItemAmount"].ToString());
                newDB.itemPrice = int.Parse(rdr["ItemPrice"].ToString());
                newDB.itemSellPrice = int.Parse(rdr["ItemSellPrice"].ToString());

                allItems.Add(newDB.itemID,newDB);
            }
            rdr.Close();
            conn.Close();
        }
    }

    

    [System.Serializable]
    public class MonsterDB
    {
        public int monsterID;
        public string monsterName;
        public int monsterHP;
        public int monsterDmg;
        public int monsterGold;
        public int monsterExp;
    }

    public List<MonsterDB> monsters = new List<MonsterDB>();

    public static class EnumUtil<T> { 
        public static T Parse(string s) 
        {
            return (T)Enum.Parse(typeof(T), s);
        } 
    }

    public enum ItemType
    {
        Weapon = 1,
        Armor = 1 << 1,
        Shield = 1 << 2,
        Ring = 1 << 3,
        Potion = 1 << 4,
    }

    [System.Serializable]
    public class ItemDB
    {
        public int itemID;
        public string itemName;
        public string itemExplain;
        public string iconName;
        public ItemType itemType;
        public int itemAmount;
        public int itemPrice;
        public int itemSellPrice;
    }
    [System.Serializable]
    public class LvDB
    {
        public int level;
        public int maxExp;
    }
    public List<LvDB> LvDBs = new List<LvDB>();

    public Dictionary<int, ItemDB> allItems = new Dictionary<int, ItemDB>();

    public void QuestDataSelect()
    {
        string connStr = "Server=localhost;Port=3306;Database=rpgdata;Uid=root;Pwd=!Imsua0312;";

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            conn.Open();
            string sql = "SELECT * FROM quests";

            //ExecuteReader를 이용하여
            //연결 모드로 데이타 가져오기
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                QuestDB questDB = new QuestDB();
                questDB.questID = int.Parse(rdr["questID"].ToString());
                questDB.questTitle = rdr["questTitle"].ToString();
                questDB.questInfo = rdr["questInfo"].ToString();
                questDB.rewardGold = int.Parse(rdr["rewardGold"].ToString());
                questDB.rewardExp = int.Parse(rdr["rewardExp"].ToString());
                questDB.questTargetID = int.Parse(rdr["questTargetID"].ToString());
                questDB.questAmount = int.Parse(rdr["questAmount"].ToString());
                questDB.questTotalGoal = int.Parse(rdr["questTotalGoal"].ToString());

                questDBs.Add(questDB);
            }
            rdr.Close();
            conn.Close();
        }
    }

    [System.Serializable]
    public class QuestDB
    {
        public int questID;
        public string questTitle;
        public string questInfo;
        public int rewardGold;
        public int rewardExp;
        public int questTargetID;
        public int questAmount;
        public int questTotalGoal;
        public bool isQuestAccept;  // 퀘스트를 수락했는가?
        public bool isQuestClear;   // 퀘스트를 완료했는가?
        public bool isQuestReward;   // 퀘스트를 보상을 받았는가?

        // 퀘스트 수락과 완료 여부를 false로 지정해준다
        public QuestDB()
        {
            isQuestAccept = false;
            isQuestClear = false;
            isQuestReward = false;
        }
    }
    public List<QuestDB> questDBs = new List<QuestDB>();
    public Dictionary<int, QuestDB> questDbDictionary = new Dictionary<int, QuestDB>();
    // 리스트에 이미 퀘스트 정보를 넣었다
    // 딕셔너리로 바꾸려면 리스트에 있는 값을 이용해서 차례대로 딕셔너리에 넣어줄 수 있다.
    void initQuestDictionary()
    {
        for (int i = 0; i < questDBs.Count; i++)
        {
            questDbDictionary.Add(questDBs[i].questID, questDBs[i]);
        }
    }
    // 딕셔너리에 값이 잘 들어갔으니
    // 만약 유저가 퀘스트 수락 버튼으로 퀘스트를 받으면 리스트에 퀘스트를 추가시켜준다
    // 그리고 수락을 받았으면 isQuestAccept도 true로 변경해준다.
}
