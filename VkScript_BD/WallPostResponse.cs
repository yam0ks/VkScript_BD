using System;
using System.Collections.Generic;
using System.Text;

namespace VkScript_BD
{
    public class WallPostResponse
    {
        public ResponseInfo response { get; set; }

        public class ResponseInfo
        {
            public string post_id { get; set; }
        }
    }
}
