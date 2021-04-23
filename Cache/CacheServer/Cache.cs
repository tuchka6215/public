using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CacheServer
{
    class Cache
    {
        public enum ItemExpirationMode
        {
            Eternal = 0,
            SinceLastAccess = 1,
            SinceCreation = 2
        }

        public enum ValueType
        {
            String = 0,
            List = 1,
            Set = 2,
            Map = 3
        }

        public class Value
        {
            public readonly ItemExpirationMode ExpirationMode;
            public readonly ValueType Type;
            public object Content;
            public DateTime Time;
            public TimeSpan Life;

            public Value(ValueType type, ItemExpirationMode expirationMode)
            {
                Type = type;
                ExpirationMode = expirationMode;
            }
        }

        private Thread CleanupThread;
        private Dictionary<string, Value> Items = new Dictionary<string, Value>();

        public Cache()
        {
            CleanupThread = new Thread(new ThreadStart(() =>
            {
                for (; ; Thread.Sleep(60000))
                {
                    lock (Items)
                    {
                        var items = Items.ToArray();
                        foreach (var item in items)
                        {
                            if (item.Value.Time + item.Value.Life > DateTime.UtcNow)
                            {
                                Items.Remove(item.Key);
                            }

                        }
                    }
                }
            }));

            CleanupThread.Start();
        }

        public Value GetValue(string key)
        {
            lock (Items)
            {
                Value value; if (!Items.TryGetValue(key, out value)) return null;

                if (value.ExpirationMode == ItemExpirationMode.SinceLastAccess || value.ExpirationMode == ItemExpirationMode.SinceCreation)
                {
                    if (value.ExpirationMode == ItemExpirationMode.SinceLastAccess)
                    {
                        value.Time = DateTime.UtcNow;
                    }
                }

                return value;
            }
        }

        private void SetValue(string key, ValueType type, object content, TimeSpan life = default(TimeSpan), ItemExpirationMode expirationMode = ItemExpirationMode.Eternal)
        {
            var item = new Value(type, expirationMode) { Content = content, Time = DateTime.UtcNow, Life = life };
            lock (Items)
            {
                Items[key] = item;
            }
        }

        public void SetStringValue(string key, string content, TimeSpan life = default(TimeSpan), ItemExpirationMode expirationMode = ItemExpirationMode.Eternal)
        {
            SetValue(key, ValueType.String, content, life, expirationMode);
        }

        public void SetListValue(string key, string[] content, TimeSpan life = default(TimeSpan), ItemExpirationMode expirationMode = ItemExpirationMode.Eternal)
        {
            SetValue(key, ValueType.List, content, life, expirationMode);
        }

        public void SetSetValue(string key, string[] content, TimeSpan life = default(TimeSpan), ItemExpirationMode expirationMode = ItemExpirationMode.Eternal)
        {
            content = content.Distinct().ToArray();
            SetValue(key, ValueType.Set, content, life, expirationMode);
        }

        public void SetMapValue(string key, Dictionary<string, string> content, TimeSpan life = default(TimeSpan), ItemExpirationMode expirationMode = ItemExpirationMode.Eternal)
        {
            SetValue(key, ValueType.Map, content, life, expirationMode);
        }

        public Dictionary<string, string> GetMapValue(string key)
        {
            var value = GetValue(key);
            if (value == null || value.Type != ValueType.Map) return null;
            return value.Content as Dictionary<string, string>;
        }

        public bool RemoveValueItem(string key, string item)
        {
            lock (Items)
            {
                var value = GetValue(key); 
                if (value == null) return false;

                if (value.Type == ValueType.List || value.Type == ValueType.Set)
                {
                    var array = value.Content as string[];

                    if (array != null && array.Contains(item))
                    {
                        value.Content = array.Where(i => i != item).ToArray();
                        return true;
                    }
                }

                if (value.Type == ValueType.Map)
                {
                    var map = value.Content as Dictionary<string, string>;

                    if (map != null && map.ContainsKey(item))
                    {
                        map.Remove(item);
                        return true;
                    }
                }

                return false;
            }
        }

        public bool AddValueItem(string key, string item)
        {
            lock (Items)
            {
                var value = GetValue(key);
                if (value == null) return false;

                if (value.Type == ValueType.List || value.Type == ValueType.Set)
                {
                    var array = value.Content as string[];

                    if (array != null)
                    {
                        if (value.Type == ValueType.Set && array.Contains(item))
                        {
                            return false;
                        }

                        value.Content = array.Union(new string[] { item }).ToArray();
                        return true;
                    }
                }

                return false;
            }
        }

        public bool MapValueItem(string key, string item, string data)
        {
            lock (Items)
            {
                var value = GetValue(key);
                if (value == null) return false;

                if (value.Type == ValueType.Map)
                {
                    var map = value.Content as Dictionary<string, string>;

                    if (map != null)
                    {
                        map[item] = data;
                        return true;
                    }
                }

                return false;
            }
        }
    }
}