using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using static RE8FOV.PInvoke.Kernel32;

namespace RE8FOV
{
    public unsafe class ProcessMemory
    {
        private IntPtr processHandle;
        private byte* basePointer; // Done as a byte* instead of long* because + 1 on the pointer will advance it by 8 (size of a long).
        private int[] fovOffsets = new int[] { 0x1F8, 0x68, 0x78, 0x128, 0x20, 0x20, 0x50 };

        public ProcessMemory(string processName)
        {
            byte[] checksum;
            using (Process proc = Process.GetProcessesByName(processName)[0])
            {
                this.processHandle = OpenProcess(ProcessAccessRightsFlags.QUERY_INFORMATION | ProcessAccessRightsFlags.VM_READ | ProcessAccessRightsFlags.VM_WRITE, false, proc.Id);
                this.basePointer = (byte*)proc.MainModule.BaseAddress.ToPointer();

                using (SHA256 hashFunc = SHA256.Create())
                using (FileStream fs = new FileStream(proc.MainModule.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                    checksum = hashFunc.ComputeHash(fs);
            }

            // Checksum check to determine which base pointer to use.
            // TODO: Transition to use app_GlobalService's list of static pointers.
            // app_GlobalService: "re8.exe"+A1B2AA0
            if (checksum.SequenceEqual(GameHashes.re8WW_20211217_1))
                this.basePointer += 0x0A060C88L + 0x10F0;
            else if (checksum.SequenceEqual(GameHashes.re8WW_20210824_4) || checksum.SequenceEqual(GameHashes.re8WW_20210810_1) || checksum.SequenceEqual(GameHashes.re8WW_20211012_5))
                this.basePointer += 0x0A060C88L;
            else if (checksum.SequenceEqual(GameHashes.re8WW_20210719_1))
                this.basePointer += 0x0A05ECB8L;
            else if (checksum.SequenceEqual(GameHashes.re8WW_20210506_1))
                this.basePointer += 0x0A1A74F0L;
            else if (checksum.SequenceEqual(GameHashes.re8ceroD_20210506_1))
                this.basePointer += 0x0A1A74F0L + 0x2000L;
            else if (checksum.SequenceEqual(GameHashes.re8ceroZ_20210508_1))
                this.basePointer += 0x0A1A74F0L + 0x1000L;
            else if (checksum.SequenceEqual(GameHashes.re8promo01_20210426_1))
                this.basePointer += 0x0A1A74F0L + 0x1030L;
            else
                throw new FileFormatException("re8.exe SHA256 checksum not recognized! Possible new version or modified game file!");
        }

        private unsafe bool GetPlayerCameraParameterAddress(out long address)
        {
            fixed (long* addressPointer = &address)
            {
                if (!ReadProcessMemory(processHandle, basePointer, addressPointer, (UIntPtr)sizeof(long), out UIntPtr _))
                    return false;

                for (int i = 0; i < fovOffsets.Length; ++i)
                    if (!ReadProcessMemory(processHandle, (byte*)address + fovOffsets[i], addressPointer, (UIntPtr)sizeof(long), out UIntPtr _))
                        return false;
            }

            return true;
        }

        public unsafe bool GetFOVValues(out float[] values)
        {
            values = new float[2];
            if (GetPlayerCameraParameterAddress(out long address))
                fixed (float* f = values)
                    return ReadProcessMemory(processHandle, (byte*)address + 0x38L, f, (UIntPtr)(sizeof(float) * 2), out UIntPtr _);
            else
                return false;
        }

        public unsafe bool SetFOVValues(float normal = 81f, float aiming = 70f)
        {

            if (GetPlayerCameraParameterAddress(out long address))
            {
                float[] values = new float[2] { normal, aiming };
                fixed (float* f = values)
                    return WriteProcessMemory(processHandle, (byte*)address + 0x38L, f, (UIntPtr)(sizeof(float) * 2), out UIntPtr _);
            }
            else
                return false;
        }
    }
}
