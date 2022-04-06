using Next2.Enums;

namespace Next2.Helpers
{
    public class SwitchingPaymentStepMessage
    {
        public SwitchingPaymentStepMessage(EPaymentPageSteps step)
        {
            Step = step;
        }

        #region -- Public properties --

        public EPaymentPageSteps Step { get; }

        #endregion
    }
}
