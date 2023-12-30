namespace CommandManagment.backend.Dtos
{
    public class ScrumBoardColumnsMoveDto
    {
        public List<ScrumBoardColumnMoved> NewColumns { get; set; }

        public class ScrumBoardColumnMoved
        {
            public Guid Id { get; set; }
            public int Order { get; set; }
            public int ScrumBoardId { get; set; }
        }
    }
}

  
