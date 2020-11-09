﻿using Microsoft.EntityFrameworkCore;

using R8.Lib.Localization;

using System.Globalization;
using System.Linq;

namespace R8.Lib.EntityFrameworkCore
{
    public static class EntityBaseGlobalizedExtensions
    {
        /// <summary>
        /// Filters a sequence of values based on <c><see cref="localizedName"/></c> and <c><see cref="canonicalName"/></c> in given <see cref="IQueryable{TResult}"/>.
        /// </summary>
        /// <typeparam name="TSource">A generic type of <see cref="EntityBaseGlobalized"/></typeparam>
        /// <param name="source">A <see cref="IQueryable{T}"/> that representing translating query.</param>
        /// <param name="canonicalName">A <see cref="string"/> value that representing canonical name that already stored in entity.</param>
        /// <param name="localizedName">A <see cref="string"/> value that representing localized name that already stored in <see cref="LocalizerContainer"/> in entity.</param>
        /// <returns>Continues chains of <see cref="IQueryable"/> plus adding current filter.</returns>
        public static IQueryable<TSource> WhereHas<TSource>(this IQueryable<TSource> source, string canonicalName, string localizedName) where TSource : EntityBaseGlobalized
        {
            var twoLetter = CultureInfo.CurrentCulture.GetTwoLetterCulture();
            return source.Where(x =>
                x.CanonicalName.Equals(canonicalName) ||
                EF.Functions.Like(x.NameJson, $"%\"{twoLetter}\":\"{localizedName}\"%"));
        }
    }
}