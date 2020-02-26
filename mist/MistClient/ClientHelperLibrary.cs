
namespace MistClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Runtime.Serialization;
    using Mist;
    using Mist.Interface;

    /// <summary>
    /// Client library for test purposes.
    /// </summary>
    public class MistHelper : IMistServer
    {
        public MistHelper(string baseUri)
        {
            this.baseUri = baseUri;
        }

        private string baseUri
        {
            get;
            set;
        }

        /// <summary>
        /// Creates, saves, and returns a new blob.  
        /// </summary>
        /// <returns>Returns the created blob.</returns>
        public Blob Create(Blob instance)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.baseUri + '/');
            request.Method = WebRequestMethods.Http.Post;
            request.ContentLength = 0;
            request.ContentType = "text/xml";

            MemoryStream memStream = new MemoryStream();
            DataContractSerializer ser = new DataContractSerializer(typeof(Blob), new Type[] { typeof(Blob) });

            // bounce the blob through a MemoryStream to get the serialized length (must be better way)
            ser.WriteObject(memStream, instance);
            request.ContentLength = memStream.Length;
            memStream.Seek(0, SeekOrigin.Begin);
            memStream.CopyTo(request.GetRequestStream());

            Blob returnBlob;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader streamReaderResponse = new StreamReader(response.GetResponseStream()))
                {
                    string streamResponse = streamReaderResponse.ReadToEnd();
                    returnBlob = (Blob)Deserialize(streamResponse, typeof(Blob), new Type[] { typeof(Blob) });
                }
            }
            return returnBlob;
        }

        /// <summary>
        /// Gets the named blob from the store.
        /// </summary>
        /// <param name="Uri">Name of the blob.</param>
        /// <returns></returns>
        public Blob Get(string stringGuid)
        {
            Blob returnBlob;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.baseUri + '/' + stringGuid);
            request.Method = WebRequestMethods.Http.Get;
            request.ContentLength = 0;
            request.ContentType = "text/xml";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader streamReaderResponse = new StreamReader(response.GetResponseStream()))
                {
                    string streamResponse = streamReaderResponse.ReadToEnd();
                    returnBlob = (Blob)Deserialize(streamResponse, typeof(Blob), new Type[] { typeof(Blob) });
                }
            }
            return returnBlob;
        }

        /// <summary>
        /// Updates the persistent copy of the given Blob.
        /// </summary>
        /// <param name="blob">New data to write to the store.</param>
        public Blob Update(string id, Blob instance)
        {
            Blob returnBlob;
            string updateUri = this.baseUri + '/' + id;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(updateUri);

            request.ContentType = "text/xml";
            request.Method = WebRequestMethods.Http.Post;
            request.Accept = "application/1.0+xml";

            MemoryStream memStream = new MemoryStream();
            DataContractSerializer ser = new DataContractSerializer(typeof(Blob), new Type[] { typeof(Blob) });
            // ser.WriteObject( request.GetRequestStream(), instance );

            // bounce the blob through a MemoryStream to get the serialized length (must be better way)
            ser.WriteObject(memStream, instance);
            request.ContentLength = memStream.Length;
            memStream.Seek(0, SeekOrigin.Begin);
            memStream.CopyTo(request.GetRequestStream());

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader streamReaderResponse = new StreamReader(response.GetResponseStream()))
                {
                    string streamResponse = streamReaderResponse.ReadToEnd();
                    returnBlob = (Blob)Deserialize(streamResponse, typeof(Blob), new Type[] { typeof(Blob) });
                }
            }
            return returnBlob;
        }

        /// <summary>
        /// Delete the object specified.
        /// </summary>
        /// <param name="uri">Uri to the object to delete.</param>
        public void Delete(string uri)
        {
            string deleteUri = baseUri + '/' + uri;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(deleteUri);
            request.Method = "DELETE";
            request.ContentLength = 0;
            request.ContentType = "text/xml";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new InvalidOperationException(string.Format(
                        "Delete() failed.  StatusCode: {0}",
                        response.StatusCode));
                }
            }
        }

        /// <summary>
        /// Get and display the list of blobs in the service.
        /// </summary>
        // public List<Blob> ListBlobs()
        public List<Blob> GetCollection()
        {
            List<Blob> returnBlobs;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.baseUri);
            request.Method = WebRequestMethods.Http.Get;
            request.ContentLength = 0;
            request.ContentType = "text/xml";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader streamReaderResponse = new StreamReader(response.GetResponseStream()))
                {
                    string streamResponse = streamReaderResponse.ReadToEnd();
                    returnBlobs = (List<Blob>)Deserialize(streamResponse, typeof(List<Blob>), new Type[] { typeof(List<Blob>) });
                }
            }
            return returnBlobs;
        }

        /// <summary>
        /// Internal function for non-replicated up-sert.
        /// </summary>
        /// <param name="blob">New data to write to the store.</param>
        public void InternalUpsert(string id, Mist.Blob instance)
        {
            throw new NotImplementedException("InternalUpdate() -- internal only.");
        }   

        /// <summary>
        /// Serializes an instance into an Xml string.
        /// </summary>
        /// <param name="x">Any isntance.</param>
        /// <param name="knownTypes">other types which x might use (not base ones).</param>
        /// <returns>Xml string.</returns>
        public static string Serialize(object x, params Type[] knownTypes)
        {
            DataContractSerializer ser = new DataContractSerializer(x.GetType(), knownTypes);
            MemoryStream stream = new MemoryStream();
            ser.WriteObject(stream, x);
            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);
            string xml = reader.ReadToEnd();
            return xml;
        }

        /// <summary>
        /// Deserializes a class instance from Xml string.
        /// </summary>
        /// <param name="xml">Xml string with the data.</param>
        /// <param name="t">Type of the object we want to deserialize.</param>
        /// <param name="knownTypes">other types which the object might use (not base ones).</param>
        /// <returns></returns>
        public static object Deserialize(string xml, Type t, params Type[] knownTypes)
        {
            DataContractSerializer ser = new DataContractSerializer(t, knownTypes);
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(xml);
                writer.Flush();
                stream.Position = 0;
                return ser.ReadObject(stream);
            }
        }
    }
}