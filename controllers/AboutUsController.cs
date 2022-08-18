using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuickClean.Controllers {
    public class AboutUsController : Controller {
        // GET: AboutUs
        public ActionResult Index() {
            Models.User u = new Models.User();

            u.FirstName = "Nick";
            u.LastName = "Kohlman";
            u.Email = "jnkohlman@cincinnatistate.edu";
            u.Role = "owner";

            return View(u);
        }

        [HttpPost]
        public ActionResult Index(FormCollection col) {
            try {
                Models.User u = new Models.User();

                if (col["btnSubmit"] == "more") { //more button pressed
                    return RedirectToAction("More");
                }
                else if (col["btnSubmit"] == "contactus") { // Contact Us btn Pushed
                    return RedirectToAction("ContactUs");
                }
                else if (col["btnClose"] == "close") { //Close button pressed
                    return RedirectToAction("Properties", "Home");
                }
                else {
                    return View(u);
                }
            }
            catch (Exception) {
                Models.User u = new Models.User();
                return View(u);
            }

        }

        // GET: More
        public ActionResult More() {

            Models.User u = new Models.User();

            u.FirstName = "Nick";
            u.LastName = "Kohlman";
            u.Email = "jnkohlman@cincinnatistate.edu";
            u.Role = "owner";

            return View(u);
        }

        [HttpPost]
        public ActionResult More(FormCollection col) {
            return RedirectToAction("Index");

        }

        // GET: ContactUs
        public ActionResult ContactUs() {

            Models.User u = new Models.User();

            u.FirstName = "Nick";
            u.LastName = "Kohlman";
            u.Email = "jnkohlman@cincinnatistate.edu";
            u.Role = "owner";

            return View(u);
        }

        [HttpPost]
        public ActionResult ContactUs(FormCollection col) {
            return RedirectToAction("Index");

        }


    }

}
