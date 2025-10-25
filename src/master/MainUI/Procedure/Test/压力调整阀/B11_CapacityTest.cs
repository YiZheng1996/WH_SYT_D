using System.Diagnostics;

namespace MainUI.Procedure.Test
{
    /// <summary>
    /// 供给容量试验
    /// </summary>
    public class B11_CapacityTest : GeneralBaseTest
    {
        public override async Task<bool> Execute(CancellationToken cancellationToken)
        {
            await base.Execute(cancellationToken);
            try
            {
                // 供给容量
                // TODO:暂时没有报表
                double MRPressure = 785 /*Read("SET1").ToDouble()*/; //MR路充气值
                double UpkPa = 490; /*Read("").ToDouble()*/  //上升至气压标准值
                double DownkPa = 235; /*Read("").ToDouble()*/  //上升至气压标准值
                VoltageOutput(100);   // 输出电压100V

                VX01(false);
                VX02(false);
                VX09(false);

                VX03(true);
                VX08(true);
                VX06(true);
                Delay(20, 100, () => PE06() <= 10); //排空气压
                VoltageControl(false); // 电压输出关闭
                VX06(false);
                EP01(MRPressure);
                VX01(true);
                VX02(true);
                VX09(true);

                Stopwatch sw = Stopwatch.StartNew();
                Delay(10, 100, () => PE06() >= UpkPa); //排空气压
                sw.Stop();
                // 充气到目标值时间，Ticks以获得更高的精度
                var InflateTime = sw.ElapsedTicks / (double)Stopwatch.Frequency; 
                //TODO:暂无报表
                Write("", InflateTime.ToString("F1")); // 保存时间，单位：秒
                Delay(10);

                // 排气容量
                sw = Stopwatch.StartNew();
                VoltageControl(true); // 电压输出关闭
                Delay(10, 100, () => PE06() <= DownkPa); //排空气压
                sw.Stop();
                //TODO:暂无报表
                var ExhaustTime = sw.ElapsedTicks / (double)Stopwatch.Frequency;
                Write("", ExhaustTime.ToString("F1")); // 保存时间，单位：秒


                return true;
            }
            catch (Exception ex)
            {
                NlogHelper.Default.Error($"B11_SupplyCapacityTest：{ex.Message}");
                throw new($"B11_SupplyCapacityTest：{ex.Message}");
            }
            finally
            {
                // 试验结束后的清理操作
            }
        }
    }
}
