using events;
using stat;

namespace menu
{
    public class UserChangedEvent : BasicEvent
    {
        public readonly UserStatHolder holder;

        public UserChangedEvent(UserStatHolder holder)
        {
            this.holder = holder;
        }
    }
}