using ApiBase.Repository.Models;
using ApiBase.Repository.Repository;
using ApiBase.Service.Constants;
using ApiBase.Service.Infrastructure;
using ApiBase.Service.Services.UserService;
using ApiBase.Service.Utilities;
using ApiBase.Service.ViewModels;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApiBase.Service.Services.CommentService
{

    public interface ICommentService : IService<Comment, Comment>
    {
        Task<ResponseEntity> getCommentByTask(int taskId=0);
        Task<ResponseEntity> insertComment(CommentModelInsert model,string token);
        Task<ResponseEntity> deleteComment(int idComment, string token);
        Task<ResponseEntity> updateComment(CommentModelUpdate commentUpdate, string token);

    }
    public class CommentService : ServiceBase<Comment, Comment>, ICommentService
    {
        ICommentRepository _commentRepository;
        IUserService _userService;
        IUserJiraRepository _userJira;
        public CommentService(ICommentRepository proRe, IUserService userService, IUserJiraRepository usersv,
            IMapper mapper)
            : base(proRe, mapper)
        {
            _commentRepository = proRe;
            _userService = userService;
            _userJira = usersv;
        }

     
        public async Task<ResponseEntity> deleteComment(int idComment, string token)
        {
            try
            {
                var userJira = await _userService.getUserByToken(token);
                Comment comment = await _commentRepository.GetSingleByIdAsync(idComment);

                if(comment == null)
                {
                    return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "Comment is not found", MessageConstants.MESSAGE_ERROR_404);

                }
                if(comment.userId != userJira.id)
                {
                    return new ResponseEntity(StatusCodeConstants.FORBIDDEN, "403 Forbidden !", MessageConstants.MESSAGE_ERROR_500);

                }
                await _commentRepository.DeleteByIdAsync(new List<dynamic>() { idComment});
                return new ResponseEntity(StatusCodeConstants.OK, "Deleted comment success", MessageConstants.MESSAGE_SUCCESS_200);

            }
            catch (Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, "Comment is not found", MessageConstants.INSERT_ERROR);
            }
        }

        public async Task<ResponseEntity> getCommentByTask(int taskId)
        {
            try
            {
                var result =  _commentRepository.GetMultiByConditionAsync("taskId", taskId).Result;

                List<CommentViewModel> lstComment = new List<CommentViewModel>();
                foreach(var item in result)
                {
                    var user = _userJira.GetSingleByConditionAsync("id",item.userId).Result;

                    CommentViewModel cmt = new CommentViewModel();
                    cmt.alias = item.alias;
                    cmt.contentComment = FuncUtilities.Base64Decode(item.contentComment);
                    cmt.id = item.id;
                    cmt.taskId = item.taskId;
                    cmt.userId = item.userId;
                    cmt.user.userId = user.id;
                    cmt.user.avatar = user.avatar;
                    cmt.user.name = user.name;


                    lstComment.Add(cmt);
                }
                return new ResponseEntity(StatusCodeConstants.OK, lstComment, MessageConstants.MESSAGE_SUCCESS_200);
            }catch(Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, "Comment is not found", MessageConstants.INSERT_ERROR);
            }
        }

        public async Task<ResponseEntity> insertComment(CommentModelInsert model,string token)
        {
            try
            {
                var userJira =  _userService.getUserByToken(token).Result;
                Comment cmt = new Comment();
                cmt.alias = FuncUtilities.BestLower(model.contentComment);
                cmt.deleted = false;
                cmt.contentComment = FuncUtilities.Base64Encode( model.contentComment);
                cmt.userId = userJira.id;
                cmt.taskId = model.taskId;
                cmt = await _commentRepository.InsertAsync(cmt);
                if (cmt == null)
                    return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, model, MessageConstants.INSERT_ERROR);
                cmt.contentComment = FuncUtilities.Base64Decode(cmt.contentComment);
                return new ResponseEntity(StatusCodeConstants.OK, cmt, MessageConstants.INSERT_SUCCESS);
            }catch (Exception ex)
            {
                    return new ResponseEntity(StatusCodeConstants.AUTHORIZATION, "Unauthorize", MessageConstants.MESSAGE_ERROR_401);
                
            }
        }

        
        public async Task<ResponseEntity> updateComment(CommentModelUpdate commentUpdate,string token)
        {
            try
            {
                var userJira = _userService.getUserByToken(token);
                Comment cmt =  _commentRepository.GetSingleByConditionAsync("id", commentUpdate.id).Result;
                if(cmt == null)
                {
                    return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, "Comment is not found !", MessageConstants.MESSAGE_ERROR_500);
                }
                if(cmt.userId != userJira.Result.id)
                {
                    return new ResponseEntity(StatusCodeConstants.FORBIDDEN, "403 Forbidden !", MessageConstants.MESSAGE_ERROR_500);
                }

                cmt.contentComment = FuncUtilities.Base64Encode(commentUpdate.contentComment);
                cmt.alias = FuncUtilities.BestLower(cmt.contentComment);

                await _commentRepository.UpdateAsync(cmt.id, cmt);

                cmt.contentComment = FuncUtilities.Base64Decode(cmt.contentComment);

                return new ResponseEntity(StatusCodeConstants.OK, cmt, MessageConstants.UPDATE_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, "Update fail", MessageConstants.UPDATE_ERROR);

            }
        }
    }

}
