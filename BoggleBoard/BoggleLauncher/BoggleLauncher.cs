using System; 
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using BB;


namespace BB
{
    class BoggleLauncher
    {
        static void Main(string[] args)
        {
           //TODO: fix so that args are coming through as opposed to below line which defaults
            //TODO: thread these
            new BoggleServer(2000,new String[]{"Boggle_Client","200"});
            new Thread(() =>  BoggleView.Main()).Start();
            new Thread(() => BoggleView.Main()).Start();
        }
    }
}
