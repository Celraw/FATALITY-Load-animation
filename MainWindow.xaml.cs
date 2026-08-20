using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Runtime.InteropServices;

namespace FatalityVisual;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _closeTimer;

    public MainWindow()
    {
        InitializeComponent();

        _closeTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5.45)
        };
        _closeTimer.Tick += (_, _) => Close();

        SourceInitialized += EnableBackdrop;
        ContentRendered += StartAnimation;
        Closed += (_, _) => _closeTimer.Stop();
    }

    private void StartAnimation(object? sender, EventArgs e)
    {
        ContentRendered -= StartAnimation;

        var storyboard = ((Storyboard)FindResource("IntroStoryboard")).Clone();
        storyboard.Completed += (_, _) => Close();
        storyboard.Begin(this, HandoffBehavior.SnapshotAndReplace, true);
        _closeTimer.Start();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }

    private void EnableBackdrop(object? sender, EventArgs e)
    {
        var handle = new WindowInteropHelper(this).Handle;
        var source = HwndSource.FromHwnd(handle);

        if (source?.CompositionTarget is not null)
        {
            source.CompositionTarget.BackgroundColor = Colors.Transparent;
        }

        // Native Windows acrylic. GradientColor is encoded as AABBGGRR.
        var accent = new AccentPolicy
        {
            AccentState = AccentState.EnableAcrylicBlurBehind,
            AccentFlags = 2,
            GradientColor = unchecked((int)0x990A0606)
        };

        var accentSize = Marshal.SizeOf<AccentPolicy>();
        var accentPointer = Marshal.AllocHGlobal(accentSize);

        try
        {
            Marshal.StructureToPtr(accent, accentPointer, false);
            var data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.AccentPolicy,
                Data = accentPointer,
                SizeOfData = accentSize
            };

            if (SetWindowCompositionAttribute(handle, ref data) == 0)
            {
                // Older systems can support classic blur even when acrylic is unavailable.
                accent.AccentState = AccentState.EnableBlurBehind;
                Marshal.StructureToPtr(accent, accentPointer, true);
                SetWindowCompositionAttribute(handle, ref data);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(accentPointer);
        }
    }

    [DllImport("user32.dll")]
    private static extern int SetWindowCompositionAttribute(
        IntPtr windowHandle,
        ref WindowCompositionAttributeData data);

    private enum WindowCompositionAttribute
    {
        AccentPolicy = 19
    }

    private enum AccentState
    {
        Disabled = 0,
        EnableBlurBehind = 3,
        EnableAcrylicBlurBehind = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }
}
