// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using CppAst;

namespace Generator;

public static class Program
{
     public static void err(string msg, params object[] args)
 {
     Console.ForegroundColor = ConsoleColor.Red;
     Console.WriteLine(msg, args);
     Console.ResetColor();
 }

 public static void warn(string msg, params object[] args)
 {
     Console.ForegroundColor = ConsoleColor.Yellow;
     Console.WriteLine(msg, args);
     Console.ResetColor();
 }

 public static void msg(string msg, params object[] args)
 {
     Console.ForegroundColor = ConsoleColor.White;
     Console.WriteLine(msg, args);
     Console.ResetColor();
 }

 public static void msg()
 {
     msg("");
 }

 public static void dbg(string msg, params object[] args)
 {
     Console.WriteLine(msg, args);
 }

 public static bool IsValidString(string s)
 {
     return !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s);
 }

 private static readonly List<string> ignoredIncludes = new List<string>() { "SDL_oldnames.h", "SDL_bits.h", "SDL_intrin.h", "SDL_main_impl.h", "SDL_test.h", "SDL_test_assert.h", "SDL_test_common.h", "SDL_test_compare.h", "SDL_test_crc32.h", "SDL_test_font.h", "SDL_test_fuzzer.h", "SDL_test_harness.h", "SDL_test_log.h", "SDL_test_md5.h", "SDL_test_memory.h", "SDL_assert.h", "SDL_stdinc.h", "SDL_endian.h" };

 private static List<string> GetAllIncludes(string rootPath, string sdlHeaderFile)
 {
     if (!File.Exists(sdlHeaderFile))
     {
         throw new FileNotFoundException("Could not find the SDL header file: " + sdlHeaderFile);
     }

     string[] lines = File.ReadAllLines(sdlHeaderFile);
     if (lines == null || lines.Length < 1)
     {
         err("SDL header file had no includes. (" + sdlHeaderFile + ")");
         return new List<string>();
     }

     List<string> incls = new List<string>();
     for (int i = 0; i < lines.Length; ++i)
     {
         string line = lines[i].Trim();
         if (!IsValidString(line))
             continue;

         if (line.StartsWith("//"))
             continue;

         if (line.StartsWith("/*"))
         {
             i++;
             while (i < lines.Length)
             {
                 line = lines[i].Trim();
                 if (line.StartsWith("*/") || line.EndsWith("*/"))
                     break;

                 i++;
             }

             if (i + 1 >= lines.Length)
                 break;

             line = lines[i + 1].Trim();
         }

         if (line.StartsWith("#include"))
         {
             line = line.Remove(0, 8).Trim();
             if ((line.StartsWith("<") && line.EndsWith(">")) || (line.StartsWith("\"") && line.EndsWith("\"")))
             {
                 line = line.Remove(line.Length - 1, 1);
                 line = line.Remove(0, 1);

                 string fname = Path.GetFileName(line);

                 if (ignoredIncludes.Exists(t => t.ToLowerInvariant().Trim() == fname.ToLowerInvariant().Trim()))
                     continue;

                 if (fname.ToLowerInvariant().Trim().StartsWith("sdl_"))
                 {
                     string p = Path.Combine(rootPath, line);
                     msg("Adding include {0} as {1}", line, p);
                     incls.Add(p);
                 }
             }
             else
             {
                 warn("Unknown include type. Ignoring...");
                 continue;
             }
         }
     }

     return incls;
}
    
    public static int Main(string[] args)
    {
        // Make sure we do not care about the culture of the current system, so that it always generates the code correctly (e.g. in some cultures the float comes as 1,0 instead of 1.0, this fixes it)
        System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        System.Globalization.CultureInfo.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
        System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
        
        string outputPath = AppContext.BaseDirectory;
        if (args.Length > 0)
        {
            outputPath = args[0];
        }

        if (!Path.IsPathRooted(outputPath))
        {
            outputPath = Path.Combine(AppContext.BaseDirectory, outputPath);
        }

        if (!outputPath.EndsWith("Generated"))
        {
            outputPath = Path.Combine(outputPath, "Generated");
        }

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        CsCodeGeneratorOptions generateOptions = new()
        {
            OutputPath = outputPath,
            ClassName = "SDL3",
            Namespace = "SDL3",
            PublicVisiblity = true,
            EnumWriteUnmanagedTag = true,
            BooleanMarshalType = "U1",
        };

        string sdlIncludePath = Path.Combine(AppContext.BaseDirectory, "include");

        /*List<string> headers =
        [
            Path.Combine(sdlIncludePath, "SDL3/SDL_atomic.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_audio.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_blendmode.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_camera.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_clipboard.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_cpuinfo.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_dialog.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_error.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_events.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_filesystem.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_gamepad.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_guid.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_haptic.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_hidapi.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_hints.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_init.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_iostream.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_joystick.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_keyboard.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_keycode.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_loadso.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_locale.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_log.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_messagebox.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_metal.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_misc.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_mouse.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_mutex.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_pen.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_pixels.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_platform.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_power.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_properties.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_rect.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_render.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_revision.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_scancode.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_sensor.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_stdinc.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_storage.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_surface.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_thread.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_time.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_timer.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_touch.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_version.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_video.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_vulkan.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_system.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_gpu.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_main.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_process.h"),
        ];*/

        // Automatic import from SDL.h for further changes in the api to be automatically merged while generating.
        List<string> headers = GetAllIncludes(sdlIncludePath, Path.Combine(sdlIncludePath, "SDL3/SDL.h"));

        // Add any includes here that are not available in the SDL.h 
        List<string> additionalIncludes = new List<string>()
        {
            Path.Combine(sdlIncludePath, "SDL3/SDL_vulkan.h"),
            Path.Combine(sdlIncludePath, "SDL3/SDL_opengl.h"),
        };
        headers.AddRange(additionalIncludes);

        int errorCounts = 0;
        int warningCounts = 0;
        Stopwatch sw = new Stopwatch();
        sw.Start();

        var options = new CppParserOptions
        {
            ParseMacros = false,
            SystemIncludeFolders =
            {
                Path.Combine(AppContext.BaseDirectory, "include")
            },
            Defines =
            {
                "SDL_PLATFORM_ANDROID",
                "SDL_PLATFORM_IOS",
                "SDL_PLATFORM_WINRT",
            },
            
            // 64 bit stuff must be the priority, if you do not wish, you can convert this to X86 only.
            TargetCpu = CppTargetCpu.X86_64,

            // Defaults to true, added for clarity.
            ParseComments = true,
        };

        CsCodeGenerator generator = new(generateOptions);

        foreach (string header in headers)
        {
            CppCompilation compilation = CppParser.ParseFile(header, options);

            // Print diagnostic messages
            // Null check added in case the Messages gets modified and being able to return null (which is not the case for now).
            if (compilation.Diagnostics.Messages != null && compilation.Diagnostics.Messages.Count > 0)
            {
                // Print all of the available logs before returning.
                bool hadErrors = false;
                foreach (CppDiagnosticMessage message in compilation.Diagnostics.Messages)
                {
                    if (message.Type == CppLogMessageType.Error)
                    {
                        errorCounts++;
                        hadErrors = true;
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else if (message.Type == CppLogMessageType.Warning)
                    {
                        warningCounts++;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    else if (message.Type == CppLogMessageType.Info)
                        Console.ForegroundColor = ConsoleColor.White;

                    Console.WriteLine($"[{message.Type}] [{Path.GetFileName(message.Location.File)}] [Column: {message.Location.Column}] [Line: {message.Location.Line}] [Offset: {message.Location.Offset}] -> {message.Text}");

                    Console.ResetColor();
                }

                if (hadErrors)
                    return -1; // Let the OS know that the app failed with -1 (can be changed to any error code but 0).
            }

            generator.Collect(compilation);
        }

        generator.Generate();
        sw.Stop();

        msg();
        msg("Finished in {0} ms with {1} warning(s) and {2} error(s).", sw.ElapsedMilliseconds, warningCounts, errorCounts);

        return 0;
    }
}
