using System;

namespace H5ServerSideProgrammering.Models.DB
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Added { get; set; }
        public string Description { get; set; }

        public Login login { get; set; }
        public int LoginId { get; set; }
    }
}
