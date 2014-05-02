using System;
using System.Collections.Generic;
using System.Text;
using Jungo.wdapi_dotnet;

namespace ComputerMonitor.CAN
{
    class CANCardLib
    {
        private static string CANCARD_DEFAULT_LICENSE_STRING = "6C3CC2CFE89E7AD04238DF2EF24449E848CDA951.3ddown.com";
        private static string CANCARD_DEFAULT_DRIVER_NAME = "windrvr6";
        public static uint CANCARD_LibInit()
        {
            if (windrvr_decl.WD_DriverName(CANCARD_DEFAULT_DRIVER_NAME) == null)
            {

                return (uint)WD_ERROR_CODES.WD_SYSTEM_INTERNAL_ERROR;
            }

            uint dwStatus = wdc_lib_decl.WDC_SetDebugOptions(wdc_lib_consts.WDC_DBG_DEFAULT, null);
            if (dwStatus != (uint)WD_ERROR_CODES.WD_STATUS_SUCCESS)
            {

                return dwStatus;
            }

            dwStatus = wdc_lib_decl.WDC_DriverOpen((uint)wdc_lib_consts.WDC_DRV_OPEN_DEFAULT, CANCARD_DEFAULT_LICENSE_STRING);
            if (dwStatus != (uint)WD_ERROR_CODES.WD_STATUS_SUCCESS)
            {

                return dwStatus;
            }
            return 0;
            
        }
    }
}
