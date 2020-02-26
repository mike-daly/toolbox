namespace mist
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;

    [ServiceContract]
    public interface IMistServer
    {
        /// <summary>
        /// List all blobs in store.
        /// </summary>
        /// <returns>List of blob objects.  BUGBUG should be just names?</returns>
        [WebGet(UriTemplate = "", ResponseFormat = WebMessageFormat.Xml)]
        List<Blob> GetCollection();

        /// <summary>
        /// Creates a new entry in the store and returns the instance.
        /// </summary>
        /// <param name="instance">Initial data.</param>
        /// <returns>Initial data, updated with the ID (uri name).</returns>
        [WebInvoke(UriTemplate = "", Method = "POST")]
        Blob Create(Blob instance);
        
        [WebGet(UriTemplate = "{stringGuid}")]
        Blob Get(string stringGuid);
        
        [WebInvoke(UriTemplate = "{id}", Method = "POST")]
        Blob Update(string id, Blob instance);
        
        [WebInvoke(UriTemplate = "{id}", Method = "DELETE")]
        void Delete(string id);

    }
}
