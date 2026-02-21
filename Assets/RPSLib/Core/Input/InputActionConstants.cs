/* Author:		Matthew Vale
 * Role:		Lead Game Developer
 * Company:		Red Phoenix Studios
*/
namespace RPSCore {

    public static class InputActionConstants {

        // ----- Common Input ----- //
        // ------------------------ //
        /// <summary>Common action to poll the mouse cursor screen position.</summary>
        public static string MousePosition = "MousePosition";

        /// <summary>Common action to poll the left mouse mouse.</summary>
        public static string LMB = "LMB"; // LMB

        /// <summary>Common action to poll the right mouse mouse.</summary>
        public static string RMB = "RMB"; // LMB

        /// <summary>Common action to poll the mouse scrollwheel.</summary>
        public static string MouseScrollWheel = "MouseScrollWheel";

        /// <summary>Common action to poll the mouse scrollwheel button.</summary>
        public static string MouseScrollButton = "MouseScrollButton";

        /// <summary>Common action to go "Back" or "Escape".</summary>
        public static string Escape = "Escape"; // Escape

        /// <summary>Common action for Tab, perhaps for inventory or a map.</summary>
        public static string Tab = "Tab"; // Tab

        /// <summary>Common action for Journey (quests).</summary>
        public static string OpenQuests = "OpenQuests"; // J

        /// <summary>Common action to open the Map.</summary>
        public static string OpenMap = "OpenMap"; // M

        /// <summary>Common action for sprinting/boosting.</summary>
        public static string Sprint = "Sprint"; // LShift

        /// <summary>Common action for Spacebar.</summary>
        public static string Space = "Space"; // Spacebar

        /// <summary>Common action for directional movement, WASD, QE for strafing.</summary>
        public static string DirectionalMove = "DirectionalMove";
        public static string StrafeMove = "StrafeMove";

        /// <summary>Common action for interacting with a prompt.</summary>
        public static string InteractKey = "Interact"; // E

        /// <summary>Common action for opening the player inventory.</summary>
        public static string InventoryKey = "OpenInventory"; // I


        // ----- UI - MiniMap ----- //
        // ------------------------ //
        public static string MiniMapZoom = "MiniMapZoom";
        //public static string MiniMapToggle = "MiniMapToggle";


        // ----- Ship Movement - Combat ----- //
        // ---------------------------------- //
        /// <summary>Specific action for shooting.</summary>
        public static string Shoot = "Shoot"; // LMB, Space

        /// <summary>Specific action for stopping a ship.</summary>
        public static string FullStop = "FullStop"; // LControl

        /// <summary>Specific action for entering/leaving combat mode.</summary>
        public static string ToggleCombatMode = "ToggleCombatMode"; // C


        // ----- Game Specific ----- //
        // ------------------------------------- //
        /// <summary>Specific action for viewing current crew members.</summary>
        public static string ViewCrew = "ViewCrew"; // T
        public static string ViewFactions = "ViewFactions"; // F
        public static string OpenShipStats = "OpenShipStats"; // U

    }

}