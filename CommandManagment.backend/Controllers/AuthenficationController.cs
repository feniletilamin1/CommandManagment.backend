using CommandManagment.backend.Data;
using CommandManagment.backend.Dtos;
using CommandManagment.backend.Helpers;
using CommandManagment.backend.Models;
using CommandManagment.backend.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;

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
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);
            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null) return BadRequest(new ResponseModel("Wrong user"));
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

        [Authorize]
        [HttpPut("PasswordChange")]
        public async Task<IActionResult> PasswordChange([FromBody] PasswordChangeDto dto)
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);
            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null) return BadRequest(new ResponseModel("Wrong user"));

            if (!BCrypt.Net.BCrypt.Verify(dto.oldPassword, user.Password)) return BadRequest(new ResponseModel("Неверный текущий пароль"));

            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.newPassword);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("PasswordReset/{userEmail}")]
        public async Task<IActionResult> PasswordReset(string userEmail)
        {
            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (!_context.Users.Any(u => u.Email == userEmail)) return BadRequest(new ResponseModel("Пользователя с указанным email не существует"));

            string resetToken = _jwtService.GenerateResetPasswordToken(userEmail);

            string resetLink = AuthOptions.AUDIENCE + "/passwordResetConf/" + resetToken;

            string smtpServer = "smtp.yandex.ru";
            int smtpPort = 587;

            // Логин и пароль для авторизации на SMTP-сервере
            string login = "tsibutsinini.serezha@yandex.ru";
            string password = "fpmjvoushkiicqqg";

            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress("tsibutsinini.serezha@yandex.ru", "Команда WorkFlow");

            MailAddress to = new("tsibutsinini.serezha@yandex.ru");

            MailMessage message = new(from, to)
            {
                // тема письма
                Subject = "Сброс пароля",
                // текст письма
                Body = @$"<h2>Сброс пароля</h2>
                <p>Здравствуйте, {userEmail}!</p>
                <p>Мы получили запрос на сброс вашего пароля. Ваша ссылка для сброса пароля:</p>
                <p><strong>{resetLink}</strong></p>
                <p>С наилучшими пожеланиями,</p>
                <p>Команда поддержки WorkFlow</p>", 
                IsBodyHtml = true
            };

            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient(smtpServer, smtpPort);
            // логин и пароль
            smtp.Credentials = new NetworkCredential(login, password);
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            // отправляем асинхронно
            await smtp.SendMailAsync(message);
            message.Dispose();

            return Ok();
        }

        [HttpGet("PasswordChangeConfirm/{token}")]
        public async Task<IActionResult> PasswordChangeConfirm(string token)
        {
            string newPassword = _jwtService.GenerateRandomPassword(10);

            JwtSecurityTokenHandler tokenHandler = new();
            JwtSecurityToken validateToken = _jwtService.ValidateToken(token);

            if (validateToken == null) return BadRequest(new ResponseModel("Wrong token or expires"));

            ResetPasswordToken resetPasswordToken = await _context.ResetPasswordTokens.FirstOrDefaultAsync(t => t.Token == token);

            if (resetPasswordToken == null) return BadRequest(new ResponseModel("Wrong token or expires"));

            string userEmail = validateToken.Claims.First(claim => claim.Type == "userEmail").Value;

            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong user email"));

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _context.Users.Update(user);
            _context.Remove(resetPasswordToken);
            await _context.SaveChangesAsync();

            string smtpServer = "smtp.yandex.ru";
            int smtpPort = 587;

            // Логин и пароль для авторизации на SMTP-сервере
            string login = "tsibutsinini.serezha@yandex.ru";
            string password = "fpmjvoushkiicqqg";

            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress("tsibutsinini.serezha@yandex.ru", "Команда WorkFlow");

            MailAddress to = new("tsibutsinini.serezha@yandex.ru");

            MailMessage message = new(from, to)
            {
                // тема письма
                Subject = "Сброс пароля",
                // текст письма
                Body = @$"<h2>Сброс пароля</h2>
                <p>Здравствуйте, {userEmail}!</p>
                <p>Мы получили запрос на сброс вашего пароля. Ваш новый временный пароль:</p>
                <p><strong>{newPassword}</strong></p>
                <p>Пожалуйста, войдите в свой личный кабинет с использованием нового пароля и сразу же измените его на более безопасный.</p>
                <p>С наилучшими пожеланиями,</p>
                <p>Команда поддержки WorkFlow</p>",
                IsBodyHtml = true
            };

            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient(smtpServer, smtpPort);
            // логин и пароль
            smtp.Credentials = new NetworkCredential(login, password);
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            // отправляем асинхронно
            await smtp.SendMailAsync(message);
            message.Dispose();


            return Ok();
        }

    }
}
