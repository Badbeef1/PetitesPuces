using System.Web.Mvc;

namespace PetitesPuces.Controllers
{
   public class LoginController : Controller
   {

      // GET: Login
      public ActionResult Index() => View();

      [HttpPost]
      public ActionResult VerifyLogin(Models.userExemple model)
      {
         var username = model.username;
         var password = model.password;

         if (username == null || password == null)
         {
            model.errorMessage = "Vous avez oublié au moins un champ !";
            model.password = (password != null) ? "" : null;  // null = red outline, "" = none
            return View("Index", model);
         }
         else if (username.Equals("a") && password.Equals("a"))
         {
            Session["username"] = "Utilisateur test";
         }
         else
         {
            model.errorMessage = "Le courriel ou le mot de passe n'est pas valide.";
            model.password = ""; //No red border
            return View("Index", model);
         }

         return RedirectToAction("Index", "Home");// Return to catalog
      }

      public ActionResult RecupererPassword() => View();
      
   }
}