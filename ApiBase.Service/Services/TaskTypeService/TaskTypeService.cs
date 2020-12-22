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
using System.Linq;

namespace ApiBase.Service.Services.TaskTypeService
{
    public interface ITaskTypeService : IService<TaskType, TaskType>
    {
        Task<ResponseEntity> getAllTaskType();
    }

    public class TaskTypeService : ServiceBase<TaskType, TaskType>,ITaskTypeService
    {

        ITaskTypeRepository _taskTypeRepository;
        public TaskTypeService(ITaskTypeRepository taskPre,
            IMapper mapper) : base(taskPre, mapper)
        {
            _taskTypeRepository = taskPre;
        }

        public async Task<ResponseEntity> getAllTaskType()
        {
            var lstTaskType = _taskTypeRepository.GetAllAsync().Result.Select(n => new  {   n.id,n.taskType});

            return new ResponseEntity(StatusCodeConstants.OK, lstTaskType, MessageConstants.MESSAGE_SUCCESS_200);
        }
    }


}
