using Core.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace eLearning_Admin.Pages
{
    //[Authorize]
    public class IndexModel : PageModel
    {
        private readonly E_LearningDbContext dbContext;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, E_LearningDbContext e_Learning)
        {
            _logger = logger;
            dbContext = e_Learning;
        }

        public int StudentsCount { get; set; }
        public int InstructorsCount { get; set; }
        public int CoursesCount { get; set; }
        public int LessonsCount { get; set; }
        public int ZoomSessionsCount { get; set; }
        public int GoogleMeetSessionsCount { get; set; }
        public int BlogsCount { get; set; }
        public int ContactFormsCount { get; set; }

        public void OnGet()
        {
            StudentsCount = dbContext.BaseUsers.OfType<StudentUser>().Count();
            InstructorsCount = dbContext.BaseUsers.OfType<InstructorUser>().Count();
            CoursesCount = dbContext.Courses.Count();
            LessonsCount = dbContext.Lessons.Count();
            ZoomSessionsCount = dbContext.ZoomUserConnections.Count();
            GoogleMeetSessionsCount = dbContext.GoogleMeetLessons.Count();
            BlogsCount = dbContext.Blogs.Count();
            ContactFormsCount = dbContext.ContactForms.Count();
        }
    }
}
