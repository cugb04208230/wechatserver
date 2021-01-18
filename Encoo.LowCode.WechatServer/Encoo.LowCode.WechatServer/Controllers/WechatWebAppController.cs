using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Controllers
{
    public class WechatWebAppController : Controller
    {

        public async Task<IActionResult> Home()
        {
            return View();
        }
    }
}
