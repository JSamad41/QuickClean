using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace QuickClean.Controllers
{
	public class HomeController : Controller
	{

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
			//close button
			return RedirectToAction("Index");
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
