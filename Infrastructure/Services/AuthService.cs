using Core.DTOs;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AuthService : IAuth
    {
        private readonly MediaUploadsService uploadsService;
        private readonly UserManager<BaseUser> userManager;
        private readonly IJWT jWT;
        public AuthService(MediaUploadsService images, UserManager<BaseUser> manager, IJWT _jWT)
        {
            uploadsService = images;
            userManager = manager;
            jWT = _jWT;
        }
        public async Task<AuthDTO> InstructorRegisterAsync(InstructorRegisterDTO registerDTO)
        {
            var validateErrors = await ValidateInstructorRegisterAsync(registerDTO);
            if (validateErrors is not null && validateErrors.Count > 0)
            {
                return FailResult(string.Join(", ", validateErrors));
            }

            // Convert the VideoPath (string) to an IFormFile before calling UploadVideo
            var cv = await uploadsService.UploadPDF(registerDTO.CvPath, registerDTO.FirstName + registerDTO.LastName);
            var video = await uploadsService.UploadVideo(registerDTO.VideoPath, registerDTO.FirstName + registerDTO.LastName);
            var user = new InstructorUser
            {
                InstructorId = Guid.NewGuid().ToString(),
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber,
                CVPath = cv,
                IntroVideoPath = video,
                Bio = registerDTO.Bio,
                DateOfBirth = registerDTO.DateOfBirth,
                UserName = registerDTO.Email.Split("@")[0],
                IsApproved = false
            };

            var result = await userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
            {
                return FailResult(string.Join(", ", validateErrors));
            }
            await userManager.AddToRoleAsync(user, UserRoles.Instructor.ToString());

            var authDTO = new AuthDTO
            {
                IsAuthenticated = true,
                Message = "تم استلام طلبكم وسيتم التواصل معكم قريبا"
            };
            return authDTO;
        }

        public async Task<AuthDTO> LoginAsync(LoginDTO loginDTO)
        {
            var authDto = new AuthDTO();
            var user = await userManager.FindByEmailAsync(loginDTO.Email);

            if (user is null || !await userManager.CheckPasswordAsync(user, loginDTO.Password))
            {
                authDto.IsAuthenticated = false;
                authDto.Message = "الايمل او الرقم السري غير صحيح";
                return authDto;
            }

            if (user.IsDeleted)
            {
                authDto.IsAuthenticated = false;
                authDto.Message = "هذا الحساب غير متاح";
                return authDto;
            }

            authDto.IsAuthenticated = true;
            authDto.Email = user.Email;
            authDto.UserName = user.UserName;
            authDto.UserId = user.Id;
            authDto.FirstName = user.FirstName;
            authDto.LastName = user.LastName;
            authDto.PhoneNumber = user.PhoneNumber;
            authDto.DateOfBirth = (DateOnly)user.DateOfBirth;
            var roles = await userManager.GetRolesAsync(user);
            authDto.Roles = roles.ToList();
            authDto.Token = await jWT.GenerateAccessTokenAsync(user);
            authDto.ConcurrencyStamp = user.ConcurrencyStamp;

            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var ActiveRefreshToken = user.RefreshTokens.First(t => t.IsActive);
                authDto.RefreshToken = ActiveRefreshToken.Token;
                authDto.RefreshTokenExpiration = ActiveRefreshToken.ExpiresOn;
            }
            else
            {
                var RefreshToken = await jWT.GenerateRefreshTokenAsync();
                authDto.RefreshToken = RefreshToken.Token;
                authDto.RefreshTokenExpiration = RefreshToken.ExpiresOn;
                user.RefreshTokens.Add(RefreshToken);
                await userManager.UpdateAsync(user);
            }
            if (user is InstructorUser instructor)
            {
                authDto.CV = instructor.CVPath;
                authDto.IntroVideo = instructor.IntroVideoPath;
                authDto.Bio = instructor.Bio;
                authDto.IsApproved = instructor.IsApproved;
            }
            authDto.Message = "تم تسجيل الدخول بنجاح";
            Console.WriteLine($"ConcurrencyStamp: {user.ConcurrencyStamp}");
            return authDto;
        }

        public Task<AuthDTO> RefreshTokenAsync(string token)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthDTO> RegisterAsync(RegisterDTO registerDTO)
        {
            var validateErrors = await ValidateRegisterAsync(registerDTO);
            if (validateErrors is not null && validateErrors.Count > 0)
            {
                return FailResult(string.Join(", ", validateErrors));
            }

            var user = new StudentUser
            {
                StudentId = Guid.NewGuid().ToString(),
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber,
                DateOfBirth = registerDTO.DateOfBirth,
                UserName = registerDTO.Email.Split("@")[0]
            };
            var result = await userManager.CreateAsync(user, registerDTO.Password);

            //Add Role
            if (!result.Succeeded)
            {
                return FailResult(string.Join(", ", validateErrors));
            }
            await userManager.AddToRoleAsync(user, UserRoles.Student.ToString());

            var authDTO = new AuthDTO
            {
                IsAuthenticated = true,
                Message = "تم تسجيل حساب جديد بنجاح, يمكنك تسجيل دخول"
            };
            return authDTO;
        }

        public async Task<List<string>> ValidateRegisterAsync(RegisterDTO registerDTO)
        {
            var errors = new List<string>();

            // Email Validation
            if (string.IsNullOrWhiteSpace(registerDTO.Email))
            {
                errors.Add("بريد الكتروني غير صالح");
            }
            if (await userManager.FindByEmailAsync(registerDTO.Email) is not null)
            {
                errors.Add("هذا الايميل موجود بالفعل");
            }

            //Password Validation
            if (string.IsNullOrWhiteSpace(registerDTO.Password))
            {
                errors.Add("الرقم السري مطلوب");
            } 
            else if (registerDTO.Password.Length < 6)
            {
                errors.Add("الرقم السري يجب ان يكون 6 احرف على الاقل");
            }

            //Confirm Password
            //if (registerDTO.Password != registerDTO.ConfirmPassword)
            //{
            //    errors.Add("كلمة المرور غير متطابقة");
            //}

            //Phone 
            if (string.IsNullOrWhiteSpace(registerDTO.PhoneNumber))
            {
                errors.Add("رقم الهاتف مطلوب");
            }

            //Name
            if (string.IsNullOrWhiteSpace(registerDTO.FirstName))
            {
                errors.Add("الاسم الاول مطلوب");
            }
            if (string.IsNullOrWhiteSpace(registerDTO.LastName))
            {
                errors.Add("الاسم الاخير مطلوب");
            }

            return errors;
        }
        public async Task<List<string>> ValidateInstructorRegisterAsync(InstructorRegisterDTO registerDTO)
        {
            var errors = new List<string>();

            // Email Validation
            if (string.IsNullOrWhiteSpace(registerDTO.Email))
            {
                errors.Add("بريد الكتروني غير صالح");
            }
            if (await userManager.FindByEmailAsync(registerDTO.Email) is not null)
            {
                errors.Add("هذا الايميل موجود بالفعل");
            }

            //Password Validation
            if (string.IsNullOrWhiteSpace(registerDTO.Password))
            {
                errors.Add("الرقم السري مطلوب");
            } 
            else if (registerDTO.Password.Length < 6)
            {
                errors.Add("الرقم السري يجب ان يكون 6 احرف على الاقل");
            }

            //Confirm Password
            //if (registerDTO.Password != registerDTO.ConfirmPassword)
            //{
            //    errors.Add("كلمة المرور غير متطابقة");
            //}

            //Phone 
            if (string.IsNullOrWhiteSpace(registerDTO.PhoneNumber))
            {
                errors.Add("رقم الهاتف مطلوب");
            }

            //Name
            if (string.IsNullOrWhiteSpace(registerDTO.FirstName))
            {
                errors.Add("الاسم الاول مطلوب");
            }
            if (string.IsNullOrWhiteSpace(registerDTO.LastName))
            {
                errors.Add("الاسم الاخير مطلوب");
            }

            return errors;
        }
        public static AuthDTO FailResult(string message)
        {
            return new AuthDTO
            {
                IsAuthenticated = false,
                Message = message
            };
        }

        public async Task<AuthDTO> UpdateUserAsync(string userId, UpdateUserDTO updatedUser)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null || user.IsDeleted)
            {
                throw new KeyNotFoundException("User cannot be found or has been deleted");
            }

            if (!string.IsNullOrEmpty(updatedUser.ConcurrencyStamp))
            {
                user.ConcurrencyStamp = updatedUser.ConcurrencyStamp;
            }

            user.FirstName = updatedUser.FirstName ?? user.FirstName;
            user.LastName = updatedUser.LastName ?? user.LastName;
            user.PhoneNumber = updatedUser.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(updatedUser.Email) && updatedUser.Email != user.Email)
            {
                var changeEmailResult = await userManager.ChangeEmailAsync(user, updatedUser.Email, userId);
                if (!changeEmailResult.Succeeded)
                {
                    throw new InvalidOperationException("Failed to update email: " + string.Join(", ", changeEmailResult.Errors.Select(e => e.Description)));
                }
            }
            //if (user is InstructorUser)
            //{
            //    user.
            //}
            user.UpdatedAt = DateTime.UtcNow;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Failed to update user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            var authDTO = new AuthDTO
            {
                IsAuthenticated = true,
                Message = "تم تحديث بياناتك بنجاح", 
            };
            return authDTO;
        }

        public async Task<UserDTO> GetUserByIDAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null || user.IsDeleted)
            {
                return null;
            }
            var userDTO = new UserDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                ConcurrencyStamp = user.ConcurrencyStamp
            };
            return userDTO;
        }

        public async Task<InstructorUser> GetInstructorByIdAsync(string instructorId)
        {
            var instructor = await userManager.FindByIdAsync(instructorId);
            if (instructor is null || instructor.IsDeleted)
            {
                throw new KeyNotFoundException("Instructor not found or has been deleted");
            }
            return (InstructorUser)instructor;
        }
        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return false;
            }
            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;
            var result = await userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
