using AutoMapper;
using FTravel.Service.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.AccountBusinessModels;
using SchedulifySystem.Service.BusinessModels.EmailModels;
using SchedulifySystem.Service.BusinessModels.RoleAssignmentBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.Exceptions;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.UnitOfWork;
using SchedulifySystem.Service.Utils;
using SchedulifySystem.Service.Utils.Constants;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;
        private readonly IOtpService _otpService;

        public UserService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration, IMailService mailService, IOtpService otpService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mailService = mailService;
            _otpService = otpService;
        }
        #region confirm create account
        public async Task<BaseResponseModel> ConfirmCreateSchoolManagerAccount(int schoolManagerId, int schoolId, AccountStatus accountStatus)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var schoolManager = await _unitOfWork.UserRepo.GetByIdAsync(schoolManagerId) ?? throw new NotExistsException(ConstantResponse.SCHOOL_ACCOUNT_NOT_EXIST);
                    var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
                    if (accountStatus == AccountStatus.Pending || accountStatus == 0)
                    {
                        return new BaseResponseModel
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = ConstantResponse.CHANGE_ACCOUNT_STATUS
                        };
                    }
                    var isConfirm = schoolManager.IsConfirmSchoolManager;
                    if (isConfirm == true)
                    {
                        return new BaseResponseModel
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = ConstantResponse.SCHOOL_MANAGERR_ALREADY_CONFIRM
                        };
                    }
                    schoolManager.IsConfirmSchoolManager = true;
                    schoolManager.Status = (int)accountStatus;
                    if(schoolManager.Status == 1)
                    {
                        school.Status = 1;
                        school.UpdateDate = DateTime.UtcNow;
                    }
                    schoolManager.UpdateDate = DateTime.UtcNow;
                    _unitOfWork.UserRepo.Update(schoolManager);
                    await _unitOfWork.SaveChangesAsync();

                    var messageRequest = new EmailRequest
                    {
                        To = schoolManager.Email,
                        Subject = "Đăng ký tài khoản thành công",
                        Content = MailTemplate.ConfirmTemplate(school.Name)
                    };
                    await _mailService.SendEmailAsync(messageRequest);
                    await transaction.CommitAsync();
                    return new BaseResponseModel
                    {
                        Status = StatusCodes.Status200OK,
                        Message = ConstantResponse.SCHOOL_MANAGERR_CONFIRM
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
        #endregion

        #region sign up - create admin
        public async Task<BaseResponseModel> CreateAdminAccount(CreateAdmin createSchoolManagerModel)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var account = _mapper.Map<Account>(createSchoolManagerModel);
                    account.Password = AuthenticationUtils.HashPassword(createSchoolManagerModel.Password);
                    account.CreateDate = DateTime.UtcNow;
                    account.IsConfirmSchoolManager = false;
                    account.Status = (int)AccountStatus.Active;
                    await _unitOfWork.UserRepo.AddAsync(account);
                    await _unitOfWork.SaveChangesAsync();
                    var role = await _unitOfWork.RoleRepo.GetRoleByNameAsync(RoleEnum.Admin.ToString());
                    if (role == null)
                    {
                        Role newRole = new()
                        {
                            Name = RoleEnum.Admin.ToString()
                        };
                        await _unitOfWork.RoleRepo.AddAsync(newRole);
                        await _unitOfWork.SaveChangesAsync();
                        role = newRole;
                    }

                    var accountRoleModel = new RoleAssigntmentAddModel
                    {
                        AccountId = account.Id,
                        RoleId = role.Id
                    };
                    var accountRoleEntyties = _mapper.Map<RoleAssignment>(accountRoleModel);
                    await _unitOfWork.RoleAssignmentRepo.AddAsync(accountRoleEntyties);
                    await _unitOfWork.SaveChangesAsync();

                    var result = _mapper.Map<AccountViewModel>(account);
                    await transaction.CommitAsync();
                    return new BaseResponseModel
                    {
                        Status = StatusCodes.Status201Created,
                        Message = "Create admin successful!",
                        Result = result
                    };
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }

            }
        }
        #endregion

        #region sign up - create school manager account
        public async Task<BaseResponseModel> CreateSchoolManagerAccount(CreateSchoolManagerModel createSchoolManagerModel)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var existUser = await _unitOfWork.UserRepo.GetAccountByEmail(createSchoolManagerModel.Email);
                    if (existUser != null)
                    {
                        throw new AccountAlreadyExistsException();
                    }
                    var school = await _unitOfWork.SchoolRepo.GetByIdAsync(createSchoolManagerModel.SchoolId);
                    if (school == null)
                    {
                        throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
                    }

                    var schoolsInAccount = await _unitOfWork.UserRepo.GetAsync(filter: t => t.SchoolId == createSchoolManagerModel.SchoolId);
                    if (schoolsInAccount != null && schoolsInAccount.Any())
                    {
                        return new BaseResponseModel
                        {
                            Status = StatusCodes.Status409Conflict,
                            Message = ConstantResponse.SCHOOL_ALREADY_ASSIGNED,
                        };
                    }

                    var account = _mapper.Map<Account>(createSchoolManagerModel);
                    account.Password = AuthenticationUtils.HashPassword(createSchoolManagerModel.Password);
                    account.CreateDate = DateTime.UtcNow;
                    account.IsConfirmSchoolManager = false;
                    await _unitOfWork.UserRepo.AddAsync(account);
                    await _unitOfWork.SaveChangesAsync();
                    var role = await _unitOfWork.RoleRepo.GetRoleByNameAsync(RoleEnum.SchoolManager.ToString());
                    if (role == null)
                    {
                        Role newRole = new()
                        {
                            Name = RoleEnum.SchoolManager.ToString()
                        };
                        await _unitOfWork.RoleRepo.AddAsync(newRole);
                        await _unitOfWork.SaveChangesAsync();
                        role = newRole;
                    }

                    var accountRoleModel = new RoleAssigntmentAddModel
                    {
                        AccountId = account.Id,
                        RoleId = role.Id
                    };
                    var accountRoleEntyties = _mapper.Map<RoleAssignment>(accountRoleModel);
                    await _unitOfWork.RoleAssignmentRepo.AddAsync(accountRoleEntyties);
                    await _unitOfWork.SaveChangesAsync();

                    account.School = school;
                    var result = _mapper.Map<AccountViewModel>(account);
                    await transaction.CommitAsync();
                    return new BaseResponseModel
                    {
                        Status = StatusCodes.Status201Created,
                        Message = ConstantResponse.SCHOOL_MANAGER_CREAT_SUCCESSFUL,
                        Result = result
                    };
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }

            }
        }
        #endregion

        #region sign in
        public async Task<AuthenticationResponseModel> SignInAccountAsync(SignInModel signInModel)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var existUser = await _unitOfWork.UserRepo.GetAccountByEmail(signInModel.Email);
                    if (existUser == null)
                    {
                        return new AuthenticationResponseModel
                        {
                            Status = StatusCodes.Status401Unauthorized,
                            Message = ConstantResponse.ACCOUNT_NOT_EXIST
                        };
                    }
                    var verifyUser = AuthenticationUtils.VerifyPassword(signInModel.Password, existUser.Password);
                    if (verifyUser)
                    {
                        if (existUser.Status == (int)AccountStatus.Inactive
                            || existUser.Status == (int)AccountStatus.Pending
                            || existUser.IsDeleted == true)
                        {
                            return new AuthenticationResponseModel
                            {
                                Status = StatusCodes.Status401Unauthorized,
                                Message = ConstantResponse.ACCOUNT_CAN_NOT_ACCESS
                            };
                        }

                        await transaction.CommitAsync();

                        return await GenerateAuthenticationResponse(existUser);
                    }
                    return new AuthenticationResponseModel
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = ConstantResponse.PASSWORD_INCORRECT
                    };
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            };
        }

        #endregion

        #region refresh token validate
        public async Task<AuthenticationResponseModel> RefreshToken(string refreshToken)
        {
            var handler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;

            try
            {
                var principal = handler.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"])),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JWT:ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JWT:ValidAudience"],
                    ValidateLifetime = true, // validate time 
                    ClockSkew = TimeSpan.Zero
                }, out validatedToken);

                var emailClaim = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
                if (emailClaim == null)
                {
                    return new AuthenticationResponseModel
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = ConstantResponse.INVALID_REFRESH
                    };
                }

                var existUser = await _unitOfWork.UserRepo.GetAccountByEmail(emailClaim.Value);
                if (existUser != null)
                {
                    var newAccessToken = GenerateJWTToken.CreateAccessToken(await GetAuthClaims(existUser), _configuration, DateTime.UtcNow);
                    var newRefreshToken = GenerateJWTToken.CreateRefreshToken(GetAuthClaimsRefresh(existUser), _configuration, DateTime.UtcNow);

                    return new AuthenticationResponseModel
                    {
                        Status = StatusCodes.Status200OK,
                        Message = ConstantResponse.REFRESH_TOKEN_SUCCESS,
                        JwtToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                        Expired = TimeZoneInfo.ConvertTimeFromUtc(newAccessToken.ValidTo, TimeZoneInfo.Local),
                        JwtRefreshToken = new JwtSecurityTokenHandler().WriteToken(newRefreshToken),
                    };
                }

                return new AuthenticationResponseModel
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = ConstantResponse.ACCOUNT_NOT_EXIST_AUTH
                };
            }
            catch
            {
                return new AuthenticationResponseModel
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = ConstantResponse.REFRESH_TOKEN_INVALID
                };
            }
        }
        #endregion

        #region jwt service
        private async Task<List<Claim>> GetAuthClaims(Account user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("accountId", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var userRolesAssignments = await _unitOfWork.RoleAssignmentRepo.GetRolesByAccountIdAsync(user.Id);

            var roleIds = userRolesAssignments.Select(assignment => assignment.RoleId).ToList();

            var userRoles = await _unitOfWork.RoleRepo.GetRolesByIdsAsync(roleIds);

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim("role", role.Name));
            }

            return authClaims;
        }


        private List<Claim> GetAuthClaimsRefresh(Account user)
        {
            var authClaims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Email, user.Email),
            };

            return authClaims;
        }

        private async Task<AuthenticationResponseModel> GenerateAuthenticationResponse(Account account)
        {
            var authClaims = await GetAuthClaims(account);
            var authClaimsRefresh = GetAuthClaimsRefresh(account);
            var token = GenerateJWTToken.CreateAccessToken(authClaims, _configuration, DateTime.UtcNow);
            var refreshToken = GenerateJWTToken.CreateRefreshToken(authClaimsRefresh, _configuration, DateTime.UtcNow);

            return new AuthenticationResponseModel
            {
                Status = StatusCodes.Status200OK,
                Message = "Login successfully!",
                JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
                Expired = TimeZoneInfo.ConvertTimeFromUtc(token.ValidTo, TimeZoneInfo.Local),
                JwtRefreshToken = new JwtSecurityTokenHandler().WriteToken(refreshToken).ToString(),
            };
        }

        #endregion

        #region request reset password
        public async Task<BaseResponseModel> RequestResetPassword(string gmail)
        {
            var existUser = await _unitOfWork.UserRepo.GetAccountByEmail(gmail) ?? throw new NotExistsException(ConstantResponse.ACCOUNT_NOT_EXIST);
            if (existUser.Status != (int)AccountStatus.Active)
            {
                return new BaseResponseModel() 
                { 
                    Status = StatusCodes.Status400BadRequest, 
                    Message = ConstantResponse.ACCOUNT_CAN_NOT_ACCESS
                };
            }

            var sendOtp = await _otpService.SendOTPResetPassword(existUser.Id ,gmail);
            if(sendOtp != false)
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status200OK,
                    Message = ConstantResponse.REQUEST_RESET_PASSWORD_SUCCESSFUL
                };
            }
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status400BadRequest,
                Message = ConstantResponse.REQUEST_RESET_PASSWORD_FAILED
            };
        }
        #endregion

        #region Confirm reset password
        public async Task<BaseResponseModel> ConfirmResetPassword(string gmail, int code)
        {
            var existUser = await _unitOfWork.UserRepo.GetAccountByEmail(gmail) ?? throw new NotExistsException(ConstantResponse.ACCOUNT_NOT_EXIST);
            if (existUser.Status != (int)AccountStatus.Active)
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.ACCOUNT_CAN_NOT_ACCESS
                };
            }

            var sendOtp = await _otpService.ConfirmResetPassword(existUser.Id, code);
            if (sendOtp != false)
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status200OK,
                    Message = ConstantResponse.CONFIRM_RESET_PASSWORD_SUCCESSFUL
                };
            }
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status400BadRequest,
                Message = ConstantResponse.OTP_NOT_VALID
            };
        }
        #endregion

        #region excute reset password
        public async Task<BaseResponseModel> ExcuteResetPassword(ResetPasswordModel resetPasswordModel)
        {
            var existedUser = await _unitOfWork.UserRepo.GetAccountByEmail(resetPasswordModel.Email) ?? throw new NotExistsException(ConstantResponse.ACCOUNT_NOT_EXIST);
            existedUser.Password = AuthenticationUtils.HashPassword(resetPasswordModel.Password);
            existedUser.UpdateDate = DateTime.UtcNow;
            _unitOfWork.UserRepo.Update(existedUser);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.RESET_PASSWORD_SUCCESS
            };
        }
        #endregion
    }
}
