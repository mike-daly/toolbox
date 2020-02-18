using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mist.Interface;

namespace Mist
{
    public class MistServerInstance : IMistServer
    {
        public List<Blob> GetCollection()
        {
            Console.WriteLine(String.Format("GetCollection()"));
            throw new NotImplementedException("GetCollection()");
        }

        public Blob Create(Blob instance)
        {
            Console.WriteLine(String.Format("Create({0})", instance.id));
            throw new NotImplementedException("Create()");
        }
        
        public Blob Get(string stringGuid)
        {
            Console.WriteLine(String.Format("Get({0})", stringGuid));
            throw new NotImplementedException("Get()");
        }
        
        public Blob Update(string id, Blob instance)
        {
            Console.WriteLine(String.Format("Update({0}, blob)", id));
            throw new NotImplementedException("Update()");
        }
        
        public void Delete(string id)
        {
            Console.WriteLine(String.Format("Delete({0})", id));
            throw new NotImplementedException("Delete()");
        }
        
        public void InternalUpsert(string id, Blob instance)
        {
            Console.WriteLine(String.Format("InternalUpsert({0})", id));
            throw new NotImplementedException("InternalUpsert()");
        }
    }
}
