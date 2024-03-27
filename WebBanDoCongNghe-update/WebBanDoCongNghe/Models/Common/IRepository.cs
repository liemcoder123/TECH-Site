using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanDoCongNghe.Models.Common
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        T GetById(int id);
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        void SaveChanges();
        void Dispose();
    }

}