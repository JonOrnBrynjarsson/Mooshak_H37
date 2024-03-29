﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Mooshak___H37.Models;
using Mooshak___H37.Models.Entities;
using Mooshak___H37.Models.Viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mooshak___H37.Services
{
	public class UsersService
	{

		private readonly IAppDataContext _db;

		public UsersService(IAppDataContext dbContext)
		{
			_db = dbContext ?? new ApplicationDbContext();
		}

		/// <summary>
		/// Get a list of all users currenty in the system
		/// </summary>
		/// <returns>UserViewModel with name, id, and mail of all users</returns>
        public List<UserViewModel> getAllUsersName()
        {
			//gets all the users in the system
            var Users = (from x in _db.Users
						 where x.IsRemoved == false
						 orderby x.Name ascending
                         select x).ToList();

			//retrevies the mail of all Users found above and
			//inserts it into the list of strings
			List<string> mail = new List<string>();
			foreach (var item in Users)
			{
				var mailinfo = (from y in _db.Users
								where item.AspNetUserId == y.AspNetUser.Id
								&& y.IsRemoved == false
								select y.AspNetUser.Email).SingleOrDefault();

				mail.Add(mailinfo);
			}


            var viewModel = new List<UserViewModel>();
			//combines all information gathered into the userViewmodel
            for(int i = 0; i< Users.Count(); i++)
            {
                UserViewModel model = new UserViewModel
                {
                    Name = Users[i].Name,
					Email = mail[i],
					ID = Users[i].ID
                };
                viewModel.Add(model);
            }


            return viewModel;
        }

		/// <summary>
		/// Gets a list of student that are in a specific course.
		/// </summary>
		/// <param name="courseId">The ID of the course</param>
		/// <returns>A list of users in a course</returns>
		public List<UserViewModel> getUsersInCourse(int courseId)
		{
			var usersInfo = (from ucr in _db.UserCourseRelations
				join u in _db.Users on ucr.UserID equals u.ID
				join c in _db.Courses on ucr.CourseID equals c.ID
				where  u.IsRemoved == false && c.IsRemoved == false &&
					   c.ID == courseId
					   select  new {
							ID = u.ID,
							CourseID = courseId,
							RoleID = ucr.RoleID,
							Email = u.AspNetUser.Email,
							Name = u.Name
					}).ToList().OrderBy(x => x.Name);
			List<UserViewModel> userList = new List<UserViewModel>();
			foreach (var user in usersInfo)
			{
				UserViewModel model = new UserViewModel();
				model.ID = user.ID;
				model.CourseID = user.CourseID;
				model.Email = user.Email;
				model.RoleID = user.RoleID;
				model.Name = user.Name;
				userList.Add(model);
			}
			return userList;
		}

		/// <summary>
		/// Gets all the users in system and all users in the course and filters
		/// the users that are not in the specific course so they wont be included in the complete list
		/// </summary>
		/// <param name="courseId">the course id that we use to filter students out of the whole list</param>
        internal dynamic getAllUsersNameNotInCourse(int courseId)
        {

            //gets all the users in the system
            var usersInCourse = getUsersInCourse(courseId);

            var allUsers = getAllUsersName();

            int flag = 0;

            var viewModel = new List<UserViewModel>();
            //combines all information gathered into the userViewmodel
            for(int i = 0; i< allUsers.Count; i++)
            {
                for(int x = 0; x < usersInCourse.Count; x++)
                {
                    if(allUsers[i].ID == usersInCourse[x].ID)
                    {
                        flag = 1;
                    }
                }

                if (flag == 0)
                {
                    UserViewModel model = new UserViewModel
                    {
                        Name = allUsers[i].Name,
                        ID = allUsers[i].ID
                    };
                    viewModel.Add(model);
                }
                else
                {
                    flag = 0;
                }
            }

            return viewModel;
        }

        /// <summary>
        /// In order to 
        /// </summary>
        /// <returns>Returns the User.ID for the current ApplicationUser</returns>
        public int getUserIdForCurrentApplicationUser()
		{
			var aspUser = System.Web.HttpContext.Current.User.Identity.GetUserId();
			var userId = (from user in _db.Users
							where user.AspNetUserId == aspUser &&
							user.IsRemoved == false
							select user.ID).SingleOrDefault();
			if (userId == 0)
			{
				throw new Exception("User does not exist or has been removed.");
			}
			return userId;
		}

		/// <summary>
		/// Get the role of a user with the specific
		/// ID from the asp table
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public string getAspUserRole(int userId)
		{
			var aspUser = (from user in _db.Users
				where user.ID == userId
				&& user.IsRemoved == false
				select user.AspNetUserId).SingleOrDefault();

			var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var result = um.GetRoles(aspUser).FirstOrDefault();
			return result;
			
		}

		/// <summary>
		/// Adds the specific user to the database
		/// and connects him the asp user
		/// </summary>
		/// <param name="name"></param>
		/// <param name="user"></param>
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

            var role = getAspUserRole(User.ID);


        }

		/// <summary>
		/// Here we send in a LoginViewModel with the email attribute filled and we find the id of the current user
		/// </summary>
		/// <returns>the Int id of a user</returns>
		internal int getUserIdByEmail(LoginViewModel model)
        {

            var User = (from x in _db.Users
                        where x.AspNetUser.Email == model.Email
                        select x.ID).SingleOrDefault();

            return User;
        }

		/// <summary>
		/// Adds the specifc user to the selected role
		/// </summary>
		/// <param name="model"></param>
		/// <param name="role"></param>
        internal void setRole(ApplicationUser model, string role)
        {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            um.AddToRole(model.Id, role);
        }

		/// <summary>
		/// Gets the user info for a particular user based on his ID.
		/// </summary>
		/// <param name="userId">The ID of the user</param>
		/// <returns>A User object with the user info</returns>
		public User getUserById(int userId)
		{
			return (from us in _db.Users
						 where us.ID == userId
						 && us.IsRemoved == false
						 select us).FirstOrDefault();
		}

		/// <summary>
		/// Gets the user email from AspNetUser.
		/// </summary>
		/// <param name="aspNetUserId">The AspNetUserId</param>
		/// <returns>The email as string</returns>
		public string getUserEmailByAspNetUserId(string aspNetUserId)
		{
			return (from us in _db.Users
						 where  us.AspNetUser.Id == aspNetUserId
						 select us.AspNetUser.Email).SingleOrDefault();
		}

		/// <summary>
		/// Gets user info and email.
		/// </summary>
		/// <param name="userId">The ID of the user</param>
		/// <returns>UserViewModel object with the info</returns>
        public UserViewModel getSingleUserInfo(int userId)
        {
	        User user = getUserById(userId);
			if (user != null)
			{
				string email = getUserEmailByAspNetUserId(user.AspNetUserId);
				UserViewModel model = new UserViewModel
				{
					ID = user.ID,
					Name = user.Name,
					Email = email,
				};
				return model;
			}
			else
			{
				throw new Exception("User Does Not Exists");
			}

        }

		/// <summary>
		/// Gets the username of the user with the specific ID
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public string getUserNameById(int userId)
		{
			var name = (from n in _db.Users
						where n.ID == userId
						&& n.IsRemoved == false
						select n.Name).SingleOrDefault();
			return name;
		}

		/// <summary>
		/// Gets the user with the specific ID and updates
		/// him with new informations
		/// </summary>
		/// <param name="model"></param>
		internal void editUser(UserViewModel model)
		{
			var edit = (from user in _db.Users
						where model.ID == user.ID
						&& user.IsRemoved == false
						select user).FirstOrDefault();
			
			if (edit != null)
			{
				edit.Name = model.Name;
			
				_db.SaveChanges();
			
				editUserEmail(edit.AspNetUserId, model.Email);
				editUserPassword(edit.AspNetUserId, model.Password);
				string oldRole = getAspUserRole(edit.ID);
				editUserRole(edit.AspNetUserId, oldRole, model.RoleName);
			}
		}

		/// <summary>
		/// Updates the email on a user with a specific ID
		/// </summary>
		/// <param name="aspNetId"></param>
		/// <param name="email"></param>
		private void editUserEmail(string aspNetId, string email)
		{
			var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
			um.SetEmail(aspNetId, email);
		}

		/// <summary>
		/// Updates the password on a user with a specific ID
		/// </summary>
		/// <param name="aspNetId"></param>
		/// <param name="email"></param>
		private void editUserPassword(string aspNetId, string password)
		{
			var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
			um.RemovePassword(aspNetId);
			um.AddPassword(aspNetId, password);
		}

		/// <summary>
		/// Updates the role on a user with a specific ID
		/// </summary>
		/// <param name="aspNetId"></param>
		/// <param name="email"></param>
		private void editUserRole(string aspNetId,string oldRole, string role)
		{
			var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
			um.RemoveFromRole(aspNetId, oldRole);
			um.AddToRole(aspNetId, role);
		}

		/// <summary>
		/// Gets the number of users with a particular role in the system
		/// </summary>
		/// <param name="roleId">The ID of the role</param>
		/// <returns>The number of users with this roleId</returns>
		public int usersInSystemByRole(int roleId)
		{
			return (from user in _db.UserCourseRelations
							where user.RoleID == roleId
							&& user.IsRemoved == false
							select user).Count();
		}

		/// <summary>
		/// Gets a list of the number of users in the system, 
		/// as well as the number of students and teachers
		/// </summary>
		/// <returns>A list of integers for users, students and teachers</returns>
        public List<int> UsersInSystem()
        {
            List<int> UserList = new List<int>();
			
	        int students = usersInSystemByRole(1);
			int teachers = usersInSystemByRole(2);
			int admins = usersInSystemByRole(3);
	        int users = students + teachers + admins;

			UserList.Add(users);
			UserList.Add(students);
			UserList.Add(teachers);
			
            return UserList;
        }

		/// <summary>
		/// Sets isRemoved on a user to true so it counts as removed and finds the connections
		/// and does the same thing to them from the userId
		/// </summary>
		/// <param name="userId"></param>
		public void removeUserById(int? userId)
		{
			if(userId != null)
			{
				var usr = getUserById(userId.Value);

				if (usr != null)
				{
					string aspUser = usr.AspNetUserId;

					usr.IsRemoved = true;
					_db.SaveChanges();

					removeUserConnections(userId.Value);

					var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
					um.RemovePassword(usr.AspNetUserId);
				}
			}
		}

		/// <summary>
		/// Removes the connection of a user and courses
		/// </summary>
		/// <param name="userId"></param>
		private void removeUserConnections(int userId)
		{

			var connections = (from x in _db.UserCourseRelations                                    
							   where x.UserID == userId                                             
							   select x).ToList(); 
			
			if(connections != null)
			{
				foreach (var item in connections)
				{
					item.IsRemoved = true;
				}
				_db.SaveChanges();
			}                                       
			
		}
	}
}
