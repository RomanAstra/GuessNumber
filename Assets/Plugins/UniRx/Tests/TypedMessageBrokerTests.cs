using System.Collections;
using NUnit.Framework;
using UniRx;
using UnityEngine.TestTools;

namespace Plugins.UniRx.Tests
{
    public class TypedMessageBrokerTests
    {
        [Test]
        public void TypedMessageBrokerTestsSimplePasses()
        {
            var broker = new MessageBroker<ITestMessage>();
            var testMessageOne = new TestMessageOne();
            var testMessageTwo = new TestMessageTwo();
            TestMessageOne actualMessageOne = null;
            TestMessageTwo actualMessageTwo = null;
            broker.Receive<TestMessageOne>().Subscribe(a => actualMessageOne = a);
            broker.Receive<TestMessageTwo>().Subscribe(a => actualMessageTwo = a);

            broker.Publish(testMessageOne);
            
            Assert.AreEqual(testMessageOne, actualMessageOne);
            Assert.IsNull(actualMessageTwo);
            
            broker.Publish(testMessageTwo);
            Assert.AreEqual(testMessageTwo, actualMessageTwo);
        }
    }

    public interface ITestMessage : IMessage
    {
    }

    class TestMessageTwo : ITestMessage
    {
    }

    class TestMessageOne : ITestMessage
    {
    }
}