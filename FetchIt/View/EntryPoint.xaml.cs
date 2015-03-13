using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FetchItClassLib.Persistence.EF;
using FetchItClassLib.Profile;

namespace FetchIt.View
{
    /// <summary>
    /// Interaction logic for EntryPoint.xaml
    /// </summary>
    public partial class EntryPoint : Window
    {
        private List<ProfileModel> allProfiles;

        public EntryPoint()
        {
            InitializeComponent();
            allProfiles = new List<ProfileModel>();

            allProfiles.AddRange(ProfileHandler.GetAllProfiles());
            label_test.Content = allProfiles[0].ProfileName;
        }


    }
}
