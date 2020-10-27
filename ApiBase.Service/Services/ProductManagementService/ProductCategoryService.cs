using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ApiBase.Repository.Models;
using ApiBase.Repository.Repository;
using ApiBase.Service.Constants;
using ApiBase.Service.Infrastructure;
using ApiBase.Service.Utilities;
using ApiBase.Service.ViewModels;
using ApiBase.Service.ViewModels.ProductCategoryViewModel;
using ApiBase.Service.ViewModels.ProductViewModel;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace ApiBase.Service.Services.ProductManagementService
{
    public interface ICategoryService : IService<Category, ProductCategoryViewModel>
    {
        Task<ResponseEntity> GetAll(string keyword="");
        //Task<ResponseEntity> GetAllProductPaging(int pageIndex, int pageSize, string keywords, string filter = "");
        //Task<ResponseEntity> GetAllProductAsync(string keyword = "");
        //Task<ResponseEntity> GetAllProductByCategoryIdAsync(string categoryId);


    }
    public class CategoryService : ServiceBase<Category, ProductCategoryViewModel>,ICategoryService
    {
        IProductRepository _producRepository;
        ICategoryRepository _categoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        string myHostUrlImage = "";

        public CategoryService( ICategoryRepository categoryRepository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(categoryRepository, mapper)
        {
            _categoryRepository = categoryRepository;
            _httpContextAccessor = httpContextAccessor;
            myHostUrlImage = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/images/";

        }


        public async Task<ResponseEntity> GetAll(string keyword = "")
        {
            keyword = FuncUtilities.BestLower(keyword);
            var dsCategory = await _categoryRepository.GetMultiByConditionLikeAsync("alias", keyword);

            return new ResponseEntity(StatusCodeConstants.OK, dsCategory, MessageConstants.MESSAGE_SUCCESS_200);

        }

     
    }
}
