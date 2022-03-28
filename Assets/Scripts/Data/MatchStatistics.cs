using System.Collections.Generic;

namespace Data
{
    public class MatchStatistics
    {
        public GameController.PlayerState WinOrLoose;
        public TeamColor WinTeamColor;
        public int KillEnemy;
        public int KillPlayers;
        public int Exp;

        public List<int> TeamEnemyKill ;
    }

    public enum TeamColor
    {
        Red = 0,
        Blue = 1,
        None
    }
}