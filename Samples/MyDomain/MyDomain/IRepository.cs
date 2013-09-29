﻿using System.Linq;

namespace MyDomain
{
    public interface IRepository<TEntity> : IQueryable<TEntity> where TEntity : class
    {
        TEntity Get(dynamic id);
        void Add(TEntity newEntity);
        void Remove(TEntity entity);
    }
}