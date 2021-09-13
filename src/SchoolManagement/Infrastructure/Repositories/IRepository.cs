using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SchoolManagement.Infrastructure.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity">实体</typeparam>
    /// <typeparam name="TPrimaryKey">主键</typeparam>
    public interface IRepository<TEntity, TPrimaryKey> where TEntity : class
    {
        #region 查询

        /// <summary>
        /// 获取用于从整个表中检索实体的IQueryable
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// 用于获取所有实体
        /// </summary>
        /// <returns>所有实体列表</returns>
        List<TEntity> GetAllList();

        /// <summary>
        /// 用于获取所有实体的异步实现
        /// </summary>
        /// <returns>所有实体列表</returns>
        Task<List<TEntity>> GetAllListAsync();

        /// <summary>
        /// 用于获取传入本方法的所有实体 <paramref name="predicate">
        /// </summary>
        /// <param name="predicate">筛选实体的条件</param>
        /// <returns>所有实体列表</returns>
        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 用于获取传入本方法的所有实体 <paramref name="predicate">的异步实现
        /// </summary>
        /// <param name="predicate">筛选实体的条件</param>
        /// <returns>所有实体列表</returns>
        Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 通过传入的筛选条件来获取实体信息
        /// 如果查询不到返回值，则会引发异常
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>Entity</returns>
        TEntity Single(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 通过传入的筛选条件来获取实体信息
        /// 如果查询不到返回值，则会引发异常
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>Entity</returns>
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 通过传入的筛选条件来获取实体信息，如果查询不到返回值，则返回null
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>Entity</returns>
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 通过传入的筛选条件来获取实体信息，如果查询不到返回值，则返回null
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>Entity</returns>
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region Insert

        /// <summary>
        /// 添加一个新实体信息
        /// </summary>
        /// <param name="entity">被添加的实体</param>
        /// <returns></returns>
        TEntity Insert(TEntity entity);

        /// <summary>
        /// 添加一个新实体信息
        /// </summary>
        /// <param name="entity">被添加的实体</param>
        /// <returns></returns>
        Task<TEntity> InsertAsync(TEntity entity);

        #endregion

        #region Update

        /// <summary>
        /// 更新现有实体
        /// </summary>
        /// <param name="entity">被修改的实体</param>
        /// <returns></returns>
        TEntity Update(TEntity entity);

        /// <summary>
        /// 更新现有实体
        /// </summary>
        /// <param name="entity">被修改的实体</param>
        /// <returns></returns>
        Task<TEntity> UpdateAsync(TEntity entity);

        #endregion

        #region Delete

        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="entity">被删除的实体</param>
        /// <returns>无返回值</returns>
        void Delete(TEntity entity);

        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="entity">被删除的实体</param>
        /// <returns>无返回值</returns>
        Task DeleteAsync(TEntity entity);

        /// <summary>
        /// 按传入的条件可删除多个实体
        /// 注意：所有符合给定条件的实体都将被检索和删除
        /// 如果条件比较多，则待删除的实体也比较多，这可能导致主要的性能问题
        /// </summary>
        /// <param name="predicate">筛选实体的条件</param>
        /// <returns>无返回值</returns>
        void Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 按传入的条件可删除多个实体
        /// 注意：所有符合给定条件的实体都将被检索和删除
        /// 如果条件比较多，则待删除的实体也比较多，这可能导致主要的性能问题
        /// </summary>
        /// <param name="predicate">筛选实体的条件</param>
        /// <returns>无返回值</returns>
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region 总和计算

        /// <summary>
        /// 获取此仓储中所有实体的总和
        /// </summary>
        /// <returns>实体的总数</returns>
        int Count();

        /// <summary>
        /// 获取此仓储中所有实体的总和
        /// </summary>
        /// <returns>实体的总数</returns>
        Task<int> CountAsync();

        /// <summary>
        /// 支持筛选条件 <paramref name="predicate">计算仓储中的实体总和
        /// </summary>
        /// <param name="predicate">筛选条件</param>
        /// <returns>实体的总数</returns>
        int Count(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 支持筛选条件 <paramref name="predicate">计算仓储中的实体总和
        /// </summary>
        /// <param name="predicate">筛选条件</param>
        /// <returns>实体的总数</returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 获取此仓储中所有实体的总和
        /// <see cref="int.MaxValue"/>
        /// </summary>
        /// <returns>实体的总数</returns>
        long LongCount();

        /// <summary>
        /// 获取此仓储中所有实体的总和
        /// <see cref="int.MaxValue"/>
        /// </summary>
        /// <returns>实体的总数</returns>
        Task<long> LongCountAsync();

        /// <summary>
        /// 支持筛选条件 <paramref name="predicate">计算仓储中的实体总和
        /// <see cref="int.MaxValue"/>
        /// </summary>
        /// <param name="predicate">筛选条件</param>
        /// <returns>实体的总数</returns>
        long LongCount(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 支持筛选条件 <paramref name="predicate">计算仓储中的实体总和
        /// <see cref="int.MaxValue"/>
        /// </summary>
        /// <param name="predicate">筛选条件</param>
        /// <returns>实体的总数</returns>
        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion
    }
}
