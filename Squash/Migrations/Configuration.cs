namespace Squash.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Squash.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Squash.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Squash.Models.ApplicationDbContext context)
        {
            if (!System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Launch();
            var rolestore = new RoleStore<IdentityRole>(context);
            var rolemanager = new RoleManager<IdentityRole>(rolestore);
            var userstore = new UserStore<ApplicationUser>(context);
            var usermanager = new UserManager<ApplicationUser>(userstore);

            ApplicationUser demopm = null;
            ApplicationUser demoadmin = null;
            ApplicationUser demosubmitter = null;
            ApplicationUser demodeveloper = null;
            #region Role Seeding
            if (!context.Roles.Any(r => r.Name == "Administrator")) 
                rolemanager.Create(new IdentityRole { Name = "Administrator" });
            if (!context.Roles.Any(r => r.Name == "Project Manager"))
                rolemanager.Create(new IdentityRole { Name = "Project Manager" });
            if (!context.Roles.Any(r => r.Name == "Developer"))
                rolemanager.Create(new IdentityRole { Name = "Developer" });
            if (!context.Roles.Any(r => r.Name == "Submitter"))
                rolemanager.Create(new IdentityRole { Name = "Submitter" });
            #endregion

            #region Demo User Seeds
            if (!context.Users.Any(u => u.UserName == "administrator@mailinator.com"))
            {
                var user = new ApplicationUser { UserName = "administrator@mailinator.com", Role="Administrator", FirstName = "Demo", LastName = "Admin", Email = "administrator@mailinator.com" };
                usermanager.Create(user, "demoPassword1!");
                usermanager.AddToRole(user.Id, "Administrator");
                demoadmin = user;
            }
            if (!context.Users.Any(u => u.UserName == "projectmanager@mailinator.com"))
            {
                var user = new ApplicationUser { UserName = "projectmanager@mailinator.com", Role = "Project Manager", FirstName = "Demo", LastName = "Manager", Email = "projectmanager@mailinator.com" };
                usermanager.Create(user, "demoPassword1!");
                usermanager.AddToRole(user.Id, "Project Manager");
                demopm = user;
            }
            if (!context.Users.Any(u => u.UserName == "developer@mailinator.com"))
            {
                var user = new ApplicationUser { UserName = "developer@mailinator.com", Role = "Developer", FirstName = "Demo", LastName = "Developer", Email = "developer@mailinator.com" };
                usermanager.Create(user, "demoPassword1!");
                usermanager.AddToRole(user.Id, "Developer");
                demodeveloper = user;
            }
            if (!context.Users.Any(u => u.UserName == "Submitter@mailinator.com"))
            {
                var user = new ApplicationUser { UserName = "Submitter@mailinator.com", Role = "Submitter", FirstName = "Demo", LastName = "Submitter", Email = "Submitter@mailinator.com" };
                usermanager.Create(user, "demoPassword1!");
                usermanager.AddToRole(user.Id, "Submitter");
                demosubmitter = user;
            }
            #endregion




            //#region Project Seed
            //context.Projects.AddOrUpdate(
            //    p => p.Name,
            //    new Project() { Name = "First Demo", Description = "The first demo project seed", CreatedDate = DateTime.Now },
            //    new Project() { Name = "Second Demo", Description = "The second demo project seed", CreatedDate = DateTime.Now },
            //    new Project() { Name = "Third Demo", Description = "The third demo project seed", CreatedDate = DateTime.Now },
            //    new Project() { Name = "Fourth Demo", Description = "The fourth demo project seed", CreatedDate = DateTime.Now });
            //#endregion
            #region Ticket Status Seed
            context.Statuses.AddOrUpdate(
                s => s.Name,
                new Status() { Name = "Created"},
                new Status() { Name = "Assigned" },
                new Status() { Name = "Resolved" },
                new Status() { Name = "Reopened"},
                new Status() { Name = "Archived"});
            #endregion
            #region Ticket Priority Seed
            context.Priorities.AddOrUpdate(
                p => p.Name,
                new Priority() { Name = "Low"},
                new Priority() { Name = "Medium" },
                new Priority() { Name = "High"});
            #endregion
            #region Ticket Type Seed
            context.Types.AddOrUpdate(
                p => p.Name,
                new Models.Type() { Name = "Software"},
                new Models.Type() { Name = "Hardware" },
                new Models.Type() { Name = "UI" },
                new Models.Type() { Name = "Other" });
            #endregion

            context.SaveChanges();
            var projects = context.Projects;
            var statuses = context.Statuses;
            var priorities = context.Priorities;
            var types = context.Types;
            var users = context.Users;
            var roles = context.Roles;




            for (int i = 0; i < 12; i++)
            {
                context.Projects.AddOrUpdate(p => p.Name, new Project() { Name = "Demo Project #" + i, Description = "The is a seeded demo project.", CreatedDate = DateTime.Now });

            }
            context.SaveChanges();
            foreach(var project in context.Projects)
            {
                var count = 1;
                foreach(var status in context.Statuses)
                {
                    foreach (var type in context.Types)
                    {
                        foreach (var priority in context.Priorities)
                        {
                            var rng = new Random();
                            if (rng.NextDouble() < 0.5 || count<20)
                            {

                                context.Tickets.AddOrUpdate(
                                    t => t.Title,
                                    new Ticket
                                    {
                                        ProjectId = project.Id,
                                        TypeId = type.Id,
                                        PriorityId = priority.Id,
                                        StatusId = status.Id,
                                        OwnerId = demosubmitter.Id,
                                        AssignedUserId = demodeveloper.Id,
                                        Title = "Demo Ticket " + count,
                                        Summary = "A demo ticket of priority '" + priority.Name + "', type '" + type.Name + "', status '" + status.Name + "'",
                                        CreatedDate = DateTime.Now
                                    });
                                count++;
                            }
                        }
                    }
                }
            }
            context.SaveChanges();
            foreach (var ticket in context.Tickets)
            {
                for(var i = 0; i < 4; i++)
                {
                    context.TicketComment.AddOrUpdate(
                        t => t.Id,
                        new TicketComment()
                        {
                            OwnerId = demosubmitter.Id,
                            Body = "Test Comment " + i,
                            TicketId = ticket.Id,
                            CreatedDate = DateTime.Now
                        });
                    context.TicketAttachments.AddOrUpdate(
                        t => t.Id,
                        new TicketAttachment()
                        {
                            UserId = demosubmitter.Id,
                            Description = "Test Attachment " + i,
                            TicketId = ticket.Id,
                            FilePath = "/Uploads/637081515767025812-0.jpg",
                            UploadDate = DateTime.Now
                        });
                    context.TicketHistory.AddOrUpdate(
                        t => t.Id,
                        new TicketHistory()
                        {
                            UpdaterId = demoadmin.Id,
                            Property = "Test Property",
                            OldValue = "Test Property Old",
                            NewValue = "Test Property New",
                            TicketId = ticket.Id,
                            UpdateDate = DateTime.Now
                        });
                }
            }
            context.SaveChanges();


            for(var i =0; i < 10; i++)
            {
                context.Notifications.AddOrUpdate(
                    t => t.Id,
                    new Notification()
                    {
                        TicketId = i + 1,
                        IsRead = false,
                        Title = "Test " + (i + 1),
                        Body = "Test Ticket Notification",
                        SentDate = DateTime.Now,
                        ReciepentId = demoadmin.Id
                    },
                    new Notification()
                    {
                        TicketId = i + 1,
                        IsRead = false,
                        Title = "Test " + (i + 1),
                        Body = "Test Ticket Notification",
                        SentDate = DateTime.Now,
                        ReciepentId = demodeveloper.Id
                    },
                    new Notification()
                    {
                        TicketId = i + 1,
                        IsRead = false,
                        Title = "Test " + (i + 1),
                        Body = "Test Ticket Notification",
                        SentDate = DateTime.Now,
                        ReciepentId = demodeveloper.Id
                    },
                    new Notification()
                    {
                        TicketId = i + 1,
                        IsRead = false,
                        Title = "Test " + (i + 1),
                        Body = "Test Ticket Notification",
                        SentDate = DateTime.Now,
                        ReciepentId = demosubmitter.Id
                    },
                    new Notification()
                    {
                        ProjectId = i + 1,
                        IsRead = false,
                        Title = "Test " + (i + 1),
                        Body = "Test Project Notification",
                        SentDate = DateTime.Now,
                        ReciepentId = demoadmin.Id
                    },
                    new Notification()
                    {
                        ProjectId = i + 1,
                        IsRead = false,
                        Title = "Test " + (i + 1),
                        Body = "Test Project Notification",
                        SentDate = DateTime.Now,
                        ReciepentId = demodeveloper.Id
                    },
                    new Notification()
                    {
                        ProjectId = i + 1,
                        IsRead = false,
                        Title = "Test " + (i + 1),
                        Body = "Test Project Notification",
                        SentDate = DateTime.Now,
                        ReciepentId = demodeveloper.Id
                    },
                    new Notification()
                    {
                        ProjectId = i + 1,
                        IsRead = false,
                        Title = "Test " + (i + 1),
                        Body = "Test Project Notification",
                        SentDate = DateTime.Now,
                        ReciepentId = demosubmitter.Id
                    }
                    );
            }

        }
    }
}
