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
        private int gameTime;

        private Player player1;

        private Player player2;
        public BoggleGame(Tuple<StringSocket, string> player1, Tuple<StringSocket, string> player2, int gameTime)
        {
            this.player1 = new Player {player_name = player1.Item2, player_connection = player1.Item1, score = 0};
            this.player2 = new Player { player_name = player2.Item2, player_connection = player2.Item1, score = 0};
            this.gameTime = gameTime;
        }


        /// <summary>
        /// comment me!
        /// </summary>
        /// <param name="name"></param>
        public void increment_score(string name)
        {
            Player p = get_player(name);
            p.score += 1;

        }


        /// <summary>
        /// comment me!
        /// </summary>
        /// <param name="name"></param>
        public void decrement_score(string name)
        {
            Player p = get_player(name);
            p.score = p.score - 1;
        }


        /// <summary>
        /// method to get the player object from the provided name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Player get_player(string name)
        {
            if (player1.player_name.Equals(name))
            {
                return player1;
            }
            else if (player2.player_name.Equals(name))
            {
                return player2;
            }
            else return null; //i don't know why it would ever get here, but should keep in mind
        }


        /// <summary>
        /// comment me!
        /// </summary>
        public void broadcastGameScore()
        {
            string msg = "SCORE ";
            //we just have to send, don't care if it makes it (theortically? 
            //this may have to do with the dead conn.... TODO
            player1.player_connection.BeginSend(msg + player1.score.ToString() + " " + player2.score.ToString(), (e, o) => { }, null);
            player2.player_connection.BeginSend(msg + player2.score.ToString() + " " + player1.score.ToString(), (e, o) => { }, null);
        }

    }
}
internal class Player
{
    public string player_name { get; set; }

    public StringSocket player_connection { get; set; }

    public int score { get; set; }

}