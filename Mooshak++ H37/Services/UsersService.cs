using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Mooshak___H37.Models;
using Mooshak___H37.Models.Entities;
using Mooshak___H37.Models.Viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mooshak___H37.Services
{
	class UsersService
	{
		private ApplicationDbContext _db;

		public UsersService()
		{
			_db = new ApplicationDbContext();
		}

        public List<UserViewModel> getAllUsersName()
        {

            var Users = (from x in _db.Users
						 orderby x.Name ascending
                           select x).ToList();

            var viewModel = new List<UserViewModel>();
            
            foreach (var user in Users)
            {
                UserViewModel model = new UserViewModel
                {
                    Name = user.Name
                    
                };
                viewModel.Add(model);
            }


            return viewModel;
        }
		/// <summary>
		/// In order to 
		/// </summary>
		/// <returns>Returns the User.ID for the current ApplicationUser</returns>
		public int getUserIdForCurrentyApplicationUser()
		{
			var aspUser = System.Web.HttpContext.Current.User.Identity.GetUserId();
			var userId = (from user in _db.Users
							where user.AspNetUserId == aspUser
							select user.ID).FirstOrDefault();
			return userId;
		}

        internal void setUser(string name, ApplicationUser user)
        {

            string ID = user.Id;

            var User = new User
            {
                Name = name,
                AspNetUserId = ID
            };

            _db.Users.Add(User);
            _db.SaveChanges();

        }

        internal int getUserIDbyEmail(LoginViewModel model)
        {

            var User = (from x in _db.Users
                        where x.AspNetUser.Email == model.Email
                        select x.ID).FirstOrDefault();



            return User;
        }

        internal string getRolebyID(int userID)
        {
            var roleID = (from x in _db.UserCourseRelations
                          where x.UserID == userID
                          select x.RoleID).ToString();

            var Role = (from x in _db.Roles
                        where x.Id == roleID
                        select x.Name).ToString();

                return Role;
        }
    }
}
