using System;
using System.IO;
using System.Threading.Tasks;

namespace VkScript_BD
{
    class Program
    {
        public static void Log(string Message)
        {
            File.AppendAllText("..\\..\\..\\log.txt", Message);
        }

        static async Task Main(string[] args)
        {
            try
            {
                VkApi VkApi = new VkApi(81981776, "87e937f094f8a32f9f0c0e1f61e8a13c5edb98a4b2c404ecc75d897cfb51e86fe3509eacc1707be9f4ec0", "5.126");
                await VkApi.MakeFinalRequest(VkApi.GetFinalMessage()); 
            }
            catch (Exception Exception) { Log("\n" + DateTime.Now.ToString() + " - " + Exception.Message + "\n" + Exception.StackTrace + "\n"); }
        }
    }
}
