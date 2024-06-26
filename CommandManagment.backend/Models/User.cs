﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CommandManagment.backend.Models
{
    public class User
    {   
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? MiddleName { get; set; }
        public string Email { get; set; }
        public string Specialization { get; set; }
        public string Photo { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        [JsonIgnore]
        public List<Team>? Teams { get; set; }
        [JsonIgnore]
        public List<Board>? ScrumBoards { get; set; }
        [JsonIgnore]
        public List<BoardTask>? UserTasks { get; set; }
        public List<BoardTask>? CreatedTasks { get; set; }

        public User()
        {
            Teams = new List<Team>();
            ScrumBoards = new List<Board>();
            UserTasks = new List<BoardTask>();
            CreatedTasks = new List<BoardTask>();
        }
    }
}
