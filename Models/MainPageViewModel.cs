using System.Collections.ObjectModel;
using Geotracker.Models;

namespace Geotracker.ViewModels
{
    public class MainPageViewModel
    {
        public ObservableCollection<Trail> Trails { get; set; }

        public MainPageViewModel()
        {
            Trails = new ObservableCollection<Trail>
            {
                new Trail { Name = "Forest Path", Location = "Blackwood", ImageUrl = "path.svg" },
                new Trail { Name = "Mountain Trail", Location = "Alpine Ridge", ImageUrl = "path.svg" },
                new Trail { Name = "River Walk", Location = "Blue River", ImageUrl = "path.svg" }
            };
        }
    }
}
