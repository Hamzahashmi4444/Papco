namespace TLAS.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using TLAS.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            this.AddUserAndRoles();
        }


        bool AddUserAndRoles()
        {
            bool success = false;

            var idManager = new IdentityManager();
            success = idManager.CreateRole("Admin");
            if (!success == true) return success;

            success = idManager.CreateRole("Supervisor");
            if (!success == true) return success;

            success = idManager.CreateRole("Operator");
            if (!success) return success;


            var newUser = new ApplicationUser()
            {
                UserName = "ahad",
                Email = "aomer@avanceon.ae"
            };

            // Be careful here - you  will need to use a password which will 
            // be valid under the password rules for the application, 
            // or the process will abort:
            success = idManager.CreateUser(newUser, "Voilet123!");
            if (!success) return success;

            success = idManager.AddUserToRole(newUser.Id, "Admin");
            if (!success) return success;

            success = idManager.AddUserToRole(newUser.Id, "Supervisor");
            if (!success) return success;

            success = idManager.AddUserToRole(newUser.Id, "Operator");
            if (!success) return success;

            return success;
        }
    }
}