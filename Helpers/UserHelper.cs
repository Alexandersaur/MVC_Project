﻿using MVC_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_Project.Helpers
{
    public class UserHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string GetDisplayName(string userId)
        {
            var user = db.Users.Find(userId);
            return user.DisplayName;
        }

        public string GetFullName(string userId)
        {
            var user = db.Users.Find(userId);
            var firstName = user.FirstName;
            var lastName = user.LastName;
            return firstName + " " + lastName;
        }

        public string LastNameFirst(string userId)
        {
            var user = db.Users.Find(userId);
            return $"{user.LastName}, {user.FirstName}";
        }
    }
}