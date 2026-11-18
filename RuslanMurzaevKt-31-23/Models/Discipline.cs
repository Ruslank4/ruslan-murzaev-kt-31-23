using System.Diagnostics;

namespace RuslanMurzaevKt_31_23.Models
{
    public class Discipline
    {
        public int DisciplineId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
