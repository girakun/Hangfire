﻿// This file is part of Hangfire.
// Copyright © 2013-2014 Sergey Odinokov.
// 
// Hangfire is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as 
// published by the Free Software Foundation, either version 3 
// of the License, or any later version.
// 
// Hangfire is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public 
// License along with Hangfire. If not, see <http://www.gnu.org/licenses/>.

using System;
#if FEATURE_TRANSACTIONSCOPE
using System.Transactions;
#else
using System.Data;
#endif

namespace Hangfire.SqlServer
{
    public class SqlServerStorageOptions
    {
        private TimeSpan _queuePollInterval;
        private string _schemaName;
        private string _tablePrefix;
		private TimeSpan _jobExpirationCheckInterval;
        private TimeSpan? _slidingInvisibilityTimeout;

        public SqlServerStorageOptions()
        {
            QueuePollInterval = TimeSpan.FromSeconds(15);
            SlidingInvisibilityTimeout = null;
#pragma warning disable 618
            InvisibilityTimeout = TimeSpan.FromMinutes(30);
#pragma warning restore 618
            JobExpirationCheckInterval = TimeSpan.FromMinutes(30);
            CountersAggregateInterval = TimeSpan.FromMinutes(5);
            PrepareSchemaIfNecessary = true;
            DashboardJobListLimit = 10000;
            _schemaName = Constants.DefaultSchema;
            _tablePrefix = Constants.DefaultTablePrefix;
			TransactionTimeout = TimeSpan.FromMinutes(1);
            DisableGlobalLocks = false;
            UsePageLocksOnDequeue = false;
        }

        [Obsolete("TransactionIsolationLevel option is deprecated, please set UseRecommendedIsolationLevel instead. Will be removed in 2.0.0.")]
        public IsolationLevel? TransactionIsolationLevel { get; set; }

        public TimeSpan QueuePollInterval
        {
            get { return _queuePollInterval; }
            set
            {
                var message = $"The QueuePollInterval property value should be positive. Given: {value}.";

                if (value != value.Duration())
                {
                    throw new ArgumentException(message, nameof(value));
                }

                _queuePollInterval = value;
            }
        }

        [Obsolete("Does not make sense anymore. Background jobs re-queued instantly even after ungraceful shutdown now. Will be removed in 2.0.0.")]
        public TimeSpan InvisibilityTimeout { get; set; }

        public TimeSpan? SlidingInvisibilityTimeout
        {
            get { return _slidingInvisibilityTimeout; }
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException("Sliding timeout should be greater than zero");
                }

                _slidingInvisibilityTimeout = value;
            }
        }

        public bool PrepareSchemaIfNecessary { get; set; }

        public TimeSpan JobExpirationCheckInterval 
        {
            get { return _jobExpirationCheckInterval; } 
            set {
                if (value.TotalMilliseconds > int.MaxValue)
                {
                    throw new ArgumentOutOfRangeException("Job expiration check interval cannot be greater than int.MaxValue");
                }
                _jobExpirationCheckInterval = value;
            }
        }

        public TimeSpan CountersAggregateInterval { get; set; }

        public int? DashboardJobListLimit { get; set; }
        public TimeSpan TransactionTimeout { get; set; }
        public TimeSpan? CommandTimeout { get; set; }
        public TimeSpan? CommandBatchMaxTimeout { get; set; }

        public string SchemaName
        {
            get { return _schemaName; }
            set
            {
                if (string.IsNullOrWhiteSpace(_schemaName))
                {
                    throw new ArgumentException(_schemaName, nameof(value));
                }
                _schemaName = value;
            }
        }

		public string TablePrefix
		{
			get { return _tablePrefix; }
			set
			{
				if (string.IsNullOrWhiteSpace(_tablePrefix))
				{
					throw new ArgumentException(_tablePrefix, nameof(value));
				}
				_tablePrefix = value;
			}
		}

		public Func<IDisposable> ImpersonationFunc { get; set; }
        public bool DisableGlobalLocks { get; set; }
        public bool UsePageLocksOnDequeue { get; set; }
        public bool UseRecommendedIsolationLevel { get; set; }
        public bool EnableHeavyMigrations { get; set; }
        public bool UseFineGrainedLocks { get; set; }
    }
}
