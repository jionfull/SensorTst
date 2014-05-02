using System;
using System.Collections.Generic;
using System.Text;
using Jungo.wdapi_dotnet;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ComputerMonitor.CAN
{
    class CANDevice
    {
        uint ioStart=0x270;
        uint ioRange = 0x10;
        uint irqNum = 0x0a;
        WDC_DEVICE dev = new WDC_DEVICE();
        INT_HANDLER IRQCallback;
        public event EventHandler IRQEvent = null;
        /// <summary>
        /// IO起始地址
        /// </summary>
        public uint IOStart
        {
            get
            {
                return ioStart;
            }
            set
            {
                ioStart = value;
            }
            
        }
        /// <summary>
        /// IO结束地址
        /// </summary>
        public uint IORange
        {
            get
            {
                return ioRange;
            }
            set
            {
                ioRange = value;
            }
            
        }
        /// <summary>
        /// 中断号
        /// </summary>
        public uint IRQNum
        {
            get
            {
                return irqNum;
            }
            set
            {
                irqNum = value;
            }
        }

        public CANDevice()
        {
            dev.hDev = IntPtr.Zero;
        }

        /// <summary>
        /// 打开设备
        /// </summary>
        /// <returns></returns>
        public bool OpenDevice()
        {
            if (dev.hDev == IntPtr.Zero)
            {
                CANCard card = new CANCard();
                card.dwItems = 2;
                card.Item1.item = (uint)ITEM_TYPE.ITEM_BUS;
                card.Item1.item = (uint)ITEM_TYPE.ITEM_IO;
                card.Item1.fNotSharable = 0;
                card.Item1.I.IO.dwAddr = ioStart;
                card.Item1.I.IO.dwBytes = ioRange;
                card.Item1.I.IO.dwBar = 0;


                //Int - Int: Interrupt Number a, Edge Triggered 
                card.Item0.item = (uint)ITEM_TYPE.ITEM_INTERRUPT;
                card.Item0.fNotSharable = 0;
                card.Item0.I.Int.dwInterrupt = irqNum;
                card.Item0.I.Int.dwOptions = 0;
                IntPtr ptrCard = Marshal.AllocHGlobal(Marshal.SizeOf(card));
                Marshal.StructureToPtr(card, ptrCard, true);

                


                uint status = wdc_lib_decl.WDC_IsaDeviceOpen(ref dev.hDev, ptrCard, IntPtr.Zero, IntPtr.Zero, null, IntPtr.Zero);


                if (status == (uint)WD_ERROR_CODES.WD_STATUS_SUCCESS)
                {
                    //bool state = wdc_lib_decl.WDC_IntIsEnabled(dev.hDev);

                    IRQCallback = new INT_HANDLER(ProcessIRQ);

                    wdc_lib_decl.WDC_IntEnable((WDC_DEVICE)(dev), null, 0, 0, IRQCallback, IntPtr.Zero, false);
                    wdc_lib_decl.WDC_IntIsEnabled(dev.hDev);
                    return true;
                }
                else
                {
                    dev.hDev = IntPtr.Zero;
                    return false;
                }

            }
            return true;
            
        }

        public bool IsOpen
        {
            get
            {
                if (dev.hDev == IntPtr.Zero)
                {
                    return false;
                }

                return true;
            }
        }

        public void CloseDevice()
        {
            wdc_lib_decl.WDC_IntDisable((WDC_DEVICE)(dev));
            wdc_lib_decl.WDC_IsaDeviceClose(dev.hDev);
            dev.hDev = IntPtr.Zero;
        }

        private void ProcessIRQ(IntPtr data)
        {
            if (IRQEvent != null)
            {
                IRQEvent(null, null);
            }
        }

        public ushort ReadData(uint offset)
        {
            ushort data = 0; ;
            wdc_lib_decl.WDC_ReadAddr16(dev.hDev, 0, offset, ref data);
            return data;
        }

        public void WriteData(uint offset,ushort data)
        {
            if (offset == 0)
            {
                canRead = false;
            }
            if (offset == 2)
            {
                canRead = true;
            }
            wdc_lib_decl.WDC_WriteAddr16(dev.hDev, 0, offset, data);
            
        }
        bool canRead = false;
        public bool CanRead
        {
            get
            {
                return canRead;
            }
        }
    }

    struct CANCard
    {
        public uint dwItems;
        public WD_ITEMS Item0;
        public WD_ITEMS Item1;
        
    };



}
