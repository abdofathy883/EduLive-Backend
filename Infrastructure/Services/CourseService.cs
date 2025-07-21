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
        private readonly IGenericRepo<Course> repo;
        private readonly MediaUploadsService uploadsService;
        private readonly IGenericRepo<Category> catRepo;
        public CourseService(
            IGenericRepo<Course> repo, 
            MediaUploadsService _uploadsService, 
            IGenericRepo<Category> catRepo
            )
        {
            this.repo = repo;
            uploadsService = _uploadsService;
            this.catRepo = catRepo;
        }
        public async Task<Course> AddCourseAsync(CourseDTO course)
        {
            if (course is null)
                throw new ArgumentNullException(nameof(course), "Course cannot be null");

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
            var course = await repo.GetByIdAsync(courseId)
                ?? throw new KeyNotFoundException("Course not found or has been deleted");
            
            course.IsDeleted = true;
            repo.Update(course);
            return await repo.SaveAllAsync();
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            var categories = await catRepo.FindAsync(c => !c.IsDeleted)
                ?? throw new KeyNotFoundException("No categories found");

            return (List<Category>)categories;
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            var courses = await repo.FindAsync(c => !c.IsDeleted)
                ?? throw new KeyNotFoundException("No courses found");

            return (List<Course>)courses;
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            var category = await catRepo.GetByIdAsync(categoryId)
                ?? throw new KeyNotFoundException("Category not found or has been deleted");
            if (category.IsDeleted)
                throw new KeyNotFoundException("Category has been deleted");

            return category;
        }

        public async Task<Course> GetCourseByIdAsync(int courseId)
        {
            var course = await repo.GetByIdAsync(courseId)
                ?? throw new KeyNotFoundException("Course not found or has been deleted");
            
            if (course.IsDeleted)
                throw new KeyNotFoundException("Course has been deleted");

            return course;
        }

        public async Task<List<Course>> GetEnrolledCoursesAsync(string studentId)
        {
            var courses = await repo.FindAsync(c => !c.IsDeleted && c.EnrolledStudents.Any(s => s.StudentId == studentId))
                ?? throw new KeyNotFoundException("No enrolled courses found for this student");
            
            return (List<Course>)courses;
        }

        public async Task<List<Course>> GetOwnedCoursesAsync(string instructorId)
        {
            var courses = await repo.FindAsync(courses => !courses.IsDeleted && courses.InstructorId == instructorId)
                ?? throw new KeyNotFoundException("No owned courses found for this instructor");

            return (List<Course>)courses;
        }

        public async Task<Course> UpdateCourseAsync(int oldCourseId, CourseDTO newCourse)
        {
            var oldCourse = await repo.GetByIdAsync(oldCourseId)
                ?? throw new KeyNotFoundException("Course not found or has been deleted");
            if (oldCourse.IsDeleted)
                throw new KeyNotFoundException("Course has been deleted");

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

        public async Task<List<AuthDTO>> GetStudentsByCourseIdAsync(int courseId)
        {
            var course = await repo.GetByIdAsync(courseId)
                ?? throw new KeyNotFoundException("Course not found or has been deleted");

            if (course.IsDeleted)
                throw new KeyNotFoundException("Course has been deleted");

            var students = course.EnrolledStudents
                ?? throw new KeyNotFoundException("No students enrolled in this course");

            var studentDTOs = students.Select(s => new AuthDTO
            {
                UserId = s.StudentId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                ConcurrencyStamp = s.ConcurrencyStamp
            }).ToList();

            return studentDTOs;
        }
    }
}
