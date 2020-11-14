// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.containers
{
    #region Using Clauses
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    #endregion

    /// <summary>
    /// Functionality to determine the details of the container and OS environment
    /// </summary>
    public static class ContainerEnvironment
    {
        #region properties
        /// <summary>
        /// The command line arguments for the current process
        /// </summary>
        public static string[] CommandLineArgs { get => Environment.GetCommandLineArgs(); }

        /// <summary>
        /// The processes command line
        /// </summary>
        public static string CommandLine { get => Environment.CommandLine; }

        /// <summary>
        /// Returns the current process start time
        /// </summary>
        public static DateTime CurrentProcessStartTime { get => Process.GetCurrentProcess().StartTime; }

        /// <summary>
        /// Returns the current process name
        /// </summary>
        public static string CurrentProcessName { get => Process.GetCurrentProcess().ProcessName; }

        /// <summary>
        /// Returns the current process id
        /// </summary>
        public static int CurrentProcessId { get => Process.GetCurrentProcess().Id; }

        /// <summary>
        /// Returns true if the operating system is ARM
        /// </summary>
        public static bool IsARM { get => RuntimeInformation.OSArchitecture == Architecture.Arm; }


        /// <summary>
        /// Returns true if the operating system is ARM 64 bit
        /// </summary>
        public static bool IsARM64 { get => RuntimeInformation.OSArchitecture == Architecture.Arm64; }

        /// <summary>
        /// Returns true if the operating system is X86
        /// </summary>
        public static bool IsX86 { get => RuntimeInformation.OSArchitecture == Architecture.X86; }

        /// <summary>
        /// Returns true if the operating system is X64
        /// </summary>
        public static bool IsX64 { get => RuntimeInformation.OSArchitecture == Architecture.X64; }

        /// <summary>
        /// Returns true if the current environment is ARM
        /// </summary>
        public static bool IsProcessARM { get => RuntimeInformation.ProcessArchitecture == Architecture.Arm; }

        /// <summary>
        /// Returns true if the current environment is ARM 64 bit
        /// </summary>
        public static bool IsProcessARM64 { get => RuntimeInformation.ProcessArchitecture == Architecture.Arm64; }

        /// <summary>
        /// Returns true if the current environment is X86
        /// </summary>
        public static bool IsProcessX86 { get => RuntimeInformation.ProcessArchitecture == Architecture.X86; }

        /// <summary>
        /// Returns true if the current environment is X64
        /// </summary>
        public static bool IsProcessX64 { get => RuntimeInformation.ProcessArchitecture == Architecture.X64; }

        /// <summary>
        /// Returns true if the current environment is Windows
        /// </summary>
        public static bool IsWindows { get => RuntimeInformation.IsOSPlatform(OSPlatform.Windows); }

        /// <summary>
        /// Returns true if the current environment is Linux
        /// </summary>
        public static bool IsLinux { get => RuntimeInformation.IsOSPlatform(OSPlatform.Linux); }

        /// <summary>
        /// Returns true if the current environment is MAC OS
        /// </summary>
        public static bool IsMacOS { get => RuntimeInformation.IsOSPlatform(OSPlatform.OSX); }

        /// <summary>
        /// Returns true if the current environment is FreeBSD
        /// </summary>
        public static bool IsFreeBSD { get => RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD); }
        #endregion
    }
}
