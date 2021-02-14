using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http;
using System.Threading.Tasks;

namespace VkScript_BD
{
    class VkApi
    {
        readonly int Group_Id;
        readonly string Version;
        readonly string Access_Token;
        readonly int Owner_ID;
        readonly Images Images;

        public VkApi(int Group_ID, string Access_Token, string Version)
        {
            Images = new Images();
            this.Group_Id = Group_ID;
            this.Version = Version;
            this.Access_Token = Access_Token;
            Owner_ID = -Group_ID;
        }

        public string MakeGetRequest(string URL)
        {
            HttpWebRequest HttpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
            HttpWebResponse HttpWebResponse = (HttpWebResponse)HttpWebRequest.GetResponse();
            string Response;
            using (StreamReader Streamreader = new StreamReader(HttpWebResponse.GetResponseStream()))
            {
                Response = Streamreader.ReadToEnd();
            }
            return Response;
        }

        public  void MakePostRequest(string Owner_ID, string ID, string message)
        {
            WebRequest Request = WebRequest.Create("https://api.vk.com/method/wall.post");
            Request.Method = "POST";
            string PostData = $"v={Version}&access_token={Access_Token}&owner_id={this.Owner_ID.ToString()}&from_group=1&message={message}&attachments=photo{Owner_ID}_{ID}";
            byte[] ByteArray = Encoding.UTF8.GetBytes(PostData);
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.ContentLength = ByteArray.Length;
            Stream DataStream = Request.GetRequestStream();
            DataStream.Write(ByteArray, 0, ByteArray.Length);
            DataStream.Close();
            Request.GetResponse();
        }

        static string ToString(UsersInfo.ItemsInfo Obj)
        {
            string Format = $"[id{Obj.id}|{Obj.first_name} {Obj.last_name}]";
            return Format;
        }

        public List<UsersInfo.ItemsInfo> GetBirthdayUsers(int Offset)
        {
            int Index = DateTime.Today.ToString("d.M").IndexOf('.');
            string Day = DateTime.Today.ToString("d.M").Substring(0, Index);
            string Month = DateTime.Today.ToString("d.M").Substring(Index + 1);

            string URL = $"https://api.vk.com/method/users.search?v={Version}&group_id={Group_Id}&offset=";
            URL += $"{Offset.ToString()}&birth_day={Day}&birth_month={Month}&has_photo=0&fields=photo_200&count=1000&access_token={Access_Token}";

            UsersInfo Users = JsonConvert.DeserializeObject<UsersInfo>(MakeGetRequest(URL));

            return Users.GetUsers();
        }

        public string MainMethod(string FileName, ImageFormat Format)
        {
            string Message = "🎉 Наши именинники сегодня. Поздравляем! 🎂 \n \n";
            int OffSet = 0;
            List<UsersInfo.ItemsInfo> BirthdayUsers = new List<UsersInfo.ItemsInfo>();

            while (GetBirthdayUsers(OffSet) != null)
            {
                BirthdayUsers.AddRange(GetBirthdayUsers(OffSet));
                OffSet += 1000;
            }

            for (int i = 0; i < BirthdayUsers.Count; i++)
            {
                if (i != 0)
                    Message += ", ";

                Message += ToString(BirthdayUsers[i]);
            }

            WebClient WebClient = new WebClient();
            Bitmap Bitmap;

            for (int i = 0; i < BirthdayUsers.Count; i++)
            {
                if (BirthdayUsers[i].photo_200 == "https://vk.com/images/camera_200.png")
                    continue;

                Stream Stream = WebClient.OpenRead(BirthdayUsers[i].photo_200);
                Bitmap = new Bitmap(Stream);

                if (Bitmap != null)
                {
                    Bitmap.Save(FileName + $"{i}.jpeg", Format);
                }

                Bitmap.Dispose();

                Stream.Flush();
                Stream.Close();
            }

            WebClient.Dispose();

            return Message;
        }

        public string GetFinalMessage()
        {
            string Message = MainMethod("..\\..\\..\\Images\\image", ImageFormat.Jpeg);

            DirectoryInfo Directory = new DirectoryInfo("..\\..\\..\\Images");
            if (Directory != null)
            {
                FileInfo[] Files = Directory.GetFiles();
                Images.CombineImages(Files);
            }

            return Message;
        }

        public async Task MakeFinalRequest(string message)
        {
            string UploadServerURL = $"https://api.vk.com/method/photos.getWallUploadServer?v={Version}&access_token={Access_Token}&group_id={Group_Id.ToString()}";
            WallUploadServer UploadServer = JsonConvert.DeserializeObject<WallUploadServer>(MakeGetRequest(UploadServerURL));

            byte[] ImgData = System.IO.File.ReadAllBytes("..\\..\\..\\Images\\FinalImage.jpeg");
            var ImageContent = new ByteArrayContent(ImgData);
            var MultipartContent = new MultipartFormDataContent();
            MultipartContent.Add(ImageContent, "photo", "..\\..\\..\\Images\\FinalImage.jpeg");

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string Response;
            using (var HttpClient = new HttpClient())
            {
                var HttpResponse = await HttpClient.PostAsync(UploadServer.response.upload_url, MultipartContent);
                var HttpContent = await HttpResponse.Content.ReadAsByteArrayAsync();
                Response = Encoding.GetEncoding(1251).GetString(HttpContent, 0, HttpContent.Length);
            }

            UploadResponse UploadResponse = JsonConvert.DeserializeObject<UploadResponse>(Response);
            string SaveWallPhotoUrl = $"https://api.vk.com/method/photos.saveWallPhoto?v={Version}&access_token={Access_Token}&group_id={Group_Id.ToString()}&photo={UploadResponse.photo}&server={UploadResponse.server}&hash={UploadResponse.hash}";
            SaveWallPhoto WallPhoto = JsonConvert.DeserializeObject<SaveWallPhoto>(MakeGetRequest(SaveWallPhotoUrl));

            MakePostRequest(WallPhoto.response[0].owner_id, WallPhoto.response[0].id, message);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            DirectoryInfo DirInfo = new DirectoryInfo("..\\..\\..\\Images");
            foreach (FileInfo File in DirInfo.GetFiles())
            {
                File.Delete();
            }
        }
    }
}
