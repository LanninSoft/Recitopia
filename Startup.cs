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
using Recitopia.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Recitopia.Models;
using Recitopia.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Recitopia
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
            
            //NEW
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<RecitopiaDBContext>()
              .AddDefaultTokenProviders();

            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")));

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<RecitopiaDBContext>(options =>
                {
                    options.UseSqlServer(Configuration["DefaultConnection"],
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        });
                }
                );

            services.AddControllersWithViews()
                    .AddRazorRuntimeCompilation();
            services.AddRazorPages();

            services.AddTransient<IEmailSender, EmailSender>(i =>
                new EmailSender(
                    Configuration["EmailSender:PrimaryDomain"],
                    Configuration.GetValue<int>("EmailSender:PrimaryPort"),
                    Configuration["EmailSender:UsernameEmail"],
                    Configuration["EmailSender:UsernamePassword"],
                    Configuration["EmailSender:UsernameEmail"],
                    Configuration["EmailSender:ToEmail"],
                    Configuration["EmailSender:CcEmail"],
                    Configuration.GetValue<bool>("EmailSender:enableSsl")
                )
            );

            services.AddMvc()
               .AddRazorPagesOptions(options =>
               {
                   //options.AllowAreas = true;
                   options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                   options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
               });
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });



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
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}
