using System.ComponentModel.DataAnnotations;

namespace ApiBase.Service.ViewModels
{
    public class FacebookViewModel
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [EmailAddress]
        public string facebookEmail { get; set; }

        [Required]
        public string facebookId { get; set; }

        [Required]
        public string avatar { get; set; }
        public string accessToken { get; set; }
    }

    public class FacebookResult
    {
        //id,name,email,first_name,last_name,age_range,birthday,gender,locale,picture
            public string id { get; set; }
            public string name { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string age_range { get; set; }
            public string birthday { get; set; }
            public string gender { get; set; }
            public string locale { get; set; }

        

    }


    public class DangNhapFacebookViewModel
    {


        [Required]
        public string facebookToken { get; set; }

       
    }
}