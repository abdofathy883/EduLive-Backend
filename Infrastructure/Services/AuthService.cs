using Core.DTOs;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class AuthService : IAuth
    {
        private readonly MediaUploadsService uploadsService;
        private readonly UserManager<BaseUser> userManager;
        private readonly IEmailService emailService;
        private readonly IJWT jWT;
        public AuthService(
            MediaUploadsService images, 
            UserManager<BaseUser> manager, IJWT _jWT, 
            IEmailService _emailService
            )
        {
            uploadsService = images;
            userManager = manager;
            jWT = _jWT;
            emailService = _emailService;
        }
        public async Task<AuthDTO> InstructorRegisterAsync(InstructorRegisterDTO registerDTO)
        {
            if (registerDTO is null) 
                throw new ArgumentNullException(nameof(registerDTO), "Register DTO cannot be null");

            var validateErrors = await ValidateInstructorRegisterAsync(registerDTO);
            if (validateErrors is not null && validateErrors.Count > 0)
                return FailResult(string.Join(", ", validateErrors));

            // Convert the VideoPath (string) to an IFormFile before calling UploadVideo
            if (registerDTO.CvPath is null || registerDTO.VideoPath is null)
                return FailResult("الرجاء تحميل السيرة الذاتية وفيديو التعريف");

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
                return FailResult(string.Join(", ", validateErrors ?? new List<string>()));

            await userManager.AddToRoleAsync(user, UserRoles.Instructor.ToString());

            var replacements = new Dictionary<string, string>
            {
                { "FirstName", user.FirstName },
                { "LastName", user.LastName },
                { "Email", user.Email },
                { "PhoneNumber", user.PhoneNumber ?? string.Empty },
                { "Bio", user.Bio ?? string.Empty }
            };

            await emailService.SendEmailWithTemplateAsync(user.Email, "تم تسجيل حساب معلم جديد في منصة تحفيظ قران", "InstructorRegistrationConfirmation", replacements);

            return new AuthDTO
            {
                IsAuthenticated = true,
                Message = "تم استلام طلبكم وسيتم التواصل معكم قريبا"
            };
        }

        public async Task<AuthDTO> LoginAsync(LoginDTO loginDTO)
        {
            if (loginDTO == null)
                throw new ArgumentNullException(nameof(loginDTO), "Login DTO cannot be null");

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
            authDto.PhoneNumber = user.PhoneNumber ?? string.Empty;
            authDto.DateOfBirth = user.DateOfBirth ?? default;
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
                authDto.CV = instructor.CVPath ?? string.Empty;
                authDto.IntroVideo = instructor.IntroVideoPath ?? string.Empty;
                authDto.Bio = instructor.Bio ?? string.Empty;
                authDto.IsApproved = instructor.IsApproved;
            }
            authDto.Message = "تم تسجيل الدخول بنجاح";
            return authDto;
        }

        public async Task<AuthDTO> RefreshTokenAsync(string token)
        {
            var user = userManager.Users.FirstOrDefault(u => u.RefreshTokens.Any(rt => rt.Token == token));
            if (user == null)
                return FailResult("Invalid refresh token.");

            var refreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == token);
            if (refreshToken == null || !refreshToken.IsActive)
                return FailResult("Refresh token is expired or revoked.");

            // Revoke the old refresh token
            refreshToken.RevokedOn = DateTime.UtcNow;

            // Generate new tokens
            var newAccessToken = await jWT.GenerateAccessTokenAsync(user);
            var newRefreshToken = await jWT.GenerateRefreshTokenAsync();

            user.RefreshTokens.Add(newRefreshToken);
            await userManager.UpdateAsync(user);

            var authDto = new AuthDTO
            {
                IsAuthenticated = true,
                UserId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                DateOfBirth = user.DateOfBirth ?? default,
                Roles = (await userManager.GetRolesAsync(user)).ToList(),
                Token = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiration = newRefreshToken.ExpiresOn,
                Message = "Token refreshed successfully"
            };

            if (user is InstructorUser instructor)
            {
                authDto.CV = instructor.CVPath ?? string.Empty;
                authDto.IntroVideo = instructor.IntroVideoPath ?? string.Empty;
                authDto.Bio = instructor.Bio ?? string.Empty;
                authDto.IsApproved = instructor.IsApproved;
            }

            return authDto;
        }

        public async Task<AuthDTO> RegisterAsync(RegisterDTO registerDTO)
        {
            if (registerDTO is null)
                throw new ArgumentNullException(nameof(registerDTO), "Register DTO cannot be null");
            
            var validateErrors = await ValidateRegisterAsync(registerDTO);
            if (validateErrors is not null && validateErrors.Count > 0)
                return FailResult(string.Join(", ", validateErrors));

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
                return FailResult(string.Join(", ", validateErrors ?? new List<string>()));

            await userManager.AddToRoleAsync(user, UserRoles.Student.ToString());

            var replacements = new Dictionary<string, string>
            {
                { "FirstName", user.FirstName },
                { "LastName", user.LastName },
                { "Email", user.Email },
                { "PhoneNumber", user.PhoneNumber ?? string.Empty }
            };

            await emailService.SendEmailWithTemplateAsync(user.Email, "تم تسجيل حساب طالب جديد في منصة تحفيظ قران", "StudentRegistrationConfirmation", replacements);

            return new AuthDTO
            {
                IsAuthenticated = true,
                Message = "تم تسجيل حساب جديد بنجاح, يمكنك تسجيل دخول"
            };
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
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty");

            if (updatedUser is null)
                throw new ArgumentNullException(nameof(updatedUser), "Updated user data cannot be null");

            var user = await userManager.FindByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found");

            if (user.IsDeleted)
                throw new KeyNotFoundException("User has been deleted");

            if (!string.IsNullOrEmpty(updatedUser.ConcurrencyStamp))
                user.ConcurrencyStamp = updatedUser.ConcurrencyStamp;

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
            user.UpdatedAt = DateTime.UtcNow;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException("Failed to update user: " + string.Join(", ", result.Errors.Select(e => e.Description)));

            var replacements = new Dictionary<string, string>
            {
                { "FirstName", user.FirstName },
                { "LastName", user.LastName },
                { "Email", user.Email },
                { "PhoneNumber", user.PhoneNumber ?? string.Empty }
            };

            await emailService.SendEmailWithTemplateAsync(user.Email, "تم تحديث بياناتك في منصة تحفيظ قران بنجاح", "UpdateProfileConfirmation", replacements);

            return new AuthDTO
            {
                IsAuthenticated = true,
                Message = "تم تحديث بياناتك بنجاح", 
            };
        }

        public async Task<UserDTO> GetUserByIDAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found");

            if (user.IsDeleted)
                throw new KeyNotFoundException("User has been deleted");

            return new UserDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                ConcurrencyStamp = user.ConcurrencyStamp
            };
        }

        public async Task<InstructorUser> GetInstructorByIdAsync(string instructorId)
        {
            var instructor = await userManager.FindByIdAsync(instructorId)
                ?? throw new KeyNotFoundException("Instructor not found");

            if (instructor.IsDeleted)
                throw new KeyNotFoundException("Instructor has been deleted");

            return (InstructorUser)instructor;
        }
        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found");

            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;
            var result = await userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
