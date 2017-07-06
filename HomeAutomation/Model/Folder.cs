using System;
using Windows.System;
using Windows.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAutomation.Model
{
    class Folder
    {
        public List<Folder> ChildFolders { get; set; }

        public string Name
        {
            get;
            set;
        }

        public string FolderIcon { get; set; }
    }

}
