using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfService1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMistStorage" in both code and config file together.
    [ServiceContract]
    public interface IMistStorage
    {

        [OperationContract]
        string GetData(int value);

        /*
        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);
        */

        // TODO: Add your service operations here
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.

    [DataContract(Namespace="mist")]
    public class Blob
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public int checksum { get; set; }

        [DataMember]
        public string StringValue { get; set; }

    }
}
