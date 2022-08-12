using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;

namespace QuickClean.Models
{
    public class Database
    {
        public bool InsertReport(long UID, long IDToReport, int ProblemID)
        {
            try
            {
                SqlConnection cn = null;
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("INSERT_REPORTS", cn);

                SetParameter(ref cm, "@uid", UID, SqlDbType.BigInt);
                SetParameter(ref cm, "@id_to_report", IDToReport, SqlDbType.BigInt);
                SetParameter(ref cm, "@problem_id", ProblemID, SqlDbType.TinyInt);

                SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

                cm.ExecuteReader();

                return true;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public int RateProperty(long UID, long ID, long Rating)
        {
            try
            {
                SqlConnection cn = null;
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("UPDATE_PROPERTY_RATING", cn);
                int intReturnValue = -1;

                SetParameter(ref cm, "@rating_id", null, SqlDbType.BigInt, Direction: ParameterDirection.Output);
                SetParameter(ref cm, "@uid", UID, SqlDbType.BigInt);
                SetParameter(ref cm, "@property_id", ID, SqlDbType.BigInt);
                SetParameter(ref cm, "@rating", Rating, SqlDbType.TinyInt);

                SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

                cm.ExecuteReader();

                //1 = new rate added
                //2 = existing rate updated
                intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
                CloseDBConnection(ref cn);
                return intReturnValue;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public List<Rating> GetPropertyRatings(long UID)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlDataAdapter da = new SqlDataAdapter("SELECT_USER_PROPERTY_RATINGS", cn);
                List<Rating> ratings = new List<Rating>();

                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                SetParameter(ref da, "@uid", UID, SqlDbType.BigInt);

                try
                {
                    da.Fill(ds);
                }
                catch (Exception ex2)
                {
                    //SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
                }
                finally { CloseDBConnection(ref cn); }

                if (ds.Tables[0].Rows.Count != 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Rating r = new Rating();
                        r.Type = Rating.Types.Property;
                        r.ID = (long)dr["PropertyID"];
                        r.Rate = (byte)dr["Rating"];
                        ratings.Add(r);
                    }
                }
                return ratings;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public List<Property> GetActiveProperties()
        {
            try
            {
                DataSet ds = new DataSet();
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlDataAdapter da = new SqlDataAdapter("SELECT_PROPERTIES_ACTIVE", cn);
                List<Property> properties = new List<Property>();

                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                try
                {
                    da.Fill(ds);
                }
                catch (Exception ex2)
                {
                    //SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
                }
                finally { CloseDBConnection(ref cn); }

                if (ds.Tables[0].Rows.Count != 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Property e = new Property();
                        e.ID = (long)dr["PropertyID"];
                        if (dr["StartDate"] != null) e.Start = (DateTime)dr["StartDate"];

                        if (dr["IsActive"].ToString() == "N") e.IsActive = false;
                        e.AverageRating = (int)dr["AvgRating"];
                        e.squareFootage = (string)dr["squareFootage"];
                        e.numberOfBedrooms = (string)dr["numberOfBedrooms"];
                        e.numberOfBathrooms = (string)dr["numberOfBathrooms"];

                        if (dr["standardCleaning"].ToString() == "N") e.standardCleaning = false;

                        if (dr["carpetCleaning"].ToString() == "N") e.carpetCleaning = false;
                        if (dr["baseboardCleaning"].ToString() == "N") e.baseboardCleaning = false;
                        if (dr["laundryCleaning"].ToString() == "N") e.laundryCleaning = false;
                        if (dr["dishCleaning"].ToString() == "N") e.dishCleaning = false;

                        e.Details = (string)dr["Details"];
                        e.Compensation = (string)dr["Compensation"];

                        e.Location = new Location();

                        e.Location.Address = new Address();
                        e.Location.Address.Address1 = (string)dr["Address1"];
                        e.Location.Address.Address2 = (string)dr["Address2"];
                        e.Location.Address.City = (string)dr["City"];
                        e.Location.Address.State = (string)dr["State"];
                        e.Location.Address.Zip = (string)dr["Zip"];

                        e.User = new User();
                        e.User.UID = (long)dr["OwnerUID"];
                        e.User.FirstName = (string)dr["FirstName"];
                        e.User.LastName = (string)dr["LastName"];


                        List<Image> images = GetPropertyImages(e.ID, 0, true);
                        if (images.Count > 0) e.PropertyImage = images[0];

                        properties.Add(e);
                    }
                }
                return properties;

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public int TogglePropertyLike(long UID, long ID)
        {
            try
            {
                SqlConnection cn = null;
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("TOGGLE_PROPERTY_LIKE", cn);
                int intReturnValue = -1;

                SetParameter(ref cm, "@uid", UID, SqlDbType.BigInt);
                SetParameter(ref cm, "@property_id", ID, SqlDbType.BigInt);

                SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

                cm.ExecuteReader();

                //1 = added
                //0 = removed
                intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
                CloseDBConnection(ref cn);
                return intReturnValue;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public List<Like> GetPropertyLikes(long UID)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlDataAdapter da = new SqlDataAdapter("SELECT_USER_PROPERTY_LIKES", cn);
                List<Like> likes = new List<Like>();

                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                SetParameter(ref da, "@uid", UID, SqlDbType.BigInt);

                try
                {
                    da.Fill(ds);
                }
                catch (Exception ex2)
                {
                    //SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
                }
                finally { CloseDBConnection(ref cn); }

                if (ds.Tables[0].Rows.Count != 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Like l = new Like();
                        l.Type = Like.Types.Property;
                        l.ID = (long)dr["PropertyID"];
                        likes.Add(l);
                    }
                }
                return likes;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }



        public bool DeleteProperty(long ID)
        {
            try
            {
                SqlConnection cn = null;
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("DELETE_PROPERTY", cn);
                int intReturnValue = -1;

                SetParameter(ref cm, "@id", ID, SqlDbType.BigInt);
                SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

                cm.ExecuteReader();

                intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
                CloseDBConnection(ref cn);

                if (intReturnValue == 1) return true;
                return false;

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public bool DeletePropertyImage(long ID)
        {
            try
            {
                SqlConnection cn = null;
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("DELETE_PROPERTY_IMAGE", cn);
                int intReturnValue = -1;

                SetParameter(ref cm, "@id", ID, SqlDbType.BigInt);
                SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

                cm.ExecuteReader();

                intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
                CloseDBConnection(ref cn);

                if (intReturnValue == 1) return true;
                return false;

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public Property.ActionTypes InsertProperty(Property e)
        {
            try
            {
                SqlConnection cn = null;
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("INSERT_PROPERTIES", cn);
                int intReturnValue = -1;

                SetParameter(ref cm, "@id", e.ID, SqlDbType.BigInt, Direction: ParameterDirection.Output);
                SetParameter(ref cm, "@owner_uid", e.User.UID, SqlDbType.BigInt);
                SetParameter(ref cm, "@start_date", e.Start, SqlDbType.DateTime);
                SetParameter(ref cm, "@address1", e.Location.Address.Address1, SqlDbType.NVarChar);
                SetParameter(ref cm, "@address2", e.Location.Address.Address2, SqlDbType.NVarChar);
                SetParameter(ref cm, "@city", e.Location.Address.City, SqlDbType.NVarChar);
                SetParameter(ref cm, "@state", e.Location.Address.State, SqlDbType.NVarChar);
                SetParameter(ref cm, "@zip", e.Location.Address.Zip, SqlDbType.NVarChar);
                SetParameter(ref cm, "@squareFootage", e.squareFootage, SqlDbType.NVarChar);
                SetParameter(ref cm, "@numberOfBedrooms", e.numberOfBedrooms, SqlDbType.NVarChar);
                SetParameter(ref cm, "@numberOfBathrooms", e.numberOfBathrooms, SqlDbType.NVarChar);
                SetParameter(ref cm, "@details", e.Details, SqlDbType.NVarChar);
                SetParameter(ref cm, "@compensation", e.Compensation, SqlDbType.NVarChar);

                if (e.standardCleaning)
                    SetParameter(ref cm, "@standardCleaning", "Y", SqlDbType.Char);
                else
                    SetParameter(ref cm, "@standardCleaning", "N", SqlDbType.Char);

                if (e.carpetCleaning)
                    SetParameter(ref cm, "@carpetCleaning", "Y", SqlDbType.Char);
                else
                    SetParameter(ref cm, "@carpetCleaning", "N", SqlDbType.Char);

                if (e.baseboardCleaning)
                    SetParameter(ref cm, "@baseboardCleaning", "Y", SqlDbType.Char);
                else
                    SetParameter(ref cm, "@baseboardCleaning", "N", SqlDbType.Char);

                if (e.laundryCleaning)
                    SetParameter(ref cm, "@laundryCleaning", "Y", SqlDbType.Char);
                else
                    SetParameter(ref cm, "@laundryCleaning", "N", SqlDbType.Char);

                if (e.dishCleaning)
                    SetParameter(ref cm, "@dishCleaning", "Y", SqlDbType.Char);
                else
                    SetParameter(ref cm, "@dishCleaning", "N", SqlDbType.Char);


                if (e.IsActive)
                    SetParameter(ref cm, "@is_active", "Y", SqlDbType.Char);
                else
                    SetParameter(ref cm, "@is_active", "N", SqlDbType.Char);

                SetParameter(ref cm, "ReturnValue", 0, SqlDbType.TinyInt, Direction: ParameterDirection.ReturnValue);

                cm.ExecuteReader();

                intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
                CloseDBConnection(ref cn);

                switch (intReturnValue)
                {
                    case 1: // new property created
                        e.ID = (long)cm.Parameters["@id"].Value;
                        return Property.ActionTypes.InsertSuccessful;
                    default:
                        return Property.ActionTypes.Unknown;
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public List<Image> GetPropertyImages(long PropertyID = 0, long PropertyImageID = 0, bool PrimaryOnly = false)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlDataAdapter da = new SqlDataAdapter("SELECT_PROPERTY_IMAGES", cn);
                List<Image> imgs = new List<Image>();

                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                if (PropertyID > 0) SetParameter(ref da, "@property_id", PropertyID, SqlDbType.BigInt);
                if (PropertyImageID > 0) SetParameter(ref da, "@property_image_id", PropertyImageID, SqlDbType.BigInt);
                if (PrimaryOnly) SetParameter(ref da, "@primary_only", "Y", SqlDbType.Char);

                try
                {
                    da.Fill(ds);
                }
                catch (Exception ex2)
                {
                    //SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
                }
                finally { CloseDBConnection(ref cn); }

                if (ds.Tables[0].Rows.Count != 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Image i = new Image();
                        i.ImageID = (long)dr["PropertyImageID"];
                        i.ImageData = (byte[])dr["Image"];
                        i.FileName = (string)dr["FileName"];
                        i.Size = (long)dr["ImageSize"];
                        if (dr["PrimaryImage"].ToString() == "Y")
                            i.Primary = true;
                        else
                            i.Primary = false;
                        imgs.Add(i);
                    }
                }
                return imgs;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public List<Property> GetProperties(long ID = 0, long UID = 0)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlDataAdapter da = new SqlDataAdapter("SELECT_PROPERTIES", cn);
                List<Property> properties = new List<Property>();

                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                if (ID > 0) SetParameter(ref da, "@id", ID, SqlDbType.BigInt);
                if (UID > 0) SetParameter(ref da, "@uid", UID, SqlDbType.BigInt);
                //if (LocationTitle != "") SetParameter(ref da, "@location_title", LocationTitle, SqlDbType.NVarChar);

                try
                {
                    da.Fill(ds);
                }
                catch (Exception ex2)
                {
                    //SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
                }
                finally { CloseDBConnection(ref cn); }

                if (ds.Tables[0].Rows.Count != 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Property e = new Property();
                        e.ID = (long)dr["PropertyID"];
                        if (dr["StartDate"] != null) e.Start = (DateTime)dr["StartDate"];

                        if (dr["IsActive"].ToString() == "N") e.IsActive = false;

                        e.squareFootage = (string)dr["squareFootage"];
                        e.numberOfBedrooms = (string)dr["numberOfBedrooms"];
                        e.numberOfBathrooms = (string)dr["numberOfBathrooms"];

                        if (dr["standardCleaning"].ToString() == "N") e.standardCleaning = false;

                        if (dr["carpetCleaning"].ToString() == "N") e.carpetCleaning = false; else e.carpetCleaning = true;
                        if (dr["baseboardCleaning"].ToString() == "N") e.baseboardCleaning = false; else e.baseboardCleaning = true;
                        if (dr["laundryCleaning"].ToString() == "N") e.laundryCleaning = false; else e.laundryCleaning = true;
                        if (dr["dishCleaning"].ToString() == "N") e.dishCleaning = false; else e.dishCleaning = true;

                        e.Details = (string)dr["Details"];
                        e.Compensation = (string)dr["Compensation"];

                        e.Location = new Location();

                        e.Location.Address = new Address();
                        e.Location.Address.Address1 = (string)dr["Address1"];
                        e.Location.Address.Address2 = (string)dr["Address2"];
                        e.Location.Address.City = (string)dr["City"];
                        e.Location.Address.State = (string)dr["State"];
                        e.Location.Address.Zip = (string)dr["Zip"];

                        e.User = new User();
                        e.User.UID = (long)dr["UID"];
                        e.User.UserID = (string)dr["UserID"];
                        e.User.FirstName = (string)dr["FirstName"];
                        e.User.LastName = (string)dr["LastName"];
                        e.User.Email = (string)dr["Email"];


                        List<Image> images = GetPropertyImages(e.ID, 0, true);
                        if (images.Count > 0) e.PropertyImage = images[0];

                        properties.Add(e);
                    }
                }
                return properties;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public List<Property> GetBookings(long ID = 0, long UID = 0)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlDataAdapter da = new SqlDataAdapter("SELECT_BOOKINGS", cn);
                List<Property> properties = new List<Property>();

                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                if (ID > 0) SetParameter(ref da, "@id", ID, SqlDbType.BigInt);
                if (UID > 0) SetParameter(ref da, "@uid", UID, SqlDbType.BigInt);
                //if (LocationTitle != "") SetParameter(ref da, "@location_title", LocationTitle, SqlDbType.NVarChar);

                try
                {
                    da.Fill(ds);
                }
                catch (Exception ex2)
                {
                    //SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
                }
                finally { CloseDBConnection(ref cn); }

                if (ds.Tables[0].Rows.Count != 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Property e = new Property();
                        e.ID = (long)dr["PropertyID"];
                        if (dr["StartDate"] != null) e.Start = (DateTime)dr["StartDate"];

                        if (dr["IsActive"].ToString() == "N") e.IsActive = false;

                        e.squareFootage = (string)dr["squareFootage"];
                        e.numberOfBedrooms = (string)dr["numberOfBedrooms"];
                        e.numberOfBathrooms = (string)dr["numberOfBathrooms"];

                        if (dr["standardCleaning"].ToString() == "N") e.standardCleaning = false;

                        if (dr["carpetCleaning"].ToString() == "N") e.carpetCleaning = false; else e.carpetCleaning = true;
                        if (dr["baseboardCleaning"].ToString() == "N") e.baseboardCleaning = false; else e.baseboardCleaning = true;
                        if (dr["laundryCleaning"].ToString() == "N") e.laundryCleaning = false; else e.laundryCleaning = true;
                        if (dr["dishCleaning"].ToString() == "N") e.dishCleaning = false; else e.dishCleaning = true;

                        e.Details = (string)dr["Details"];
                        e.Compensation = (string)dr["Compensation"];

                        e.Location = new Location();

                        e.Location.Address = new Address();
                        e.Location.Address.Address1 = (string)dr["Address1"];
                        e.Location.Address.Address2 = (string)dr["Address2"];
                        e.Location.Address.City = (string)dr["City"];
                        e.Location.Address.State = (string)dr["State"];
                        e.Location.Address.Zip = (string)dr["Zip"];

                        e.User = new User();
                        e.User.UID = (long)dr["UID"];
                        e.User.UserID = (string)dr["UserID"];
                        e.User.FirstName = (string)dr["FirstName"];
                        e.User.LastName = (string)dr["LastName"];
                        e.User.Email = (string)dr["Email"];


                        List<Image> images = GetPropertyImages(e.ID, 0, true);
                        if (images.Count > 0) e.PropertyImage = images[0];

                        properties.Add(e);
                    }
                }
                return properties;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public long InsertPropertyImage(Property e)
        {
            try
            {
                SqlConnection cn = null;
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("INSERT_PROPERTY_IMAGE", cn);

                SetParameter(ref cm, "@property_image_id", null, SqlDbType.BigInt, Direction: ParameterDirection.Output);
                SetParameter(ref cm, "@property_id", e.ID, SqlDbType.BigInt);
                if (e.PropertyImage.Primary)
                    SetParameter(ref cm, "@primary_image", "Y", SqlDbType.Char);
                else
                    SetParameter(ref cm, "@primary_image", "N", SqlDbType.Char);

                SetParameter(ref cm, "@image", e.PropertyImage.ImageData, SqlDbType.VarBinary);
                SetParameter(ref cm, "@file_name", e.PropertyImage.FileName, SqlDbType.NVarChar);
                SetParameter(ref cm, "@image_size", e.PropertyImage.Size, SqlDbType.BigInt);

                cm.ExecuteReader();
                CloseDBConnection(ref cn);
                return (long)cm.Parameters["@property_image_id"].Value;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public Property.ActionTypes UpdateProperty(Property e)
        {
            try
            {
                SqlConnection cn = null;
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("UPDATE_PROPERTY", cn);
                int intReturnValue = -1;

                SetParameter(ref cm, "@id", e.ID, SqlDbType.BigInt);
                SetParameter(ref cm, "@owner_uid", e.User.UID, SqlDbType.BigInt);
                SetParameter(ref cm, "@start", e.Start, SqlDbType.DateTime);
                SetParameter(ref cm, "@address1", e.Location.Address.Address1, SqlDbType.NVarChar);
                SetParameter(ref cm, "@address2", e.Location.Address.Address2, SqlDbType.NVarChar);
                SetParameter(ref cm, "@city", e.Location.Address.City, SqlDbType.NVarChar);
                SetParameter(ref cm, "@state", e.Location.Address.State, SqlDbType.NVarChar);
                SetParameter(ref cm, "@zip", e.Location.Address.Zip, SqlDbType.NVarChar);
                SetParameter(ref cm, "@squareFootage", e.squareFootage, SqlDbType.NVarChar);
                SetParameter(ref cm, "@numberOfBedrooms", e.numberOfBedrooms, SqlDbType.NVarChar);
                SetParameter(ref cm, "@numberOfBathrooms", e.numberOfBathrooms, SqlDbType.NVarChar);
                SetParameter(ref cm, "@details", e.Details, SqlDbType.NVarChar);
                SetParameter(ref cm, "@compensation", e.Compensation, SqlDbType.NVarChar);

                if (e.standardCleaning)
                    SetParameter(ref cm, "@standardCleaning", "Y", SqlDbType.Char);
                else
                    SetParameter(ref cm, "@standardCleaning", "N", SqlDbType.Char);

                if (e.carpetCleaning)
                    SetParameter(ref cm, "@carpetCleaning", "Y", SqlDbType.Char);
                else
                    SetParameter(ref cm, "@carpetCleaning", "N", SqlDbType.Char);

                if (e.baseboardCleaning)
                    SetParameter(ref cm, "@baseboardCleaning", "Y", SqlDbType.Char);
                else
                    SetParameter(ref cm, "@baseboardCleaning", "N", SqlDbType.Char);

                if (e.laundryCleaning)
                    SetParameter(ref cm, "@laundryCleaning", "Y", SqlDbType.Char);
                else
                    SetParameter(ref cm, "@laundryCleaning", "N", SqlDbType.Char);

                if (e.dishCleaning)
                    SetParameter(ref cm, "@dishCleaning", "Y", SqlDbType.Char);
                else
                    SetParameter(ref cm, "@dishCleaning", "N", SqlDbType.Char);

                if (e.IsActive)
                    SetParameter(ref cm, "@is_active", "Y", SqlDbType.Char);
                else
                    SetParameter(ref cm, "@is_active", "N", SqlDbType.Char);

                SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

                cm.ExecuteReader();

                intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
                CloseDBConnection(ref cn);

                switch (intReturnValue)
                {
                    case 1: //new updated
                        return Property.ActionTypes.UpdateSuccessful;
                    default:
                        return Property.ActionTypes.Unknown;
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public Property.ActionTypes UpdatePropertyCleaner(Property e)
        {
            try
            {
                SqlConnection cn = null;
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("UPDATE_PROPERTY_CLEANER", cn);
                int intReturnValue = -1;

                SetParameter(ref cm, "@id", e.ID, SqlDbType.BigInt);
                SetParameter(ref cm, "@cleaner_uid", e.User.UID, SqlDbType.BigInt);
              

                SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

                cm.ExecuteReader();

                intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
                CloseDBConnection(ref cn);

                switch (intReturnValue)
                {
                    case 1: //new updated
                        return Property.ActionTypes.UpdateSuccessful;
                    default:
                        return Property.ActionTypes.Unknown;
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public long UpdatePropertyImage(Property e)
        {
            try
            {
                SqlConnection cn = null;
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("UPDATE_PROPERTY_IMAGE", cn);

                SetParameter(ref cm, "@property_image_id", e.PropertyImage.ImageID, SqlDbType.BigInt);
                if (e.PropertyImage.Primary)
                    SetParameter(ref cm, "@primary_image", "Y", SqlDbType.Char);
                else
                    SetParameter(ref cm, "@primary_image", "N", SqlDbType.Char);

                SetParameter(ref cm, "@image", e.PropertyImage.ImageData, SqlDbType.VarBinary);
                SetParameter(ref cm, "@file_name", e.PropertyImage.FileName, SqlDbType.NVarChar);
                SetParameter(ref cm, "@image_size", e.PropertyImage.Size, SqlDbType.BigInt);

                cm.ExecuteReader();
                CloseDBConnection(ref cn);

                return 0; //success	
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public long InsertUserImage(User u)
        {
            try
            {
                SqlConnection cn = null;
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("INSERT_USER_IMAGE", cn);

                SetParameter(ref cm, "@user_image_id", null, SqlDbType.BigInt, Direction: ParameterDirection.Output);
                SetParameter(ref cm, "@uid", u.UID, SqlDbType.BigInt);
                if (u.UserImage.Primary)
                    SetParameter(ref cm, "@primary_image", "Y", SqlDbType.Char);
                else
                    SetParameter(ref cm, "@primary_image", "N", SqlDbType.Char);

                SetParameter(ref cm, "@image", u.UserImage.ImageData, SqlDbType.VarBinary);
                SetParameter(ref cm, "@file_name", u.UserImage.FileName, SqlDbType.NVarChar);
                SetParameter(ref cm, "@image_size", u.UserImage.Size, SqlDbType.BigInt);

                cm.ExecuteReader();
                CloseDBConnection(ref cn);
                return (long)cm.Parameters["@user_image_id"].Value;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public long UpdateUserImage(User u)
        {
            try
            {
                SqlConnection cn = null;
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("UPDATE_USER_IMAGE", cn);

                SetParameter(ref cm, "@user_image_id", u.UserImage.ImageID, SqlDbType.BigInt);
                if (u.UserImage.Primary)
                    SetParameter(ref cm, "@primary_image", "Y", SqlDbType.Char);
                else
                    SetParameter(ref cm, "@primary_image", "N", SqlDbType.Char);

                SetParameter(ref cm, "@image", u.UserImage.ImageData, SqlDbType.VarBinary);
                SetParameter(ref cm, "@file_name", u.UserImage.FileName, SqlDbType.NVarChar);
                SetParameter(ref cm, "@image_size", u.UserImage.Size, SqlDbType.BigInt);

                cm.ExecuteReader();
                CloseDBConnection(ref cn);

                return 0; //success	
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public List<Image> GetUserImages(long UID = 0, long UserImageID = 0, bool PrimaryOnly = false)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlDataAdapter da = new SqlDataAdapter("SELECT_USER_IMAGES", cn);
                List<Image> imgs = new List<Image>();

                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                if (UID > 0) SetParameter(ref da, "@uid", UID, SqlDbType.BigInt);
                if (UserImageID > 0) SetParameter(ref da, "@user_image_id", UserImageID, SqlDbType.BigInt);
                if (PrimaryOnly) SetParameter(ref da, "@primary_only", "Y", SqlDbType.Char);

                try
                {
                    da.Fill(ds);
                }
                catch (Exception ex2)
                {
                    throw new Exception(ex2.Message);
                }
                finally { CloseDBConnection(ref cn); }

                if (ds.Tables[0].Rows.Count != 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Image i = new Image();
                        i.ImageID = (long)dr["UserImageID"];
                        i.ImageData = (byte[])dr["Image"];
                        i.FileName = (string)dr["FileName"];
                        i.Size = (long)dr["ImageSize"];
                        if (dr["PrimaryImage"].ToString() == "Y")
                            i.Primary = true;
                        else
                            i.Primary = false;
                        imgs.Add(i);
                    }
                }
                return imgs;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public bool DeleteUserImage(long ID)
        {
            try
            {
                SqlConnection cn = null;
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("DELETE_USER_IMAGE", cn);
                int intReturnValue = -1;

                SetParameter(ref cm, "@id", ID, SqlDbType.BigInt);
                SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

                cm.ExecuteReader();

                intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
                CloseDBConnection(ref cn);

                if (intReturnValue == 1) return true;
                return false;

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public User.ActionTypes InsertUser(User u)
        {
            try
            {
                SqlConnection cn = null;
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("INSERT_USER", cn);
                int intReturnValue = -1;

                SetParameter(ref cm, "@uid", u.UID, SqlDbType.BigInt, Direction: ParameterDirection.Output);
                SetParameter(ref cm, "@user_id", u.UserID, SqlDbType.NVarChar);
                SetParameter(ref cm, "@password", u.Password, SqlDbType.NVarChar);
                SetParameter(ref cm, "@first_name", u.FirstName, SqlDbType.NVarChar);
                SetParameter(ref cm, "@last_name", u.LastName, SqlDbType.NVarChar);
                SetParameter(ref cm, "@address", u.LastName, SqlDbType.NVarChar);
                SetParameter(ref cm, "@city", u.LastName, SqlDbType.NVarChar);
                SetParameter(ref cm, "@state", u.LastName, SqlDbType.NVarChar);
                SetParameter(ref cm, "@zip", u.LastName, SqlDbType.NVarChar);
                SetParameter(ref cm, "@phone_number", u.LastName, SqlDbType.NVarChar);
                SetParameter(ref cm, "@email", u.Email, SqlDbType.NVarChar);
                SetParameter(ref cm, "@role", u.Role, SqlDbType.NVarChar);

                SetParameter(ref cm, "ReturnValue", 0, SqlDbType.TinyInt, Direction: ParameterDirection.ReturnValue);

                cm.ExecuteReader();

                intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
                CloseDBConnection(ref cn);

                switch (intReturnValue)
                {
                    case 1: // new user created
                        u.UID = (long)cm.Parameters["@uid"].Value;
                        return User.ActionTypes.InsertSuccessful;
                    case -1:
                        return User.ActionTypes.DuplicateEmail;
                    case -2:
                        return User.ActionTypes.DuplicateUserID;
                    default:
                        return User.ActionTypes.Unknown;
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public User Login(User u)
        {
            try
            {
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlDataAdapter da = new SqlDataAdapter("LOGIN", cn);
                DataSet ds;
                User newUser = null;

                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                SetParameter(ref da, "@user_id", u.UserID, SqlDbType.NVarChar);
                SetParameter(ref da, "@password", u.Password, SqlDbType.NVarChar);

                try
                {
                    ds = new DataSet();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        newUser = new User();
                        DataRow dr = ds.Tables[0].Rows[0];
                        newUser.UID = (long)dr["UID"];
                        newUser.UserID = u.UserID;
                        newUser.Password = u.Password;
                        newUser.FirstName = (string)dr["FirstName"];
                        newUser.LastName = (string)dr["LastName"];
                        newUser.Address = (string)dr["Address"];
                        newUser.City = (string)dr["City"];
                        newUser.State = (string)dr["State"];
                        newUser.Zip = (string)dr["Zip"];
                        newUser.PhoneNumber = (string)dr["PhoneNumber"];
                        newUser.Email = (string)dr["Email"];
                        newUser.Role = (string)dr["Role"];
                    }
                }
                catch (Exception ex) { throw new Exception(ex.Message); }
                finally
                {
                    CloseDBConnection(ref cn);
                }
                return newUser; //alls well in the world
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public User.ActionTypes UpdateUser(User u)
        {
            try
            {
                SqlConnection cn = null;
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("UPDATE_USER", cn);
                int intReturnValue = -1;

                SetParameter(ref cm, "@uid", u.UID, SqlDbType.BigInt);
                SetParameter(ref cm, "@user_id", u.UserID, SqlDbType.NVarChar);
                SetParameter(ref cm, "@password", u.Password, SqlDbType.NVarChar);
                SetParameter(ref cm, "@first_name", u.FirstName, SqlDbType.NVarChar);
                SetParameter(ref cm, "@last_name", u.LastName, SqlDbType.NVarChar);
                SetParameter(ref cm, "@address", u.Address, SqlDbType.NVarChar);
                SetParameter(ref cm, "@city", u.City, SqlDbType.NVarChar);
                SetParameter(ref cm, "@state", u.State, SqlDbType.NVarChar);
                SetParameter(ref cm, "@zip", u.Zip, SqlDbType.NVarChar);
                SetParameter(ref cm, "@phone_number", u.PhoneNumber, SqlDbType.NVarChar);
                SetParameter(ref cm, "@email", u.Email, SqlDbType.NVarChar);
                SetParameter(ref cm, "@role", u.Role, SqlDbType.NVarChar);

                SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

                cm.ExecuteReader();

                intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
                CloseDBConnection(ref cn);

                switch (intReturnValue)
                {
                    case 1: //new updated
                        return User.ActionTypes.UpdateSuccessful;
                    default:
                        return User.ActionTypes.Unknown;
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        private bool GetDBConnection(ref SqlConnection SQLConn)
        {
            try
            {
                if (SQLConn == null) SQLConn = new SqlConnection();
                if (SQLConn.State != ConnectionState.Open)
                {
                    SQLConn.ConnectionString = ConfigurationManager.AppSettings["AppDBConnect"];
                    SQLConn.Open();
                }
                return true;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        private bool CloseDBConnection(ref SqlConnection SQLConn)
        {
            try
            {
                if (SQLConn.State != ConnectionState.Closed)
                {
                    SQLConn.Close();
                    SQLConn.Dispose();
                    SQLConn = null;
                }
                return true;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        private int SetParameter(ref SqlCommand cm, string ParameterName, Object Value
            , SqlDbType ParameterType, int FieldSize = -1
            , ParameterDirection Direction = ParameterDirection.Input
            , Byte Precision = 0, Byte Scale = 0)
        {
            try
            {
                cm.CommandType = CommandType.StoredProcedure;
                if (FieldSize == -1)
                    cm.Parameters.Add(ParameterName, ParameterType);
                else
                    cm.Parameters.Add(ParameterName, ParameterType, FieldSize);

                if (Precision > 0) cm.Parameters[cm.Parameters.Count - 1].Precision = Precision;
                if (Scale > 0) cm.Parameters[cm.Parameters.Count - 1].Scale = Scale;

                cm.Parameters[cm.Parameters.Count - 1].Value = Value;
                cm.Parameters[cm.Parameters.Count - 1].Direction = Direction;

                return 0;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        private int SetParameter(ref SqlDataAdapter cm, string ParameterName, Object Value
            , SqlDbType ParameterType, int FieldSize = -1
            , ParameterDirection Direction = ParameterDirection.Input
            , Byte Precision = 0, Byte Scale = 0)
        {
            try
            {
                cm.SelectCommand.CommandType = CommandType.StoredProcedure;
                if (FieldSize == -1)
                    cm.SelectCommand.Parameters.Add(ParameterName, ParameterType);
                else
                    cm.SelectCommand.Parameters.Add(ParameterName, ParameterType, FieldSize);

                if (Precision > 0) cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Precision = Precision;
                if (Scale > 0) cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Scale = Scale;

                cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Value = Value;
                cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Direction = Direction;

                return 0;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}

