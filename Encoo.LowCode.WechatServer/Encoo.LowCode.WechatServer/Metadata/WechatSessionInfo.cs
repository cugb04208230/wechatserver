using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Metadata
{
    public class WechatSessionInfo
    {
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }
        public string Corpid { set; get; }

        public string Userid { set; get; }

        public string SessionKey { set; get; }
    }
}
