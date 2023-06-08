using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using RadioButton = System.Windows.Forms.RadioButton;
using Task = System.Threading.Tasks.Task;
using Timer = System.Windows.Forms.Timer;

namespace Mining_Hub_Launcher
{
    public partial class Main_Form : Form
    {
        NotifyIcon notify_icon;
        string Server_File_Name { get; set; }
        string Server_File_Path { get; set; }
        string Viewer_File_Name { get; set; }
        string Viewer_File_Path { get; set; }
        bool Resetting = false;
        bool StartingUp = true;
        bool IsClosing = false;
        Timer Update_Timer { get; set; }
        private void Timer_Elapsed(object sender, EventArgs e)
        {
            ((Timer)sender).Stop();

            // Update apps
            Download_And_Unzip(Path.GetFileNameWithoutExtension(Server_File_Name) + ".zip");
            Download_And_Unzip(Path.GetFileNameWithoutExtension(Viewer_File_Name) + ".zip");

            ((Timer)sender).Start();
        }


        // Start/Stop
        public Main_Form()
        {
            InitializeComponent();
            CreateNotifyIcon();

            Task.Run(() =>
            {
                Server_File_Name = "Mining_Hub_Server.exe";
                Viewer_File_Name = "Mining_Hub_Viewer.exe";
                Server_File_Path = Get_File_Name_Path(Server_File_Name);
                Viewer_File_Path = Get_File_Name_Path(Viewer_File_Name);

                Ensure_Defender_Exclusions_Exist();

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
            Update_Timer = new Timer();
            Update_Timer.Tick += Timer_Elapsed;

            // Load settings
            foreach (RadioButton radio_button in App_Selection_GroupBox.Controls)
            {
                if (bool.TryParse(App_Settings.Load(radio_button.Name), out var check_val))
                    radio_button.Checked = check_val;
            }

            if (bool.TryParse(App_Settings.Load(Auto_Start_CheckBox.Name), out var val))
                Auto_Start_CheckBox.Checked = val;

            if (bool.TryParse(App_Settings.Load(Auto_Start_Win_CheckBox.Name), out var win_val))
                Auto_Start_Win_CheckBox.Checked = win_val;

            var updates = App_Settings.Load(Updates_ComboBox.Name);
            if (updates != null && updates.Length > 0)
                Updates_ComboBox.Text = updates;
        }
        private void Main_Form_Shown(object sender, EventArgs e)
        {
            // Auto start
            if (Auto_Start_CheckBox.Checked)
            {
                WindowHelper.HideWindow(this.Handle);
                Task.Run(() => { Launch_App(); });
            }

            StartingUp = false;
        }
        void Main_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsClosing)
            {
                e.Cancel = true;  // Cancel the closing action
                this.Hide();      // Hide the form
            }
            else
                SaveSettings();
        }



