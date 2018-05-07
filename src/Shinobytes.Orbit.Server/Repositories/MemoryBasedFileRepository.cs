using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Shinobytes.Orbit.Server
{
    public abstract class MemoryBasedFileRepository<T, TKey>
    {
        private readonly string store;
        private readonly Func<T, TKey> getKey;

        protected readonly ConcurrentDictionary<TKey, T> items
            = new ConcurrentDictionary<TKey, T>();

        private bool hasLoaded = false;
        protected MemoryBasedFileRepository(string store, Func<T, TKey> getKey)
        {
            this.store = store;
            this.getKey = getKey;
        }

        public string BaseStorePath { get; set; } = "stores";

        private string StoreFolder => BaseStorePath + "/" + this.store + "/";

        public IReadOnlyList<T> All()
        {
            LoadRepository();
            return items.Values.ToList();
        }

        public void StoreMany(IEnumerable<T> newNodes)
        {
            LoadRepository();
            newNodes = newNodes.ToList();

            foreach (var n in newNodes)
            {
                var key = getKey(n);
                items[key] = n;
                Save(getKey(n), n);
            }
        }

        public void Store(TKey key, T val)
        {
            LoadRepository();
            items[key] = val;
            Save(key, val);
        }

        private void Save(TKey key, T node)
        {
            try
            {
                System.IO.File.WriteAllText(StoreFolder + key + ".json", JsonConvert.SerializeObject(node));
            }
            catch
            {
            }
        }

        public T Get(TKey id)
        {
            LoadRepository();
            return items.ContainsKey(id) ? items[id] : default(T);
        }

        private void LoadRepository()
        {
            if (hasLoaded)
            {
                return;
            }

            hasLoaded = true;

            if (!System.IO.Directory.Exists(StoreFolder))
            {
                System.IO.Directory.CreateDirectory(StoreFolder);
            }

            if (System.IO.File.Exists(StoreFolder + "all.json"))
            {
                var json = System.IO.File.ReadAllText(StoreFolder + "all.json");
                var loadedItems = JsonConvert.DeserializeObject<List<T>>(json);
                foreach (var item in loadedItems)
                {
                    this.items[getKey(item)] = item;
                }
                return;
            }
            else
            {
                var nodeFiles = System.IO.Directory.GetFiles(StoreFolder, "*.json");
                foreach (var n in nodeFiles)
                {
                    if (System.IO.Path.GetFileName(n) == "all.json")
                    {
                        continue;
                    }

                    try
                    {
                        var item = JsonConvert.DeserializeObject<T>(System.IO.File.ReadAllText(n));
                        this.items[getKey(item)] = item;
                    }
                    catch
                    {
                    }

                }
            }

            // to temporarily convert to the new format where we save 1 file for all.
            // convert to all.json so next time we load the data it will go super fast.
            try
            {
                var alljson = this.items.Values.ToList();
                System.IO.File.WriteAllText(StoreFolder + "all.json", JsonConvert.SerializeObject(alljson));
            }
            catch
            {
            }
        }
    }
}