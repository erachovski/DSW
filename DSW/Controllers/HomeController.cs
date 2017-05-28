using DSW.Models;
using System.Configuration;
using System.Web.Mvc;

namespace DSW.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TestProject()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(ContactModel details)
        {
            if (ModelState.IsValid)
            {
                var contactUsEmail = ConfigurationManager.AppSettings["ContactUsEmail"];
                Utils.EmailService.SendMail(details.Email, contactUsEmail, details.Subject, details.Message);
            }
            return View();
        }
    }
}