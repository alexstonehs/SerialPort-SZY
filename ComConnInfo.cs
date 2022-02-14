using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TYSerialPort
{
    /// <summary>
    /// 串口连接信息
    /// </summary>
    public class ComConnInfo
    {
        /// <summary>
        /// 串口名称
        /// </summary>
        public string Com { get; set; }
        /// <summary>
        /// 串口波特率
        /// </summary>
        public int Baud { get; set; }
        /// <summary>
        /// 串口数据位
        /// </summary>
        public int DataBit { get; set; }
        /// <summary>
        /// 串口检验位
        /// </summary>
        public Parity Parity { get; set; }
        /// <summary>
        /// 串口停止位
        /// </summary>
        public StopBits StopBit { get; set; }
        /// <summary>
        /// 是否启动DTR
        /// </summary>
        public bool DtrEnable { get; set; }
    }
}
