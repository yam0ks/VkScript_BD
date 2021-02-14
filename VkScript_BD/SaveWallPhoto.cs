namespace VkScript_BD
{
    public class SaveWallPhoto
    {
        public ResponseInfo[] response { get; set; }

        public class ResponseInfo
        {
            public string id { get; set; }
            public string owner_id { get; set; }
        }
    }
}