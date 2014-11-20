using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using CustomNetworking;
using System.Text.RegularExpressions;

namespace BB
{
    public class BoggleServer
    {
        private Dictionary<string, BoggleGame> player_game_map;

        private TcpListener server_receiver;

        private TcpClient server_sender;

        private Queue<Tuple<StringSocket, string>> playerList;

        private System.Text.UTF8Encoding cod = new System.Text.UTF8Encoding();

        private string temp_id = "user_id_";  //temp placeholder.. ZERO BASED

        private int count = 0;

        //variable that is initialized via command line for the game time limit
        private int gameTime;  
        // In Progress: private Dictionary<>

        /// <summary>
        /// method to allow this console application to be called from the command line
        /// </summary>
        /// <param name="arguments"></param>
        public static void Main(string[] arguments)
        {
            new BoggleServer(2000, arguments);
            throw new Exception("Use BoggleLauncher until we figure this out.");
            //Console.ReadLine();
        }

        public BoggleServer(Int32 port, String[] arguments)
        {
            Console.WriteLine("It works!");
            ////debug line
            //{ Console.ReadLine(); }
            //end debug line
            //initialize the dict which maps players to games
            player_game_map = new Dictionary<string, BoggleGame>();                                                                                                                                                                                                                                                                                                                                                                                              
            server_receiver = new TcpListener(IPAddress.Any, 2000);
            server_receiver.Start();
            // Ask the server to call ConnectionRequested at some point in the future when 
            // a connection request arrives.  It could be a very long time until this happens.
            // The waiting and the calling will happen on another thread.
            server_receiver.BeginAcceptSocket(ConnectionRequested, null);
        }


        private void ConnectionRequested(IAsyncResult result)
        {
            Socket temp = server_receiver.EndAcceptSocket(result);
            //start listening for more connections
            server_receiver.BeginAcceptSocket(ConnectionRequested, null);
            //initialize a new StringSocket to do the communicating
            StringSocket new_conn = new StringSocket(temp, cod);
            //TODO: need to test/discuss this with doug
            new_conn.BeginReceive(SendCallback, new Tuple<StringSocket, string>(new_conn, (temp_id + count.ToString())));
          
       //     playerList.Enqueue(new Tuple<StringSocket, String>(new_conn, temp_id + count.ToString()));
           // playerList.Enqueue(Tuple.Create())  DELETEME
            count += 1;
        }

        /// <summary>
        /// callback for the initial begin_receive stringsocket call
        /// </summary>
        private void SendCallback(string s, Exception e, object payload)
        {

            char[] delim = { ' ' };
            string[] msg = s.Split(delim);
            
            switch(msg[0])
            {
                case ("PLAY"):
                    {
                        int temp = count - 1;
                        Tuple<StringSocket, string> load = (Tuple<StringSocket, string>)payload;
                        if (!(temp_id + count.ToString()).Equals(load.Item2))
                        {
                            Console.WriteLine("Callback Payload is F'd up!!");
                            return;
                        }
                        //now get the name
                        string name = "";
                        int msg_count = 0;
                        foreach (string ss in msg)  //this gurantees if they gave us a name with spaces we get all
                        {
                            if (msg_count == 0)
                            {
                                msg_count += 1;
                                continue;
                            }
                            name += ss;
                            name += " ";
                        }
                        name = name.Substring(0, name.Length - 1);  //trim off last space
                        //finally, enqueue the name and stringsocket for further play
                        //TODO, check queue to see if anyone is in it already, if so, then partner them up                       
                        Tuple<StringSocket, string> tempy = new Tuple<StringSocket, string>(load.Item1, name);
                        if (playerList.Count > 0) //should only ever have one..
                        {
                            lock (playerList)
                            {
                               load =  playerList.Dequeue(); //we don't need load anymore, so let's use that
                            }
                            BoggleGame new_game = new BoggleGame(tempy, load, gameTime); //offload them to a game object
                            //pair the players to a game
                            player_game_map.Add(tempy.Item2, new_game);
                            player_game_map.Add(load.Item2, new_game);
                            //by this point, the game will have initialized and the start game commands sent via the new_game
                            //so initialize some begin recieves to wait for words to come in from the socket
                            tempy.Item1.BeginReceive(SendCallback, tempy.Item2);
                            load.Item1.BeginReceive(SendCallback, load.Item2);
                        }
                        else
                        {
                            lock (playerList)
                            {
                                playerList.Enqueue(tempy);
                            }
                        }
                        break;
                    }
                case ("WORD"):
                    {
                        //we don't have to check payload, this is only initialized once the players have been added to the dict
                       //get the game associated with the payload (player name)
                        BoggleGame g;
                        try
                        {
                            BoggleGame g = player_game_map[(String)payload];
                        }
                        catch (KeyNotFoundException) //no player exists, ignore 
                            {
                                ignore_msg("WORD");
                            }
                        // if we get to here, the game was found from the player, so now we have the object to increment or decrement the score
                        //test to see if the word had a space in it, if so throw an exception
                        if (msg.Length != 2)
                        {
                            //TODO: make a special exception or something for this
                            throw new NotImplementedException();
                        }
                        //if we get here, we test the words validity against the dictionary
                        if(isValidWord(msg[1]))
                        {
                            g.increment_score((String)payload);
                        }
                        else
                        {
                            g.decrement_score((String)payload);
                        }
                        g.broadcastGameScore();
                       break;
                    }
                default:   //call from the client is incorrect
                    {
                        //not equal to anything
                        //TODO : send through the stringsocket an ignore
                        Console.WriteLine("beginPlayCallback invalid payload (not equal to play)");
                        ignore_msg(s);
                        break;
                    }
            }
        }  //end sendcallback method

        /// <summary>
        /// This function will send a message through the stringsocket telling the client that we are 
        /// ignoring the message that they sent, as it came at an incorrect time or something like that
        /// </summary>
        /// <param name="msg"></param>
        private void ignore_msg(string msg)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// method to check the word in the dictionary and return whether or not it exists
        /// </summary>
        /// <param name="word">word to check against the dictionary</param>
        /// <returns></returns>
        private bool isValidWord(string word)
        {
            throw new NotImplementedException();
            
        }

    }  //end of class
} //end of namespace
