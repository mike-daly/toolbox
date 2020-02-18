namespace Mist
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;

    // using System.ServiceModel.Activation;
    // using System.ServiceModel.Web;

    // [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    // NOTE: If the service is renamed, remember to update the global.asax.cs file
    public class MistClass : Mist.Interface.IMistServer
    {
        MistStorage storage;

        public MistClass()
        {
            Console.WriteLine("MistClass() constructor.");
            this.storage = new MistStorage(System.IO.Path.GetTempPath(), 10);
        }

        public MistClass(MistStorage storage)
        {
            if (storage != null)
            {
                Console.WriteLine(string.Format("MistClass(storage({0}, {1}) constructor.", storage.storageRoot, storage.maxStorageInGB));
            }
            else
            {
                Console.WriteLine("MistClass(null) constructor -- bugbug.");
            }
            this.storage = storage;
        }

        /// <summary>
        /// This is the (temporary) state store for the service.
        /// </summary>
        private Dictionary<Guid, Blob> store = new Dictionary<Guid, Blob>();

        /// <summary>
        /// List all blobs in store.
        /// </summary>
        /// <returns>List of blob objects.  BUGBUG should be just names?</returns>
        public List<Blob> GetCollection()
        {
            Console.Write("GetCollection()");
            List<Blob> rtnBlobs = new List<Blob>();
            foreach (Blob b in this.store.Values)
            {
                rtnBlobs.Add(b);
            }
            Console.WriteLine(string.Format(" -- {0} blobs", rtnBlobs.Count));
            return rtnBlobs;
        }

        /// <summary>
        /// Creates a new entry in the store and returns the instance.
        /// </summary>
        /// <param name="instance">Initial data.</param>
        /// <returns>Initial data, updated with the ID (uri name).</returns>
        public Blob Create(Blob instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            Console.WriteLine(string.Format("Create() -- {0}", instance.id));
            if (instance == null)
            {
                instance = new Blob();
            }

            // If the user gave us a new object, it does not an an id.
            // If we created the object, it does not have an id.
            if (instance.id.Equals(Guid.Empty))
            {
                instance.id = Guid.NewGuid();
            }

            bool containsKey = this.store.ContainsKey(instance.id);
            if (containsKey)
            {
                // BUGBUG -- create will fail
                throw new InvalidOperationException(string.Format("Key {0} exists.", instance.id));
            }

            this.store.Add(instance.id, instance);

            ///---------
            this.storage.Write(instance.id, 1, instance.data);

            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringGuid"></param>
        /// <returns></returns>
        public Blob Get(string stringGuid)
        {
            Console.WriteLine(string.Format("Get() -- {0}", stringGuid));
            // split out the tokens in the string, the last is ID_VERSION
            // string[] justTheId = stringGuid.Split(new char[] {'/'});

            Blob returnBlob = new Blob();

            try
            {
                /*
                // Guid guid = new Guid( stringGuid);
                Guid guid = new Guid(justTheId[justTheId.Length - 1]);
                if (this.store.ContainsKey(guid))
                {
                    return this.store[guid];
                }
                */

                returnBlob.id = new Guid(stringGuid);
                returnBlob.data = this.storage.Read(stringGuid, 1);
            }
            catch (Exception ex)
            {
                ArgumentOutOfRangeException newEx = new ArgumentOutOfRangeException(stringGuid, ex);
                throw newEx;
            }
            return returnBlob;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public Blob Update(string id, Blob instance)
        {
            Console.WriteLine(string.Format("Update() -- {0}", id));
            // WebOperationContext webContext = WebOperationContext.Current;

            Guid g = new Guid(id);
            if (this.store.ContainsKey(g))
            {
                this.store[g] = instance;
            }

            this.storage.Write(g, 1, instance.data);

            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void Delete(string id)
        {
            Console.WriteLine(string.Format("Delete() -- {0}", id));
            Guid g = new Guid(id);
            if (this.store.ContainsKey(g))
            {
                this.store.Remove(g);
            }
        }

        public void InternalUpsert(string id, Blob instance)
        {
            Console.WriteLine(string.Format("InternalUpsert() -- {0}", id));
            throw new NotImplementedException("InternalUpsert()");
        }
    }
}
