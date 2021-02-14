using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace VkScript_BD
{
    class Program
    {
        static List<string> ReadData()
        {
            List<string> Data = new List<string>();

            using (StreamReader sr = new StreamReader("..\\..\\..\\DATA.txt", System.Text.Encoding.Default))
            {
                for(int i = 0; i < 3; i++)
                {
                    Data.Add(sr.ReadLine());
                }
            }

            return Data;
        }

        public static void Log(string Message)
        {
            File.AppendAllText("..\\..\\..\\log.txt", Message);
        }

        static async Task Main(string[] args)
        {
            try
            {
                List<string> Data= ReadData();
                VkApi VkApi = new VkApi(int.Parse(Data[0]), Data[1], Data[2]);
                await VkApi.MakeFinalRequest(VkApi.GetFinalMessage());
            }
            catch (Exception Exception) { Log(DateTime.Now.ToString() + " - " + Exception.Message + "\n" + Exception.StackTrace + "\n\n\n"); }
        }
    }
}
