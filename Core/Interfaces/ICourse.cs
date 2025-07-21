using Core.DTOs;
using Core.Models;

namespace Core.Interfaces
{
    public interface ICourse
    {
        Task<List<Course>> GetAllCoursesAsync();
        Task<List<Course>> GetEnrolledCoursesAsync(string studentId);
        Task<List<Course>> GetOwnedCoursesAsync(string instructorId);
        Task<Course> GetCourseByIdAsync(int courseId);
        Task<Course> AddCourseAsync(CourseDTO course);
        Task<Course> UpdateCourseAsync(int oldcourseId, CourseDTO newCourse);
        Task<bool> DeleteCourseAsync(int courseId);

        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int categoryId);
    }
}
