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
        private readonly ImagesUploadsService uploadsService;
        private readonly UserManager<BaseUser> userManager;
        private readonly IJWT jWT;
        public AuthService(ImagesUploadsService images, UserManager<BaseUser> manager, IJWT _jWT)
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
            var cv = await uploadsService.UploadPDF(registerDTO.CvPath, registerDTO.FirstName + " " + registerDTO.LastName);
            var video = await uploadsService.UploadVideo(registerDTO.VideoPath, registerDTO.FirstName + " " + registerDTO.LastName);
            var user = new InstructorUser
            {
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
            }
            authDto.Message = "تم تسجيل الدخول بنجاح";
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
            var userName = registerDTO.Email.Split("@")[0];
            var user = new StudentUser
            {
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber,
                DateOfBirth = registerDTO.DateOfBirth,
                UserName = userName
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
            if (registerDTO.Password != registerDTO.ConfirmPassword)
            {
                errors.Add("كلمة المرور غير متطابقة");
            }

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
            if (registerDTO.Password != registerDTO.ConfirmPassword)
            {
                errors.Add("كلمة المرور غير متطابقة");
            }

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
        //public async Task<AuthDTO> ApproveInstructorAsync(string instructorId)
        //{
        //    //To be continued
        //    //var test = await userManager.
        //    var instructor = await userManager.FindByIdAsync(instructorId);
        //    if (instructor is null)
        //    {
        //        throw new KeyNotFoundException("User can not be found");
        //    }

        //    var newInstructor = new InstructorUser
        //    {
        //        FirstName = instructor.FirstName,
        //        LastName = instructor.LastName,
        //        Email = instructor.Email,
        //        PhoneNumber = instructor.PhoneNumber,
        //        //CV = instructor.
        //    };
        //    await userManager.AddToRoleAsync(instructor, UserRoles.Instructor.ToString());
        //    //await userManager.addas
        //    return new AuthDTO
        //    {
        //        IsAuthenticated = true,
        //        Message = "تم تاكيد تسجيلك على منصتنا ك معلم"
        //    };
        //}

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
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.PhoneNumber = updatedUser.PhoneNumber;
            user.Email = updatedUser.Email;

            var result = await userManager.UpdateAsync(user);

            var authDTO = new AuthDTO
            {
                IsAuthenticated = true,
                Message = "تم تحديث بياناتك بنجاح"
            };
            return authDTO;
        }

        public async Task<UserDTO> GetUserByIDAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null || user.IsDeleted)
            {
                user = null;
            }
            var userDTO = new UserDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth
            };
            return userDTO;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return false;
            }
            user.IsDeleted = true;
            var result = await userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<AuthDTO> ApproveInstructorAsync(string instructorId)
        {
            var user = await userManager.FindByIdAsync(instructorId);
            if (user is not InstructorUser instructor)
            {
                throw new KeyNotFoundException("Instructor not found");
            }
            if (instructor.IsApproved)
            {
                return new AuthDTO
                {
                    IsAuthenticated = true,
                    Message = "المعلم تم تأكيده بالفعل"
                };
            }
            instructor.IsApproved = true;
            var result = await userManager.UpdateAsync(instructor);
            if (!result.Succeeded)
            {
                return FailResult("Failed to approve instructor");
            }
            return new AuthDTO
            {
                IsAuthenticated = true,
                Message = "تم الموافقة على المعلم بنجاح"
            };
        }
    }
}
