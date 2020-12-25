using ApiBase.Repository.Models;
using ApiBase.Repository.Repository;
using ApiBase.Service.Constants;
using ApiBase.Service.Infrastructure;
using ApiBase.Service.ViewModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApiBase.Service.Services.StatusService
{
    public interface IStatusService : IService<Status, Status>
    {

        Task<ResponseEntity> getAll(int taskId, string token);





    }
    public class StatusService : ServiceBase<Status, Status>, IStatusService
    {
        IStatusRepository _statusRepository;
        public StatusService(IStatusRepository status,
            IMapper mapper)
            : base(status, mapper)
        {
            _statusRepository = status;

        }

        public async Task<ResponseEntity> getAll(int taskId, string token)
        {
            var result = _statusRepository.GetAllAsync().Result;
            return new ResponseEntity(StatusCodeConstants.OK,result , MessageConstants.MESSAGE_SUCCESS_200);

        }
    }
}
