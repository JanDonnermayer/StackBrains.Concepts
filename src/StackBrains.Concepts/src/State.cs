using System.Collections.Immutable;

namespace FrogStack.Brightpearl
{
    public class State<TValue, TError>
    {
        public State(TValue value) : this(value, ImmutableList<TError>.Empty) { }

        private State(TValue value, IImmutableList<TError> errors)
        {
            Value = value;
            Errors = errors;
        }

        public TValue Value { get; }

        public IImmutableList<TError> Errors { get; }

        public State<TValue, TError> WithErrors(params TError[] errors) =>
            new(
                value: Value,
                errors: Errors.AddRange(errors)
            );

        public State<TValue, TError> WithValue(TValue value) =>
            new(
                value: value,
                errors: Errors
            );
    }

}