using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.UnitTests.Utilities.Faker;

internal static partial class DF
{
    public static InstanceOrderCancelledEvent FakeOrderCancelledEvent(
       Action<InstanceOrderCancelledEvent>? modifier = null)
    {
        var cancelEv = AppDbJsonField.Create(() => new InstanceOrderCancelledEvent
        {
            Reason = "test",
            RefundedAmount = 0
        });

        modifier?.Invoke(cancelEv);

        return cancelEv;
    }

    public static InstanceOrderSentEvent FakeSentEvent(
        Action<InstanceOrderSentEvent>? modifier = null)
    {
        var sentEv = AppDbJsonField.Create(() => new InstanceOrderSentEvent());
        modifier?.Invoke(sentEv);

        return sentEv;
    }

    public static InstanceOrderDeliveredEvent FakeDeliveredEvent(
        Action<InstanceOrderDeliveredEvent>? modifier = null)
    {
        var deliverEv = AppDbJsonField.Create(() => new InstanceOrderDeliveredEvent());
        modifier?.Invoke(deliverEv);

        return deliverEv;
    }
}
