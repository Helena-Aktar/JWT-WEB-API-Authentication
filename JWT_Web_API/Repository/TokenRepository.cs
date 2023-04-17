//using JWT_Web_API.Models;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace JWT_Web_API.Repository
//{
//    public class TokenRepository : ITokenRepository
//    {

//            Dictionary<string, string> UsersRecords = new Dictionary<string, string>
//        {
//            { "admin","admin"},
//            { "password","password"}
//        };


//        private readonly IConfiguration _configuration;
//        public TokenRepository(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public Tokens Authenticate(Users users)
//        {
//            //throw new NotImplementedException();
//            if (!UsersRecords.Any(x => x.Key == users.Name && x.Value == users.Password))
//            {
//                return null;
//            }
//            // We have Authenticated

//            //Generate JSON Web Token
//            var tokenHandler = new JwtSecurityTokenHandler();
//            var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
//            var tokenDescriptor = new SecurityTokenDescriptor
//            {
//                Subject = new ClaimsIdentity(new Claim[]
//              {
//             new Claim(ClaimTypes.Name, users.Name)
//              }),
//                Expires = DateTime.UtcNow.AddMinutes(10),
//                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
//            };
//            var token = tokenHandler.CreateToken(tokenDescriptor);
//            return new Tokens { Token = tokenHandler.WriteToken(token) };
//        }
//    }
//}
using JWT_Web_API.Models;
//using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.Data;
//using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
//using System.Linq;
//using System.Collections.Generic;

namespace JWT_Web_API.Repository
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration _configuration;

        public TokenRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Users> Users()
        {
            List<Users> UserList = new List<Users>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand("select * from user_table", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Users obj = new Users();
                    //obj.Id = int.Parse(dt.Rows[i]["id"].ToString());
                    //obj.Name = dt.Rows[i]["name"].ToString();
                    obj.UserName = dt.Rows[i]["user_name"].ToString();
                    obj.Password = dt.Rows[i]["password"].ToString();
                    UserList.Add(obj);
                }
            }
            return UserList;
        }

        public Tokens Authenticate(Users users)
        {
            List<Users> UsersRecords = Users();

            if (!UsersRecords.Any(x => x.UserName == users.UserName && x.Password == users.Password))
            {
                return null;
            }

            // We have Authenticated
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, users.UserName)
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new Tokens { Token = tokenHandler.WriteToken(token) };
        }
    }
}

