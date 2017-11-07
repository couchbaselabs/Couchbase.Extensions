using System;
using System.Collections.Generic;
using System.Text;

namespace Couchbase.Extensions.Session.UnitTests
{
    public class BucketItem : IDisposable
    {
        public BucketItem(Dictionary<string, BucketItem> store, string key, byte[] value, TimeSpan duration)
        {
            Value = value;
            Key = key;

            _timer.Interval = duration.TotalSeconds;
            _timer.AutoReset = false;
            _timer.Elapsed += (sender, e) =>
            {
                if (store.TryGetValue(key, out BucketItem item))
                {
                    item.Dispose();
                    store.Remove(item.Key);
                }
            };
            _timer.Start();
        }

        private readonly System.Timers.Timer _timer = new System.Timers.Timer();

        public byte[] Value { get; set; }

        public string Key { get; set; }

        public void Reset(TimeSpan duration)
        {
            _timer.Stop();
            _timer.Interval = duration.TotalSeconds;
            _timer.Start();
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

#region [ License information          ]
/* ************************************************************
 * 
 *    @author Couchbase <info@couchbase.com>
 *    @copyright 2017 Couchbase, Inc.
 *    
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *    
 *        http://www.apache.org/licenses/LICENSE-2.0
 *    
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *    
 * ************************************************************/
#endregion
