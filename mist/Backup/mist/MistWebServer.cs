namespace Mist
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;

    // Start the service and browse to http://<machine_name>:<port>/Service1/help to view the service's generated help page
    // NOTE: By default, a new instance of the service is created for each call; change the InstanceContextMode to Single if you want
    // a single instance of the service to process all calls.	
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    // NOTE: If the service is renamed, remember to update the global.asax.cs file
    public class MistWebServer : Mist.Interface.IMistServer
    {

        /// <summary>
        /// This is the (temporary) state store for the service.
        /// </summary>
        private Dictionary<Guid,Blob> store = new Dictionary<Guid,Blob>();

        /// <summary>
        /// List all blobs in store.
        /// </summary>
        /// <returns>List of blob objects.  BUGBUG should be just names?</returns>
        public List<Blob> GetCollection()
        {
            List<Blob> rtnBlobs = new List<Blob>();
            foreach (Blob b in this.store.Values)
            {
                rtnBlobs.Add(b);
            }
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
                instance = new Blob();
            }

            // If the user gave us a new object, it does not an an id.
            // If we created the object, it does not have an id.
            if (null == instance.id)
            {
                instance.id = Guid.NewGuid();
            }

            Guid guid = instance.id;
            bool containsKey = this.store.ContainsKey(guid);
            if (containsKey)
            {
                // BUGBUG -- create will fail
                throw new InvalidOperationException(String.Format("Key {0} exists.", instance.id ));
            }

            this.store.Add(guid, instance);
            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringGuid"></param>
        /// <returns></returns>
        public Blob Get(String stringGuid)
        {
            string[] justTheGuid = stringGuid.Split(new char[] {'/'});
            try 
            {
                //Guid guid = new Guid(  stringGuid);
                Guid guid = new Guid(justTheGuid[justTheGuid.Length - 1]);
                if (this.store.ContainsKey(guid))
                {
                    return this.store[guid];
                }
            }
            catch  (Exception ex)
            {
                ArgumentOutOfRangeException newEx = new ArgumentOutOfRangeException(stringGuid, ex);
                throw newEx;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public Blob Update(String id, Blob instance)
        {
            WebOperationContext webContext = WebOperationContext.Current;

            Guid g = new Guid(id);
            if (this.store.ContainsKey(g))
            {
                this.store[g] = instance;
            }
            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void Delete(String id)
        {
            Guid g = new Guid(id);
            if (this.store.ContainsKey(g))
            {
                this.store.Remove(g);
            }
        }

        
        public void InternalUpsert(string id, Blob instance)
        {
            throw new NotImplementedException("InternalUpsert()");
        }
    }
}
