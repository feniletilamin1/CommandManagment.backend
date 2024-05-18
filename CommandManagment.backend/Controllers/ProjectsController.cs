using CommandManagment.backend.Data;
using CommandManagment.backend.Dtos;
using CommandManagment.backend.Helpers;
using CommandManagment.backend.Models;
using CommandManagment.backend.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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


            List<Project> projects = await _context.Projects.Include(o => o.Team)
                .Include(o => o.CreateUser)
                .Include(p => p.Board)
                .Where(p => p.Team.Users.Contains(user))
                .ToListAsync();

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

            List<BoardColumn> scrumBoardInitialColumns = new()
            {
                new BoardColumn
                {
                    Name = "Сделать",
                    Order = 0,

                },
                new BoardColumn
                {
                    Name = "В работе",
                    Order = 1,
                },
                new BoardColumn
                {
                    Name = "Сделано",
                    Order = 2,
                },
            };

            Board scrumBoard = new()
            {
                Project = project,
                User = user,
                ScrumBoardColumns = scrumBoardInitialColumns,
            };

            await _context.ScrumBoards.AddAsync(scrumBoard);
            await _context.SaveChangesAsync();

            return Ok(project);
        }


        [Authorize]
        [HttpDelete("DeleteProject/{projectId}")]
        public async Task<IActionResult> DeleteProject(int projectId)
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);

            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong user email"));

            Project project = await _context.Projects.FirstOrDefaultAsync(i => i.Id == projectId);

            if(project == null)
                return BadRequest(new ResponseModel("Wrong project id"));

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return Ok(project);
        }

        [Authorize]
        [HttpGet("GetUserTasks")]
        public async Task<IActionResult> GetUserTasks()
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);

            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong user"));

            List<BoardTask> scrumBoardTasks = await _context.ScrumBoardTasks.Where(t => t.ResponsibleUser == user).ToListAsync();

            return Ok(scrumBoardTasks);
        }

        [Authorize]
        [HttpGet("GetUserArchivedTasks")]
        public async Task<IActionResult> GetUserArchivedTasks()
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);

            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong user"));

            List<BoardTask> scrumBoardTasks = await _context.ScrumBoardTasks.Where(t => t.ResponsibleUser == user && t.IsArchived == true).ToListAsync();

            return Ok(scrumBoardTasks);
        }
    }
}
