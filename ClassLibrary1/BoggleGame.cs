using CustomNetworking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    public class BoggleGame
    {
        private struct Player
        {
            public string player_name { get; set; }

            public StringSocket player_connection {get; set; }

            public int score { get; set; }

        }

        Player player1;

        Player player2;
        public BoggleGame(Tuple<StringSocket, string> player1, Tuple<StringSocket, string> player2)
        {
            this.player1 = new Player {player_name = player1.Item2, player_connection = player1.Item1, score = 0};
            this.player2 = new Player { player_name = player2.Item2, player_connection = player2.Item1, score = 0};
        }


    }
}
