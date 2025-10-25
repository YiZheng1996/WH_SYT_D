namespace MainUI.Procedure.Test
{
    /// <summary>
    /// 供给阀泄漏试验
    /// </summary>
    public class EP_SupplyValveTest : GeneralBaseTest
    {
        public override async Task<bool> Execute(CancellationToken cancellationToken)
        {
            await base.Execute(cancellationToken);
            try
            {
                double MRPressure = Read("SET1").ToDouble(); //MR路充气值
                int StabilivoltTime = Read("SET2").ToInt(); // 稳压时间
                int TestTime = Read("SET3").ToInt(); // 测试时间

                MRInflate(MRPressure);// MR充气

                // 排空后端（EX缓解C、E路）压力
                VX05(true); //TCX(缓解C) 持续信号
                ERoadExhaust(true); // E路排气

                Delay(StabilivoltTime, "稳压时间"); // 稳压时间

                Write("ks1", PE03().ToString()); // 开始压力

                Delay(TestTime, "测试时间"); // 测试时间

                Write("js1", PE03().ToString()); // 结束压力

                VX05(false);
                ERoadExhaust(false); // E路排气关闭

                return true;
            }
            catch (Exception ex)
            {
                NlogHelper.Default.Error($"EP_SupplyValveTest：{ex.Message}");
                throw new($"EP_SupplyValveTest：{ex.Message}");
            }
            finally
            {
                // 试验结束后的清理操作
            }
        }
    }
}
