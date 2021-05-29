using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace RE8FOV
{
    public partial class MainUI : Form
    {
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        private ProcessMemory? processMemory = null;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        private Settings settings;
        private JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            AllowTrailingCommas = true,
            NumberHandling = JsonNumberHandling.Strict,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        public MainUI()
        {
            // Initialize WinForm components.
            InitializeComponent();
        }

        private void MainUI_Load(object sender, EventArgs e)
        {
            // Load settings from disk or initialize defaults.
            if (File.Exists("RE8FOV_Config.json"))
            {
                try
                {
                    settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText("RE8FOV_Config.json", Encoding.UTF8), jsonSerializerOptions);
                }
                catch
                {
                    settings = new Settings();
                }
            }
            else
                settings = new Settings();

            // Set properties on the form.
            normalFOVTextBox.Text = settings.NormalFOV.ToString();
            aimingFOVTextBox.Text = settings.AimingFOV.ToString();
        }

        private void MainUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.WriteAllText("RE8FOV_Config.json", JsonSerializer.Serialize(settings, jsonSerializerOptions), Encoding.UTF8);
        }

        private void normalFOVTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!float.TryParse(normalFOVTextBox.Text, out settings.normalFOV))
                settings.NormalFOV = 81f;
        }

        private void aimingFOVTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!float.TryParse(aimingFOVTextBox.Text, out settings.aimingFOV))
                settings.AimingFOV = 70f;
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            // If ProcessMemory is not initialized yet, initialize it.
            if (processMemory == null)
                processMemory = new ProcessMemory("re8");

            processMemory.SetFOVValues(settings.NormalFOV, settings.AimingFOV);
        }
    }
}
