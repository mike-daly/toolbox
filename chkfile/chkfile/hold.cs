using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkfile
{
    internal class hold
    {
    }
}

   public static IEnumerable<PhysicalDiskInfo> GetPhysicalDisks()
        {
            // WMI class Win32_DiskDrive gives us the raw hardware info
            using (var searcher = new ManagementObjectSearcher(
                "SELECT DeviceID, Model, SerialNumber, Signature, PartitionStyle FROM Win32_DiskDrive"))
            {
                foreach (ManagementObject mo in searcher.Get())
                {
                    string deviceId = mo["DeviceID"] as string;               // \\.\PHYSICALDRIVE0
                    string model    = mo["Model"]    as string ?? "";
                    string serial   = mo["SerialNumber"] as string ?? "";
                    uint?  signature = mo["Signature"] as uint?;              // MBR only
                    string partStyle = mo["PartitionStyle"] as string ?? ""; // "GPT", "MBR", "RAW"

                    // The UniqueId that PowerShell shows lives in the Setup API
                    string uniqueId = GetDiskUniqueId(deviceId, partStyle, signature);

                    // Disk number (0-based) – strip the prefix
                    int diskNumber = int.Parse(deviceId.AsSpan(deviceId.LastIndexOf("VE") + 2));

                    yield return new PhysicalDiskInfo
                    {
                        DiskNumber  = diskNumber,
                        DeviceId    = deviceId,
                        Model       = model.Trim(),
                        SerialNumber= serial.Trim(),
                        UniqueId    = uniqueId,
                        PartitionStyle = partStyle
                    };
                }
            }
        }

        /// <summary>
        /// Calls the low-level SetupDi API to read the Disk's UniqueId.
        /// For GPT disks this is the Disk-GUID {xxxxxxxx-xxxx…}.
        /// For MBR disks it is the 4-byte signature + offset formatted as a GUID.
        /// </summary>
        private static string GetDiskUniqueId(string deviceId, string partStyle, uint? mbrSignature)
        {
            IntPtr hDevInfo = SetupApi.SetupDiGetClassDevs(
                ref GuidDevInterfaceDisk, null, IntPtr.Zero,
                SetupApi.DIGCF_PRESENT | SetupApi.DIGCF_DEVICEINTERFACE);

            if (hDevInfo == IntPtr.Zero) return "<error>";

            try
            {
                var interfaceData = new SetupApi.SP_DEVICE_INTERFACE_DATA();
                interfaceData.cbSize = Marshal.SizeOf(interfaceData);

                // Enumerate all disk interfaces
                for (uint i = 0; SetupApi.SetupDiEnumDeviceInterfaces(hDevInfo, null,
                         ref GuidDevInterfaceDisk, i, ref interfaceData); i++)
                {
                    // Get the required buffer size for the detail structure
                    SetupApi.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref interfaceData,
                        IntPtr.Zero, 0, out uint requiredSize, IntPtr.Zero);

                    IntPtr detailPtr = Marshal.AllocHGlobal((int)requiredSize);
                    try
                    {
                        var detail = new SetupApi.SP_DEVICE_INTERFACE_DETAIL_DATA();
                        detail.cbSize = (uint)(IntPtr.Size == 8 ? 8 : 4 + Marshal.SystemDefaultCharSize);
                        Marshal.StructureToPtr(detail, detailPtr, false);

                        if (SetupApi.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref interfaceData,
                            detailPtr, requiredSize, out _, IntPtr.Zero))
                        {
                            string path = Marshal.PtrToStringAuto(
                                IntPtr.Add(detailPtr, Marshal.SizeOf(typeof(uint)))) ?? "";

                            // Match the \\.\PHYSICALDRIVEn path
                            if (string.Equals(path, deviceId, StringComparison.OrdinalIgnoreCase))
                            {
                                // We have the right device – now ask for the UniqueId property
                                return GetUniqueIdFromDevice(hDevInfo, interfaceData);
                            }
                        }
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(detailPtr);
                    }
                }
            }
            finally
            {
                SetupApi.SetupDiDestroyDeviceInfoList(hDevInfo);
            }

            // Fallback for very old drivers that don’t expose the property
            if (partStyle == "GPT") return "<GPT-Unknown>";
            if (mbrSignature.HasValue)
                return $"{mbrSignature.Value:X8}-0000-0000-0000-000000000000";
            return "<Unknown>";
        }

        private static string GetUniqueIdFromDevice(IntPtr hDevInfo, SetupApi.SP_DEVICE_INTERFACE_DATA interfaceData)
        {
            const uint PROPERTYKEY_SIZE = 20; // sizeof(GUID) + sizeof(DWORD)

            // DEVPKEY_Device_InstanceId → we need the device instance path first
            if (!SetupApi.SetupDiGetDeviceInstanceId(hDevInfo, ref interfaceData,
                null, 0, out uint needed))
            {
                needed = 256;
            }

            var sb = new System.Text.StringBuilder((int)needed);
            if (!SetupApi.SetupDiGetDeviceInstanceId(hDevInfo, ref interfaceData, sb, needed, out _))
                return "<error-instance>";

            string instanceId = sb.ToString();

            // Open the registry key for this device
            using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                $@"SYSTEM\CurrentControlSet\Enum\{instanceId}"))
            {
                if (key == null) return "<no-reg>";

                // The UniqueId is stored under the "UniqueId" value (GUID string)
                var val = key.GetValue("UniqueId");
                if (val is string s && Guid.TryParse(s, out _))
                    return "{" + s.Trim('{', '}') + "}";

                // Some drivers store it as a byte array
                if (val is byte[] bytes && bytes.Length == 16)
                    return new Guid(bytes).ToString("B");
            }

            return "<not-found>";
        }

        // --------------------------------------------------------------
        // P/Invoke definitions
        // --------------------------------------------------------------
        private static class SetupApi
        {
            public const uint DIGCF_PRESENT = 0x00000002;
            public const uint DIGCF_DEVICEINTERFACE = 0x00000010;

            [StructLayout(LayoutKind.Sequential)]
            public struct SP_DEVICE_INTERFACE_DATA
            {
                public int cbSize;
                public Guid InterfaceClassGuid;
                public uint Flags;
                public IntPtr Reserved;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct SP_DEVICE_INTERFACE_DETAIL_DATA
            {
                public uint cbSize;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
                public string DevicePath;
            }

            [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern IntPtr SetupDiGetClassDevs(
                ref Guid ClassGuid, string Enumerator, IntPtr hwndParent, uint Flags);

            [DllImport("setupapi.dll", SetLastError = true)]
            public static extern bool SetupDiEnumDeviceInterfaces(
                IntPtr hDevInfo, IntPtr devInfo, ref Guid interfaceClassGuid,
                uint memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

            [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern bool SetupDiGetDeviceInterfaceDetail(
                IntPtr hDevInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
                IntPtr deviceInterfaceDetailData, uint deviceInterfaceDetailDataSize,
                out uint requiredSize, IntPtr deviceInfoData);

            [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern bool SetupDiGetDeviceInterfaceDetail(
                IntPtr hDevInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
                IntPtr deviceInterfaceDetailData, uint deviceInterfaceDetailDataSize,
                out uint requiredSize, IntPtr deviceInfoData);

            [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

            [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern bool SetupDiGetDeviceInstanceId(
                IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData,
                System.Text.StringBuilder DeviceInstanceId, uint DeviceInstanceIdSize,
                out uint RequiredSize);
        }

        private static readonly Guid GuidDevInterfaceDisk =
            new Guid("53F56307-B6BF-11D0-94F2-00A0C91EFB8B");

        // --------------------------------------------------------------
        // Helper DTO
        // --------------------------------------------------------------
        public class PhysicalDiskInfo
        {
            public int    DiskNumber     { get; set; }
            public string DeviceId       { get; set; } = "";
            public string Model          { get; set; } = "";
            public string SerialNumber   { get; set; } = "";
            public string UniqueId       { get; set; } = "";
            public string PartitionStyle { get; set; } = "";

            public override string ToString()
            {
                return $"Disk {DiskNumber} | {Model} | SN: {SerialNumber} | UniqueId: {UniqueId}";
            }
        }
