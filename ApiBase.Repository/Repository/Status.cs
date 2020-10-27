using ApiBase.Repository.Infrastructure;
using ApiBase.Repository.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Repository.Repository
{
    public interface IStatusRepository : IRepository<Status>
    {

    }
    public class StatusRepository : RepositoryBase<Status>, IStatusRepository
    {
        public StatusRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
}
