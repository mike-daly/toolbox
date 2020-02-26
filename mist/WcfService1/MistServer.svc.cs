using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Mist.Interface;

namespace Mist
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MistServer" in code, svc and config file together.
    public class MistWcfServer : IMistServer
    {
        /// <summary>
        /// List all blobs in store.
        /// </summary>
        /// <returns>List of blob objects.  BUGBUG should be just names?</returns>
        public List<Blob> GetCollection()
        { 
            throw new NotImplementedException("GetCollection");
        }

        /// <summary>
        /// Creates a new entry in the store and returns the instance.
        /// </summary>
        /// <param name="instance">Initial data.</param>
        /// <returns>Initial data, updated with the ID (uri name).</returns>
        public Blob Create(Blob instance)
        {
            throw new NotImplementedException("Create()");
        }
        
        public Blob Get(string stringGuid)
        {
            throw new NotImplementedException("Get()");
        }
        
        public Blob Update(string id, Blob instance)
        {
            throw new NotImplementedException("Update()");
        }
        
        public void Delete(string id)
        {
            throw new NotImplementedException("Delete()");
        }
        
        public void InternalUpsert(string id, Blob instance)
        {
            throw new NotImplementedException("InternalUpsert()");
        }

        /*
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
        */
    }
}
