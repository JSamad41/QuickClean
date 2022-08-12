using System;
using System.Collections.Generic;
using System.Web;
using System.IO;

namespace QuickClean.Models {
	public class Property {
		public long ID = 0;
		public string Title = string.Empty;
		
		public string Description = string.Empty;
		public DateTime Start;
		public DateTime End;
		public User User;
		public Location Location;

		public int CleanerID;
		public string CleanerName;
		public string CleanerEmail;

		public string OwnerEmail;


		public List<Image> Images;
		public Image PropertyImage;

		public ActionTypes ActionType = ActionTypes.NoType;
		public bool IsActive = true;
		public int AverageRating = 0;
		public int TotalLikes = 0;

		//added cleaning options
		public bool carpetCleaning = false;
		public bool baseboardCleaning = false;
		public bool laundryCleaning = false;
		public bool dishCleaning = false;
		public string Details = string.Empty;
		public string Compensation;
        public string squareFootage { get; set; }
        public string numberOfBedrooms;
        public string numberOfBathrooms;
        public bool standardCleaning = true;
        public int intCompensation = 0;


        public Image PrimaryImage {
			get {
				if (this.Images != null) {
					foreach (Image i in this.Images) {
						if (i.Primary) return i;
					}
				}
				return new Image();
			}
		}

		public bool Editable {
			get {
				if (this.Start == null) return true;
				if (this.Start > DateTime.Now) return true;
				return false;
			}
		}

		public Property GetProperty(long ID) {
			try {
				Database db = new Database();
				List<Property> properties = new List<Property>();
				if (this.User == null) {
					properties = db.GetProperties(ID);
				}
				else {
					properties = db.GetProperties(ID, this.User.UID);
				}
				return properties[0];
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public Property GetBooking(long ID)
		{
			try
			{
				Database db = new Database();
				List<Property> properties = new List<Property>();
				if (this.User == null)
				{
					properties = db.GetBookings(ID);
				}
				else
				{
					properties = db.GetBookings(ID, this.User.UID);
				}
				return properties[0];
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public Property.ActionTypes Save() {
			try {
				Database db = new Database();
				if (ID == 0) { //insert new user
					this.ActionType = db.InsertProperty(this);
				}
				else {
					this.ActionType = db.UpdateProperty(this);
				}
				return this.ActionType;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

	
		/// ADD CLEANER
		public Property.ActionTypes SaveCleaner()
		{
			try
			{
				Database db = new Database();
				
				this.ActionType = db.UpdatePropertyCleaner(this);
		
				return this.ActionType;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public sbyte AddPropertyImage(HttpPostedFileBase f) {
			try {
				this.PropertyImage = new Image();
				this.PropertyImage.Primary = false;
				this.PropertyImage.FileName = Path.GetFileName(f.FileName);

				if (this.PropertyImage.IsImageFile()) {
					this.PropertyImage.Size = f.ContentLength;
					Stream stream = f.InputStream;
					BinaryReader binaryReader = new BinaryReader(stream);
					this.PropertyImage.ImageData = binaryReader.ReadBytes((int)stream.Length);
					this.UpdatePrimaryImage();
				}
				return 0;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }

		}


		public sbyte UpdatePrimaryImage() {
			try {
				Database db = new Database();
				long NewID;
				if (this.PropertyImage.ImageID == 0) {
					NewID = db.InsertPropertyImage(this);
					if (NewID > 0) PropertyImage.ImageID = NewID;
				}
				else {
					db.UpdatePropertyImage(this);
				}
				return 0;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public enum ActionTypes {
			NoType = 0,
			InsertSuccessful = 1,
			UpdateSuccessful = 2,
			DuplicateEmail = 3,
			DuplicateUserID = 4,
			Unknown = 5,
			RequiredFieldsMissing = 6
		}


	}
}
