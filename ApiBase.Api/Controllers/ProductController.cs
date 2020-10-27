using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiBase.Repository.Repository;
using ApiBase.Service.Constants;
using ApiBase.Service.Services.ProductManagementService;
using ApiBase.Service.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ApiBase.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private IProductManagementService _productService;
        private ICategoryService _categoryService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        string myHostUrlImage = "";

        public ProductController(IProductManagementService proService, IHttpContextAccessor httpContextAccessor, ICategoryService categoryService)
        {
            _productService = proService;
            _httpContextAccessor = httpContextAccessor;
            _categoryService = categoryService;
            myHostUrlImage = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/images/";
        }


        [HttpGet]
        public async Task<IActionResult> Get(string keyword = "")
        {
            return await _productService.GetAllProductAsync(keyword);
        }

        [HttpGet("getProductByCategory")]
        public async Task<IActionResult> getProductByCategory(string categoryId = "")
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, categoryId, "CategoryId not found!");

            }
            return await _productService.GetAllProductByCategoryIdAsync(categoryId);

        }

        [HttpGet("getProductByFeature")]
        public async Task<IActionResult> getProductByFeature(string feature = "")
        {
            if (string.IsNullOrEmpty(feature))
            {
                return await _productService.GetAllProductAsync("");

            }
            return await _productService.getProductByFeature(feature);

        }

        [HttpGet("getAllCategory")]
        public async Task<IActionResult> getAllCategory(string keyword = "")
        {

            return await _categoryService.GetAll(keyword);
        }


        [HttpGet("getpaging")]
        public async Task<IActionResult> GetPaging(int pageIndex = 1, int pageSize = 10, string keywords = "")
        {
            return await _productService.GetPagingAsync(pageIndex, pageSize, keywords, "");
        }

        [HttpGet("getbyid")]

        public async Task<IActionResult> GetById(int id)
        {
            return await _productService.GetById(id);
        }


        //[HttpGet("getproductstoreid")]
        //public async Task<IActionResult> GetProductByStoreId(int storeId)
        //{
        //    return await _productService.getProductByStoreId(storeId);
        //}
        [HttpGet("getAllStore")]
        public async Task<IActionResult> getAllStore(string keyword="")
        {
            return await _productService.getAllStore(keyword);
        }
    }
}