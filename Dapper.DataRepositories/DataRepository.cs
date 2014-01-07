using MicroOrm.Pocos.SqlGenerator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Dapper.DataRepositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class DataRepository<TEntity> : DataConnection, IDataRepository<TEntity> where TEntity : new()
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        protected DataRepository(IDbConnection connection, ISqlGenerator<TEntity> sqlGenerator)
            : base(connection)
        {
            SqlGenerator = sqlGenerator;
        }

        #endregion

        #region Properties

        protected ISqlGenerator<TEntity> SqlGenerator { get; private set; }

        #endregion

        #region Repository sync base actions

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetAll()
        {
            var sql = SqlGenerator.GetSelectAll();
            return Connection.Query<TEntity>(sql);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetWhere(object filters)
        {
            var sql = SqlGenerator.GetSelect(filters);
            return Connection.Query<TEntity>(sql, filters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public virtual TEntity GetFirst(object filters)
        {
            return this.GetWhere(filters).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual bool Insert(TEntity instance)
        {
            bool added = false;
            var sql = SqlGenerator.GetInsert();

            if (SqlGenerator.IsIdentity)
            {
                var newId = Connection.Query<decimal>(sql, instance).Single();
                added = newId > 0;

                if (added)
                {
                    var newParsedId = Convert.ChangeType(newId, SqlGenerator.IdentityProperty.PropertyInfo.PropertyType);
                    SqlGenerator.IdentityProperty.PropertyInfo.SetValue(instance, newParsedId);
                }
            }
            else
            {
                added = Connection.Execute(sql, instance) > 0;
            }

            return added;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool Delete(object key)
        {
            var sql = SqlGenerator.GetDelete();
            return Connection.Execute(sql, key) > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual bool Update(TEntity instance)
        {
            var sql = SqlGenerator.GetUpdate();
            return Connection.Execute(sql, instance) > 0;
        }

        #endregion

        #region Repository async base action

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var sql = SqlGenerator.GetSelectAll();
            return await Connection.QueryAsync<TEntity>(sql);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> GetWhereAsync(object filters)
        {
            var sql = SqlGenerator.GetSelect(filters);
            return await Connection.QueryAsync<TEntity>(sql, filters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> GetFirstAsync(object filters)
        {
            var sql = SqlGenerator.GetSelect(filters);
            Task<IEnumerable<TEntity>> queryTask = Connection.QueryAsync<TEntity>(sql, filters);
            IEnumerable<TEntity> data = await queryTask;
            return data.FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual async Task<bool> InsertAsync(TEntity instance)
        {
            bool added = false;
            var sql = SqlGenerator.GetInsert();

            if (SqlGenerator.IsIdentity)
            {
                Task<IEnumerable<decimal>> queryTask = Connection.QueryAsync<decimal>(sql, instance);
                IEnumerable<decimal> result = await queryTask;
                var newId = result.Single();
                added = newId > 0;

                if (added)
                {
                    var newParsedId = Convert.ChangeType(newId, SqlGenerator.IdentityProperty.PropertyInfo.PropertyType);
                    SqlGenerator.IdentityProperty.PropertyInfo.SetValue(instance, newParsedId);
                }
            }
            else
            {
                Task<IEnumerable<int>> queryTask = Connection.QueryAsync<int>(sql, instance);
                IEnumerable<int> result = await queryTask;
                added = result.Single() > 0;
            }

            return added;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(object key)
        {
            var sql = SqlGenerator.GetDelete();
            Task<IEnumerable<int>> queryTask = Connection.QueryAsync<int>(sql, key);
            IEnumerable<int> result = await queryTask;
            return result.Single() > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync(TEntity instance)
        {
            var sql = SqlGenerator.GetUpdate();
            Task<IEnumerable<int>> queryTask = Connection.QueryAsync<int>(sql, instance);
            IEnumerable<int> result = await queryTask;
            return result.Single() > 0;
        }

        #endregion
    }
}
