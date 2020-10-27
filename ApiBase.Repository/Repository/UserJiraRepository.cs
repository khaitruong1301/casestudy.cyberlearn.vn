using ApiBase.Repository.Infrastructure;
using ApiBase.Repository.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Repository.Repository
{
    public interface IUserJiraRepository : IRepository<UserJira>
    {

    }
    public class UserJiraRepository : RepositoryBase<UserJira>, IUserJiraRepository
    {
        public UserJiraRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
}
