using ApiBase.Repository.Infrastructure;
using ApiBase.Repository.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Repository.Repository
{

    public interface IOrderProductRepository : IRepository<OrderProduct>
    {

    }

    public class OrderProductRepository : RepositoryBase<OrderProduct>, IOrderProductRepository
    {
        public OrderProductRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
}
