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
        private TcpListener server_receiver;

        private TcpClient server_sender;

        private Queue<Tuple<StringSocket, string>> playerList;

        private System.Text.UTF8Encoding cod = new System.Text.UTF8Encoding();

        private string temp_id = "user_id_";  //temp placeholder.. ZERO BASED

        private int count = 0;

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
            new_conn.BeginReceive(beginPlayCallback, new Tuple<StringSocket, string>(new_conn, (temp_id + count.ToString())));
          
       //     playerList.Enqueue(new Tuple<StringSocket, String>(new_conn, temp_id + count.ToString()));
           // playerList.Enqueue(Tuple.Create())  DELETEME
            count += 1;
        }

        /// <summary>
        /// callback for the initial begin_receive stringsocket call
        /// </summary>
        private void beginPlayCallback(string s, Exception e, object payload)
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
                        foreach (string ss in msg)  //this gurantees if they gave us a name with spaces we get all
                        {
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
                            BoggleGame new_game = new BoggleGame(tempy, load); //offload them to a game object
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
            }


        }
    }
}
