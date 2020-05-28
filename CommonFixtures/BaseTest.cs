using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture;
using FakeItEasy;

namespace CommonFixtures
{
    /// <summary>
    /// Contains basic mocking and stubbing methods to supply standard DSL for all tests.  
    /// </summary>
    public abstract class BaseTest : IDisposable
    {
        public static T Mock<T>(bool strict = false) where T : class
        {
            return A.Fake<T>(cfg =>
            {
                if (strict)
                {
                    cfg.Strict();
                }
            });
        }

        protected static void Verify(Expression<Action> callExpression, int numberOfTimes = 1)
            => A.CallTo(callExpression).MustHaveHappened(numberOfTimes, Times.Exactly);

        protected static void VerifyNoAction(Expression<Action> callExpression)
            => A.CallTo(callExpression).DoesNothing();

        protected static void Stub<T>(Expression<Func<T>> callExpression, T stubbingValue)
            => A.CallTo(callExpression).Returns(stubbingValue);

        protected static void StubAsync<T>(Expression<Func<Task<T>>> callExpression, T stubbingValue)
            => A.CallTo(callExpression).Returns(Task.FromResult(stubbingValue));

        protected static T ArgIgnore<T>() => A<T>.Ignored;

        protected static T ArgMatches<T>(Expression<Func<T, bool>> predicate) => A<T>.That.Matches(predicate);

        private Fixture _fixture;

        /// <summary>
        /// Supplies random instance of given type
        /// </summary>
        /// <returns></returns>
        protected T Random<T>()
        {
            if (typeof(T) == typeof(long))
            {
                return (T) Convert.ChangeType(new Random().Next(), typeof(T)); // workaround to avoid autoFixture's similer long values 
            }

            if (_fixture == null)
            {
                _fixture = new Fixture();
                _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                    .ForEach(b => _fixture.Behaviors.Remove(b));
                _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            }

            return _fixture.Create<Generator<T>>().First();
        }

        public virtual void Dispose()
        {
        }
    }
}