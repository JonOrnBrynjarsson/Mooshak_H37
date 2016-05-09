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

		public List<UserViewModel> getUsersInCourse(int courseID)
		{

			List<UserViewModel> model = new List<UserViewModel>();

			//here we select the connection table that gives us the userID, roleID, courseID, etc.
			//of the users in the given courseID param.
			var userInfo = (from x in _db.UserCourseRelations
						  where courseID == x.CourseID && x.IsRemoved == false
						  select x).ToList();
			//this is to get only the student id for next linq expression (.Contains)
			var userIdList = (from x in _db.UserCourseRelations
							where courseID == x.CourseID && x.IsRemoved == false
							select x.UserID).ToList();
			//here we select all the student names that that are in the course
			//from the ID we collected just above
			var users = (from y in _db.Users
						 where userIdList.Contains(y.ID) 
						 select y).ToList();
			//here we select the AspNetUser table to get the email from all the students in the course
			var aspInfo = (from z in _db.Users
						   where z.AspNetUser.Id == z.AspNetUserId
						   select z.AspNetUser.Email).ToList();



			for(int i = 0; i<userInfo.Count; i++)
			{

				UserViewModel temp = new UserViewModel
				{
					Name = users[i].Name,
					Email = aspInfo[i],
					RoleID = userInfo[i].RoleID
				};

				model.Add(temp);
			}

			return model;
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
                        select x.ID).SingleOrDefault();



            return User;
        }

        internal string getRoleNamebyID(int userID)
        {
            var roleID = (from x in _db.UserCourseRelations
                        where x.UserID == userID
                        select x.RoleID).FirstOrDefault();

            switch(roleID)
            {
                case 1:
                    {
                        return "Student";
                    }
                case 2:
                    {
                        return "Teacher";
                    }
                case 3:
                    {
                        return "Admin";
                    }
            }

            return null;
        }
    }
}
