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

        internal void setUser(string name, ApplicationUser user)
        {

            string ID = user.Id;

            var bla = new User
            {
                Name = name,
                AspNetUserId = ID
            };

            _db.Users.Add(bla);
            _db.SaveChanges();

        }
    }
}
