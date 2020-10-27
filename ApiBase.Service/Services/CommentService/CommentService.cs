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
        Task<ResponseEntity> updateComment(CommentModelUpdate commentUpdate, int idComment, string token);

    }
    public class CommentService : ServiceBase<Comment, Comment>, ICommentService
    {
        ICommentRepository _commentRepository;
        IUserService _userService;
        public CommentService(ICommentRepository proRe, IUserService userService,
            IMapper mapper)
            : base(proRe, mapper)
        {
            _commentRepository = proRe;
            _userService = userService;
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
                var result = await _commentRepository.GetSingleByConditionAsync("id", taskId);
                return new ResponseEntity(StatusCodeConstants.OK, result, MessageConstants.MESSAGE_SUCCESS_200);
            }catch(Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, "Comment is not found", MessageConstants.INSERT_ERROR);
            }
        }

        public async Task<ResponseEntity> insertComment(CommentModelInsert model,string token)
        {
            try
            {
                var userJira = await _userService.getUserByToken(token);
                Comment cmt = new Comment();
                cmt.alias = FuncUtilities.BestLower(model.contentComment);
                cmt.deleted = false;
                cmt.contentComment = model.contentComment;
                cmt.userId = userJira.id;
                cmt.taskId = model.taskId;
                cmt = await _commentRepository.InsertAsync(cmt);
                if (cmt == null)
                    return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, model, MessageConstants.INSERT_ERROR);

                return new ResponseEntity(StatusCodeConstants.CREATED, cmt, MessageConstants.INSERT_SUCCESS);
            }catch (Exception ex)
            {
                    return new ResponseEntity(StatusCodeConstants.AUTHORIZATION, "Unauthorize", MessageConstants.MESSAGE_ERROR_401);
                
            }
        }

        public async Task<ResponseEntity> updateComment(CommentModelUpdate commentUpdate,int idComment,string token)
        {
            try
            {
                var userJira = _userService.getUserByToken(token);
                Comment cmt = await _commentRepository.GetSingleByConditionAsync("id", idComment);
                if(cmt == null)
                {
                    return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, "Comment is not found !", MessageConstants.MESSAGE_ERROR_500);
                }
                if(cmt.userId != userJira.Id)
                {
                    return new ResponseEntity(StatusCodeConstants.FORBIDDEN, "403 Forbidden !", MessageConstants.MESSAGE_ERROR_500);
                }

                cmt.contentComment = commentUpdate.contentComment;
                cmt.alias = FuncUtilities.BestLower(cmt.contentComment);

                await _commentRepository.UpdateAsync(cmt.id, cmt);


                return new ResponseEntity(StatusCodeConstants.OK, cmt, MessageConstants.UPDATE_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, "Update fail", MessageConstants.UPDATE_ERROR);

            }
        }
    }

}
