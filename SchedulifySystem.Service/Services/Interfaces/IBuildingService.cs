using SchedulifySystem.Service.BusinessModels.BuildingBusinessModels;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface IBuildingService
    {
        Task<BaseResponseModel> GetBuildings(int schoolId, bool? includeRoom = false, int pageIndex = 1, int pageSize = 20);
        Task<BaseResponseModel> GetBuildingById(int id);
        Task<BaseResponseModel> AddBuildings(int schoolId, List<AddBuildingModel> models);
        Task<BaseResponseModel> CheckValidDataAddBuilding(int schoolId, List<AddBuildingModel> models);
        Task<BaseResponseModel> UpdateBuildings(int buildingId, UpdateBuildingModel model);
        Task<BaseResponseModel> DeleteBuildings(int buildingId);

    }
}
