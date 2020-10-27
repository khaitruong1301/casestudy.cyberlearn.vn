using ApiBase.Repository.Models;
using ApiBase.Repository.Repository;
using ApiBase.Service.Constants;
using ApiBase.Service.Infrastructure;
using ApiBase.Service.ViewModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiBase.Service.Services.PriorityService
{
    public interface IPriorityService : IService<Priority, Priority>
    {
        Task<ResponseEntity> getAll(int? id);

    }
    public class PriorityService : ServiceBase<Priority, Priority>, IPriorityService
    {
        IPriorityRepository _priorityRepository;

        public PriorityService(IPriorityRepository proRe,
            IMapper mapper)
            : base(proRe, mapper)
        {
            _priorityRepository = proRe;
        }

        public async Task<ResponseEntity> getAll(int? id=0)
        {

            var lstResult = await _priorityRepository.GetMultiByConditionAsync("priorityId", id);

            if (lstResult.Count() == 0)
            {
                lstResult = await _priorityRepository.GetAllAsync();
            }

            return new ResponseEntity(StatusCodeConstants.OK, lstResult, MessageConstants.MESSAGE_SUCCESS_200);

        }
    }
}
