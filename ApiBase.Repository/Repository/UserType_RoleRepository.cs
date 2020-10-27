using ApiBase.Repository.Infrastructure;
using ApiBase.Repository.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Repository.Repository
{
   
    public interface IUserType_RoleRepository : IRepository<UserType_Role>
    {

    }

    public class UserType_RoleRepository : RepositoryBase<UserType_Role>, IUserType_RoleRepository
    {
        public UserType_RoleRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
}
