namespace VkScript_BD
{
    public class WallUploadServer
    {
        public ResponseInfo response { get; set; }

        public class ResponseInfo
        {
            public string upload_url { get; set; }
        }
    }
}