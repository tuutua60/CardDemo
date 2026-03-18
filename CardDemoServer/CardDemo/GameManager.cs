namespace CardDemo
{
    internal class GameManager
    {
        static void Main(string[] args)
        {
            Initialize();

            ServerSocket.Instance.Start("127.0.0.1",8080,10);

            //CreateWorld();

            while (true)
            {
                
            }
        }

        static void Initialize()
        {
            BinaryDataMgr.Instance.Initialize();
            WorldMgr.Instance.Initialize();
        }

        public static void CreateWorld(BattleMsg msg)
        {
            List<HeroData> playerList = new List<HeroData>();
            List<HeroData> enemyList = new List<HeroData>();
            for (int i = 1; i <= 5; i++)
            {
                playerList.Add(new HeroData(msg.playerDataList[i-1], i, E_GlobalTeamType.Player));
            }
            for (int i = 1; i <= 5; i++)
            {
                enemyList.Add(new HeroData(msg.enemyDataList[i-1], i, E_GlobalTeamType.Enemy));
            }
            WorldMgr.Instance.CreateWorld(playerList,enemyList,msg.seed);
        }
    }
}
