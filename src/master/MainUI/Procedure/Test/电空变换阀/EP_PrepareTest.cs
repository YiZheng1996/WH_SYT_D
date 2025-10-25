namespace MainUI.Procedure.Test
{
    /// <summary>
    /// 试验准备
    /// </summary>
    public class EP_PrepareTest : GeneralBaseTest
    {
        public override async Task<bool> Execute(CancellationToken cancellationToken)
        {
            await base.Execute(cancellationToken);
            try
            {
                // 带图标类型的确认
                if (!ShowConfirmDialog("请确认EPLA电空变换阀在工装上夹紧?", AntdUI.TType.Warn))
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                NlogHelper.Default.Error($"EP_TestPrepare：{ex.Message}");
                throw new($"EP_TestPrepare：{ex.Message}");
            }
            finally
            {
                // 试验结束后的清理操作
            }
        }
    }
}
