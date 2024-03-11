using System;
using System.Runtime.InteropServices;
using Photon.Deterministic;

namespace Quantum;

public partial struct FrameTimer
{
    public bool IsRunning => _frame > 0;

    public int? TargetFrame => _frame > 0 ? _frame : 0;

    public bool Expired(Frame frame) => _frame > 0 && _frame <= frame.Number;

    public bool ExpiredOrNotRunning(Frame frame) => _frame == 0 || Expired(frame);

    public int? RemainingTicks(Frame frame)
    {
        return IsRunning ? Math.Max(0, _frame - frame.Number) : null;
    }

    public FP? RemainingTime(Frame frame)
    {
        var remainingTicks = RemainingTicks(frame);
        return remainingTicks.HasValue ? remainingTicks.Value * frame.DeltaTime : null;
    }

    public bool CheckAndStopIfExpired(Frame frame)
    {
        if (Expired(frame))
        {
            Stop();
            return true;
        }
        return false;
    }

    public void Stop()
    {
        _frame = 0;
    }
    
    public static FrameTimer CreateFromSeconds(Frame frame, FP delayInSeconds)
    {
        var timer = new FrameTimer
        {
            _frame = frame.Number + FPMath.CeilToInt(delayInSeconds / frame.DeltaTime)
        };
        return timer;
    }

    public static FrameTimer CreateFromFrames(Frame frame, int frames)
    {
        var timer = new FrameTimer
        {
            _frame = frame.Number + frames,
        };
        return timer;
    }
}