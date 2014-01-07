using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dapper.DataRepositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IDataRepository<TEntity> where TEntity : new()
    {
        #region Sync

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        IEnumerable<TEntity> GetWhere(object filters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        TEntity GetFirst(object filters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        bool Insert(TEntity instance);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Delete(object key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        bool Update(TEntity instance);

        #endregion

        #region Async

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetAllAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetWhereAsync(object filters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        Task<TEntity> GetFirstAsync(object filters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(TEntity instance);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        Task<bool> InsertAsync(TEntity instance);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(object key);

        #endregion
    }
}
