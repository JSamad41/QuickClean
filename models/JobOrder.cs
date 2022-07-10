using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickClean.Models
{
    public class JobOrder
    {
		public long ID = 0;

		//added cleaning options
		public bool carpetCleaning = false;
		public bool baseboardCleaning = false;
		public bool laundryCleaning = false;
		public bool dishCleaning = false;
		public string Details = string.Empty;
		public string Compensation;
		public bool standardCleaning = true;
		public bool deepCleaning = false;
	}
}
