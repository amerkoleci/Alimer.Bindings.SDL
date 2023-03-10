// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;

namespace Alimer.Bindings.SDL;

/// <summary>
/// The different sensors defined by SDL.
/// </summary>
public enum SDL_SensorType : int
{
    /// <summary>
    /// Returned for an invalid sensor
    /// </summary>
    SDL_SENSOR_INVALID = -1,
    /// <summary>
    /// Unknown sensor type
    /// </summary>
    SDL_SENSOR_UNKNOWN,
    /// <summary>
    /// Accelerometer
    /// </summary>
    SDL_SENSOR_ACCEL,
    /// <summary>
    /// Gyroscope
    /// </summary>
    SDL_SENSOR_GYRO,
    /// <summary>
    /// Accelerometer for left Joy-Con controller and Wii nunchuk
    /// </summary>
    SDL_SENSOR_ACCEL_L,
    /// <summary>
    /// Gyroscope for left Joy-Con controller
    /// </summary>
    SDL_SENSOR_GYRO_L,
    /// <summary>
    /// Accelerometer for right Joy-Con controller
    /// </summary>
    SDL_SENSOR_ACCEL_R,
    /// <summary>
    /// Gyroscope for right Joy-Con controller 
    /// </summary>
    SDL_SENSOR_GYRO_R
}

unsafe partial class SDL
{
    public const float SDL_STANDARD_GRAVITY = 9.80665f;

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_SensorID* SDL_GetSensors(out int count);

    public static ReadOnlySpan<SDL_SensorID> SDL_GetSensors()
    {
        SDL_SensorID* ptr = SDL_GetSensors(out int count);
        return new(ptr, count);
    }

    [DllImport(LibName, EntryPoint = nameof(SDL_GetSensorInstanceName), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetSensorInstanceName(SDL_SensorID instance_id);

    public static string SDL_GetSensorInstanceName(SDL_SensorID instance_id)
    {
        return GetString(INTERNAL_SDL_GetSensorInstanceName(instance_id));
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_SensorType SDL_GetSensorInstanceType(SDL_SensorID instance_id);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetSensorInstanceNonPortableType(SDL_SensorID instance_id);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Sensor SDL_OpenSensor(SDL_SensorID instance_id);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Sensor SDL_GetSensorFromInstanceID(SDL_SensorID instance_id);

    [DllImport(LibName, EntryPoint = nameof(SDL_GetSensorName), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetSensorName(SDL_Sensor sensor);

    public static string SDL_GetSensorName(SDL_Sensor sensor)
    {
        return GetString(INTERNAL_SDL_GetSensorName(sensor));
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_SensorType SDL_GetSensorType(SDL_Sensor sensor);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetSensorNonPortableType(SDL_Sensor sensor);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_SensorID SDL_GetSensorInstanceID(SDL_Sensor sensor);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetSensorData(
        SDL_Sensor sensor,
        float* data,
        int num_values
    );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_CloseSensor(SDL_Sensor sensor);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_UpdateSensors();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_LockSensors();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_UnlockSensors();
}
