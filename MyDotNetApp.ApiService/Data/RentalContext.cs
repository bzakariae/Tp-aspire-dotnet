using Microsoft.EntityFrameworkCore;
using LuxuryRental.Api.Models;

namespace LuxuryRental.Api.Data
{
    public class RentalContext : DbContext
    {
        public RentalContext(DbContextOptions<RentalContext> options) : base(options) { }

        public DbSet<Car> Cars => Set<Car>();
        public DbSet<Rental> Rentals => Set<Rental>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<RentalDocument> RentalDocuments => Set<RentalDocument>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.User)      
                .WithMany()            
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Car)     
                .WithMany()              
                .HasForeignKey(r => r.CarId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.RelatedRental)
                .WithMany()
                .HasForeignKey(n => n.RelatedRentalId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<RentalDocument>()
                .HasOne(d => d.Rental)
                .WithMany()
                .HasForeignKey(d => d.RentalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Car>().HasData(
  new Car { Id = 1, Make = "Ferrari", Model = "Roma", Class = "Supercar", PricePerDay = 1200m, IsAvailable = true, ImageUrl = "https://cdn.dicklovett.co.uk/uploads/used_stock_image/1_2507021_e.jpg?v=1758203924" },
new Car { Id = 2, Make = "Lamborghini", Model = "Huracán EVO", Class = "Supercar", PricePerDay = 1500m, IsAvailable = true, ImageUrl = "https://www.lamborghini.com/sites/it-en/files/DAM/lamborghini/facelift_2019/model_detail/huracan/evo/connected_car/hura_connect_over_03_m.jpg" },
new Car { Id = 3, Make = "McLaren", Model = "720S", Class = "Supercar", PricePerDay = 1300m, IsAvailable = true, ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSKg_qn5PLG9GS20nutnZqVVue14ZlkSo2-oQ&s" },
new Car { Id = 4, Make = "Porsche", Model = "911 Turbo S", Class = "Sports", PricePerDay = 950m, IsAvailable = true, ImageUrl = "https://www.kbb.com/wp-content/uploads/2020/04/2021-porsche-911-turbo-s-coupe-front.jpg" },
new Car { Id = 5, Make = "Aston Martin", Model = "DB11", Class = "Grand Tourer", PricePerDay = 1100m, IsAvailable = true, ImageUrl = "https://images.caricos.com/a/aston_martin/2017_aston_martin_db11_lightning_silver/images/2560x1440/2017_aston_martin_db11_lightning_silver_130_2560x1440.jpg" },
new Car { Id = 6, Make = "Bentley", Model = "Continental GT", Class = "Luxury", PricePerDay = 850m, IsAvailable = true, ImageUrl = "https://www.bentleymotors.com/content/dam/bm/websites/bmcom/bentleymotors-com/models/25my/25my-gt/Gallery%202.jpg/_jcr_content/renditions/original.image_file.1440.810.file/Gallery%202.jpg" },
new Car { Id = 7, Make = "Rolls-Royce", Model = "Wraith", Class = "Ultra Luxury", PricePerDay = 1800m, IsAvailable = true, ImageUrl = "https://editorial.pxcrush.net/carsales/general/editorial/rolls-royce-wraith-black-arrow-exterior-06.jpg?height=682&width=1024" },
new Car { Id = 8, Make = "Bugatti", Model = "Chiron", Class = "Hypercar", PricePerDay = 3500m, IsAvailable = true, ImageUrl = "https://fortune.com/img-assets/wp-content/uploads/2016/02/01_chiron_front_web.jpg" },

    // LUXURY SEDANS
    new Car { Id = 9, Make = "Mercedes‑Benz", Model = "S‑Class", Class = "Luxury Sedan", PricePerDay = 600m, IsAvailable = true, ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTvgP7ERASZM6mX5tulnpIkSJkyir4LUJd7uA&s" },
    new Car { Id = 10, Make = "BMW", Model = "7 Series", Class = "Luxury Sedan", PricePerDay = 580m, IsAvailable = true, ImageUrl = "https://first-gt.com/wp-content/uploads/2021/05/bmw-7-series.jpg" },
    new Car { Id = 11, Make = "Audi", Model = "A8", Class = "Luxury Sedan", PricePerDay = 550m, IsAvailable = true, ImageUrl = "https://www.latribuneauto.com/media/cache/resolve/vehicule_slider/photos/AUDI/A8/AUDI-A8-BE-22-119947/01%20Audi%20A8%202022%20Exterieur%203%204%20Avant.jpg" },
    new Car { Id = 12, Make = "Lexus", Model = "LS 500", Class = "Luxury Sedan", PricePerDay = 500m, IsAvailable = true, ImageUrl = "https://hips.hearstapps.com/hmg-prod/images/2025-lexus-ls-500-f-sport-awd-101-67e30ab19aa60.jpg?crop=0.635xw:0.476xh;0.274xw,0.404xh&resize=1200:*" },

    // ELECTRIC
  new Car { Id = 13, Make = "Tesla", Model = "Model S Plaid", Class = "Electric", PricePerDay = 400m, IsAvailable = true, ImageUrl = "https://octane.rent/wp-content/uploads/2024/11/tesla_model_s_black_01.jpg" },
new Car { Id = 14, Make = "Lucid", Model = "Air Dream Edition", Class = "Electric", PricePerDay = 420m, IsAvailable = true, ImageUrl = "https://lucidmotors.com/s3fs-public/2022-10/slideshow-00-regular.webp" },
new Car { Id = 15, Make = "Porsche", Model = "Taycan Turbo S", Class = "Electric Sports", PricePerDay = 480m, IsAvailable = true, ImageUrl = "https://cms-assets.autoscout24.com/uaddx06iwzdz/4ShcqL35AjjiQepEF2N6Y3/21f6bd40209e3016673b970d9143464d/porsche_taycan_732.jpeg?w=1100" },
new Car { Id = 16, Make = "BMW", Model = "i7", Class = "Electric Luxury", PricePerDay = 450m, IsAvailable = true, ImageUrl = "https://mediacloud.carbuyer.co.uk/image/private/s--X-WVjvBW--/f_auto,t_content-image-full-desktop@1/v1723215907/autoexpress/2024/08/BMW%20i7%20eDrive50%20M%20Sport%202024%20UK-22.jpg" },
new Car { Id = 17, Make = "Tesla", Model = "Model 3 Performance", Class = "Electric", PricePerDay = 250m, IsAvailable = true, ImageUrl = "https://c0.lestechnophiles.com/www.numerama.com/wp-content/uploads/2025/08/essai-tesla-model-3-performance-2025-12.jpg?resize=1600,900&key=190b7a19&watermark" },

// SUV / CROSSOVER
new Car { Id = 18, Make = "Range Rover", Model = "Sport", Class = "SUV", PricePerDay = 550m, IsAvailable = true, ImageUrl = "https://www.dealndrive.com/Content/images/upload/28669-full.jpg" },
new Car { Id = 19, Make = "Mercedes‑Benz", Model = "GLE", Class = "SUV", PricePerDay = 400m, IsAvailable = true, ImageUrl = "https://autobunny-docs.s3.ca-central-1.amazonaws.com/945/car/forsale/images/2024/02/pic-b470b1283d251a143bc46acd4f1036a5.jpg" },
new Car { Id = 20, Make = "BMW", Model = "X5", Class = "SUV", PricePerDay = 380m, IsAvailable = true, ImageUrl = "https://bmw.scene7.com/is/image/BMW/bmw-x-series-overview-x5-g05?wid=2560&hei=2560" },
new Car { Id = 21, Make = "Audi", Model = "Q8", Class = "SUV", PricePerDay = 420m, IsAvailable = true, ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQkmYnqx2o6zA7CCuyzBrdkyFkwxcAA7RDAbQ&s" },
new Car { Id = 22, Make = "Lamborghini", Model = "Urus", Class = "Super SUV", PricePerDay = 1200m, IsAvailable = true, ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSU_8P8oHyUfeGjls8ax7mRIlZGEufIyQQCvg&s" },
new Car { Id = 23, Make = "Bentley", Model = "Bentayga", Class = "Luxury SUV", PricePerDay = 950m, IsAvailable = true, ImageUrl = "https://www.bentleymedia.com/image/dda38b10-c8f6-41b6-af44-e17e820ced39?anchor=middlecenter&bgcolor=Transparent&borderWidth=0&crop=0%2C0%2C0%2C0&width=460" },
new Car { Id = 24, Make = "Maserati", Model = "Levante", Class = "SUV", PricePerDay = 600m, IsAvailable = true, ImageUrl = "https://www.turbo.fr/sites/default/files/2019-09/maserati-levante-trofeo-essai.png" },
new Car { Id = 45, Make = "Mercedes-Benz", Model = "G-Class", Class = "Luxury 4x4", PricePerDay = 950m, IsAvailable = true, ImageUrl = "https://ik.imagekit.io/vyro/public/autofox-api-mapped-to-vehicles/b81f83f332e9a3fce6d255c0f45f2103033e7f706068f23d967c85d99b593635.jpg" },

 
// SUPER CARS / SPORTS
new Car { Id = 51, Make = "Ferrari", Model = "F8 Tributo", Class = "Supercar", PricePerDay = 1400m, IsAvailable = true, ImageUrl = "https://cdn.ferrari.com/cms/network/media/img/resize/5d26fdb7c3f9ec0af6475619-01_fb_ppl_intro_lp3lhwq8?width=1080" },
new Car { Id = 52, Make = "Lamborghini", Model = "Aventador", Class = "Supercar", PricePerDay = 2000m, IsAvailable = true, ImageUrl = "https://www.lamborghini.com/sites/it-en/files/DAM/lamborghini/facelift_2019/model_detail/aventador/s/s/s-3_M.jpg" },
new Car { Id = 54, Make = "Porsche", Model = "718 Cayman GT4", Class = "Sports", PricePerDay = 900m, IsAvailable = true, ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/3/32/Porsche_718_Cayman_%28MSP17%29.jpg" },
new Car { Id = 55, Make = "Aston Martin", Model = "Vantage", Class = "Sports", PricePerDay = 1100m, IsAvailable = true, ImageUrl = "https://www.largus.fr/images/styles/max_1300x1300/public/2024-02/aston-martin-vantage-2024-jaune-avg-mk.jpg?itok=EhIDUYJs" },

    // ELECTRIQUES
  new Car { Id = 56, Make = "Tesla", Model = "Model Y Performance", Class = "Electric SUV", PricePerDay = 350m, IsAvailable = true, ImageUrl = "https://images.caradisiac.com/logos/2/0/2/6/272026/S8-essai-tesla-model-y-performance-2022-on-en-a-pour-son-argent-196566.jpg" },
new Car { Id = 57, Make = "BMW", Model = "iX", Class = "Electric SUV", PricePerDay = 400m, IsAvailable = true, ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQNn-A9iJfxvdmAV4gdDmORCazOtdvVadTxfw&s" },
new Car { Id = 58, Make = "Audi", Model = "e-tron GT", Class = "Electric Sports", PricePerDay = 450m, IsAvailable = true, ImageUrl = "https://www.automobile-magazine.fr/asset/cms/1200x750/226448/config/173544/audi-rs-e-tron-gt-av.jpg" },
new Car { Id = 59, Make = "Porsche", Model = "Taycan 4S", Class = "Electric Sports", PricePerDay = 420m, IsAvailable = true, ImageUrl = "https://www.hoonited.com/wp-content/uploads/2024/12/Porsche-Taycan-4S-2024-15.jpg" },

// SUV / CROSSOVER
new Car { Id = 61, Make = "Audi", Model = "Q7", Class = "SUV", PricePerDay = 400m, IsAvailable = true, ImageUrl = "https://hips.hearstapps.com/hmg-prod/images/2020-audi-q7-134-1583273378.jpg?crop=0.891xw:0.751xh;0.0782xw,0.249xh&resize=2048:*" },
new Car { Id = 62, Make = "BMW", Model = "X6", Class = "SUV", PricePerDay = 450m, IsAvailable = true, ImageUrl = "https://www.largus.fr/images/styles/max_1300x1300/public/images/bmw-x6-2019-vue-av.jpg?itok=QvwSVz1Z" },
new Car { Id = 63, Make = "Mercedes-Benz", Model = "GLS", Class = "Luxury SUV", PricePerDay = 550m, IsAvailable = true, ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c6/Mercedes-Benz_X_167_GLS580_IAA_2019_JM_0265.jpg/330px-Mercedes-Benz_X_167_GLS580_IAA_2019_JM_0265.jpg" },
new Car { Id = 64, Make = "Range Rover", Model = "Velar", Class = "SUV", PricePerDay = 480m, IsAvailable = true, ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/8/88/Range_Rover_Velar.jpg" },
new Car { Id = 65, Make = "Maserati", Model = "Levante Trofeo", Class = "Luxury SUV", PricePerDay = 600m, IsAvailable = true, ImageUrl = "https://www.largus.fr/images/styles/max_1300x1300/public/images/maserati-levante-trofeo-24.jpg?itok=Lr0Hjp4N" },

    // BERLINES / LUXURY
  new Car { Id = 66, Make = "Mercedes-Benz", Model = "E-Class", Class = "Luxury Sedan", PricePerDay = 350m, IsAvailable = true, ImageUrl = "https://www.mbusa.com/content/dam/mb-nafta/us/myco/my26/e-class/e-sedan/class-page/2026-E-SEDAN-HC-D.jpg" },
new Car { Id = 67, Make = "BMW", Model = "5 Series", Class = "Luxury Sedan", PricePerDay = 340m, IsAvailable = true, ImageUrl = "https://cdn.motor1.com/images/mgl/KrpgM/s3/2017-bmw-5-series.jpg" },
new Car { Id = 68, Make = "Audi", Model = "A6", Class = "Luxury Sedan", PricePerDay = 330m, IsAvailable = true, ImageUrl = "https://images.ctfassets.net/uaddx06iwzdz/7aPSWTuHLGAcn6ps5BniGs/aa11e9bf7f6d7d89365f4481522eaa89/audi-a6-front.jpg" }
 );
        }
    }
}
