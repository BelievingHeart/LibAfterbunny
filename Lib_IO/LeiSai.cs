using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Lib_Enums;

namespace Lib_IO
{
    public class LeiSai : ICommunicator
    {
        private readonly ushort _bitIn, _bitOutOk, _bitOutNg, _bitOutProductMissing;
        private readonly int _durationSleep = 100;
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        private readonly int _triggeredState;
        private int _previousState;

        public LeiSai(ushort BitIn, ushort BitOut_OK, ushort BitOut_NG, ushort BitOut_NOPRODUCT,
            TriggerType triggerType = TriggerType.FallingEdge, int DurationSleep = 100, int IntervalScan = 3)
        {
            _bitIn = BitIn;
            _bitOutOk = BitOut_OK;
            _bitOutNg = BitOut_NG;
            _bitOutProductMissing = BitOut_NOPRODUCT;

            _durationSleep = DurationSleep;

            _triggeredState = triggerType == TriggerType.FallingEdge ? 0 : 1;
            _previousState = triggerType == TriggerType.FallingEdge ? 1 : 0;

            _timer.Interval = TimeSpan.FromMilliseconds(IntervalScan);
            _timer.Tick += TimerOnTick;
        }

        public event EventHandler Triggered;

        public async Task ReportPLCAsync(RunResult runResult)
        {
            var outBit = GetOutBit(runResult);
            await ReportBackAsync(outBit);
        }

        public void ReportPLC(RunResult runResult)
        {
            var outBit = GetOutBit(runResult);
            ReportBack(outBit);
        }


        public void StartListening()
        {
            _timer.Start();
        }

        /// <summary>
        ///     Init LeiSai IO card
        /// </summary>
        /// <returns>true equals success</returns>
        public static bool Init()
        {
            return IOC0640.ioc_board_init() > 0;
        }

        private void OnTriggered(EventArgs e)
        {
            var handler = Triggered;
            handler?.Invoke(this, e);
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            var currentState = IOC0640.ioc_read_inbit(0, _bitIn);
            if (currentState == _previousState) return;
            _previousState = currentState;
            if (currentState == _triggeredState) OnTriggered(EventArgs.Empty);
        }

        private async Task ReportBackAsync(ushort outBit)
        {
            IOC0640.ioc_write_outbit(0, outBit, 0);
            await Task.Delay(_durationSleep);
            IOC0640.ioc_write_outbit(0, outBit, 1);
        }

        private ushort GetOutBit(RunResult runResult)
        {
            return runResult == RunResult.OK ? _bitOutOk :
                runResult == RunResult.NG ? _bitOutNg : _bitOutProductMissing;
        }


        private void ReportBack(ushort outBit)
        {
            IOC0640.ioc_write_outbit(0, outBit, 0);
            Thread.Sleep(_durationSleep);
            IOC0640.ioc_write_outbit(0, outBit, 1);
        }

        public static void Disconnet()
        {
            IOC0640.ioc_board_close();
        }
    }

    internal class IOC0640
    {
        public delegate uint IOC0640_OPERATE(IntPtr operate_data);

        [DllImport("IOC0640.dll", EntryPoint = "ioc_board_init", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ioc_board_init();

        [DllImport("IOC0640.dll", EntryPoint = "ioc_board_close", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void ioc_board_close();

        [DllImport("IOC0640.dll", EntryPoint = "ioc_read_inbit", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ioc_read_inbit(ushort cardno, ushort bitno);

        [DllImport("IOC0640.dll", EntryPoint = "ioc_read_outbit", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ioc_read_outbit(ushort cardno, ushort bitno);

        [DllImport("IOC0640.dll", EntryPoint = "ioc_write_outbit", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern uint ioc_write_outbit(ushort cardno, ushort bitno, int on_off);

        [DllImport("IOC0640.dll", EntryPoint = "ioc_read_inport", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ioc_read_inport(ushort cardno, ushort m_PortNo);

        [DllImport("IOC0640.dll", EntryPoint = "ioc_read_outport", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ioc_read_outport(ushort cardno, ushort m_PortNo);

        [DllImport("IOC0640.dll", EntryPoint = "ioc_write_outport", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern uint ioc_write_outport(ushort cardno, ushort m_PortNo, uint port_value);

        [DllImport("IOC0640.dll", EntryPoint = "ioc_int_enable", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern uint ioc_int_enable(ushort cardno, IOC0640_OPERATE funcIntHandler, IntPtr operate_data);

        [DllImport("IOC0640.dll", EntryPoint = "ioc_int_disable", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern uint ioc_int_disable(ushort cardno);

        [DllImport("IOC0640.dll", EntryPoint = "ioc_config_intbitmode", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern uint ioc_config_intbitmode(ushort cardno, ushort bitno, ushort enable, ushort logic);

        [DllImport("IOC0640.dll", EntryPoint = "ioc_config_intbitmode", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern uint ioc_config_intbitmode(ushort cardno, ushort bitno, ushort[] enable, ushort[] logic);

        [DllImport("IOC0640.dll", EntryPoint = "ioc_read_intbitstatus", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ioc_read_intbitstatus(ushort cardno, ushort bitno);

        [DllImport("IOC0640.dll", EntryPoint = "ioc_config_intporten", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern uint ioc_config_intporten(ushort cardno, ushort m_PortNo, uint port_en);

        [DllImport("IOC0640.dll", EntryPoint = "ioc_config_intportlogic", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern uint ioc_config_intportlogic(ushort cardno, ushort m_PortNo, uint port_logic);

        [DllImport("IOC0640.dll", EntryPoint = "ioc_read_intportmode", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern uint ioc_read_intportmode(ushort cardno, ushort m_PortNo, uint[] enable, uint[] logic);

        [DllImport("IOC0640.dll", EntryPoint = "ioc_read_intportstatus", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ioc_read_intportstatus(ushort cardno, ushort m_PortNo);

        [DllImport("IOC0640.dll", EntryPoint = "ioc_set_filter", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern uint ioc_set_filter(ushort cardno, double filter);
    }
}