// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL3;

unsafe partial class SDL3
{
    public static ReadOnlySpan<SDL_SensorID> SDL_GetSensors()
    {
        SDL_SensorID* ptr = SDL_GetSensors(out int count);
        return new(ptr, count);
    }

    public static string SDL_GetSensorNameForIDString(SDL_SensorID instance_id)
    {
        return GetStringOrEmpty(SDL_GetSensorNameForID(instance_id));
    }

    public static string SDL_GetSensorNameString(SDL_Sensor sensor)
    {
        return GetStringOrEmpty(SDL_GetSensorName(sensor));
    }
}
