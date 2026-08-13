using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace bretts_services.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP TABLE IF EXISTS [dbo].[AttributeValues];
                DROP TABLE IF EXISTS [dbo].[InventoryItemAttributeValues];
                DROP TABLE IF EXISTS [dbo].[InventoryItemDefinitionAttributes];
                DROP TABLE IF EXISTS [dbo].[InventoryItemDefinitionComponents];
                DROP TABLE IF EXISTS [dbo].[InventoryItemInstances];
                DROP TABLE IF EXISTS [dbo].[AttributeDefinitions];
                DROP TABLE IF EXISTS [dbo].[InventoryItemAttributeDefinitions];
                DROP TABLE IF EXISTS [dbo].[InventoryItemDefinitions];
                DROP TABLE IF EXISTS [dbo].[RoleUser];
                DROP TABLE IF EXISTS [dbo].[Logs];
                DROP TABLE IF EXISTS [dbo].[Users];
                DROP TABLE IF EXISTS [dbo].[Roles];
                DELETE FROM [dbo].[__EFMigrationsHistory];
                """);

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeStamp = table.Column<DateTime>(type: "datetime", nullable: true),
                    Exception = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogEvent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceContext = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    ServerName = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Environment = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Password = table.Column<byte[]>(type: "varbinary(64)", maxLength: 64, nullable: false),
                    Salt = table.Column<byte[]>(type: "varbinary(64)", maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "RoleUser",
                columns: table => new
                {
                    RolesRoleID = table.Column<long>(type: "bigint", nullable: false),
                    UsersUserID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleUser", x => new { x.RolesRoleID, x.UsersUserID });
                    table.ForeignKey(
                        name: "FK_RoleUser_Roles_RolesRoleID",
                        column: x => x.RolesRoleID,
                        principalTable: "Roles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleUser_Users_UsersUserID",
                        column: x => x.UsersUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleID", "Name", "RoleGuid" },
                values: new object[,]
                {
                    { 1L, "Admin", new Guid("cdf2beff-ea73-4d8b-9fe8-33818e52776f") },
                    { 2L, "User", new Guid("111224ad-f6a4-4ca1-ade2-2e6ab407d8e8") }
                });

            migrationBuilder.Sql(
                """
                SET IDENTITY_INSERT [dbo].[Users] ON;
                INSERT INTO [dbo].[Users] ([UserID], [UserGuid], [Email], [Password], [Salt], [DisplayName], [Phone], [CreatedAt]) VALUES (1, 'A1270C3B-C081-4CDD-B0F9-AC773EA8B3A0', N'adminonly@brettdrake.org', 0xEA6088E15AB0DBE5BFE76B09DB6ABACCB2579E71B574E9D95D75411C4897791FC6E474F623F27C4DCA560322480A6974CDD5AB898CA561249DFED876FFD62F5E, 0x8F76675FC0358A0733D7552765DB68BBFA382362652E466DA064BFD54DA2DF45128BCE7569AA9A9E7564144D4BF85EB8269E36316019DC4A670DF791F9B30F2E, N'Admin Only User', NULL, '2025-12-19T14:30:52.9633333');
                INSERT INTO [dbo].[Users] ([UserID], [UserGuid], [Email], [Password], [Salt], [DisplayName], [Phone], [CreatedAt]) VALUES (2, '968E69EE-DDA0-47BD-84DD-BBE85E23C036', N'useronly@brettdrake.org', 0x3B14D3706B1278F6D2372B35F4323988176DF680762BB93E3DF0B53DF7FC64A64FD6975974234797E1F71D359D65EEB47FBA6AC0C3B97FFE2D50C6F0EB6BA0FB, 0x3051AAE6197E18635AE478152C799A382ABF208C6B5F92530E7B7E3D73EDB866835977004C6476B063D8C19F06071F9D04BD738253793A2662E6CCBAC9317889, N'User Only User', NULL, '2025-12-19T14:33:07.6800000');
                INSERT INTO [dbo].[Users] ([UserID], [UserGuid], [Email], [Password], [Salt], [DisplayName], [Phone], [CreatedAt]) VALUES (10002, '7DCA43C6-BF57-4D32-B0E3-A8E29C3459CD', N'recordstuff@hotmail.com', 0xC06407B3B1040DD98A63B3D7D43ABFFF8A8A2FEF6EFD36C39E477ED6DC43E8A767E965173293A4531CB216508B60AA8F214936CD2DE064DFC38C79E939D21A82, 0x1CFCBD5332A3DCCFA5C76C225133ECEFED6267F216B73F99104E7776B4AB83F7305890FB10D84752C7A9DD60428B10BA4C43696C79435DDD8FAB2FE0007E3912, N'Brett Drake', N'', '2026-02-21T14:33:17.7433333');
                INSERT INTO [dbo].[Users] ([UserID], [UserGuid], [Email], [Password], [Salt], [DisplayName], [Phone], [CreatedAt]) VALUES (20002, '9D65569C-4D49-47D3-8F6B-7A255FA2D4B8', N'bigdog@fake.org', 0xA4DBD32E361D824D57B6DD67508DF5B4C19F8285C7F7297987449D46AE8BF5783D6352BE3439AD8A63D5DFB72F71EB1BAD40A6DCC15FC06F091F3E76A1268088, 0x012B834E61B3F921C2BEA4B07AAEBFD15F153F1D30DC3637FA83F89EADE1A86263BF2D05FFB7CE92FE53DEC94075D8B522E2A314AB1FC9A61586C3EF0CD35FF8, N'Big Dog', N'3375550101', '2026-06-30T12:56:01.4133333');
                INSERT INTO [dbo].[Users] ([UserID], [UserGuid], [Email], [Password], [Salt], [DisplayName], [Phone], [CreatedAt]) VALUES (20003, 'A772C987-CBEA-4552-A1FD-BFEBB04427DE', N'crazy.eddie@fireworks.com', 0x17771983A302D815EE10CDB9DD1DBE5410AF6248914B4635E035466CF489DC3D3CBDB3B66FC012088B5020F58C05826094E258010C57F470C77A3550D41D863E, 0x814629AFBB18ED24CD43363659A8440B62BA25FDAC71273830239EB81789A2AC177BD588135218CCAC3B96A5A3CB3A0DC47F57A8BD7D8F92CD86B1A779659527, N'Crazy Eddie', N'', '2026-06-30T12:57:27.0300000');
                INSERT INTO [dbo].[Users] ([UserID], [UserGuid], [Email], [Password], [Salt], [DisplayName], [Phone], [CreatedAt]) VALUES (30002, '0E2A7487-EEE5-4B91-AFEB-B12A5C7B310F', N'rory@starshollow.com', 0x66FD54B4A592086E288126530E78461EE024AEC490786C6B110B70FDE7606A1ED42402665FDB0F8D59DC1014ED274B4576707A85AA6715B46ED7EB668A4612CF, 0xF3731CC3F289526729012460DA5E773A5297A171879CBF0B05E6CCFEBB88917D9E8FCFF347C73D618F972A078854C61E55D41835D850DDC8CF40144480C29F83, N'Rory Gilmore', N'', '2026-08-07T01:44:45.6233333');
                INSERT INTO [dbo].[Users] ([UserID], [UserGuid], [Email], [Password], [Salt], [DisplayName], [Phone], [CreatedAt]) VALUES (30003, '9C9A1646-9660-40B3-BA65-F6F176B030C9', N'lorelai@starshollow.com', 0xDE13DE47BBC3BB09AB8C46A3607A47128F719BB5F62989EFC6682A7DD438AD8CBF3A7C7F9540AE17B284DE8138652917CA48280B19FE45E511B4392A5BADD42B, 0x8AEC3272078E4F0B08D94E381BFAC2B6C946A10194138E05A606DF5F985649CB94E52738824E35CE26DF392865906520453DD132FD5FBBAF5E3ABB74BFC92226, N'Lorelai Gilmore', N'', '2026-08-07T02:39:25.0100000');
                INSERT INTO [dbo].[Users] ([UserID], [UserGuid], [Email], [Password], [Salt], [DisplayName], [Phone], [CreatedAt]) VALUES (30004, 'F0A40365-142C-44B4-9F40-B4559FB75B85', N'adminanduser@brettdrake.org', 0xB3BB512A45341F9AB15B7CC6CC77B25C4599F41748548FA9C2BECF9E29ADEA3B3A77D470054C171E6186A3B0556375D5704D6992DD1C3732D148F3B69537A318, 0xAC70390804D8A96C29188D80680B525495C97FADFFBDE120A0CC6F24504CE2CFD8677C3185E928A3EBCC584E125605D1213FFACDF1FB28C5FF02FD8E456AA016, N'Admin And User', N'', '2026-08-07T13:36:03.6566667');
                INSERT INTO [dbo].[Users] ([UserID], [UserGuid], [Email], [Password], [Salt], [DisplayName], [Phone], [CreatedAt]) VALUES (30005, 'ACC3EB02-021F-494D-9F57-38E75BBB3A41', N'richard@castleinvestications.com', 0x09C967D9379EADDF7962888F9D324C233CFD615010D024053A91CACA5BB5416BF0B93043483C3EA084A4079646BC8A04B512D6A9504BEC700AAE4121C49C7942, 0x0566F27AD2D83A30E0925FC27BB777DF38CB6E825FDB2367245B765F129ECA9449D0F296A210EB10CA018F19DE274F3E3ADDA4EFC57EE6AE0ABBB3D2018AEC74, N'Richard Castle', N'', '2026-08-07T14:59:46.5166667');
                INSERT INTO [dbo].[Users] ([UserID], [UserGuid], [Email], [Password], [Salt], [DisplayName], [Phone], [CreatedAt]) VALUES (30006, 'AE703ADA-6FE4-4AC3-B56C-9362FD9A3286', N'doris@dd.com', 0xC4E2CCCAA8C5B6FC1AC721E2CEF418C89DA46544455477963C0B18FDDCBA03E5C6FDB5E3CFCBFA03F2E3B468E698960D85643F8B239F4EA0B62A5224E875517D, 0xB423BEE8C39045D775DE6B83194BB7B40678F8FADAD7114D6F1E233D9451D3EEA550A6BFA04DD354572BD2470E3F8377D08670B9AE3304604E257A6332C88F8C, N'Doris Day', N'', '2026-08-07T15:24:37.9133333');
                INSERT INTO [dbo].[Users] ([UserID], [UserGuid], [Email], [Password], [Salt], [DisplayName], [Phone], [CreatedAt]) VALUES (30008, '2396C2EC-72CC-4CAE-8366-CAFE175DE50A', N'charlie@peanuts.org', 0x651BB2835763B049EB2C27D4574A5F96824D4218FD58A42267CECB9547CF57CCF4E35D3A2B4427239A90C925A3A16739862ACD9F421D795EF993064F5EE05A9A, 0xAC6C7F9804C670FB081D95DD5383249213B0518599CFD9CE29C4C9D94B58C28EC2ADD7E0B13E93DB91FA00EEB631CBE11B66FB35393F2CA66970AD207471F75C, N'Charlie Brown', N'', '2026-08-07T15:48:51.8900000');
                INSERT INTO [dbo].[Users] ([UserID], [UserGuid], [Email], [Password], [Salt], [DisplayName], [Phone], [CreatedAt]) VALUES (30012, '56EEC365-6627-491A-800A-6E3512713CC6', N'test', 0xBD842CE35E61E3A40161B4D6C2EC4A2B7397E27300C0E52B520F608362DA6E49FD9C57E0374D1921310FB0A5AF578ABEEABF409445A983C650D20E32B4790F27, 0x27F62DE1039526C0BDE8E6683C75840603AD9D317E30740593F27ECE5F872B9172D30E779BC329BFCC47628252931C013D565287D7F3B225793624D264A58C18, N'test', N'', '2026-08-08T01:31:21.1533333');
                INSERT INTO [dbo].[Users] ([UserID], [UserGuid], [Email], [Password], [Salt], [DisplayName], [Phone], [CreatedAt]) VALUES (30014, '6B6437E8-1B0C-4A69-9D1D-4E007C508CA0', N'imacreep@radiohead.com', 0x3610FD5DC13C3CF3D67B62D8C0722F15E845F0D50293382E3DCE3EE8035FE0C7515B6B19069C70CA86D9564FC67D859A9A2DE41F3A13411EBF615580E1948A1D, 0x0589C05DBBF8293C873C792BC745C92C286B039A084828B2D3B4753E01D091FC719B06BD6916A96F81020B884A36134F58835310A13D0BB18FB717E7552E1598, N'Creepy User', NULL, '2026-08-08T22:45:50.3733333');
                SET IDENTITY_INSERT [dbo].[Users] OFF;
                """);

            migrationBuilder.InsertData(
                table: "RoleUser",
                columns: new[] { "RolesRoleID", "UsersUserID" },
                values: new object[,]
                {
                    { 1L, 1L },
                    { 2L, 2L },
                    { 1L, 10002L },
                    { 2L, 10002L },
                    { 1L, 20002L },
                    { 2L, 20002L },
                    { 2L, 20003L },
                    { 1L, 30002L },
                    { 2L, 30002L },
                    { 1L, 30003L },
                    { 2L, 30003L },
                    { 1L, 30004L },
                    { 2L, 30004L },
                    { 1L, 30005L },
                    { 2L, 30005L },
                    { 1L, 30006L },
                    { 2L, 30006L },
                    { 1L, 30008L },
                    { 2L, 30008L },
                    { 1L, 30012L },
                    { 2L, 30012L },
                    { 1L, 30014L },
                    { 2L, 30014L }
                });

            migrationBuilder.CreateIndex(
                name: "IX1_Logs",
                table: "Logs",
                column: "TimeStamp");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleUser_UsersUserID",
                table: "RoleUser",
                column: "UsersUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException(
                "This baseline cannot be reversed because applying it permanently deletes inventory and log data.");
        }
    }
}
