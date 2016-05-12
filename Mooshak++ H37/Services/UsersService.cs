using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Mooshak___H37.Models;
using Mooshak___H37.Models.Entities;
using Mooshak___H37.Models.Viewmodels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;

namespace Mooshak___H37.Services
{
	class UsersService
	{
		private readonly ApplicationDbContext _db;

		public UsersService()
		{
			_db = new ApplicationDbContext();
		}

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
				where c.ID == courseId
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
						model.ID == x.UserID
						select x).FirstOrDefault();

			model.RoleID = role.RoleID;

		}

        internal dynamic getAllUsersNameNotInCourse(int courseID)
        {
            //gets all the users in the system
            var usersInCourse = getUsersInCourse(courseID);

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
							where user.AspNetUserId == aspUser
							select user.ID).FirstOrDefault();
			return userId;
		}

		public string getAspUserRole(int userId)
		{
			var aspUser = (from user in _db.Users
				where user.ID == userId
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

		//public string GetNameFromUserID(int userID)
		//{
		//	var username = (from name in _db.Users
		//					where name.ID == userID
		//					select name.Name).FirstOrDefault();
		//	return username;
		//}

        internal int getUserIDbyEmail(LoginViewModel model)
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

        public UserViewModel GetSingleUser(int userID)
		{
			var user = (from us in _db.Users
						where us.ID == userID
						select us).FirstOrDefault();

			var email = (from us in _db.Users
						 where user.AspNetUserId == us.AspNetUser.Id
						 select us.AspNetUser.Email).SingleOrDefault();

			if (user == null)
			{
				//do something
			}

			UserViewModel model = new UserViewModel
			{
				ID = user.ID,
				Name = user.Name,
				Email = email,
			};

			return model;
		}

        internal int getRoleNamebyID(int userID)
        {
            var roleID = (from x in _db.UserCourseRelations
                        where x.UserID == userID
                        select x.RoleID).FirstOrDefault();

            return roleID;
        }

		public string getUserNameByID(int userId)
		{
			var name = (from n in _db.Users
						where n.ID == userId
						select n.Name).SingleOrDefault();
			return name;
		}

		internal void EditUser(UserViewModel model)
		{
			var edit = (from user in _db.Users
						where model.ID == user.ID
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

        public List<int> UsersInSystem()
        {
            List<int> UserList = new List<int>();

            var users = (from user in _db.Users
                         where user.IsRemoved != true
                         select user).Count();

            UserList.Add(users);

            var students = (from user in _db.UserCourseRelations
                         where user.RoleID == 1
                         select user).Count();

            UserList.Add(students);

            var teachers = (from user in _db.UserCourseRelations
                            where user.RoleID == 2
                            select user).Count();

            UserList.Add(teachers);

            //var admins = (from user in _db.UserCourseRelations
            //                where user.RoleID == 3
            //                select user).Count();

            //UserList.Add(admins);

            return UserList;
        }

		public void removeUserByID(int? id)
		{
			if(id != null)
			{
				//finds the user in the Users table from the id param.
				var usr = (from user in _db.Users
						   where user.ID == id.Value
						   select user).SingleOrDefault();

				if (usr != null)
				{
					string aspUser = usr.AspNetUserId;
					//sets IsRemoved in the Users table to true for the right user
					usr.IsRemoved = true;
					_db.SaveChanges();

					//deletes the account created in AspNetUsers Table	============	\\
					//var usrName = (from user in _db.Users                               //
					//			   where usr.AspNetUserId == user.AspNetUser.Id         //
					//			   select user.AspNetUser.UserName).FirstOrDefault();   //
					//if (usrName != null)                                                //
					//{                                                                   //
					//	Membership.DeleteUser(usrName);                                 //
					//}//         ========================================				//

					//retrieves all the connections to courses the user to be deleted has and removes them all
					var connections = (from x in _db.UserCourseRelations									//
									   where x.UserID == usr.ID												//
									   select x).ToList();													//
					foreach(var item in connections)														//
					{																						//
						item.IsRemoved = true;		
					
					}//			=========================================									//
					_db.SaveChanges();

					//var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
					//ApplicationUser u = um.FindById(aspUser);	
					//Membership.DeleteUser(u.UserName);				
				}
			}
		}

		public bool deleteUserById(int Id)
		{
				


			return false;
		}
	}
}
