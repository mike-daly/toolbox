using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Mist
{
    [DataContract(Namespace="Mist")]
    public class Blob
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public int checksum { get; set; }

        [DataMember]
        public string StringValue { get; set; }

        public override string ToString()
        {
            return String.Format("{0} ({1})", this.id, this.checksum);
        }
    }
}
