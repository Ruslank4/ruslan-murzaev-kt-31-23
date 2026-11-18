using Microsoft.EntityFrameworkCore;
using RuslanMurzaevKt_31_23.Database;
using RuslanMurzaevKt_31_23.Filters.StudentFilters;
using RuslanMurzaevKt_31_23.Models;

namespace RuslanMurzaevKt_31_23.Interfaces.StudentsInterfaces
{
    public interface IStudentService
    {
        Task<string[]> GetUniqueStudentNamesBySpecialtyAsync(StudentSpecialtyFilter filter, CancellationToken cancellationToken);
        Task<StudentDto[]> GetAllStudentsAsync(CancellationToken cancellationToken);
        Task<StudentDto?> GetStudentByIdAsync(int studentId, CancellationToken cancellationToken);
        Task<StudentDto> AddStudentAsync(AddStudentDto dto, CancellationToken cancellationToken);
        Task<StudentDto?> UpdateStudentAsync(UpdateStudentDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteStudentAsync(int studentId, CancellationToken cancellationToken);
    }

    public class StudentService : IStudentService
    {
        private readonly StudentDbContext _dbContext;

        public StudentService(StudentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string[]> GetUniqueStudentNamesBySpecialtyAsync(StudentSpecialtyFilter filter, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<Student>()
                .Where(w => !w.IsDeleted
                         && !w.Group.IsDeleted
                         && w.Group.Specialty.Title == filter.SpecialtyTitle)
                .Select(s => s.FirstName)
                .Distinct()
                .ToArrayAsync(cancellationToken);
        }

        public async Task<StudentDto[]> GetAllStudentsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<Student>()
                .Where(s => !s.IsDeleted)
                .Select(s => new StudentDto
                {
                    StudentId = s.StudentId,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    GroupId = s.GroupId,
                    SpecialtyTitle = s.Group.Specialty.Title
                })
                .ToArrayAsync(cancellationToken);
        }

        public async Task<StudentDto?> GetStudentByIdAsync(int studentId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<Student>()
                .Where(s => s.StudentId == studentId && !s.IsDeleted)
                .Select(s => new StudentDto
                {
                    StudentId = s.StudentId,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    GroupId = s.GroupId,
                    SpecialtyTitle = s.Group.Specialty.Title
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<StudentDto> AddStudentAsync(AddStudentDto dto, CancellationToken cancellationToken = default)
        {
            var group = await GetOrCreateGroupByNumberAsync(dto.GroupId, dto.SpecialtyTitle, cancellationToken);

            var student = new Student
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                GroupId = group.GroupId,
                IsDeleted = false
            };

            await _dbContext.Set<Student>().AddAsync(student, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new StudentDto
            {
                StudentId = student.StudentId,
                FirstName = student.FirstName,
                LastName = student.LastName,
                GroupId = group.GroupId,
                SpecialtyTitle = group.Specialty.Title
            };
        }

        public async Task<StudentDto?> UpdateStudentAsync(UpdateStudentDto dto, CancellationToken cancellationToken = default)
        {
            var student = await _dbContext.Set<Student>()
                .Include(s => s.Group)
                    .ThenInclude(g => g.Specialty)
                .FirstOrDefaultAsync(s => s.StudentId == dto.StudentId && !s.IsDeleted, cancellationToken);

            if (student == null)
                return null;

            student.FirstName = dto.FirstName;
            student.LastName = dto.LastName;

            var targetGroupNum = dto.GroupId > 0 ? dto.GroupId : student.GroupId;
            var targetSpecialtyTitle = string.IsNullOrWhiteSpace(dto.SpecialtyTitle) ? student.Group.Specialty.Title : dto.SpecialtyTitle;

            var group = await GetOrCreateGroupByNumberAsync(targetGroupNum, targetSpecialtyTitle, cancellationToken);
            student.GroupId = group.GroupId;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new StudentDto
            {
                StudentId = student.StudentId,
                FirstName = student.FirstName,
                LastName = student.LastName,
                GroupId = group.GroupId,
                SpecialtyTitle = group.Specialty.Title
            };
        }

        public async Task<bool> DeleteStudentAsync(int studentId, CancellationToken cancellationToken = default)
        {
            var student = await _dbContext.Set<Student>()
                .FirstOrDefaultAsync(s => s.StudentId == studentId && !s.IsDeleted, cancellationToken);

            if (student == null)
                return false;

            student.IsDeleted = true;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        private async Task<Group> GetOrCreateGroupByNumberAsync(int groupNumber, string specialtyTitle, CancellationToken cancellationToken)
        {
            // 1. Находим или создаем специальность по названию
            var specialty = await _dbContext.Set<Specialty>()
                .FirstOrDefaultAsync(s => s.Title == specialtyTitle, cancellationToken);

            if (specialty == null)
            {
                specialty = new Specialty
                {
                    Title = specialtyTitle,
                    Code = "00.00.00"
                };
                await _dbContext.Set<Specialty>().AddAsync(specialty, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            // 2. Ищем группу с таким номером (по GroupId или Name) и нужной специальностью
            var group = await _dbContext.Set<Group>()
                .FirstOrDefaultAsync(g => (g.GroupId == groupNumber || g.Name == groupNumber.ToString())
                                       && g.SpecialtyId == specialty.SpecialtyId
                                       && !g.IsDeleted, cancellationToken);

            // 3. Если такой группы для этой специальности еще нет — создаем с именем "1", "2" и т.д.
            if (group == null)
            {
                group = new Group
                {
                    Name = groupNumber.ToString(),
                    Course = 1,
                    IsDeleted = false,
                    SpecialtyId = specialty.SpecialtyId
                };
                await _dbContext.Set<Group>().AddAsync(group, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            group.Specialty = specialty;
            return group;
        }
    }
}
