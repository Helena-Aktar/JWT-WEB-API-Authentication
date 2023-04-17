using JWT_Web_API.Models;
using JWT_Web_API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace JWT_Web_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]

    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ITokenRepository _tokenRepository;

        //public UsersController(ITokenRepository tokenRepository)
        //{
        //    _tokenRepository = tokenRepository;
        //}
        private readonly IConfiguration _configuration;
        //public UsersController(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}
        public UsersController(ITokenRepository tokenRepository, IConfiguration configuration)
        {
            _tokenRepository = tokenRepository;
            _configuration = configuration;
        }
        // GET: api/<UsersController>
        //[HttpGet]
        //public List<string> Get()
        //{
        //    var users = new List<string>
        //{
        //    "John Doe",
        //    "Jane Doe",
        //    "Junior Doe"
        //};

        //    return users;
        //}
        [HttpGet]
        [Route("GetAllUsers")]
        public List<Users> AllUsers()
        {
            List<Users> UserList = new List<Users>();

            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            SqlCommand command = new SqlCommand("select * from user_table", connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Users obj = new Users();
                obj.Id = int.Parse(dt.Rows[i]["id"].ToString());
               obj.Name = dt.Rows[i]["name"].ToString();
                obj.UserName = dt.Rows[i]["user_name"].ToString();
                obj.Password = dt.Rows[i]["password"].ToString();
                UserList.Add(obj);
            }

            return UserList;
        }
        [HttpPost]
        [Route("AddUsers")]
        public string AddUsers(Users user)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            SqlCommand cmd = new SqlCommand("insert into user_table values (@id,@name,@pass,@userName)", con);
            cmd.CommandType = CommandType.Text;
            SqlParameter sqlParameter = cmd.Parameters.AddWithValue("@id", user.Id);
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@userName", user.UserName);
            cmd.Parameters.AddWithValue("@pass", user.Password);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return "User Added Successfully";
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("authenticate")]
        public IActionResult Authenticate(Users usersdata)
        {
            var token = _tokenRepository.Authenticate(usersdata);

            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(token);

        }
    }
}
