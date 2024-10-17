using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.AccountBusinessModels;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.Exceptions;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.UnitOfWork;
using SchedulifySystem.Service.Utils;
using SchedulifySystem.Service.Utils.Constants;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region get account detail
        public async Task<BaseResponseModel> GetAccountInformation(int accountId)
        {
            var account = await _unitOfWork.UserRepo.GetByIdAsync(accountId) ?? throw new NotExistsException(ConstantResponse.ACCOUNT_NOT_EXIST);
            var school = account.SchoolId.HasValue
                ? await _unitOfWork.SchoolRepo.GetByIdAsync(account.SchoolId.Value)
                : null;
            account.School = school;
            var result = _mapper.Map<AccountDetailModel>(account);
            var role = await _unitOfWork.RoleAssignmentRepo.GetAsync(
                filter: sc => !sc.IsDeleted && sc.AccountId == account.Id, includeProperties: "Role");

            result.AccountRole = role.Select(x => x.Role.Name).ToList();
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_ACCOUNT_DETAIL_SUCCESS,
                Result = result
            };
        }
        #endregion

        #region get account list
        public async Task<BaseResponseModel> GetListAccount(AccountStatus? accountStatus, int pageIndex, int pageSize)
        {
            Expression<Func<Account, bool>>? filter = accountStatus switch
            {
                0 => u => u.RoleAssignments.Any(ra => ra.RoleId == 2),
                _ => u => u.Status == (int)accountStatus.Value && u.RoleAssignments.Any(ra => ra.RoleId == 2)
            };

            var accounts = await _unitOfWork.UserRepo.ToPaginationIncludeAsync(
                pageIndex, pageSize,
                query => query.Include(s => s.School)
                              .Include(u => u.RoleAssignments),
                filter: filter
            );

            if (accounts.Items.Count == 0)
            {
                throw new NotExistsException(ConstantResponse.ACCOUNT_LIST_NOT_EXIST);
            }

            var result = _mapper.Map<Pagination<AccountViewModel>>(accounts);
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Result = result
            };
        }
        #endregion

        #region update account info
        public async Task<BaseResponseModel> UpdateAccountInformation(int accountId, UpdateAccountModel updateAccountModel)
        {
            var account = await _unitOfWork.UserRepo.GetByIdAsync(accountId) ?? throw new NotExistsException(ConstantResponse.ACCOUNT_NOT_EXIST);

            var accountUpdate = _mapper.Map(updateAccountModel, account);
            accountUpdate.UpdateDate = DateTime.UtcNow;
            _unitOfWork.UserRepo.Update(accountUpdate);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.UPDATE_ACCOUNT_DETAIL_SUCCESS
            };
        }
        #endregion

        #region change password
        public async Task<BaseResponseModel> ChangeAccountPassword(int accountId, ChangePasswordModel changePasswordModel)
        {
            var account = await _unitOfWork.UserRepo.GetByIdAsync(accountId) ?? throw new NotExistsException(ConstantResponse.ACCOUNT_NOT_EXIST);
            var verifyUser = AuthenticationUtils.VerifyPassword(changePasswordModel.OldPasswoorrdPassword, account.Password);
            if (verifyUser)
            {
                account.Password = AuthenticationUtils.HashPassword(changePasswordModel.NewPassword);
                account.UpdateDate = DateTime.UtcNow;
                _unitOfWork.UserRepo.Update(account);
                await _unitOfWork.SaveChangesAsync();
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status200OK,
                    Message = ConstantResponse.CHANGE_PASSWORD_SUCCESSFUL
                };
            }
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status400BadRequest,
                Message = ConstantResponse.CHANGE_PASSWORD_FAILED
            };
        }
        #endregion
    }
}
