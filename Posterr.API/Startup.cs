using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Posterr.DB;
using Posterr.DB.Models;
using Posterr.Infra.Interfaces;
using Posterr.Infra.Repository;
using Posterr.Services;
using Posterr.Services.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Posterr
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //In Memory
            services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("Posterr"));

            //LocalDB
            //services.AddDbContext<ApiContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("ApiContext")));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Posterr", Version = "v1" });
            });

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IFollowService, FollowService>();
            services.AddScoped<IFollowRepository, FollowRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Posterr v1"));
            }

            //1st run
            var context = serviceProvider.GetService<ApiContext>();
            _InsertFirstRunTestData(context);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void _InsertFirstRunTestData(ApiContext context)
        {
            if (!context.Users.Any()) // First run
            {
                using (StreamReader sr = File.OpenText(Directory.GetCurrentDirectory() + "\\TestData\\users.json"))
                {
                    List<User> users = JsonConvert.DeserializeObject<List<User>>(sr.ReadToEnd());
                    context.Users.AddRange(users);
                    context.SaveChanges();
                }

                using (StreamReader sr = File.OpenText(Directory.GetCurrentDirectory() + "\\TestData\\follows.json"))
                {
                    List<Follow> follows = JsonConvert.DeserializeObject<List<Follow>>(sr.ReadToEnd());
                    context.Follows.AddRange(follows);
                    context.SaveChanges();
                }
                using (StreamReader sr = File.OpenText(Directory.GetCurrentDirectory() + "\\TestData\\posts.json"))
                {
                    List<Post> posts = JsonConvert.DeserializeObject<List<Post>>(sr.ReadToEnd());
                    foreach (Post post in posts)
                    {
                        context.Posts.Add(post);
                        context.SaveChanges();
                    }
                }
            }
        }
    }
}
