using Microsoft.AspNetCore.Mvc;
using RuslanMurzaevKt_31_23.Filters.StudentFilters;
using RuslanMurzaevKt_31_23.Interfaces.StudentsInterfaces;

namespace RuslanMurzaevKt_31_23.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly ILogger<StudentsController> _logger;
        private readonly IStudentService _studentService;

        public StudentsController(ILogger<StudentsController> logger, IStudentService studentService)
        {
            _logger = logger;
            _studentService = studentService;
        }

        [HttpGet(Name = "GetAllStudents")]
        public async Task<IActionResult> GetAllStudentsAsync(CancellationToken cancellationToken = default)
        {
            var students = await _studentService.GetAllStudentsAsync(cancellationToken);
            return Ok(students);
        }

        [HttpGet("{id}", Name = "GetStudentById")]
        public async Task<IActionResult> GetStudentByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var student = await _studentService.GetStudentByIdAsync(id, cancellationToken);
            if (student == null)
                return NotFound("Студент не найден или был удален");

            return Ok(student);
        }

        [HttpPost("unique-names", Name = "GetUniqueStudentNamesBySpecialty")]
        public async Task<IActionResult> GetUniqueStudentNamesBySpecialtyAsync(StudentSpecialtyFilter filter, CancellationToken cancellationToken = default)
        {
            var names = await _studentService.GetUniqueStudentNamesBySpecialtyAsync(filter, cancellationToken);
            return Ok(names);
        }

        [HttpPost("add", Name = "AddStudent")]
        public async Task<IActionResult> AddStudentAsync(AddStudentDto dto, CancellationToken cancellationToken = default)
        {
            var createdStudent = await _studentService.AddStudentAsync(dto, cancellationToken);
            return Ok(createdStudent);
        }

        [HttpPut("update", Name = "UpdateStudent")]
        public async Task<IActionResult> UpdateStudentAsync(UpdateStudentDto dto, CancellationToken cancellationToken = default)
        {
            var updatedStudent = await _studentService.UpdateStudentAsync(dto, cancellationToken);
            if (updatedStudent == null)
                return NotFound("Студент не найден или уже удален");

            return Ok(updatedStudent);
        }

        [HttpDelete("{id}", Name = "DeleteStudent")]
        public async Task<IActionResult> DeleteStudentAsync(int id, CancellationToken cancellationToken = default)
        {
            var isDeleted = await _studentService.DeleteStudentAsync(id, cancellationToken);
            if (!isDeleted)
                return NotFound("Студент не найден или уже удален");

            return Ok("Студент удален");
        }
    }
}
