﻿using CommandManagment.backend.Data;
using CommandManagment.backend.Helpers;
using CommandManagment.backend.Models;
using CommandManagment.backend.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommandManagment.backend.Controllers
{

    [Route("api/[controller]")]
    public class BoardController : Controller
    {

        readonly AppDbContext _context;
        readonly ContextHelper _contextHelper;
        readonly JwtService _jwtService;

        public BoardController(AppDbContext context, ContextHelper contextHelper, JwtService jwtService)
        {
            _context = context;
            _contextHelper = contextHelper;
            _jwtService = jwtService;
        }

        [Authorize]
        [HttpGet("GetScrumBoard/{scrumBoardId}")]
        public async Task<IActionResult> GetScrumBoard(int scrumBoardId)
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);
            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong user email"));

            Board scrumBoard = await _context.ScrumBoards.Include(p => p.ScrumBoardTasks).Include(p => p.ScrumBoardColumns).Include(p => p.Project).FirstOrDefaultAsync(p => p.Id == scrumBoardId);

            Project project = await _context.Projects.Include(p => p.Team).FirstOrDefaultAsync(p => p.Id == scrumBoard.ProjectId);

            scrumBoard.ScrumBoardColumns = scrumBoard.ScrumBoardColumns.OrderBy(o => o.Order).ToList();
            scrumBoard.ScrumBoardTasks = scrumBoard.ScrumBoardTasks.OrderBy(o => o.Order).Where(t => t.IsArchived == false).ToList();
            scrumBoard.TeamUsers = await _context.Users.Where(u => u.Teams.Contains(project.Team)).ToListAsync();

            return Ok(scrumBoard);
        }


        [Authorize]
        [HttpPut("ColumnsMove")]
        public async Task<IActionResult> ColumnsMove([FromBody] List<BoardColumn> scrumBoardColumns)
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);
            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong user email"));

            foreach (BoardColumn newColumn in scrumBoardColumns)
            {
                await _context.ScrumBoardColumns.Where(t => t.Id == newColumn.Id).ExecuteUpdateAsync(s => s.SetProperty(u => u.Order, newColumn.Order));
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPost("ColumnAdd")]
        public async Task<IActionResult> ColumnAdd([FromBody] BoardColumn scrumBoardColumn)
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);
            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong user email"));

            Board scrumBoard = await _context.ScrumBoards.Include(p => p.ScrumBoardColumns).FirstOrDefaultAsync(p => p.Id == scrumBoardColumn.ScrumBoardId);
            scrumBoard.ScrumBoardColumns.Add(scrumBoardColumn);

            scrumBoardColumn.Id = null;

            _context.ScrumBoards.Update(scrumBoard);
            await _context.SaveChangesAsync();

            return Ok(scrumBoardColumn);
        }

        [Authorize]
        [HttpDelete("ColumnDelete/{columnId}")]
        public async Task<IActionResult> ColumnDelete(Guid columnId)
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);
            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong user email"));

            await _context.ScrumBoardColumns.Where(c => c.Id == columnId).ExecuteDeleteAsync();

            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPut("ColumnUpdate")]
        public async Task<IActionResult> ColumnUpdate([FromBody] BoardColumn scrumBoardColumn)
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);
            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong user email"));

            _context.ScrumBoardColumns.Update(scrumBoardColumn);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPost("TaskAdd")]
        public async Task<IActionResult> TaskAdd([FromBody] BoardTask scrumBoardTask)
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);
            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong user email"));
            scrumBoardTask.ResponsibleUser = await _contextHelper.GetUserById(scrumBoardTask.ResponsibleUserId);

            Board scrumBoard = await _context.ScrumBoards.Include(p => p.ScrumBoardTasks).FirstOrDefaultAsync(p => p.Id == scrumBoardTask.ScrumBoardId);

            scrumBoardTask.Id = null;

            scrumBoard.ScrumBoardTasks.Add(scrumBoardTask);

            _context.ScrumBoards.Update(scrumBoard);
            await _context.SaveChangesAsync();

            return Ok(scrumBoardTask);
        }

        [Authorize]
        [HttpDelete("TaskDelete/{taskId}")]
        public async Task<IActionResult> TaskDelete(Guid taskId)
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);
            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong user email"));

            await _context.ScrumBoardTasks.Where(t => t.Id == taskId).ExecuteDeleteAsync();
            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPut("TasksMove")]
        public async Task<IActionResult> TasksMove([FromBody] List<BoardTask> scrumBoardTasks)
        {
            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);
            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong user email"));

            foreach (BoardTask newColumn in scrumBoardTasks)
            {
                await _context.ScrumBoardTasks.Where(t => t.Id == newColumn.Id).ExecuteUpdateAsync(s => s.SetProperty(u => u.Order, newColumn.Order).SetProperty(u => u.ScrumBoardColumnId, newColumn.ScrumBoardColumnId));
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPut("TaskUpdate")]
        public async Task<IActionResult> TaskUpdate([FromBody] BoardTask scrumBoardTask)
        {
            scrumBoardTask.ResponsibleUser = null;
            scrumBoardTask.CreateUserTask = null;

            string userEmail = _jwtService.GetUserEmailFromJwt(Request.Headers["Authorization"]);
            User user = await _contextHelper.GetUserByEmail(userEmail);

            if (user == null)
                return BadRequest(new ResponseModel("Wrong user email"));

            _context.ScrumBoardTasks.Update(scrumBoardTask);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
