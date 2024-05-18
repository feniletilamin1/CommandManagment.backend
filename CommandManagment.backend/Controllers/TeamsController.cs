using CommandManagment.backend.Data;
using CommandManagment.backend.Dtos;
using CommandManagment.backend.Helpers;
using CommandManagment.backend.Models;
using CommandManagment.backend.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace CommandManagment.backend.Controllers
{

    [Route("api/[controller]")]
    public class TeamsController : Controller
    {
        readonly AppDbContext _context;
        readonly ContextHelper _contextHelper;
        readonly JwtService _jwtService;

        public TeamsController(AppDbContext context, ContextHelper contextHelper, JwtService jwt)
        {
            _context = context;
            _contextHelper = contextHelper;
            _jwtService = jwt;
        }

        [Authorize]
        [HttpGet("GetTeams")]
        public async Task<IActionResult> GetTeams()
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);

            User userExist = await _context.Users.FirstOrDefaultAsync(p => p.Email == userEmail);

            if (userExist == null)
                return BadRequest(new ResponseModel("Wrong Email"));

            List<Team> teams = await _context.Teams.Include(p => p.Users).Where((p => p.Users.Contains(userExist))).ToListAsync();

            List<TeamCardDto> cards = new();

            foreach (Team team in teams)
            {
                team.Users = team.Users.Take(3).ToList(); ;
                foreach (User user in team.Users)
                {
                    if(!user.Photo.Contains("https://"))
                    {
                        user.Photo = "https://" + HttpContext.Request.Host + user.Photo;
                    }
                }
                cards.Add(new TeamCardDto {Id = team.Id, TeamName = team.TeamName, users = team.Users});
            }


            return Ok(cards);
        }

        [Authorize]
        [HttpPost("AddTeam")]
        public async Task<IActionResult> AddTeam([FromBody] TeamDto teamDto)
        { 
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);
            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong Email"));

            Team newTeam = new()
            {
                TeamName = teamDto.TeamName,
                TeamDescription = teamDto.TeamDescription,
                CreateUserId = teamDto.CreateUserId,
                Users = new(new[] { user }),
            };

            await _context.AddAsync(newTeam);
            await _context.SaveChangesAsync();

            newTeam.Users[0].Photo = "https://" + HttpContext.Request.Host + user.Photo;

            return Ok(newTeam);
        }

        [Authorize]
        [HttpGet("GetCurrentTeam/{teamId}")]
        public async Task<IActionResult> GetCurrentTeam(int teamId)
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);

            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong user id"));

            Team team = await _context.Teams.Include(o => o.Users).Where(p => p.Id == teamId).FirstOrDefaultAsync();

            if(team == null) 
                return BadRequest(new ResponseModel("Wrong team id"));

            if (!team.Users.Contains(user))
                return Unauthorized(new ResponseModel("Access deniend"));

            foreach (User userItem in team.Users)
            {
                userItem.Photo = "https://" +  HttpContext.Request.Host + userItem.Photo;
            }

            return Ok(team);
        }

        [Authorize]
        [HttpPost("AddUserToTeam")]
        public async Task<IActionResult> AddUserToTeam([FromBody] AddUserTeamDto addUserTeamDto)
        {
            User user = await _context.Users.Where(p => p.Email == addUserTeamDto.Email).FirstOrDefaultAsync();
            Team team = await _context.Teams.Where(p =>  p.Id == addUserTeamDto.TeamId).FirstOrDefaultAsync();
           
            if (team == null || user == null)
                return BadRequest(new ResponseModel("Wrong email or team id"));

            team.Users.Add(user);

            _context.Update(team);

            await _context.SaveChangesAsync();

            return Ok(new ResponseModel("Success"));
        }

        [Authorize]
        [HttpDelete("DeleteTeam/{teamId}")]
        public async Task<IActionResult> DeleteUserFromTeam(int teamId)
        {
            Team team = await _contextHelper.GetTeamById(teamId);
                                                                                                                                                            
            if (team == null)
                return BadRequest(new ResponseModel("Wrong team id"));

            _context.Teams.Remove(team);

            await _context.SaveChangesAsync();

            return Ok(new ResponseModel("Success"));
        }


        [Authorize]
        [HttpPut("UpdateTeam")]
        public async Task<IActionResult> UpdateTeam([FromBody] TeamDto teamDto)
        {
            Team team = await _contextHelper.GetTeamById(teamDto.Id != null ? (int)teamDto.Id : 0);
            
            if (team == null)
                return BadRequest(new ResponseModel("Wrong team id"));

            if (team.CreateUserId != teamDto.CreateUserId)
                return Unauthorized(new ResponseModel("Access deniend"));

            team.TeamName = teamDto.TeamName;
            team.TeamDescription = teamDto.TeamDescription;

            _context.Teams.Update(team);

            await _context.SaveChangesAsync();

            return Ok(new ResponseModel("Success"));
        }

        [Authorize]
        [HttpGet("GetInviteLink/{teamId}")]
        public async Task<IActionResult> GetInviteLink(int teamId)
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);
            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong user id"));

            Team team = await _contextHelper.GetTeamById(teamId);

            if (team == null)
                return BadRequest(new ResponseModel("Wrong team id"));

            if (user.Id != team.CreateUserId)
                return Unauthorized(new ResponseModel("Access Denied"));

            string token = _jwtService.GenerateInviteToken(teamId);

            _context.InviteTokens.Add(new InviteToken
            {
                Token = token
            });

            await _context.SaveChangesAsync();

            return Ok(token);
        }

        [Authorize]
        [HttpGet("InviteUserToTeam/{token}")]
        public async Task<IActionResult> InviteUserToTeam(string token)
        {
            InviteToken inviteToken = await _context.InviteTokens.Where(t => t.Token == token).FirstOrDefaultAsync();

            if(inviteToken == null) return BadRequest("Token doesn`t exists");

            JwtSecurityTokenHandler tokenHandler = new();
            JwtSecurityToken validateToken = _jwtService.ValidateToken(token);

            if (validateToken == null ) return BadRequest("Wrong token or expires");

            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);
            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong user email"));

            int teamId = int.Parse(validateToken.Claims.First(claim => claim.Type == "teamId").Value);

            Team team = await _context.Teams.Where(p => p.Id == teamId).Include(p => p.Users).FirstOrDefaultAsync();

            if (team == null)
                return BadRequest(new ResponseModel("Wrong team id"));

            if (team.Users.Contains(user))
                return BadRequest(new ResponseModel("User already in team"));

            team.Users.Add(user);

            _context.Teams.Update(team);

            await _context.SaveChangesAsync();

            return Ok(new ResponseModel("Success"));
        }
    }
}
