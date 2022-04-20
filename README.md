Isabella Riquetti Emidio

# Coding and Runiing the API
    Phase 1, coding
    Estimated time: 7 hours
    
    - Build out a RESTful API and corresponding backend system to handle the features detailed above. This RESTful API would communicate with single page JS app. This API you build should enable all the features on both of the pages.
    - You should implement a real, production-ready database, and queries should be performant.
    - Do not implement additional features beyond what is explained in the overview.
    - Write some automated tests for this project.
    - Do not build a front-end.
 ## Running the project
 **Required .NET Version:5.0**
 
  ##### Using InMemory DB
* No changes required

 ##### Using localdb
In order to use a localdb you need to first create the server and database.
The expected server: **(localdb)\Strider**
The expected database name: **Posterr**
* Run on command line: `cd {YOUR_PROJECT_LOCATION}\Posterr\Posterr.DB`
* Then run to run migrations and create the tables: `dotnet ef database update`
* Comment line 35 `services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("Posterr"));` in `Posterr.API/Startup.cs`
* Uncomment lines 38 and 39 in `Posterr.API/Startup.cs`
* Run the Posterr.API


 ### Testing the API
* Run the Posterr.API project
* Use Swagger in the API first page or import the following Postman collection to test the endpoints

# Planning
    Phase 2, planning
    Estimated time: 30 minutes
    
    The Product Manager wants to implement a new feature called "reply-to-post" (it's a lot like Twitter's These are regular posts that use "@ mentioning" at the beginning to indicate that it is a reply directly to a post. Reply posts should only be shown in a new, secondary feed on the user profile called "Posts and Replies" where all original posts and reply posts are shown. They should not be shown in the homepage feed.
    
    What you need to do:
    - Write down questions you have for the Product Manager about implementation.
    - Write about how you would solve this problem in as much detail as possible. Write about all of the changes to database/front-end/api/etc that you expect. You should write down any assumptions you are making from any questions for the Product Manager that you previously mentioned.
    - **Be thorough!**

    This should be added as a section called "Planning" in the README.
    
# Critique
    ## Phase 3, self-critique & scaling
    Estimated time: 30 minutes
    
    In any project, it is always a challenge to get the code perfectly how you'd want it. Here is what you need to do for this section:
    
    - Reflect on this project, and write what you would improve if you had more time.
    - Write about scaling. If this project were to grow and have many users and posts, which parts do you think would fail first? In a real-life situation, what steps would you take to scale this product? What other types of technology and infrastructure might you need to use?
    - **Be thorough!**
    
    This should be added as a section called "Critique" in the README.