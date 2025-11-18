namespace MainUI.Procedure.Test
{
    /// <summary>
    /// 通用方法测试类，继承自BaseTest，提供具体的测试实现
    /// </summary>
    public class GeneralBaseTest : BaseTest
    {
        #region Execute 方法重写(确保设置 CurrentCancellationToken)

        /// <summary>
        /// 重写Execute方法,确保设置CurrentCancellationToken
        /// </summary>
        public override Task<bool> Execute(CancellationToken cancellationToken)
        {
            // 调用基类方法设置CurrentCancellationToken
            base.Execute(cancellationToken);

            // 子类可以继续重写此方法
            return Task.FromResult(true);
        }

        #endregion

        #region 全局初始化方法
        /// <summary>
        /// 全局初始化 - 整个测试序列开始前只调用一次
        /// </summary>
        public static void GlobalInit()
        {
            if (_IsTesting)
                return; // 已经初始化过，直接返回

            try
            {
                TestStatus(true);

                Write("TuHao", VarHelper.TestViewModel.DrawingNo);  // 图号
                Write("XH", VarHelper.TestViewModel.ModelName);     // 型号
                Write("SYLB", VarHelper.TestViewModel.ModelTypeName);   // 试验类别
                Write("SYSJ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));   // 试验时间
                Write("CheHao", ""); // 车号
                Write("ZZBH", VarHelper.TestViewModel.MakeNumber);   // 制造编号
                Write("XLZM", "");   // 修理者名
                Write("ShiDu", "");  // 湿度
                Write("ZZS", "");    // 制造商
                Write("SYY", NewUsers.NewUserInfo.Username);    // 试验员
                Write("BeiZhu", VarHelper.TestViewModel.Remarks);    // 备注
                
                // 试验前排空所有气压
                SetSolenoidValueS(0.0, true);
                SetCurrentOutput(0); // 电流给0
                VoltageOutput(0);    // 电压给0
                VoltageControl(false); // 电压输出关闭
                Thread.Sleep(10000); // 等待10秒确保排空完成
                SetSolenoidValueS(0.0, false);

                NlogHelper.Default.Info("全局初始化完成");
            }
            catch (Exception ex)
            {
                _IsTesting = false;
                NlogHelper.Default.Error("全局初始化失败", ex);
                throw;
            }
        }

        /// <summary>
        /// 全局清理 - 测试序列结束后调用
        /// </summary>
        public static void GlobalCleanup()
        {
            try
            {
                // 在这里添加你的全局清理逻辑
                // 试验结束排空所有气压
                SetSolenoidValueS(0.0, true);
                SetCurrentOutput(0); // 电流给0
                VoltageOutput(0);    // 电压给0
                VoltageControl(false); // 电压输出关闭
                Thread.Sleep(10000); // 等待10秒确保排空完成
                SetSolenoidValueS(0.0, false);

                TestStatus(false);
                NlogHelper.Default.Info("全局清理完成");
            }
            catch (Exception ex)
            {
                _IsTesting = false;
                NlogHelper.Default.Error("全局清理失败", ex);
            }
        }

        #endregion

        #region 通用方法编写处
        /// <summary>
        /// 所有电磁阀控制及EP值设置
        /// </summary>
        /// <param name="EPValue">EP阀设定值</param>
        /// <param name="Solenoid">电磁阀设定值</param>
        public static void SetSolenoidValueS(double EPValue, bool Solenoid)
        {
            EP01(EPValue);
            VX01(Solenoid);
            VX02(Solenoid);
            VX03(Solenoid);
            VX04(Solenoid);
            VX05(Solenoid);
            VX06(Solenoid);
            VX07(Solenoid);
            VX08(Solenoid);
            VX09(Solenoid);
            VX10(Solenoid);
            VX11(Solenoid);
            VX12(Solenoid);
        }

        /// <summary>
        /// 读取报表单元格
        /// </summary>
        /// <param name="Cell">单元格名称</param>
        /// <returns></returns>
        public string Read(string Cell)
        {
            return Report.Read(Cell).ToString();
        }

        /// <summary>
        /// 报表写入单元格
        /// </summary>
        /// <param name="CellName">单元格名称</param>
        /// <param name="CellValue">需要写入的值</param>
        public static void Write(string CellName, string CellValue)
        {
            Report.Write(CellName, CellValue);
        }

        /// <summary>
        /// MR路进行充气
        /// </summary>
        public void MRInflate(double Pressure)
        {
            EP01(Pressure);
            VX01(true);
            VX02(true);
            VX03(true);
            Delay(10, "MR充气");
        }

        /// <summary>
        /// MR路控制
        /// </summary>
        public void MRInflate(double Pressure, bool value)
        {
            EP01(Pressure);
            VX01(value);
            VX02(value);
            VX03(value);
        }

        /// <summary>
        /// BC路电磁阀控制
        /// </summary>
        public void BCRoadExhaust(bool value)
        {
            VX08(value);
            VX09(value);
        }

        /// <summary>
        /// E路电磁阀控制
        /// </summary>
        /// <param name="Value">操作电磁阀</param>
        public void ERoadExhaust(bool Value)
        {
            VX11(Value);
            VX12(Value);
        }

        #region 数字量控制

        /// <summary>
        /// 试验电压输出控制
        /// </summary>
        /// <param name="Value">操作值</param>
        public static void VoltageControl(bool Value)
        {
            //TODO:由于未出点表，暂定DO20控制电压输出
            // 20251118,将DO20改为DO4
            OPCHelper.DOgrp[4] = Value;
        }
        #endregion

        #region 模拟量输出控制
        /// <summary>
        /// EP阀控制
        /// </summary>
        /// <param name="Pressure"></param>
        public static void EP01(double Pressure)
        {
            OPCHelper.AOgrp.CA00 = Pressure;
        }

        /// <summary>
        /// 输出电流
        /// </summary>
        /// <param name="Current">电流值</param>
        public static void SetCurrentOutput(double Current)
        {
            //TODO:由于未出点表，暂定CA01输出电流
            // 电流不得高于750mA以上。如果高于750mA以上，则试验品有可能被烧毁。

            // 20251118,将CA01改为CA02
            if (Current >= 750) Current = 750;
            OPCHelper.AOgrp.CA02 = Current;
        }

        /// <summary>
        /// 获取当前输出电流值
        /// </summary>
        /// <returns></returns>
        public static double GetCurrentOutput()
        {
            //TODO:由于未出点表，暂定CA01输出电流
            return OPCHelper.AOgrp.CA01;
        }

        /// <summary>
        /// 输出电压
        /// </summary>
        /// <param name="Voltage">电压值</param>
        public static void VoltageOutput(double Voltage)
        {
            //TODO:由于未出点表，暂定CA02输出电压，好像还少了DO点 确认输出点
            // 20251118,将CA02改为CA01
            OPCHelper.AOgrp.CA01 = Voltage;
            Thread.Sleep(1000); // 等待1000ms确保电压输出稳定
            VoltageControl(true);
        }
        #endregion

        #region 电磁阀控制
        /// <summary>
        /// VX01电磁阀控制
        /// </summary>
        /// <param name="value">打开或者关闭</param>
        public static void VX01(bool value)
        {
            OPCHelper.DOgrp[8] = value;
        }

        /// <summary>
        /// VX02电磁阀控制
        /// </summary>
        /// <param name="value">打开或者关闭</param>
        public static void VX02(bool value)
        {
            OPCHelper.DOgrp[9] = value;
        }

        /// <summary>
        /// VX03电磁阀控制
        /// </summary>
        /// <param name="value">打开或者关闭</param>
        public static void VX03(bool value)
        {
            OPCHelper.DOgrp[10] = value;
        }

        /// <summary>
        /// VX04电磁阀控制
        /// </summary>
        /// <param name="value">打开或者关闭</param>
        public static void VX04(bool value)
        {
            OPCHelper.DOgrp[11] = value;
        }

        /// <summary>
        /// VX05电磁阀控制
        /// </summary>
        /// <param name="value">打开或者关闭</param>
        public static void VX05(bool value)
        {
            OPCHelper.DOgrp[12] = value;
        }

        /// <summary>
        /// VX06电磁阀控制
        /// </summary>
        /// <param name="value">打开或者关闭</param>
        public static void VX06(bool value)
        {
            OPCHelper.DOgrp[13] = value;
        }

        /// <summary>
        /// VX07电磁阀控制
        /// </summary>
        /// <param name="value">打开或者关闭</param>
        public static void VX07(bool value)
        {
            OPCHelper.DOgrp[14] = value;
        }

        /// <summary>
        /// VX08电磁阀控制
        /// </summary>
        /// <param name="value">打开或者关闭</param>
        public static void VX08(bool value)
        {
            OPCHelper.DOgrp[15] = value;
        }

        /// <summary>
        /// VX09电磁阀控制
        /// </summary>
        /// <param name="value">打开或者关闭</param>
        public static void VX09(bool value)
        {
            OPCHelper.DOgrp[16] = value;
        }

        /// <summary>
        /// VX10电磁阀控制
        /// </summary>
        /// <param name="value">打开或者关闭</param>
        public static void VX10(bool value)
        {
            OPCHelper.DOgrp[17] = value;
        }

        /// <summary>
        /// VX11电磁阀控制
        /// </summary>
        /// <param name="value">打开或者关闭</param>
        public static void VX11(bool value)
        {
            OPCHelper.DOgrp[18] = value;
        }

        /// <summary>
        /// VX12电磁阀控制
        /// </summary>
        /// <param name="value">打开或者关闭</param>
        public static void VX12(bool value)
        {
            OPCHelper.DOgrp[19] = value;
        }
        #endregion

        #region 获取传感器值
        /// <summary>
        /// 获取电流值
        /// </summary>
        /// <returns></returns>
        public double GetCurrent()
        {
            //TODO:由于未出点表，暂定AI09为电流传感器
            return OPCHelper.AIgrp[9];
        }

        /// <summary>
        /// 获取PE01传感器值
        /// </summary>
        /// <param name="decimalPlace">保留小数位，默认保留1位</param>
        /// <returns></returns>
        public double PE01(int decimalPlace = 1)
        {
            return OPCHelper.AIgrp[0].ToDouble(decimalPlace);
        }

        /// <summary>
        /// 获取PE02传感器值
        /// </summary>
        /// <param name="decimalPlace">保留小数位，默认保留1位</param>
        /// <returns></returns>
        public double PE02(int decimalPlace = 1)
        {
            return OPCHelper.AIgrp[1].ToDouble(decimalPlace);
        }

        /// <summary>
        /// 获取PE03传感器值
        /// </summary>
        /// <param name="decimalPlace">保留小数位，默认保留1位</param>
        /// <returns></returns>
        public double PE03(int decimalPlace = 1)
        {
            return OPCHelper.AIgrp[2].ToDouble(decimalPlace);
        }

        /// <summary>
        /// 获取PE04传感器值
        /// </summary>
        /// <param name="decimalPlace">保留小数位，默认保留1位</param>
        /// <returns></returns>
        public double PE04(int decimalPlace = 1)
        {
            return OPCHelper.AIgrp[3].ToDouble(decimalPlace);
        }

        /// <summary>
        /// 获取PE05传感器值
        /// </summary>
        /// <param name="decimalPlace">保留小数位，默认保留1位</param>
        /// <returns></returns>
        public double PE05(int decimalPlace = 1)
        {
            return OPCHelper.AIgrp[4].ToDouble(decimalPlace);
        }

        /// <summary>
        /// 获取PE06传感器值
        /// </summary>
        /// <param name="decimalPlace">保留小数位，默认保留1位</param>
        /// <returns></returns>
        public double PE06(int decimalPlace = 1)
        {
            return OPCHelper.AIgrp[5].ToDouble(decimalPlace);
        }

        /// <summary>
        /// 获取PE07传感器值
        /// </summary>
        /// <param name="decimalPlace">保留小数位，默认保留1位</param>
        /// <returns></returns>
        public double PE07(int decimalPlace = 1)
        {
            return OPCHelper.AIgrp[6].ToDouble(decimalPlace);
        }

        /// <summary>
        /// 获取PE08传感器值
        /// </summary>
        /// <param name="decimalPlace">保留小数位，默认保留1位</param>
        /// <returns></returns>
        public double PE08(int decimalPlace = 1)
        {
            return OPCHelper.AIgrp[7].ToDouble(decimalPlace);
        }

        /// <summary>
        /// 获取PE09传感器值
        /// </summary>
        /// <param name="decimalPlace">保留小数位，默认保留1位</param>
        /// <returns></returns>
        public double PE09(int decimalPlace = 1)
        {
            return OPCHelper.AIgrp[8].ToDouble(decimalPlace);
        }

        #endregion

        #endregion
    }
}
