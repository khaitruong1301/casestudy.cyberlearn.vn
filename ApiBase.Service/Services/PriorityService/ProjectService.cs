﻿using ApiBase.Repository.Models;
using ApiBase.Repository.Repository;
using ApiBase.Service.Constants;
using ApiBase.Service.Infrastructure;
using ApiBase.Service.Services.Project_UserService;
using ApiBase.Service.Services.UserService;
using ApiBase.Service.Utilities;
using ApiBase.Service.ViewModels;
using ApiBase.Service.ViewModels.ProjectViewModel;
using ApiBase.Service.ViewModels.Task;
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
        Task<ResponseEntity> updateProject(int? idProject,ProjectUpdate projectUpdate,string token);
        Task<ResponseEntity> addUserProject(Project_User project,string token="");

        Task<ResponseEntity> getAllProject();
        Task<ResponseEntity> updateStatusTask(UpdateStatusVM statusTask,string token );
        Task<ResponseEntity> updatePiority(UpdatePiority model,string token);
        Task<ResponseEntity> updateDescription(UpdateDescription model,string token);
        Task<ResponseEntity> updateTimeTracking(TimeTrackingUpdate model,string token);
        Task<ResponseEntity> updateEstimate(updateEstimate model,string token);
        Task<ResponseEntity> addTaskUser(TaskUser model,string token);
        Task<ResponseEntity> removeUserFromTask(TaskUser model,string token);
        Task<ResponseEntity> removeUSerFromProject(Project_User model,string token);
        Task<ResponseEntity> createTask(taskInsert model,string token);
        Task<ResponseEntity> removeTask(int taskId,string token);



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
        IProject_UserReponsitory _projectUserRepository;

        ICommentRepository _userComment;

        public ProjectService(IProjectRepository proRe, IProjectCategoryRepository proCa, IStatusRepository status,ITaskRepository taskRe,IPriorityRepository pri,ITask_UserRepository taskUSer,IUserJiraRepository us,ICommentRepository cmt, IUserService usService, IProject_UserReponsitory project_userService,
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
            _projectUserRepository = project_userService;

        }

        public async Task<ResponseEntity> createProject(ProjectInsert model, string token = "")
        {



            string alias = FuncUtilities.BestLower(model.projectName.Trim());

            var project = await _projectRepository.GetSingleByConditionAsync("alias", alias);


            if (project != null)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, "Project name already exists", MessageConstants.MESSAGE_ERROR_500);

            }
            var projectCate = _projectCategoryRepository.GetSingleByConditionAsync("categoryId", model.categoryId).Result;
            if (projectCate == null)
            {
                model.categoryId = 1;
            }
            Project newProject = new Project();
            newProject.alias = alias;
            newProject.categoryId = model.categoryId;
            newProject.deleted = false;
            newProject.description = model.description;
            newProject.projectName = model.projectName;
            if (token != "")
            {
                var user = _userJira.GetSingleByConditionAsync("id", _userService.getUserByToken(token).Id);
                newProject.creator = user.Id;
            }
            else
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
                var result = new ProjectViewModelResult { id = n.id, projectName = n.projectName, alias = n.alias, deleted = n.deleted, description = n.description, categoryName = _projectCategoryRepository.GetSingleByIdAsync(n.categoryId).Result.projectCategoryName, categoryId = n.categoryId, creator = creator , members = getListMember(n.id) };
                listResult.Add(result);
            }

            return new ResponseEntity(StatusCodeConstants.OK, listResult, MessageConstants.MESSAGE_SUCCESS_200);
        }
        public List<Member> getListMember (int projectId)
        {
            List<Member> lst = new List<Member>();
            var userProject = _projectUserRepository.GetMultiByConditionAsync("projectId", projectId).Result;
            foreach(var project in userProject)
            {
                Member mem = new Member();
                mem.userId = project.userId;
                dynamic id = mem.userId;
                mem.name = _userJira.GetSingleByIdAsync(id).Result.name;
                lst.Add(mem);
            }
            return lst;
        }

        public async Task<ResponseEntity> updateProject(int? idProject=0, ProjectUpdate projectUpdate=null,string token="")
        {
          

            Project project = _projectRepository.GetSingleByIdAsync(idProject).Result;
           
            if(project == null)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "Project is not found", MessageConstants.MESSAGE_ERROR_404);

            }
            project.alias = FuncUtilities.BestLower(projectUpdate.projectName);
            //project.creator = project.creator;
            project.creator = _userService.getUserByToken(token).Result.id;
            project.description = projectUpdate.description;
            project.projectName = projectUpdate.projectName;

            var result = _projectRepository.UpdateAsync(idProject, project).Result;

            

            return new ResponseEntity(StatusCodeConstants.OK, result, MessageConstants.MESSAGE_SUCCESS_200);

        }


        public async Task<ResponseEntity> addUserProject(Project_User project, string token = "")
        {
            UserJira user = _userService.getUserByToken(token).Result;
            Project pro = _projectRepository.GetSingleByConditionAsync("id",project.projectId).Result;
           
            if(pro == null)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "Project is not found!", MessageConstants.MESSAGE_ERROR_404);

            }
            if(pro.creator != user.id)
            {
                return new ResponseEntity(StatusCodeConstants.FORBIDDEN, "User is unthorization!", MessageConstants.MESSAGE_ERROR_403);

            }
            List<KeyValuePair<string, dynamic>> columns = new List<KeyValuePair<string, dynamic>>();
            columns.Add(new KeyValuePair<string, dynamic>("projectId", project.projectId));
            columns.Add(new KeyValuePair<string, dynamic>("userId", project.userId));


            IEnumerable<Project_User> lstProjectUser = _projectUserRepository.GetMultiByListConditionAndAsync(columns).Result;
            if (lstProjectUser.Count() > 0)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, "User already exists in the project!", MessageConstants.MESSAGE_ERROR_500);
            }
            Project_User model = new Project_User() ;
            model.userId = project.userId;
            model.projectId = project.projectId;
            model.deleted = false;
            model.alias = project.userId + "_" + project.projectId;
            await _projectUserRepository.InsertAsync(model);
            return new ResponseEntity(StatusCodeConstants.OK, "has added the user to the project !", MessageConstants.MESSAGE_SUCCESS_200);

        }

        public async Task<ResponseEntity> updateStatusTask(UpdateStatusVM statusTask, string token)
        {
            UserJira user = _userService.getUserByToken(token).Result;
            var task = _taskRepository.GetSingleByConditionAsync("id", statusTask.taskId).Result;


            List<KeyValuePair<string, dynamic>> columns = new List<KeyValuePair<string, dynamic>>();
            columns.Add(new KeyValuePair<string, dynamic>("projectId", task.projectId));
            columns.Add(new KeyValuePair<string, dynamic>("userId", user.id));

            var projectUser = _projectUserRepository.GetMultiByListConditionAndAsync(columns).Result;
            if(projectUser.Count() > 0)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "task is not found!", MessageConstants.MESSAGE_ERROR_404);

            }



            if (task == null)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "task is not found!", MessageConstants.MESSAGE_ERROR_404);

            }

            task.statusId = statusTask.statusId;

            await _taskRepository.UpdateAsync(task.taskId, task);

            return new ResponseEntity(StatusCodeConstants.OK, "Update task successfully!", MessageConstants.MESSAGE_SUCCESS_200);
        }

        public async Task<ResponseEntity> updatePiority(UpdatePiority model,string token="")
        {
            var task = _taskRepository.GetSingleByConditionAsync("taskId", model.taskId).Result;

            if (task == null)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "task is not found!", MessageConstants.MESSAGE_ERROR_404);

            }

            UserJira user = _userService.getUserByToken(token).Result;
            Project pro = _projectRepository.GetSingleByConditionAsync("id", task.projectId).Result;

            if (pro == null)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "Project is not found!", MessageConstants.MESSAGE_ERROR_404);

            }
            if (pro.creator != user.id)
            {
                return new ResponseEntity(StatusCodeConstants.FORBIDDEN, "User is unthorization!", MessageConstants.MESSAGE_ERROR_403);

            }

            task.priorityId = model.priorityId;


            await _taskRepository.UpdateAsync(model.taskId, task);

            return new ResponseEntity(StatusCodeConstants.OK, "Update task successfully!", MessageConstants.UPDATE_SUCCESS);
        }

        public async Task<ResponseEntity> updateDescription(UpdateDescription model, string token)
        {
            var task = _taskRepository.GetSingleByConditionAsync("taskId", model.taskId).Result;

            if (task == null)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "task is not found!", MessageConstants.MESSAGE_ERROR_404);

            }

            UserJira user = _userService.getUserByToken(token).Result;
            Project pro = _projectRepository.GetSingleByConditionAsync("id", task.projectId).Result;

            if (pro == null)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "Project is not found!", MessageConstants.MESSAGE_ERROR_404);

            }
            if (pro.creator != user.id)
            {
                return new ResponseEntity(StatusCodeConstants.FORBIDDEN, "User is unthorization!", MessageConstants.MESSAGE_ERROR_403);

            }

            task.description = model.description;


            await _taskRepository.UpdateAsync(model.taskId, task);

            return new ResponseEntity(StatusCodeConstants.OK, "Update task successfully!", MessageConstants.UPDATE_SUCCESS);
        }

        public async Task<ResponseEntity> updateTimeTracking(TimeTrackingUpdate model, string token)
        {
            var task = _taskRepository.GetSingleByConditionAsync("taskId", model.taskId).Result;

            if (task == null)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "task is not found!", MessageConstants.MESSAGE_ERROR_404);

            }

            UserJira user = _userService.getUserByToken(token).Result;
            Project pro = _projectRepository.GetSingleByConditionAsync("id", task.projectId).Result;

            if (pro == null)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "Project is not found!", MessageConstants.MESSAGE_ERROR_404);

            }
            if (pro.creator != user.id)
            {
                return new ResponseEntity(StatusCodeConstants.FORBIDDEN, "User is unthorization!", MessageConstants.MESSAGE_ERROR_403);

            }

            task.timeTracking = model.timeTracking;
            task.timeTrackingMax = model.timeTrackingMax;


            await _taskRepository.UpdateAsync(model.taskId, task);

            return new ResponseEntity(StatusCodeConstants.OK, "Update task successfully!", MessageConstants.UPDATE_SUCCESS);
        }

        public async Task<ResponseEntity> updateEstimate(updateEstimate model, string token)
        {

            var task = _taskRepository.GetSingleByConditionAsync("taskId", model.taskId).Result;

            if (task == null)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "task is not found!", MessageConstants.MESSAGE_ERROR_404);

            }

            UserJira user = _userService.getUserByToken(token).Result;
            Project pro = _projectRepository.GetSingleByConditionAsync("id", task.projectId).Result;

            if (pro == null)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "Project is not found!", MessageConstants.MESSAGE_ERROR_404);

            }
            if (pro.creator != user.id)
            {
                return new ResponseEntity(StatusCodeConstants.FORBIDDEN, "User is unthorization!", MessageConstants.MESSAGE_ERROR_403);

            }

            task.originalEstimate = model.originalEstimate;


            await _taskRepository.UpdateAsync(model.taskId, task);

            return new ResponseEntity(StatusCodeConstants.OK, "Update task successfully!", MessageConstants.UPDATE_SUCCESS);
        }

        public async Task<ResponseEntity> addTaskUser(TaskUser model, string token)
        {

            var task = _taskRepository.GetSingleByConditionAsync("taskId", model.taskId).Result;

            if (task == null)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "task is not found!", MessageConstants.MESSAGE_ERROR_404);

            }

            UserJira user = _userService.getUserByToken(token).Result;
            Project pro = _projectRepository.GetSingleByConditionAsync("id", task.projectId).Result;

            if (pro == null)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "Project is not found!", MessageConstants.MESSAGE_ERROR_404);

            }
            if (pro.creator != user.id)
            {
                return new ResponseEntity(StatusCodeConstants.FORBIDDEN, "User is unthorization!", MessageConstants.MESSAGE_ERROR_403);

            }

            List<KeyValuePair<string, dynamic>> columns = new List<KeyValuePair<string, dynamic>>();
            columns.Add(new KeyValuePair<string, dynamic>("taskId", model.taskId));
            columns.Add(new KeyValuePair<string, dynamic>("userId", model.userId));
            var taskUser = _taskUserRepository.GetMultiByListConditionAndAsync(columns).Result;
            if(taskUser.Count() >0)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, "User is unthorization!", MessageConstants.ACCOUNT_EXITST_TASK);

            }
            Task_User taskUserInsert = new Task_User();
            taskUserInsert.useId = model.taskId;
            taskUserInsert.taskId = model.userId;
            await _taskUserRepository.InsertAsync(taskUserInsert);
            return new ResponseEntity(StatusCodeConstants.OK, "add user to task successfully!", MessageConstants.UPDATE_SUCCESS);
        }

        public async Task<ResponseEntity> removeUserFromTask(TaskUser model, string token)
        {
            var task = _taskRepository.GetSingleByConditionAsync("taskId", model.taskId).Result;

            if (task == null)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "task is not found!", MessageConstants.MESSAGE_ERROR_404);

            }

            UserJira user = _userService.getUserByToken(token).Result;
            Project pro = _projectRepository.GetSingleByConditionAsync("id", task.projectId).Result;

            if (pro == null)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "Project is not found!", MessageConstants.MESSAGE_ERROR_404);

            }
            if (pro.creator != user.id)
            {
                return new ResponseEntity(StatusCodeConstants.FORBIDDEN, "User is unthorization!", MessageConstants.MESSAGE_ERROR_403);

            }
            List<KeyValuePair<string, dynamic>> columns = new List<KeyValuePair<string, dynamic>>();
            columns.Add(new KeyValuePair<string, dynamic>("taskId", model.taskId));
            columns.Add(new KeyValuePair<string, dynamic>("userId", model.userId));
            var taskUser = _taskUserRepository.GetMultiByListConditionAndAsync(columns).Result;
            List<dynamic> lstId = new List<dynamic>();
            foreach(var item in taskUser)
            {
                lstId.Add(item.id);
            }

            await _taskUserRepository.DeleteByIdAsync(lstId);

            return new ResponseEntity(StatusCodeConstants.OK, "remove user from task successfully!", MessageConstants.MESSAGE_SUCCESS_200);
        }

        public async Task<ResponseEntity> removeUSerFromProject(Project_User project, string token)
        {
            UserJira user = _userService.getUserByToken(token).Result;
            Project pro = _projectRepository.GetSingleByConditionAsync("id", project.projectId).Result;

            if (pro == null)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "Project is not found!", MessageConstants.MESSAGE_ERROR_404);

            }
            if (pro.creator != user.id)
            {
                return new ResponseEntity(StatusCodeConstants.FORBIDDEN, "User is unthorization!", MessageConstants.MESSAGE_ERROR_403);

            }
            List<KeyValuePair<string, dynamic>> columns = new List<KeyValuePair<string, dynamic>>();
            columns.Add(new KeyValuePair<string, dynamic>("projectId", project.projectId));
            columns.Add(new KeyValuePair<string, dynamic>("userId", project.userId));


            IEnumerable<Project_User> lstProjectUser = _projectUserRepository.GetMultiByListConditionAndAsync(columns).Result;
            List<dynamic> lstId = new List<dynamic>();
            foreach (var item in lstProjectUser)
            {
                lstId.Add(item.id);
            }

            await _projectUserRepository.DeleteByIdAsync(lstId);
            return new ResponseEntity(StatusCodeConstants.OK, "remove user from project successfully !", MessageConstants.MESSAGE_SUCCESS_200);
        }

        public async Task<ResponseEntity> createTask(taskInsert model, string token)
        {
            UserJira user = _userService.getUserByToken(token).Result;
            Project pro = _projectRepository.GetSingleByConditionAsync("id", model.projectId).Result;

            if (pro == null)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "Project is not found!", MessageConstants.MESSAGE_ERROR_404);

            }
            if (pro.creator != user.id)
            {
                return new ResponseEntity(StatusCodeConstants.FORBIDDEN, "User is unthorization!", MessageConstants.MESSAGE_ERROR_403);

            }

            Repository.Models.Task task = new Repository.Models.Task();
            task.taskName = model.taskName;
            task.alias = FuncUtilities.BestLower(model.taskName);
            task.description = model.description;
            task.statusId = model.statusId;
            task.originalEstimate = model.originalEstimate;
            task.timeTracking = model.timeTracking;
            task.timeTrackingMax = model.timeTrackingMax;
            task.projectId = model.projectId;
            task.typeId = model.typeId;
            task.reporterId = model.reporterId;
            task.priorityId = model.priorityId;
            task.deleted = false;

            await _taskRepository.InsertAsync(task);


            return new ResponseEntity(StatusCodeConstants.OK, "create task successfully!", MessageConstants.MESSAGE_SUCCESS_200);

        }

        public async Task<ResponseEntity> removeTask(int taskId, string token)
        {
            var task = _taskRepository.GetSingleByConditionAsync("taskId", taskId).Result;

            if (task == null)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "task is not found!", MessageConstants.MESSAGE_ERROR_404);

            }

            UserJira user = _userService.getUserByToken(token).Result;
            Project pro = _projectRepository.GetSingleByConditionAsync("id", task.projectId).Result;

            if (pro == null)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "Project is not found!", MessageConstants.MESSAGE_ERROR_404);

            }
            if (pro.creator != user.id)
            {
                return new ResponseEntity(StatusCodeConstants.FORBIDDEN, "User is unthorization!", MessageConstants.MESSAGE_ERROR_403);

            }
            dynamic taskIDD = taskId;
            IEnumerable<Task_User> taskUser = await _taskUserRepository.GetMultiByConditionAsync("taskId", taskIDD).Result;
            List<dynamic> lstIdTaskUser = new List<dynamic>();
            //Xóa task user
            foreach(var taskU in taskUser)
            {
                lstIdTaskUser.Add(taskU.id);
            }
            await _taskUserRepository.DeleteByIdAsync(lstIdTaskUser);
            //Xóa task comment
            dynamic taskCommnetId = taskId;
            IEnumerable<Comment> comment = await _userComment.GetMultiByConditionAsync("taskId", taskCommnetId).Result;
            List<dynamic> lstIdComment = new List<dynamic>();
            foreach(var item in comment)
            {
                lstIdComment.Add(item.id);
            }

            await _userComment.DeleteByIdAsync(lstIdComment);
            //Xóa task
            List<dynamic> lst = new List<dynamic>();
            lst.Add(task.taskId);

            await _taskRepository.DeleteByIdAsync(lst);
            return new ResponseEntity(StatusCodeConstants.OK, "Remove task successfully!", MessageConstants.MESSAGE_SUCCESS_200);

        }
    }

}
