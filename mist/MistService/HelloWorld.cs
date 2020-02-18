
namespace Mist
{
    using System.ServiceModel;
    using System.ServiceModel.Web;

    [ServiceContract]
    public class HelloWorld
    {
        [WebGet(UriTemplate = "{str}")]
        string Get(string str)
        {
            return string.Format("Hi World ({0})", str);
        }
    }
}