using DSW.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DSW
{
    public static class DefaultSettingsAndData
    {
        public const string PROJECT_NAME = "Data science wrapper";

        public const string ROLE_ADMIN = "Admin";
        public const string ROLE_USER = "User";
        public const string USER_ADMIN_PASS = "Admin123";
        public const string USER_ADMIN_EMAIL = "admin@dsw.proj";
        public const string USER_TEST_PASS = "Test123";
        public const string USER_TEST_EMAIL = "test@dsw.proj";

        public const int PASS_REQUIRED_LENGTH = 6;
        public const bool PASS_REQUIRE_NONLETTERORDIGIT = false;
        public const bool PASS_REQUIRE_DIGIT = true;
        public const bool PASS_REQUIRE_LOWERCASE = true;
        public const bool PASS_REQUIRE_UPPERCASE = true;

        public const bool USER_LOCKOUT_ENABLED_BY_DEFAULT = true;
        public const int DEFAULT_ACCOUNT_LOCKOUT_TIMESPAN = 5;
        public const int MAX_FAILED_ACCESS_ATTEMPTS_BEFORE_LOCKOUT = 3;
    }

    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            var from = ConfigurationManager.AppSettings["SystemEmailSender"];
            await Utils.EmailService.SendMailAsync(from, message.Destination, message.Subject, message.Body);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = DefaultSettingsAndData.PASS_REQUIRED_LENGTH,
                RequireNonLetterOrDigit = DefaultSettingsAndData.PASS_REQUIRE_NONLETTERORDIGIT,
                RequireDigit = DefaultSettingsAndData.PASS_REQUIRE_DIGIT,
                RequireLowercase = DefaultSettingsAndData.PASS_REQUIRE_LOWERCASE,
                RequireUppercase = DefaultSettingsAndData.PASS_REQUIRE_UPPERCASE
            };
            
            manager.UserLockoutEnabledByDefault = DefaultSettingsAndData.USER_LOCKOUT_ENABLED_BY_DEFAULT;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(DefaultSettingsAndData.DEFAULT_ACCOUNT_LOCKOUT_TIMESPAN);
            manager.MaxFailedAccessAttemptsBeforeLockout = DefaultSettingsAndData.MAX_FAILED_ACCESS_ATTEMPTS_BEFORE_LOCKOUT;

            manager.EmailService = new EmailService();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }

            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            CreateDefaultEntitiesEF(context);
            base.Seed(context);
        }

        private void CreateDefaultEntitiesEF(ApplicationDbContext context)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!roleManager.RoleExists(DefaultSettingsAndData.ROLE_ADMIN))
            {
                roleManager.Create(new IdentityRole(DefaultSettingsAndData.ROLE_ADMIN));

                var adminUser = ApplicationUser.Create(DefaultSettingsAndData.USER_ADMIN_EMAIL);
                var adminResult = userManager.Create(adminUser, DefaultSettingsAndData.USER_ADMIN_PASS);
                if (adminResult.Succeeded)
                {
                    userManager.AddToRole(adminUser.Id, DefaultSettingsAndData.ROLE_ADMIN);
                }
                else
                {
                    throw new Exception(string.Join("; ", adminResult.Errors));
                }
            }

            if (!roleManager.RoleExists(DefaultSettingsAndData.ROLE_USER))
            {
                roleManager.Create(new IdentityRole(DefaultSettingsAndData.ROLE_USER));

                var testUser = ApplicationUser.CreateWithBillingAccount(DefaultSettingsAndData.USER_TEST_EMAIL);
                var userResult = userManager.Create(testUser, DefaultSettingsAndData.USER_TEST_PASS);
                if (userResult.Succeeded)
                {
                    userManager.AddToRole(testUser.Id, DefaultSettingsAndData.ROLE_USER);
                }
                else
                {
                    throw new Exception(string.Join("; ", userResult.Errors));
                }
            }

        }
    }
}
