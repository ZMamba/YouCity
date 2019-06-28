using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DProject.Models;

namespace DProject.Data
{
    public static class SeedData
    {
        public static void Initialize(DProjectContext context)
        {
            context.Database.EnsureCreated();

            if (context.Roles.Any())
            {
                return;
            }

            var roles = new Role[] { new Role { Name = "admin" }, new Role {Name = "user" } };
            foreach (var item in roles)
            {
                context.Roles.Add(item);
            }
            context.SaveChanges();
        }
    }
}
