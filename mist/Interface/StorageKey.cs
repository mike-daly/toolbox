namespace Mist
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// This class is the key used to determine where a piece of data is stored.
    /// Each service instance manages a range of StorageKey values.
    ///
    /// The class provides a representation of the key and comparison operators for same.
    /// </summary>
    [DataContract(Namespace = "Mist")]
    public class StorageKey
    {
        public byte[] Key { get; set; }

        /// <summary>
        /// Standard compare function for StorageKeys, returning -1,0,1 as appropriate.  
        /// </summary>
        /// <param name="key1">First StorageKey to compare.</param>
        /// <param name="key2">Second StorageKey to compare.</param>
        /// <returns>Returns -1 when key1 comes before key2, +1 when key1 comes after key2, and 0 when equal.</returns>
        public static int Compare(StorageKey key1, StorageKey key2)
        {
            if (key1.Key.Length != key2.Key.Length)
            {
                throw new ArgumentException( "key sizes do not match.");
            }
            for (int i = 0; i < key1.Key.Length; i++)
            {
                if (key1.Key[i] != key2.Key[i])
                {
                    return key1.Key[i] < key2.Key[i] ? -1 : (key1.Key[i] > key2.Key[i] ? 1 : 0);
                }
            }
            return 0;
        }

    }
}
