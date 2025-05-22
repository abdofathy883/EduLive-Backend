using Core.DTOs;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ICourse
    {
        Task<List<Course>> GetAllCoursesAsync();
        Task<List<Course>> GetEnrolledCoursesAsync(int studentId);
        Task<List<Course>> GetOwnedCoursesAsync(string instructorId);
        Task<Course> GetCourseByIdAsync(int courseId);
        Task<Course> AddCourseAsync(CourseDTO course);
        Task<Course> UpdateCourseAsync(int oldcourseId, CourseDTO newCourse);
        Task<bool> DeleteCourseAsync(int courseId);

        Task<List<Category>> GetAllCategoriesAsync();
        Task<InstructorUser> GetInstructorByIdAsync(string instructorId);
    }
}
