
namespace web2.Models {
	public class PropertyContent
	{
		public Property Property;
		public User User;

		public bool CurrentUserIsOwner {
			get {
				if (Property == null) return false;
				if (Property.User == null) return false;
				if (User == null) return false;
				if (User.UID != Property.User.UID) return false;
				return true;
			}
		}
	}
}