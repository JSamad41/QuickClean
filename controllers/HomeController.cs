using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace QuickClean.Controllers
{
	public class HomeController : Controller
	{

		public ActionResult PublicGallery()
		{
			Models.Property e = new Models.Property();
			Models.User u = new Models.User();
			u = u.GetUserSession();
			e.User = u;

				Models.Database db = new Models.Database();
				long lngID = Convert.ToInt64(RouteData.Values["id"]);
				e = e.GetProperty(lngID);
				e.Images = db.GetPropertyImages(lngID);

			return View(e);
		}

		[HttpPost]
		public ActionResult PublicGallery(IEnumerable<HttpPostedFileBase> files)
		{
			Models.Property e = new Models.Property();
			e.User = new Models.User();
			e.User = e.User.GetUserSession();
			e.ID = Convert.ToInt64(RouteData.Values["id"]);
			foreach (var file in files)
			{
				e.AddPropertyImage(file);
			}
			return Json("file(s) uploaded successfully");
		}

		public ActionResult Properties()
		{
			Models.Database db = new Models.Database();
			Models.HomeContent h = new Models.HomeContent();

			h.Properties = new List<Models.Property>();
			h.Properties = db.GetActiveProperties();

			h.User = new Models.User();
			h.User = h.User.GetUserSession();

			if (h.User.IsAuthenticated)
			{
				h.User.Likes = db.GetPropertyLikes(h.User.UID);
				h.User.Ratings = db.GetPropertyRatings(h.User.UID);
			}
			return View(h);
		}

		public ActionResult Index()
		{
			Models.Database db = new Models.Database();
			Models.HomeContent h = new Models.HomeContent();

			h.Properties = new List<Models.Property>();
			h.Properties = db.GetActiveProperties();

			h.User = new Models.User();
			h.User = h.User.GetUserSession();

			if (h.User.IsAuthenticated)
			{
				h.User.Likes = db.GetPropertyLikes(h.User.UID);
				h.User.Ratings = db.GetPropertyRatings(h.User.UID);
			}
			return View(h);
		}

		public ActionResult Property()
		{
			Models.PropertyContent ec = new Models.PropertyContent();
			Models.Database db = new Models.Database();

			ec.User = new Models.User();
			ec.User = ec.User.GetUserSession();

			if (ec.User.IsAuthenticated)
			{
				ec.User.Likes = db.GetPropertyLikes(ec.User.UID);
				ec.User.Ratings = db.GetPropertyRatings(ec.User.UID);
			}

			long id = Convert.ToInt64(RouteData.Values["id"]);
			ec.Property = new Models.Property();
			ec.Property = ec.Property.GetProperty(id);

			return View(ec);
		}

		[HttpPost]
		public ActionResult Property(FormCollection col)
		{
			Models.User u = new Models.User();
			u = u.GetUserSession();

			if (col["btnSubmit"] == "close")
			{
				return RedirectToAction("Index");
			}

			if (col["btnSubmit"] == "property-gallery")
			{
				return RedirectToAction("PublicGallery", "Home", new { @id = Convert.ToInt64(RouteData.Values["id"]) });
			}

			if (col["btnSubmit"] == "update")
			{

				Models.Property e = new Models.Property();

				if (RouteData.Values["id"] != null) e.ID = Convert.ToInt64(RouteData.Values["id"]);
				e.User = u;
		
				e.SaveCleaner();

				if (e.ID > 0)
				{
					return RedirectToAction("Property", new { @id = e.ID });
				}

                
            }
			return View();


			//close button
			//return RedirectToAction("Index");
		}

		[HttpPost]
		public JsonResult SaveReport(long UID, long IDToReport, int ProblemID)
		{
			try
			{
				Models.Database db = new Models.Database();
				System.Threading.Thread.Sleep(3000);
				bool b = false;
				b = db.InsertReport(UID, IDToReport, ProblemID);
				return Json(new { Status = b });
			}
			catch (Exception ex)
			{
				return Json(new { Status = -1 }); //error
			}
		}

		[HttpPost]
		public JsonResult TogglePropertyLike(long UID, long ID)
		{
			try
			{
				Models.Database db = new Models.Database();
				int intReturn = 0;
				intReturn = db.TogglePropertyLike(UID, ID);
				return Json(new { Status = intReturn });
			}
			catch (Exception ex)
			{
				return Json(new { Status = -1 }); //error
			}
		}

		[HttpPost]
		public JsonResult RateProperty(long UID, long ID, long Rating)
		{
			try
			{
				Models.Database db = new Models.Database();
				int intReturn = 0;
				intReturn = db.RateProperty(UID, ID, Rating);
				return Json(new { Status = intReturn });
			}
			catch (Exception ex)
			{
				return Json(new { Status = -1 }); //error
			}
		}
	}
}
