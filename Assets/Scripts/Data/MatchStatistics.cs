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

    }

    public enum TeamColor
    {
        None,
        Red,
        Blue
    }
}