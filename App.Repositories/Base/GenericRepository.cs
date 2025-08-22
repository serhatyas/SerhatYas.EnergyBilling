using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace App.Repositories.Base
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly List<T> _data;

        public GenericRepository(List<T> data)
        {
            _data = data ?? new List<T>();
        }

        // Tüm kayıtları getir
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Task.FromResult(_data.AsEnumerable());
        }

        // ID'ye göre kayıt bul
        public virtual async Task<T?> GetByIdAsync(object id)
        {
            return await Task.FromResult(_data.FirstOrDefault());
        }

        // Koşula göre bul
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await Task.FromResult(_data.AsQueryable().Where(predicate));
        }

        // Tek kayıt getir
        public virtual async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await Task.FromResult(_data.AsQueryable().FirstOrDefault(predicate));
        }

        // Kayıt ekle
        public virtual async Task AddAsync(T entity)
        {
            _data.Add(entity);
            await Task.CompletedTask;
        }

        // Çoklu kayıt ekle
        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            _data.AddRange(entities);
            await Task.CompletedTask;
        }

        // Kayıt güncelle
        public virtual void Update(T entity)
        {
            // Memory'de çalıştığımız için özel işlem yapmıyoruz
        }

        // Kayıt sil
        public virtual void Remove(T entity)
        {
            _data.Remove(entity);
        }

        // Çoklu kayıt sil
        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                _data.Remove(entity);
            }
        }
    }
}
