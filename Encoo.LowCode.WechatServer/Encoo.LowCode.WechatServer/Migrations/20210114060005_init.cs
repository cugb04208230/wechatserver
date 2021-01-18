using Microsoft.EntityFrameworkCore.Migrations;

namespace Encoo.LowCode.WechatServer.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WechatCaches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WechatCaches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WechatPermanentCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorpId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuiteId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PermanentCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WechatPermanentCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WechatSessionInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Corpid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Userid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionKey = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WechatSessionInfos", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WechatCaches");

            migrationBuilder.DropTable(
                name: "WechatPermanentCodes");

            migrationBuilder.DropTable(
                name: "WechatSessionInfos");
        }
    }
}
