using ApiBase.Repository.Infrastructure;
using ApiBase.Repository.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace ApiBase.Repository.Repository
{
    public interface IUserRepository : IRepository<AppUser>
    {
        Task<AppUser> GetByFacebookAsync(string facebookId);

    }

    public class UserRepository : RepositoryBase<AppUser>, IUserRepository
    {
        public UserRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public async Task<AppUser> GetByFacebookAsync(string facebookId)
        {
            List<KeyValuePair<string, dynamic>> columns = new List<KeyValuePair<string, dynamic>>();
            columns.Add(new KeyValuePair<string, dynamic>("FacebookId", facebookId));

            try
            {
                using (var conn = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@tableName", _table);
                    parameters.Add("@listColumn", JsonConvert.SerializeObject(columns));
                    return await conn.QueryFirstOrDefaultAsync<AppUser>("GET_SINGLE_DATA", parameters, null, null, CommandType.StoredProcedure);
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }
    }
}
