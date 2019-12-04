using static AServer.Enviroment;
namespace AServer
{
    public enum Enviroment
    {
        Dev,
        Production
    }

    public class Config
    {
        public static Enviroment Env = Enviroment.Dev;

        public static string ConfigPath
        {
            get
            {
                switch (Env)
                {
                    case Dev:
                        return devConfigPath;
                    case Production:
                        return prodConfigPath;
                }
                return null;
            }
        }

        private const string devConfigPath = @"F:\Unity\StudyProject\ARPG\Assets\Resources\ResCfgs\";
        private const string prodConfigPath = "/root/netcoreapps/Cfgs/arpgXml/";
    }
}