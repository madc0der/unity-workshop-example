using events;

namespace game
{
    public struct LastScoreUpdatedEvent : BasicEvent
    {
        public readonly int score;

        public LastScoreUpdatedEvent(int score)
        {
            this.score = score;
        }
    }
}