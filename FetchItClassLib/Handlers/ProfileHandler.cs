using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FetchItClassLib.Persistence.EF;

namespace FetchItClassLib.Profile
{
    static public class ProfileHandler
    {
        static public List<ProfileModel> GetAllProfiles()
        {
            using (FetchItDatabaseEntities3 dbConn = new FetchItDatabaseEntities3())
            {
                return dbConn.ProfileModels.ToList();
            }
        }
    }
}
