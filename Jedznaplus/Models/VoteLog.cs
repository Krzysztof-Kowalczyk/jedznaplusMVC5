namespace Jedznaplus.Models
{
    public class VoteLog
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public int VoteForId { get; set; }
        public string UserName { get; set; }
        public int Vote { get; set; }
        public bool Active { get; set; }

    }
}