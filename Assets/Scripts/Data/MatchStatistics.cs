using System.Collections.Generic;

namespace Data
{
    public class MatchStatistics
    {
        public GameController.PlayerState WinOrLoose;
        public TeamColor WinTeamColor;
        public int playerNum;
        public Dictionary<int,int> KillEnemy;
        public Dictionary<int,int> KillPlayers;
        public Dictionary<int,int> Exp;

        public List<int> TeamEnemyKill ;
    }

    public enum TeamColor
    {
        Red = 0,
        Blue = 1,
        None
    }
}