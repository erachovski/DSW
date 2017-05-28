using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DSW.Models
{
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            return userIdentity;
        }

        public virtual BillingAccount BillingAccount { get; set; }
        public virtual ICollection<Project> Projects { get; set; }

        public static ApplicationUser Create(string email)
        {
            return new ApplicationUser
            {
                UserName = email,
                Email = email
            };
        }

        public static ApplicationUser CreateWithBillingAccount(string email)
        {
            return new ApplicationUser
            {
                UserName = email,
                Email = email,
                BillingAccount = new BillingAccount
                {
                    Info = "Account is active",
                    StartDate = "15/06/2016",
                    EndDate = "15/06/2018",
                    Balance = "$120"
                }
            };
        }
    }

    public class BillingAccount
    {
        public int Id { get; set; }
        public string Info { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Balance { get; set; }
    }

    public class Project
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}