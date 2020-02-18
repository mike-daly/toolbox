namespace Mist
{
    using System;
    using System.IO;
    using System.Numerics;
    using System.Runtime.Serialization;
    using System.Security.Cryptography;

    [DataContract(Namespace = "Mist")]
    public partial class Blob
    {
        [DataMember]
        // public string id  {get;set;}
        public Guid id { get; set; }

        [DataMember]
        public byte[] checksum { get; set; }

        [DataMember]
        public string version { get; set; }            // vector clock versioning?

        [DataMember]
        public byte[] data { get; set; }

        public byte[] ComputeChecksum()
        {
            // use MD5 because it is 128 bits so a bit easier to handle
            MD5 hashEngine = MD5.Create();
            return hashEngine.ComputeHash(this.data);
        }

        /// <summary>
        /// Returns the array of keys which indentify the places where this object should be stored.
        /// The number of keys is the same as the number of replicas to write.
        /// 
        /// The key generation algorithm is critical to insure that data is well dispersed.  
        /// 
        /// The key space is divided int *replicaCount* sub-spaces.  
        /// A key value is computed for each replica as a hash(id, replica);
        /// the key values are adjusted by adding appropriate base addresses for each range.
        /// 
        /// A more sophisticated, geographically aware algorithm can be substituted here as long as it returns
        /// a set of keys that interop.
        /// </summary>
        /// <returns>Returns StorageKeys[].</returns>
        public StorageKey[] GetKeys()
        {
            MD5 hashEngine = MD5.Create();

            const int replicaCount = 3;
            const int keySizeBytes = 16;
            BigInteger maxKey = BigInteger.Pow(Byte.MaxValue, keySizeBytes);
            BigInteger maxKeySegment = maxKey / replicaCount - 1;

            int keySize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Guid));
            const int replicaSize = 2;     // a replica field in bytes

            // the buffer is two parts, the first part contains the object ID, the second contains the replica
            // number.  stuff the key and the replica id in there, compute the hash, and mask and offset the results.
            byte[] buffer = new byte[keySize + replicaSize];
            BigInteger[] intKeys = new BigInteger[replicaCount];

            using (MemoryStream ms = new MemoryStream(buffer))
            {

                // compute the key value inside each segment (hash the name+repl, mask, add offset)
                // BigInteger math slow? BUGBUG
                for (int i = 0; i < replicaCount; i++)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Write(this.id.ToByteArray(), 0, keySize);
                    ms.Write(new byte[] { 0, (byte)i }, keySize, replicaSize);

                    intKeys[i] = new BigInteger(hashEngine.ComputeHash(buffer));    // get a hash
                    intKeys[i] /= replicaCount;                                     // size it for segment
                    intKeys[i] += (maxKeySegment * i);                              // move to right segment
                }
            }

            StorageKey[] returnKeys = new StorageKey[replicaCount];
            // move the secondary replicas to smaller ranges
            for (int i = 0; i < replicaCount; i++)
            {
                returnKeys[i] = new StorageKey();
                returnKeys[i].Key = intKeys[i].ToByteArray();
            }

            return returnKeys;
        }


        public override string ToString()
        {
            return string.Format("{0} ({1})", this.id, this.checksum);
        }
    }
}
