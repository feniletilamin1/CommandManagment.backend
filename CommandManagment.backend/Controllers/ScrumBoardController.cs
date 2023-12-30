using CommandManagment.backend.Data;
using CommandManagment.backend.Dtos;
using CommandManagment.backend.Helpers;
using CommandManagment.backend.Models;
using CommandManagment.backend.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static CommandManagment.backend.Dtos.ScrumBoardColumnsMoveDto;

namespace CommandManagment.backend.Controllers
{

    [Route("api/[controller]")]
    public class ScrumBoardController : Controller
    {

        readonly AppDbContext _context;
        readonly ContextHelper _contextHelper;
        readonly JwtService _jwtService;

        public ScrumBoardController(AppDbContext context, ContextHelper contextHelper, JwtService jwtService)
        {
            _context = context;
            _contextHelper = contextHelper;
            _jwtService = jwtService;
        }

        [Authorize]
        [HttpGet("GetScrumBoard/{projectId}")]
        public async Task<IActionResult> GetScrumBoard(int projectId)
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);
            User user = await _contextHelper.GetUserByEmail(userEmail);
            Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

            if (user == null || project == null)
                return BadRequest(new ResponseModel("Wrong user email or projectId"));

            ScrumBoard scrumBoard = await _context.ScrumBoards.Include(p => p.ScrumBoardTasks).Include(p => p.ScrumBoardColumns).FirstOrDefaultAsync(p => p.ProjectId == projectId && p.UserId == user.Id);
            
            scrumBoard.ScrumBoardColumns = scrumBoard.ScrumBoardColumns.OrderBy(o => o.Order).ToList();
            scrumBoard.ScrumBoardTasks = scrumBoard.ScrumBoardTasks.OrderBy(o => o.Order).ToList();

            return Ok(scrumBoard);
        }


        [Authorize]
        [HttpPost("ColumnsMove")]
        public async Task<IActionResult> ColumnsMove([FromBody] ScrumBoardColumnsMoveDto scrumBoardColumnsMoveDto)
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);
            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong user email"));

            ScrumBoard scrumBoard = await _context.ScrumBoards.Include(p => p.ScrumBoardColumns).FirstOrDefaultAsync(p => p.Id == scrumBoardColumnsMoveDto.NewColumns[0].ScrumBoardId && p.UserId == user.Id);

            foreach (ScrumBoardColumn column in scrumBoard.ScrumBoardColumns)
            {
                foreach (ScrumBoardColumnMoved newColumn in scrumBoardColumnsMoveDto.NewColumns)
                {
                    if(column.Id == newColumn.Id)
                    {
                        column.Order = newColumn.Order;
                        continue;
                    }
                }
            }

            _context.ScrumBoards.Update(scrumBoard);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPost("ColumnAdd")]
        public async Task<IActionResult> ColumnAdd([FromBody] ScrumBoardColumn scrumBoardColumn)
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);
            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong user email"));

            ScrumBoard scrumBoard = await _context.ScrumBoards.Include(p => p.ScrumBoardColumns).FirstOrDefaultAsync(p => p.Id == scrumBoardColumn.ScrumBoardId);
            scrumBoard.ScrumBoardColumns.Add(scrumBoardColumn);

            scrumBoardColumn.Id = null;

            _context.ScrumBoards.Update(scrumBoard);
            await _context.SaveChangesAsync();

            return Ok(scrumBoardColumn);
        }

        [Authorize]
        [HttpPost("ColumnDelete")]
        public async Task<IActionResult> ColumnDelete([FromBody] ScrumBoardColumn scrumBoardColumn)
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);
            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong user email"));

            ScrumBoard scrumBoard = await _context.ScrumBoards.Include(p => p.ScrumBoardColumns).FirstOrDefaultAsync(p => p.Id == scrumBoardColumn.ScrumBoardId);
            ScrumBoardColumn scrumBoardColumnDeleted = scrumBoard.ScrumBoardColumns.Single(r => r.Id == scrumBoardColumn.Id);
            scrumBoard.ScrumBoardColumns.Remove(scrumBoardColumnDeleted);

            _context.ScrumBoards.Update(scrumBoard);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPost("ColumnUpdate")]
        public async Task<IActionResult> ColumnUpdate([FromBody] ScrumBoardColumn scrumBoardColumn)
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);
            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong user email"));

            ScrumBoard scrumBoard = await _context.ScrumBoards.Include(p => p.ScrumBoardColumns).FirstOrDefaultAsync(p => p.Id == scrumBoardColumn.ScrumBoardId);
            ScrumBoardColumn scrumBoardColumnUpdated = scrumBoard.ScrumBoardColumns.Single(r => r.Id == scrumBoardColumn.Id);

            scrumBoardColumnUpdated.Name = scrumBoardColumn.Name;

            _context.ScrumBoardColumns.Update(scrumBoardColumnUpdated);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPost("TaskAdd")]
        public async Task<IActionResult> TaskAdd([FromBody] ScrumBoardTask scrumBoardTask)
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);
            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong user email"));

            ScrumBoard scrumBoard = await _context.ScrumBoards.Include(p => p.ScrumBoardTasks).FirstOrDefaultAsync(p => p.Id == scrumBoardTask.ScrumBoardId);

            scrumBoardTask.Id = null;

            scrumBoard.ScrumBoardTasks.Add(scrumBoardTask);

            _context.ScrumBoards.Update(scrumBoard);
            await _context.SaveChangesAsync();

            return Ok(scrumBoardTask);
        }
    }
}
