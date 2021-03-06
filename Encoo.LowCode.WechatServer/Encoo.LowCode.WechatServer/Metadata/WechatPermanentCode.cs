﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Metadata
{
    public class WechatPermanentCode
    {

        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        public string CorpId { set; get; }

        public string SuiteId { set; get; }

        public string PermanentCode { set; get; }
    }
}
