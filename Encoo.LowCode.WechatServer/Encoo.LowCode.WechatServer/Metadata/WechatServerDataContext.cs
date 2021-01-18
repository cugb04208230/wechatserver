using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Metadata
{
    public class WechatServerDataContext : DbContext
    {
        public WechatServerDataContext(DbContextOptions<WechatServerDataContext> options):base(options)
        {

        }

        public DbSet<WechatSessionInfo> WechatSessionInfos { set; get; }

        public DbSet<WechatCache> WechatCaches { set; get; }

        public DbSet<WechatPermanentCode> WechatPermanentCodes { set; get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
