using ApiBase.Repository.Models;
using ApiBase.Service.ViewModels.ProductViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using ApiBase.Service.Infrastructure;
using ApiBase.Repository.Repository;
using AutoMapper;
using System.Threading.Tasks;
using ApiBase.Service.ViewModels;
using ApiBase.Service.Constants;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using ApiBase.Service.Utilities;
using System.Threading;
using System.Linq;

namespace ApiBase.Service.Services.ProductManagementService
{

    public interface IProductManagementService : IService<Product, ProductViewModel>
    {
        Task<ResponseEntity> GetById(int id);
        Task<ResponseEntity> GetAllProductPaging(int pageIndex, int pageSize, string keywords, string filter = "");
        Task<ResponseEntity> GetAllProductAsync(string keyword = "");
        Task<ResponseEntity> GetAllProductByCategoryIdAsync(string categoryId);
        Task<ResponseEntity> getProductByFeature(string feature);
        Task<ResponseEntity> getProductByStoreId(int storeId);
        Task<ResponseEntity> getAllStore(string keyword);



    }
    public class ProductManagementService : ServiceBase<Product, ProductViewModel>, IProductManagementService
    {
        IProductRepository _producRepository;
        ICategoryRepository _categoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        IStoreRepository _stoRepository;
        string myHostUrlImage = "";

        public ProductManagementService(IProductRepository proRe,IStoreRepository storeRepository, ICategoryRepository categoryRepository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(proRe, mapper)
        {
            _producRepository = proRe;
            _categoryRepository = categoryRepository;
            _httpContextAccessor = httpContextAccessor;
            myHostUrlImage = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/images/";
            _stoRepository = storeRepository;

        }

        public async Task<ResponseEntity> GetById(int id)
        {

            Product pro = await _producRepository.GetSingleByIdAsync(id);
            if (pro == null)
            {
                return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, id, MessageConstants.MESSAGE_ERROR_400);
            }

            //Lấy danh sách categorry 


            List<ProductRelationViewModel> lstRelatedPro = new List<ProductRelationViewModel>();
            //Lấy sản phẩm liên quan 
            if (!string.IsNullOrEmpty(pro.relatedProducts) && pro.relatedProducts != "[]")
            {
                List<string> relatePro = JsonConvert.DeserializeObject<List<string>>(pro.relatedProducts);
                foreach (var item in relatePro)
                {

                    Product p = await _producRepository.GetSingleByIdAsync(item);
                    ProductRelationViewModel rePro = new ProductRelationViewModel();

                    rePro.id = p.id;
                    rePro.alias = p.alias;
                    rePro.description = p.description;
                    rePro.name = p.name;
                    rePro.price = p.price;
                    rePro.shortDescription = p.shortDescription;
                    rePro.image = myHostUrlImage + p.image;
                    lstRelatedPro.Add(rePro);
                }
            }

            //Product proRelate = await 

            ProductDetailViewModel proDetail = new ProductDetailViewModel();
            proDetail.id = pro.id;
            proDetail.alias = pro.alias;
            proDetail.categories = JsonConvert.DeserializeObject<List<ProCategory>>(pro.categories);
            proDetail.description = pro.description;
            proDetail.name = pro.name;
            proDetail.price = pro.price;
            proDetail.quantity = pro.quantity;
            proDetail.shortDescription = pro.shortDescription;
            proDetail.size = JsonConvert.DeserializeObject<List<string>>(pro.size);
            proDetail.image = myHostUrlImage + pro.image;
            proDetail.relatedProducts = lstRelatedPro;
            //string danhSachQuyen = JsonConvert.SerializeObject(lstDanhSachQuyen);
            //List<string> roles = JsonConvert.DeserializeObject<List<string>>(danhSachQuyen);
            //proDetail.categories = JsonConvert.DeserializeObject<List<string>>(pro);


            return new ResponseEntity(StatusCodeConstants.OK, proDetail, MessageConstants.MESSAGE_SUCCESS_200);


        }

        public async Task<ResponseEntity> GetAllProductPaging(int pageIndex = 0, int pageSize = 10, string keywords = "", string filter = "")
        {
            PagingResult<Product> lstResult = await _producRepository.GetPagingAsync(pageIndex, pageSize, keywords, filter);
            if (lstResult.Items != null)
            {
                foreach (var item in lstResult.Items)
                {
                    item.image = myHostUrlImage + item.image;

                }
            }
            return new ResponseEntity(StatusCodeConstants.OK, lstResult, MessageConstants.MESSAGE_SUCCESS_200);


        }

        public async Task<ResponseEntity> GetAllProductAsync(string keyword = "")
        {
            keyword = FuncUtilities.BestLower(keyword);
            IEnumerable<Product> lstResult = await _producRepository.GetMultiByConditionLikeAsync("alias", keyword);
            if (lstResult != null)
            {
                foreach (var item in lstResult)
                {
                    item.image = myHostUrlImage + item.image;

                }
            }
            return new ResponseEntity(StatusCodeConstants.OK, lstResult, MessageConstants.MESSAGE_SUCCESS_200);
        }

