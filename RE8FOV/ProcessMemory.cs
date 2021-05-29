﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace RE8FOV
{
    public class ProcessMemory
    {
        private PInvoke.Kernel32.SafeObjectHandle processHandle;
        private long processBaseAddress;
        private long basePointer;
        private int[] fovOffsets = new int[] { 0x1F8, 0x68, 0x78, 0x128, 0x20, 0x20, 0x50 };

        public ProcessMemory(string processName)
        {
            byte[] checksum;
            using (Process proc = Process.GetProcessesByName(processName)[0])
            {
                this.processHandle = PInvoke.Kernel32.OpenProcess(PInvoke.Kernel32.ProcessAccess.PROCESS_QUERY_INFORMATION | PInvoke.Kernel32.ProcessAccess.PROCESS_VM_READ | PInvoke.Kernel32.ProcessAccess.PROCESS_VM_WRITE, false, proc.Id);
                this.processBaseAddress = proc.MainModule.BaseAddress.ToInt64();

                using (SHA256 hashFunc = SHA256.Create())
                using (FileStream fs = new FileStream(proc.MainModule.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                    checksum = hashFunc.ComputeHash(fs);
            }

            // Checksum check to determine which base pointer to use.
            if (checksum.SequenceEqual(GameHashes.re8WW_20210506_1))
                this.basePointer = 0x0A1A74F0L;
            else if (checksum.SequenceEqual(GameHashes.re8ceroD_20210506_1))
                this.basePointer = 0x0A1A74F0L + 0x2000L;
            else if (checksum.SequenceEqual(GameHashes.re8ceroZ_20210508_1))
                this.basePointer = 0x0A1A74F0L + 0x1000L;
            else if (checksum.SequenceEqual(GameHashes.re8promo01_20210426_1))
                this.basePointer = 0x0A1A74F0L + 0x1030L;
            else
                throw new FileFormatException("re8.exe SHA256 checksum not recognized! Possible new version or modified game file!");
        }

        private unsafe long GetPlayerCameraParameterAddress()
        {
            long address = 0L;
            UIntPtr bytesRead = UIntPtr.Zero;
            int lastError = 0;

            if (!PInvoke.Kernel32.ReadProcessMemory(processHandle, new IntPtr(processBaseAddress + basePointer), new IntPtr(&address), new UIntPtr((uint)sizeof(IntPtr)), out bytesRead))
            {
                lastError = Marshal.GetLastWin32Error();
            }

            for (int i = 0; i < fovOffsets.Length; ++i)
            {
                if (!PInvoke.Kernel32.ReadProcessMemory(processHandle, new IntPtr(address + fovOffsets[i]), new IntPtr(&address), new UIntPtr((uint)sizeof(IntPtr)), out bytesRead))
                {
                    lastError = Marshal.GetLastWin32Error();
                }
            }

            return address;
        }

        public unsafe float[] GetFOVValues()
        {
            float[] value = new float[2];
            UIntPtr bytesRead = UIntPtr.Zero;
            int lastError = 0;

            fixed (float* f = value)
            {
                if (!PInvoke.Kernel32.ReadProcessMemory(processHandle, new IntPtr(GetPlayerCameraParameterAddress() + 0x38), (IntPtr)f, (UIntPtr)8, out bytesRead))
                {
                    lastError = Marshal.GetLastWin32Error();
                }
            }

            return value;
        }

        public unsafe bool SetFOVValues(float normal = 81f, float aiming = 70f)
        {
            UIntPtr bytesRead = UIntPtr.Zero;
            try
            {
                bool success = false;
                processHandle.DangerousAddRef(ref success);
                if (success)
                {
                    float[] values = new float[2] { normal, aiming };
                    fixed (float* f = values)
                        return PInvoke.Kernel32.WriteProcessMemory(processHandle.DangerousGetHandle(), new IntPtr(GetPlayerCameraParameterAddress() + 0x38), (IntPtr)f, (UIntPtr)8, out bytesRead);
                }
                else
                    return false;
            }
            finally
            {
                processHandle.DangerousRelease();
            }
        }
    }
}