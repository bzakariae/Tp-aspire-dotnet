using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyDotNetApp.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Make = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    Class = table.Column<string>(type: "text", nullable: false),
                    PricePerDay = table.Column<decimal>(type: "numeric", nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rentals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CarId = table.Column<int>(type: "integer", nullable: false),
                    CarId1 = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    UserId1 = table.Column<int>(type: "integer", nullable: true),
                    RenterName = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rentals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rentals_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rentals_Cars_CarId1",
                        column: x => x.CarId1,
                        principalTable: "Cars",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rentals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rentals_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    RelatedRentalId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Rentals_RelatedRentalId",
                        column: x => x.RelatedRentalId,
                        principalTable: "Rentals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RentalDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RentalId = table.Column<int>(type: "integer", nullable: false),
                    DocumentType = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FileContent = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalDocuments_Rentals_RentalId",
                        column: x => x.RentalId,
                        principalTable: "Rentals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "Id", "Class", "ImageUrl", "IsAvailable", "Make", "Model", "PricePerDay" },
                values: new object[,]
                {
                    { 1, "Supercar", "https://cdn.dicklovett.co.uk/uploads/used_stock_image/1_2507021_e.jpg?v=1758203924", true, "Ferrari", "Roma", 1200m },
                    { 2, "Supercar", "https://www.lamborghini.com/sites/it-en/files/DAM/lamborghini/facelift_2019/model_detail/huracan/evo/connected_car/hura_connect_over_03_m.jpg", true, "Lamborghini", "Huracán EVO", 1500m },
                    { 3, "Supercar", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSKg_qn5PLG9GS20nutnZqVVue14ZlkSo2-oQ&s", true, "McLaren", "720S", 1300m },
                    { 4, "Sports", "https://www.kbb.com/wp-content/uploads/2020/04/2021-porsche-911-turbo-s-coupe-front.jpg", true, "Porsche", "911 Turbo S", 950m },
                    { 5, "Grand Tourer", "https://images.caricos.com/a/aston_martin/2017_aston_martin_db11_lightning_silver/images/2560x1440/2017_aston_martin_db11_lightning_silver_130_2560x1440.jpg", true, "Aston Martin", "DB11", 1100m },
                    { 6, "Luxury", "https://www.bentleymotors.com/content/dam/bm/websites/bmcom/bentleymotors-com/models/25my/25my-gt/Gallery%202.jpg/_jcr_content/renditions/original.image_file.1440.810.file/Gallery%202.jpg", true, "Bentley", "Continental GT", 850m },
                    { 7, "Ultra Luxury", "https://editorial.pxcrush.net/carsales/general/editorial/rolls-royce-wraith-black-arrow-exterior-06.jpg?height=682&width=1024", true, "Rolls-Royce", "Wraith", 1800m },
                    { 8, "Hypercar", "https://fortune.com/img-assets/wp-content/uploads/2016/02/01_chiron_front_web.jpg", true, "Bugatti", "Chiron", 3500m },
                    { 9, "Luxury Sedan", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTvgP7ERASZM6mX5tulnpIkSJkyir4LUJd7uA&s", true, "Mercedes‑Benz", "S‑Class", 600m },
                    { 10, "Luxury Sedan", "https://first-gt.com/wp-content/uploads/2021/05/bmw-7-series.jpg", true, "BMW", "7 Series", 580m },
                    { 11, "Luxury Sedan", "https://www.latribuneauto.com/media/cache/resolve/vehicule_slider/photos/AUDI/A8/AUDI-A8-BE-22-119947/01%20Audi%20A8%202022%20Exterieur%203%204%20Avant.jpg", true, "Audi", "A8", 550m },
                    { 12, "Luxury Sedan", "https://hips.hearstapps.com/hmg-prod/images/2025-lexus-ls-500-f-sport-awd-101-67e30ab19aa60.jpg?crop=0.635xw:0.476xh;0.274xw,0.404xh&resize=1200:*", true, "Lexus", "LS 500", 500m },
                    { 13, "Electric", "https://octane.rent/wp-content/uploads/2024/11/tesla_model_s_black_01.jpg", true, "Tesla", "Model S Plaid", 400m },
                    { 14, "Electric", "https://lucidmotors.com/s3fs-public/2022-10/slideshow-00-regular.webp", true, "Lucid", "Air Dream Edition", 420m },
                    { 15, "Electric Sports", "https://cms-assets.autoscout24.com/uaddx06iwzdz/4ShcqL35AjjiQepEF2N6Y3/21f6bd40209e3016673b970d9143464d/porsche_taycan_732.jpeg?w=1100", true, "Porsche", "Taycan Turbo S", 480m },
                    { 16, "Electric Luxury", "https://mediacloud.carbuyer.co.uk/image/private/s--X-WVjvBW--/f_auto,t_content-image-full-desktop@1/v1723215907/autoexpress/2024/08/BMW%20i7%20eDrive50%20M%20Sport%202024%20UK-22.jpg", true, "BMW", "i7", 450m },
                    { 17, "Electric", "https://c0.lestechnophiles.com/www.numerama.com/wp-content/uploads/2025/08/essai-tesla-model-3-performance-2025-12.jpg?resize=1600,900&key=190b7a19&watermark", true, "Tesla", "Model 3 Performance", 250m },
                    { 18, "SUV", "https://www.dealndrive.com/Content/images/upload/28669-full.jpg", true, "Range Rover", "Sport", 550m },
                    { 19, "SUV", "https://autobunny-docs.s3.ca-central-1.amazonaws.com/945/car/forsale/images/2024/02/pic-b470b1283d251a143bc46acd4f1036a5.jpg", true, "Mercedes‑Benz", "GLE", 400m },
                    { 20, "SUV", "https://bmw.scene7.com/is/image/BMW/bmw-x-series-overview-x5-g05?wid=2560&hei=2560", true, "BMW", "X5", 380m },
                    { 21, "SUV", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQkmYnqx2o6zA7CCuyzBrdkyFkwxcAA7RDAbQ&s", true, "Audi", "Q8", 420m },
                    { 22, "Super SUV", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSU_8P8oHyUfeGjls8ax7mRIlZGEufIyQQCvg&s", true, "Lamborghini", "Urus", 1200m },
                    { 23, "Luxury SUV", "https://www.bentleymedia.com/image/dda38b10-c8f6-41b6-af44-e17e820ced39?anchor=middlecenter&bgcolor=Transparent&borderWidth=0&crop=0%2C0%2C0%2C0&width=460", true, "Bentley", "Bentayga", 950m },
                    { 24, "SUV", "https://www.turbo.fr/sites/default/files/2019-09/maserati-levante-trofeo-essai.png", true, "Maserati", "Levante", 600m },
                    { 45, "Luxury 4x4", "https://ik.imagekit.io/vyro/public/autofox-api-mapped-to-vehicles/b81f83f332e9a3fce6d255c0f45f2103033e7f706068f23d967c85d99b593635.jpg", true, "Mercedes-Benz", "G-Class", 950m },
                    { 51, "Supercar", "https://cdn.ferrari.com/cms/network/media/img/resize/5d26fdb7c3f9ec0af6475619-01_fb_ppl_intro_lp3lhwq8?width=1080", true, "Ferrari", "F8 Tributo", 1400m },
                    { 52, "Supercar", "https://www.lamborghini.com/sites/it-en/files/DAM/lamborghini/facelift_2019/model_detail/aventador/s/s/s-3_M.jpg", true, "Lamborghini", "Aventador", 2000m },
                    { 54, "Sports", "https://upload.wikimedia.org/wikipedia/commons/3/32/Porsche_718_Cayman_%28MSP17%29.jpg", true, "Porsche", "718 Cayman GT4", 900m },
                    { 55, "Sports", "https://www.largus.fr/images/styles/max_1300x1300/public/2024-02/aston-martin-vantage-2024-jaune-avg-mk.jpg?itok=EhIDUYJs", true, "Aston Martin", "Vantage", 1100m },
                    { 56, "Electric SUV", "https://images.caradisiac.com/logos/2/0/2/6/272026/S8-essai-tesla-model-y-performance-2022-on-en-a-pour-son-argent-196566.jpg", true, "Tesla", "Model Y Performance", 350m },
                    { 57, "Electric SUV", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQNn-A9iJfxvdmAV4gdDmORCazOtdvVadTxfw&s", true, "BMW", "iX", 400m },
                    { 58, "Electric Sports", "https://www.automobile-magazine.fr/asset/cms/1200x750/226448/config/173544/audi-rs-e-tron-gt-av.jpg", true, "Audi", "e-tron GT", 450m },
                    { 59, "Electric Sports", "https://www.hoonited.com/wp-content/uploads/2024/12/Porsche-Taycan-4S-2024-15.jpg", true, "Porsche", "Taycan 4S", 420m },
                    { 61, "SUV", "https://hips.hearstapps.com/hmg-prod/images/2020-audi-q7-134-1583273378.jpg?crop=0.891xw:0.751xh;0.0782xw,0.249xh&resize=2048:*", true, "Audi", "Q7", 400m },
                    { 62, "SUV", "https://www.largus.fr/images/styles/max_1300x1300/public/images/bmw-x6-2019-vue-av.jpg?itok=QvwSVz1Z", true, "BMW", "X6", 450m },
                    { 63, "Luxury SUV", "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c6/Mercedes-Benz_X_167_GLS580_IAA_2019_JM_0265.jpg/330px-Mercedes-Benz_X_167_GLS580_IAA_2019_JM_0265.jpg", true, "Mercedes-Benz", "GLS", 550m },
                    { 64, "SUV", "https://upload.wikimedia.org/wikipedia/commons/8/88/Range_Rover_Velar.jpg", true, "Range Rover", "Velar", 480m },
                    { 65, "Luxury SUV", "https://www.largus.fr/images/styles/max_1300x1300/public/images/maserati-levante-trofeo-24.jpg?itok=Lr0Hjp4N", true, "Maserati", "Levante Trofeo", 600m },
                    { 66, "Luxury Sedan", "https://www.mbusa.com/content/dam/mb-nafta/us/myco/my26/e-class/e-sedan/class-page/2026-E-SEDAN-HC-D.jpg", true, "Mercedes-Benz", "E-Class", 350m },
                    { 67, "Luxury Sedan", "https://cdn.motor1.com/images/mgl/KrpgM/s3/2017-bmw-5-series.jpg", true, "BMW", "5 Series", 340m },
                    { 68, "Luxury Sedan", "https://images.ctfassets.net/uaddx06iwzdz/7aPSWTuHLGAcn6ps5BniGs/aa11e9bf7f6d7d89365f4481522eaa89/audi-a6-front.jpg", true, "Audi", "A6", 330m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RelatedRentalId",
                table: "Notifications",
                column: "RelatedRentalId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalDocuments_RentalId",
                table: "RentalDocuments",
                column: "RentalId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_CarId",
                table: "Rentals",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_CarId1",
                table: "Rentals",
                column: "CarId1");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_UserId",
                table: "Rentals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_UserId1",
                table: "Rentals",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "RentalDocuments");

            migrationBuilder.DropTable(
                name: "Rentals");

            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
