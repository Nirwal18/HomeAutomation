using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Search;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HomeAutomation.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FileExpo : Page
    {
        public FileExpo()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            StorageLibrary music1 = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);


            List<StorageFolder> fl = new List<StorageFolder>();

            foreach (var folder in music1.Folders)
            {
                fl.Add(folder);
            }

            FolderListBox.ItemsSource = fl;



            StorageItemQueryResult f2 = fl[1].CreateItemQuery();

            List<IStorageItem> fl2 = new List<IStorageItem>();
            IReadOnlyList<IStorageItem> x= await f2.GetItemsAsync();

            int i = 0;
            foreach (var folder in x)
            {
              fl2.Add(x[i]);
                i++;
            }
            FileListBox.ItemsSource = fl2;
            //FileListBox.ItemsSource = lista;
        }

    }
}
