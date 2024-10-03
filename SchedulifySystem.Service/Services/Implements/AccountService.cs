using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.AccountBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.Exceptions;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.UnitOfWork;
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

        public async Task<BaseResponseModel> GetListAccount(AccountStatus? accountStatus, int pageIndex, int pageSize)
        {

            Expression<Func<Account, bool>> filter;

            if (accountStatus == null)
            {
                filter = null;
            }
            else
            {
                filter = u => u.Status == (int)accountStatus.Value;
            }

            var accounts = await _unitOfWork.UserRepo.ToPaginationIncludeAsync(
            pageIndex, pageSize,
            query => query.Include(s => s.School),
            filter: filter
            );
            if (accounts.Items.Count == 0)
            {
                throw new NotExistsException("Account list not exist");
            }
            var result = _mapper.Map<Pagination<AccountViewModel>>(accounts);
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Result = result
            };
        }
    }
}
