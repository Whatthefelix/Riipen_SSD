using System.Reflection;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using Riipen_SSD.Models;
using Owin;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Integration.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin;
using System.Collections.Generic;
using System.Web;
using Riipen_SSD.DAL;
using SimpleInjector.Integration.Web;

namespace Riipen_SSD.App_Start
{
    public static class SimpleInjectorInitializer
    {
        public static Container Initialize(IAppBuilder app)
        {
            var container = GetInitializeContainer(app);

            container.Verify();

            DependencyResolver.SetResolver(
                new SimpleInjectorDependencyResolver(container));

            return container;
        }

        public static Container GetInitializeContainer(IAppBuilder app)
        {
            var container = new Container();

            container.RegisterSingleton<IAppBuilder>(app);
            container.RegisterPerWebRequest<ApplicationUserManager>();
            container.RegisterPerWebRequest<ApplicationDbContext>(() => new ApplicationDbContext("DefaultConnection"));
            container.RegisterPerWebRequest<IUserStore<ApplicationUser>>(() => new UserStore<ApplicationUser>(container.GetInstance<ApplicationDbContext>()));
            container.RegisterInitializer<ApplicationUserManager>(manager => InitializeUserManager(manager, app));
            container.RegisterPerWebRequest<IAuthenticationManager>(() => AdvancedExtensions.IsVerifying(container) ? new OwinContext(new Dictionary<string, object>()).Authentication : HttpContext.Current.GetOwinContext().Authentication);
            container.RegisterInitializer<ApplicationSignInManager>(x => new ApplicationSignInManager(container.GetInstance<ApplicationUserManager>(), container.GetInstance<IAuthenticationManager>()));
            container.RegisterPerWebRequest<ApplicationSignInManager>();
            container.RegisterPerWebRequest<SSD_RiipenEntities>();
            container.RegisterPerWebRequest<IUnitOfWork, UnitOfWork>();
            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

            return container;
        }

        private static void InitializeUserManager(
            ApplicationUserManager manager, IAppBuilder app)
        {
            manager.UserValidator =
             new UserValidator<ApplicationUser>(manager)
             {
                 AllowOnlyAlphanumericUserNames = false,
                 RequireUniqueEmail = true
             };

            //Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator()
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            var dataProtectionProvider =
                 app.GetDataProtectionProvider();

            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                 new DataProtectorTokenProvider<ApplicationUser>(
                  dataProtectionProvider.Create("ASP.NET Identity"));
            }
        }
    }
}