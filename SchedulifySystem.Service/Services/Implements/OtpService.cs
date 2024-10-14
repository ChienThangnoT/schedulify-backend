using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.EmailModels;
using SchedulifySystem.Service.Exceptions;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.UnitOfWork;
using SchedulifySystem.Service.Utils;
using SchedulifySystem.Service.Utils.Constants;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Implements
{
    public class OtpService : IOtpService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;

        public OtpService(IUnitOfWork unitOfWork, IMapper mapper, IMailService mailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mailService = mailService;
        }

        #region sent otp reset password 
        public async Task<bool> SendOTPResetPassword(int accountId, string email)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    OTP otp = new()
                    {
                        AccountId = accountId,
                        Code = GenerateNumberUtils.GenerateDigitNumber(),
                        ExpiredDate = DateTime.UtcNow.AddMinutes(5),
                        CreateDate = DateTime.UtcNow,
                        IsDeleted = false,
                        isUsed = false
                    };
                    await _unitOfWork.OTPRepo.AddAsync(otp);
                    await _unitOfWork.SaveChangesAsync();

                    EmailRequest emailRequest = new EmailRequest()
                    {
                        To = email,
                        Subject = "Yêu cầu cấp lại mật khẩu",
                        Content = MailTemplate.ResetPasswordTemplate(email, otp.Code)
                    };
                    await _mailService.SendEmailAsync(emailRequest);

                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }
        #endregion

        #region confirm reset password
        public async Task<bool> ConfirmResetPassword(int accountId, int code)
        {
            var otp = await _unitOfWork.OTPRepo.GetOTPByCodeAsync(code) ?? throw new NotExistsException(ConstantRespones.OTP_NOT_VALID);
            DateTime otpExpiryTimeInUtcPlus7 = otp.ExpiredDate.AddHours(7);
            if (otp.AccountId == accountId && otpExpiryTimeInUtcPlus7 > DateTime.UtcNow.AddHours(7) && otp.isUsed == false)
            {
                otp.isUsed = true;
                _unitOfWork.OTPRepo.Update(otp);
                return true;
            }
            return false;
        }
        #endregion
    }
}
