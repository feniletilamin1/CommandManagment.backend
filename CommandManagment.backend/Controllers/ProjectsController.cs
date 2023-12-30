using CommandManagment.backend.Data;
using CommandManagment.backend.Dtos;
using CommandManagment.backend.Helpers;
using CommandManagment.backend.Models;
using CommandManagment.backend.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CommandManagment.backend.Controllers 
{ 

    [Route("api/[controller]")]
    public class ProjectsController : Controller 
    {

        readonly AppDbContext _context;
        readonly ContextHelper _contextHelper;
        readonly JwtService _jwtService;

        public ProjectsController(AppDbContext context, ContextHelper contextHelper, JwtService jwtService)
        {
            _context = context;
            _contextHelper = contextHelper;
            _jwtService = jwtService;
        }

        [Authorize]
        [HttpGet("GetProjects")]
        public async Task<IActionResult> GetProjects()
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);

            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong user email"));

            List<Project> projects = await _context.Projects.Where(p => p.CreateUserId == user.Id).Include(o => o.Team).Include(o => o.CreateUser).ToListAsync();

            return Ok(projects);
        }


        [Authorize]
        [HttpPost("AddProject")]
        public async Task<IActionResult> AddProject([FromBody] ProjectDto projectDto)
        {
            User user = await _contextHelper.GetUserById(projectDto.CreateUserId);
            
            if (user == null)
                return BadRequest(new ResponseModel("Wrong user id"));

            Team team = await _contextHelper.GetTeamById(projectDto.TeamId);

            if (team == null)
                return BadRequest(new ResponseModel("Wrong team id"));

            Project project = new()
            {
                Name = projectDto.TeamName,
                TeamId = projectDto.TeamId,
                CreateUserId = projectDto.CreateUserId,
                Team = team,
            };

            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();

            List<ScrumBoardColumn> scrumBoardInitialColumns = new()
            {
                new ScrumBoardColumn
                {
                    Name = "Сделать",
                    Order = 0,

                },
                new ScrumBoardColumn
                {
                    Name = "В работе",
                    Order = 1,
                },
                new ScrumBoardColumn
                {
                    Name = "Сделано",
                    Order = 2,
                },
            };

            ScrumBoard scrumBoard = new()
            {
                Project = project,
                User = user,
                ScrumBoardColumns = scrumBoardInitialColumns,
            };

            await _context.ScrumBoards.AddAsync(scrumBoard);
            await _context.SaveChangesAsync();

            return Ok(project);
        }
    }
}
