using ApiBase.Repository.Models;
using ApiBase.Repository.Repository;
using ApiBase.Service.Constants;
using ApiBase.Service.Infrastructure;
using ApiBase.Service.Services.UserService;
using ApiBase.Service.Utilities;
using ApiBase.Service.ViewModels;
using ApiBase.Service.ViewModels.ProjectViewModel;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ApiBase.Service.Services.PriorityService
{
    public interface IProjectService : IService<Project, Project>
    {

        Task<ResponseEntity> createProject(ProjectInsert model,string token="");
        Task<ResponseEntity> getProjectById(int? idProject);
        Task<ResponseEntity> getAllProject();


    }
    public class ProjectService : ServiceBase<Project, Project>, IProjectService
    {
        IProjectRepository _projectRepository;
        IProjectCategoryRepository _projectCategoryRepository;
        IStatusRepository _statusRepository;
        ITaskRepository _taskRepository;
        IPriorityRepository _priorityRepository;
        ITask_UserRepository _taskUserRepository;
        IUserJiraRepository _userJira; 
        IUserService _userService;

        ICommentRepository _userComment;

        public ProjectService(IProjectRepository proRe, IProjectCategoryRepository proCa, IStatusRepository status,ITaskRepository taskRe,IPriorityRepository pri,ITask_UserRepository taskUSer,IUserJiraRepository us,ICommentRepository cmt, IUserService usService,
            IMapper mapper)
            : base(proRe, mapper)
        {
            _projectRepository = proRe;
            _projectCategoryRepository = proCa;
            _statusRepository = status;
            _taskRepository = taskRe;
            _priorityRepository = pri;
            _taskUserRepository = taskUSer;
            _userJira = us;
            _userComment = cmt;
            _userService = usService;

        }

        public async Task<ResponseEntity> createProject(ProjectInsert model,string token="")
        {

            

            string alias = FuncUtilities.BestLower(model.projectName.Trim());

            var project = await _projectRepository.GetSingleByConditionAsync("alias", alias);


            if(project != null )
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, "Project name already exists", MessageConstants.MESSAGE_ERROR_500);

            }
            
            Project newProject = new Project();
            newProject.alias = alias;
            newProject.categoryId = model.categoryId;
            newProject.deleted = false;
            newProject.description = model.description;
            newProject.projectName = model.projectName;
            if(token != "")
            {
                var user = _userJira.GetSingleByConditionAsync("id", _userService.getUserByToken(token).Id);
                newProject.creator = user.Id;
            }else
            {
                newProject.creator = 1;//set mặc định khai admin
            }
            newProject = await _projectRepository.InsertAsync(newProject);
            
            
            return new ResponseEntity(StatusCodeConstants.OK, newProject, MessageConstants.MESSAGE_SUCCESS_200);


        }

        public async Task<ResponseEntity> getProjectById(int? idProject)
        {

            var pro = await _projectRepository.GetSingleByConditionAsync("id",idProject);

            if(pro == null)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "Project is not found", MessageConstants.MESSAGE_ERROR_404);

            }

            ProjectCategory projectCategory = await _projectCategoryRepository.GetSingleByConditionAsync("id", pro.categoryId);


            ProjectDetail projectDetail = new ProjectDetail();

            projectDetail.alias = pro.alias;
            projectDetail.projectName = pro.projectName;
            projectDetail.projectCategory = new ProjectCategoryDetail() { id=projectCategory.id,name=projectCategory.projectCategoryName};
            projectDetail.description = pro.description;
            projectDetail.id = pro.id;
            projectDetail.projectName = pro.projectName;
            CreatorModel creator = new CreatorModel();
            if(pro.id != null)
            {
                creator.id = pro.creator;
                creator.name = _userJira.GetSingleByIdAsync(pro.creator).Result.name;
            }

            projectDetail.creator = creator;

            //List<ProjectDetail> lstResult = new List<ProjectDetail>();
            var lstStatus = await _statusRepository.GetAllAsync();

            //Lấy list priority
            IEnumerable<Priority> lstPriority = await _priorityRepository.GetAllAsync();

            //Lấy list task 
            var lstTask = await _taskRepository.GetAllAsync();
            foreach (var status in lstStatus)
            {
                var statusTask = new StatusTask { statusId = status.statusId, statusName = status.statusName, alias = status.alias };

                var task = lstTask.Where(n =>  n.projectId == projectDetail.id && n.statusId == status.statusId).Select(n=> new TaskDetail {taskId= n.taskId,taskName=n.taskName,alias=n.alias,description=n.description,statusId=n.statusId,priorityTask = getTaskPriority(n.projectId,lstPriority),originalEstimate = n.originalEstimate,timeTracking = n.timeTracking,assigness=getListUserAsign(n.taskId).ToList(),taskTypeDetail = getTaskType(n.typeId),lstComment= getListComment(n.taskId).ToList()});
                
                statusTask.lstTaskDeTail.AddRange(task.ToList());

                projectDetail.lstTask.Add(statusTask);
            }


            return new ResponseEntity(StatusCodeConstants.OK, projectDetail, MessageConstants.MESSAGE_SUCCESS_200);
        }


        public  IEnumerable<CommentTask> getListComment(int taskId)
        {
            List<KeyValuePair<string, dynamic>> columns = new List<KeyValuePair<string, dynamic>>();
            columns.Add(new KeyValuePair<string, dynamic>("taskId", taskId));
            //columns.Add(new KeyValuePair<string, dynamic>("userId", userId));

            IEnumerable<CommentTask> lstCmt =  _userComment.GetMultiByListConditionAndAsync(columns).Result.Select(n =>
            {
                var user = getUserAsync(n.userId);
                CommentTask res = new CommentTask() { idUser = n.userId, avatar = user.avatar, name = user.name, commentContent = n.contentComment };
                return res;
            });
            

            return lstCmt;
        }



        public  TaskTypeDetail getTaskType (int taskId)
        {
            var result =  _taskRepository.GetSingleByConditionAsync("taskId", taskId).Result;

            TaskTypeDetail res = new TaskTypeDetail();
            res.id = result.taskId;
            res.taskType = result.taskName;
            return res;
        } 

        public  IEnumerable<userAssign> getListUserAsign(int taskId)
        {
            var userTask =  _taskUserRepository.GetMultiByConditionAsync("taskId", taskId);

            IEnumerable<userAssign> uAssigns = userTask.Result.Select(n => { 
                var user = getUserAsync(n.id);
                return new userAssign() { id = n.id, name = user.name, alias = user.alias, avatar = user.avatar };
            });

            return uAssigns;

        }

        public UserJira getUserAsync(int id)
        {
            var userJira =  _userJira.GetSingleByConditionAsync("id",id).Result;
            return userJira;
        }

        public TaskPriority getTaskPriority (int id, IEnumerable<Priority> lst)
        {
            Priority pri = lst.Single(n => n.priorityId == id);

            return new TaskPriority() { priorityId = pri.priorityId, priority = pri.priority };
        }

        public async Task<ResponseEntity> getAllProject()
        {
            var lstProject = await _projectRepository.GetAllAsync();
            var listResult = new List<ProjectViewModelResult>();
            foreach(var n in lstProject)
            {
                Creator creator = new Creator();
                if(n.creator != null )
                {
                    creator.id = n.creator;
                    creator.name = _userJira.GetSingleByIdAsync(creator.id).Result.name;
                }
                var result = new ProjectViewModelResult { id = n.id, projectName = n.projectName, alias = n.alias, deleted = n.deleted, description = n.description, categoryName = _projectCategoryRepository.GetSingleByIdAsync(n.categoryId).Result.projectCategoryName, categoryId = n.categoryId, creator = creator  };
                listResult.Add(result);
            }

            return new ResponseEntity(StatusCodeConstants.OK, listResult, MessageConstants.MESSAGE_SUCCESS_200);
        }
    }

}
