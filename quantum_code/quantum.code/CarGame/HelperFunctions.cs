using System;
using Photon.Deterministic;

namespace Quantum;

public static class HelperFunctions
{
    public static FP ClampMinus1To1(FP value)
    {
        return FPMath.Clamp(value, -FP._1, FP._1);
    }
    
    public static FPVector2 ClampVectorComponentsMinus1To1(FPVector2 vector)
    {
        return new FPVector2(ClampMinus1To1(vector.X), ClampMinus1To1(vector.Y));
    }
}