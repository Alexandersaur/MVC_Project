namespace MVC_Project.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using MVC_Project.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MVC_Project.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        public string Email { get; private set; }
        public string UserName { get; private set; }

        protected override void Seed(MVC_Project.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            //I want to go out to the DB and see if a particular role is already present

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole() { Name = "Admin" });
            }

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            if(!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole() { Name = "Moderator" });
            }

            //writes some code that creates a user

            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            //now i need to go out and look for the presence of a user with a specific email
            // if and only if it is not found will i create a user with that email

            if(!context.Users.Any(u => u.Email == "jeremy.a.steward@gmail.com"))
                //jasontwichell@Coderfoundry.com
            {
                //var newUser = new ApplicationUser();
                //newUser.Email = "jeremy.a.steward@gmail.com";
                //newUser.UserName = "jeremy.a.steward@gmail.com";
                //newUser.FirstName = "Jeremy";
                //newUser.LastName = "Steward";
                //newUser.DisplayName = "JSteward";
                //userManager.Create(newUser, "Abc&123!");

                userManager.Create(new ApplicationUser()
                {
                    Email = "jeremy.a.steward@gmail.com",
                    UserName = "jeremy.a.steward@gmail.com",
                    FirstName = "Jeremy",
                    LastName = "Steward",
                    DisplayName = "Jsteward"
                }, "Abc&123!");

                userManager.Create(new ApplicationUser()
                {
                    Email = "jasontwichell@coderfoundry.com",
                    UserName = "jasontwichell@coderfoundry.com",
                    FirstName = "Jason",
                    LastName = "Twichell",
                    DisplayName = "Jtwichell"
                }, "Abc&123!");

                userManager.Create(new ApplicationUser()
                {
                    Email = "andrewrussell@coderfoundry.com",
                    UserName = "andrewrussell@coderfoundry.com",
                    FirstName = "Andrew",
                    LastName = "Russell",
                    DisplayName = "Arussell"
                }, "Abc&123!");

                //Step 1: Grab the id that was just created by adding the above user
                var userId = userManager.FindByEmail("jeremy.a.steward@gmail.com").Id;
                //now I want to assign the user to a specific role
                userManager.AddToRole(userId, "Admin");

                var userId2 = userManager.FindByEmail("jasontwichell@coderfoundry.com").Id;
                userManager.AddToRole(userId2, "Moderator");

                var userId3 = userManager.FindByEmail("andrewrussell@coderfoundry.com").Id;
                userManager.AddToRole(userId3, "Admin");
            }


        }

    }

   
}

