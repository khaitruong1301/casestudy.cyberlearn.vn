using ApiBase.Repository.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Service.ViewModels.Users
{
    public class UserViewModel
    {
        public string email { get; set; }

        public string name { get; set; }
        public string password { get; set; }

        public bool gender { get; set; }
        public string phone { get; set; }
        public string facebookId { get; set; }
        public string userTypeId { get; set; }
        public bool deleted { get; set; }
        public string avatar { get; set; }
        public string favoriteProducts { get; set; }
    }
    public class UserFavorite
    {
        public string email { get; set; } 
        public List<ProductFavorite> ProductsFavorite = new List<ProductFavorite>();
    }

    public class ProductFavorite
    {
        public int id { get; set; }
        public string name { get; set; }
        
        public string image { get; set; }
    }

    public class InfoUser
    {
        public string email { get; set; }
        public string password { get; set; }

        public string name { get; set; }
        public bool gender { get; set; }
        public string phone { get; set; }
    }
    public class UserLogin
    {
        public string email { get; set; }
        public string password { get; set; }

    }
    public class UserLoginResult { 
        public string email { get; set; }
        public string accessToken { get; set; }

    }

    public class UserNewPassword
    {
        //public string email { get; set; }
        public string newPassword { get; set; }

    }
    public class UserUploadAvatar
    {
        public string email { get; set; }
        public string avatar { get; set; }
      
    }

    public class userProfile
    {
        public string email { get; set; }

        public string name { get; set; }
        public string password { get; set; }

        public bool gender { get; set; }
        public string phone { get; set; }
        public string facebookId { get; set; }
        public bool deleted { get; set; }
        public string avatar { get; set; }
        public List<OrderHistory> ordersHistory = new List<OrderHistory>();




    }

    public class OrderHistory
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; } 
        public string email { get; set; }
        public string alias { get; set; }
        public List<OrderDetailHistory> orderDetail = new List<OrderDetailHistory>();
    }
    public class OrderDetailHistory
    {
        public string name { get; set; }
        public string alias { get; set; }
        public string shortDescription { get; set; }


        public string image { get; set; }
        public string description { get; set; }
    
    }
}
