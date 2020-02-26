using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace mist
{
    //[DataContract(Namespace="mist")]
    public partial class Blob
    {
        //[DataMember]
        public string id { get; set; }

        //[DataMember]
        public int checksum { get; set; }

        //[DataMember]
        public string StringValue { get; set; }
    }
}
