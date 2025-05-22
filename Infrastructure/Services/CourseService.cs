using Core.DTOs;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class CourseService : ICourse
    {
        private readonly E_LearningDbContext dbContext;
        private readonly IGenericRepo<Course> repo;
        private readonly ImagesUploadsService uploadsService;
        private readonly IGenericRepo<Category> catRepo;
        private readonly IGenericRepo<InstructorUser> instructorRepo;
        public CourseService(E_LearningDbContext _context, IGenericRepo<Course> repo, ImagesUploadsService _uploadsService, IGenericRepo<Category> catRepo, IGenericRepo<InstructorUser> instructorRepo)
        {
            dbContext = _context;
            this.repo = repo;
            uploadsService = _uploadsService;
            this.catRepo = catRepo;
            this.instructorRepo = instructorRepo;
        }
        public async Task<Course> AddCourseAsync(CourseDTO course)
        {
            var courseImage = await uploadsService.UploadImage(course.CourseImage, course.Title);
            var newCourse = new Course
            {
                Title = course.Title,
                Description = course.Description,
                NuOfLessons = course.NuOfLessons,
                OriginalPrice = course.OriginalPrice,
                SalePrice = course.SalePrice,
                CourseImagePath = courseImage,
                CategoryId = course.CategoryId,
                InstructorId = course.InstructorId
            };
            await repo.AddAsync(newCourse);
            await repo.SaveAllAsync();
            return newCourse;
        }

        public async Task<bool> DeleteCourseAsync(int courseId)
        {
            var course = await repo.GetByIdAsync(courseId);
            if (course is null) return false;
            course.IsDeleted = true;
            repo.Update(course);
            return await repo.SaveAllAsync();
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            var categories = await catRepo.GetAllAsync();
            return (List<Category>)categories;
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            var courses = await repo.GetAllAsync();
            return (List<Course>)courses;
        }

        public async Task<Course> GetCourseByIdAsync(int courseId)
        {
            var course = await repo.GetByIdAsync(courseId);
            if (course is not null && course.IsDeleted)
            {
                course = null;
            }
            return course;
        }

        public async Task<List<Course>> GetEnrolledCoursesAsync(int studentId)
        {
            var courses = await repo.GetAllAsync();
            var studentCourses = courses
                .Where(c => c.EnrolledStudents != null && c.EnrolledStudents.Any(s => s.StudentId == studentId))
                .ToList();
            return studentCourses;
        }

        public async Task<InstructorUser> GetInstructorByIdAsync(string instructorId)
        {
            var instructor = await instructorRepo.GetByIdAsync(instructorId);
            if (instructor is null || instructor.IsDeleted)
            {
                throw new KeyNotFoundException("Instructor not found or has been deleted");
            }
            return instructor;
        }

        public async Task<List<Course>> GetOwnedCoursesAsync(string instructorId)
        {
            var courses = await repo.GetAllAsync();
            var instructorCourses = courses.Where(c => c.InstructorId == instructorId.ToString()).ToList();
            return instructorCourses;
        }

        public async Task<Course> UpdateCourseAsync(int oldCourseId, CourseDTO newCourse)
        {
            var oldCourse = await repo.GetByIdAsync(oldCourseId);
            if (oldCourse is null || oldCourse.IsDeleted)
            {
                throw new KeyNotFoundException("Course not found or has been deleted");
            }
            oldCourse.Title = newCourse.Title;
            oldCourse.Description = newCourse.Description;
            oldCourse.NuOfLessons = newCourse.NuOfLessons;
            oldCourse.OriginalPrice = newCourse.OriginalPrice;
            oldCourse.SalePrice = newCourse.SalePrice;
            if (newCourse.CourseImage is not null)
            {
                var newCourseImage = await uploadsService.UploadImage(newCourse.CourseImage, newCourse.Title);
                oldCourse.CourseImagePath = newCourseImage;
            }
            repo.Update(oldCourse);
            await repo.SaveAllAsync();
            return oldCourse;
        }
    }
}
