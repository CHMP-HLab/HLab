using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using static HLab.Sys.Windows.API.DwmApi;
using static HLab.Sys.Windows.API.WinDef;
using static HLab.Sys.Windows.API.WinUser;

namespace HLab.Sys.Windows.API;

public class ApiWindow : IEquatable<ApiWindow>
{
    readonly nint _hWnd;
    public ApiWindow(nint hWnd)
    {
        _hWnd = hWnd;
    }

    public nint Handle => _hWnd;

    public WindowPlacement GetPlacement()
    {
        var placement = new WindowPlacement();
        GetWindowPlacement(_hWnd, ref placement);
        return placement;
    }

    public void SetFocus()
    {
        WinUser.SetFocus(_hWnd);
    }

    public bool SetPos(nint hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags uFlags) => SetWindowPos(_hWnd, hWndInsertAfter, x, y, cx, cy, uFlags);
    public bool SetPos(HandleWindow hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags uFlags) => SetWindowPos(_hWnd, hWndInsertAfter, x, y, cx, cy, uFlags);

    public Rect GetRect() => GetRect(out var rect) ? rect : default;
    public bool GetRect(out Rect rect) => GetWindowRect(_hWnd, out rect);

    public Rect ExtendedFrameBounds {
        get
        {
            var hresult = DwmGetWindowAttribute(_hWnd, DwmWindowAttribute.ExtendedFrameBounds, out var rect, Marshal.SizeOf(typeof(Rect)));
            return rect;

        }
    } 

    public void BringToFront() => SetForegroundWindow(_hWnd);

    public nint SetActive() => SetActiveWindow(_hWnd);

    public (uint,int) GetThreadProcessId()
    {
        var threadId = GetWindowThreadProcessId(_hWnd, out var lpdwProcessId);
        return threadId > 0 ? (threadId,lpdwProcessId) : default;
    }

    public bool ShowWindow(ShowWindowEnum flags) => WinUser.ShowWindow(_hWnd, flags);

    public bool IsVisible => IsWindowVisible(_hWnd);

    public static void EnumDesktopWindows(Func<ApiWindow,bool> callBackAction, nint hDesktop = 0)
    {
        bool EnumerateHandle(nint window, nint lParam) => callBackAction(new ApiWindow(window));

        WinUser.EnumDesktopWindows(hDesktop, EnumerateHandle, 0);
    }

    public bool Equals(ApiWindow other)
    {
        if (other is null) return false;
        return ReferenceEquals(this, other) || _hWnd.Equals(other._hWnd);
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((ApiWindow)obj);
    }

    public override int GetHashCode()
    {
        return _hWnd.GetHashCode();
    }

    public Bitmap Icon(CancellationToken token)
    {
        try
        {
            nint hIcon = SendMessage(_hWnd, WindowMessage.GetIcon, (int)IconSize.Big, 0);

            if(token.IsCancellationRequested) return null;

            if (hIcon == 0)
                hIcon = GetClassLongPtr(_hWnd, GetClassWindow.HIcon);

            if(token.IsCancellationRequested) return null;

            if (hIcon == 0)
                hIcon = LoadIcon(0, PredefinedIcons.Application);

            if (token.IsCancellationRequested) return null;

            if (hIcon != 0)
                return System.Drawing.Icon.FromHandle(hIcon).ToBitmap();
        }
        catch (Exception)
        {
        }

        return null;
    }

}