using AutoMapper;
using ApiBase.Repository.Infrastructure;
using ApiBase.Repository.Models;
using ApiBase.Service.Constants;
using ApiBase.Service.Utilities;
using ApiBase.Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiBase.Service.Constants;

namespace ApiBase.Service.Infrastructure
{
    public interface IService<T, V> where T : class
    {
        Task<ResponseEntity> InsertAsync(V modelVm);

        Task<ResponseEntity> UpdateAsync(dynamic id, V modelVm);
        Task<ResponseEntity> UpdateAsyncHasArrayNull(dynamic id, V modelVm);


        Task<ResponseEntity> GetAllAsync();

        Task<ResponseEntity> GetPagingAsync(int pageIndex, int pageSize, string keywords, string filter = "");

        Task<ResponseEntity> GetMultiByIdAsync(List<dynamic> listId);

        Task<ResponseEntity> GetSingleByIdAsync(dynamic id);

        Task<ResponseEntity> DeleteByIdAsync(List<dynamic> listId);
    }

    public abstract class ServiceBase<T, V> : IService<T, V> where T : class
    {
        private readonly IRepository<T> _repository;
        protected readonly IMapper _mapper;

        public ServiceBase(IRepository<T> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public virtual async Task<ResponseEntity> GetAllAsync()
        {
            try
            {
                IEnumerable<T> entities = await _repository.GetAllAsync();
                var modelVm = _mapper.Map<IEnumerable<V>>(entities);
                return new ResponseEntity(StatusCodeConstants.OK, modelVm);
            }
            catch (Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, ex.Message);
            }
        }

        public virtual async Task<ResponseEntity> GetPagingAsync(int pageIndex, int pageSize, string keywords, string filter = "")
        {
            try
            {
                keywords = FuncUtilities.BestLower(keywords);
                PagingResult<T> entity = await _repository.GetPagingAsync(pageIndex, pageSize, keywords, filter);
                var modelVm = new PagingResult<V>();
                modelVm.Items = _mapper.Map<IEnumerable<V>>(entity.Items);
                modelVm.PageIndex = entity.PageIndex;
                modelVm.PageSize = entity.PageSize;
                modelVm.Keywords = entity.Keywords;
                modelVm.TotalRow = entity.TotalRow;

                return new ResponseEntity(StatusCodeConstants.OK, modelVm);
            }
            catch (Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, ex.Message);
            }
        }

        public virtual async Task<ResponseEntity> GetSingleByIdAsync(dynamic id)
        {
            try
            {
                var entity = await _repository.GetSingleByIdAsync(id);
                if (entity == null)
                    return new ResponseEntity(StatusCodeConstants.NOT_FOUND);

                V modelVm = _mapper.Map<V>(entity);
                return new ResponseEntity(StatusCodeConstants.OK, modelVm);
            }
            catch (Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, ex.Message);
            }
        }

        public virtual async Task<ResponseEntity> GetMultiByIdAsync(List<dynamic> listId)
        {
            try
            {
                var entity = await _repository.GetMultiByIdAsync(listId);
                IEnumerable<V> modelVm = _mapper.Map<IEnumerable<V>>(entity);
                return new ResponseEntity(StatusCodeConstants.OK, modelVm);
            }
            catch (Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, ex.Message);
            }
        }

        public virtual async Task<ResponseEntity> InsertAsync(V modelVm)
        {
            try
            {
                T entity = _mapper.Map<T>(modelVm);
                entity = await _repository.InsertAsync(entity);

                modelVm = _mapper.Map<V>(entity);
                return new ResponseEntity(StatusCodeConstants.CREATED, modelVm, MessageConstants.INSERT_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, ex.Message);
            }

        }

        public virtual async Task<ResponseEntity> UpdateAsync(dynamic id, V modelVm)
        {
            try
            {
                T entity = await _repository.GetSingleByIdAsync(id);
                if (entity == null)
                    return new ResponseEntity(StatusCodeConstants.NOT_FOUND, modelVm);

                entity = _mapper.Map<T>(modelVm);
                await _repository.UpdateAsync(id, entity);

                return new ResponseEntity(StatusCodeConstants.OK, modelVm, MessageConstants.UPDATE_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, ex.Message);
            }
        }

        public virtual async Task<ResponseEntity> UpdateAsyncHasArrayNull(dynamic id, V modelVm)
        {
            try
            {
                T entity = await _repository.GetSingleByIdAsync(id);
                if (entity == null)
                    return new ResponseEntity(StatusCodeConstants.NOT_FOUND, modelVm);

                entity = _mapper.Map<T>(modelVm);
                await _repository.UpdateAsync(id, entity);

                return new ResponseEntity(StatusCodeConstants.OK, modelVm, MessageConstants.UPDATE_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, ex.Message);
            }
        }
        public virtual async Task<ResponseEntity> DeleteByIdAsync(List<dynamic> listId)
        {
            try
            {
                if (await _repository.DeleteByIdAsync(listId) != 0)
                    return new ResponseEntity(StatusCodeConstants.OK, listId, MessageConstants.DELETE_SUCCESS);
                return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, MessageConstants.DELETE_SUCCESS, MessageConstants.DELETE_ERROR);
            }
            catch (Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, ex.Message);
            }
        }
    }
}