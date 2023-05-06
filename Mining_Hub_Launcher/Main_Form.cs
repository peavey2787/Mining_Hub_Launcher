using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using RadioButton = System.Windows.Forms.RadioButton;

namespace Mining_Hub_Launcher
{
    public partial class Main_Form : Form
    {
        string Server_File { get; set; }
        string Viewer_File { get; set; }        

        public Main_Form()
        {
            InitializeComponent();

            Task.Run(() =>
            {
                Server_File = File_Exists("Mining_Hub_Server.exe");
                Viewer_File = File_Exists("Mining_Hub_Viewer.exe");

                bool ready = false;
                int retries = 5;
                do
                {
                    ready = Ensure_App_Files_Exist();
                    retries--;
                    Thread.Sleep(100);
                } while (!ready && retries > 0);

                if (!ready) MessageBox.Show("Unable to get app files, please try again");
            });
        }

        void Main_Form_Load(object sender, EventArgs e)
        {
            // Defaults
            Updates_ComboBox.SelectedItem = "On App Start";

            // Load settings
            foreach (RadioButton radio_button in App_Selection_GroupBox.Controls)
            {
                if (bool.TryParse(App_Settings.Load(radio_button.Name), out var check_val))
                    radio_button.Checked = check_val;
            }
            
            if (bool.TryParse(App_Settings.Load(Auto_Start_CheckBox.Name), out var val))
                Auto_Start_CheckBox.Checked = val;

            var updates = App_Settings.Load(Updates_ComboBox.Name);
            if (updates != null && updates.Length > 0)
                Updates_ComboBox.Text = updates;

            // Auto start
            if (Auto_Start_CheckBox.Checked)
                Launch_App();
        }
        void Main_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save settings
            foreach(RadioButton radio_button in App_Selection_GroupBox.Controls)
            {
                App_Settings.Save(radio_button.Name, radio_button.Checked.ToString());
            }
            App_Settings.Save(Auto_Start_CheckBox.Name, Auto_Start_CheckBox.Checked.ToString());
            App_Settings.Save(Updates_ComboBox.Name, Updates_ComboBox.Text);
        }
        void Start_Button_Click(object sender, EventArgs e)
        {
            Launch_App();
        }
        void Launch_App()
        {
            RadioButton selectedRadioButton = App_Selection_GroupBox.Controls
                .OfType<RadioButton>()
                .FirstOrDefault(rb => rb.Checked);

            if (selectedRadioButton == null) return;

            if (selectedRadioButton.Name.Contains("Both"))
            {
                Start_Executable(Server_File);
                Start_Executable(Viewer_File);
            }
            else if (selectedRadioButton.Name.Contains("Work"))
            {
                Start_Executable(Server_File);
            }
            else if (selectedRadioButton.Name.Contains("View"))
            {
                Start_Executable(Viewer_File);
            } 
        }

        bool Ensure_App_Files_Exist()
        {
            var error = false;
            string currentDir = Directory.GetCurrentDirectory();

            // Check Server folder
            string serverDir = Path.Combine(currentDir, "Mining_Hub_Server");
            if (!Directory.Exists(serverDir))
            {
                Directory.CreateDirectory(serverDir);
            }

            // Check Viewer folder
            string viewerDir = Path.Combine(currentDir, "Mining_Hub_Viewer");
            if (!Directory.Exists(viewerDir))
            {
                Directory.CreateDirectory(viewerDir);
            }

            // Check Server file
            if (!File.Exists(Server_File))
            {
                error = !Download_And_Unzip("Mining_Hub_Server.zip");
                if (error) return false;
                Server_File = File_Exists("Mining_Hub_Server.exe");
            }

            // Check Viewer file
            if (!File.Exists(Viewer_File))
            {
                error = !Download_And_Unzip("Mining_Hub_Viewer.zip");
                if (error) return false;
                Viewer_File = File_Exists("Mining_Hub_Viewer.exe");
            }

            return true;
        }
        public static bool Start_Executable(string file)
        {
            if (file == null || file.Length == 0) return false;

            // Check if a process is already running for executable.exe
            Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(file));
            if (processes.Length > 0)
            {
                return true; // Already running
            }

            try
            {
                Process.Start(file);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting {file}: {ex.Message}");
                return false;
            }
        }
        bool Download_And_Unzip(string filename)
        {
            string url = "http://thebox.loseyourip.com:8080/updates/" + filename;
            string folder = Directory.GetCurrentDirectory() + $"\\{Path.GetFileNameWithoutExtension(filename)}\\";

            // Delete old files
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);                
            }

            // Create folder
            Directory.CreateDirectory(folder);

            // Download file
            string zipPath = Path.Combine(folder, "update.zip");
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile(url, zipPath);
                }
                catch (Exception exc)
                {
                    MessageBox.Show($"Download Failed - {filename} from {url}");
                    return false;
                }
            }

            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string outputPath = Path.Combine(folder, entry.FullName);

                    if (entry.FullName.EndsWith("/"))
                    {
                        // If the entry is a directory, create it in the output folder
                        Directory.CreateDirectory(outputPath);
                    }
                    else
                    {
                        // If the entry is a file, extract it to the output folder
                        entry.ExtractToFile(outputPath, true);
                    }
                }
            }

            File.Delete(zipPath);

            return true;
        }
        string File_Exists(string fileName)
        {
            return SearchFileRecursive(new DirectoryInfo(Environment.CurrentDirectory), fileName);
        }
        string SearchFileRecursive(DirectoryInfo directory, string fileName)
        {
            try
            {
                foreach (var file in directory.GetFiles(fileName))
                {
                    if ((File.GetAttributes(file.FullName) & FileAttributes.Hidden) == 0)
                    {
                        return file.FullName;
                    }
                }
            }
            catch (Exception)
            {
            }

            try
            {
                foreach (var subDir in directory.GetDirectories())
                {
                    string filePath = SearchFileRecursive(subDir, fileName);
                    if (filePath.Length > 0)
                    {
                        return filePath;
                    }
                }
            }
            catch (Exception)
            {
            }

            return "";
        }
    }
}
