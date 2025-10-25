namespace MainUI.Procedure.Test
{
    /// <summary>
    /// 低压侧泄漏试验
    /// </summary>
    public class B11_LowLeakTest : GeneralBaseTest
    {
        public override async Task<bool> Execute(CancellationToken cancellationToken)
        {
            await base.Execute(cancellationToken);
            try
            {
                // TODO:暂时没有报表
                double MRPressure = 785 /*Read("SET1").ToDouble()*/; //MR路充气值

                BCRoadExhaust(true);  // BC电磁阀打开
                VoltageOutput(100);   // 输出电压100V
                VoltageControl(true); // 电压输出开启
                MRInflate(MRPressure);// MR充气

                Delay(30, "充气时间");

                VX03(false);
                VX08(false);
                double StartPressure = PE05();
                Delay(30, "稳压时间");
                double StopPressure = PE05();
                double Pressure = StartPressure - StopPressure;

                // TODO:暂无报表
                //Write("", Pressure.ToString("f1"));

                return true;
            }
            catch (Exception ex)
            {
                NlogHelper.Default.Error($"B11_LowLeakTest：{ex.Message}");
                throw new($"B11_LowLeakTest：{ex.Message}");
            }
            finally
            {
                // 试验结束后的清理操作
            }
        }
    }
}
