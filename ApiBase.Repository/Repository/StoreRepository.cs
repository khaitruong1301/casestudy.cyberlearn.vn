using System;
using System.Collections.Generic;
using System.Text;
using ApiBase.Repository.Infrastructure;
using ApiBase.Repository.Models;
using Microsoft.Extensions.Configuration;

namespace ApiBase.Repository.Repository
{
    public interface IStoreRepository : IRepository<Store>
    {

    }

    public class StoreRepository : RepositoryBase<Store>, IStoreRepository
    {
        public StoreRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
}
