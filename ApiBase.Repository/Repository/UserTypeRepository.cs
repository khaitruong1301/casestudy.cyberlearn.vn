using ApiBase.Repository.Infrastructure;
using ApiBase.Repository.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Repository.Repository
{
  
    public interface IUserTypeRepository : IRepository<UserType>
    {

    }

    public class UserTypeRepository : RepositoryBase<UserType>, IUserTypeRepository
    {
        public UserTypeRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
}
