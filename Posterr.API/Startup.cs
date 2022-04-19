using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Posterr.DB;
using System;
using Posterr.DB.Models;
using Posterr.Services.User;
using Posterr.Services;

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
            //In Memory
            //services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("Posterr"));

            services.AddDbContext<ApiContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ApiContext")));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Posterr", Version = "v1" });
            });

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IFollowService, FollowService>();
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

            //In Memory or 1st local run
            //var context = serviceProvider.GetService<ApiContext>();
            //TestData(context);

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
                //Id = 1,
                Name = "Isabella Emidio",
                Username = "isabellaemidio",
                CreatedAt = DateTime.Now,
            };
            var testUser2 = new User
            {
                //Id = 2,
                Name = "Sandra Regina",
                Username = "sandraregina",
                CreatedAt = DateTime.Now,
            };
            var testUser3 = new User
            {
                //Id = 3,
                Name = "Fabio Emidio",
                Username = "fabioemidio",
                CreatedAt = DateTime.Now,
            };
            context.Users.Add(testUser);
            context.Users.Add(testUser2);
            context.Users.Add(testUser3);

            context.SaveChanges();

            var follow = new Follow
            {
                //Id = 1,
                FollowingId = testUser.Id,
                FollowerId = testUser2.Id
            };
            var follow2 = new Follow
            {
                //Id = 2,
                FollowingId = testUser.Id,
                FollowerId = testUser3.Id
            };
            var follow3 = new Follow
            {
                //Id = 3,
                FollowingId = testUser3.Id,
                FollowerId = testUser.Id
            };
            context.Follows.Add(follow);
            context.Follows.Add(follow2);
            context.Follows.Add(follow3);

            // Basic post
            var testPost = new Post
            {
                //Id = 1,
                Content = "Hello Posterr, I'm Isabella Emidio",
                CreatedAt = DateTime.Now,
                UserId = testUser.Id
            };
            context.Posts.Add(testPost);
            context.SaveChanges();
            
            // Quote post
            var testPost2 = new Post
            {
                //Id = 2,
                Content = "Everyone is joining Posterr",
                CreatedAt = DateTime.Now,
                UserId = testUser3.Id,
                OriginalPostId = testPost.Id
            };
            context.Posts.Add(testPost2);
            context.SaveChanges();

            // Repost
            var testPost3 = new Post
            {
                //Id = 3,
                CreatedAt = DateTime.Now,
                UserId = testUser2.Id,
                OriginalPostId = testPost2.Id
            };
            context.Posts.Add(testPost3);
            context.SaveChanges();
        }
    }
}
