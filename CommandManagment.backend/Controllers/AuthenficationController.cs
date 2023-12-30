using CommandManagment.backend.Data;
using CommandManagment.backend.Dtos;
using CommandManagment.backend.Helpers;
using CommandManagment.backend.Models;
using CommandManagment.backend.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CommandManagment.backend.Controllers
{
    [Route("api/[controller]")]
    public class AuthenficationController : Controller
    {
        readonly AppDbContext _context;
        readonly JwtService _jwtService;
        readonly IWebHostEnvironment _appEnvironment;
        readonly ContextHelper _contextHelper;

        public AuthenficationController(AppDbContext context, JwtService jwtService, IWebHostEnvironment appEnvironment, ContextHelper contextHelper)
        {
            _context = context;
            _jwtService = jwtService;
            _appEnvironment = appEnvironment;
            _contextHelper = contextHelper;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto dto)
        {

            User existUser = await _contextHelper.GetUserByEmail(dto.Email);

            if (existUser != null)
            {
                return BadRequest(new ResponseModel("Email already taken"));
            }

            string myUniqueFileName = $@"{Guid.NewGuid()}{Path.GetExtension(dto.Photo.FileName)}";
            string path = "/UsersPhoto/";

            ImageService.LoadPhoto(_appEnvironment.WebRootPath + path, myUniqueFileName, dto.Photo);

            User user = new()
            {
                LastName = dto.LastName,
                FirstName = dto.FirstName,
                MiddleName = dto.MiddleName,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Photo = path + myUniqueFileName,
                Specialization = dto.Specialization,

            };

            await _context.Users.AddAsync(user);
            int result = await _context.SaveChangesAsync();
            return Created("success", result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            User user = await _contextHelper.GetUserByEmail(dto.Email);
            if (user == null) return BadRequest(new ResponseModel("Wrong login data"));
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password)) return BadRequest(new ResponseModel("Wrong login data"));

            string jwt = _jwtService.GenerateToken(dto.Email);
            user.Photo = "https://" +  HttpContext.Request.Host + user.Photo;

            return Ok(new { jwt, user });
        }

        [Authorize]
        [HttpGet("GetUser")]
        public async Task<IActionResult>GetUser()
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);
            User user = await _contextHelper.GetUserByEmail(userEmail);

            if(user == null)
            {
                return BadRequest(new ResponseModel("Wrong email"));
            }

            user.Photo = "https://" + HttpContext.Request.Host + user.Photo;

            return Ok(user);
        }

        [Authorize]
        [HttpPut("ProfileUpdate")] 
        public async Task<IActionResult> ProfileUpdate([FromForm] UpdateProfileDto dto)
        {
            User user = await _contextHelper.GetUserById(dto.Id);

            if(user == null) return BadRequest(new ResponseModel("Wrong user id"));
            user.LastName = dto.LastName;
            user.FirstName = dto.FirstName;
            user.MiddleName = dto.MiddleName;
            user.Specialization = dto.Specialization;

            if(dto.Photo != null)
            {
                System.IO.File.Delete(_appEnvironment.WebRootPath + user.Photo);

                string myUniqueFileName = $@"{Guid.NewGuid()}{Path.GetExtension(dto.Photo.FileName)}";
                string path = "/UsersPhoto/";

                ImageService.LoadPhoto(_appEnvironment.WebRootPath + path, myUniqueFileName, dto.Photo);

                user.Photo = path + myUniqueFileName;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            user.Photo = "https://" + HttpContext.Request.Host + user.Photo;

            return Ok(new { user });
        }
    }
}