        public async Task<ResponseEntity> GetAllProductByCategoryIdAsync(string categoryId = "ADIDAS")
        {
            var category = await _categoryRepository.GetSingleByConditionAsync("id", categoryId);

            List<dynamic> lstProductId = JsonConvert.DeserializeObject<List<dynamic>>(category.productList);

            List<ProductDetailViewModel> lstResult = new List<ProductDetailViewModel>();



            foreach (var item in lstProductId)
            {
                var pro = await _producRepository.GetSingleByIdAsync(item);
                ProductDetailViewModel proDetail = new ProductDetailViewModel();
                proDetail.id = pro.id;
                proDetail.alias = pro.alias;
                proDetail.categories = JsonConvert.DeserializeObject<List<ProCategory>>(pro.categories);
                proDetail.description = pro.description;
                proDetail.name = pro.name;
                proDetail.price = pro.price;
                proDetail.quantity = pro.quantity;
                proDetail.shortDescription = pro.shortDescription;
                proDetail.size = JsonConvert.DeserializeObject<List<string>>(pro.size);
                proDetail.image = myHostUrlImage + pro.image;
                //aaaaaaaaaaaaa
                List<ProductRelationViewModel> lstRelatedPro = new List<ProductRelationViewModel>();
                //Lấy sản phẩm liên quan 
                if (!string.IsNullOrEmpty(pro.relatedProducts) && pro.relatedProducts != "[]")
                {
                    List<string> relatePro = JsonConvert.DeserializeObject<List<string>>(pro.relatedProducts);
                    foreach (var idd in relatePro)
                    {

                        Product p = await _producRepository.GetSingleByIdAsync(idd);
                        ProductRelationViewModel rePro = new ProductRelationViewModel();

                        rePro.id = p.id;
                        rePro.alias = p.alias;
                        rePro.description = p.description;
                        rePro.name = p.name;
                        rePro.price = p.price;
                        rePro.shortDescription = p.shortDescription;
                        rePro.image = myHostUrlImage + p.image;
                        lstRelatedPro.Add(rePro);
                    }
                }
                proDetail.relatedProducts = lstRelatedPro;
                lstResult.Add(proDetail);
            }
            return new ResponseEntity(StatusCodeConstants.OK, lstResult, MessageConstants.MESSAGE_SUCCESS_200);



        }

        public async Task<ResponseEntity> getProductByFeature(string feature = "")
        {
            var lstProduct = await _producRepository.GetMultiByConditionAsync("feature", feature);

            List<ProductDetailViewModel> lstResult = new List<ProductDetailViewModel>();

            foreach (var item in lstProduct)
            {
                var pro = await _producRepository.GetSingleByIdAsync(item.id);
                ProductDetailViewModel proDetail = new ProductDetailViewModel();
                proDetail.id = pro.id;
                proDetail.alias = pro.alias;
                proDetail.categories = JsonConvert.DeserializeObject<List<ProCategory>>(pro.categories);
                proDetail.description = pro.description;
                proDetail.name = pro.name;
                proDetail.price = pro.price;
                proDetail.quantity = pro.quantity;
                proDetail.shortDescription = pro.shortDescription;
                proDetail.size = JsonConvert.DeserializeObject<List<string>>(pro.size);
                proDetail.image = myHostUrlImage + pro.image;
                //aaaaaaaaaaaaa
                List<ProductRelationViewModel> lstRelatedPro = new List<ProductRelationViewModel>();
                //Lấy sản phẩm liên quan 
                if (!string.IsNullOrEmpty(pro.relatedProducts) && pro.relatedProducts != "[]")
                {
                    List<string> relatePro = JsonConvert.DeserializeObject<List<string>>(pro.relatedProducts);
                    foreach (var idd in relatePro)
                    {

                        Product p = await _producRepository.GetSingleByIdAsync(idd);
                        ProductRelationViewModel rePro = new ProductRelationViewModel();

                        rePro.id = p.id;
                        rePro.alias = p.alias;
                        rePro.description = p.description;
                        rePro.name = p.name;
                        rePro.price = p.price;
                        rePro.shortDescription = p.shortDescription;
                        rePro.image = myHostUrlImage + p.image;
                        lstRelatedPro.Add(rePro);
                    }
                }
                proDetail.relatedProducts = lstRelatedPro;
                lstResult.Add(proDetail);
            }

            return new ResponseEntity(StatusCodeConstants.OK, lstResult, MessageConstants.MESSAGE_SUCCESS_200);

        }


        public Task<ResponseEntity> getProductByStoreId(int storeId)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseEntity> getAllStore(string keyword)
        {

            keyword = FuncUtilities.BestLower(keyword);
            var storeList = await _stoRepository.GetMultiByConditionLikeAsync("alias", keyword);
            var storeList1 = storeList.Select(n => new Store {id=n.id,alias=n.alias,deleted=n.deleted,description=n.description,latitude=n.latitude,longtitude=n.longtitude,name=n.name ,image = myHostUrlImage + n.image }).ToList();

            return new ResponseEntity(StatusCodeConstants.OK, storeList1, MessageConstants.MESSAGE_SUCCESS_200);
        }



    }
}
