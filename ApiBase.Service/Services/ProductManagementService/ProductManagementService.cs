using ApiBase.Repository.Models;
using ApiBase.Service.ViewModels.ProductViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using ApiBase.Service.Infrastructure;
using ApiBase.Repository.Repository;
using AutoMapper;

namespace ApiBase.Service.Services.ProductManagementService
{

    public interface IProductManagementService : IService<Product, ProductViewModel>
    {

    }
    public class ProductManagementService : ServiceBase<Product, ProductViewModel>, IProductManagementService
    {
        IProductRepository _producRepository;
        public ProductManagementService(IProductRepository proRe,
            IMapper mapper)
            : base(proRe, mapper)
        {
            
        }
    }
}
