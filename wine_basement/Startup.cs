using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using basement.database;
using basement.infrastructure;
using basement.infrastructure.Helpers;
using basement.infrastructure.Interfaces;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace wine_basement
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //creating the image folder 
            char separator = Path.DirectorySeparatorChar;
            string imageFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + separator + "wineImages";
            System.IO.Directory.CreateDirectory(imageFolderPath);
            System.IO.Directory.CreateDirectory(imageFolderPath + separator + "original");
            System.IO.Directory.CreateDirectory(imageFolderPath + separator + "thumbnail");
            System.IO.Directory.CreateDirectory(imageFolderPath + separator + "big");


            services.AddCors();
            services.AddMvcCore();

            services.AddControllers()
                .AddNewtonsoftJson(
            options => options.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            // services.AddControllers()
            //     .AddNewtonsoftJson(options =>
            //         options.SerializerSettings
            //             .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            //     );
            //



            //.AddNewtonsoftJson(
            //options => options.SerializerSettings.ReferenceLoopHandling =
            //    Newtonsoft.Json.ReferenceLoopHandling.Ignore);


            // services.AddOptions();

            try
            {
                DotNetEnv.Env.Load();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            services.AddScoped<IDatabaseBoundary, DatabaseBoundary>();
            services.AddScoped<IInfrastructureService, InfrastructureService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IImageService, ImageService>();
          //  services.AddScoped<ITranslator, Translator>();


            services.AddDbContext<DatabaseContext>(ServiceLifetime.Transient);


            ////////////firebase configuration
            var googleCredentialFileName = "privateKey.json";
            var googleCredential = Path.Combine(Environment.CurrentDirectory, googleCredentialFileName);
            var credential = GoogleCredential.FromFile(googleCredential);
            FirebaseApp.Create(new AppOptions()
            {
                Credential = credential
            });

            services
              .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options =>
             {
                 options.Authority = "https://securetoken.google.com/entecon-2a0e3";
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidIssuer = "https://securetoken.google.com/entecon-2a0e3",
                     ValidateAudience = true,
                     ValidAudience = "entecon-2a0e3",
                     ValidateLifetime = true
                 };
             });


            // auto mapper configuration
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());


            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}