using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Package_Decrypter_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string defLocation;
        private Process pkgDecProcess;

        public MainWindow()
        {
            defLocation = AppDomain.CurrentDomain.BaseDirectory;
            if(!CheckForTool())
            {
                MessageBox.Show("Please ensure pkg_dec.exe is in the same directory as this tool.", "Critical Error");
                App.Current.Shutdown();
            }

            InitializeComponent();

        }

        private void BrowseForDestination(object sender ,RoutedEventArgs e )
        {
            txtOutputPath.Text = BrowseForDest();
        }

        private void BrowseForPackage(object sender, RoutedEventArgs e)
        {
            txtPkgName.Text = BrowseForPkg();
        }

        private void ExtractPackage(object sender, RoutedEventArgs e)
        {
            ExtractPkg();
        }

        private void ExtractPkg()
        {
            var filename = txtPkgName.Text;
            var outputdir = txtOutputPath.Text;
            var key = txtKey.Text;

            pkgDecProcess = new Process();
            pkgDecProcess.EnableRaisingEvents = true;
            pkgDecProcess.Exited += PkgDecProcess_Exited;

            pkgDecProcess.StartInfo.FileName = defLocation + "\\pkg_dec.exe";
            pkgDecProcess.StartInfo.UseShellExecute = false;
            pkgDecProcess.StartInfo.CreateNoWindow = true;
            pkgDecProcess.StartInfo.Arguments = string.Format("--make-dirs=ux --license={2} {0} {1}", filename, outputdir,key);
            
            pkgDecProcess.Start();
            this.TaskbarItemInfo = new System.Windows.Shell.TaskbarItemInfo
            {
                ProgressState = System.Windows.Shell.TaskbarItemProgressState.Indeterminate
            };

        }

        private void PkgDecProcess_Exited(object sender, EventArgs e)
        {
            if (pkgDecProcess.ExitCode == 0)
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    this.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
                }));
            }
            else
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    this.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Error;
                }));
                MessageBox.Show("Something went wrong, please try again.", "Error");
            }
           
            pkgDecProcess = null;
        }

        private string BrowseForDest()
        {
            CommonOpenFileDialog openFolderDialog = new CommonOpenFileDialog();
            openFolderDialog.IsFolderPicker = true;

            var result = openFolderDialog.ShowDialog();

            if(result == CommonFileDialogResult.Ok)
            {
                return openFolderDialog.FileName;
            }

            return null;
           
        }

        private string BrowseForPkg()
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog();

            openFileDialog.Filters.Add(new CommonFileDialogFilter("Package Files", ".pkg"));

            var result = openFileDialog.ShowDialog();
          
            if(result == CommonFileDialogResult.Ok)
            {
                return openFileDialog.FileName;
            }

            return null;
        }


        private bool CheckForTool()
        {
            if(!File.Exists(defLocation + "\\pkg_dec.exe"))
            {
                return false;
            }

            return true;

        }

    }
}
