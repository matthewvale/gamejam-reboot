/* Author:		Matthew Vale
 * Role:		Lead Game Developer
 * Company:		Red Phoenix Studios
*/
namespace RPSCore {

    public class GlobalEventEntry {

        public EventConstants.EventType Type { get; private set; }
        public string EntryText { get; private set; }

        public GlobalEventEntry(EventConstants.EventType type, string entryText) {
            Type = type;
            EntryText = entryText;
        }

    }

}