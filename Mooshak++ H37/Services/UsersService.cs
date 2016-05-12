using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Mooshak___H37.Models;
using Mooshak___H37.Models.Entities;
using Mooshak___H37.Models.Viewmodels;
using System;
using SecurityWebAppTest.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;

namespace Mooshak___H37.Services
{
	public class UsersService
	{
		//private readonly ApplicationDbContext _db;

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
		/// This function takes in a userViewModel with the requerment that it has
		/// a CourseID and ID wich is a userID, then it puts in the model, the correct role.
		/// </summary>
		private void getRolesByCourseID(UserViewModel model)
		{
			var role = (from x in _db.UserCourseRelations
						where model.CourseID == x.CourseID &&
						model.ID == x.UserID &&
						x.IsRemoved == false
						select x).FirstOrDefault();

			model.RoleID = role.RoleID;

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
							select user.ID).FirstOrDefault();
			return userId;
		}

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

        internal int getRoleNamebyId(int userId)
        {
            var roleID = (from x in _db.UserCourseRelations
                        where x.UserID == userId
						&& x.IsRemoved == false
                        select x.RoleID).FirstOrDefault();

            return roleID;
        }

		public string getUserNameById(int userId)
		{
			var name = (from n in _db.Users
						where n.ID == userId
						&& n.IsRemoved == false
						select n.Name).SingleOrDefault();
			return name;
		}

		internal void EditUser(UserViewModel model)
		{
			var edit = (from user in _db.Users
						where model.ID == user.ID
						&& user.IsRemoved == false
						select user).FirstOrDefault();
			
			if (edit != null)
			{
				edit.Name = model.Name;
			}

			_db.SaveChanges();

			//var usr = (from us in _db.Users
			//			 where edit.AspNetUserId == us.AspNetUser.Id
			//			 select us.AspNetUser).SingleOrDefault();

			//if (usr == null)
			//{
			//	//TODO
			//	//throw error
			//	return;
			//}



			//var store = new UserStore<ApplicationUser>(new IdentityDbContext());
			//var manager = new UserManager<ApplicationUser>(store);

			//ApplicationUser a = new ApplicationUser
			//{
			//	AccessFailedCount = usr.AccessFailedCount,
			//	Email = usr.Email,
			//	EmailConfirmed = usr.EmailConfirmed,
			//	Id = usr.Id,
			//	LockoutEnabled = usr.LockoutEnabled,
			//	LockoutEndDateUtc = usr.LockoutEndDateUtc,
			//	PasswordHash = usr.PasswordHash,
			//	PhoneNumber = usr.PhoneNumber,
			//	PhoneNumberConfirmed = usr.PhoneNumberConfirmed,
			//	SecurityStamp = usr.SecurityStamp,
			//	TwoFactorEnabled = usr.TwoFactorEnabled,
			//	UserName = usr.UserName
			//};


			//manager.UpdateAsync(usr);

			//store.Context.SaveChanges();
			
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

					//	var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
					//	ApplicationUser u = um.FindById(aspUser);
					//	Membership.DeleteUser(u.UserName, true);
				}
			}
		}

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
