﻿using Microsoft.EntityFrameworkCore;

using R8.Lib.EntityFrameworkCore;
using R8.Lib.Enums;

using System;
using System.Linq;
using System.Linq.Expressions;

namespace R8.Lib.AspNetCore.EntityFrameworkCore
{
    public static class QueryWebExtensions
    {
        public static IQueryable<TSource> BelongToUser<TSource, TUser>(this IQueryable<TSource> query, TUser user, Expression<Func<TSource, bool>> predicate) where TSource : EntityBase where TUser : CurrentUser
        {
            if (user == null || (user.Role != Roles.Admin && user.Role != Roles.Operator))
                query = query.Where(predicate);

            return query;
        }

        public static IQueryable<TSource> InitializeQuery<TDbContext, TSource>(this TDbContext context, Expression<Func<TDbContext, DbSet<TSource>>> dbSet, CurrentUser user, bool ignoreFilterForAdmins = true) where TDbContext : DbContextBase where TSource : EntityBase
        {
            var _dbSet = dbSet.Compile().Invoke(context);
            var result = ignoreFilterForAdmins && (user.Role == Roles.Admin || user.Role == Roles.Operator)
                ? _dbSet.IgnoreQueryFilters()
                : _dbSet.AsQueryable();
            return result;
        }

        public static IQueryable<TSource> InitializeQuery<TDbContext, TSource>(this TDbContext context, Expression<Func<TDbContext, DbSet<TSource>>> dbSet, bool ignoreFilterForAdmins = true) where TDbContext : DbContextBase where TSource : EntityBase
        {
            var currentUser = context.HttpContextAccessor.HttpContext.GetCurrentUser();
            if (currentUser != null && (currentUser.Role == Roles.Admin || currentUser.Role == Roles.Operator))
            {
                return dbSet.Compile().Invoke(context)
                    .IgnoreQueryFilters()
                    .AsQueryable();
            }

            return dbSet.Compile().Invoke(context).AsQueryable();
        }

        public static IQueryable<TSource> InitializeQuery<TDbContext, TSource>(this TDbContext context, Expression<Func<TDbContext, DbSet<TSource>>> dbSet, out CurrentUser user, bool ignoreFilterForAdmins = true) where TDbContext : DbContextBase where TSource : EntityBase
        {
            user = context.HttpContextAccessor.HttpContext.GetCurrentUser();

            var result = context.InitializeQuery(dbSet, user);
            return result;
        }
    }
}