using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using NetCoreTest_001.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace NetCoreTest_001
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            // «арегистрировать контекст задач
            services.AddDbContext<TodoContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                
                // ƒобавили!!!
                .AddDefaultTokenProviders();

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();

            //ƒобавить сервис авторизации через JWT
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = Configuration["JwtIssuer"],
            //        ValidAudience = Configuration["JwtAudience"],
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSecurityKey"]))
            //    };
            //});




            //services.AddAuthentication(options =>

            //{

            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            //    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            //})

            //.AddJwtBearer(options =>

            //{

            //    options.SaveToken = true;

            //    options.RequireHttpsMetadata = false;

            //    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()

            //    {

            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = Configuration["JwtIssuer"],
            //        ValidAudience = Configuration["JwtAudience"],
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSecurityKey"]))

            //    };

            //});


            //----< JWT-Token >----
            //*reference: www.blinkingcaret.com
            services.AddAuthentication(options =>
            {
                // «десь указываетс€ тип авторизации по умолчанию!
                // ѕока - оставить как есть, чтобы можно было регистрироватьс€ и заходить через веб-приложение!
                //options.DefaultAuthenticateScheme = "JwtBearer";
                //options.DefaultChallengeScheme = "JwtBearer";
            })

            //.AddOpenIdConnectServer(options =>
            //{
            //    options.AllowInsecureHttp = true;
            //    options.TokenEndpointPath = new PathString("/token");
            //    options.AccessTokenLifetime = TimeSpan.FromDays(1);
            //    options.TokenEndpointPath = "/token";
            //    options.Provider = new SimpleAuthorizationServerProvider();
            //})

            .AddJwtBearer("JwtBearer", jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    //ValidateIssuerSigningKey = true,
                    //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Demo_SecretKey_2018-06-27")),

                    //ValidateIssuer = true,
                    //ValidIssuer = "website_freelancer",

                    //ValidateAudience = true,
                    //ValidAudience = "webclients_freelancer",

                    //ValidateLifetime = true, //validate the expiration and not before values in the token

                    //ClockSkew = TimeSpan.FromMinutes(5) //5 minute tolerance for the expiration date

                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["JwtIssuer"],
                    ValidAudience = Configuration["JwtAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSecurityKey"]))
                };
            });
            //----</ JWT-Token >----


            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
