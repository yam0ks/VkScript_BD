using System.Collections.Generic;

namespace VkScript_BD
{
    public class UsersInfo
    {
        public ResponseInfo response { get; set; }

        public class ResponseInfo
        {
            public ItemsInfo[] items { get; set; }
        }

        public class ItemsInfo
        {
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string id { get; set; }
            public string photo_200 { get; set; }
        }

        public List<ItemsInfo> GetUsers()
        {
            List<ItemsInfo> Users = new List<ItemsInfo>();

            if (response == null || response.items.Length == 0)
                return null;

            foreach (var Item in response.items)
            {
                Users.Add(Item);
            }

            return Users;
        }
    }
}