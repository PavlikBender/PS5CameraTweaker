using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PS5CameraTweaker;
public partial class MainViewModel : ObservableObject
{
    // https://github.com/psxdev/OrbisEyeCam/issues/10
    private const int autoExposureOffset = 0x010403;
    private const int autoExposureOffset2 = 0x010407;
    private const int gainOffset = 0x0103BB;
    private const int sharpnessOffset = 0x01034F;
    private const int saturationOffset = 0x01032B;
    private const int contrastOffset = 0x0102E3;

    private const string firmwareFilename = "../firmware.bin";

    [ObservableProperty]
    private ushort _contrast;

    [ObservableProperty]
    private ushort _saturation;

    [ObservableProperty]
    private ushort _sharpness;

    [ObservableProperty]
    private ushort _gain;

    [ObservableProperty]
    private bool _autoexposure;

    [RelayCommand]
    private void Save()
    {
        using var source = new FileStream(firmwareFilename, FileMode.Open, FileAccess.Write);
        using var binaryWriter = new BinaryWriter(source);

        var autoexposure = (ushort)(Autoexposure ? 2 : 4);
        source.Seek(autoExposureOffset, SeekOrigin.Begin);
        binaryWriter.Write(autoexposure);
        source.Seek(autoExposureOffset2, SeekOrigin.Begin);
        binaryWriter.Write(autoexposure);

        source.Seek(gainOffset, SeekOrigin.Begin);
        binaryWriter.Write(Gain);

        source.Seek(sharpnessOffset, SeekOrigin.Begin);
        binaryWriter.Write(Sharpness);

        source.Seek(saturationOffset, SeekOrigin.Begin);
        binaryWriter.Write(Saturation);

        source.Seek(contrastOffset, SeekOrigin.Begin);
        binaryWriter.Write(Contrast);

        // Not the type of programm for using full MVVM.
        MessageBox.Show("Saved successfully!");
    }

    public MainViewModel()
    {
        Load();
    }

    private void Load()
    {
        if (!File.Exists(firmwareFilename))
        {
            // Not the type of programm for using full MVVM.
            MessageBox.Show("Firmware not found!");
            Application.Current.Shutdown();
            return;
        }

        using var source = new FileStream(firmwareFilename, FileMode.Open, FileAccess.Read);
        using var binaryReader = new BinaryReader(source);

        source.Seek(autoExposureOffset, SeekOrigin.Begin);
        var autoExposure = binaryReader.ReadUInt16();
        Autoexposure = autoExposure == 02;

        source.Seek(gainOffset, SeekOrigin.Begin);
        Gain = binaryReader.ReadUInt16();

        source.Seek(sharpnessOffset, SeekOrigin.Begin);
        Sharpness = binaryReader.ReadUInt16();

        source.Seek(saturationOffset, SeekOrigin.Begin);
        Saturation = binaryReader.ReadUInt16();

        source.Seek(contrastOffset, SeekOrigin.Begin);
        Contrast = binaryReader.ReadUInt16();
    }
}
