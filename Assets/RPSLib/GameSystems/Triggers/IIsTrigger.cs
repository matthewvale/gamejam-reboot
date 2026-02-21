/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

namespace RPSCore
{

    public interface IIsTrigger
    {
        void ActivateTrigger();

        void DeactivateTrigger();

        bool IsActive { get; set; }
    }

}