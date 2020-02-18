namespace Mist
{
    using System.ServiceModel;
    using System.ServiceModel.Web;

    [ServiceContract]
    public class Healthbeat
    {
        [WebGet()]
        string Get()
        {
            return string.Format("Healthbeat!");
        }
    }
}