        // User Actions
        void Start_Button_Click(object sender, EventArgs e)
        {
            Launch_App();
        }
        private void Updates_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (StartingUp) return;
            StartTimer(Updates_ComboBox.Text);
            SaveSettings();
        }
        private void Main_Form_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }
        private void Auto_Start_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (StartingUp) return;
            SaveSettings();
        }
        private void Auto_Start_Win_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (StartingUp) return;

            string fullPath = Application.ExecutablePath;
            string name = Path.GetFileNameWithoutExtension(fullPath);            

            if (Auto_Start_CheckBox.Checked)
            {
                bool success = CreateSchedulerTask(name, fullPath);
                if (!success)
                {
                    notify_icon.ShowBalloonTip(3000, "Mining Hub", "Unable to add auto start to Windows task scheduler", ToolTipIcon.Info);
                    Auto_Start_CheckBox.Checked = false;
                }
                else
                    SaveSettings();
            }
            else
            {
                bool success = CreateSchedulerTask(name, fullPath, true);
                if (!success)
                {
                    notify_icon.ShowBalloonTip(3000, "Mining Hub", "Unable to delete auto start from Windows task scheduler", ToolTipIcon.Info);
                    Auto_Start_CheckBox.Checked = true;
                }
                else
                    SaveSettings();
            }
        }



        // Notify Icon
        #region
        private void CreateNotifyIcon()
        {
            // create the notify icon
            notify_icon = new NotifyIcon();
            notify_icon.Icon = Properties.Resources.picture; 
            notify_icon.Text = "Mining Hub"; // set the tooltip text
            notify_icon.Visible = true;

            // add a context menu to the notify icon
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Launch Worker", null, OnWorker);
            contextMenu.Items.Add("Launch Viewer", null, OnViewer);
            contextMenu.Items.Add("Launch Both", null, OnBoth);
            contextMenu.Items.Add("Update", null, OnUpdate);
            contextMenu.Items.Add("Settings", null, OnShow);
            contextMenu.Items.Add("Exit Launcher", null, OnExit);
            contextMenu.Items.Add("Exit All", null, OnExitAll);

            notify_icon.ContextMenuStrip = contextMenu;
        }

        private void OnBoth(object sender, EventArgs e)
        {
            Start_Executable(Server_File_Path);
            Start_Executable(Viewer_File_Path);
        }

        private void OnViewer(object sender, EventArgs e)
        {
            Start_Executable(Viewer_File_Path);
        }

        private void OnWorker(object sender, EventArgs e)
        {
            Start_Executable(Server_File_Path);
        }

        private void OnUpdate(object sender, EventArgs e)
        {
            string message = "";
            
            if (Download_And_Unzip(Path.GetFileNameWithoutExtension(Server_File_Name) + ".zip"))
                message += "Server Updated Successfully";
            else
                message += "Server Was Not Updated!";

            if (Download_And_Unzip(Path.GetFileNameWithoutExtension(Viewer_File_Name) + ".zip"))
                message += "\nViewer Updated Successfully";
            else
                message += "\nViewer Was Not Updated!";

            notify_icon.ShowBalloonTip(3000, "Mining Hub", message, ToolTipIcon.Info);
        }

        private void OnShow(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            WindowHelper.BringToFront(this.Handle);
        }

        private void OnExit(object sender, EventArgs e)
        {
            IsClosing = true;
            Application.Exit();
        }
        
        private void OnExitAll(object sender, EventArgs e)
        {
            bool reopen_server = Exit_App(Server_File_Name);
            bool reopen_viewer = Exit_App(Viewer_File_Name);
            IsClosing = true;
            Application.Exit();
        }
        #endregion



        // Utility
        void SaveSettings()
        {
            // Save settings
            foreach (RadioButton radio_button in App_Selection_GroupBox.Controls)
            {
                App_Settings.Save(radio_button.Name, radio_button.Checked.ToString());
            }
            App_Settings.Save(Auto_Start_CheckBox.Name, Auto_Start_CheckBox.Checked.ToString());
            App_Settings.Save(Auto_Start_Win_CheckBox.Name, Auto_Start_Win_CheckBox.Checked.ToString());
            App_Settings.Save(Updates_ComboBox.Name, Updates_ComboBox.Text);
        }
        void Launch_App()
        {
            RadioButton selectedRadioButton = App_Selection_GroupBox.Controls
                .OfType<RadioButton>()
                .FirstOrDefault(rb => rb.Checked);

            if (selectedRadioButton == null) return;

            if (selectedRadioButton.Name.Contains("Both"))
            {
                Start_Executable(Server_File_Path);
                Start_Executable(Viewer_File_Path);
            }
            else if (selectedRadioButton.Name.Contains("Work"))
            {
                Start_Executable(Server_File_Path);
            }
            else if (selectedRadioButton.Name.Contains("View"))
            {
                Start_Executable(Viewer_File_Path);
            }
        }
        void StartTimer(string update_interval)
        {
            Update_Timer.Stop();

            TimeSpan interval = TimeSpan.Zero;

            if (update_interval == "On App Start")
            {
                Download_And_Unzip(Path.GetFileNameWithoutExtension(Server_File_Name) + ".zip");
                Download_And_Unzip(Path.GetFileNameWithoutExtension(Viewer_File_Name) + ".zip");
                return;
            }
            else if (update_interval == "Once A Day")
                interval = TimeSpan.FromDays(1);
            else if (update_interval == "Once A Week")
                interval = TimeSpan.FromDays(7);
            else if (update_interval == "Once A Month")
                interval = TimeSpan.FromDays(30);
            else if (update_interval == "Never")
                return;

            Update_Timer.Interval = (int)interval.TotalMilliseconds;
            Update_Timer.Start();
        }
        bool Ensure_App_Files_Exist()
        {
            var error = false;
            string currentDir = Directory.GetCurrentDirectory();

            // Check Server folder
            string serverDir = Path.Combine(currentDir, Path.GetFileNameWithoutExtension(Server_File_Name));
            if (!Directory.Exists(serverDir))
            {
                Directory.CreateDirectory(serverDir);
            }

            // Check Viewer folder
            string viewerDir = Path.Combine(currentDir, Path.GetFileNameWithoutExtension(Viewer_File_Name));
            if (!Directory.Exists(viewerDir))
            {
                Directory.CreateDirectory(viewerDir);
            }

            // Check Server file
            if (!File.Exists(Server_File_Path))
            {
                error = !Download_And_Unzip(Path.GetFileNameWithoutExtension(Server_File_Name) + ".zip");
                if (error) return false;
                Server_File_Path = Get_File_Name_Path(Server_File_Name);
            }

            // Check Viewer file
            if (!File.Exists(Viewer_File_Path))
            {
                error = !Download_And_Unzip(Path.GetFileNameWithoutExtension(Viewer_File_Name) + ".zip");
                if (error) return false;
                Viewer_File_Path = Get_File_Name_Path(Viewer_File_Name);
            }

            return true;
        }       
        bool Ensure_Defender_Exclusions_Exist()
        {
            // Ensure app is added to defender exclusion list
            bool exclusionAdded = AddWindowsDefenderExclusion(Server_File_Path);

            if (!exclusionAdded)
            {
                notify_icon.ShowBalloonTip(3000, "Mining Hub", "Failed to add Server App to windows defender exclusion list, app may run slower than normal and get terminated randomly by windows!", ToolTipIcon.Info);
                return false;
            }

            return true;
        }
        bool Start_Executable(string file)
        {
            if (file == null || file.Length == 0) return false;

            // Check if a process is already running for executable.exe
            Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(file));
            if (processes.Length > 0)
            {
                if (processes[0].MainWindowHandle != IntPtr.Zero)
                {
                    // For WinForms app
                    WindowHelper.BringToFront(processes[0].MainWindowHandle);                    
                }
                else
                {
                    // For console app
                    string windowTitle = file;

                    if (file == Server_File_Name)
                        windowTitle = Server_File_Path;
                    else if(file == Viewer_File_Name)
                        windowTitle = Viewer_File_Path;

                    WindowHelper.BringToFront(windowTitle);
                }

                return true; // Already running
            }

            try
            {
                Process.Start(file);
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Error starting {file}: {ex.Message}");
                return false;
            }
        }
        bool Download_And_Unzip(string filename)
        {
            bool reopen_server = Exit_App(Server_File_Name);
            bool reopen_viewer = Exit_App(Viewer_File_Name);

            string url = "http://thebox.loseyourip.com:8080/updates/" + filename;
            string folder = Directory.GetCurrentDirectory() + $"\\{Path.GetFileNameWithoutExtension(filename)}\\";

            // Delete old files (except configs)
            if (Directory.Exists(folder))
            {
                int retries = 4;
                while (retries-- > 0)
                {
                    try
                    {
                        foreach (string file in Directory.GetFiles(folder))
                        {
                            if (!file.EndsWith(".config"))
                            {
                                File.Delete(file);
                            }
                        }
                        foreach (string dir in Directory.GetDirectories(folder))
                        {
                            Directory.Delete(dir, true);
                        }
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(1000);
                    }
                }
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

                    // Skip the config file to keep settings
                    if (entry.FullName.EndsWith(".config") && File.Exists(outputPath))
                        continue;

                    // Skip miners
                    if (entry.FullName.Contains("/miners/"))
                        continue;

                    if (entry.FullName.EndsWith("/") || entry.FullName.EndsWith("\\"))
                    {
                        // If the entry is a directory, create it in the output folder
                        Directory.CreateDirectory(outputPath);
                    }
                    else
                    {
                        // Ensure the parent directory exists before extracting the file
                        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                        entry.ExtractToFile(outputPath, true);
                    }
                }
            }


            File.Delete(zipPath);

            if (reopen_server)
                Start_Executable(Server_File_Path);
            if (reopen_viewer)
                Start_Executable(Viewer_File_Path);

            return true;
        }
        static bool Exit_App(string filename)
        {
            // Close apps if they are open
            Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(filename));
            if (processes.Length > 0)
            {
                processes[0].Kill();

                // Use PowerShell to find and kill the associated cmd window
                string command = $"Get-WmiObject Win32_Process | Where-Object {{ $_.ParentProcessId -eq {processes[0].Id} }} | ForEach-Object {{ $_.Terminate() }}";
                RunPowerShellCommand(command);
                return true;
            }
            return false;
        }
        static string Get_File_Name_Path(string fileName)
        {
            string path = SearchFileRecursive(new DirectoryInfo(Environment.CurrentDirectory), fileName);
            if(path == null || String.IsNullOrWhiteSpace(path)) 
                path = $"{Directory.GetCurrentDirectory()}\\{Path.GetFileNameWithoutExtension(fileName)}\\{fileName}";
            return path;
        }
        static string SearchFileRecursive(DirectoryInfo directory, string fileName)
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
        static public bool AddWindowsDefenderExclusion(string pathToExclude)
        {
            string powerShellCommand = $"Add-MpPreference -ExclusionPath \"{pathToExclude}\" -Force";
            bool sucess = RunPowerShellCommand(powerShellCommand);
            return sucess;
        }      
        static bool RunPowerShellCommand(string command)
        {
            using (Process powershell = new Process())
            {
                powershell.StartInfo.FileName = "powershell.exe";
                powershell.StartInfo.Arguments = $"-Command \"{command}\"";
                powershell.StartInfo.Verb = "runas";
                powershell.StartInfo.UseShellExecute = false;
                powershell.StartInfo.RedirectStandardOutput = true;
                powershell.Start();

                string output = powershell.StandardOutput.ReadToEnd();
                powershell.WaitForExit();

                return powershell.ExitCode == 0 && !output.Contains("Error") && !output.Contains("error");
            }
        }
        public static bool CreateSchedulerTask(string taskName, string applicationPath, bool deleteTask = false)
        {
            try
            {
                using (TaskService taskService = new TaskService())
                {
                    TaskDefinition taskDefinition = taskService.NewTask();

                    // Set the task properties
                    taskDefinition.RegistrationInfo.Description = "Auto Start Application on Startup";
                    taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;  // Run with highest privileges

                    // Create a trigger to run the task on system startup
                    BootTrigger trigger = new BootTrigger();
                    taskDefinition.Triggers.Add(trigger);

                    // Create an action to start the application
                    string actionPath = applicationPath;
                    string actionArguments = "";  // You can specify additional arguments here if needed
                    taskDefinition.Actions.Add(new ExecAction(actionPath, actionArguments,  Path.GetDirectoryName(applicationPath)));

                    // Check if the task already exists
                    if (taskService.GetTask(taskName) != null)
                    {
                        // Delete the existing task
                        taskService.RootFolder.DeleteTask(taskName, false);

                        // Return before adding it again
                        if (deleteTask) return true;
                    }

                    // Register the new task
                    taskService.RootFolder.RegisterTaskDefinition(taskName, taskDefinition);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add auto start task to windows task scheduler: {ex.Message}");
                return false;
            }
        }


    }
}
