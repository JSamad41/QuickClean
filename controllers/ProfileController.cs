using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace QuickClean.Controllers
{
	public class ProfileController : Controller
	{
		public ActionResult Property()
		{
			Models.User u = new Models.User();
			Models.Property e = new Models.Property();
			u = u.GetUserSession();
			e.User = u;

			if (e.User.IsAuthenticated)
			{
				if (RouteData.Values["id"] == null)
				{ //add an empty Property
					e.Start = new DateTime(DateTime.Now.Year + 1, DateTime.Now.Month, DateTime.Now.Day, 13, 0, 0);
					e.End = new DateTime(DateTime.Now.Year + 1, DateTime.Now.Month, DateTime.Now.Day, 17, 0, 0);
				}
				else
				{ //get the Property
					long id = Convert.ToInt64(RouteData.Values["id"]);
					e = e.GetProperty(id);
				}
			}
			return View(e);
		}

		[HttpPost]
		public ActionResult Property(HttpPostedFileBase PropertyImage, FormCollection col)
		{
			Models.User u = new Models.User();
			u = u.GetUserSession();

			if (col["btnSubmit"] == "close")
			{
				if (col["from"] == null) return RedirectToAction("MyProperties");
				return RedirectToAction("Index", "Home");
			}

			if (col["btnSubmit"] == "property-gallery")
			{
				return RedirectToAction("PropertyGallery", new { @id = Convert.ToInt64(RouteData.Values["id"]) });
			}

			if (col["btnSubmit"] == "delete")
			{
				long lngID = Convert.ToInt64(RouteData.Values["id"]);
				return RedirectToAction("DeleteProperty", new { @id = lngID });
			}

			if (col["btnSubmit"] == "save")
			{

				Models.Property e = new Models.Property();

				if (RouteData.Values["id"] != null) e.ID = Convert.ToInt64(RouteData.Values["id"]);
				e.User = u;
				e.Title = col["Title"];
				if (col["IsActive"].ToString().Contains("true")) e.IsActive = true; else e.IsActive = false;
				e.Start = DateTime.Parse(string.Concat(col["Start"].ToString(), " ", col["Start.TimeOfDay"]));
				//e.End = DateTime.Parse(string.Concat(col["End"].ToString(), " ", col["End.TimeOfDay"]));

				e.squareFootage = col["squareFootage"];
				e.numberOfBedrooms = col["numberOfBedrooms"];
				e.numberOfBathrooms = col["numberOfBathrooms"];
				e.numberOfBathrooms = col["numberOfBathrooms"];
				e.numberOfBathrooms = col["numberOfBathrooms"];
				e.numberOfBathrooms = col["numberOfBathrooms"];

				if (col["standardCleaning"].ToString().Contains("true")) e.standardCleaning = true; else e.standardCleaning = false;
				if (col["deepCleaning"].ToString().Contains("true")) e.deepCleaning = true; else e.deepCleaning = false;

				if (col["carpetCleaning"].ToString().Contains("true")) e.carpetCleaning = true; else e.carpetCleaning = false;
				if (col["baseboardCleaning"].ToString().Contains("true")) e.baseboardCleaning = true; else e.baseboardCleaning = false;
				if (col["laundryCleaning"].ToString().Contains("true")) e.laundryCleaning = true; else e.laundryCleaning = false;
				if (col["dishCleaning"].ToString().Contains("true")) e.dishCleaning = true; else e.dishCleaning = false;

				e.Details = col["Details"];
				e.Compensation = col["Compensation"];

				e.Location = new Models.Location();
				e.Location.Title = col["Location.Title"];
				e.Location.Description = col["Location.Description"];

				e.Location.Address = new Models.Address();
				e.Location.Address.Address1 = col["Location.Address.Address1"];
				e.Location.Address.Address2 = col["Location.Address.Address2"];
				e.Location.Address.City = col["Location.Address.City"];
				e.Location.Address.State = col["Location.Address.State"];
				e.Location.Address.Zip = col["Location.Address.Zip"];

				

				//if (e.Title.Length == 0 || e.Description.Length == 0 || e.Location.Title.Length == 0)
				//{
				//e.ActionType = Models.Property.ActionTypes.RequiredFieldsMissing;
				//return View(e);
				//}

				e.Save();

				if (PropertyImage != null)
				{
					e.PropertyImage = new Models.Image();
					if (col["PropertyImage.ImageID"].ToString() == "")
					{
						e.PropertyImage.ImageID = 0;
					}
					else
					{
						e.PropertyImage.ImageID = Convert.ToInt32(col["PropertyImage.ImageID"]);
					}

					e.PropertyImage.Primary = true;
					e.PropertyImage.FileName = Path.GetFileName(PropertyImage.FileName);
					if (e.PropertyImage.IsImageFile())
					{
						e.PropertyImage.Size = PropertyImage.ContentLength;
						Stream stream = PropertyImage.InputStream;
						BinaryReader binaryReader = new BinaryReader(stream);
						e.PropertyImage.ImageData = binaryReader.ReadBytes((int)stream.Length);

						e.UpdatePrimaryImage();
					}
				}

				if (e.ID > 0)
				{
					return RedirectToAction("Property", new { @id = e.ID });
				}
			}
			return View();
		}

		public ActionResult PropertyGallery()
		{
			Models.Property e = new Models.Property();
			Models.User u = new Models.User();
			u = u.GetUserSession();
			e.User = u;

			if (e.User.IsAuthenticated)
			{
				Models.Database db = new Models.Database();
				long lngID = Convert.ToInt64(RouteData.Values["id"]);
				e = e.GetProperty(lngID);
				e.Images = db.GetPropertyImages(lngID);
			}
			return View(e);
		}

		[HttpPost]
		public ActionResult PropertyGallery(IEnumerable<HttpPostedFileBase> files)
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

		[HttpPost]
		public JsonResult DeletePropertyImage(long UID, long ID)
		{
			try
			{
				string type = string.Empty;
				Models.Database db = new Models.Database();
				if (db.DeletePropertyImage(ID)) return Json(new { Status = 1 }); //deleted
				return Json(new { Status = 0 }); //not deleted
			}
			catch (Exception ex)
			{
				return Json(new { Status = -1 }); //error
			}
		}

		public ActionResult DeleteProperty()
		{
			Models.Property e = new Models.Property();
			e.User = new Models.User();
			e.User = e.User.GetUserSession();
			if (e.User.IsAuthenticated)
			{
				long lngID = Convert.ToInt64(RouteData.Values["id"]);
				e = e.GetProperty(lngID);
			}
			return View(e);
		}

		[HttpPost]
		public ActionResult DeleteProperty(FormCollection col)
		{
			Models.User u = new Models.User();
			u = u.GetUserSession();
			if (u.IsAuthenticated)
			{
				long lngID = Convert.ToInt64(RouteData.Values["id"]);

				if (col["btnSubmit"] == "close") return RedirectToAction("Property", new { @id = lngID });
				if (col["btnSubmit"] == "delete")
				{
					Models.Database db = new Models.Database();
					db.DeleteProperty(lngID);
				}
			}
			return RedirectToAction("MyProperties"); //this should never happen
		}



		public ActionResult Gallery()
		{
			Models.User u = new Models.User();
			u = u.GetUserSession();
			if (u.IsAuthenticated)
			{
				Models.Database db = new Models.Database();
				u.Images = db.GetUserImages(u.UID);
			}
			return View(u);
		}

		[HttpPost]
		public ActionResult Gallery(IEnumerable<HttpPostedFileBase> files)
		{
			Models.User u = new Models.User();
			u = u.GetUserSession();
			foreach (var file in files)
			{
				u.AddGalleryImage(file);
			}
			return Json("file(s) uploaded successfully");
		}

		[HttpPost]
		public JsonResult DeleteImage(long UID, long ID)
		{
			try
			{
				string type = string.Empty;
				Models.Database db = new Models.Database();
				if (db.DeleteUserImage(ID)) return Json(new { Status = 1 }); //deleted
				return Json(new { Status = 0 }); //not deleted
			}
			catch (Exception)
			{
				return Json(new { Status = -1 }); //error
			}
		}

		public ActionResult MyProperties()
		{
			Models.User u = new Models.User();
			u = u.GetUserSession();
			if (u.IsAuthenticated)
				u.Properties = u.GetProperties();
			return View(u);
		}

		public ActionResult Index()
		{
			Models.User u = new Models.User();
			u = u.GetUserSession();
			if (u.IsAuthenticated)
			{
				Models.Database db = new Models.Database();
				List<Models.Image> images = new List<Models.Image>();
				images = db.GetUserImages(u.UID, 0, true);
				u.UserImage = new Models.Image();
				if (images.Count > 0) u.UserImage = images[0];
			}
			return View(u);
		}

		[HttpPost]
		public ActionResult Index(HttpPostedFileBase UserImage, FormCollection col)
		{
			try
			{
				Models.User u = new Models.User();
				u = u.GetUserSession();

				u.FirstName = col["FirstName"];
				u.LastName = col["LastName"];
				u.Email = col["Email"];
				u.UserID = col["UserID"];
				u.Password = col["Password"];

				if (u.FirstName.Length == 0 || u.LastName.Length == 0 || u.Email.Length == 0 || u.UserID.Length == 0 || u.Password.Length == 0)
				{
					u.ActionType = Models.User.ActionTypes.RequiredFieldsMissing;
					return View(u);
				}
				else
				{
					if (col["btnSubmit"] == "update")
					{ //update button pressed
						u.Save();

						u.UserImage = new Models.Image();
						u.UserImage.ImageID = System.Convert.ToInt32(col["UserImage.ImageID"]);

						if (UserImage != null)
						{
							u.UserImage = new Models.Image();
							u.UserImage.ImageID = Convert.ToInt32(col["UserImage.ImageID"]);
							u.UserImage.Primary = true;
							u.UserImage.FileName = Path.GetFileName(UserImage.FileName);
							if (u.UserImage.IsImageFile())
							{
								u.UserImage.Size = UserImage.ContentLength;
								Stream stream = UserImage.InputStream;
								BinaryReader binaryReader = new BinaryReader(stream);
								u.UserImage.ImageData = binaryReader.ReadBytes((int)stream.Length);
								u.UpdatePrimaryImage();
							}
						}

						u.SaveUserSession();
						return RedirectToAction("Index");
					}
					return View(u);
				}
			}
			catch (Exception)
			{
				Models.User u = new Models.User();
				return View(u);
			}

		}

		public ActionResult SignIn()
		{
			Models.User u = new Models.User();
			return View(u);
		}

		[HttpPost]
		public ActionResult SignIn(FormCollection col)
		{
			try
			{
				Models.User u = new Models.User();

				if (col["btnSubmit"] == "signin")
				{
					u.UserID = col["UserID"];
					u.Password = col["Password"];

					if (u.UserID.Length == 0 || u.Password.Length == 0)
					{
						u.ActionType = Models.User.ActionTypes.RequiredFieldsMissing;
						return View(u);				
					}
					else
					{
						u = u.Login();
						if (u != null && u.UID > 0)
						{
							u.SaveUserSession();
							return RedirectToAction("Index");
						}
						else
						{
							u = new Models.User();
							u.UserID = col["UserID"];
							u.ActionType = Models.User.ActionTypes.LoginFailed;
						}
					}
				}
				return View(u);
			}
			catch (Exception)
			{
				Models.User u = new Models.User();
				return View(u);
			}
		}

		public ActionResult SignUp()
		{
			Models.User u = new Models.User();
			return View(u);
		}

		[HttpPost]
		public ActionResult SignUp(FormCollection col)
		{
			try
			{
				Models.User u = new Models.User();

				u.FirstName = col["FirstName"];
				u.LastName = col["LastName"];
				u.Email = col["Email"];
				u.UserID = col["UserID"];
				u.Password = col["Password"];

				if (u.FirstName.Length == 0 || u.LastName.Length == 0 || u.Email.Length == 0 || u.UserID.Length == 0 || u.Password.Length == 0)
				{
					u.ActionType = Models.User.ActionTypes.RequiredFieldsMissing;
					return View(u);
				}
				else
				{
					if (col["btnSubmit"] == "signup")
					{ //sign up button pressed
						Models.User.ActionTypes at = Models.User.ActionTypes.NoType;
						at = u.Save();
						switch (at)
						{
							case Models.User.ActionTypes.InsertSuccessful:
								u.SaveUserSession();
								return RedirectToAction("Index");
							//break;
							default:
								return View(u);
								//break;
						}
					}
					else
					{
						return View(u);
					}
				}
			}
			catch (Exception)
			{
				Models.User u = new Models.User();
				return View(u);
			}
		}

		public ActionResult SignOut()
		{
			Models.User u = new Models.User();
			u.RemoveUserSession();
			return RedirectToAction("Index", "Home");
		}
	}
}
