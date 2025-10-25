namespace MainUI.Procedure.Test
{
    /// <summary>
    /// 调压试验
    /// </summary>
    public class B11_VoltageSideTest : GeneralBaseTest
    {
        public override async Task<bool> Execute(CancellationToken cancellationToken)
        {
            await base.Execute(cancellationToken);
            try
            {
                // TODO:暂时没有报表
                double MRPressure = 785 /*Read("SET1").ToDouble()*/; //MR路充气值
                double HighPressure = 645 /*Read("SET2").ToDouble()*/; //高压侧压力值
                double HighToPressure = 655 /*Read("SET2").ToDouble()*/; //高压侧压力值

                BCRoadExhaust(true);  // BC电磁阀打开
                VoltageOutput(100);   // 输出电压100V
                VoltageControl(true); // 电压输出开启
                MRInflate(MRPressure);// MR充气

                VoltageControl(false); // 电压输出关闭
                FrmVoltageSide frmVoltage = new()
                {
                    PromptMessage = $"请转动高压侧调整螺钉（下侧）来进行调整压力至指定值({HighPressure}～{HighToPressure})后，点击是提交。"
                };
                VarHelper.ShowDialogWithOverlay(Frm, frmVoltage);

                for (int i = 0; i < 3; i++)
                {
                    VX06(true);
                    Delay(5, "排气时间");
                    VX06(false);
                    Delay(10, "稳压时间");
                    double pressure = PE05();
                    if (pressure >= HighPressure && pressure <= HighToPressure)
                    {
                        break;
                    }
                    else if (i == 2)
                    {
                        throw new("高压侧压力值不在指定范围内，试验失败！");
                    }
                    else
                    {
                        MessageHelper.MessageOK(Frm, $"高压侧压力值不在指定范围内，请转动高压侧调整螺钉（下侧）来进行重新调节！", AntdUI.TType.Error);
                        frmVoltage.PromptMessage = $"请转动高压侧调整螺钉（下侧）来进行调整压力至指定值({HighPressure}～{HighToPressure})后，点击是提交。";
                        VarHelper.ShowDialogWithOverlay(Frm, frmVoltage);
                    }
                }

                // TODO:暂时没有报表
                //Write("", PE05().ToString("f1"));

                // 低压侧试验
                double LowPressure = 435 /*Read("SET2").ToDouble()*/;   // 低压侧压力值
                double LowToPressure = 455 /*Read("SET2").ToDouble()*/; // 低压侧压力值

                VoltageControl(true); // 电压输出开启
                VX06(true);
                Delay(5, "排气时间");
                VX06(false);
                Delay(10, "稳压时间");

                frmVoltage = new()
                {
                    PromptMessage = $"请转动高压侧调整螺钉（上侧）来进行调整压力至指定值({LowPressure}～{LowToPressure})后，点击是提交。"
                };
                VarHelper.ShowDialogWithOverlay(Frm, frmVoltage);

                for (int i = 0; i < 3; i++)
                {
                    VX06(true);
                    Delay(5, "排气时间");
                    VX06(false);
                    Delay(10, "稳压时间");
                    double pressure = PE05();
                    if (pressure >= LowPressure && pressure <= LowToPressure)
                    {
                        break;
                    }
                    else if (i == 2)
                    {
                        throw new("低压侧压力值不在指定范围内，试验失败！");
                    }
                    else
                    {
                        MessageHelper.MessageOK(Frm, $"低压侧压力值不在指定范围内，请转动低压侧调整螺钉（上侧）重新调节！", AntdUI.TType.Error);
                        frmVoltage.PromptMessage = $"请转动高压侧调整螺钉（上侧）来进行调整压力至指定值({LowPressure}～{LowToPressure})后，点击确定提交。";
                        VarHelper.ShowDialogWithOverlay(Frm, frmVoltage);
                    }
                }

                // TODO:暂时没有报表
                //Write("", PE05().ToString("f1"));

                return true;
            }
            catch (Exception ex)
            {
                NlogHelper.Default.Error($"B11_LowVoltageSideTest：{ex.Message}");
                throw new($"B11_LowVoltageSideTest：{ex.Message}");
            }
            finally
            {
                // 试验结束后的清理操作
            }
        }
    }
}
