using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Posterr.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Posterr.DB.Models;

namespace Posterr
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
            services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("Posterr"));
            
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Posterr", Version = "v1" });
            });
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
            
            var context = serviceProvider.GetService<ApiContext>();
            TestData(context);
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void TestData(ApiContext context)
        {
            var testUser = new User
            {
                Id = 1,
                Name = "Isabella Emidio",
                Username = "isabellaemidio"
            };
            var testUser2 = new User
            {
                Id = 2,
                Name = "Sandra Regina",
                Username = "sandraregina"
            };
            var testUser3 = new User
            {
                Id = 3,
                Name = "Fabio Emidio",
                Username = "fabioemidio"
            };
            context.Users.Add(testUser);
            context.Users.Add(testUser2);
            context.Users.Add(testUser3);

            var follow = new Follow
            {
                FollowingId = testUser.Id,
                FollowerId = testUser2.Id
            };
            var follow2 = new Follow
            {
                FollowingId = testUser.Id,
                FollowerId = testUser3.Id
            };
            var follow3 = new Follow
            {
                FollowingId = testUser3.Id,
                FollowerId = testUser.Id
            };
            context.Follows.Add(follow);
            context.Follows.Add(follow2);
            context.Follows.Add(follow3);

            var testPost = new Post
            {
                Id = 1,
                Content = "Hello Postter, I'm Isabella Emidio",
                CreatedAt = DateTime.Now,
                UserId = testUser.Id
            };
            context.Posts.Add(testPost);
            
            context.SaveChanges();
        }
    }
}
