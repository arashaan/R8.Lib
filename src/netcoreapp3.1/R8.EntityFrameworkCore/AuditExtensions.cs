﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace R8.EntityFrameworkCore
{
    public static class AuditExtensions
    {
        /// <summary>
        /// Finds changes that occurred in entry's entity properties.
        /// </summary>
        /// <param name="entry">A <see cref="EntityEntry"/> object that containing an database entry.</param>
        /// <param name="oldValues">A <see cref="Dictionary{TKey,TValue}"/> that representing a dictionary of values that has been changed.</param>
        /// <param name="newValues">A <see cref="Dictionary{TKey,TValue}"/> that representing a dictionary of values that has been replaces with old values.</param>
        /// <exception cref="ArgumentNullException"></exception>
        private static void FindChanges(this EntityEntry entry, out Dictionary<string, object> oldValues, out Dictionary<string, object> newValues)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            oldValues = new Dictionary<string, object>();
            newValues = new Dictionary<string, object>();
            if (entry.State != EntityState.Modified && entry.State != EntityState.Added)
                return;

            var propertyEntries = entry.Members
                .Where(x => x.Metadata.Name != nameof(EntityBase.Id)
                            && x.Metadata.Name != nameof(EntityBase.Audits)
                            && x.Metadata.Name != nameof(EntityBase.RowVersion)
                            && x is PropertyEntry)
                .Cast<PropertyEntry>()
                .ToList();
            if (propertyEntries.Count == 0)
                return;

            foreach (var propertyEntry in propertyEntries)
            {
                var propertyName = propertyEntry.Metadata.Name;
                var newValue = propertyEntry.CurrentValue;
                var oldValue = propertyEntry.OriginalValue;

                if (newValue == null && oldValue == null)
                    continue;

                if (newValue?.Equals(oldValue) == true)
                    continue;

                oldValues.Add(propertyName, oldValue);
                newValues.Add(propertyName, newValue);
            }
        }

        /// <summary>
        /// Generates and adds a audit in type of <see cref="IAudit"/> to given entity entry according to request.
        /// </summary>
        /// <param name="entry">A <see cref="EntityEntry"/> object that containing an database entry.</param>
        /// <param name="flag">A <see cref="AuditFlags"/> enumerator constant that representing type of audit.</param>
        /// <param name="userId">A <see cref="Guid"/> object that representing which user did changes.</param>
        /// <param name="ipAddress">A <see cref="IPAddress"/> object that representing what is user's ip address.</param>
        /// <param name="userAgent">A <see cref="string"/> value that representing user-agent according to request.</param>
        /// <param name="stackFrame">A <see cref="StackFrame"/> object that representing method caller.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void GenerateAudit(this EntityEntry entry, AuditFlags flag, Guid? userId, IPAddress ipAddress, string userAgent, StackFrame stackFrame)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            if (!(entry.Entity is EntityBase entityBase))
                return;

            var caller = stackFrame?.GetMethod()?.Name;
            var callerPath = stackFrame?.GetFileName();
            var callerLine = stackFrame?.GetFileLineNumber() ?? 0;

            FindChanges(entry, out var oldValues, out var newValues);
            var audit = new Audit(userId, ipAddress, userAgent, flag, entityBase.Id, caller, $"{callerPath}::{callerLine}", oldValues, newValues);
            entityBase.Audits ??= new AuditCollection();
            entityBase.Audits.Add(audit);
        }
    }
}